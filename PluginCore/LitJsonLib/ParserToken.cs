// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
#region Header
/**
 * ParserToken.cs
 *   Internal representation of the tokens used by the lexer and the parser.
 *
 * The authors disclaim copyright to this source code. For more details, see
 * the COPYING file included with this distribution.
 **/
#endregion


namespace LitJson
{
    internal enum ParserToken
    {
        // Lexer tokens
        None = char.MaxValue + 1,
        Number,
        True,
        False,
        Null,
        CharSeq,
        // Single char
        Char,

        // Parser Rules
        Text,
        Object,
        ObjectPrime,
        Pair,
        PairRest,
        Array,
        ArrayPrime,
        Value,
        ValueRest,
        String,

        // End of input
        End,

        // The empty rule
        Epsilon
    }
}
