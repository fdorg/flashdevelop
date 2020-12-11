using PluginCore;
using System.Collections.Generic;

namespace LintingHelper
{
    public delegate void LintCallback(List<LintingResult> results);

    public interface ILintProvider
    {
        void LintAsync(IEnumerable<string> files, LintCallback callback);
        void LintProjectAsync(IProject project, LintCallback callback);
    }
}
