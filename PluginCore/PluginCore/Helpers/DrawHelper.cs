// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
            if (String.IsNullOrEmpty(text)) return 0;

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
