using System;
using System.Collections.Generic;

namespace Fileshare.Models
{
    public class LocalFile
    {
        public Guid Id { get; private set; }

        public string Checksum { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        public virtual List<Upload> Uploads { get; protected set; } //Nav Property

        public LocalFile(string checksum)
        {
            Id = Guid.NewGuid();

            Checksum = checksum;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        protected LocalFile()
        {
        }
    }
}
