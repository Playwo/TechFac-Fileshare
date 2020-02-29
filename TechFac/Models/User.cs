using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Fileshare.Extensions;

namespace Fileshare.Models
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; }

        [JsonIgnore]
        public string PasswordHash { get; private set; }
        public string Token { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        [JsonIgnore]
        public virtual List<Upload> Uploads { get; protected set; } //Nav Property

        public User(string username, string password, string token)
        {
            Id = Guid.NewGuid();
            Username = username;
            PasswordHash = password.GetHashString();
            Token = token;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        protected User()
        {
        }
    }
}
