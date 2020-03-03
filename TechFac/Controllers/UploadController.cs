﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fileshare.Models;
using Fileshare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<Upload>> GetUploadAsync([FromRoute]string fileName)
        {
            var upload = await DbContext.Uploads.Where(x => x.Filename == fileName)
                                                .FirstOrDefaultAsync();

            return upload == null
                ? (ActionResult<Upload>) NotFound("Invalid uploadId")
                : Ok(upload);
        }

        //upload/data/get/{Id}
        [HttpGet("data/get/{uploadId}")]
        public async Task<ActionResult> GetUploadDataAsync([FromRoute]Guid uploadId)
        {
            var upload = await DbContext.Uploads.Where(x => x.Id == uploadId)
                                                .FirstOrDefaultAsync();

            return upload == null
                ? (ActionResult) NotFound("Invalid uploadId")
                : PhysicalFile(DataService.MakePath(upload.Id), upload.ContentType, upload.Filename, true);
        }

        //upload/data/find/{fileName}
        [HttpGet("data/find/{fileName}")]
        public async Task<ActionResult> GetUploadDataAsync([FromRoute]string fileName)
        {
            var upload = await DbContext.Uploads.Where(x => x.Filename == fileName)
                                                .FirstOrDefaultAsync();

            return upload == null
                ? (ActionResult) NotFound("Invalid uploadId")
                : PhysicalFile(DataService.MakePath(upload.Id), upload.ContentType, upload.Filename, true);
        }

        //upload/send
        [HttpPost("send")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [RequestSizeLimit(50 * 1024 * 1024)]
        public async Task<ActionResult<Upload>> SendUploadAsync(string fileName = null)

        {
            var userId = Guid.Parse(User.FindFirstValue("UserId"));
            var upload = new Upload(userId, fileName, Request.ContentType);

            await DataService.StoreUploadDataAsync(upload, Request.Body);
            DbContext.Add(upload);
            await DbContext.SaveChangesAsync();

            return upload;
        }
    }
}
