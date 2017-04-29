// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using CodeRefactor.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginCore;
using PluginCore.Localization;

namespace CodeRefactor.BatchProcessors
{
    class ConsistentEOLProcessor : IBatchProcessor
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
                return TextHelper.GetString("Info.ConsistentEOLs");
            }
        }

        public void Process(ITabbedDocument document)
        {
            document.SciControl.ConvertEOLs(document.SciControl.EOLMode);
        }
    }
}
