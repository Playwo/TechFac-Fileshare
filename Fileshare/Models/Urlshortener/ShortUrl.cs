using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fileshare.Models
{
    public class ShortUrl
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid TargetId { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public string Name { get; private set; }
        public int UseCount { get; set; }
        
        [JsonIgnore]
        public virtual RedirectTarget Target { get; protected set; } //Nav Property
        [JsonIgnore]
        public virtual User User { get; protected set; } //Nav Property

        public ShortUrl(Guid userId, Guid targetId, string name)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            TargetId = targetId;
            Name = name;

            CreatedAt = DateTimeOffset.UtcNow;
            UseCount = 0;
        }
    }
}
