using System;
using System.Text.RegularExpressions;
using PluginCore;
using Array = System.Array;

namespace ASCompletion.Completion
{
    /// <summary>
    /// Represents a set of brace characters used for automatic closing braces.
    /// </summary>
    [Serializable]
    public sealed class Brace
    {
        string name;
        char open;
        char close;
        bool addSpace;
        bool ignoreWhitespace;
        Rule[] rules;

        public string Name
        {
            get => name;
            set => name = value;
        }
        
        public char Open
        {
            get => open;
            set => open = value;
        }
        
        public char Close
        {
            get => close;
            set => close = value;
        }

        public bool AddSpace
        {
            get => addSpace;
            set => addSpace = value;
        }

        public bool IgnoreWhitespace
        {
            get => ignoreWhitespace;
            set => ignoreWhitespace = value;
        }

        public Rule[] Rules
        {
            get => rules;
            set => rules = value ?? Array.Empty<Rule>();
        }
        
        /// <summary>
        /// Creates an instance of <see cref="Brace"/>.
        /// </summary>
        public Brace(string name, char open, char close, bool addSpace, bool ignoreWhitespace, Rule[] rules)
        {
            Name = name;
            Open = open;
            Close = close;
            AddSpace = addSpace;
            IgnoreWhitespace = ignoreWhitespace;
            Rules = rules;
        }
        
        /// <summary>
        /// Returns whether this brace should open with the matching close brace automatically inserted.
        /// </summary>
        public bool ShouldOpen(char charBefore, byte styleBefore, char charAfter, byte styleAfter)
        {
            foreach (var it in rules)
            {
                if (it.Matches(charBefore, styleBefore, charAfter, styleAfter))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns whether this brace should close with the matching close brace overwritten.
        /// </summary>
        public bool ShouldClose(int currentPosition, int nextPosition)
        {
            return ignoreWhitespace || currentPosition == nextPosition;
        }

        /// <summary>
        /// Returns whether this brace should remove the matching close brace.
        /// </summary>
        public bool ShouldRemove(int currentPosition, int nextPosition)
        {
            return ignoreWhitespace || currentPosition == nextPosition;
        }

        /// <summary>
        /// Returns a <see cref="string"/> object that represents the current <see cref="Brace"/> object.
        /// </summary>
        public override string ToString()
        {
            return open + " " + name + " " + close;
        }

        [Serializable]
        public sealed class Rule
        {
            bool notAfterChars;
            Regex afterChars;
            bool notAfterStyles;
            Style[] afterStyles;
            bool notBeforeChars;
            Regex beforeChars;
            bool notBeforeStyles;
            Style[] beforeStyles;
            Logic logic;

            public Rule()
            {
                notAfterChars   = false;
                afterChars      = null;
                notAfterStyles  = false;
                afterStyles     = null;
                notBeforeChars  = false;
                beforeChars     = null;
                notBeforeStyles = false;
                BeforeStyles    = null;
                logic           = 0;
            }

            public Rule(bool? notAfterChars, string afterChars, bool? notAfterStyles, Style[] afterStyles, bool? notBeforeChars, string beforeChars, bool? notBeforeStyles, Style[] beforeStyles, Logic? logic)
            {
                Logic = logic ?? 0;
                NotAfterChars   = notAfterChars ?? this.logic != 0;
                AfterChars      = afterChars;
                NotAfterStyles  = notAfterStyles ?? this.logic != 0;
                AfterStyles     = afterStyles;
                NotBeforeChars  = notBeforeChars ?? this.logic != 0;
                BeforeChars     = beforeChars;
                NotBeforeStyles = notBeforeStyles ?? this.logic != 0;
                BeforeStyles    = beforeStyles;
            }

            public bool NotAfterChars
            {
                get => notAfterChars;
                set => notAfterChars = value;
            }

            public string AfterChars
            {
                get => FromRegex(afterChars);
                set => afterChars = ToRegex(value);
            }

            public bool NotAfterStyles
            {
                get => notAfterStyles;
                set => notAfterStyles = value;
            }

            public Style[] AfterStyles
            {
                get => afterStyles ?? Array.Empty<Style>();
                set => afterStyles = value.IsNullOrEmpty() ? null : value;
            }
            
            public bool NotBeforeChars
            {
                get => notBeforeChars;
                set => notBeforeChars = value;
            }

            public string BeforeChars
            {
                get => FromRegex(beforeChars);
                set => beforeChars = ToRegex(value);
            }
            
            public bool NotBeforeStyles
            {
                get => notBeforeStyles;
                set => notBeforeStyles = value;
            }

            public Style[] BeforeStyles
            {
                get => beforeStyles ?? Array.Empty<Style>();
                set => beforeStyles = value.IsNullOrEmpty() ? null : value;
            }

            public Logic Logic
            {
                get => logic;
                set => logic = value;
            }

            static Regex ToRegex(string value)
            {
                if (string.IsNullOrEmpty(value)) return null;
                return new Regex("[" + Escape(value) + "]", RegexOptions.Compiled | RegexOptions.Singleline);
            }

            static string FromRegex(Regex value)
            {
                if (value is null) return string.Empty;
                var str = value.ToString();
                return Unescape(str.Substring(1, str.Length - 2));
            }

            static string Escape(string value) => value.Replace("(", @"\(").Replace(")", @"\)").Replace("[", @"\[").Replace("]", @"\]").Replace("{", @"\{").Replace("}", @"\}");

            static string Unescape(string value) => value.Replace(@"\(", "(").Replace(@"\)", ")").Replace(@"\[", "[").Replace(@"\]", "]").Replace(@"\{", "{").Replace(@"\}", "}");

            static bool RegexCheck(Regex regex, char c, bool exclude) => (regex?.IsMatch(c.ToString()) ^ exclude) ?? exclude;

            static bool ArrayCheck(Style[] array, byte s, bool exclude)
            {
                if (array is null) return exclude;
                return Array.IndexOf(array, (Style) s) >= 0 ^ exclude;
            }

            public Rule Clone()
            {
                return new Rule
                {
                    notAfterChars = notAfterChars,
                    afterChars = afterChars,
                    notAfterStyles = notAfterStyles,
                    afterStyles = afterStyles,
                    notBeforeChars = notBeforeChars,
                    beforeChars = beforeChars,
                    notBeforeStyles = notBeforeStyles,
                    beforeStyles = beforeStyles,
                    logic = logic
                };
            }

            public bool Matches(char charBefore, byte styleBefore, char charAfter, byte styleAfter)
            {
                if (logic == Logic.Or)
                {
                    return RegexCheck(afterChars, charBefore, notAfterChars)
                        || ArrayCheck(afterStyles, styleBefore, notAfterStyles)
                        || RegexCheck(beforeChars, charAfter, notBeforeChars)
                        || ArrayCheck(beforeStyles, styleAfter, notBeforeStyles);
                }

                return RegexCheck(afterChars, charBefore, notAfterChars)
                       && ArrayCheck(afterStyles, styleBefore, notAfterStyles)
                       && RegexCheck(beforeChars, charAfter, notBeforeChars)
                       && ArrayCheck(beforeStyles, styleAfter, notBeforeStyles);
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