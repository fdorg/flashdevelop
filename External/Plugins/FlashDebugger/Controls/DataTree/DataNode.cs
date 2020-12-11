// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using Aga.Controls.Tree;

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