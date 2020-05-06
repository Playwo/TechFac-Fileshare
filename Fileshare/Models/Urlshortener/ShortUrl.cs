using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fileshare.Models
{
    public class ShortUrl
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid TargetId { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public int UseCount { get; set; }
        
        public virtual RedirectTarget Target { get; protected set; } //Nav Property
        public virtual User User { get; protected set; } //Nav Property

        public ShortUrl(Guid userId, Guid targetId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            TargetId = targetId;

            CreatedAt = DateTimeOffset.UtcNow;
            UseCount = 0;
        }
    }
}
