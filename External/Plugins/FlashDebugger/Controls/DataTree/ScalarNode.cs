// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
