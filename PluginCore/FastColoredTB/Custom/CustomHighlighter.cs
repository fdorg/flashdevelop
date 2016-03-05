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
        private int counter = 0;
        private string lexer = "cpp";
        private FastColoredTextBox editor;
        private Dictionary<int, int> mapping = new Dictionary<int, int>();
        private Regex[] regexes = new Regex[32];
        private Style[] styles = new Style[32];

        /// <summary>
        /// Constructor of the class
        /// </summary>
        public CustomHighlighter(FastColoredTextBox fctb)
        {
            this.editor = fctb;
            this.ResetStyles();
            this.editor.VisibleRangeChangedDelayed += this.OnVisibleRangeChanged;
        }

        /// <summary>
        /// Highlights code after delayed visible range change
        /// </summary>
        private void OnVisibleRangeChanged(Object sender, EventArgs e)
        {
            Range range = editor.VisibleRange;
            if (editor.Text.Length < 10240) range = editor.Range;
            this.HighlightSyntax(range);
        }

        /// <summary>
        /// Hightlights editor with current style and adjust params by lexer
        /// </summary>
        private void HighlightSyntax(Range range)
        {
            // set options
            if (this.lexer == "css")
            {
                range.tb.CommentPrefix = null;
                range.tb.LeftBracket = '{';
                range.tb.RightBracket = '}';
                range.tb.AutoIndentCharsPatterns = @"";
            }
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
                int style = this.GetStyleIndex(i);
                if (this.styles[i] != null && this.regexes[style] != null)
                {
                    // xml and html comments can overwrite
                    if ((this.lexer == "xml" || this.lexer == "html") && style == (int)XML.COMMENT)
                    {
                        this.styles[i].AllowSeveralTextStyles = true;
                    }
                    // css strings and comments can overwrite
                    if ((this.lexer == "css") && (style == (int)CSS.DOUBLESTRING || style == (int)CSS.SINGLESTRING || style == (int)CSS.COMMENT))
                    {
                        this.styles[i].AllowSeveralTextStyles = true;
                    }
                    // cpp string and dockeywords can overwrite
                    if (this.lexer == "cpp" && (style == (int)CPP.STRING) || (style == (int)CPP.COMMENTDOCKEYWORD || style == (int)CPP.COMMENTDOCKEYWORDERROR))
                    {
                        this.styles[i].AllowSeveralTextStyles = true;
                    }
                    range.SetStyle(this.styles[i], this.regexes[style]);
                }
            }

            // set folding markers
            range.ClearFoldingMarkers();
            if (this.lexer == "xml" || this.lexer == "html")
            {
                //range.SetFoldingMarkers("<!--", "-->"); // comment
                //range.SetFoldingMarkers(@"(<(?!meta|link|base)\w+(?:)\w+.+(?<!\/)(?<!<\/\w+)>)", @"(</\w+.\w+>)"); // tag, not meta
            }
            else // Others
            {
                //range.SetFoldingMarkers("{", "}"); // bracket
                //range.SetFoldingMarkers(@"/\*", @"\*/"); // comment
                //range.SetFoldingMarkers(@"////{", @"////}"); // comment bracket
            }
        }

        /// <summary>
        /// Gets a style by index or null
        /// </summary>
        public FastColoredTextBoxNS.Style GetStyle(int index)
        {
            int mapped = this.GetStyleIndex(index);
            return mapped > -1 ? this.styles[mapped] : null;
        }

        /// <summary>
        /// Gets and maps style index of a requested style index
        /// </summary>
        public Int32 GetStyleIndex(int index)
        {
            // ignore negative and do not map common styles
            if (index < 0 || (index > 31 && index < 40)) return -1;
            if (!this.mapping.ContainsKey(index))
            {
                if (this.counter <= 31)
                {
                    this.mapping.Add(index, this.counter);
                    this.counter++;
                }
                else return -1;
            }
            return this.mapping[index];
        }

        /// <summary>
        /// Sets string value of a style
        /// </summary>
        public void SetStyleString(int index, string value, string type)
        {
            Style style = this.GetStyle(index);
            if (style != null && style is TextStyle)
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

        /// <summary>
        /// Sets integer value of a style
        /// </summary>
        public void SetStyleInt(int index, int value, string type)
        {
            Style style = this.GetStyle(index);
            if (style != null && style is TextStyle)
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

        /// <summary>
        /// Activates the colorization of the editor
        /// </summary>
        public void Colourize()
        {
            //this.OnVisibleRangeChanged(editor, new EventArgs());
        }

        /// <summary>
        /// Applies the common editor styles
        /// </summary>
        /// <param name="lang"></param>
        public void ApplyBaseStyles(ScintillaNet.Configuration.Language lang)
        {
            foreach (UseStyle style in lang.usestyles)
            {
                if (style.key == 0) // Default
                {
                    FontStyle fontStyle = FontStyle.Regular;
                    if (style.IsBold) fontStyle |= FontStyle.Bold;
                    if (style.IsItalics) fontStyle |= FontStyle.Italic;
                    this.editor.Font = new Font(style.FontName, style.FontSize, FontStyle.Regular);
                    Color fore = DataConverter.BGRToColor(style.ForegroundColor);
                    Color back = DataConverter.BGRToColor(style.BackgroundColor);
                    this.editor.DefaultStyle = new FastColoredTextBoxNS.TextStyle(new SolidBrush(fore), new SolidBrush(back), fontStyle);
                    this.editor.BackColor = this.editor.PaddingBackColor = this.editor.IndentBackColor = back;
                    this.editor.FoldingIndicatorColor = Color.Lime; // TODO?
                }
                else if (style.key == (Int32)ScintillaNet.Enums.StylesCommon.LineNumber)
                {
                    this.editor.LineNumberColor = DataConverter.BGRToColor(style.ForegroundColor);
                }
                else if (style.key == (Int32)ScintillaNet.Enums.StylesCommon.IndentGuide)
                {
                    this.editor.ServiceLinesColor = DataConverter.BGRToColor(style.ForegroundColor);
                }
                else if (style.key == (Int32)ScintillaNet.Enums.StylesCommon.BraceLight)
                {
                    Color color = DataConverter.BGRToColor(style.BackgroundColor, 125);
                    this.editor.BracketsStyle = new FastColoredTextBoxNS.MarkerStyle(new SolidBrush(color));
                    this.editor.BracketsStyle2 = new FastColoredTextBoxNS.MarkerStyle(new SolidBrush(color));
                    this.editor.BracketsStyle3 = new FastColoredTextBoxNS.MarkerStyle(new SolidBrush(color));
                }
                else if (style.key == (Int32)ScintillaNet.Enums.StylesCommon.Default)
                {
                    if (lang.editorstyle.ColorizeMarkerBack)
                    {
                        Color fore = DataConverter.BGRToColor(style.ForegroundColor);
                        this.editor.ServiceColors.ServiceAreaBackColor = fore;
                    }
                    else this.editor.ServiceColors.ServiceAreaBackColor = Color.Empty;
                }
            }
        }

        /// <summary>
        /// Resets styles and related regexes
        /// </summary>
        public void ResetStyles()
        {
            this.styles = new Style[32];
            this.regexes = new Regex[32];
            for (int i = 0; i < 32; i++)
            {
                this.regexes[i] = new Regex("");
                this.styles[i] = new TextStyle(null, null, FontStyle.Regular);
            }
        }

        /// <summary>
        /// Sets the lexer, resets styling and sets regexes
        /// </summary>
        public void SetLexer(String lexer)
        {
            if (string.IsNullOrEmpty(lexer)) return;
            this.ResetStyles();
            this.lexer = lexer;
            this.mapping.Clear();
            this.counter = 0;
            switch (lexer)
            {
                case "cpp":
                    this.regexes = Regexes.CppRegexes;
                    break;
                case "css":
                    this.regexes = Regexes.CssRegexes;
                    break;
                case "xml":
                    this.regexes = Regexes.XmlRegexes;
                    break;
                case "html":
                    this.regexes = Regexes.HtmlRegexes;
                    break;
                case "properties":
                    this.regexes = Regexes.PropRegexes;
                    break;
            }
            this.Colourize();
        }

        /// <summary>
        /// Sets the keyword regexes to styles
        /// </summary>
        public void SetKeywords(int keywordSet, string keyWords)
        {
            string regex = "";
            int style = this.GetKeywordStyle(keywordSet);
            int mapped = this.GetStyleIndex(style);
            if (!string.IsNullOrEmpty(keyWords) && style > -1)
            {
                keyWords = Regex.Replace(keyWords, "\\s+", " ").Trim();
                if (this.lexer == "cpp" && (style == (int)CPP.COMMENTDOCKEYWORD || style == (int)CPP.COMMENTDOCKEYWORDERROR))
                {
                    // cpp doc keywords need @ char
                    regex = "@(" + keyWords.Replace(" ", "|") + ")";
                }
                else regex = @"\b(" + keyWords.Replace(" ", "|") + @")\b";
            }
            if (mapped > -1)
            {
                this.regexes[mapped] = new Regex(regex);
            }
        }

        /// <summary>
        /// Gets the style for a keyword set
        /// </summary>
        private int GetKeywordStyle(int keywordSet)
        {
            switch (this.lexer)
            {
                case "css":
                    if (keywordSet == 0) return (int)CSS.IDENTIFIER;
                    else if (keywordSet == 1) return (int)CSS.PSEUDOCLASS;
                    else if (keywordSet == 2) return (int)CSS.IDENTIFIER2;
                    else if (keywordSet == 3) return (int)CSS.IDENTIFIER3;
                    else if (keywordSet == 5) return (int)CSS.EXTENDED_IDENTIFIER;
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
                    /*if (keywordSet == 1) return (int)XML.J_KEYWORD;
                    else if (keywordSet == 4) return (int)XML.PHP_WORD;
                    else*/ if (keywordSet == 5) return (int)XML.SGML_COMMAND;
                    break;
                case "html":
                    /*if (keywordSet == 0) return (int)HTML.SCRIPT;
                    else if (keywordSet == 1) return (int)XML.J_KEYWORD;
                    else if (keywordSet == 4) return (int)XML.PHP_WORD;
                    else*/ if (keywordSet == 5) return (int)XML.SGML_COMMAND;
                    break;
            }
            return -1;
        }

    }

}
