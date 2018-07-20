using System;
using System.Security.Cryptography;

namespace PluginCore.Utilities
{
    public class Base16
    {
        /// <summary>
        /// Encodes bytes to a base16 string.
        /// </summary>
        public static String Encode(Byte[] bytes)
        {
            String hex = "";
            for (Int32 i = 0; i < bytes.Length; i++)
            {
                hex += bytes[i].ToString("x2");
            }
            return hex;
        }

        /// <summary>
        /// Decodes base16 string to bytes.
        /// </summary>
        public static Byte[] Decode(String base16)
        {
            Byte[] bytes = new Byte[base16.Length / 2];
            for (Int32 i = 0; i < base16.Length; i += 2)
            {
                Byte value = (Byte)Convert.ToInt32(base16.Substring(i, 2), 16);
                bytes.SetValue(value, i / 2);
            }
            return bytes;
        }

    }

    public class Base64
    {
        /// <summary>
        /// Encodes bytes to a base64 string.
        /// </summary>
        public static String Encode(Byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Decodes base64 string to bytes.
        /// </summary>
        public static Byte[] Decode(String base64)
        {
            return Convert.FromBase64String(base64);
        }

    }

    public class MD5
    {
        /// <summary>
        /// Computes the MD5 checksum for the bytes.
        /// </summary>
        public static Byte[] Compute(Byte[] bytes)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(bytes);
        }

        /// <summary>
        /// Computes the HMAC-MD5 for the key and bytes.
        /// </summary>
        public static Byte[] ComputeHMAC(Byte[] key, Byte[] bytes)
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
        public static Byte[] Compute(Byte[] bytes)
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
        static Byte[] ComputeFIPS(Byte[] bytes)
        {
            SHA1Cng sha1 = new SHA1Cng();
            return sha1.ComputeHash(bytes);
        }

        /// <summary>
        /// Computes the HMAC-SHA-1 for the key and bytes.
        /// </summary>
        public static Byte[] ComputeHMAC(Byte[] key, Byte[] bytes)
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
        public static Byte[] Compute(Byte[] bytes)
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
        static Byte[] ComputeFIPS(Byte[] bytes)
        {
            SHA256Cng sha256 = new SHA256Cng();
            return sha256.ComputeHash(bytes);
        }

        /// <summary>
        /// Computes the HMAC-SHA-256 for the key and bytes.
        /// </summary>
        public static Byte[] ComputeHMAC(Byte[] key, Byte[] bytes)
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
        public static Byte[] Compute(Byte[] bytes)
        {
            RIPEMD160Managed rmd160 = new RIPEMD160Managed();
            return rmd160.ComputeHash(bytes);
        }

        /// <summary>
        /// Computes the HMAC-RIPEMD-160 for the key and bytes.
        /// </summary>
        public static Byte[] ComputeHMAC(Byte[] key, Byte[] bytes)
        {
            HMACRIPEMD160 hmac = new HMACRIPEMD160(key);
            return hmac.ComputeHash(bytes);
        }

    }

}