﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
