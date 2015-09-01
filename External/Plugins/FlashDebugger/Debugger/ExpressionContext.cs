#if true
using System;
using ASCompletion.Context;
using ASCompletion.Model;
using flash.tools.debugger;
using flash.tools.debugger.concrete;
using flash.tools.debugger.expression;
using PluginCore.Localization;
using Object = java.lang.Object;
using String = java.lang.String;

namespace FlashDebugger
{
    public class ExpressionContext : Context
    {
        Session session;
        Frame frame;
        Value contextVal;

        public ExpressionContext(Session session, Frame frame, Value contextVal)
            : this(session, frame)
        {
            this.contextVal = contextVal;
        }
        public ExpressionContext(Session session, Frame frame)
        {
            this.session = session;
            this.frame = frame;
        }

        public void assign(Object par0, Value par1)
        {
            Variable var = lookup(par0) as Variable;
            if (var != null)
            {
                int type = var.getValue().getType();
                if (type == VariableType_.BOOLEAN || type == VariableType_.NUMBER || type == VariableType_.STRING)
                    var.setValue(session, par1.getType(), par1.getValueAsString());
                else
                    throw new NotSupportedException(TextHelper.GetString("Error.NoScalar"));
            } 
            else
                throw new NoSuchVariableException(System.String.Format(TextHelper.GetString("Error.NoSuchVariable"), par0));
        }

        public Context createContext(Object par0)
        {
            Value val;
            if (par0 is Variable) val = ((Variable)par0).getValue();
            else if (par0 is Value) val = (Value)par0;
            else if (par0 is String) val = DValue.forPrimitive(par0, getIsolateId());
            else throw new NotImplementedException();
            return new ExpressionContext(session, frame, val);
        }

        public void createPseudoVariables(bool par0)
        {
            throw new NotImplementedException();
        }

        public Session getSession()
        {
            return session;
        }

        public Object lookup(Object par0)
        {
            if (par0 is String)
            {
                if (null != contextVal)
                {
                    foreach (Variable v in contextVal.getMembers(session))
                    {
                        if (v.getName().Equals(par0)) return (Object)v;
                    }
                    throw new NoSuchVariableException(System.String.Format(TextHelper.GetString("Error.NoSuchVariable"), par0));
                }

                if ((String)par0 == "this")
                {
                    return (Object)frame.getThis(session);
                }
                foreach (Variable v in frame.getArguments(session))
                {
                    if (v.getName().Equals(par0)) return (Object)v;
                }
                foreach (Variable v in frame.getLocals(session))
                {
                    if (v.getName().Equals(par0)) return (Object)v;
                }
                foreach (Variable v in frame.getThis(session).getValue().getMembers(session))
                {
                    if (v.getName().Equals(par0)) return (Object)v;
                }
                foreach (Variable scope in frame.getScopeChain(session))
                {
                    foreach (Variable v in scope.getValue().getMembers(session))
                    {
                        if (v.getName().Equals(par0)) return (Object)v;
                    }
                }
                var fullClassName = findClassName((String)par0);
                if (null != fullClassName)
                {
                    return (Object)session.getGlobal(new String(fullClassName));
                }
            }
            throw new NoSuchVariableException(System.String.Format(TextHelper.GetString("Error.NoSuchVariable"), par0));
            //Value_.UNDEFINED;
        }

        public string findClassName(String className)
        {
            string endOfClassName = "." + className;

            MemberList imports = ASContext.Context.GetVisibleExternalElements();
            foreach (MemberModel member in imports)
            {
                if (member.Name == className || member.Name.EndsWith(endOfClassName))
                {
                    // If our member is some global variable/constant, return it
                    if ((member.Flags & (FlagType.Constant | FlagType.Variable)) > 0)
                        return member.Name;
                    
                    // Feeding a member that is not actually a class will cause the debugger to crash
                    if ((member.Flags & (FlagType.Class | FlagType.Interface)) == 0)
                        return null;

                    var lastPos = member.Type.LastIndexOf('.');
                    return lastPos > 0 ? member.Type.Substring(0, lastPos) + "::" + className : member.Type;
                }
            }
            return null;
        }

