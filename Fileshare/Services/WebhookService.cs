using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Fileshare.Extensions;
using Fileshare.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Fileshare.Services
{
    public class WebhookService : Service
    {
        private readonly Uri WebhookUrl;
        private readonly bool Enabled;
        private readonly BlockingCollection<Upload> WebhookUploadQueue;
        private readonly HttpClient Client;
        private readonly ILogger Logger;

        public WebhookService(IConfiguration configuration, HttpClient client, ILogger<WebhookService> logger)
        {
            Enabled = configuration.TryGetWebhookUrl(out var webhookUrl);

            if (Enabled)
            {
                WebhookUrl = webhookUrl;
                WebhookUploadQueue = new BlockingCollection<Upload>();
                Client = client;
                Logger = logger;
            }
        }

        public override ValueTask RunAsync()
        {
            _ = Task.Run(() => SendWebHooksAsync());
            return base.RunAsync();
        }

        public void QueueUpload(Upload upload)
        {
            if (Enabled)
            {
                WebhookUploadQueue.Add(upload);
            }
        }

        private async Task SendWebHooksAsync()
        {
            foreach (var upload in WebhookUploadQueue.GetConsumingEnumerable())
            {
                var delay = Task.Delay(2500); //Prevent Spam
                string content = CreateDiscordEmbed(upload);
                var result = await Client.PostAsync(WebhookUrl, new StringContent(content, Encoding.UTF8, "application/json"));

                if (!result.IsSuccessStatusCode)
                {
                    Logger.LogWarning($"A webhook request has failed! Reason: {result.ReasonPhrase}");
                }

                await delay;
            }
        }

        private string CreateDiscordEmbed(Upload upload)
        {
            var content = new
            {
                username = $"{upload.User.Username}",
                embeds = new[]
                {
                    new
                    {
                        title = "I've just uploaded a file!",
                        fields = new[]
                        {
                            new
                            {
                                name = "Type",
                                value = $"{upload.ContentType}",
                                inline = true
                            }
                        }
                    }
                }
            };

            return JsonConvert.SerializeObject(content);
        }
    }
}
