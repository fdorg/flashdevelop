using CodeRefactor.Managers;
using System.Collections.Generic;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;

namespace CodeRefactor.BatchProcessors
{
    internal class FormatCodeProcessor : IBatchProcessor
    {
        public string Text => TextHelper.GetString("Info.FormatCode");

        public bool IsAvailable => true;

        public void Process(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var document = (ITabbedDocument) PluginBase.MainForm.OpenEditableDocument(file);
                if (document is null) continue;
                var de = new DataEvent(EventType.Command, "CodeFormatter.FormatDocument", document);
                EventManager.DispatchEvent(this, de);
            }
        }

        public void ProcessProject(IProject project) => Process(BatchProcessManager.GetAllProjectFiles(project));
    }
}