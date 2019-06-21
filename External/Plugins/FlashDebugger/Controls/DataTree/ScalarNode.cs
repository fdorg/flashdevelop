using System;

namespace FlashDebugger.Controls.DataTree
{
    public class ScalarNode : DataNode
    {

        private string m_Value;
        public override string Value
        {
            get => m_Value;
            set => throw new NotSupportedException();
        }

        public override bool IsLeaf => true;

        public ScalarNode(string text, string value)
            : base(text)
        {
            m_Value = value;
        }

    }
}
