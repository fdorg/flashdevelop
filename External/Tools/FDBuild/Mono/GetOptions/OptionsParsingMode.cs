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

