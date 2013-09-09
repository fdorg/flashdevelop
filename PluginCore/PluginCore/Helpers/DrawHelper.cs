using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

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
                System.Drawing.StringFormat format = new System.Drawing.StringFormat();
                System.Drawing.RectangleF rect = new System.Drawing.RectangleF(0, 0, 1000, 1000);
                System.Drawing.CharacterRange[] ranges = { new System.Drawing.CharacterRange(0, text.Length) };
                System.Drawing.Region[] regions = new System.Drawing.Region[1];
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
