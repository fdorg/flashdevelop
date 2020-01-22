// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using CodeRefactor.Managers;
using System.Collections.Generic;
using PluginCore;
using CodeRefactor.Commands;
using CodeRefactor.Provider;
using PluginCore.Localization;

namespace CodeRefactor.BatchProcessors
{
    class OrganizeImportsProcessor : IBatchProcessor
    {
        public bool IsAvailable => true;

        public string Text => TextHelper.GetStringWithoutMnemonics("Label.OrganizeImports");

        public void Process(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var document = (ITabbedDocument) PluginBase.MainForm.OpenEditableDocument(file);
                var command = (OrganizeImports)CommandFactoryProvider.GetFactory(document)?.CreateOrganizeImportsCommand();
                if (command is null) continue;
                command.SciControl = document.SciControl;
                command.Execute();
            }
        }

        public void ProcessProject(IProject project)
        {
            var files = BatchProcessManager.GetAllProjectFiles(project);
            Process(files);
        }
    }
}
