using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public ScalarNode(string text, string value)
            : base(text)
        {
            m_Value = value;
        }

        public override bool IsLeaf
        {
            get
            {
                return true;
            }
        }

    }
}
