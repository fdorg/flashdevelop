using System;
using System.Text;

/**
 * Credit: http://www.ko-sw.com/Blog/post/An-Ultra-Fast-CSS-Minify-Algorithm.aspx
 */

namespace CssCompletion
{
    /// <summary>Compacts strings containing CSS code.</summary>
    public static class CssMinifier
    {
        /// <summary>
        /// Identifies CSS parsing state for a single character
        /// within a CSS string.
        /// </summary>
        enum CssState
        {
            /// <summary>
            /// Represents a punctuation character. This can be any character
            /// that is not classified as a Token, StringD, or StringS
            /// </summary>
            Punctuation,

            /// <summary>
            /// Represents a token character: an alphanumeric value
            /// or a special character that is treated the same way.
            /// </summary>
            Token,

            /// <summary>
            /// Represents a double quote character or a
            /// character after it until a closing un-escaped
            /// double quote character is found.
            /// </summary>
            StringD,

            /// <summary>
            /// Represents a single quote character or a
            /// character after it until a closing un-escaped
            /// single quote character is found.
            /// </summary>
            StringS
        }

        /// <summary>
        /// Stores characters that are may appear in a CSS token.
        /// </summary>
        const string AdditionalTokenChars = ".#_-@*%()";    //

        /// <summary>Minifies a specified string containing CSS style sheet.</summary>
        /// <param name="theCss">A string with input CSS.</param>
        /// <returns>A minified string.</returns>
        public static string Minify(string theCss)
        {
            // Assume that the length of the output string
            // will be at most 75% the length of the incoming one to avoid
            // additional StringBuilder reallocations.
            StringBuilder aRet = new StringBuilder(theCss.Length * 3 / 4);

            int aNumChars = theCss.Length;

            CssState aPrevState = CssState.Punctuation;
            int aPrevPos = 0;

            int i = 0;

            while(i < aNumChars)
            {
                CssState aCurState = GetState(theCss, ref i, aPrevState);

                if (aPrevState == CssState.Punctuation && i > 0
                    && theCss[aPrevPos] == '}' && theCss[i] != '}')
                {
                    if (Char.IsDigit(theCss[i])) aRet.Append(' ');
                    else aRet.Append('\n'); // keep blocks on new lines
                }

                if (i > aPrevPos + 1)
                {
                    // If whitespace is found between two tokens, keep it compact
                    if (aPrevState != CssState.Punctuation && aCurState != CssState.Punctuation)
                        aRet.Append(' ');

                    // Otherwise, no whitespace is needed, skip everything between aPrevPos and i
                }

                aPrevPos = i;
                aPrevState = aCurState;
                aRet.Append(theCss[i++]);
            }

            return aRet.ToString();
        }

        /// <summary>
        /// Skips any comments and whitespace characters found in the incoming string,
        /// updates the position variable (i) to a value represented by a meaningful state
        /// </summary>
        /// 
        /// <param name="theCss">
        /// The incoming CSS string being processed.
        /// The string itself remains intact during processing.
        /// </param>
        /// 
        /// <param name="thePos">
        /// Upon entry, the current character index within <paramref name="theCss" />.
        /// Upon exit, the index at which processing should continue. All characters
        /// between the two values will skipped from the resulting output.
        /// </param>
        /// 
        /// <param name="theCurState">The current CSS parsing state.</param>
        /// 
        /// <returns>The future CSS parsing state.</returns>
        private static CssState GetState(string theCss, ref int thePos, CssState theCurState)
        {
            int aLen = theCss.Length;
            int i = thePos;

            if (theCurState == CssState.StringD)
            {
                if(theCss[i] == '\"')
                {
                    // Make sure the double quote character is not escaped
                    if(thePos > 0)
                        if(theCss[i-1] == '\\')
                            return CssState.StringD;

                    // Enforce a whitespace afterwards
                    return CssState.Token;
                }
                else
                    return CssState.StringD;
            }
            else if (theCurState == CssState.StringS)
            {
                if(theCss[i] == '\'')
                {
                    // Make sure the single quote character is not escaped
                    if(thePos > 0)
                        if(theCss[i-1] == '\\')
                            return CssState.StringS;

                    // Enforce a whitespace afterwards
                    return CssState.Token;
                }
                else
                    return CssState.StringS;
            }


            bool aSkip = true;

            while(aSkip)
            {
                /////////////////////////////////////////
                // Skip whitespace
                while(aSkip = (i < aLen - 1 && IsWhitespaceChar(theCss[i]) ) )
                    i++;

                /////////////////////////////////////////
                // Skip comments
                if(i < aLen - 1)
                    if (theCss[i] == '/' && theCss[i+1] == '*') // comment opening
                    {
                        aSkip = true;

                        while(i < aLen - 1)
                        {
                            i++;

                            if(theCss[i-1] == '*' && theCss[i] == '/') // comment closing
                            {
                                i++;
                                break;
                            }
                        }
                        if (i >= aLen) i = aLen - 1; // overflow
                    }
            }   // end while(aSkip)
            
            thePos = i;
            char c = theCss[i];
            if (IsTokenChar(c))
                return CssState.Token;

            else if (c == '\"')
                return CssState.StringD;

            else if (c == '\'')
                return CssState.StringS;

            else
                return CssState.Punctuation;
        }

        private static bool IsWhitespaceChar(char p)
        {
            return p == '\t' || p == '\r' || p == '\n' || p == ' ';
        }

        private static bool IsTokenChar(char theChar)
        {
            if(theChar >= 'a' && theChar <= 'z')
                return true;

            else if (theChar >= '0' && theChar <= '9')
                return true;

            else if(theChar >= 'A' && theChar <= 'Z')
                return true;

            else if ( AdditionalTokenChars.IndexOf(theChar) >= 0 )
                return true;

            else
                return false;
        }
    }

}
