using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet.Configuration;
using ScintillaNet.Lexers;
using ScintillaNet.Enums;

namespace FastColoredTextBoxNS
{
    public class CustomHighlighter
    {
        private FastColoredTextBox editor;
        private Regex[] regexes = new Regex[32];
        private Style[] styles = new Style[32];

        /**
        * Built in styles:
        * DefaultStyle, SelectionStyle, FoldedBlockStyle, BracketsStyle, BracketsStyle2, BracketsStyle3
        * Built in colors:
        * BackColor (BackBrush too), ForeColor, CurrentLineColor, ChangedLineColor, BookmarkColor, LineNumberColor, IndentBackColor, PaddingBackColor, 
        * EdgeColor, DisabledColor, CaretColor, ServiceLinesColor, FoldingIndicatorColor, ServiceColors (has 6)
        */

        public CustomHighlighter(FastColoredTextBox fctb)
        {
            editor = fctb;
            styles = new Style[32];
            regexes = new Regex[32];
            for (int i = 0; i < 32; i++)
            {
                regexes[i] = new Regex("");
                styles[i] = new TextStyle(new SolidBrush(Color.Blue), null, System.Drawing.FontStyle.Regular);
            }
            // Try few...
            regexes[(int)CPP.COMMENT] = new Regex(@"//.*$", RegexOptions.Multiline | RegexOptions.Compiled);
            regexes[(int)CPP.COMMENTLINE] = new Regex(@"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline | RegexOptions.Compiled);
            //
            regexes[(int)CPP.NUMBER] = new Regex(@"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b", RegexOptions.Compiled);
            regexes[(int)CPP.WORD] = new Regex(@"\b(public|private|static|const|import|package|class|function|default|throw|new|switch|case|var|else|if|return|null|for|while)\b");
            regexes[(int)CPP.STRING] = new Regex(@"""""|"".*?[^\\]""", RegexOptions.Compiled);
            regexes[(int)CPP.CHARACTER] = new Regex(@"''|'.*?[^\\]'", RegexOptions.Compiled);
            regexes[(int)CPP.PREPROCESSOR] = new Regex(@"#(if|elseif|else|end|error)\b");
            //
            regexes[(int)CPP.COMMENTDOCKEYWORD] = new Regex(@"\*?@\S+", RegexOptions.Singleline | RegexOptions.Compiled);
            editor.VisibleRangeChanged += OnVisibleRangeChanged;
        }

        public void Colourize()
        {
            this.OnVisibleRangeChanged(editor, new EventArgs());
        }

        public void ApplyBaseStyles(ScintillaNet.Configuration.Language lang)
        {
            foreach (UseStyle style in lang.usestyles)
            {
                if (style.key == 0) // Default
                {
                    FontStyle fontStyle = FontStyle.Regular;
                    if (style.IsBold) fontStyle |= FontStyle.Bold;
                    if (style.IsItalics) fontStyle |= FontStyle.Italic;
                    editor.Font = new Font(style.FontName, style.FontSize, FontStyle.Regular);
                    Color fore = DataConverter.BGRToColor(style.ForegroundColor);
                    Color back = DataConverter.BGRToColor(style.BackgroundColor);
                    editor.DefaultStyle = new FastColoredTextBoxNS.TextStyle(new SolidBrush(fore), new SolidBrush(back), fontStyle);
                    editor.BackColor = editor.PaddingBackColor = editor.IndentBackColor = back;
                    editor.FoldingIndicatorColor = Color.Lime; // TODO?
                }
                else if (style.key == (Int32)ScintillaNet.Enums.StylesCommon.LineNumber)
                {
                    editor.LineNumberColor = DataConverter.BGRToColor(style.ForegroundColor);
                }
                else if (style.key == (Int32)ScintillaNet.Enums.StylesCommon.IndentGuide)
                {
                    editor.ServiceLinesColor = DataConverter.BGRToColor(style.ForegroundColor);
                }
                else if (style.key == (Int32)ScintillaNet.Enums.StylesCommon.BraceLight)
                {
                    Color color = DataConverter.BGRToColor(style.BackgroundColor, 125);
                    editor.BracketsStyle = new FastColoredTextBoxNS.MarkerStyle(new SolidBrush(color));
                    editor.BracketsStyle2 = new FastColoredTextBoxNS.MarkerStyle(new SolidBrush(color));
                    editor.BracketsStyle3 = new FastColoredTextBoxNS.MarkerStyle(new SolidBrush(color));
                }
            }
        }

        public void SetKeywords(int keywordSet, string keyWords)
        {
            string regex = "";
            int styleIndex = StyleFromKeywordSet(keywordSet);
            if (!string.IsNullOrEmpty(keyWords))
            {
                keyWords = Regex.Replace(keyWords, "\\s+", " ").Trim();
                if (IsDocKeyWord(styleIndex)) regex = "@(" + keyWords.Replace(" ", "|") + ")";
                else regex = @"\b(" + keyWords.Replace(" ", "|") + @")\b";
            }
            if (styleIndex >= 0) regexes[styleIndex] = new Regex(regex);
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

        private void OnVisibleRangeChanged(Object sender, EventArgs e)
        {
            Range range = editor.VisibleRange;

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
                if (styles[i] != null)
                {
                    // string and dockeyword can overwrite
                    if (IsString(i) || IsDocKeyWord(i)) styles[i].AllowSeveralTextStyles = true;
                    range.SetStyle(styles[i], regexes[i]);
                }
            }

            // set folding markers
            range.ClearFoldingMarkers();
            range.SetFoldingMarkers("{", "}"); // bracket block
            range.SetFoldingMarkers(@"/\*", @"\*/"); // comment block
        }

        private int StyleFromKeywordSet(int keywordSet)
        {
            switch (keywordSet)
            {
                case KeywordSet.PRIMARY: return (int)CPP.WORD;
                case KeywordSet.SECONDARY: return (int)CPP.WORD2;
                case KeywordSet.DOCUMENTATION: return (int)CPP.COMMENTDOCKEYWORD;
                case KeywordSet.GLOBAL: return (int)CPP.GLOBALCLASS;
                case KeywordSet.EXTENDED1: return (int)CPP.WORD3;
                case KeywordSet.EXTENDED2: return (int)CPP.WORD4;
                case KeywordSet.EXTENDED3: return (int)CPP.WORD5;
            }
            return -1;
        }

        private bool IsDocKeyWord(int style)
        {
            return style == (int)CPP.COMMENTDOCKEYWORD || style == (int)CPP.COMMENTDOCKEYWORDERROR; // CPP
        }

        private bool IsString(int style)
        {
            return style == (int)CPP.STRING; // CPP
        }

    }

    internal static class KeywordSet
    {
        public const int PRIMARY = 0;
        public const int SECONDARY = 1;
        public const int DOCUMENTATION = 2;
        public const int GLOBAL = 3;
        public const int PREPOCESSOR = 4; // unused
        public const int EXTENDED1 = 5;
        public const int EXTENDED2 = 6;
        public const int EXTENDED3 = 7;
    }

}
