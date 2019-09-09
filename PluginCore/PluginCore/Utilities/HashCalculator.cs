// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Text;

namespace PluginCore.Utilities
{
    public class HashCalculator
    {
        /// <summary>
        /// Calculates the MD5 checksum
        /// </summary>
        public static string CalculateMD5(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = MD5.Compute(inputBytes);
            StringBuilder builder = new StringBuilder();
            foreach (var it in hash)
            {
                builder.Append(it.ToString("X2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Calculates the SHA-1 checksum
        /// </summary>
        public static string CalculateSHA1(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = SHA1.Compute(inputBytes);
            StringBuilder builder = new StringBuilder();
            foreach (var it in hash)
            {
                builder.Append(it.ToString("X2"));
            }
            return builder.ToString();
        }

    }

}
