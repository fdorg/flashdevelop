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
using FaultEvent = Flash.Tools.Debugger.Events.FaultEvent;

namespace Flash.Tools.Debugger
{
	
	/// <summary> A Variable is any ActionScript entity, such as a MovieClip,
	/// String, Number, etc.  It encapsulates the concept of a type
	/// and a value.
	/// </summary>
	public interface Variable
	{
		/// <summary> The fully qualified name of the variable, i.e. "namespace::name"
		/// if there is a namespace, or just "name" if not.
		/// </summary>
		String QualifiedName
		{
			get;
			
		}
		/// <summary> The namespace of the variable.  This is everything before the
		/// "::".  For example:
		/// 
		/// <ul>
		/// <li> If a variable was declared "private var x", then the
		/// namespace is "ClassName$3", where "3" might be
		/// any number. </li>
		/// <li> If a variable was declared within a namespace, e.g.
		/// "mynamespace var x", then the namespace might be
		/// "http://blahblah::x", where "http://blahblah" is the URL
		/// of the namespace.</li>
		/// <li> If a variable was declared neither public nor private
		/// (and is therefore "internal"), and it is inside of a
		/// package, then the namespace might be
		/// "packagename". </li>
		/// </ul>
		/// 
		/// </summary>
		/// <returns> namespace or "", never <code>null</code>
		/// </returns>
		String Namespace
		{
			get;
			
		}
		/// <summary> Returns just the scope bits of the attributes. The scope values from
		/// VariableAttribute (PUBLIC_SCOPE etc.) are NOT bitfields, so the returned
		/// value can be compared directly to VariableAttribute.PUBLIC_SCOPE, etc.
		/// using "==".
		/// 
		/// </summary>
		/// <seealso cref="VariableAttribute">
		/// </seealso>
		int Scope
		{
			get;
			
		}
		/// <summary> For a member variable of an instance of some class, its "level" indicates
		/// how far up the class hierarchy it is from the actual class of the instance.
		/// For example, suppose you have this code:
		/// 
		/// <pre>
		/// class A           { int a }
		/// class B extends A { int b }
		/// class C extends B { int c }
		/// var myObject: C
		/// </pre>
		/// 
		/// In this case, for <code>myObject</code>, the "level" of variable <code>c</code>
		/// is 0; the level of <code>b</code> is 1; and the level of <code>a</code> is 2.
		/// </summary>
		int Level
		{
			get;
			
		}
		/// <summary> The name of the variable.</summary>
		String getName();
		
		/// <summary> The class in which this member was actually defined.  For example, if class
		/// B extends class A, and class A has member variable V, then for variable
		/// V, the defining class is always "A", even though the parent variable might
		/// be an instance of class B.
		/// </summary>
		String getDefiningClass();
		
		/// <summary> Variable attributes define further information 
		/// regarding the variable.  They are bitfields identified
		/// as VariableAttribute.xxx
		/// 
		/// </summary>
		/// <seealso cref="VariableAttribute">
		/// </seealso>
		int getAttributes();
		
		/// <seealso cref="VariableAttribute">
		/// </seealso>
		bool isAttributeSet(int variableAttribute);
		
		/// <summary> Returns the value of the variable.</summary>
		Value getValue();
		
		/// <summary> Returns whether the value of the variable has changed since the last
		/// time the program was suspended.  If the previous value of the
		/// variable is unknown, this function will return <code>false</code>.
		/// </summary>
		bool hasValueChanged();
		
		/// <summary> Changes the value of a variable. New members cannot be added to a Variable,
		/// only the value of existing scalar members can be modified.
		/// 
		/// </summary>
		/// <param name="type">the type of the member which is being set. Use
		/// VariableType.UNDEFINED in order to set the variable to an
		/// undefined state; the contents of 'value' will be ignored.
		/// </param>
		/// <param name="value">the string value of the member. May be 'true' or 'false' for
		/// Boolean types or any valid number for Number types.
		/// </param>
		/// <returns> null, if set was successful; or a FaultEvent if a setter was
		/// invoked and the setter threw an exception. In that case, look at
		/// FaultEvent.information to see the error text of the exception
		/// that occurred.
		/// </returns>
		/// <throws>  NoResponseException </throws>
		/// <summary>             if times out
		/// </summary>
		/// <throws>  NotSuspendedException </throws>
		/// <summary>             if Player is running
		/// </summary>
		/// <throws>  NotConnectedException </throws>
		/// <summary>             if Player is disconnected from Session
		/// </summary>
		FaultEvent setValue(int type, String value);
		
		/// <returns> True if this variable has a getter, and the getter has not yet been invoked.
		/// </returns>
		bool needsToInvokeGetter();
		
		/// <summary> Executes the getter for this variable, and changes its value accordingly.  Note that
		/// the <code>HAS_GETTER</code> flag is not affected by this call -- even after this
		/// call, <code>HAS_GETTER</code> will still be true.  If you want to test whether the
		/// getter has already been executed, call <code>needsToInvokeGetter()</code>.
		/// <p />
		/// Has no effect if <code>needsToInvokeGetter()</code> is false.
		/// 
		/// </summary>
		/// <throws>  NotSuspendedException </throws>
		/// <throws>  NoResponseException </throws>
		/// <throws>  NotConnectedException </throws>
		void  invokeGetter();
	}
}
