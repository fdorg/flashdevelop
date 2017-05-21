using CodeRefactor.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;

namespace CodeRefactor.BatchProcessors
{
    class FormatCodeProcessor : IBatchProcessor
    {
        public string Text => TextHelper.GetString("Info.FormatCode");

        public bool IsAvailable => true;

        public void Process(string[] files)
        {
            foreach (var file in files)
            {
                var document = PluginBase.MainForm.OpenEditableDocument(file) as ITabbedDocument;
                DataEvent de = new DataEvent(EventType.Command, "CodeFormatter.FormatDocument", document);
                EventManager.DispatchEvent(this, de);
            }
        }
    }
}
