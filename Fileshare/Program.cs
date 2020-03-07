using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using YamlDotNet.Serialization;

namespace Fileshare
{
    public class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.ListenLocalhost(7777);
                    });

                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    string path = $"{context.HostingEnvironment.EnvironmentName}.config.yaml";

                    TryCreateConfig(path);

                    config.Sources.Clear();
                    config.AddYamlFile(path);
                });

        public static void TryCreateConfig(string path)
        {
            if (!File.Exists(path))
            {
                string defaultConfig = new Serializer().Serialize(new
                {
                    ConnectionString = "AddPostgresConnectionString",
                    FileStoragePath = "TempFolder",
                    ApiKey = "YourAPIKey"
                });

                File.WriteAllText(path, defaultConfig); ;
            }
        }
    }
}
