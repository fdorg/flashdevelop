// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2006-2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using Flash.Tools.Debugger.Expression;
using ArrayUtil = Flash.Util.ArrayUtil;

namespace Flash.Tools.Debugger.Concrete
{
	
	/// <author>  mmorearty
	/// </author>
	public class DValue : Value
	{
		public override int Id
		{
			/*
			* @see Flash.Tools.Debugger.Value#getId()
			*/
			
			get
			{
				// see if we support an id concept
				if (m_value is Int64)
					return (int) ((Int64) m_value);
				else
					return Flash.Tools.Debugger.Value.UNKNOWN_ID;
			}
			
		}
        public virtual bool MembersObtained
		{
			set
			{
				if (value)
				{
					if (m_members == null)
					{
						m_members = (System.Collections.IDictionary) new System.Collections.Hashtable();
					}
				}
				else
				{
					m_members = null;
				}
			}
			
		}
        public override Object ValueAsObject
		{
			/*
			* @see Flash.Tools.Debugger.Value#getValueAsObject()
			*/
			
			get
			{
				return m_value;
			}
			
		}
        public override String ValueAsString
		{
			/*
			* @see Flash.Tools.Debugger.Value#getValueAsString()
			*/
			
			get
			{
				if (m_value == null)
					return "undefined"; //$NON-NLS-1$
				
				if (m_value is Double)
				{
					// Java often formats whole numbers in ugly ways.  For example,
					// the number 3 might be formatted as "3.0" and, even worse,
					// the number 12345678 might be formatted as "1.2345678E7" !
					// So, if the number has no fractional part, then we override
					// the default display behavior.
					double value = ((Double) m_value);
					long longValue = (long) value;
					if (value == longValue)
						return Convert.ToString(longValue);
				}
				
				return m_value.ToString();
			}
			
		}
        public virtual Object Value
		{
			set
			{
				m_value = value;
			}
			
		}
		/// <seealso cref="VariableType">
		/// </seealso>
		private int m_type;

		/// <seealso cref="Flash.Tools.Debugger.Value.getTypeName()">
		/// </seealso>
		private String m_typeName;

		/// <seealso cref="Flash.Tools.Debugger.Value.getClassName()">
		/// </seealso>
		private String m_className;
		
		/// <seealso cref="ValueAttribute">
		/// </seealso>
		private int m_attribs;
		
		/// <summary>Maps "varname" (without its namespace) to a Variable </summary>
		private System.Collections.IDictionary m_members;
		
		/// <summary> Either my own ID, or else my parent's ID if I am <code>__proto__</code>.</summary>
		internal int m_nonProtoId;
		
		/// <summary> <code>m_value</code> can have one of several possible meanings:
		/// 
		/// <ul>
		/// <li> If this variable's value is an <code>Object</code> or a <code>MovieClip</code>,
		/// then <code>m_value</code> contains the ID of the <code>Object</code> or
		/// <code>MovieClip</code>, stored as a <code>Long</code>. </li>
		/// <li> If this variable refers to a Getter which has not yet been invoked, then
		/// <code>m_value</code> contains the ID of the Getter, stored as a
		/// <code>Long</code>. </li>
		/// <li> Otherwise, this variable's value is a simple type such as <code>int</code> or
		/// <code>String</code>, in which case <code>m_value</code> holds the actual value.</li>
		/// </ul>
		/// </summary>
		private Object m_value;
		
		/// <summary> The list of classes that contributed members to this object, from
		/// the class itself all the way down to Object.
		/// </summary>
		private String[] m_classHierarchy;
		
		/// <summary> How many members of <code>m_classHierarchy</code> actually contributed
		/// members to this object.
		/// </summary>
		private int m_levelsWithMembers;
		
		/// <summary> Create a top-level variable which has no parent.  This may be used for
		/// _global, _root, stack frames, etc.
		/// 
		/// </summary>
		/// <param name="id">the ID of the variable
		/// </param>
		public DValue(long id)
		{
			init(VariableType.UNKNOWN, null, null, 0, (Object) id);
		}
		
		/// <summary> Create a value.
		/// 
		/// </summary>
		/// <param name="type">see <code>VariableType</code>
		/// </param>
		/// <param name="typeName">
		/// </param>
		/// <param name="className">
		/// </param>
		/// <param name="attribs">the attributes of this value; see <code>ValueAttribute</code>
		/// </param>
		/// <param name="value">for an Object or MovieClip, this should be a Long which contains the
		/// ID of this variable.  For a variable of any other type, such as integer
		/// or string, this should be the value of the variable.
		/// </param>
		public DValue(int type, String typeName, String className, int attribs, Object value)
		{
			init(type, typeName, className, attribs, value);
		}
		
		/// <summary> Initialize a variable.
		/// 
		/// For the meanings of the arguments, see the DVariable constructor.
		/// </summary>
		private void  init(int type, String typeName, String className, int attribs, Object value)
		{
			m_type = type;
			m_typeName = typeName;
			m_className = className;
			m_attribs = attribs;
			m_value = value;
			m_members = null;
			m_nonProtoId = Id;
		}
		
		/*
		* @see Flash.Tools.Debugger.Value#getAttributes()
		*/
		public override int getAttributes()
		{
			return m_attribs;
		}
		
