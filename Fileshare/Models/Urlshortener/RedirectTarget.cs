using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fileshare.Models
{
    public class RedirectTarget
    {
        public Guid Id { get; private set; }
        public string TargetUrl { get; private set; }

        public DateTimeOffset CreatedAt { get; set; }

        public virtual List<ShortUrl> ShortUrls { get; protected set; } //Nav Property
    }
}
