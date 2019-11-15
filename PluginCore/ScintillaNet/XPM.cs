// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using PluginCore.PluginCore.Utilities;

namespace ScintillaNet
{
    public class XPM
    {
        /// <summary>
        /// Converts Bitmap images to XPM data for use with ScintillaNet.
        /// Warning: images with more than (around) 50 colors will generate incorrect XPM
        /// tColor: specified transparent color in format: "#00FF00".
        /// </summary>
        public static string ConvertToXPM(Bitmap bmp, string tColor)
        {
            var sb = new StringBuilder();
            var colors = new List<string>();
            var chars  = new List<char>();
            
            OctreeQuantizer quantizer = new OctreeQuantizer(50, 8, ColorTranslator.FromHtml(tColor));
            Bitmap reducedBmp = quantizer.Quantize(bmp);

            int width = reducedBmp.Width;
            int height = reducedBmp.Height;
            sb.Append("/* XPM */static char * xmp_data[] = {\"").Append(width).Append(" ").Append(height).Append(" ? 1\"");
            int colorsIndex = sb.Length;
            for (int y = 0; y<height; y++)
            {
                sb.Append(",\"");
                for (int x = 0; x<width; x++)
                {
                    var col = ColorTranslator.ToHtml(reducedBmp.GetPixel(x,y));
                    var index = colors.IndexOf(col);
                    char c;
                    if (index < 0)
                    {
                        colors.Add(col);
                        index = colors.Count + 65;
                        if (index > 90) index += 6;
                        c = Encoding.ASCII.GetChars(new[] {(byte) (index & 0xff)})[0];
                        chars.Add(c);
                        sb.Insert(colorsIndex, ",\"" + c + " c " + col + "\"");
                        colorsIndex += 14;
                    }
                    else c = chars[index];
                    sb.Append(c);
                }
                sb.Append("\"");
            }
            sb.Append("};");
            string result = sb.ToString();
            int p = result.IndexOf('?');
            string finalColor = result.Substring(0, p) + colors.Count + result.Substring(p + 1).Replace(tColor.ToUpper(), "None");
            return finalColor;
        }
        
    }
    
}
