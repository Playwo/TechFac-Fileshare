using System;
using System.Text;

namespace Fileshare.Extensions
{
    public static partial class Extension
    {
        private readonly static char[] Letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

        public static string NextString(this Random random, int lenght)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < lenght; i++)
            {
                int index = random.Next(0, Letters.Length);
                builder.Append(Letters[index]);
            }

            return builder.ToString();
        }

        public static string NextToken(this Random random) => NextString(random, 32);
    }
}
