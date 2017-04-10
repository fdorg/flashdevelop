// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Text;

namespace PluginCore.Utilities
{
    public class HashCalculator
    {
        /// <summary>
        /// Calculates the MD5 checksum
        /// </summary>
        public static String CalculateMD5(String input)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hash = MD5.Compute(inputBytes);
            StringBuilder builder = new StringBuilder();
            for (Int32 i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Calculates the SHA-1 checksum
        /// </summary>
        public static String CalculateSHA1(String input)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hash = SHA1.Compute(inputBytes);
            StringBuilder builder = new StringBuilder();
            for (Int32 i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }
            return builder.ToString();
        }

    }

}
