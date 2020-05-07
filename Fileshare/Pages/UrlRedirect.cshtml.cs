using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fileshare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fileshare.Pages
{
    public class UrlRedirectModel : PageModel
    {
        private readonly WebShareContext DbContext;

        public UrlRedirectModel(WebShareContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<ActionResult> OnGetAsync(string name)
        {
            var shortUrl = await DbContext.ShortUrls.Include(x => x.Target)
                                          .Where(x => x.Name == name)
                                          .FirstOrDefaultAsync();

            if (shortUrl == null)
            {
                return NotFound();
            }

            shortUrl.UseCount++;

            await DbContext.SaveChangesAsync();
            return Redirect(shortUrl.Target.TargetUrl);
        }
    }
}
