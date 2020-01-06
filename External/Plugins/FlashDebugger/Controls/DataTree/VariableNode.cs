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
            get => base.Value;
            set
            {
                if (variable is null) return;
                var flashInterface = PluginMain.debugManager.FlashInterface;
                var b = new ASTBuilder(false);
                var exp = b.parse(new StringReader(this.GetVariablePath() + "=" + value));
                var ctx = new ExpressionContext(flashInterface.Session, flashInterface.GetFrames()[PluginMain.debugManager.CurrentFrame]);
                exp.evaluate(ctx);
            }
        }

        Variable variable;

        public Variable Variable
        {
            get => variable;
            set
            {
                if (variable == value) return;

                variable = value;
                if (variable != null)
                {
                    m_Value = variable.getValue();
                    Text = variable.getName();
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
            this.variable = variable;
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
            return variable.getName().length() > 0 && variable.getName().startsWith("_") ? 1 : -1;
        }

    }

    internal static class NodeExtensions
    {
        public static string GetVariablePath(this Node node)
        {
            var result = string.Empty;
            if (node.Tag is string tag) return tag; // fix for: live tip value has no parent
            if (node.Parent != null) result = node.Parent.GetVariablePath();
            var datanode = node as VariableNode;
            if (datanode?.Variable != null)
            {
                if (result == "") return datanode.Variable.getName();
                if ((datanode.Variable.getAttributes() & 0x00020000) == 0x00020000) //VariableAttribute_.IS_DYNAMIC
                {
                    result += "[\"" + datanode.Variable.getName() + "\"]";
                }
                else
                {
                    result += "." + datanode.Variable.getName();
                }
            }
            return result;
        }

    }
}