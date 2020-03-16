using System;
using System.Drawing;
using System.Text.Json.Serialization;

namespace Fileshare.Models
{
    public class PreviewOptions
    {
        public Guid UserId { get; private set; }

        public bool RedirectAgents { get; set; }
        public ContentCategory RedirectCategories { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BoxColor { get; set; }
        
        [JsonIgnore]
        public virtual User User { get; protected set; } //Nav Property

        public PreviewOptions(Guid userId)
        {
            UserId = userId;

            RedirectAgents = true;
            RedirectCategories = ContentCategory.Image;

            BackgroundColor = Color.DarkSlateGray;
            BoxColor = Color.FromArgb(66, 45, 4);
        }
    }
}
