// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Security.Cryptography;

namespace PluginCore.Utilities
{
    public class Base16
    {
        /// <summary>
        /// Encodes bytes to a base16 string.
        /// </summary>
        public static string Encode(byte[] bytes)
        {
            var result = string.Empty;
            foreach (var it in bytes)
            {
                result += it.ToString("x2");
            }
            return result;
        }

        /// <summary>
        /// Decodes base16 string to bytes.
        /// </summary>
        public static byte[] Decode(string base16)
        {
            var result = new byte[base16.Length / 2];
            for (int i = 0; i < base16.Length; i += 2)
            {
                var value = (byte)Convert.ToInt32(base16.Substring(i, 2), 16);
                result.SetValue(value, i / 2);
            }
            return result;
        }
    }

    public class Base64
    {
        /// <summary>
        /// Encodes bytes to a base64 string.
        /// </summary>
        public static string Encode(byte[] bytes) => Convert.ToBase64String(bytes);

        /// <summary>
        /// Decodes base64 string to bytes.
        /// </summary>
        public static byte[] Decode(string base64) => Convert.FromBase64String(base64);
    }

    public class MD5
    {
        /// <summary>
        /// Computes the MD5 checksum for the bytes.
        /// </summary>
        public static byte[] Compute(byte[] bytes)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(bytes);
        }

        /// <summary>
        /// Computes the HMAC-MD5 for the key and bytes.
        /// </summary>
        public static byte[] ComputeHMAC(byte[] key, byte[] bytes)
        {
            HMACMD5 hmac = new HMACMD5(key);
            return hmac.ComputeHash(bytes);
        }
    }

    public class SHA1
    {
        static bool fipsComplianceRequired;

        /// <summary>
        /// Computes the SHA-1 checksum for the bytes.
        /// </summary>
        public static byte[] Compute(byte[] bytes)
        {
            if (fipsComplianceRequired) return ComputeFIPS(bytes);
            try
            {
                SHA1Managed sha1 = new SHA1Managed();
                return sha1.ComputeHash(bytes);
            }
            catch (InvalidOperationException)
            {
                fipsComplianceRequired = true;
                return ComputeFIPS(bytes);
            }
        }
        static byte[] ComputeFIPS(byte[] bytes)
        {
            SHA1Cng sha1 = new SHA1Cng();
            return sha1.ComputeHash(bytes);
        }

        /// <summary>
        /// Computes the HMAC-SHA-1 for the key and bytes.
        /// </summary>
        public static byte[] ComputeHMAC(byte[] key, byte[] bytes)
        {
            HMACSHA1 hmac = new HMACSHA1(key);
            return hmac.ComputeHash(bytes);
        }

    }

    public class SHA256
    {
        static bool fipsComplianceRequired;

        /// <summary>
        /// Computes the SHA-256 checksum for the bytes.
        /// </summary>
        public static byte[] Compute(byte[] bytes)
        {
            if (fipsComplianceRequired) return ComputeFIPS(bytes);
            try
            {
                SHA256Managed sha256 = new SHA256Managed();
                return sha256.ComputeHash(bytes);
            }
            catch (InvalidOperationException)
            {
                fipsComplianceRequired = true;
                return ComputeFIPS(bytes);
            }
        }
        static byte[] ComputeFIPS(byte[] bytes)
        {
            SHA256Cng sha256 = new SHA256Cng();
            return sha256.ComputeHash(bytes);
        }

        /// <summary>
        /// Computes the HMAC-SHA-256 for the key and bytes.
        /// </summary>
        public static byte[] ComputeHMAC(byte[] key, byte[] bytes)
        {
            HMACSHA256 hmac = new HMACSHA256(key);
            return hmac.ComputeHash(bytes);
        }

    }

    public class RMD160
    {
        /// <summary>
        /// Computes the RIPEMD-160 checksum for the bytes.
        /// </summary>
        public static byte[] Compute(byte[] bytes)
        {
            RIPEMD160Managed rmd160 = new RIPEMD160Managed();
            return rmd160.ComputeHash(bytes);
        }

        /// <summary>
        /// Computes the HMAC-RIPEMD-160 for the key and bytes.
        /// </summary>
        public static byte[] ComputeHMAC(byte[] key, byte[] bytes)
        {
            HMACRIPEMD160 hmac = new HMACRIPEMD160(key);
            return hmac.ComputeHash(bytes);
        }
    }
}