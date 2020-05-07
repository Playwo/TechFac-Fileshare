using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Fileshare.Extensions;
using Fileshare.Models;
using Microsoft.Extensions.Configuration;
using MimeTypes;

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
            Directory.CreateDirectory(GetBasePath());
            return base.RunAsync();
        }

        public async Task<LocalFile> StoreUploadDataAsync(Stream content)
        {
            string checksum = content.GenerateChecksum();
            var localFile = new LocalFile(checksum);

            content.Position = 0;

            string path = GetFilePath(localFile);
            using var fileStream = File.Create(path);
            await content.CopyToAsync(fileStream);
            await fileStream.FlushAsync();

            return localFile;
        }
        public async Task<byte[]> LoadUploadDataAsync(LocalFile file)
        {
            string path = GetFilePath(file);
            return await File.ReadAllBytesAsync(path);
        }
        public void DeleteUploadData(LocalFile file)
        {
            string path = GetFilePath(file);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public string GetFilePath(LocalFile file)
            => Path.Combine(GetBasePath(), $"{file.Id}.data");

        public string GetBasePath()
            => Path.IsPathRooted(Configuration.GetStorageDir())
                ? Configuration.GetStorageDir()
                : Path.Combine(Environment.CurrentDirectory, Configuration.GetStorageDir());

    }
}
