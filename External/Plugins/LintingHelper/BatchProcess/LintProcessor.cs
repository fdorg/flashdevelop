using CodeRefactor.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginCore;
using LintingHelper.Managers;

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
                return "Analyze Code"; //TODO: localize
            }
        }

        public LintProcessor()
        {
        }

        public void Process(ITabbedDocument document)
        {
            Managers.LintingManager.LintDocument(document);
        }
    }
}
