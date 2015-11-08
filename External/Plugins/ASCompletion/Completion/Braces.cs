using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ASCompletion.Completion
{
    [Serializable]
    public class Braces
    {
        char opening, closing;
        string afterChars, beforeChars;
        Style[] afterStyles, beforeStyles;
        Mode acMode, asMode, bcMode, bsMode;
        Logic logic;

        [Category("Brace Character")]
        public char Opening { get { return opening; } set { opening = value; } }

        [Category("Brace Character")]
        public char Closing { get { return closing; } set { closing = value; } }

        [Category("Trigger Auto Close")]
        public string AfterChars { get { return afterChars; } set { afterChars = value; } }

        [Category("Trigger Auto Close")]
        public Mode AfterCharsMode { get { return acMode; } set { acMode = value; } }

        [Category("Trigger Auto Close")]
        public Style[] AfterStyles { get { return afterStyles; } set { afterStyles = value; } }

        [Category("Trigger Auto Close")]
        public Mode AfterStylesMode { get { return asMode; } set { asMode = value; } }

        [Category("Trigger Auto Close")]
        public string BeforeChars { get { return beforeChars; } set { beforeChars = value; } }

        [Category("Trigger Auto Close")]
        public Mode BeforeCharsMode { get { return bcMode; } set { bcMode = value; } }

        [Category("Trigger Auto Close")]
        public Style[] BeforeStyles { get { return beforeStyles; } set { beforeStyles = value; } }

        [Category("Trigger Auto Close")]
        public Mode BeforeStylesMode { get { return bsMode; } set { bsMode = value; } }

        [Category("Trigger Auto Close")]
        public Logic TriggerLogic { get { return logic; } set { logic = value; } }

        [Browsable(false)]
        public bool IsValid => Opening != '\0' && Closing != '\0';

        public Braces() : this('\0', '\0', null, null, null, null, null, null, null, null, null) { }

        public Braces(char opening, char closing, string afterChars, Mode? acMode, Style[] afterStyles, Mode? asMode, string beforeChars, Mode? bcMode, Style[] beforeStyles, Mode? bsMode, Logic? logic)
        {
            this.opening = opening;
            this.closing = closing;
            this.logic = logic ?? 0;
            this.afterChars = afterChars ?? string.Empty;
            this.acMode = acMode ?? (Mode) this.logic;
            this.afterStyles = afterStyles ?? new Style[0];
            this.asMode = asMode ?? (Mode) this.logic;
            this.beforeChars = beforeChars ?? string.Empty;
            this.bcMode = bcMode ?? (Mode) this.logic;
            this.beforeStyles = beforeStyles ?? new Style[0];
            this.bsMode = bsMode ?? (Mode) this.logic;
        }

        public bool ShouldAutoClose(char charAfter, byte styleAfter, char charBefore, byte styleBefore)
        {
            if (IsValid)
            {
                switch (logic)
                {
                    case Logic.OR:
                        return Check(beforeChars, charAfter, bcMode)
                            || Check(beforeStyles, (Style) styleAfter, bsMode)
                            || Check(afterChars, charBefore, acMode)
                            || Check(afterStyles, (Style) styleBefore, asMode);
                    case Logic.AND:
                        return Check(beforeChars, charAfter, bcMode)
                            && Check(beforeStyles, (Style) styleAfter, bsMode)
                            && Check(afterChars, charBefore, acMode)
                            && Check(afterStyles, (Style) styleBefore, asMode);
                }
            }

            return false;
        }

        static bool Check<T>(IEnumerable<T> array, T value, Mode mode) => array.Contains(value) == (mode == 0);

        public override string ToString() => IsValid ? $"Opening: {opening} Closing: {closing}" : "New Braces";
    }

    public enum Logic : byte
    {
        OR,
        AND,
    }

    public enum Mode : byte
    {
        Inclusive,
        Exclusive,
    }

    public enum Style : byte
    {
        Default = 0,
        Comment = 1,
        CommentLine = 2,
        CommentDoc = 3,
        Number = 4,
        Predefined = 5,
        String = 6,
        Character = 7,
        //UUID = 8,
        Preprocessor = 9,
        Operator = 10,
        Identifier = 11,
        StringEOL = 12,
        Verbatim = 13,
        RegExp = 14,
        CommentLineDoc = 15,
        Class = 16,
        CommentDocKeyword = 17,
        CommentDocKeywordError = 18,
        Keyword = 19,
        //STRINGRAW = 20,
        //TRIPLEVERBATIM = 21,
        //HASHQUOTEDSTRING = 22,
        //PREPROCESSORCOMMENT = 23,
        Attribute = 24,
        Word4 = 25,
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
