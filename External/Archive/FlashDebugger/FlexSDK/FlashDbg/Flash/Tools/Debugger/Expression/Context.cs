// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2003-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;

namespace Flash.Tools.Debugger.Expression
{
	
	/// <summary> An object which returns a value given a name and
	/// appropriate context information.  
	/// </summary>
	public interface Context
	{
		/// <summary> Look for an object of the given name.
		/// 
		/// The returned Object can be of any type at all.  For example, it could be:
		/// 
		/// <ul>
		/// <li> a <code>Flash.Tools.Debugger.Variable</code> </li>
		/// <li> your own wrapper around <code>Variable</code> </li>
		/// <li> a <code>Flash.Tools.Debugger.Value</code> </li>
		/// <li> any built-in Java primitive such as <code>Long</code>, <code>Integer</code>,
		/// <code>Double</code>, <code>Boolean</code>, or <code>String</code> </li>
		/// <li> any other type you want which has a good <code>toString()</code>; see below </li>
		/// </ul>
		/// 
		/// No matter what type you return, make sure that it is a type whose <code>toString()</code>
		/// function returns a string representing the underlying value, in a form which
		/// the expression evaluator can use to either (1) return to the caller, or
		/// (2) attempt to convert to a number if the user typed an expression
		/// such as <code>"3" + "4"</code>.
		/// </summary>
		Object lookup(Object o);
		
		/// <summary> Look for the members of an object.</summary>
		/// <param name="o">A variable whose members we want to look up
		/// </param>
		/// <returns> Some object which represents the members; could even be just a string.
		/// See lookup() for more information about the returned type.
		/// </returns>
		/// <seealso cref="lookup(Object)">
		/// </seealso>
		Object lookupMembers(Object o);
		
		/// <summary> Create a new context object by combining the current one and o.
		/// For example, if the user typed "myVariable.myMember", then this function
		/// will get called with o equal to the object which represents "myVariable".
		/// This function should return a new context which, when called with
		/// lookup("myMember"), will return an object for that member.
		/// 
		/// </summary>
		/// <param name="o">any object which may have been returned by this class's lookup() function
		/// </param>
		Context createContext(Object o);
		
		/// <summary> Assign the object o, the value v.</summary>
		/// <returns> Boolean true if worked, false if failed.
		/// </returns>
		Object assign(Object o, Object v);
		
		/// <summary> Enables/disables the creation of variables during lookup calls.
		/// This is ONLY used by AssignmentExp for creating a assigning a value 
		/// to a property which currently does not exist.
		/// </summary>
		void  createPseudoVariables(bool oui);
	}
}
