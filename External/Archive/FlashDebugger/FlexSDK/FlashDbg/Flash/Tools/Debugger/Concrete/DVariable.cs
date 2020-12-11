// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2003-2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using Context = Flash.Tools.Debugger.Expression.Context;
using FaultEvent = Flash.Tools.Debugger.Events.FaultEvent;

namespace Flash.Tools.Debugger.Concrete
{
	
	public class DVariable : Variable, IComparable
	{
		virtual public String Namespace
		{
			get
			{
				return m_namespace;
			}
			
		}
		virtual public int Scope
		{
			get
			{
				return m_attribs & VariableAttribute.SCOPE_MASK;
			}
			
		}
		virtual public int Level
		{
			get
			{
				return m_level;
			}
			
		}
		virtual public int Attribute
		{
			set
			{
				if ((value & VariableAttribute.SCOPE_MASK) == value)
					m_attribs = (m_attribs & ~ VariableAttribute.SCOPE_MASK) | value;
				else
					m_attribs |= value;
			}
			
		}
		virtual public String RawName
		{
			get
			{
				return m_rawName;
			}
			
		}
		virtual public String QualifiedName
		{
			/*
			* @see Flash.Tools.Debugger.Variable#getQualifiedName()
			*/
			
			get
			{
				if (m_namespace.Length > 0)
					return m_namespace + "::" + m_name;
				//$NON-NLS-1$
				else
					return m_name;
			}
			
		}
		virtual public Session Session
		{
			set
			{
				m_session = value;
			}
			
		}
		/// <summary> The raw name, exactly as it came back from the Player.  For example, this
		/// might be <code>mynamespace@12345678::myvar</code>, which indicates that
		/// the variable is in namespace "mynamespace", which has atom 12345678.
		/// </summary>
		private String m_rawName;
		
		/// <summary>Just name, without namespace </summary>
		private String m_name;
		
		/// <seealso cref="Variable.Namespace">
		/// </seealso>
		private String m_namespace = ""; //$NON-NLS-1$
		
		/// <seealso cref="VariableAttribute">
		/// </seealso>
		private int m_attribs;
		
		/// <summary> The variable's value.</summary>
		protected internal Value m_value;
		
		/// <summary> Whether we have fired the getter for this value.  Only applicable if
		/// the VariableAttribute.HAS_GETTER attribute is set.
		/// </summary>
		private bool m_firedGetter;
		
		/// <summary> The class in which this member was actually defined.  For example, if class
		/// B extends class A, and class A has member variable V, then for variable
		/// V, the defining class is always "A", even though the parent variable might
		/// be an instance of class B.
		/// </summary>
		private String m_definingClass;
		
		/// <summary> The variable's "level" -- see <code>Variable.getLevel()</code></summary>
		/// <seealso cref="Variable.Level">
		/// </seealso>
		private sbyte m_level;
		
		/// <summary> The session object that was used when creating this variable, if known.</summary>
		private Session m_session;
		
		/// <summary> My parent's <code>m_nonProtoId</code>.  In other words, either my
		/// parent's ID, or else my parent's parent's ID if my parent is <code>__proto__</code>.
		/// </summary>
		internal int m_nonProtoParentId;
		
		/// <summary> Create a variable and its value.
		/// 
		/// </summary>
		/// <param name="name">the name of the variable within the context of its parent.  For example,
		/// when resolving member "bar" of object "foo", the name will be "bar".
		/// </param>
		/// <param name="value">the variable's value.
		/// </param>
		public DVariable(String name, DValue value)
		{
			m_rawName = name;
			
			// If the name contains "::", then the name is of the form "namespace::name"
			if (name != null)
			{
				int doubleColon = name.LastIndexOf("::"); //$NON-NLS-1$
				if (doubleColon >= 0)
				{
					m_namespace = name.Substring(0, (doubleColon) - (0));
					int at = m_namespace.IndexOf('@');
					if (at != - 1)
						m_namespace = m_namespace.Substring(0, (at) - (0));
					name = name.Substring(doubleColon + 2);
				}
			}
			
			m_name = name;
			m_attribs = value.getAttributes();
			m_nonProtoParentId = Value.UNKNOWN_ID;
			m_value = value;
		}
		
