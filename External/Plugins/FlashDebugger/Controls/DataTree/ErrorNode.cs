// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
