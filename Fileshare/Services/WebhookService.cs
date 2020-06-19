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
using YamlDotNet.Core.Tokens;

namespace Fileshare.Services
{
    public class WebhookService : Service
    {
        private readonly HttpClient Client;
        private readonly ILogger Logger;

        private readonly Uri WebhookUrl;
        private readonly bool Enabled;
        private readonly BlockingCollection<string> WebhookQueue;

        public WebhookService(IConfiguration configuration, HttpClient client, ILogger<WebhookService> logger)
        {
            Enabled = configuration.TryGetWebhookUrl(out var webhookUrl);

            if (Enabled)
            {
                WebhookUrl = webhookUrl;
                WebhookQueue = new BlockingCollection<string>();
                Client = client;
                Logger = logger;
            }
        }

        public override ValueTask RunAsync()
        {
            if (Enabled)
            {
                _ = Task.Run(() => SendWebHooksAsync());
            }

            return base.RunAsync();
        }

        public void QueueUpload(Upload upload)
        {
            if (Enabled)
            {
                string message = CreateUploadEmbed(upload);
                WebhookQueue.Add(message);
            }
        }

        public void QueueShortUrl(ShortUrl shortUrl)
        {
            if (Enabled)
            {
                string message = CreateShortUrlEmbed(shortUrl);
                WebhookQueue.Add(message);
            }
        }


        private async Task SendWebHooksAsync()
        {
            foreach (string message in WebhookQueue.GetConsumingEnumerable())
            {
                var delay = Task.Delay(2500); //Prevent Spam
                var result = await Client.PostAsync(WebhookUrl, new StringContent(message, Encoding.UTF8, "application/json"));

                if (!result.IsSuccessStatusCode)
                {
                    Logger.LogWarning($"A webhook request has failed! Reason: {result.ReasonPhrase}");
                }

                await delay;
            }
        }

        private string CreateUploadEmbed(Upload upload)
            => CreateEmbed(upload.User.Username, "I've just uploaded a file!", fields: new[] { new Field("Type", upload.ContentType, true) });

        private string CreateShortUrlEmbed(ShortUrl shortUrl) 
            => CreateEmbed(shortUrl.User.Username, "I've just shortened an url!", shortUrl.Target.TargetUrl);

        private string CreateEmbed(string username, string title, string description = "", params Field[] fields)
        {
            var content = new
            {
                username = username,
                embeds = new[]
                {
                    new
                    {
                        title = title,
                        description = description,
                        fields = fields,
                     }
                  }
            };

            return JsonConvert.SerializeObject(content).ToLower();
        }

        class Field
        {
            public readonly string Name;
            public readonly string Value;
            public readonly bool Inline;

            public Field(string name, string value, bool inline)
            {
                Name = name;
                Value = value;
                Inline = inline;
            }
        }
    }
}
