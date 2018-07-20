// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2004-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;

namespace Flash.Tools.Debugger
{
	
	/// <summary> The Frame object represents a single frame of the actionscript call stack.
	/// Each Frame contains a Location object, which identifies the line of source 
	/// for the frame, and a set of variables that are available within the frame.
	/// The set of variables includes a 'this' pointer, arguments passed into 
	/// the function and locals available within the scope of the function.
	/// A given frame is only valid when execution has suspended.  
	/// </summary>
	/// <since> Version 2
	/// </since>
	public interface Frame
	{
		/// <summary> Location object related to this frame.</summary>
		Location Location
		{
			get;
			
		}
		/// <summary> Returns a string which contains the raw signature of
		/// the call.  This information can be used for display
		/// purposes in the event the Location object contains
		/// a null SourceFile, which happens when a call is
		/// made into or through a non-debug executable.
		/// The format of the string is one of the following:
		/// <ul>
		/// <li> <code>declaringClass/[[namespace::]function]</code> (for regular functions) </li>
		/// <li> <code>declaringClass$cinit</code> (class constructor for statics) </li>
		/// <li> <code>declaringClass$iinit</code> (class instance ctor)</li>
		/// <li> <code>global$init</code> </li>
		/// </ul>
		/// <p>
		/// where <code>declaringClass</code> is the name of the
		/// class in which the function is declared (even if it
		/// is an anonymous inner function); <code>namespace</code>
		/// is the namespace of the function (the meaning of this
		/// varies depending on whether the function is private,
		/// protected etc.; see <code>Variable.getNamespace()</code>
		/// for more information); and <code>function</code> is
		/// the name of the function, or <code>""</code> if the
		/// function is anonymous.
		/// </p><p> 
		/// If the signature is unknown then the value
		/// "" will be returned.  Note: this may occur even when
		/// Location contains a non-null SourceFile.
		/// </p><p>
		/// Examples:
		/// <ul>
		/// <li> <code>MyClass/myFunction</code> for a public function </li>
		/// <li> <code>MyClass/MyClass::myFunction</code> for a private function </li>
		/// <li> <code>MyClass/</code> for an anonymous inner function declared
		/// somewhere inside <code>MyClass</code> </li>
		/// <li> <code>""</code> if unknown </li>
		/// </ul>
		/// </p>
		/// </summary>
		String CallSignature
		{
			get;
			
		}
		
		/// <summary> 'this' variable for the frame.  Will return null
		/// if no 'this' pointer available for the frame.
		/// </summary>
		/// <throws>  NoResponseException </throws>
		/// <throws>  NotSuspendedException </throws>
		/// <throws>  NotConnectedException </throws>
		Variable getThis(Session s);
		
		/// <summary> Arguments that were passed into the function.  An empty
		/// array is used to denote that no arguments were passed into 
		/// this function scope.
		/// </summary>
		/// <throws>  NoResponseException </throws>
		/// <throws>  NotSuspendedException  </throws>
		/// <throws>  NotConnectedException  </throws>
		Variable[] getArguments(Session s);
		
		/// <summary> Locals used within this function scope.  An empty
		/// array is used to denote no locals are available 
		/// within this function scope.
		/// </summary>
		/// <throws>  NoResponseException </throws>
		/// <throws>  NotSuspendedException  </throws>
		/// <throws>  NotConnectedException  </throws>
		Variable[] getLocals(Session s);
		
		/// <summary> Returns a list of objects which make up the scope chain of
		/// this frame.
		/// <p />
		/// Some of the entries will be classes; some will be instances
		/// of classes; some will be functions; etc.
		/// <p />
		/// <b>Bug:</b> Currently, this does <em>not</em> include any
		/// scope chain entries which were created via "with var".
		/// </summary>
		Variable[] getScopeChain(Session s);
	}
}
