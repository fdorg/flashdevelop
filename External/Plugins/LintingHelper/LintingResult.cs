// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace LintingHelper
{
    public class LintingResult
    {
        /// <summary>
        /// The file, the result occured in.
        /// </summary>
        public string File;

        /// <summary>
        /// The line, the result occured on.
        /// </summary>
        public int Line;

        /// <summary>
        /// The first character position on the line. (starting at 0)
        /// </summary>
        public int FirstChar = 0;

        /// <summary>
        /// The character length of the result.
        /// If this is -1, the whole line is marked.
        /// </summary>
        public int Length = -1;

        /// <summary>
        /// The description the user should be shown.
        /// </summary>
        public string Description;

        public LintingSeverity Severity;

        public bool Equals(LintingResult other)
        {
            return string.Equals(File, other.File) && Line == other.Line && FirstChar == other.FirstChar && Length == other.Length
                && string.Equals(Description, other.Description) && Severity == other.Severity;
        }
    }

    public enum LintingSeverity
    {
        Info,
        Warning,
        Error
    }
}
