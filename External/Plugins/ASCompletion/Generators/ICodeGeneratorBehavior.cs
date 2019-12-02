// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using ASCompletion.Completion;
using PluginCore;
using ScintillaNet;

namespace ASCompletion.Generators
{
    public interface ICodeGeneratorBehavior
    {
        bool ContextualGenerator(ScintillaControl sci, int position, ASResult expr, ICollection<ICompletionListItem> options);
    }
}
