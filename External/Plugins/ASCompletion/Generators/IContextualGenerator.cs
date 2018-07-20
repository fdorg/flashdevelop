// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.Collections.Generic;
using PluginCore;
using ScintillaNet;

namespace ASCompletion.Generators
{
    public interface IContextualGenerator
    {
        bool ContextualGenerator(ScintillaControl sci, int position, List<ICompletionListItem> options);
    }
}
