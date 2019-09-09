// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        internal string m_name;
        internal string m_path;

        public java.lang.String getQualifiedName() => m_var.getQualifiedName();

        public java.lang.String getNamespace() => m_var.getNamespace();

        public int getLevel() => m_var.getLevel();

        public int getScope() => m_var.getScope();

        public virtual string Path
        {
            get => m_path;
            set => m_path = value;
        }

        /// <summary> Our lone get context (i.e. parent) interface </summary>
        public virtual int Context => (int) m_context;

        public virtual Variable Variable => m_var;

        public VariableFacade(Variable v, long context) => Init(context, v, null);

        public VariableFacade(long context, string name) => Init(context, null, name);

        internal virtual void Init(long context, Variable v, string name)
        {
            m_var = v;
            m_context = context;
            m_name = name;
        }
        
        /// <summary> The variable interface </summary>
        public virtual java.lang.String getName() => (m_var is null)?m_name:(string)m_var.getName();

        public virtual java.lang.String getDefiningClass() => m_var.getDefiningClass();

        public virtual int getAttributes() => m_var.getAttributes();

        public virtual bool isAttributeSet(int variableAttribute) => m_var.isAttributeSet(variableAttribute);

        public virtual Value getValue() => m_var.getValue();

        public virtual bool hasValueChanged(Session session) => m_var.hasValueChanged(session);

        public virtual FaultEvent setValue(Session session, int type, java.lang.String value) => m_var.setValue(session, type, value);

        public override string ToString() => m_var is null ? m_name : m_var.ToString();

        public virtual bool needsToInvokeGetter() => m_var.needsToInvokeGetter();

        public virtual void  invokeGetter(Session session) => m_var.invokeGetter(session);

        public int getIsolateId() => getIsolateId();
    }
}