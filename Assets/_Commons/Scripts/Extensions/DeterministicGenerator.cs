using System;
using System.Security.Cryptography;
using System.Text;

namespace Commons.Extensions
{
    public static class DeterministicGenerator
    {
        public static int GetInt(string input)
        {
            byte[] hash = ComputeHash(input);
            return BitConverter.ToInt32(hash, 0);
        }

        public static Guid GetGuid(string input)
        {
            byte[] hash = ComputeHash(input);
            byte[] guidBytes = new byte[16];
            Array.Copy(hash, guidBytes, 16);

            return new Guid(guidBytes);
        }

        private static byte[] ComputeHash(string input)
        {
            using var sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            return sha256.ComputeHash(inputBytes);
        }
    }
}
