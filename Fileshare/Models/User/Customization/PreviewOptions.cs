using System;
using System.Text.Json.Serialization;

namespace Fileshare.Models
{
    public class PreviewOptions
    {
        public Guid UserId { get; private set; }

        public bool RedirectAgents { get; set; }
        public ContentCategory RedirectCategories { get; set; }

        [JsonIgnore]
        public virtual User User { get; protected set; } //Nav Property

        public PreviewOptions(Guid userId)
        {
            UserId = userId;
        }
    }
}
