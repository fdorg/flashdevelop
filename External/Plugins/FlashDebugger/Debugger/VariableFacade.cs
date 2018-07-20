using System;
using flash.tools.debugger;
using flash.tools.debugger.events;

namespace FlashDebugger
{
    /// <summary> 
    /// A VariableFacade provides a wrapper around a Variable object
    /// that provides a convenient way of storing parent information.
    /// Don't ask me why we didn't just add a parent member to 
    /// Variable and be done with it.
    /// </summary>
    public class VariableFacade : Variable
    {
        internal Variable m_var;
        internal long m_context;
        internal String m_name;
        internal String m_path;

        public java.lang.String getQualifiedName()
        {
            return m_var.getQualifiedName();
        }

        public java.lang.String getNamespace()
        {
            return m_var.getNamespace();
        }

        public int getLevel()
        {
            return m_var.getLevel();
        }

        public int getScope()
        {
            return m_var.getScope();
        }

        virtual public String Path
        {
            get
            {
                return m_path;
            }
            
            set
            {
                m_path = value;
            }
            
        }

        /// <summary> Our lone get context (i.e. parent) interface </summary>
        virtual public int Context
        {
            get
            {
                return (int) m_context;
            }
            
        }

        virtual public Variable Variable
        {
            get
            {
                return m_var;
            }
            
        }
        
        public VariableFacade(Variable v, long context)
        {
            init(context, v, null);
        }

        public VariableFacade(long context, String name)
        {
            init(context, null, name);
        }
        
        internal virtual void  init(long context, Variable v, String name)
        {
            m_var = v;
            m_context = context;
            m_name = name;
        }
        
        /// <summary> The variable interface </summary>
        public virtual java.lang.String getName()
        {
            return (m_var == null)?m_name:(string)m_var.getName();
        }

        public virtual java.lang.String getDefiningClass()
        {
            return m_var.getDefiningClass();
        }

        public virtual int getAttributes()
        {
            return m_var.getAttributes();
        }

        public virtual bool isAttributeSet(int variableAttribute)
        {
            return m_var.isAttributeSet(variableAttribute);
        }

        public virtual Value getValue()
        {
            return m_var.getValue();
        }

        public virtual bool hasValueChanged(Session session)
        {
            return m_var.hasValueChanged(session);
        }

        public virtual FaultEvent setValue(Session session, int type, java.lang.String value)
        {
            return m_var.setValue(session, type, value);
        }

        public override String ToString()
        {
            return (m_var == null)?m_name:m_var.ToString();
        }

        public virtual bool needsToInvokeGetter()
        {
            return m_var.needsToInvokeGetter();
        }

        public virtual void  invokeGetter(Session session)
        {
            m_var.invokeGetter(session);
        }

        public int getIsolateId()
        {
            return this.getIsolateId();
        }
    }

}