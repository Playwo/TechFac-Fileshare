using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Fileshare.Extensions;
using Fileshare.Models;

namespace Fileshare.Services
{
    public class UploadDataService : Service
    {
        private readonly IConfiguration Configuration;

        public UploadDataService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public override ValueTask RunAsync()
        {
            Directory.CreateDirectory(MakeBasePath());
            return base.RunAsync();
        }

        public async Task StoreUploadDataAsync(Upload upload, Stream content)
        {
            string path = MakePath(upload.Id);
            using var fileStream = File.Create(path);
            await content.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
        }
        public async Task<byte[]> LoadUploadDataAsync(Upload upload)
        {
            string path = MakePath(upload.Id);
            return await File.ReadAllBytesAsync(path);      
        }

        public string MakePath(Guid uploadId)
            => Path.Combine(MakeBasePath(), $"{uploadId}.dat");

        public string MakeBasePath()
            => Path.IsPathRooted(Configuration.GetStorageDir())
                ? Configuration.GetStorageDir()
                : Path.Combine(Environment.CurrentDirectory, Configuration.GetStorageDir());
    }
}
