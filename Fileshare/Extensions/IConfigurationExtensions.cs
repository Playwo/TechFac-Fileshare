using System;
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

        public static bool TryGetWebhookUrl(this IConfiguration configuration, out Uri webHookUrl)
        {
            if (configuration.GetValue<bool>("EnableWebhook"))
            {
                string url = configuration.GetValue<string>("WebhookUrl");

                return Uri.TryCreate(url, UriKind.Absolute, out webHookUrl);
            }

            webHookUrl = null;
            return false;
        }
    }
}
