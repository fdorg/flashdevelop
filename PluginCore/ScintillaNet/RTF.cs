// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using PluginCore;
using ScintillaNet.Configuration;

namespace ScintillaNet
{
    /// <summary>
    /// This class handles conversions of the Scintilla text to RTF.
    /// </summary>
    public static class RTF
    {
        public const string RTF_HEADEROPEN = @"{\rtf\ansi";
        public const string RTF_HEADERCLOSE = @"}";
        public const string RTF_FONTTABLEOPEN = @"{\fonttbl";
        public const string RTF_FONTTABLECLOSE = @"}";
        public const string RTF_COLORTABLEOPEN = @"{\colortbl";
        public const string RTF_COLORTABLECLOSE = @"}";
        public const string RTF_FONTDEFOPEN = @"{";
        public const string RTF_FONTDEFCLOSE = @";}";
        public const string RTF_COLORDEFDELIMITER = @";";
        public const string RTF_SETFONTFACE = @"\f";
        public const string RTF_SETFONTSIZE = @"\fs";
        public const string RTF_SETCOLORFORE = @"\cf";
        public const string RTF_SETCOLORBACK = @"\cb";
        public const string RTF_SETHIGHLIGHT = @"\highlight";
        public const string RTF_SETCOLORRED = @"\red";
        public const string RTF_SETCOLORGREEN = @"\green";
        public const string RTF_SETCOLORBLUE = @"\blue";
        public const string RTF_BOLD_ON = @"\b ";
        public const string RTF_BOLD_OFF = @"\b0 ";
        public const string RTF_ITALIC_ON = @"\i ";
        public const string RTF_ITALIC_OFF = @"\i0 ";
        public const string RTF_UNDERLINE_ON = @"\ul ";
        public const string RTF_UNDERLINE_OFF = @"\ul0 ";
        public const string RTF_STRIKE_ON = @"\strike ";
        public const string RTF_STRIKE_OFF = @"\strike0 ";
        public const string RTF_TAB = @"\tab ";
        public const string RTF_EOL = @"\par ";
        public const string RTF_LINEBREAK = @"\line ";
        public const string RTF_HEX = @"\'";
        public const string RTF_UNICODE = @"\u";
        public const string RTF_UNICODESUBSTITUTE = @"?";
        public const int MAX_STYLEDEF = 40;

