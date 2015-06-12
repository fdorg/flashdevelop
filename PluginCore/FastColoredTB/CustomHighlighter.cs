using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using ScintillaNet.Enums;
using PluginCore.Managers;
using PluginCore.Utilities;

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
        * DisabledColor, CaretColor, ServiceLinesColor, FoldingIndicatorColor, ServiceColors (has 6), 
        */

        public CustomHighlighter(FastColoredTextBox fctb)
        {
            editor = fctb;
            //editor.AllowSeveralTextStyleDrawing = true;
            styles = new Style[32];
            regexes = new Regex[32];
            for (int i = 0; i < 32; i++)
            {
                regexes[i] = new Regex("");
                styles[i] = new TextStyle(new SolidBrush(Color.Blue), null, System.Drawing.FontStyle.Regular);
            }
            // Try few...
            regexes[1] = new Regex(@"//.*$", RegexOptions.Multiline | RegexOptions.Compiled); // COMMENT
            regexes[2] = new Regex(@"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline | RegexOptions.Compiled); // COMMENTLINE
            //
            regexes[4] = new Regex(@"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b", RegexOptions.Compiled); // NUMBER
            regexes[5] = new Regex(@"\b(public|private|static|const|import|package|class|function|default|throw|new|switch|case|var|else|if|return|null|for|while)\b"); // WORD
            regexes[6] = new Regex(@"""((\\[^\n]|[^""\n])*)"""); // STRING
            editor.TextChanged += OnTextChangedDelayed;
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

        private void OnTextChangedDelayed(Object sender, TextChangedEventArgs e)
        {
            Range range = editor.Range;

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

            ((TextStyle)GetStyle((int)ScintillaNet.Lexers.CPP.COMMENT)).ForeBrush = new SolidBrush(Color.Green);
            ((TextStyle)GetStyle((int)ScintillaNet.Lexers.CPP.COMMENTDOC)).ForeBrush = new SolidBrush(Color.Green);
            ((TextStyle)GetStyle((int)ScintillaNet.Lexers.CPP.NUMBER)).ForeBrush = new SolidBrush(Color.Orange);

            // set folding markers
            range.ClearFoldingMarkers();
            range.SetFoldingMarkers("{", "}"); // bracket block
            range.SetFoldingMarkers(@"/\*", @"\*/"); // comment block
        }

    }

}
