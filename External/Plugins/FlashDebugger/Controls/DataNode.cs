using System;
using Aga.Controls.Tree;
using flash.tools.debugger;
using PluginCore.Utilities;

namespace FlashDebugger.Controls
{
	public class DataNode : Node, IComparable<DataNode>
    {
        public override string Text
        {
            get
			{
				return base.Text;
			}
        }

		private int m_ChildrenShowLimit = 500;
		public int ChildrenShowLimit
		{
			get { return m_ChildrenShowLimit; }
			set { m_ChildrenShowLimit = value; }
		}

        private Variable m_Value;
		private bool m_bEditing = false;

		public int CompareTo(DataNode otherNode)
		{
			String thisName = Text;
			String otherName = otherNode.Text;
			if (thisName == otherName)
			{
				return 0;
			}
			if (thisName.Length>0 && thisName[0] == '_')
			{
				thisName = thisName.Substring(1);
			}
			if (otherName.Length>0 && otherName[0] == '_')
			{
				otherName = otherName.Substring(1);
			}
			int result = LogicalComparer.Compare(thisName, otherName);
			if (result != 0)
			{
				return result;
			}
			return m_Value.getName().length()>0 && m_Value.getName().startsWith("_") ? 1 : -1;
		}

		public string Value
        {
            get
			{
				if (m_Value == null)
				{
					return string.Empty;
				}
                int type = m_Value.getValue().getType();
                string temp = null;
				if (type == VariableType_.MOVIECLIP || type == VariableType_.OBJECT)
				{
                    return m_Value.getValue().getTypeName();
				}
				else if (type == VariableType_.NUMBER)
				{
					double number = ((java.lang.Double)m_Value.getValue().getValueAsObject()).doubleValue();
					if (!Double.IsNaN(number) && (double)(long)number == number)
					{
						if (!m_bEditing)
						{
							if (number < 0 && number >= Int32.MinValue)
							{
								return number.ToString() + " [0x" + ((Int32)number).ToString("x") + "]";
							}
							else if (number < 0 || number > 9)
							{
								return number.ToString() + " [0x" + ((Int64)number).ToString("x") + "]";
							}
						}
						else return number.ToString();
					}
				}
				else if (type == VariableType_.BOOLEAN)
				{
					return m_Value.getValue().getValueAsString().toLowerCase();
				}
				else if (type == VariableType_.STRING)
				{
					if (m_Value.getValue().getValueAsObject() != null)
					{
						if (!m_bEditing)
						{
                            temp = "\"" + escape(m_Value.ToString()) + "\"";
						}
						else
						{
                            temp = m_Value.ToString();
						}
                        return temp;
					}
				}
				else if (type == VariableType_.NULL)
				{
					return "null";
				}
				else if (type == VariableType_.FUNCTION)
				{
                    m_Value.ToString();
					//return "<setter>";
				}
                if (temp == null) temp = m_Value.ToString();
                if (!m_bEditing)
                {
                    temp = escape(temp);
                }
                return temp;
			}
			set
			{
				if (m_Value == null)
				{
					return;
				}
                throw new NotImplementedException();
#if false
				int type = m_Value.getValue().getType();
				if (type == VariableType_.NUMBER)
				{
					m_Value.setValue(type, value);
				}
				else if (type == VariableType.BOOLEAN)
				{
					m_Value.setValue(type, value.ToLower());
				}
				else if (type == VariableType.STRING)
				{
					m_Value.setValue(type, value);
				}
#endif
			}
        }

        private string escape(string text)
        {
            text = text.Replace("\\", "\\\\");
            text = text.Replace("\"", "\\\"");
            text = text.Replace("\0", "\\0");
            text = text.Replace("\a", "\\a");
            text = text.Replace("\b", "\\b");
            text = text.Replace("\f", "\\f");
            text = text.Replace("\n", "\\n");
            text = text.Replace("\r", "\\r");
            text = text.Replace("\t", "\\t");
            text = text.Replace("\v", "\\v");
            if (text.Length > 65533)
                text = text.Substring(0, 65533 - 5) + "[...]";
            return text;
        }

		public Variable Variable
		{
			get
			{
				return m_Value;
			}
		}

        public override bool IsLeaf
        {
            get
            {
				if (m_Value == null)
				{
					return (this.Nodes.Count == 0);
				}
				return m_Value.getValue().getType() != VariableType_.MOVIECLIP && m_Value.getValue().getType() != VariableType_.OBJECT;
            }
        }

		public bool IsEditing
		{
			get
			{
				return m_bEditing;
			}
			set
			{
				m_bEditing = value;
			}
		}

        public DataNode(Variable value) : base(value.getName())
        {
            m_Value = value;
        }

		public DataNode(string value) : base(value)
		{
			m_Value = null;
		}

	}

}
