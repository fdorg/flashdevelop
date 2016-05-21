using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ASCompletion.Completion
{
    /// <summary>
    /// Represents a set of brace characters used for automatic closing braces.
    /// </summary>
    [Serializable]
    public sealed class Braces
    {
        char opening;
        char closing;
        string afterChars;
        string beforeChars;
        Style[] afterStyles;
        Style[] beforeStyles;
        Mode acMode;
        Mode asMode;
        Mode bcMode;
        Mode bsMode;
        Logic logic;

        #region Browsable properties

        /// <summary>
        /// The opening brace character.
        /// </summary>
        [Category("Brace Character")]
        [Description("The opening brace character.")]
        public char Opening
        {
            get { return opening; }
            set { opening = value; }
        }

        /// <summary>
        /// The closing brace character.
        /// </summary>
        [Category("Brace Character")]
        [Description("The closing brace character.")]
        public char Closing
        {
            get { return closing; }
            set { closing = value; }
        }

        /// <summary>
        /// An array of Unicode characters to automatically close brace after.
        /// </summary>
        [Category("Trigger Auto Close")]
        [Description("List of characters to automatically add a closing brace after.")]
        public string AfterChars
        {
            get { return afterChars; }
            set { afterChars = value; }
        }

        /// <summary>
        /// Gets or set the mode to treat <see cref="AfterChars"/> as.
        /// </summary>
        [Category("Trigger Auto Close"), DefaultValue(Mode.Inclusive)]
        [Description("The mode for AfterChars:\nInclusive - Add closing brace after the characters.\nExclusive - Do not add closing brace after the characters.")]
        public Mode AfterCharsMode
        {
            get { return acMode; }
            set { acMode = value; }
        }

        /// <summary>
        /// An array of <see cref="Style"/> values to automatically close brace after.
        /// </summary>
        [Category("Trigger Auto Close")]
        [Description("List of styles to automatically add a closing brace after.")]
        public Style[] AfterStyles
        {
            get { return afterStyles; }
            set { afterStyles = value; }
        }

        /// <summary>
        /// Gets or set the mode to treat <see cref="AfterStyles"/> as.
        /// </summary>
        [Category("Trigger Auto Close"), DefaultValue(Mode.Inclusive)]
        [Description("The mode for AfterStyles:\nInclusive - Add closing brace after the styles.\nExclusive - Do not add closing brace after the styles.")]
        public Mode AfterStylesMode
        {
            get { return asMode; }
            set { asMode = value; }
        }

        /// <summary>
        /// An array of Unicode characters to automatically close brace before.
        /// </summary>
        [Category("Trigger Auto Close")]
        [Description("List of characters to automatically add a closing brace before.")]
        public string BeforeChars
        {
            get { return beforeChars; }
            set { beforeChars = value; }
        }

        /// <summary>
        /// Gets or set the mode to treat <see cref="BeforeChars"/> as.
        /// </summary>
        [Category("Trigger Auto Close"), DefaultValue(Mode.Inclusive)]
        [Description("The mode for BeforeChars:\nInclusive - Add closing brace before the characters.\nExclusive - Do not add closing brace before the characters.")]
        public Mode BeforeCharsMode
        {
            get { return bcMode; }
            set { bcMode = value; }
        }

        /// <summary>
        /// An array of <see cref="Style"/> values to automatically close brace before.
        /// </summary>
        [Category("Trigger Auto Close")]
        [Description("List of styles to automatically add a closing brace before.")]
        public Style[] BeforeStyles
        {
            get { return beforeStyles; }
            set { beforeStyles = value; }
        }

        /// <summary>
        /// Gets or set the mode to treat <see cref="BeforeStyles"/> as.
        /// </summary>
        [Category("Trigger Auto Close"), DefaultValue(Mode.Inclusive)]
        [Description("The mode for BeforeStyles:\nInclusive - Add closing brace before the styles.\nExclusive - Do not add closing brace before the styles.")]
        public Mode BeforeStylesMode
        {
            get { return bsMode; }
            set { bsMode = value; }
        }

        /// <summary>
        /// The logic to use for conditions for automatic closing.
        /// </summary>
        [Category("Trigger Auto Close"), DefaultValue(Logic.OR)]
        [Description("The logic to use for the automatic insertion of closing braces.\nOR - One or more conditions are met.\nAND - All conditions are met.")]
        public Logic TriggerLogic
        {
            get { return logic; }
            set { logic = value; }
        }

        #endregion

        /// <summary>
        /// Creates an empty instance of <see cref="Braces"/>.
        /// </summary>
        public Braces() : this(null, null, null, null, null, null, null, null, null, null, null)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="Braces"/>.
        /// </summary>
        /// <param name="arg0"><see cref="Opening"/></param>
        /// <param name="arg1"><see cref="Closing"/></param>
        /// <param name="arg2"><see cref="AfterChars"/></param>
        /// <param name="arg3"><see cref="AfterCharsMode"/></param>
        /// <param name="arg4"><see cref="AfterStyles"/></param>
        /// <param name="arg5"><see cref="AfterStylesMode"/></param>
        /// <param name="arg6"><see cref="BeforeChars"/></param>
        /// <param name="arg7"><see cref="BeforeCharsMode"/></param>
        /// <param name="arg8"><see cref="BeforeStyles"/></param>
        /// <param name="arg9"><see cref="BeforeStylesMode"/></param>
        /// <param name="arg10"><see cref="TriggerLogic"/></param>
        public Braces(char? arg0, char? arg1, string arg2, Mode? arg3, Style[] arg4, Mode? arg5, string arg6, Mode? arg7, Style[] arg8, Mode? arg9, Logic? arg10)
        {
            opening = arg0 ?? '\0';
            closing = arg1 ?? '\0';
            logic = arg10 ?? 0;
            afterChars = arg2 ?? string.Empty;
            acMode = arg3 ?? (Mode) logic;
            afterStyles = arg4 ?? new Style[0];
            asMode = arg5 ?? (Mode) logic;
            beforeChars = arg6 ?? string.Empty;
            bcMode = arg7 ?? (Mode) logic;
            beforeStyles = arg8 ?? new Style[0];
            bsMode = arg9 ?? (Mode) logic;
        }
        
        /// <summary>
        /// Checks whether the specified value meets the current condition and mode.
        /// </summary>
        static bool Check<T>(IEnumerable<T> array, T value, Mode mode)
        {
            return mode == 0 == array.Contains(value);
        }

        /// <summary>
        /// Returns whether this brace is valid.
        /// </summary>
        [Browsable(false)]
        public bool IsValid
        {
            get { return opening != '\0' && closing != '\0'; }
        }

        /// <summary>
        /// Returns whether this brace should close for the specified condition.
        /// </summary>
        /// <param name="charAfter">The Unicode character followed by this brace.</param>
        /// <param name="styleAfter">The <see cref="byte"/> value style followed by this brace.</param>
        /// <param name="charBefore">The Unicode character followed by this brace.</param>
        /// <param name="styleBefore">The <see cref="byte"/> value style followed by this brace.</param>
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

        /// <summary>
        /// Returns a <see cref="string"/> object that represents the current <see cref="Braces"/> object.
        /// </summary>
        public override string ToString()
        {
            return IsValid ? opening + " braces " + closing : "New braces";
        }
    }

    /// <summary>
    /// Defines a set of logic values used for <see cref="Braces"/>.
    /// </summary>
    [Serializable]
    public enum Logic : byte
    {
        /// <summary>
        /// The logical OR operator.
        /// </summary>
        OR,

        /// <summary>
        /// The logical AND operator.
        /// </summary>
        AND
    }

    /// <summary>
    /// Defines modes for conditions used for <see cref="Braces"/>.
    /// </summary>
    [Serializable]
    public enum Mode : byte
    {
        /// <summary>
        /// The associated condition specifies a set of values that are inclusive.
        /// </summary>
        Inclusive,

        /// <summary>
        /// The associated condition specifies a set of values that are exclusive.
        /// </summary>
        Exclusive
    }

    /// <summary>
    /// Defines style values used for <see cref="Braces"/>.
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
