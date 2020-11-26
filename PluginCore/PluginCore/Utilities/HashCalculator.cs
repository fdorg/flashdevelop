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
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = MD5.Compute(bytes);
            var sb = new StringBuilder();
            foreach (var it in hash)
            {
                sb.Append(it.ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Calculates the SHA-1 checksum
        /// </summary>
        public static string CalculateSHA1(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA1.Compute(bytes);
            var sb = new StringBuilder();
            foreach (var it in hash)
            {
                sb.Append(it.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}