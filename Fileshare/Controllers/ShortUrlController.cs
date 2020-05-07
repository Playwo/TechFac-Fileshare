using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fileshare.Models;
using Fileshare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Fileshare.Controllers
{
    [Route("url")]
    [ApiController]
    public class ShortUrlController : ControllerBase
    {
        private readonly WebShareContext DbContext;
        private readonly UploadDataService DataService;

        public ShortUrlController(WebShareContext dbContext, UploadDataService dataService)
        {
            DbContext = dbContext;
            DataService = dataService;
        }

        [HttpPost("shorten")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<ShortUrl>> ShortenUrlAsync([FromForm] string targetUrl, [FromForm]string name = null, [FromForm]bool generateName = false)
        {
            if (!Uri.TryCreate(targetUrl, UriKind.Absolute, out _))
            {
                return UnprocessableEntity("Invalid url!");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                name = DataService.GetNextFileName();
            }
            else
            {
                bool nameUsed = await DbContext.ShortUrls.AnyAsync(x => x.Name == name);

                if (nameUsed)
                {
                    if (!generateName)
                    {
                        return Conflict("this name is already being used");
                    }

                    name = DataService.GetNextFileName();
                }
            }

            var target = await DbContext.RedirectTargets.Where(x => x.TargetUrl == targetUrl).FirstOrDefaultAsync();

            if (target == null)
            {
                target = new RedirectTarget(targetUrl);
                DbContext.RedirectTargets.Add(target);
            }

            var userId = Guid.Parse(User.FindFirstValue("UserId"));
            var shortUrl = new ShortUrl(userId, target.Id, name);

            DbContext.ShortUrls.Add(shortUrl);
            await DbContext.SaveChangesAsync();

            return Ok(shortUrl);
        }
    }
}
