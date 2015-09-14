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
        public static String ChangeEncoding(String text, Int32 from, Int32 to)
        {
            try
            {
                Encoding toEnc = Encoding.GetEncoding(from);
                Encoding fromEnc = Encoding.GetEncoding(to);
                Byte[] fromBytes = fromEnc.GetBytes(text);
                Byte[] toBytes = Encoding.Convert(fromEnc, toEnc, fromBytes);
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
        public static String KeysToString(Keys keys)
        {
            KeysConverter kc = new KeysConverter();
            return kc.ConvertToString(keys);
        }

        /// <summary>
        /// Converts a string to a Base64 string
        /// </summary>
        public static String StringToBase64(String text, Encoding encoding)
        {
            try
            {
                Char[] chars = text.ToCharArray();
                Byte[] bytes = encoding.GetBytes(chars);
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
        public static String Base64ToString(String base64, Encoding encoding)
        {
            try
            {
                Byte[] bytes = Convert.FromBase64String(base64);
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
        public static Int32 StringToColor(String aColor)
        {
            if (aColor != null)
            {
                Color c = Color.FromName(aColor);
                if (c.ToArgb() == 0 && aColor.Length >= 6)
                {
                    Int32 col = 0;
                    if (aColor.StartsWith("0x")) Int32.TryParse(aColor.Substring(2), NumberStyles.HexNumber, null, out col);
                    else Int32.TryParse(aColor, out col);
                    return TO_COLORREF(col);
                }
                return TO_COLORREF(c.ToArgb() & 0x00ffffff);
            }
            return 0;
        }
        private static Int32 TO_COLORREF(Int32 c)
        {
            return (((c & 0xff0000) >> 16) + ((c & 0x0000ff) << 16) + (c & 0x00ff00));
        }

        /// <summary>
        /// Converts a color to a string
        /// </summary>
        public static String ColorToHex(Color color)
        {
            return String.Concat("0x", color.R.ToString("X2", null), color.G.ToString("X2", null), color.B.ToString("X2", null));
        }

        /// <summary>
        /// Converts a integer (BGR order) to a color
        /// </summary>
        private static Color BGRToColor(Int32 bgr)
        {
            return Color.FromArgb((bgr >> 0) & 0xff, (bgr >> 8) & 0xff, (bgr >> 16) & 0xff);
        }

        /// <summary>
        /// Converts a color to an integer (BGR order)
        /// </summary>
        public static Int32 ColorToBGR(Color color)
        {
            return TO_COLORREF(color.ToArgb() & 0x00ffffff);
        }

        /// <summary>
        /// Alias for ColorToBGR to not break the API.
        /// </summary>
        public static Int32 ColorToInt32(Color color)
        {
            return ColorToBGR(color);
        }

    }

}
