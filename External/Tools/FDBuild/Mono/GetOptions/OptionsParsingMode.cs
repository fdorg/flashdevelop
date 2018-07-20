// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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

