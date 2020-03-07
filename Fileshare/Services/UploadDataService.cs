using System;
using System.IO;
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
        private readonly object IdGeneratorLock;
        private DateTimeOffset LastIdTime;

        public UploadDataService(IConfiguration configuration)
        {
            Configuration = configuration;

            IdGeneratorLock = new object();
            LastIdTime = DateTimeOffset.UtcNow;
        }

        public override ValueTask RunAsync()
        {
            Directory.CreateDirectory(GetBasePath());
            return base.RunAsync();
        }

        public async Task StoreUploadDataAsync(Upload upload, Stream content)
        {
            string path = GetFilePath(upload);
            using var fileStream = File.Create(path);
            await content.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
        }
        public async Task<byte[]> LoadUploadDataAsync(Upload upload)
        {
            string path = GetFilePath(upload);
            return await File.ReadAllBytesAsync(path);
        }
        public void DeleteUploadData(Upload upload)
        {
            string path = GetFilePath(upload);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public string GetFilePath(Upload upload)
            => Path.Combine(GetBasePath(), $"{upload.Id}.data");

        public string GetBasePath()
            => Path.IsPathRooted(Configuration.GetStorageDir())
                ? Configuration.GetStorageDir()
                : Path.Combine(Environment.CurrentDirectory, Configuration.GetStorageDir());

        //ToDo: Add shorter filename
        public string GetNextFileName(string contentType)
        {
            string extension = MimeTypeMap.GetExtension(contentType, false);

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".other";
            }

            lock (IdGeneratorLock)
            {
                var time = DateTimeOffset.UtcNow;

                if ((time - LastIdTime).TotalMilliseconds < 2)
                {
                    time = LastIdTime.AddMilliseconds(2);
                }

                LastIdTime = time;

                return $"{time.Year}{time.Month}{time.Day}{time.Hour}{time.Minute}{time.Second}{time.Millisecond}{extension}";
            }
        }
    }
}
