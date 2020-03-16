using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Fileshare.Extensions;
using Fileshare.Models;
using Fileshare.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fileshare
{
    public class UploadPreviewModel : PageModel
    {
        private readonly FileshareContext DbContext;
        private readonly UploadDataService DataService;

        public Upload Upload { get; private set; }
        public PreviewOptions PreviewOptions { get; private set; }

        public byte[] Data { get; private set; }
        public string DataString => Encoding.ASCII.GetString(Data);

        public UploadPreviewModel(FileshareContext dbContext, UploadDataService dataService)
        {
            DbContext = dbContext;
            DataService = dataService;
        }

        public async Task<ActionResult> OnGetAsync(string path)
        {
            Upload = await DbContext.Uploads.Include(x => x.User)
                                            .ThenInclude(x => x.PreviewOptions)
                                            .Where(x => x.Id.ToString() == path || x.Filename == path)
                                            .FirstOrDefaultAsync();

            if (Upload == null)
            {
                return NotFound();
            }

            PreviewOptions = DbContext.PreviewOptions.Where(x => x.UserId == Upload.UserId)
                                                     .First();

            if (TryGetRedirectionTarget(out string target))
            {
                return Redirect(target);
            }

            if (Upload.ContentType.DoesSupportPreview())
            {
                Data = await DataService.LoadUploadDataAsync(Upload);
            }

            return Page();
        }

        private bool TryGetRedirectionTarget(out string target)
        {
            target = null;

            switch (PreviewOptions.Redirection)
            {
                case RedirectionMode.Never:
                    return false;

                case RedirectionMode.Agents:
                    if (!Request.Headers.TryGetValue("User-Agent", out var agent))
                    {
                        return false;
                    }
                    string userAgent = agent;
                    target = GetRelativeDownloadUrl(directDownload: true);
                    return userAgent.StartsWithAny(StringComparison.OrdinalIgnoreCase, "curl", "wget");

                case RedirectionMode.AlwaysView:
                    target = GetRelativeDownloadUrl(directDownload: false);
                    return true;

                case RedirectionMode.AlwaysDownload:
                    target = GetRelativeDownloadUrl(directDownload: true);
                    return true;

                default:
                    return false;
            }
        }


        public string GetRelativeDownloadUrl(bool directDownload = false)
        {
            string url = $"/upload/data/get/{Upload.Id}";

            if (directDownload)
            {
                url += "/1";
            }

            return url;
        }
    }
}