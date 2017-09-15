using PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LintingHelper
{
    public delegate void LintCallback(List<LintingResult> results);

    public interface ILintProvider
    {
        void LintAsync(string[] files, LintCallback callback);
        void LintProjectAsync(IProject project, LintCallback callback);
    }
}
