using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ScintillaNet.Lexers;

namespace ASCompletion.Completion
{
    [Serializable]
    public class Braces
    {
        [Category("1) Braces")]
        public char Opening { get; set; }

        [Category("1) Braces")]
        public char Closing { get; set; }

        [Category("2) Auto Close Options")]
        public string AfterChars { get; set; }

        [Category("2) Auto Close Options")]
        public Mode AfterCharsMode { get; set; }

        [Category("2) Auto Close Options")]
        public AS3Style[] AfterStyles { get; set; }

        [Category("2) Auto Close Options")]
        public Mode AfterStylesMode { get; set; }

        [Category("2) Auto Close Options")]
        public string BeforeChars { get; set; }

        [Category("2) Auto Close Options")]
        public Mode BeforeCharsMode { get; set; }

        [Category("2) Auto Close Options")]
        public AS3Style[] BeforeStyles { get; set; }

        [Category("2) Auto Close Options")]
        public Mode BeforeStylesMode { get; set; }

        [Category("2) Auto Close Options")]
        public Logic TriggerLogic { get; set; }

        [Browsable(false)]
        public bool IsValid => Opening != '\0' && Closing != '\0';

        public Braces() : this('\0', '\0', null, null, null, null, null, null, null, null, null) { }

        public Braces(char opening, char closing, string afterChar, Mode? acMode, AS3Style[] afterStyle, Mode? asMode, string beforeChar, Mode? bcMode, AS3Style[] beforeStyle, Mode? bsMode, Logic? logic)
        {
            Opening = opening;
            Closing = closing;
            AfterChars = afterChar ?? string.Empty;
            AfterCharsMode = acMode ?? 0;
            AfterStyles = afterStyle ?? new AS3Style[0];
            AfterStylesMode = asMode ?? 0;
            BeforeChars = beforeChar ?? string.Empty;
            BeforeCharsMode = bcMode ?? 0;
            BeforeStyles = beforeStyle ?? new AS3Style[0];
            BeforeStylesMode = bsMode ?? 0;
            TriggerLogic = logic ?? 0;
        }

        public bool ShouldAutoClose(char charAfter, int styleAfter, char charBefore, int styleBefore)
        {
            if (IsValid)
            {
                switch (TriggerLogic)
                {
                    case Logic.OR:
                        return Check(BeforeChars, charAfter, BeforeCharsMode)
                            || Check(BeforeStyles, (AS3Style) styleAfter, BeforeStylesMode)
                            || Check(AfterChars, charBefore, AfterCharsMode)
                            || Check(AfterStyles, (AS3Style) styleBefore, AfterStylesMode);
                    case Logic.AND:
                        return (BeforeChars.Length == 0 || Check(BeforeChars, charAfter, BeforeCharsMode))
                            && (BeforeStyles.Length == 0 || Check(BeforeStyles, (AS3Style) styleAfter, BeforeStylesMode))
                            && (AfterChars.Length == 0 || Check(AfterChars, charBefore, AfterCharsMode))
                            && (AfterStyles.Length == 0 || Check(AfterStyles, (AS3Style) styleBefore, AfterStylesMode));
                }
            }

            return false;
        }

        bool Check<T>(IEnumerable<T> array, T value, Mode mode)
        {
            return array.Contains(value) == (mode == 0);
        }

        public override string ToString()
        {
            return IsValid ? $"Opening: {Opening} Closing: {Closing}" : "New Braces";
        }
    }

    public enum Logic
    {
        OR,
        AND,
    }

    public enum Mode
    {
        Inclusive,
        Exclusive,
    }

    public enum AS3Style
    {
        DEFAULT = 0,
        COMMENT = 1,
        COMMENTLINE = 2,
        COMMENTDOC = 3,
        NUMBER = 4,
        WORD = 5,
        STRING = 6,
        CHARACTER = 7,
        //UUID = 8,
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
        //TRIPLEVERBATIM = 21,
        //HASHQUOTEDSTRING = 22,
        //PREPROCESSORCOMMENT = 23,
        WORD3 = 24,
        WORD4 = 25,
        WORD5 = 26,
        //GDEFAULT = 32,
        //LINENUMBER = 33,
        //BRACELIGHT = 34,
        //BRACEBAD = 35,
        //CONTROLCHAR = 36,
        //INDENTGUIDE = 37,
        //LASTPREDEFINED = 39
    }
}
