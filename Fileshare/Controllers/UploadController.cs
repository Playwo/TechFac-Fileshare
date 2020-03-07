using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fileshare.Models;
using Fileshare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Net.Http.Headers;

namespace Fileshare.Controllers
{
    [Route("upload")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly UploaderContext DbContext;
        private readonly UploadDataService DataService;

        public UploadController(UploaderContext dbContext, UploadDataService dataService)
        {
            DbContext = dbContext;
            DataService = dataService;
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
            var upload = await DbContext.Uploads.Where(x => x.Filename == fileName)
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
            var upload = await DbContext.Uploads.Where(x => x.Id == uploadId)
                                                .FirstOrDefaultAsync();

            if (upload == null)
            {
                return NotFound("Invalid uploadId");
            }

            AddContentDispositionHeader(upload, dl == 1);

            byte[] uploadData = await DataService.LoadUploadDataAsync(upload);
            return File(uploadData, upload.ContentType);
        }

        //upload/data/find/{fileName}
        [HttpGet("data/find/{fileName}/{dl?}")]
        public async Task<ActionResult> FindUploadDataAsync([FromRoute]string fileName, [FromRoute] int dl = 0)
        {
            var upload = await DbContext.Uploads.Where(x => x.Filename == fileName)
                                                .FirstOrDefaultAsync();

            if (upload == null)
            {
                return NotFound("Invalid fileName");
            }

            AddContentDispositionHeader(upload, dl == 1);

            byte[] uploadData = await DataService.LoadUploadDataAsync(upload);
            return File(uploadData, upload.ContentType);
        }

        private void AddContentDispositionHeader(Upload upload, bool isDirectDownload)
        {
            string type = isDirectDownload
                ? "attachment"
                : "inline";

            var cd = new ContentDispositionHeaderValue(type)
            {
                FileName = upload.Filename,
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
        public async Task<ActionResult<Upload>> ReceiveUploadAsync([FromHeader]string fileName = null)
        {
            var userId = Guid.Parse(User.FindFirstValue("UserId"));

            fileName ??= DataService.GetNextFileName(Request.ContentType);
            var upload = new Upload(userId, fileName, Request.ContentType);

            await DataService.StoreUploadDataAsync(upload, Request.Body);
            DbContext.Add(upload);
            await DbContext.SaveChangesAsync();

            return upload;
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

            DataService.DeleteUploadData(upload);

            DbContext.Uploads.Remove(upload);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("deleteName")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> DeleteUploadByIdAsync([FromForm]string fileName)
        {
            var upload = await DbContext.Uploads.Where(x => x.Filename == fileName)
                                    .FirstOrDefaultAsync();

            if (upload == null)
            {
                return NotFound("Invalid fileName");
            }

            DataService.DeleteUploadData(upload);

            DbContext.Uploads.Remove(upload);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        #endregion
    }
}
