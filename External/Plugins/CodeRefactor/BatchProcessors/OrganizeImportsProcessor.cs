using CodeRefactor.Managers;
using System.Collections.Generic;
using PluginCore;
using CodeRefactor.Commands;
using CodeRefactor.Provider;
using PluginCore.Localization;

namespace CodeRefactor.BatchProcessors
{
    internal class OrganizeImportsProcessor : IBatchProcessor
    {
        public bool IsAvailable => true;

        public string Text => TextHelper.GetStringWithoutMnemonics("Label.OrganizeImports");

        public void Process(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var document = (ITabbedDocument) PluginBase.MainForm.OpenEditableDocument(file);
                if (document is null) continue;
                var command = (OrganizeImports)CommandFactoryProvider.GetFactory(document)?.CreateOrganizeImportsCommand();
                if (command is null) continue;
                command.SciControl = document.SciControl;
                command.Execute();
            }
        }

        public void ProcessProject(IProject project) => Process(BatchProcessManager.GetAllProjectFiles(project));
    }
}