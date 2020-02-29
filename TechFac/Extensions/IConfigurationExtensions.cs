using Microsoft.Extensions.Configuration;

namespace Fileshare.Extensions
{
    public static partial class Extensions
    {
        public static string GetApiKey(this IConfiguration configuration)
            => configuration.GetValue<string>("ApiKey");

        public static string GetStorageDir(this IConfiguration configuration)
            => configuration.GetValue<string>("StorageDir");
    }
}
