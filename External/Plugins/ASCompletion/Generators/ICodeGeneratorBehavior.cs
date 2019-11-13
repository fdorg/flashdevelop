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
