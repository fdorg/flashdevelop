﻿using CodeRefactor.Managers;
using System.Collections.Generic;
using System.Linq;
using PluginCore;
using LintingHelper.Managers;
using PluginCore.Localization;

namespace LintingHelper.BatchProcess
{
    class LintProcessor : IBatchProcessor
    {
        public string Text => TextHelper.GetString("Label.RunLinters");

        public bool IsAvailable => PluginBase.CurrentProject != null && LintingManager.HasLanguage(PluginBase.CurrentProject.Language);

        public void Process(IEnumerable<string> files) => LintingManager.LintFiles(files.ToArray());

        public void ProcessProject(IProject project) => LintingManager.LintProject(project);
    }
}
