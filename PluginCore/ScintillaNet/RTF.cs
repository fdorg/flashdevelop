using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using ScintillaNet.Configuration;

namespace ScintillaNet
{
    class ColorData 
    {
        private Int32 index;
        private Color color;

        public ColorData(Int32 index, Color color) 
        {
            this.index = index;
            this.color = color;
        }

        /// <summary>
        /// The index of the color in the color table
        /// </summary>
        public Int32 ColorIndex 
        {
            get { return this.index; }
        }

        /// <summary>
        /// The actual color of the data
        /// </summary>
        public Color Color 
        {
            get { return this.color; }
        }

    }

    class FontData 
    {
        private Int32 index;
        private String fontName;

        public FontData(Int32 index, String fontName)
        {
            this.index = index;
            this.fontName = fontName;
        }

        /// <summary>
        /// The index of the font in the font table
        /// </summary>
        public Int32 FontIndex
        {
            get { return this.index; }
        }

        /// <summary>
        /// The name of the font
        /// </summary>
        public String FontName 
        {
            get { return this.fontName; }
        }
    }

    /// <summary>
    /// This class will handle conversions of the Scintilla text to RTF.
    /// </summary>
    public class RTF 
    {
        public static String RTF_HEADEROPEN = "{\\rtf1\\ansi\\ansicpg1252\\deff0";
        public static String RTF_FONTDEFOPEN = "{\\fonttbl";
        public static String RTF_COLORDEFOPEN = "{\\colortbl;";
        public static String RTF_COLORDEFCLOSE = "}";
        public static String RTF_HEADERCLOSE = "\n";
        public static String RTF_BODYOPEN = "";
        public static String RTF_BODYCLOSE = "}";
        public static String RTF_SET_FORMAT = "\\f{0}\\fs{1}\\cb{2}\\cf{3}\\b{4}\\i{5} ";
        public static String RTF_SET_COLOR = "\\red{0}\\green{1}\\blue{2};";
        public static String RTF_SETFONTFACE = "\\f";
        public static String RTF_SETFONTSIZE = "\\fs";
        public static String RTF_SETCOLOR = "\\cf";
        public static String RTF_SETBACKGROUND = "\\highlight";
        public static String RTF_BOLD_ON = "\\b";
        public static String RTF_BOLD_OFF = "\\b0";
        public static String RTF_ITALIC_ON = "\\i";
        public static String RTF_ITALIC_OFF = "\\i0";
        public static String RTF_UNDERLINE_ON = "\\ul";
        public static String RTF_UNDERLINE_OFF = "\\ulnone";
        public static String RTF_STRIKE_ON = "\\i";
        public static String RTF_STRIKE_OFF = "\\strike0";
        public static String RTF_EOLN = "\\par\n";
        public static String RTF_TAB = "\\tab ";
        public static Int32 MAX_STYLEDEF = 128;
        public static Int32 MAX_FONTDEF = 64;
        public static Int32 MAX_COLORDEF = 8;
        public static String RTF_FONTFACE = "Courier New";
        public static String RTF_COLOR = "#000000";
        public static Int32 STYLE_DEFAULT = 32;

