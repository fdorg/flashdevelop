// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace Flash.Tools.Debugger.Expression
{
    /// <summary> 
    /// </summary>
    public class Util
    {
        public static Boolean isJavaIdentifierStart(char ch)
        {
            return Char.IsLetter(ch) || ch == '$' || ch == '_';
        }
        public static Boolean isJavaIdentifierPart(char ch)
        {
            return Char.IsLetterOrDigit(ch) || ch == '$' || ch == '_';
        }
    }
}
