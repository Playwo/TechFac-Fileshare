using System.IO;
using System.Net;
using System.Threading.Tasks;
using Fileshare.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;

namespace Fileshare
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            string configPath = "config.yaml";

            TryCreateConfig(configPath);

            var config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddYamlFile(configPath, false)
                            .AddEnvironmentVariables()
                            .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseUrls(config.GetListenUrl())
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            return host.RunAsync();
        }

        public static void TryCreateConfig(string path)
        {
            if (!File.Exists(path))
            {
                string defaultConfig = new Serializer().Serialize(new
                {
                    ConnectionString = "AddPostgresConnectionString",
                    FileStoragePath = "TempFolder",
                    ApiKey = "YourAPIKey",
                    EnableWebhook = "false",
                    WebhookUrl = "",
                    ListenUrl = "http://localhost:1003"
                });

                File.WriteAllText(path, defaultConfig); ;
            }
        }
    }
}