        public Object lookupMembers(Object par0)
        {
            String name = "?";
            Value val = null;

            if (par0 is Value)
            {
                val = (Value)par0;
            }
            
            if (par0 is Variable)
            {
                Variable var0 = (Variable)par0;
                name = var0.getName();
                val = var0.getValue();
            }

            if (val != null)
            {
                int type = val.getType();
                if (type == VariableType_.MOVIECLIP || type == VariableType_.OBJECT)
                {
                    String ret = name + " = " + FormatValue(val) + "\r\n";
                    foreach (Variable v in val.getMembers(session))
                    {
                        ret += " " + v.getName() + " = " + FormatValue(v.getValue()) + "\r\n";
                    }
                    return ret;
                }
                return new String(name + " = " + val.getValueAsString());
            }

            //NoSuchVariableException

            throw new NotImplementedException();
        }

        public String FormatValue(Value val)
        {
            String ret = "";
            if (val == null) return "null";
            int type = val.getType();
            if (type == VariableType_.MOVIECLIP || type == VariableType_.OBJECT)
            {
                ret = val.getTypeName();
            }
            else
            {
                ret = val.getValueAsString();
            }
            return ret;
        }

        public Value toValue(Object par0)
        {
            if (par0 is Value) return (Value)par0;
            if (par0 is Variable) return ((Variable)par0).getValue();
            var val = DValue.forPrimitive(par0, this.getIsolateId());
            return val;
        }

        public Value toValue()
        {
            return contextVal;
        }


        public int getIsolateId()
        {
            if (contextVal == null) return frame.getIsolateId();
            return contextVal.getIsolateId();
        }
    }
}
#else
using System;
using flash.tools.debugger;
using flash.tools.debugger.events;
using flash.tools.debugger.expression;


namespace FlashDebugger
{
    public class ExpressionContext : Context
    {
        public virtual Object Context
        {
            set { m_current = value; }
        }

        public Session getSession()
        {
            return Session;
        }

        public virtual Session Session
        {
            get { return m_session; }
        }

        public virtual String CurrentPackageName
        {
            get
            {
                Location loc = Session.getFrames()[Depth].getLocation();
                if (loc != null && loc.getFile() != null)
                {
                    return loc.getFile().getPackageName().Replace('\\', '.');
                }
                return null;
            }
        }

        public virtual int Depth
        {
            get { return m_depth; }
            set { m_depth = value; }
        }

        private int m_depth;
        private Session m_session;
        private Object m_current;
        private bool m_createIfMissing; // set if we need to create a variable if it doesn't exist
        private System.Collections.ArrayList m_namedPath;
        private bool m_nameLocked;
        private String m_newline = Environment.NewLine; //$NON-NLS-1$
        
        // used when evaluating an expression 
        public ExpressionContext(Session session)
        {
            m_depth = 0;
            m_session = session;
            m_current = null;
            m_createIfMissing = false;
            m_namedPath = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
            m_nameLocked = false;
        }
        
        internal virtual void  pushName(String name)
        {
            if (m_nameLocked || name.Length == 0) return;
            m_namedPath.Add(name);
        }

        internal virtual bool setName(String name)
        {
            if (m_nameLocked) return true;
            m_namedPath.Clear(); pushName(name); return true;
        }

        internal virtual void  lockName()
        {
            m_nameLocked = true;
        }
        
        public virtual String getName()
        {
            int size = m_namedPath.Count;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < size; i++)
            {
                String s = (String) m_namedPath[i];
                if (i > 0) sb.Append('.');
                sb.Append(s);
            }
            return (sb.ToString());
        }
        
        // Start of Context API implementation 
        public virtual void  createPseudoVariables(bool oui)
        {
            m_createIfMissing = oui;
        }
        
