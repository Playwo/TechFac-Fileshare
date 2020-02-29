using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fileshare.Models;
using Fileshare.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fileshare
{
    public class UploadPreviewModel : PageModel
    {
        private readonly UploaderContext DbContext;
        private readonly UploadDataService DataService;

        public Upload Upload { get; private set; }
        public byte[] Data { get; private set; }
        public string DataString => Encoding.ASCII.GetString(Data);

        public UploadPreviewModel(UploaderContext dbContext, UploadDataService dataService)
        {
            DbContext = dbContext;
            DataService = dataService;
        }

        public async Task<ActionResult> OnGetAsync(string path)
        {
            Upload = await DbContext.Uploads.Where(x => x.Id.ToString() == path || x.Filename == path)
                                            .FirstOrDefaultAsync();

            if (Upload == null)
            {
                return NotFound();
            }

            Data = await DataService.LoadUploadDataAsync(Upload);

            return Upload == null 
                ? NotFound() 
                : (ActionResult) Page();
        }
    }
}