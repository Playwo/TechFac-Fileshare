using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fileshare.Extensions;
using Fileshare.Models;
using Fileshare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using MimeTypes;

namespace Fileshare.Controllers
{
    [Route("upload")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly FileshareContext DbContext;
        private readonly UploadDataService DataService;
        private readonly WebhookService WebhookService;

        public UploadController(FileshareContext dbContext, UploadDataService dataService, WebhookService webhookService)
        {
            DbContext = dbContext;
            DataService = dataService;
            WebhookService = webhookService;
        }

        #region GetUpload
        //upload/info/get/{Id}
        [HttpGet("info/get/{uploadId}")]
        public async Task<ActionResult<Upload>> GetUploadAsync([FromRoute]Guid uploadId)
        {
            var upload = await DbContext.Uploads.Where(x => x.Id == uploadId)
                                                .FirstOrDefaultAsync();

            return upload == null
                ? (ActionResult<Upload>) NotFound("Invalid uploadId")
                : Ok(upload);
        }

        //upload/info/find/{fileName}
        [HttpGet("info/find/{fileName}")]
        public async Task<ActionResult<Upload>> FindUploadAsync([FromRoute]string fileName)
        {
            var upload = await DbContext.Uploads.Where(x => x.Name == fileName)
                                                .FirstOrDefaultAsync();

            return upload == null
                ? (ActionResult<Upload>) NotFound("Invalid uploadId")
                : Ok(upload);
        }
        #endregion
        #region GetData
        //upload/data/get/{Id}
        [HttpGet("data/get/{uploadId}/{dl?}")]
        public async Task<ActionResult> GetUploadDataAsync([FromRoute]Guid uploadId, [FromRoute] int dl = 0)
        {
            var upload = await DbContext.Uploads.Include(x => x.LocalFile)
                                                .Where(x => x.Id == uploadId)
                                                .FirstOrDefaultAsync();

            if (upload == null)
            {
                return NotFound("Invalid uploadId");
            }

            AddContentDispositionHeader(upload, dl == 1);

            byte[] uploadData = await DataService.LoadUploadDataAsync(upload.LocalFile);
            return File(uploadData, upload.ContentType);
        }

        //upload/data/find/{fileName}
        [HttpGet("data/find/{fileName}/{dl?}")]
        public async Task<ActionResult> FindUploadDataAsync([FromRoute]string fileName, [FromRoute] int dl = 0)
        {
            var upload = await DbContext.Uploads.Include(x => x.LocalFile)
                                                .Where(x => x.Name == fileName)
                                                .FirstOrDefaultAsync();

            if (upload == null)
            {
                return NotFound("Invalid fileName");
            }

            AddContentDispositionHeader(upload, dl == 1);

            byte[] uploadData = await DataService.LoadUploadDataAsync(upload.LocalFile);
            return File(uploadData, upload.ContentType);
        }

        private void AddContentDispositionHeader(Upload upload, bool isDirectDownload)
        {
            string type = isDirectDownload
                ? "attachment"
                : "inline";

            var cd = new ContentDispositionHeaderValue(type)
            {
                FileName = upload.Name,
                CreationDate = upload.CreatedAt,
            };
            Response.Headers.Add(HeaderNames.ContentDisposition, cd.ToString());
        }
        #endregion
        #region ReceiveUpload
        //upload/send
        [HttpPost("send")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [RequestSizeLimit(50 * 1024 * 1024)]
        public async Task<ActionResult<Upload>> ReceiveUploadAsync([FromHeader]string filename = null, [FromHeader]bool generateName = false)
        {
            if (string.IsNullOrWhiteSpace(filename) && string.IsNullOrWhiteSpace(Request.ContentType))
            {
                return BadRequest();
            }

            string extension = null;
            string name = null;

            if (string.IsNullOrWhiteSpace(filename))
            {
                if (!TryGetExtension(Request.ContentType, out extension))
                {
                    return BadRequest();
                }

                name = DataService.GetNextFileName();
            }
            else
            {
                if (!TryGetContentType(filename, out string ctype, out name, out extension))
                {
                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(Request.ContentType))
                {
                    Request.ContentType = ctype;
                }

                bool nameUsed = await DbContext.Uploads.AnyAsync(x => x.Name == name);

                if (nameUsed)
                {
                    if (!generateName)
                    {
                        return Conflict("The fileName is already being used!");
                    }

                    name = DataService.GetNextFileName();
                }
            }

            var content = new MemoryStream();
            await Request.Body.CopyToAsync(content);

            string checksum = content.GenerateChecksum();
            var localFile = await DbContext.LocalFiles.Where(x => x.Checksum == checksum)
                                                      .FirstOrDefaultAsync();

            if (localFile == null)
            {
                localFile = await DataService.StoreUploadDataAsync(content);
                DbContext.Add(localFile);
            }

            var userId = Guid.Parse(User.FindFirstValue("UserId"));
            var upload = new Upload(userId, localFile.Id, name, extension, Request.ContentType);

            DbContext.Add(upload);

            await DbContext.SaveChangesAsync();

            WebhookService.QueueUpload(upload);

            return upload;
        }
        private bool TryGetContentType(string filename, out string contentType, out string name, out string extension)
        {
            string[] parts = filename.Split('.');

            if (parts.Length < 2)
            {
                contentType = null;
                extension = null;
                name = null;
                return false;
            }

            name = string.Concat(parts.Take(parts.Length - 1));
            extension = parts.Last();
            contentType = MimeTypeMap.GetMimeType(extension);

            return true;
        }
        private bool TryGetExtension(string contentType, out string extension)
        {
            extension = MimeTypeMap.GetExtension(contentType, false).Replace(".", "");
            return string.IsNullOrWhiteSpace(extension);
        }

        #endregion
        #region DeleteUpload
        [HttpDelete("deleteId")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> DeleteUploadByIdAsync([FromForm]Guid uploadId)
        {
            var upload = await DbContext.Uploads.Where(x => x.Id == uploadId)
                                                .FirstOrDefaultAsync();

            if (upload == null)
            {
                return NotFound("Invalid uploadId");
            }

            DbContext.Uploads.Remove(upload);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("deleteName")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> DeleteUploadByIdAsync([FromForm]string fileName)
        {
            var upload = await DbContext.Uploads.Where(x => x.Name == fileName)
                                    .FirstOrDefaultAsync();

            if (upload == null)
            {
                return NotFound("Invalid fileName");
            }

            DbContext.Uploads.Remove(upload);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        #endregion
    }
}
