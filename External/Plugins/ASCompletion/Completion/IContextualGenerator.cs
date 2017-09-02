using System.Collections.Generic;
using PluginCore;
using ScintillaNet;

namespace ASCompletion.Completion
{
    public interface IContextualGenerator
    {
        void ContextualGenerator(ScintillaControl sci, List<ICompletionListItem> options, ASResult expr);
    }
}
