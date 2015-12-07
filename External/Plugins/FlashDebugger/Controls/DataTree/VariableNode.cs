using System;
using Aga.Controls.Tree;
using flash.tools.debugger;
using flash.tools.debugger.expression;
using java.io;
using PluginCore.Utilities;

namespace FlashDebugger.Controls.DataTree
{
    public class VariableNode : ValueNode, IComparable<VariableNode>
    {

        public override string Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if (m_Variable == null)
                    return;

                var flashInterface = PluginMain.debugManager.FlashInterface;
                var b = new ASTBuilder(false);
                var exp = b.parse(new StringReader(this.GetVariablePath() + "=" + value));
                var ctx = new ExpressionContext(flashInterface.Session, flashInterface.GetFrames()[PluginMain.debugManager.CurrentFrame]);
                exp.evaluate(ctx);
            }
        }

        private Variable m_Variable;
        public Variable Variable
        {
            get { return m_Variable; }
            set
            {
                if (m_Variable == value) return;

                m_Variable = value;
                if (m_Variable != null)
                {
                    m_Value = m_Variable.getValue();
                    Text = m_Variable.getName();
                } 
                else
                {
                    m_Value = null;
                    Text = "";
                }
            }
        }

        public VariableNode(string text)
            : base(text)
        {
        }

        public VariableNode(Variable variable)
            : base(variable.getName())
        {
            m_Variable = variable;
            m_Value = variable.getValue();
        }

        public int CompareTo(VariableNode otherNode)
        {
            string thisName = Text;
            string otherName = otherNode.Text;
            if (thisName == otherName)
            {
                return 0;
            }
            if (thisName.Length > 0 && thisName[0] == '_')
            {
                thisName = thisName.Substring(1);
            }
            if (otherName.Length > 0 && otherName[0] == '_')
            {
                otherName = otherName.Substring(1);
            }
            int result = LogicalComparer.Compare(thisName, otherName);
            if (result != 0)
            {
                return result;
            }
            return m_Variable.getName().length() > 0 && m_Variable.getName().startsWith("_") ? 1 : -1;
        }

    }

    internal static class NodeExtensions
    {
        public static String GetVariablePath(this Node node)
        {
            String ret = string.Empty;
            if (node.Tag != null && node.Tag is String)
                return (String)node.Tag; // fix for: live tip value has no parent
            if (node.Parent != null) ret = node.Parent.GetVariablePath();
            if (node is VariableNode)
            {
                VariableNode datanode = node as VariableNode;
                if (datanode.Variable != null)
                {
                    if (ret == "") return datanode.Variable.getName();
                    if ((datanode.Variable.getAttributes() & 0x00020000) == 0x00020000) //VariableAttribute_.IS_DYNAMIC
                    {
                        ret += "[\"" + datanode.Variable.getName() + "\"]";
                    }
                    else
                    {
                        ret += "." + datanode.Variable.getName();
                    }
                }
            }
            return ret;
        }

    }
}
