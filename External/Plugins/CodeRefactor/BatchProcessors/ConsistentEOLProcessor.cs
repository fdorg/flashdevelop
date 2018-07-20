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
                var document = PluginBase.MainForm.OpenEditableDocument(file) as ITabbedDocument;
                document.SciControl.ConvertEOLs(document.SciControl.EOLMode);
            }
        }

        public void ProcessProject(IProject project)
        {
            var files = BatchProcessManager.GetAllProjectFiles(project);
            Process(files);
        }
    }
}
