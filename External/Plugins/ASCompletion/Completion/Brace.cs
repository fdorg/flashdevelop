using System;

namespace ASCompletion.Completion
{
    /// <summary>
    /// Represents a set of brace characters used for automatic closing braces.
    /// </summary>
    [Serializable]
    public sealed class Brace
    {
        private string name;
        private char open;
        private char close;
        private Rule[] rules;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        
        public char Open
        {
            get { return open; }
            set { open = value; }
        }
        
        public char Close
        {
            get { return close; }
            set { close = value; }
        }

        public Rule[] Rules
        {
            get { return rules; }
            set { rules = value ?? new Rule[0]; }
        }
        
        /// <summary>
        /// Creates an instance of <see cref="Brace"/>.
        /// </summary>
        public Brace(string name, char open, char close, Rule[] rules)
        {
            Name = name;
            Open = open;
            Close = close;
            Rules = rules;
        }
        
        /// <summary>
        /// Returns whether this brace should close for the specified condition.
        /// </summary>
        public bool ShouldAutoClose(char charBefore, byte styleBefore, char charAfter, byte styleAfter)
        {
            for (int i = 0; i < rules.Length; i++)
            {
                if (rules[i].Matches(charBefore, styleBefore, charAfter, styleAfter))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a <see cref="string"/> object that represents the current <see cref="Brace"/> object.
        /// </summary>
        public override string ToString()
        {
            return open + " " + name +" " + close;
        }

        [Serializable]
        public sealed class Rule
        {
            bool notAfterChars;
            string afterChars;
            Logic logic1;
            bool notAfterStyles;
            Style[] afterStyles;
            Logic logic2;
            bool notBeforeChars;
            string beforeChars;
            Logic logic3;
            bool notBeforeStyles;
            Style[] beforeStyles;

            public Rule()
                :this(null, null, null, null, null, null, null, null, null, null, null)
            {

            }

            public Rule(bool? notAfterChars, string afterChars, Logic? logic1, bool? notAfterStyles, Style[] afterStyles, Logic? logic2, bool? notBeforeChars, string beforeChars, Logic? logic3, bool? notBeforeStyles, Style[] beforeStyles)
            {
                NotAfterChars   = notAfterChars ?? false;
                AfterChars      = afterChars;
                Logic1          = logic1 ?? Logic.Or;
                NotAfterStyles  = notAfterStyles ?? false;
                AfterStyles     = afterStyles;
                Logic2          = logic2 ?? Logic.Or;
                NotBeforeChars  = notBeforeChars ?? false;
                BeforeChars     = beforeChars;
                Logic3          = logic3 ?? Logic.Or;
                NotBeforeStyles = notBeforeStyles ?? false;
                BeforeStyles    = beforeStyles;
            }

            public bool NotAfterChars
            {
                get { return notAfterChars; }
                set { notAfterChars = value; }
            }

            public string AfterChars
            {
                get { return Escape(afterChars); }
                set { afterChars = Unescape(value ?? ""); }
            }

            public Logic Logic1
            {
                get { return logic1; }
                set { logic1 = value; }
            }

            public bool NotAfterStyles
            {
                get { return notAfterStyles; }
                set { notAfterStyles = value; }
            }

            public Style[] AfterStyles
            {
                get { return afterStyles; }
                set { afterStyles = value ?? new Style[0]; }
            }

            public Logic Logic2
            {
                get { return logic2; }
                set { logic2 = value; }
            }

            public bool NotBeforeChars
            {
                get { return notBeforeChars; }
                set { notBeforeChars = value; }
            }

            public string BeforeChars
            {
                get { return Escape(beforeChars); }
                set { beforeChars = Unescape(value ?? ""); }
            }

            public Logic Logic3
            {
                get { return logic3; }
                set { logic3 = value; }
            }

            public bool NotBeforeStyles
            {
                get { return notBeforeStyles; }
                set { notBeforeStyles = value; }
            }

            public Style[] BeforeStyles
            {
                get { return beforeStyles; }
                set { beforeStyles = value ?? new Style[0]; }
            }

            private static string Escape(string value)
            {
                return value.Replace("\\", @"\\").Replace("\t", @"\t").Replace("\r", @"\r").Replace("\n", @"\n");
            }

            private static string Unescape(string value)
            {
                return value.Replace(@"\\", "\\").Replace(@"\t", "\t").Replace(@"\r", "\r").Replace(@"\n", "\n");
            }

            private static bool StringCheck(string str, char c, bool exclude)
            {
                return (str.IndexOf(c) == -1) == exclude;
            }

            private static bool ArrayCheck(Style[] arr, byte s, bool exclude)
            {
                return (Array.IndexOf(arr, (Style) s) == -1) == exclude;
            }

            public Rule Clone()
            {
                return new Rule(notAfterChars, afterChars, logic1, notAfterStyles, afterStyles, logic2, notBeforeChars, beforeChars, logic3, notBeforeStyles, beforeStyles);
            }

            public bool Matches(char charBefore, byte styleBefore, char charAfter, byte styleAfter)
            {
                bool match = StringCheck(afterChars, charBefore, notAfterChars);
                if (logic1 == Logic.Or)
                {
                    match |= ArrayCheck(afterStyles, styleBefore, notAfterStyles);
                }
                else
                {
                    match &= ArrayCheck(afterStyles, styleBefore, notAfterStyles);
                }
                if (logic2 == Logic.Or)
                {
                    match |= StringCheck(beforeChars, charAfter, notBeforeChars);
                }
                else
                {
                    match &= StringCheck(beforeChars, charAfter, notBeforeChars);
                }
                if (logic1 == Logic.Or)
                {
                    match |= ArrayCheck(beforeStyles, styleAfter, notBeforeStyles);
                }
                else
                {
                    match &= ArrayCheck(beforeStyles, styleAfter, notBeforeStyles);
                }
                return match;
            }
        }

        /// <summary>
        /// Defines a set of logic values used for <see cref="Brace"/>.
        /// </summary>
        [Serializable]
        public enum Logic : byte
        {
            /// <summary>
            /// The logical OR operator.
            /// </summary>
            Or,

            /// <summary>
            /// The logical AND operator.
            /// </summary>
            And
        }
    }

    /// <summary>
    /// Defines style values used for <see cref="Brace"/>.
    /// </summary>
    [Serializable]
    public enum Style : byte
    {
        Default = 0,
        Comment = 1,
        CommentLine = 2,
        CommentDoc = 3,
        Number = 4,
        Constant = 5,
        String = 6,
        Character = 7,
        Uuid = 8,
        Preprocessor = 9,
        Operator = 10,
        Identifier = 11,
        StringEOL = 12,
        Verbatim = 13,
        Regex = 14,
        CommentLineDoc = 15,
        Type = 16,
        CommentDocKeyword = 17,
        CommentDocKeywordError = 18,
        Keyword = 19,
        //STRINGRAW = 20,
        //TRIPLEVERBATIM = 21,
        //HASHQUOTEDSTRING = 22,
        //PREPROCESSORCOMMENT = 23,
        Attribute = 24,
        SpecialWord = 25,
        SpecialKeyword = 26,
        //GDEFAULT = 32,
        //LINENUMBER = 33,
        //BRACELIGHT = 34,
        //BRACEBAD = 35,
        //CONTROLCHAR = 36,
        //INDENTGUIDE = 37,
        //LASTPREDEFINED = 39
    }
}
