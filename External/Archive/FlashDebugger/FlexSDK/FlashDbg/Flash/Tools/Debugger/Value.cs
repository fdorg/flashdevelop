// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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

namespace Flash.Tools.Debugger
{
	
	/// <author>  mmorearty
	/// </author>
	public abstract class Value
	{
        /// <summary> The value returned if somone calls Id for a Variable
        /// which stores a variable of simple type such as String or
        /// integer, rather than an Object or MovieClip.
        /// </summary>
        /// <seealso cref="Id">
        /// </seealso>
        public readonly static int UNKNOWN_ID = -1;
        /// <summary> The special ID for pseudo-variable "_global".  (Note, this only
        /// exists in AS2, not AS3.)
        /// </summary>
        /// <seealso cref="Id">
        /// </seealso>
        public readonly static int GLOBAL_ID = -2;
        /// <summary> The special ID for pseudo-variable "this".</summary>
        /// <seealso cref="Id">
        /// </seealso>
        public readonly static int THIS_ID = -3;
        /// <summary> The special ID for pseudo-variable "_root".  (Note, this only
        /// exists in AS2, not AS3.)
        /// </summary>
        /// <seealso cref="Id">
        /// </seealso>
        public readonly static int ROOT_ID = -4;
        /// <summary> The special ID for the top frame of the stack.  Locals and
        /// arguments are "members" of this pseudo-variable.
        /// 
        /// All the stack frames have IDs counting down from here.  For example,
        /// the top stack frame has ID <code>BASE_ID</code>; the next
        /// stack frame has ID <code>BASE_ID - 1</code>; and so on.
        /// 
        /// </summary>
        /// <seealso cref="Id">
        /// </seealso>
        public readonly static int BASE_ID = -100;
        /// <summary> _level0 == LEVEL_ID, _level1 == LEVEL_ID-1, ...
        /// 
        /// all IDs below this line are dynamic.
        /// </summary>
        public readonly static int LEVEL_ID = -300;
        /// <summary> The return value of getTypeName() if this value represents the traits of a class.</summary>
        public readonly static String TRAITS_TYPE_NAME = "traits"; //$NON-NLS-1$

    
        /// <summary> Returns a unique ID for the object referred to by this variable.
		/// If two variables point to the same underlying object, their
		/// getId() functions will return the same value.
		/// 
		/// This is only meaningful for variables that store an Object or
		/// MovieClip.  For other types of variables (e.g. integers and
		/// strings), this returns <code>UNKNOWN_ID</code>.
		/// </summary>
        public abstract int Id
		{
			get;
			
		}
		/// <summary> Returns the value of the variable, as an Object.  For example,
		/// if the variable is an integer, the returned object will be an
		/// <code>Integer</code>.
		/// </summary>
        public abstract Object ValueAsObject
		{
			get;
			
		}
		/// <summary> Returns the value of the variable, converted to a string.</summary>
        public abstract String ValueAsString
		{
			get;
		}
		
		/// <summary> Variable type can be one of VariableType.OBJECT,
		/// VariableType.FUNCTION, VariableType.NUMBER, VariableType.STRING,
		/// VariableType.UNDEFINED, VariableType.NULL.
		/// </summary>
        public abstract int getType();

        public abstract String getTypeName();

        public abstract String getClassName();
		
		/// <summary> Variable attributes define further information 
		/// regarding the variable.  They are bitfields identified
		/// as VariableAttribute.xxx
		/// 
		/// </summary>
		/// <seealso cref="VariableAttribute">
		/// </seealso>
        public abstract int getAttributes();
		
		/// <seealso cref="VariableAttribute">
		/// </seealso>
        public abstract bool isAttributeSet(int variableAttribute);
		
		/// <summary> Returns all child members of this variable.  Can only be called for
		/// variables of type Object or MovieClip.
		/// </summary>
		/// <throws>  NotConnectedException  </throws>
		/// <throws>  NoResponseException  </throws>
		/// <throws>  NotSuspendedException  </throws>
        public abstract Variable[] getMembers(Session s);
		
		/// <summary> Returns a specific child member of this variable.  Can only be called for
		/// variables of type <code>Object</code> or <code>MovieClip</code>.
		/// </summary>
		/// <param name="s">the session
		/// </param>
		/// <param name="name">just a varname name, without its namespace (see <code>getName()</code>)
		/// </param>
		/// <returns> the specified child member, or null if there is no such child.
		/// </returns>
		/// <throws>  NotConnectedException  </throws>
		/// <throws>  NoResponseException  </throws>
		/// <throws>  NotSuspendedException  </throws>
        public abstract Variable getMemberNamed(Session s, String name);
		
		/// <summary> Returns the number of child members of this variable.  If called for
		/// a variable which has a simple type such as integer or string,
		/// returns zero.
		/// </summary>
		/// <throws>  NotConnectedException  </throws>
		/// <throws>  NoResponseException  </throws>
		/// <throws>  NotSuspendedException  </throws>
        public abstract int getMemberCount(Session s);
		
		/// <summary> Returns the list of classes that contributed members to this object, from
		/// the class itself all the way down to <code>Object</code> (or, if
		/// allLevels == false, down to the lowest-level class that actually
		/// contributed members).
		/// 
		/// </summary>
		/// <param name="allLevels">if <code>true</code>, the caller wants the entire class
		/// hierarchy. If <code>false</code>, the caller wants only
		/// that portion of the class hierarchy that actually contributed
		/// member variables to the object. For example,
		/// <code>Object</code> has no members, so if the caller passes
		/// <code>true</code> then the returned array of strings will
		/// always end with <code>Object</code>, but if the caller
		/// passes <code>false</code> then the returned array of strings
		/// will <em>never</em> end with <code>Object</code>.
		/// </param>
		/// <returns> an array of fully qualified class names.
		/// </returns>
        public abstract String[] getClassHierarchy(bool allLevels);
	}
}
