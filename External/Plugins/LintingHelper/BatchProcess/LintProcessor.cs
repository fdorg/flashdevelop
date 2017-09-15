using CodeRefactor.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginCore;
using LintingHelper.Managers;
using PluginCore.Localization;

namespace LintingHelper.BatchProcess
{
    class LintProcessor : IBatchProcessor
    {
        public bool IsAvailable => LintingManager.HasLanguage(PluginBase.CurrentProject.Language);

        public string Text => TextHelper.GetString("Label.RunLinters");

        public void Process(IEnumerable<string> files)
        {
            LintingManager.LintFiles(files.ToArray());
        }

        public void ProcessProject(IProject project)
        {
            LintingManager.LintProject(project);
        }
    }
}
