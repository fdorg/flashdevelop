using System;
using flash.tools.debugger;
using Double = java.lang.Double;
using System.Text;

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

        public bool HideFullClasspath { get; set; }
        public bool HideClassId { get; set; }

        protected Value m_Value;
        private bool m_bEditing = false;

        /// <summary>
        /// Get the display value based on user's preferences
        /// </summary>
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
                    string typeStr = m_Value.getTypeName().ToString();
                    // rename vector
                    typeStr = typeStr.Replace("__AS3__.vec::Vector.<", "Vector.<");
                    if (HideFullClasspath)
                    {
                        // return class type without classpath
                        typeStr = CleanTypeClassPaths(typeStr);
                    }
                    else
                    {
                        // return class type with classpath
                        typeStr = typeStr.Replace("::", ".");
                    }
                    
                    // show / hide IDs
                    typeStr = HideClassId ? typeStr.Before("@") : typeStr.Replace("@", " @");

                    // rename array
                    if (typeStr.StartsWith("[]"))
                    {
                        typeStr = typeStr.Replace("[]", "Array");
                    }

                    return typeStr;
                }
                else if (type == VariableType_.NUMBER)
                {
                    double number = ((Double)m_Value.getValueAsObject()).doubleValue();
                    if (!System.Double.IsNaN(number) && (double)(long)number == number)
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
                        return "\"" + Escape(m_Value.ToString()) + "\"";
                    }
                }
                else if (type == VariableType_.NULL)
                {
                    return "null";
                }
                else if (type == VariableType_.FUNCTION)
                {
                    return "Function @" + m_Value.ToString();
                }
                temp = m_Value.ToString();
                if (!m_bEditing)
                {
                    temp = Escape(temp);
                }
                return temp;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Get the full classpath of the value, eg: "flash.display.MovieClip"
        /// </summary>
        public string ClassPath
        {
            get
            {

                if (m_Value == null)
                {
                    return null;
                }
                int type = m_Value.getType();
                if (type == VariableType_.MOVIECLIP || type == VariableType_.OBJECT)
                {
                    string typeStr = m_Value.getTypeName().replaceAll("::", ".").ToString().Before("@");

                    if (typeStr == "[]")
                    {
                        return "Array";
                    }
                    return typeStr;

                }
                else if (type == VariableType_.NUMBER)
                {
                    return "Number";
                }
                else if (type == VariableType_.BOOLEAN)
                {
                    return "Boolean";
                }
                else if (type == VariableType_.STRING)
                {
                    return "String";
                }
                else if (type == VariableType_.NULL)
                {
                    return "null";
                }
                else if (type == VariableType_.FUNCTION)
                {
                    return "Function";
                }
                return null;
            }
        }

        /// <summary>
        /// Get the object ID with the class name
        /// </summary>
        public string Id
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

        /// <summary>
        /// Check if the value is a primitive type (int, Number, String, Boolean)
        /// </summary>
        public bool IsPrimitive
        {
            get
            {
                if (m_Value == null)
                {
                    return false;
                }
                int type = m_Value.getType();
                return type == VariableType_.NUMBER || type == VariableType_.BOOLEAN ||
                       type == VariableType_.STRING || type == VariableType_.NULL;
            }
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

        internal static string Escape(string text)
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

        /// <summary>
        /// Removes any class path from a fully qualified name. Eg.: flash.display::Sprite becomes Sprite
        /// </summary>
        /// <param name="qualifiedName">The fully qualified name to clean</param>
        /// <returns>The class name with no class paths</returns>
        internal static string CleanTypeClassPaths(string qualifiedName)
        {
            char[] delims = { ',', ' ', '<', '>' };
            var buffer = new StringBuilder();
            bool inPackage = false;

            /**
               In order to strip the class-path we are going to traverse the class type in 
                 reverse so we don't need to keep extra buffers or use complex splitting or regexes.
               Packages are always separated by colons or dots, so the first time we found one we
                 are on the package (watch out with Vector.<> notation), and the whole type is always
                 together, so any delim means that we finished with the type, and since this comes
                 from the debugger we know that we are not going to find any other type delimiter,
                 or incomplete/wrong case. We could check for Char.IsWhiteSpace for other uses.
               In resume, with this method we could support without much problem even complex cases like:
                 Collections.Generic.Dictionary<Collections.Generic.Dictionary<String, Test.CustomClassKey>, Collections.Generic.List<Test.CustomClass>>
            */
            for (int i = qualifiedName.Length - 1; i >= 0; i--)
            {
                char c = qualifiedName[i];

                if (inPackage)
                {
                    if (Array.IndexOf(delims, c) < 0)
                        continue;

                    inPackage = false;
                }
                else if ((c == '.' && qualifiedName[i + 1] != '<') || c == ':')
                {
                    inPackage = true;
                    continue;
                }

                buffer.Insert(0, c);
            }

            return buffer.ToString();
        }
    }

}
