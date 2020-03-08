using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;

namespace Fileshare.Extensions
{
    public static partial class Extensions
    {
        public static byte[] GetHash(this string inputString)
        {
            using HashAlgorithm algorithm = SHA512.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(this string inputString)
        {
            var sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        public static bool ContainsAny(this string inputString, StringComparison comparisonType, params string[] values)
            => values.Any(x => inputString.Contains(x, comparisonType));

        public static bool StartsWithAny(this string inputString, StringComparison comparisonType, params string[] values)
            => values.Any(x => inputString.StartsWith(x, comparisonType));

        public static bool IsImageContentType(this string contentType)
            => contentType.ContainsAny(StringComparison.OrdinalIgnoreCase,
                "image/gif",
                "image/x-icon",
                "image/jpeg",
                "image/png",
                "image/apng",
                "image/bmp",
                "image/tiff",
                "image/svg+xml",
                "image/webp");

        public static bool IsVideoContentType(this string contentType)
            => contentType.ContainsAny(StringComparison.OrdinalIgnoreCase,
                "video/mp4");

        public static bool IsTextContentType(this string contentType)
            => contentType.ContainsAny(StringComparison.OrdinalIgnoreCase,
                "text/plain",
                "application/json",
                "text/xml",
                "text/html");

        public static bool DoesSupportPreview(this string contentType)
            => IsImageContentType(contentType) ||
               IsTextContentType(contentType);

    }
}
