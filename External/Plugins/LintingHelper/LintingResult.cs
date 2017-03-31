using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public LintingResult()
        {
        }
    }

    public enum LintingSeverity
    {
        Info,
        Warning,
        Error
    }
}
