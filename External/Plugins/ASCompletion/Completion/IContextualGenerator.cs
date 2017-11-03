using System.Collections.Generic;
using PluginCore;
using ScintillaNet;

namespace ASCompletion.Completion
{
    public interface IContextualGenerator
    {
        bool ContextualGenerator(ScintillaControl sci, List<ICompletionListItem> options, ASResult expr);
    }
}
