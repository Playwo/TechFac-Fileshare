using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fileshare.Models
{
    public class RedirectTarget
    {
        public Guid Id { get; private set; }
        public string TargetUrl { get; private set; }

        public DateTimeOffset CreatedAt { get; set; }

        [JsonIgnore]
        public virtual List<ShortUrl> ShortUrls { get; protected set; } //Nav Property

        public RedirectTarget(string targetUrl)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;

            TargetUrl = targetUrl;
        }
    }
}
