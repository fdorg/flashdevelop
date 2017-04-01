using CodeRefactor.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginCore;
using CodeRefactor.Commands;
using CodeRefactor.Provider;
using PluginCore.Localization;

namespace CodeRefactor.BatchProcessors
{
    class TruncateImportsProcessor : IBatchProcessor
    {
        public bool IsAvailable
        {
            get
            {
                return true;
            }
        }

        public string Text
        {
            get
            {
                return TextHelper.GetStringWithoutMnemonics("Label.TruncateImports");
            }
        }

        public void Process(ITabbedDocument document)
        {
            var command = (OrganizeImports)CommandFactoryProvider.GetFactory(document).CreateOrganizeImportsCommand();
            command.SciControl = document.SciControl;
            command.TruncateImports = true;
            command.Execute();
        }
    }
}