		/*
		* @see Flash.Tools.Debugger.Value#getClassName()
		*/
		public override String getClassName()
		{
			return m_className;
		}
		
		/*
		* @see Flash.Tools.Debugger.Value#getMemberCount(Flash.Tools.Debugger.Session)
		*/
		public override int getMemberCount(Session s)
		{
			obtainMembers(s);
			return (m_members == null)?0:m_members.Count;
		}
		
		/*
		* @see Flash.Tools.Debugger.Value#getMemberNamed(Flash.Tools.Debugger.Session, java.lang.String)
		*/
		public override Variable getMemberNamed(Session s, String name)
		{
			obtainMembers(s);
			return findMember(name);
		}
		
		/*
		* @see Flash.Tools.Debugger.Value#getClassHierarchy(boolean)
		*/
		public override String[] getClassHierarchy(bool allLevels)
		{
			if (allLevels)
			{
				return m_classHierarchy;
			}
			else
			{
				String[] partialClassHierarchy;
				
				if (m_classHierarchy != null)
				{
					partialClassHierarchy = new String[m_levelsWithMembers];
					Array.Copy(m_classHierarchy, 0, partialClassHierarchy, 0, m_levelsWithMembers);
				}
				else
				{
					partialClassHierarchy = new String[0];
				}
				return partialClassHierarchy;
			}
		}
		
		/* TODO should this really be public? */
        public virtual DVariable findMember(String named)
		{
			if (m_members == null)
				return null;
			else
				return (DVariable) m_members[named];
		}
		
		/*
		* @see Flash.Tools.Debugger.Value#getMembers(Flash.Tools.Debugger.Session)
		*/
		public override Variable[] getMembers(Session s)
		{
			obtainMembers(s);
			
			/* find out the size of the array */
			int count = getMemberCount(s);
			DVariable[] ar = new DVariable[count];
			
			if (count > 0)
			{
                m_members.Values.CopyTo(ar, 0);
				
				// sort the member list by name
				ArrayUtil.sort(ar);
			}
			
			return ar;
		}
		
		/// <summary> WARNING: this call will initiate a call to the session to obtain the members
		/// the first time around.
		/// </summary>
		/// <throws>  NotConnectedException </throws>
		/// <throws>  NoResponseException </throws>
		/// <throws>  NotSuspendedException </throws>
		private void  obtainMembers(Session s)
		{
			if (m_members == null && s != null)
			{
				// performing a get on this variable obtains all its members
				int id = Id;
				if (id != Flash.Tools.Debugger.Value.UNKNOWN_ID)
				{
					if (((PlayerSession) s).getRawValue(id) == this)
						((PlayerSession) s).obtainMembers(id);
					if (m_members != null)
					{
                        foreach (Object member in m_members.Values)
						{
							if (member is DVariable)
							{
                                ((DVariable)member).Session = s;
							}
						}
					}
				}
			}
		}

        public virtual bool membersObtained()
		{
			return (Id == Flash.Tools.Debugger.Value.UNKNOWN_ID || m_members != null);
		}

        public virtual void addMember(DVariable v)
		{
			if (m_members == null)
			{
				m_members = new System.Collections.Hashtable();
			}
			
			// if we are a proto member house away our original parent id
			String name = v.getName();
			DValue val = (DValue) v.getValue();
			val.m_nonProtoId = (name != null && name.Equals("__proto__"))?m_nonProtoId:val.Id; //$NON-NLS-1$ // TODO is this right?
			v.m_nonProtoParentId = m_nonProtoId;
			
			m_members[name] = v;
		}

        public virtual void removeAllMembers()
		{
			m_members = null;
		}
		
		/*
		* @see Flash.Tools.Debugger.Value#getType()
		*/
        public override int getType()
		{
			return m_type;
		}
		
		/*
		* @see Flash.Tools.Debugger.Value#getTypeName()
		*/
		public override String getTypeName()
		{
			return m_typeName;
		}
		
		/*
		* @see Flash.Tools.Debugger.Value#isAttributeSet(int)
		*/
		public override bool isAttributeSet(int variableAttribute)
		{
			return (m_attribs & variableAttribute) != 0;
		}

        public virtual void setTypeName(String s)
		{
			m_typeName = s;
		}
        public virtual void setClassName(String s)
		{
			m_className = s;
		}
        public virtual void setType(int t)
		{
			m_type = t;
		}
        public virtual void setAttributes(int f)
		{
			m_attribs = f;
		}

        public virtual void setClassHierarchy(String[] classHierarchy, int levelsWithMembers)
		{
			m_classHierarchy = classHierarchy;
			m_levelsWithMembers = levelsWithMembers;
		}

        public virtual String membersToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			/* find out the size of the array */
			if (m_members == null)
				sb.Append(PlayerSessionManager.LocalizationManager.getLocalizedTextString("empty"));
			//$NON-NLS-1$
			else
			{
                foreach (DVariable sf in m_members.Values)
				{
					sb.Append(sf);
					sb.Append(",\n"); //$NON-NLS-1$
				}
			}
			return sb.ToString();
		}
		
		/// <summary> Necessary for expression evaluation.</summary>
		/// <seealso cref="Context.lookup(Object)">
		/// </seealso>
		public override String ToString()
		{
			return ValueAsString;
		}
	}
}