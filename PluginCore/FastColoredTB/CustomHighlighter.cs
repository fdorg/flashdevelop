using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace FastColoredTextBoxNS
{
    public static class CustomHighlighter
    {
        private static FastColoredTextBox editor;
        private static Regex[] regexes = new Regex[32];
        private static Style[] styles = new Style[32];

        /*
        DEFAULT = 0,
        COMMENT = 1,
        COMMENTLINE = 2,
        COMMENTDOC = 3,
        NUMBER = 4,
        WORD = 5,
        STRING = 6,
        CHARACTER = 7,
        UUID = 8,
        PREPROCESSOR = 9,
        OPERATOR = 10,
        IDENTIFIER = 11,
        STRINGEOL = 12,
        VERBATIM = 13,
        REGEX = 14,
        COMMENTLINEDOC = 15,
        WORD2 = 16,
        COMMENTDOCKEYWORD = 17,
        COMMENTDOCKEYWORDERROR = 18,
        GLOBALCLASS = 19,
        STRINGRAW = 20,
        TRIPLEVERBATIM = 21,
        HASHQUOTEDSTRING = 22,
        PREPROCESSORCOMMENT = 23,
        WORD3 = 24,
        WORD4 = 25,
        WORD5 = 26,
        GDEFAULT = 32,
        LINENUMBER = 33,
        BRACELIGHT = 34,
        BRACEBAD = 35,
        CONTROLCHAR = 36,
        INDENTGUIDE = 37,
        LASTPREDEFINED = 39
        */

        public static void Init(FastColoredTextBox fctb)
        {
            editor = fctb;
            styles = new Style[32];
            regexes = new Regex[32];
            for (int i = 0; i < 32; i++)
            {
                regexes[i] = new Regex("");
                styles[i] = new TextStyle(new SolidBrush(SystemColors.Highlight), null, System.Drawing.FontStyle.Regular);
            }
            editor.TextChanged += OnTextChangedDelayed;
        }

        private static void OnTextChangedDelayed(Object sender, TextChangedEventArgs e)
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
            range.tb.AutoIndentCharsPatterns = @"^\s*[\w\.]+(\s\w+)?\s*(?<range>=)\s*(?<range>[^;]+);^\s*(case|default)\s*[^:]*(?<range>:)\s*(?<range>[^;]+);";
            range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;

            // set styles
            range.ClearStyle(styles);
            range.SetStyle(styles[31], @"\b(class|var|else|if)\b");

            // set folding markers
            range.ClearFoldingMarkers();
            range.SetFoldingMarkers("{", "}"); //allow to collapse brackets block
            range.SetFoldingMarkers(@"//{", @"//}"); //allow to collapse #region blocks
            range.SetFoldingMarkers(@"/\*", @"\*/"); //allow to collapse comment block
        }

    }

}

