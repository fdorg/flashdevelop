using System;
using Aga.Controls.Tree;
using flash.tools.debugger;
using PluginCore.Utilities;

namespace FlashDebugger.Controls.DataTree
{
    public abstract class DataNode : Node
    {

        public abstract string Value { get; set; }

        public DataNode(string text) : base(text)
        {
        }

    }
}
