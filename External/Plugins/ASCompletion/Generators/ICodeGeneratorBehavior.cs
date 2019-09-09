using System.Collections.Generic;
using ASCompletion.Completion;
using PluginCore;
using ScintillaNet;

namespace ASCompletion.Generators
{
    public interface ICodeGeneratorBehavior
    {
        void ContextualGenerator(ScintillaControl sci, int position, ASResult expr, List<ICompletionListItem> options);
    }
}