        /// <summary>
        /// Converts a string to RTF based on scintilla configuration
        /// </summary>
        public static String GetConversion(Language lang, ScintillaControl sci, int start, int end) 
        {
            UseStyle[] useStyles = lang.usestyles;
            Dictionary<uint, ColorData> StyleColors = new Dictionary<uint, ColorData>(MAX_COLORDEF);
            Dictionary<string, FontData> StyleFonts = new Dictionary<string, FontData>(MAX_FONTDEF);
            String text = sci.Text.Clone().ToString();
            StringBuilder rtfHeader = new StringBuilder(RTF_HEADEROPEN);
            StringBuilder rtfFont = new StringBuilder(RTF_FONTDEFOPEN);
            StringBuilder rtfColor = new StringBuilder(RTF_COLORDEFOPEN);
            StringBuilder rtf = new StringBuilder();
            char[] chars = text.ToCharArray();
            int lengthDoc = text.Length;
            int lastStyleByte = -1;
            string lastFontName = "";
            int lastFontSize = -1;
            bool lastBold = false;
            bool lastItalic = false;
            uint lastBack = 0;
            uint lastFore = 0;
            if (end < 0 || end > lengthDoc) 
            {
                end = lengthDoc;
            }
            int totalColors = 1;
            int totalFonts = 0;
            //----------------------------------------------------
            //  Grab all styles used based on the Style Byte. 
            //  Then store the basic properties in a Dictionary.
            //----------------------------------------------------
            for (int istyle = start; istyle < end; istyle++) 
            {
                // Store Byte
                int styleByte = sci.StyleAt(istyle);
                // Check Difference
                if (styleByte != lastStyleByte) 
                {
                    // Store Style
                    UseStyle sty = useStyles[styleByte];
                    // Grab Properties
                    string fontName = sty.FontName;
                    int fontSize = sty.FontSize * 2;
                    bool bold = sty.IsBold;
                    bool italic = sty.IsItalics;
                    uint back = (uint)sty.BackgroundColor;
                    uint fore = (uint)(!string.IsNullOrEmpty(sty.fore) ? int.Parse(sty.fore.Substring(2, sty.fore.Length - 2), NumberStyles.HexNumber) : 0);
                    if (lastFontName != fontName || lastFontSize != fontSize || lastBold != bold || lastItalic != italic || lastBack != back || lastFore != fore) 
                    {
                        // Check Colors
                        ColorData backColorTest;
                        ColorData foreColorTest;
                        if (!StyleColors.TryGetValue(back, out backColorTest)) 
                        {
                            Color newColor = Color.FromArgb((int)back);
                            backColorTest = new ColorData(totalColors++, newColor);
                            StyleColors.Add(back, backColorTest);
                            rtfColor.AppendFormat(RTF_SET_COLOR, newColor.R, newColor.G, newColor.B);
                            Console.WriteLine(Color.FromArgb((int)back));
                        }
                        if (!StyleColors.TryGetValue(fore, out foreColorTest)) 
                        {
                            Color newColor = Color.FromArgb((int)fore);
                            foreColorTest = new ColorData(totalColors++, newColor);
                            StyleColors.Add(fore, foreColorTest);
                            rtfColor.AppendFormat(RTF_SET_COLOR, newColor.R, newColor.G, newColor.B);
                            Console.WriteLine(Color.FromArgb((int)fore));
                        }
                        // Check Fonts
                        FontData fontTest;
                        if (!StyleFonts.TryGetValue(fontName, out fontTest)) 
                        {
                            fontTest = new FontData(totalFonts, fontName);
                            StyleFonts.Add(fontName, fontTest);
                            rtfFont.Append(@"{" + RTF_SETFONTFACE + totalFonts + " " + fontName + ";}");
                            totalFonts++;
                            Console.WriteLine(fontName);
                        }
                        rtf.Append((lastStyleByte == -1 ? "{\\pard\\plain" : "}{\\pard\\plain"));
                        // Write out RTF
                        rtf.AppendFormat(RTF_SET_FORMAT, fontTest.FontIndex, fontSize, backColorTest.ColorIndex, foreColorTest.ColorIndex, (bold ? "" : "0"), (italic ? "" : "0"));
                    }
                    lastFontName = fontName;
                    lastFontSize = fontSize;
                    lastBold = bold;
                    lastItalic = italic;
                    lastBack = back;
                    lastFore = fore;
                }
                lastStyleByte = styleByte;
                char ch = chars[istyle];
                String curr = "";
                if (ch == '{') curr = "\\{";
                else if (ch == '}') curr = "\\}";
                else if (ch == '\\') curr = "\\\\";
                else if (ch == '\t') 
                {
                    if (sci.IsUseTabs) curr = RTF_TAB;
                    else curr = "".PadRight(sci.Indent, ' ');
                } 
                else if (ch == '\n') 
                {
                    if (istyle == 0 || chars[istyle - 1] != '\r') curr = "\\line\n";
                } 
                else if (ch == '\r') curr = "\\line\n";
                else if (!(Char.IsLetterOrDigit(ch) || Char.IsWhiteSpace(ch))) curr = "\\'" + ((int)ch).ToString("x2");
                else curr = ch.ToString();
                rtf.Append(@curr);
            }
            // Close Headers
            rtfColor.Append('}');
            rtfFont.Append('}');
            rtf.Append('}');
            rtfHeader.AppendFormat("\n{0}\n{1}\n{2}\n{3}", rtfFont.ToString(), rtfColor.ToString(), rtf.ToString(), "}");
            return rtfHeader.ToString();
        }

    }

}
