using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Fileshare.Models;
using Microsoft.EntityFrameworkCore.Internal;

namespace Fileshare.Extensions
{
    public static partial class Extension
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

        public static bool FitsInContentCategory(this string contentType, ContentCategory category) 
            => contentType.ContainsAny(StringComparison.OrdinalIgnoreCase, category.GetContentTypes());

        public static ContentCategory GetContentCategory(this string contentType) 
            => Enum.GetValues(typeof(ContentCategory))
                .Cast<ContentCategory>()
                .Where(x => FitsInContentCategory(contentType, x))
                .FirstOrDefault();
    }
}
