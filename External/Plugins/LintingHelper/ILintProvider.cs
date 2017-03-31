using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LintingHelper
{
    public interface ILintProvider
    {
        List<LintingResult> DoLint(string[] files);
    }
}
