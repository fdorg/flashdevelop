using System;

namespace FlashDebugger.Controls.DataTree
{
    public class ErrorNode : DataNode
    {

        private readonly string _value;
        public override string Value
        {
            get => _value;
            set => throw new NotSupportedException();
        }

        public override bool IsLeaf => true;

        public ErrorNode(string text, Exception ex)
            : base(text)
        {
            _value = ex.Message;
        }

    }
}
