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
        public static Int32 MeasureDisplayStringWidth(Graphics graphics, String text, Font font)
        {
            try
            {
                StringFormat format = new StringFormat();
                RectangleF rect = new RectangleF(0, 0, 1000, 1000);
                CharacterRange[] ranges = { new CharacterRange(0, text.Length) };
                Region[] regions = new Region[1];
                format.SetMeasurableCharacterRanges(ranges);
                regions = graphics.MeasureCharacterRanges(text, font, rect, format);
                rect = regions[0].GetBounds(graphics);
                return (Int32)rect.Right;
            }
            catch (IndexOutOfRangeException)
            {
                return 0;
            }
        }

    }

}
