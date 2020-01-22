// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using PluginCore.Managers;

namespace PluginCore.Utilities
{
    public class DataConverter
    {
        /// <summary>
        /// Converts text to another encoding
        /// </summary>
        public static string ChangeEncoding(string text, int from, int to)
        {
            try
            {
                Encoding toEnc = Encoding.GetEncoding(from);
                Encoding fromEnc = Encoding.GetEncoding(to);
                byte[] fromBytes = fromEnc.GetBytes(text);
                byte[] toBytes = Encoding.Convert(fromEnc, toEnc, fromBytes);
                return toEnc.GetString(toBytes);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// Converts Keys to correct string presentation
        /// </summary>
        public static string KeysToString(Keys keys)
        {
            KeysConverter kc = new KeysConverter();
            return kc.ConvertToString(keys);
        }

        /// <summary>
        /// Converts a string to a Base64 string
        /// </summary>
        public static string StringToBase64(string text, Encoding encoding)
        {
            try
            {
                char[] chars = text.ToCharArray();
                byte[] bytes = encoding.GetBytes(chars);
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// Converts a Base64 string to a string
        /// </summary>
        public static string Base64ToString(string base64, Encoding encoding)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                return encoding.GetString(bytes);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// Converts a String to a color (BGR order)
        /// </summary>
        public static int StringToColor(string aColor)
        {
            if (aColor != null)
            {
                Color c = Color.FromName(aColor);
                if (c.ToArgb() == 0 && aColor.Length >= 6)
                {
                    int col;
                    if (aColor.StartsWithOrdinal("0x")) int.TryParse(aColor.Substring(2), NumberStyles.HexNumber, null, out col);
                    else int.TryParse(aColor, out col);
                    return TO_COLORREF(col);
                }
                return TO_COLORREF(c.ToArgb() & 0x00ffffff);
            }
            return 0;
        }

        static int TO_COLORREF(int c) => (((c & 0xff0000) >> 16) + ((c & 0x0000ff) << 16) + (c & 0x00ff00));

        /// <summary>
        /// Converts a color to a string
        /// </summary>
        public static string ColorToHex(Color color)
        {
            return string.Concat("0x", color.R.ToString("X2", null), color.G.ToString("X2", null), color.B.ToString("X2", null));
        }

        /// <summary>
        /// Converts a integer (BGR order) to a color
        /// </summary>
        public static Color BGRToColor(int bgr)
        {
            return Color.FromArgb((bgr >> 0) & 0xff, (bgr >> 8) & 0xff, (bgr >> 16) & 0xff);
        }

        /// <summary>
        /// Converts a color to an integer (BGR order)
        /// </summary>
        public static int ColorToBGR(Color color) => TO_COLORREF(color.ToArgb() & 0x00ffffff);

        /// <summary>
        /// Alias for ColorToBGR to not break the API.
        /// </summary>
        public static int ColorToInt32(Color color) => ColorToBGR(color);
    }

}
