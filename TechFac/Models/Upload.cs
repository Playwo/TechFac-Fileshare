using System;
using System.Text.Json.Serialization;

namespace Fileshare.Models
{
    public class Upload
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Filename { get; private set; }
        public string ContentType { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        [JsonIgnore]
        public virtual User User { get; protected set; } //Nav Property


        public Upload(Guid userId, string filename, string contentType)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Filename = filename;
            ContentType = contentType;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        protected Upload()
        {
        }
    }
}