        /// <summary>
        /// Converts a string to RTF based on scintilla configuration
        /// </summary>
        public static string GetConversion(Language lang, ScintillaControl sci, int start, int end)
        {
            int lengthDoc = sci.TextLength;
            if (end < 0 || end > lengthDoc)
            {
                end = lengthDoc;
            }

            var useStyles = new UseStyle[MAX_STYLEDEF];
            foreach (var useStyle in lang.usestyles)
            {
                if (useStyle.key >= MAX_STYLEDEF)
                {
                    // highest style is STYLE_LASTPREDEFINED, which is 39
                    // something is wrong if the code reaches here... throw an exception?
                    continue;
                }
                useStyles[useStyle.key] = useStyle;
            }

            var fontTable = new StringBuilder(RTF_FONTTABLEOPEN);
            var colorTable = new StringBuilder(RTF_COLORTABLEOPEN);
            var body = new StringBuilder();

            var fontDef = new Dictionary<string, int>();
            var colorDef = new Dictionary<int, int>();
            int fontCount = 0;
            int colorCount = 0;

            int lastStyleByte = -1;
            string lastFontName = null;
            int lastFontSize = -1;
            int lastForeColor = -1;
            int lastBackColor = -1;
            bool lastBold = false;
            bool lastItalic = false;

            // Add the 'default' style back colour without the colour definition
            // so the RTF reader uses its own default colour - the 'auto' colour
            colorDef.Add(GetBackColor(useStyles[0]), colorCount++);
            colorTable.Append(RTF_COLORDEFDELIMITER);

            string chars = sci.GetTextRange(start, end);
            int length = end - start;
            for (int i = 0; i < length; i++)
            {
                int styleByte = sci.StyleAt(start + i);

                if (styleByte >= MAX_STYLEDEF)
                {
                    // highest style is STYLE_LASTPREDEFINED, which is 39
                    // something is wrong if the code reaches here... throw an exception?
                    styleByte = 0;
                }

                if (styleByte != lastStyleByte)
                {
                    var style = useStyles[styleByte];

                    if (style == null)
                    {
                        // there shouldn't be any style that's not defined but present in the editor
                        // something is wrong if the code reaches here... throw an exception?
                        styleByte = 0;
                        style = useStyles[styleByte];
                    }

                    string fontName = style.FontName;
                    if (lastFontName != fontName)
                    {
                        int fontIndex;
                        if (!fontDef.TryGetValue(fontName, out fontIndex))
                        {
                            fontIndex = fontCount++;
                            fontDef.Add(fontName, fontIndex);
                            fontTable.Append(RTF_FONTDEFOPEN + RTF_SETFONTFACE + fontIndex + " " + fontName + RTF_FONTDEFCLOSE);
                        }
                        body.Append(RTF_SETFONTFACE + fontIndex + " ");
                        lastFontName = fontName;
                    }

                    int fontSize = style.FontSize << 1; // font size is stored in half-points in RTF
                    if (lastFontSize != fontSize)
                    {
                        body.Append(RTF_SETFONTSIZE + fontSize + " ");
                        lastFontSize = fontSize;
                    }

                    int foreColor = GetForeColor(style);
                    if (lastForeColor != foreColor)
                    {
                        int colorIndex;
                        if (!colorDef.TryGetValue(foreColor, out colorIndex))
                        {
                            var color = Color.FromArgb(foreColor);
                            colorIndex = colorCount++;
                            colorDef.Add(foreColor, colorIndex);
                            colorTable.Append(RTF_SETCOLORRED + color.R + RTF_SETCOLORGREEN + color.G + RTF_SETCOLORBLUE + color.B + RTF_COLORDEFDELIMITER);
                        }
                        body.Append(RTF_SETCOLORFORE + colorIndex + " ");
                        lastForeColor = foreColor;
                    }

                    int backColor = GetBackColor(style);
                    if (lastBackColor != backColor)
                    {
                        int colorIndex;
                        if (!colorDef.TryGetValue(backColor, out colorIndex))
                        {
                            var color = Color.FromArgb(backColor);
                            colorIndex = colorCount++;
                            colorDef.Add(backColor, colorIndex);
                            colorTable.Append(RTF_SETCOLORRED + color.R + RTF_SETCOLORGREEN + color.G + RTF_SETCOLORBLUE + color.B + RTF_COLORDEFDELIMITER);
                        }
                        body.Append(RTF_SETCOLORBACK + colorIndex + " ");
                        body.Append(RTF_SETHIGHLIGHT + colorIndex + " "); // set highlight colour as well
                        lastBackColor = backColor;
                    }

                    bool bold = style.IsBold;
                    if (lastBold != bold)
                    {
                        body.Append(bold ? RTF_BOLD_ON : RTF_BOLD_OFF);
                        lastBold = bold;
                    }

                    bool italic = style.IsItalics;
                    if (lastItalic != italic)
                    {
                        body.Append(italic ? RTF_ITALIC_ON : RTF_ITALIC_OFF);
                        lastItalic = italic;
                    }

                    lastStyleByte = styleByte;
                }

                char c = chars[i];
                switch (c)
                {
                    case '\0':
                        break; // ignore NULL characters as they will cause the clipboard to truncate the string
                    case '{':
                        body.Append(@"\{");
                        break;
                    case '}':
                        body.Append(@"\}");
                        break;
                    case '\\':
                        body.Append(@"\\");
                        break;
                    case '\t':
                        body.Append(RTF_TAB);
                        break;
                    case '\r':
                        body.Append(RTF_EOL); // alternative: RTF_LINEBREAK
                        break;
                    case '\n':
                        if (i == 0 || chars[i - 1] != '\r')
                        {
                            body.Append(RTF_EOL); // alternative: RTF_LINEBREAK
                        }
                        break;
                    default:
                        if (' ' <= c || c <= sbyte.MaxValue)
                        {
                            body.Append(c);
                        }
                        else if (c <= byte.MaxValue)
                        {
                            body.Append(RTF_HEX + ((byte) c).ToString("x2"));
                        }
                        else if (c <= ushort.MaxValue)
                        {
                            body.Append(RTF_UNICODE + ((short) c) + RTF_UNICODESUBSTITUTE);
                        }
                        break;
                }
            }

            fontTable.Append(RTF_FONTTABLECLOSE);
            colorTable.Append(RTF_COLORTABLECLOSE);

            return RTF_HEADEROPEN + fontTable + colorTable + body + RTF_HEADERCLOSE;
        }

        private static int GetBackColor(StyleClass style)
        {
            if (style.back != null)
            {
                return GetRgbColor(style.back);
            }
            var parent = style.ParentClass;
            if (parent != null)
            {
                return GetBackColor(parent);
            }
            return 0xFFFFFF;
        }

        private static int GetForeColor(StyleClass style)
        {
            if (style.fore != null)
            {
                return GetRgbColor(style.fore);
            }
            var parent = style.ParentClass;
            if (parent != null)
            {
                return GetForeColor(parent);
            }
            return 0x000000;
        }

        private static int GetRgbColor(string value)
        {
            int color = Color.FromName(value).ToArgb();
            if (color == 0x00000000)
            {
                if (value.StartsWithOrdinal("0x"))
                {
                    return int.Parse(value.Substring(2), NumberStyles.HexNumber);
                }
                try
                {
                    return int.Parse(value, NumberStyles.HexNumber);
                }
                catch (Exception) { }
            }
            return color & 0x00FFFFFF;
        }
    }
}
