using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Fileshare.Extensions
{
    public static partial class Extension
    {
        public static string GenerateChecksum(this Stream stream) 
            => GenerateChecksum(stream, SHA1.Create());

        public static string GenerateChecksum(this Stream stream, HashAlgorithm hashAlgorithm)
        {
            stream.Position = 0;
            byte[] hash = hashAlgorithm.ComputeHash(stream);
            stream.Position = 0;

            var sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
