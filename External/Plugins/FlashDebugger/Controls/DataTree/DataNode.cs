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
