using System;
using System.Drawing;

namespace PluginCore.Helpers
{
    public class DrawHelper
    {
        /// <summary>
        /// Measures the actual width of the text
        /// http://www.codeproject.com/KB/GDI-plus/measurestring.aspx
        /// </summary>
        public static int MeasureDisplayStringWidth(Graphics graphics, string text, Font font)
        {
            if (string.IsNullOrEmpty(text)) return 0;

            try
            {
                var format = new StringFormat();
                var rect = new RectangleF(0, 0, 1000, 1000);
                CharacterRange[] ranges = { new CharacterRange(0, text.Length) };
                format.SetMeasurableCharacterRanges(ranges);
                var regions = graphics.MeasureCharacterRanges(text, font, rect, format);
                rect = regions[0].GetBounds(graphics);
                return (int)rect.Right;
            }
            catch (IndexOutOfRangeException)
            {
                return 0;
            }
        }
    }
}