		/* getters/setters */
		public virtual String getName()
		{
			return m_name;
		}
		public virtual int getAttributes()
		{
			return m_attribs;
		}
		public virtual String getDefiningClass()
		{
			return m_definingClass;
		}
		
		public virtual void  makePublic()
		{
			int attributes = getAttributes();
			attributes &= ~ VariableAttribute.SCOPE_MASK;
			attributes |= VariableAttribute.PUBLIC_SCOPE;
			setAttributes(attributes);
			
			m_namespace = ""; //$NON-NLS-1$
		}
		
		/*
		* @see Flash.Tools.Debugger.Variable#getValue()
		*/
		public virtual Value getValue()
		{
			if (m_session != null && m_session.getPreference(SessionManager.PREF_INVOKE_GETTERS) != 0)
			{
				try
				{
					invokeGetter();
				}
				catch (NotSuspendedException)
				{
					// fall through -- return raw value without invoking getter
				}
				catch (NoResponseException)
				{
					// fall through -- return raw value without invoking getter
				}
				catch (NotConnectedException)
				{
					// fall through -- return raw value without invoking getter
				}
			}
			
			return m_value;
		}
		
		/*
		* @see Flash.Tools.Debugger.Variable#hasValueChanged(Flash.Tools.Debugger.Session)
		*/
		public virtual bool hasValueChanged()
		{
			bool hasValueChanged = false;

			if (m_session != null && m_session is PlayerSession)
			{
				Value previousParent = ((PlayerSession) m_session).getPreviousValue(m_nonProtoParentId);
				if (previousParent != null)
				{
					try
					{
						Variable previousMember = previousParent.getMemberNamed(null, getName());
						// If the old variable had a getter but never invoked that getter,
						// then it's too late, we don't know the old value. 
						if (previousMember is DVariable && !previousMember.needsToInvokeGetter())
						{
							Value previousValue = ((DVariable) previousMember).m_value;
							if (previousValue != null)
							{
								String previousValueAsString = previousValue.ValueAsString;
								if (previousValueAsString != null)
								{
									if (!previousValueAsString.Equals(getValue().ValueAsString))
									{
										hasValueChanged = true;
									}
								}
							}
						}
					}
					catch (PlayerDebugException)
					{
						// ignore
					}
				}
			}
			return hasValueChanged;
		}
		
		/*
		* @see Flash.Tools.Debugger.Session#setScalarMember(int, java.lang.String, int, java.lang.String)
		*/
		public virtual FaultEvent setValue(int type, String value)
		{
			FaultEvent fault = null;

			if (m_session != null && m_session is PlayerSession)
			{
				fault = ((PlayerSession) m_session).setScalarMember(m_nonProtoParentId, m_rawName, type, value);

				if (fault == null)
				{
					m_firedGetter = false;

					if (!needsToInvokeGetter())
					{
						DValue debugValue = m_value as DValue;

						if (debugValue.getType() == VariableType.BOOLEAN)
						{
							debugValue.Value = value.ToLower() == "true";
						}
						else if (debugValue.getType() == VariableType.NUMBER)
						{
							debugValue.Value = Double.Parse(value);
						}
						else if (debugValue.getType() == VariableType.STRING)
						{
							debugValue.Value = value;
						}
					}
				}
			}

			return fault;
		}
		
		/*
		* @see Flash.Tools.Debugger.Variable#isAttributeSet(int)
		*/
		public virtual bool isAttributeSet(int att)
		{
			if ((att & VariableAttribute.SCOPE_MASK) == att)
				return (Scope == att);
			else
				return (((getAttributes() & att) == att)?true:false);
		}
		
		public virtual void  clearAttribute(int att)
		{
			if ((att & VariableAttribute.SCOPE_MASK) == att)
				m_attribs = (m_attribs & ~ VariableAttribute.SCOPE_MASK) | VariableAttribute.PUBLIC_SCOPE;
			else
				m_attribs &= ~ att;
		}
		
		/// <summary> Comparator interface for sorting Variables</summary>
		public virtual int CompareTo(Object o2)
		{
			Variable v2 = (Variable) o2;
			
			String n1 = getName();
			String n2 = v2.getName();
			
			return String.Compare(n1, n2, true);
		}
		
