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
        public string Text
        {
            get
            {
                return TextHelper.GetString("Info.FormatCode");
            }
        }

        public bool IsAvailable
        {
            get
            {
                return true;
            }
        }

        public FormatCodeProcessor()
        {
        }

        public void Process(ITabbedDocument document)
        {
            DataEvent de = new DataEvent(EventType.Command, "CodeFormatter.FormatDocument", document);
            EventManager.DispatchEvent(this, de);
        }
    }
}
