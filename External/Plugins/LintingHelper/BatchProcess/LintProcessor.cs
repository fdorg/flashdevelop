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
        public bool IsAvailable
        {
            get
            {
                return LintingManager.HasLanguage(PluginBase.CurrentProject.Language);
            }
        }

        public string Text
        {
            get
            {
                return TextHelper.GetString("Label.RunLinters");
            }
        }

        public LintProcessor()
        {
        }

        public void Process(ITabbedDocument document)
        {
            LintingManager.LintDocument(document);
        }
    }
}