		/*
		* @see Flash.Tools.Debugger.Variable#needsToFireGetter()
		*/
		public virtual bool needsToInvokeGetter()
		{
			// If this variable has a getter, and the getter has not yet been invoked
			return (isAttributeSet(VariableAttribute.HAS_GETTER) && m_value.Id != Value.UNKNOWN_ID && !m_firedGetter);
		}
		
		/*
		* @see Flash.Tools.Debugger.Value#invokeGetter(Flash.Tools.Debugger.Session)
		*/
		public virtual void  invokeGetter()
		{
			if (needsToInvokeGetter())
			{
				if (m_session != null && m_session is PlayerSession)
				{
					PlayerSession playerSession = (PlayerSession)m_session;

					// If this Variable is stale (that is, the program has run since this Variable
					// was created), then we can't invoke the getter.
					if (playerSession.getRawValue(m_value.Id) == m_value)
					{
						// temporarily turn on "invoke getters" preference
						int oldInvokeGetters = playerSession.getPreference(SessionManager.PREF_INVOKE_GETTERS);
						playerSession.setPreference(SessionManager.PREF_INVOKE_GETTERS, 1);

						try
						{
							// fire the getter using the original object id. make sure we get something reasonable back
							Value v = playerSession.getValue(m_nonProtoParentId, RawName);
							if (v != null)
							{
								m_value = v;
								m_firedGetter = true;
							}
						}
						finally
						{
							playerSession.setPreference(SessionManager.PREF_INVOKE_GETTERS, oldInvokeGetters);
						}
					}
				}
			}
		}
		
		public virtual void  setName(String s)
		{
			m_name = s;
		}
		public virtual void  setAttributes(int f)
		{
			m_attribs = f; ((DValue) getValue()).setAttributes(f);
		}
		
		public virtual void  setDefiningClass(int level, String definingClass)
		{
			m_level = (sbyte) Math.Min(level, 255);
			m_definingClass = definingClass;
		}
		
		/// <summary> Added so that expressions such as <code>a.b.c = e.f</code> work in the command-line interface.</summary>
		/// <seealso cref="Context.lookup(Object)">
		/// </seealso>
		public override String ToString()
		{
			return getValue().ValueAsString;
		}
		
		/// <summary> Return the internal player string type representation for this variable.
		/// Currently used for passing in the type to the Player when doing
		/// a set variable command
		/// </summary>
		public static String typeNameFor(int type)
		{
			String s = "string"; //$NON-NLS-1$
			switch (type)
			{
				
				case VariableType.NUMBER: 
					s = "number"; //$NON-NLS-1$
					break;
				
				
				case VariableType.BOOLEAN: 
					s = "boolean"; //$NON-NLS-1$
					break;
				
				
				case VariableType.STRING: 
					s = "string"; //$NON-NLS-1$
					break;
				
				
				case VariableType.OBJECT: 
					s = "object"; //$NON-NLS-1$
					break;
				
				
				case VariableType.FUNCTION: 
					s = "function"; //$NON-NLS-1$
					break;
				
				
				case VariableType.MOVIECLIP: 
					s = "movieclip"; //$NON-NLS-1$
					break;
				
				
				case VariableType.NULL: 
					s = "null"; //$NON-NLS-1$
					break;
				
				
				case VariableType.UNDEFINED: 
				case VariableType.UNKNOWN: 
				default: 
					s = "undefined"; //$NON-NLS-1$
					break;
				}
			return s;
		}
		