        // create a new context object by combinging the current one and o 
        public virtual Context createContext(Object o)
        {
            ExpressionContext c = new ExpressionContext(m_session);
            c.Context = o;
            c.createPseudoVariables(m_createIfMissing);
            c.m_namedPath.AddRange(m_namedPath);
            return c;
        }
        
        // assign the object o, the value v; returns Boolean true if worked, false if failed
        public virtual Object assign(Object o, Object v)
        {
            bool worked = false;
            try
            {
                Variable var = resolveToVariable(o);
                if (var == null) throw new NoSuchVariableException((java.lang.Object)m_current);
                // set the value, for the case of a variable that does not exist it will not have a type
                // so we try to glean one from v.
                int type = determineType(var, v);
                FaultEvent faultEvent = var.setValue(Session, type, v.ToString());
                if (faultEvent != null) throw new PlayerFaultException(faultEvent);
                worked = true;
            }
            catch (PlayerDebugException)
            {
                worked = false;
            }
            return worked;
        }
        
        /// <summary> The Context interface which goes out and gets values from the session
        /// Expressions use this interface as a means of evaluation.
        /// 
        /// We also use this to create a reference to internal variables.
        /// </summary>
        public virtual Object lookup(Object o)
        {
            Object result = null;
            try
            {
                if ((result = resolveToVariable(o)) != null)
                {

                }
                // or value
                else if ((result = resolveToValue(o)) != null)
                {

                }
                else throw new NoSuchVariableException(o);
                // take on the path to the variable; so 'what' command prints something nice
                if ((result != null) && result is VariableFacade)
                {
                    ((VariableFacade) result).Path = getName();
                }
                // if the attempt to get the variable's value threw an exception inside the
                // player (most likely because the variable is actually a getter, and the
                // getter threw something), then throw something here
                Value resultValue = null;
                if (result is Variable) resultValue = ((Variable) result).getValue();
                else if (result is Value) resultValue = (Value) result;
                if (resultValue != null)
                {
                    if (resultValue.isAttributeSet(ValueAttribute.IS_EXCEPTION))
                    {
                        String value = resultValue.ValueAsString;
                        throw new PlayerFaultException(new ExceptionFault(value));
                    }
                }
            }
            catch (PlayerDebugException)
            {
                result = null; // null object
            }
            return result;
        }
        
