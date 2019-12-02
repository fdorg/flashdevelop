// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace Mono.GetOptions
{
    using System;

    [Flags]
    public enum OptionsParsingMode
    {
        // Fields
        Both = 3,
        GNU_DoubleDash = 4,
        Linux = 1,
        Windows = 2
    }
}