		/// <summary> These values are obtained directly from the Player.
		/// See ScriptObject in splay.h.
		/// </summary>
		public const int kNormalObjectType = 0;
		public const int kXMLSocketObjectType = 1;
		public const int kTextFieldObjectType = 2;
		public const int kButtonObjectType = 3;
		public const int kNumberObjectType = 4;
		public const int kBooleanObjectType = 5;
		public const int kNativeStringObject = 6;
		public const int kNativeArrayObject = 7;
		public const int kDateObjectType = 8;
		public const int kSoundObjectType = 9;
		public const int kNativeXMLDoc = 10;
		public const int kNativeXMLNode = 11;
		public const int kNativeCameraObject = 12;
		public const int kNativeMicrophoneObject = 13;
		public const int kNativeCommunicationObject = 14;
		public const int kNetConnectionObjectType = 15;
		public const int kNetStreamObjectType = 16;
		public const int kVideoObjectType = 17;
		public const int kTextFormatObjectType = 18;
		public const int kSharedObjectType = 19;
		public const int kSharedObjectDataType = 20;
		public const int kPrintJobObjectType = 21;
		public const int kMovieClipLoaderObjectType = 22;
		public const int kStyleSheetObjectType = 23;
		public const int kFapPacketDummyObject = 24;
		public const int kLoadVarsObject = 25;
		public const int kTextSnapshotType = 26;
		
		public static String classNameFor(long clsType, bool isMc)
		{
			String clsName;
			switch ((int) clsType)
			{
				
				case kNormalObjectType: 
					clsName = (isMc)?"MovieClip":"Object"; //$NON-NLS-1$ //$NON-NLS-2$
					break;
				
				case kXMLSocketObjectType: 
					clsName = "XMLSocket"; //$NON-NLS-1$
					break;
				
				case kTextFieldObjectType: 
					clsName = "TextField"; //$NON-NLS-1$
					break;
				
				case kButtonObjectType: 
					clsName = "Button"; //$NON-NLS-1$
					break;
				
				case kNumberObjectType: 
					clsName = "Number"; //$NON-NLS-1$
					break;
				
				case kBooleanObjectType: 
					clsName = "Boolean"; //$NON-NLS-1$
					break;
				
				case kNativeStringObject: 
					clsName = "String"; //$NON-NLS-1$
					break;
				
				case kNativeArrayObject: 
					clsName = "Array"; //$NON-NLS-1$
					break;
				
				case kDateObjectType: 
					clsName = "Date"; //$NON-NLS-1$
					break;
				
				case kSoundObjectType: 
					clsName = "Sound"; //$NON-NLS-1$
					break;
				
				case kNativeXMLDoc: 
					clsName = "XML"; //$NON-NLS-1$
					break;
				
				case kNativeXMLNode: 
					clsName = "XMLNode"; //$NON-NLS-1$
					break;
				
				case kNativeCameraObject: 
					clsName = "Camera"; //$NON-NLS-1$
					break;
				
				case kNativeMicrophoneObject: 
					clsName = "Microphone"; //$NON-NLS-1$
					break;
				
				case kNativeCommunicationObject: 
					clsName = "Communication"; //$NON-NLS-1$
					break;
				
				case kNetConnectionObjectType: 
					clsName = "Connection"; //$NON-NLS-1$
					break;
				
				case kNetStreamObjectType: 
					clsName = "Stream"; //$NON-NLS-1$
					break;
				
				case kVideoObjectType: 
					clsName = "Video"; //$NON-NLS-1$
					break;
				
				case kTextFormatObjectType: 
					clsName = "TextFormat"; //$NON-NLS-1$
					break;
				
				case kSharedObjectType: 
					clsName = "SharedObject"; //$NON-NLS-1$
					break;
				
				case kSharedObjectDataType: 
					clsName = "SharedObjectData"; //$NON-NLS-1$
					break;
				
				case kPrintJobObjectType: 
					clsName = "PrintJob"; //$NON-NLS-1$
					break;
				
				case kMovieClipLoaderObjectType: 
					clsName = "MovieClipLoader"; //$NON-NLS-1$
					break;
				
				case kStyleSheetObjectType: 
					clsName = "StyleSheet"; //$NON-NLS-1$
					break;
				
				case kFapPacketDummyObject: 
					clsName = "FapPacket"; //$NON-NLS-1$
					break;
				
				case kLoadVarsObject: 
					clsName = "LoadVars"; //$NON-NLS-1$
					break;
				
				case kTextSnapshotType: 
					clsName = "TextSnapshot"; //$NON-NLS-1$
					break;
				
				default: 
					clsName = PlayerSessionManager.LocalizationManager.getLocalizedTextString("unknown") + "<" + clsType + ">"; //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
					break;
				
			}
			return clsName;
		}
	}
}