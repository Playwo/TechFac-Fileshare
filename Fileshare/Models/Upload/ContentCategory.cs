using System;

namespace Fileshare.Models
{
    [Flags]
    public enum ContentCategory
    {
        None = 0,
        Image = 1,
        Text = 2,
        Video = 4,
        Other = 8
    }

    public static class ContentCategoryExtensions
    {
        public static string[] GetContentTypes(this ContentCategory contentCategory)
        {
            return contentCategory switch
            {
                ContentCategory.Image => new[] {
                "image/gif",
                "image/x-icon",
                "image/jpeg",
                "image/png",
                "image/apng",
                "image/bmp",
                "image/tiff",
                "image/svg+xml",
                "image/webp" },

                ContentCategory.Text => new[] {
                "text/plain",
                "application/json",
                "text/xml",
                "text/html"},

                ContentCategory.Video => new[] {
                "video/mp4"},

                _ => Array.Empty<string>(),
            };
        }
    }
}
