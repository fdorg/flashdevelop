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
        private string lexer = "cpp";
        private FastColoredTextBox editor;
        private Regex[] regexes = new Regex[32];
        private Style[] styles = new Style[32];

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
            editor.VisibleRangeChangedDelayed += OnVisibleRangeChanged;
        }

        private void OnVisibleRangeChanged(Object sender, EventArgs e)
        {
            HighlightSyntax(editor.VisibleRange);
        }

        private void HighlightSyntax(Range range)
        {
            // set options
            if (this.lexer == "xml" || this.lexer == "html")
            {
                range.tb.CommentPrefix = null;
                range.tb.LeftBracket = '<';
                range.tb.RightBracket = '>';
                range.tb.LeftBracket2 = '(';
                range.tb.RightBracket2 = ')';
                range.tb.AutoIndentCharsPatterns = @"";
            }
            else
            {
                range.tb.CommentPrefix = "//";
                range.tb.LeftBracket = '(';
                range.tb.RightBracket = ')';
                range.tb.LeftBracket2 = '{';
                range.tb.RightBracket2 = '}';
                range.tb.LeftBracket3 = '[';
                range.tb.RightBracket3 = ']';
                range.tb.AutoIndentCharsPatterns = @"^\s*[\w\.]+(\s\w+)?\s*(?<range>=)\s*(?<range>[^;]+);^\s*(case|default)\s*[^:]*(?<range>:)\s*(?<range>[^;]+);";
            }
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;

            // set styles
            range.ClearStyle(styles);
            for (int i = 0; i < 32; i++)
            {
                if (styles[i] != null && regexes[i] != null)
                {
                    // xml and html comments can overwrite
                    if ((this.lexer == "xml" || this.lexer == "html") && i == (int)XML.COMMENT)
                    {
                        styles[i].AllowSeveralTextStyles = true;
                    }
                    // css strings can overwrite
                    if ((this.lexer == "css") && (i == (int)CSS.DOUBLESTRING || i == (int)CSS.SINGLESTRING))
                    {
                        styles[i].AllowSeveralTextStyles = true;
                    }
                    // cpp string and dockeywords can overwrite
                    if (this.lexer == "cpp" && (i == (int)CPP.STRING) || (i == (int)CPP.COMMENTDOCKEYWORD || i == (int)CPP.COMMENTDOCKEYWORDERROR))
                    {
                        styles[i].AllowSeveralTextStyles = true;
                    }
                    range.SetStyle(styles[i], regexes[i]);
                }
            }

            // set folding markers
            range.ClearFoldingMarkers();
            if (this.lexer == "xml" || this.lexer == "html")
            {
                range.SetFoldingMarkers("<!--", "-->"); // comment
                range.SetFoldingMarkers(@"(<(?!meta)\w+(?:)\w+.+(?<!\/)(?<!<\/\w+)>)", @"(</\w+.\w+>)"); // tag, not meta
            }
            else // Others
            {
                range.SetFoldingMarkers("{", "}"); // bracket
                range.SetFoldingMarkers(@"/\*", @"\*/"); // comment
            }
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

        public void SetLexer(String lexer)
        {
            if (string.IsNullOrEmpty(lexer)) return;
            this.lexer = lexer;
            switch (lexer)
            {
                case "cpp":
                    regexes = Regexes.CppRegexes;
                    break;
                case "css":
                    regexes = Regexes.CssRegexes;
                    break;
                case "xml":
                    regexes = Regexes.XmlRegexes;
                    break;
                case "html":
                    regexes = Regexes.HtmlRegexes;
                    break;
                case "properties":
                    regexes = Regexes.PropRegexes;
                    break;
            }
        }

        public void SetKeywords(int keywordSet, string keyWords)
        {
            string regex = "";
            int styleIndex = StyleFromKeywordSet(keywordSet);
            if (!string.IsNullOrEmpty(keyWords) && styleIndex > -1)
            {
                keyWords = Regex.Replace(keyWords, "\\s+", " ").Trim();
                if (this.lexer == "cpp" && (styleIndex == (int)CPP.COMMENTDOCKEYWORD || styleIndex == (int)CPP.COMMENTDOCKEYWORDERROR))
                {
                    // cpp doc keywords need @ char
                    regex = "@(" + keyWords.Replace(" ", "|") + ")";
                }
                else regex = @"\b(" + keyWords.Replace(" ", "|") + @")\b";
            }
            if (styleIndex > -1) regexes[styleIndex] = new Regex(regex);
        }

        private int StyleFromKeywordSet(int keywordSet)
        {
            switch (this.lexer)
            {
                case "css":
                    /*if (keywordSet == 0) return (int)CSS.IDENTIFIER;
                    else if (keywordSet == 1) return (int)CSS.PSEUDOCLASS;
                    else if (keywordSet == 2) return (int)CSS.IDENTIFIER2;
                    else if (keywordSet == 3) return (int)CSS.IDENTIFIER3;
                    else if (keywordSet == 5) return (int)CSS.EXTENDED_IDENTIFIER;*/
                    break;
                case "cpp":
                    if (keywordSet == 0) return (int)CPP.WORD;
                    else if (keywordSet == 1) return (int)CPP.WORD2;
                    else if (keywordSet == 2) return (int)CPP.COMMENTDOCKEYWORD;
                    else if (keywordSet == 3) return (int)CPP.GLOBALCLASS;
                    else if (keywordSet == 5) return (int)CPP.WORD3;
                    else if (keywordSet == 6) return (int)CPP.WORD4;
                    else if (keywordSet == 7) return (int)CPP.WORD5;
                    break;
                case "xml":
                    //if (keywordSet == 1) return (int)XML.J_KEYWORD;
                    //else if (keywordSet == 4) return (int)XML.PHP_WORD;
                    /*else*/ if (keywordSet == 5) return (int)XML.SGML_COMMAND;
                    break;
                case "html":
                    if (keywordSet == 0) return (int)HTML.SCRIPT;
                    //else if (keywordSet == 1) return (int)XML.J_KEYWORD;
                    //else if (keywordSet == 4) return (int)XML.PHP_WORD;
                    else if (keywordSet == 5) return (int)XML.SGML_COMMAND;
                    break;
            }
            return -1;
        }

    }

}
