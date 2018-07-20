using CodeRefactor.Managers;
using System.Collections.Generic;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;

namespace CodeRefactor.BatchProcessors
{
    class FormatCodeProcessor : IBatchProcessor
    {
        public string Text => TextHelper.GetString("Info.FormatCode");

        public bool IsAvailable => true;

        public void Process(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var document = PluginBase.MainForm.OpenEditableDocument(file) as ITabbedDocument;
                DataEvent de = new DataEvent(EventType.Command, "CodeFormatter.FormatDocument", document);
                EventManager.DispatchEvent(this, de);
            }
        }

        public void ProcessProject(IProject project)
        {
            var files = BatchProcessManager.GetAllProjectFiles(project);
            Process(files);
        }
    }
}
