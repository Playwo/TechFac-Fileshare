using System;
using System.Linq;
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

            return Page();
        }

        public async Task<string> LoadContentAsTextAsync()
        {
            var data = await DataService.LoadUploadDataAsync(Upload);
            return Encoding.ASCII.GetString(data);
        }

        private bool TryGetRedirectionTarget(out string target)
        {
            target = null;

            if (PreviewOptions.RedirectAgents)
            {
                if (Request.Headers.TryGetValue("User-Agent", out var agent))
                {
                    string userAgent = agent;

                    if (userAgent.StartsWithAny(StringComparison.OrdinalIgnoreCase, "wget", "curl"))
                    {
                        target = GetRelativeDownloadUrl(directDownload: true);
                        return true;
                    }
                }
            }

            var category = Upload.ContentType.GetContentCategory();

            if (category != ContentCategory.None)
            {
                if(PreviewOptions.RedirectCategories.HasFlag(category))
                {
                    target = GetRelativeDownloadUrl(directDownload: false);
                    return true;
                }
            }

            return false;
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