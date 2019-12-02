// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace FlashDebugger.Controls.DataTree
{
    public class ScalarNode : DataNode
    {

        private readonly string m_Value;
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
