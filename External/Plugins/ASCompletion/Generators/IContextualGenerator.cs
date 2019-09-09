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
