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
        public bool IsAvailable => true;

        public string Text => TextHelper.GetStringWithoutMnemonics("Label.TruncateImports");

        public void Process(string[] files)
        {
            foreach (var file in files)
            {
                var document = PluginBase.MainForm.OpenEditableDocument(file) as ITabbedDocument;
                var command = (OrganizeImports) CommandFactoryProvider.GetFactory(document)?.CreateOrganizeImportsCommand();
                if (command == null) continue;
                command.SciControl = document.SciControl;
                command.TruncateImports = true;
                command.Execute();
            }
        }
    }
}
