using System;

namespace FlashDebugger.Controls.DataTree
{
    public class ScalarNode : DataNode
    {

        private string m_Value;
        public override string Value
        {
            get { return m_Value; }
            set { throw new NotSupportedException(); }
        }

        public override bool IsLeaf
        {
            get { return true; }
        }

        public ScalarNode(string text, string value)
            : base(text)
        {
            m_Value = value;
        }

    }
}
