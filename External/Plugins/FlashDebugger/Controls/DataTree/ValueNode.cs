using System;
using flash.tools.debugger;

namespace FlashDebugger.Controls.DataTree
{
    public class ValueNode : DataNode
    {
        private int m_ChildrenShowLimit = 500;
        public int ChildrenShowLimit
        {
            get { return m_ChildrenShowLimit; }
            set { m_ChildrenShowLimit = value; }
        }

        protected Value m_Value;
        private bool m_bEditing = false;

		public string ID
		{
			get
			{
				if (m_Value != null)
				{
					int type = m_Value.getType();
					if (type == VariableType_.MOVIECLIP || type == VariableType_.OBJECT)
					{
						return m_Value.getTypeName().replaceAll("::", ".").replaceAll("@", " - ").ToString();
					}
					else if (type == VariableType_.FUNCTION)
					{
						return "Function - " + m_Value.ToString();
					}
				}
				return "";
			}
		}

        public override string Value
        {
            get
            {
                if (m_Value == null)
                {
                    return string.Empty;
                }
                int type = m_Value.getType();
                string temp = null;
                if (type == VariableType_.MOVIECLIP || type == VariableType_.OBJECT)
                {
					// return class type without classpath
					string typeStr = Strings.AfterLast(m_Value.getTypeName().ToString(), "::", true);
					string shortStr = Strings.Before(typeStr, "@");
					if (shortStr == "[]")
					{
						return "Array";
					}
					return typeStr;

                    //return m_Value.getTypeName();
                }
                else if (type == VariableType_.NUMBER)
                {
                    double number = ((java.lang.Double)m_Value.getValueAsObject()).doubleValue();
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
                    return m_Value.getValueAsString().toLowerCase();
                }
                else if (type == VariableType_.STRING)
                {
                    if (m_Value.getValueAsObject() != null)
                    {
                        return "\"" + escape(m_Value.ToString()) + "\"";
                    }
                }
                else if (type == VariableType_.NULL)
                {
                    return "null";
                }
                else if (type == VariableType_.FUNCTION)
                {
                    return "Function";
                }
                temp = m_Value.ToString();
                if (!m_bEditing)
                {
                    temp = escape(temp);
                }
                return temp;
            }
            set
            {
                throw new NotSupportedException();
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

        public Value PlayerValue
        {
            get
            {
                return m_Value;
            }
            set
            {
                if (m_Value == value) return;

                m_Value = value;
                //this.NotifyModel();
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
                return m_Value.getType() != VariableType_.MOVIECLIP && m_Value.getType() != VariableType_.OBJECT;
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

        public ValueNode(string text)
            : this(text, null)
        {
        }

        public ValueNode(string text, Value value)
            : base(text)
        {
            m_Value = value;
        }

    }

}
