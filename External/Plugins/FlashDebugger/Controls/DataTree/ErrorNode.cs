using System;

namespace FlashDebugger.Controls.DataTree
{
    public class ErrorNode : DataNode
    {

        private string _value;
        public override string Value
        {
            get { return _value; }
            set { throw new NotSupportedException(); }
        }

        public override bool IsLeaf
        {
            get { return true; }
        }

        public ErrorNode(string text, Exception ex)
            : base(text)
        {
            _value = ex.Message;
        }

    }
}
