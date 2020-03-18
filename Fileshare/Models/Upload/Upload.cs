using System;
using System.Text.Json.Serialization;

namespace Fileshare.Models
{
    public class Upload
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid FileId { get; private set; }

        public string Name { get; private set; }
        public string Extension { get; private set; }
        public string ContentType { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        [JsonIgnore]
        public string Filename => $"{Name}.{Extension}";

        [JsonIgnore]
        public virtual User User { get; protected set; } //Nav Property
        [JsonIgnore]
        public virtual LocalFile LocalFile { get; protected set; } //Nav Property

        public Upload(Guid userId, Guid fileId, string name, string extension, string contentType)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            FileId = fileId;

            Name = name;
            Extension = extension;
            ContentType = contentType;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        protected Upload()
        {
        }
    }
}
