// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using CodeRefactor.Managers;
using System.Collections.Generic;
using PluginCore;
using PluginCore.Localization;

namespace CodeRefactor.BatchProcessors
{
    class ConsistentEOLProcessor : IBatchProcessor
    {
        public bool IsAvailable => true;

        public string Text => TextHelper.GetString("Info.ConsistentEOLs");

        public void Process(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var document = (ITabbedDocument) PluginBase.MainForm.OpenEditableDocument(file);
                document.SciControl.ConvertEOLs(document.SciControl.EOLMode);
            }
        }

        public void ProcessProject(IProject project)
        {
            var files = BatchProcessManager.GetAllProjectFiles(project);
            if (files != null) Process(files);
        }
    }
}