        /* returns a string consisting of formatted member names and values */
        public virtual Object lookupMembers(Object o)
        {
            Variable var = null;
            Value val = null;
            Variable[] mems = null;
            try
            {
                var = resolveToVariable(o);
                if (var != null) val = var.getValue();
                else val = resolveToValue(o);
                mems = val.getMembers(Session);
            }
            catch (NullReferenceException)
            {
                throw new NoSuchVariableException(o);
            }
            catch (PlayerDebugException)
            {
                throw new NoSuchVariableException(o); // not quite right...
            }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // [mmorearty] experimenting with hierarchical display of members
            String[] classHierarchy = val.getClassHierarchy(false);
            if (classHierarchy != null && Session.getPreference(SessionManager.PREF_HIERARCHICAL_VARIABLES) != 0)
            {
                for (int c = 0; c < classHierarchy.Length; ++c)
                {
                    String classname = classHierarchy[c];
                    sb.Append(m_newline + "(Members of " + classname + ")"); //$NON-NLS-1$ //$NON-NLS-2$
                    for (int i = 0; i < mems.Length; ++i)
                    {
                        if (classname.Equals(mems[i].getDefiningClass()))
                        {
                            sb.Append(m_newline + " "); //$NON-NLS-1$
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < mems.Length; i++)
                {
                    sb.Append(m_newline + " "); //$NON-NLS-1$
                }
            }
            return sb.ToString();
        }
        
        // determine the type from the VariableFacade or use the value object
        internal virtual int determineType(Variable var, Object value)
        {
            int type = VariableType.UNKNOWN;
            if (var is VariableFacade && ((VariableFacade) var).Variable == null)
            {
                if (value is ValueType) type = VariableType.NUMBER;
                else if (value is Boolean) type = VariableType.BOOLEAN;
                else type = VariableType.STRING;
            }
            else type = var.getValue().getType();
            return type;
        }
        
        
        /// <summary> Resolve the object into a variable by various means and 
        /// using the current context.
        /// </summary>
        /// <returns> variable, or <code>null</code>
        /// </returns>
        internal virtual Variable resolveToVariable(Object o)
        {
            Variable v = null;
            // if o is a variable already, then we're done!
            if (o is Variable) return (Variable) o;
            // Resolve the name to something
            {
                // not an id so try as name 
                String name = o.ToString();
                long id = nameAsId(name);
                /*
                * if #N was used just pick up the variable, otherwise
                * we need to use the current context to resolve 
                * the name to a member
                */
                if (id != Value.UNKNOWN_ID)
                {
                    // TODO what here?
                }
                else
                {
                    // try to resolve as a member of current context (will set context if null)
                    id = determineContext(name);
                    v = locateForNamed((int) id, name, true);
                    if (v != null) v = new VariableFacade(v, id);
                    else if (v == null && m_createIfMissing && name[0] != '$')
                    {
                        v = new VariableFacade(id, name);
                    }
                }
            }
            return v;
        }
        
        /*
        * Resolve the object into a variable by various means and 
        * using the current context.
        */
        internal virtual Value resolveToValue(Object o)
        {
            Value v = null;
            // if o is a variable or a value already, then we're done!
            if (o is Value) return (Value) o;
            else if (o is Variable) return ((Variable) o).getValue();
            {
                // not an id so try as name 
                String name = o.ToString();
                long id = nameAsId(name);
                /*
                * if #N was used just pick up the variable, otherwise
                * we need to use the current context to resolve 
                * the name to a member
                */
                if (id != Value.UNKNOWN_ID)
                {
                    v = Session.getValue((int) id);
                }
                else
                {
                    // TODO what here?
                }
            }
            return v;
        }
        
        // special code for #N support. I.e. naming a variable via an ID
        internal virtual long nameAsId(String name)
        {
            long id = Value.UNKNOWN_ID;
            try
            {
                if (name[0] == '#') id = Int64.Parse(name.Substring(1));
            }
            catch (Exception)
            {
                id = Value.UNKNOWN_ID;
            }
            return id;
        }
        
        /// <summary> Using the given id as a parent find the member named
        /// name.
        /// </summary>
        /// <throws>  NoSuchVariableException if id is UNKNOWN_ID </throws>
        internal virtual Variable memberNamed(long id, String name)
        {
            Variable v = null;
            Value parent = Session.getValue((int) id);
            if (parent == null) throw new NoSuchVariableException(name);
            /* got a variable now return the member if any */
            v = parent.getMemberNamed(Session, name);
            return v;
        }
        
        /// <summary> 
        /// All the really good stuff about finding where name exists goes here!
        /// 
        /// If name is not null, then it implies that we use the existing
        /// m_current to find a member of m_current.  If m_current is null
        /// Then we need to probe variable context points attempting to locate
        /// name.  When we find a match we set the m_current to this context
        /// 
        /// If name is null then we simply return the current context.
        /// </summary>
        internal virtual int determineContext(String name)
        {
            long id = Value_.UNKNOWN_ID;
            // have we already resolved our context...
            if (m_current != null)
            {
                Object value;
                if (m_current is Variable) value = ((Variable) m_current).getValue().getValueAsObject();
                else if (m_current is Value) value = ((Value) m_current).getValueAsObject();
                else value = m_current;
                try
                {
                    id = ArithmeticExp.toLong(value);
                }
                catch (FormatException)
                {
                    id = Value.UNKNOWN_ID;
                }
            }
            // nothing to go on, so we're done
            else if (name == null)
            {

            }
            // use the name and try and resolve where we are...
            else
            {
                // Each stack frame has a root variable under (BASE_ID-depth)
                // where depth is the depth of the stack.
                // So we query for our current stack depth and use that 
                // as the context for our base computation
                int baseId = Value.BASE_ID - Depth;
                // obtain data about our current state 
                Variable contextVar = null;
                Value contextVal = null;
                Value val = null;
                // look for 'name' starting from local scope
                if ((val = locateParentForNamed(baseId, name, false)) != null)
                {

                }
                // get the this pointer, then look for 'name' starting from that point
                else if (((contextVar = locateForNamed(baseId, "this", false)) != null) && (setName("this") && (val = locateParentForNamed(contextVar.getValue().Id, name, true)) != null))
                //$NON-NLS-1$
                {

                }
                // now try to see if 'name' exists off of _root
                else if (setName("_root") && (val = locateParentForNamed((int)Value_.ROOT_ID, name, true)) != null)
                //$NON-NLS-1$
                {

                }
                // now try to see if 'name' exists off of _global
                else if (setName("_global") && (val = locateParentForNamed((int)Value_.GLOBAL_ID, name, true)) != null)
                //$NON-NLS-1$
                {

                }
                // now try off of class level, if such a thing can be found
                else if (((contextVal = locate((int)Value_.GLOBAL_ID, CurrentPackageName, false)) != null) && (setName("_global." + CurrentPackageName) && (val = locateParentForNamed(contextVal.Id, name, true)) != null))
                //$NON-NLS-1$
                {

                }
                // if we found it then stake this as our context!
                if (val != null)
                {
                    id = val.getId()
                    pushName(name);
                    lockName();
                }
            }
            return (int) id;
        }
        
        /// <summary> 
        /// Performs a search for a member with the given name using the
        /// given id as the parent variable.
        /// 
        /// If a match is found then, we return the parent variable of
        /// the member that matched.  The proto chain is optionally traversed.
        /// 
        /// No exceptions are thrown
        /// </summary>
        internal virtual Value locateParentForNamed(long id, String name, bool traverseProto)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Variable var = null;
            Value val = null;
            try
            {
                var = memberNamed(id, name);
                // see if we need to traverse the proto chain
                while (var == null && traverseProto)
                {
                    // first attempt to get __proto__, then resolve name
                    Variable proto = memberNamed(id, "__proto__"); //$NON-NLS-1$
                    sb.Append("__proto__"); //$NON-NLS-1$
                    if (proto == null) traverseProto = false;
                    else
                    {
                        id = proto.getValue().getId();
                        var = memberNamed(id, name);
                        if (var == null) sb.Append('.');
                    }
                }
            }
            catch (NoSuchVariableException)
            {
                // don't worry about this one, it means variable with id couldn't be found
            }
            catch (NullReferenceException)
            {
                // probably no session
            }
            // what we really want is the parent not the child variable
            if (var != null)
            {
                pushName(sb.ToString());
                val = Session.getValue(id);
            }
            return val;
        }
        
        // variant of locateParentForNamed, whereby we return the child variable
        internal virtual Variable locateForNamed(int id, String name, bool traverseProto)
        {
            Variable var = null;
            Value v = locateParentForNamed(id, name, traverseProto);
            if (v != null)
            {
                try
                {
                    var = memberNamed(v.Id, name);
                }
                catch (NoSuchVariableException)
                {
                    v = null;
                }
            }
            return var;
        }
        
        /// <summary> Locates the member via a dotted name starting at the given id.
        /// It will traverse any and all proto chains if necc. to find the name.
        /// </summary>
        internal virtual Value locate(int startingId, String dottedName, bool traverseProto)
        {
            if (dottedName == null) return null;
            // first rip apart the dottedName
            SupportClass.Tokenizer names = new SupportClass.Tokenizer(dottedName, "."); //$NON-NLS-1$
            Value val = Session.getValue(startingId);
            while (names.HasMoreTokens() && val != null)
            {
                val = locateForNamed(val.Id, names.NextToken(), traverseProto).getValue();
            }
            return val;
        }

    }

}
#endif