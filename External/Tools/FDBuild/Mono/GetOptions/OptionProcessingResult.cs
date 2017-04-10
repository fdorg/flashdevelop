// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace Mono.GetOptions
{
    using System;

    internal enum OptionProcessingResult
    {
        NotThisOption,
        OptionAlone,
        OptionConsumedParameter
    }
}

