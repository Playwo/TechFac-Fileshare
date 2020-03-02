using Microsoft.Extensions.Configuration;

namespace Fileshare.Extensions
{
    public static partial class Extensions
    {
        public static string GetApiKey(this IConfiguration configuration)
            => configuration.GetValue<string>("ApiKey");

        public static string GetStorageDir(this IConfiguration configuration)
            => configuration.GetValue<string>("FileStoragePath");

        public static string GetConnectionString(this IConfiguration configuration)
            => configuration.GetValue<string>("ConnectionString");

        public static string GetBaseUrl(this IConfiguration configuration)
            => configuration.GetValue<string>("BaseUrl");
    }
}
