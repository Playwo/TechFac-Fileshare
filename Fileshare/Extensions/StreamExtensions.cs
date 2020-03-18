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
            byte[] hash = hashAlgorithm.ComputeHash(stream);
            string checksum = Encoding.ASCII.GetString(hash);

            stream.Position = 0;
            return checksum;
        }
    }
}
