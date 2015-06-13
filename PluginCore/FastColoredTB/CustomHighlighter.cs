using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using ScintillaNet.Enums;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet.Configuration;

namespace FastColoredTextBoxNS
{
    public class CustomHighlighter
    {
        public ScintillaNet.Configuration.Language Language
        {
            set
            {
                language = value;
                for (int i = 0; i < 32; i++)
                {
                    regexes[i] = new Regex("");

                    UseStyle style = language.GetUseStyle(i);
                    if (style != null)
                        styles[i] = new TextStyle(
                            BrushFromRGB(style.ForegroundColor),
                            BrushFromRGB(style.BackgroundColor),
                            System.Drawing.FontStyle.Regular);
                }
                InitCppRegexes();
            }
            get { return language; }
        }

        private Brush BrushFromRGB(int color)
        {
            return new SolidBrush(DataConverter.BGRToColor(color));
        }

        private ScintillaNet.Configuration.Language language;
        private FastColoredTextBox editor;
        private Regex[] regexes = new Regex[32];
        private Style[] styles = new Style[32];

        /**
        * Built in styles:
        * DefaultStyle, SelectionStyle, FoldedBlockStyle, BracketsStyle, BracketsStyle2, BracketsStyle3
        * Built in colors:
        * BackColor (BackBrush too), ForeColor, CurrentLineColor, ChangedLineColor, BookmarkColor, LineNumberColor, IndentBackColor, PaddingBackColor, 
        * DisabledColor, CaretColor, ServiceLinesColor, FoldingIndicatorColor, ServiceColors (has 6), 
        */

        public CustomHighlighter(FastColoredTextBox fctb)
        {
            editor = fctb;
            //editor.AllowSeveralTextStyleDrawing = true;
        }

        private void InitCppRegexes()
        {
            regexes[CPP.COMMENT] = new Regex(@"//.*$", RegexOptions.Multiline | RegexOptions.Compiled);
            regexes[CPP.COMMENTLINE] = new Regex(@"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline | RegexOptions.Compiled);
            regexes[CPP.NUMBER] = new Regex(@"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b", RegexOptions.Compiled);
            regexes[CPP.WORD] = new Regex(@"\b(public|private|static|const|import|package|class|function|default|throw|new|switch|case|var|else|if|return|null|for|while)\b");
            regexes[CPP.STRING] = new Regex(@"""((\\[^\n]|[^""\n])*)""");
        }

        public void Colourize()
        {
            editor.ClearStylesBuffer();
            editor.Range.ClearStyle(StyleIndex.All);
            editor.OnSyntaxHighlight(new TextChangedEventArgs(editor.Range));
        }

        public FastColoredTextBoxNS.Style GetStyle(int index)
        {
            if (index < 0 || index > 31 || styles[index] == null) return editor.DefaultStyle;
            else return styles[index];
        }

        public void SetStyleString(int index, string value, string type)
        {
            Style style = GetStyle(index);
            if (style is TextStyle)
            {
                TextStyle cast = style as TextStyle;
                switch (type)
                {
                    case "font":
                        // TODO: CANT ADJUST FOR STYLE?
                        break;
                }
            }
        }

        public void SetStyleInt(int index, int value, string type)
        {
            if (index < 1 || index > 31 || styles[index] == null) return;
            Style style = GetStyle(index);
            if (style is TextStyle)
            {
                TextStyle cast = style as TextStyle;
                Color color = DataConverter.BGRToColor(value);
                switch (type)
                {
                    case "fore":
                        TraceManager.Add("Fore set: " + index + "=" + value + ", type: " + type);
                        cast.ForeBrush = new SolidBrush(color);
                        break;
                    case "back":
                        cast.BackgroundBrush = new SolidBrush(color);
                        break;
                    case "italic":
                        if (value == 1) cast.FontStyle |= FontStyle.Italic;
                        else cast.FontStyle &= ~FontStyle.Italic;
                        break;
                    case "bold":
                        if (value == 1) cast.FontStyle |= FontStyle.Bold;
                        else cast.FontStyle &= ~FontStyle.Bold;
                        break;
                    case "visible":
                        cast.ForeBrush = new SolidBrush(Color.Transparent); // CHECK
                        cast.BackgroundBrush = new SolidBrush(Color.Transparent);
                        break;
                    case "readonly":
                        // TODO: CANT ADJUST FOR STYLE?
                        break;
                    case "hotspot":
                        // TODO: CANT ADJUST FOR STYLE?
                        break;
                    case "charset":
                        // TODO: CANT ADJUST FOR STYLE?
                        break;
                    case "underline":
                        // TODO: CANT ADJUST FOR STYLE?
                        break;
                    case "size":
                        // TODO: CANT ADJUST FOR STYLE?
                        break;
                    case "case":
                        // TODO: CANT ADJUST FOR STYLE?
                        break;
                    case "eol":
                        // TODO: CANT ADJUST FOR STYLE?
                        break;
                }
            }
        }

        public void HighlightSyntax(Range range)
        {
            // set options
            range.tb.CommentPrefix = "//";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '{';
            range.tb.RightBracket2 = '}';
            range.tb.LeftBracket3 = '[';
            range.tb.RightBracket3 = ']';
            //range.tb.AutoIndentCharsPatterns = @"^\s*[\w\.]+(\s\w+)?\s*(?<range>=)\s*(?<range>[^;]+);^\s*(case|default)\s*[^:]*(?<range>:)\s*(?<range>[^;]+);";
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;

            // set styles
            range.ClearStyle(styles);
            for (int i = 0; i < 32; i++) 
            {
                if (styles[i] != null) range.SetStyle(styles[i], regexes[i]);
            }

            // set folding markers
            range.ClearFoldingMarkers();
            range.SetFoldingMarkers("{", "}"); // bracket block
            range.SetFoldingMarkers(@"/\*", @"\*/"); // comment block
        }
    }

    internal static class CPP
    {
        public const int DEFAULT = 0;
        public const int COMMENT = 1;
        public const int COMMENTLINE = 2;
        public const int COMMENTDOC = 3;
        public const int NUMBER = 4;
        public const int WORD = 5;
        public const int STRING = 6;
        public const int CHARACTER = 7;
        public const int UUID = 8;
        public const int PREPROCESSOR = 9;
        public const int OPERATOR = 10;
        public const int IDENTIFIER = 11;
        public const int STRINGEOL = 12;
        public const int VERBATIM = 13;
        public const int REGEX = 14;
        public const int COMMENTLINEDOC = 15;
        public const int WORD2 = 16;
        public const int COMMENTDOCKEYWORD = 17;
        public const int COMMENTDOCKEYWORDERROR = 18;
        public const int GLOBALCLASS = 19;
        public const int STRINGRAW = 20;
        public const int TRIPLEVERBATIM = 21;
        public const int HASHQUOTEDSTRING = 22;
        public const int PREPROCESSORCOMMENT = 23;
        public const int WORD3 = 24;
        public const int WORD4 = 25;
        public const int WORD5 = 26;
        public const int GDEFAULT = 32;
        public const int LINENUMBER = 33;
        public const int BRACELIGHT = 34;
        public const int BRACEBAD = 35;
        public const int CONTROLCHAR = 36;
        public const int INDENTGUIDE = 37;
        public const int LASTPREDEFINED = 39;
    }
}
