// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
namespace Flash.Tools.Debugger
{
	
	/// <summary> Specific attributes which further qualify a Variable. The values in the low
	/// 16 bits correspond to the enumeration fields defined in the player's
	/// "splay.h" file, e.g. <code>kDontEnumerate</code> etc. The values from the
	/// high 16 bits correspond to <code>enum InVariableFlags</code> from
	/// playerdebugger.h, e.g. <code>kIsArgument</code> etc.
	/// </summary>
	public class VariableAttribute
	{
		/// <summary> Indicates that this member is invisible to an enumeration
		/// of its parent.
		/// </summary>
		public const int DONT_ENUMERATE = 0x00000001;
		/// <summary> Indicates that a variable is read-only.</summary>
		public readonly static int READ_ONLY = 0x00000004;
		/// <summary> Indicates that a variable is a local.</summary>
		public readonly static int IS_LOCAL = 0x00000020;
		/// <summary> Indicates that a variable is an argument to a function.</summary>
		public readonly static int IS_ARGUMENT = 0x00010000;
		/// <summary> Indicates that a variable is "dynamic" -- that is, whether it
		/// is a dynamic property of a class declared with keyword "dynamic".
		/// Note, this attribute only works with AS3 and above.
		/// </summary>
		public readonly static int IS_DYNAMIC = 0x00020000;
		/// <summary> Indicates that a variable has a getter.</summary>
		public readonly static int HAS_GETTER = 0x00080000;
		/// <summary> Indicates that a variable has a setter.</summary>
		public readonly static int HAS_SETTER = 0x00100000;
		/// <summary> Indicates that a variable is a static member of its parent.</summary>
		public readonly static int IS_STATIC = 0x00200000;
		/// <summary> Indicates that a variable was declared "const". READ_ONLY, on the other
		/// hand, applies both to "const" variables and also to various other types
		/// of objects. IS_CONST implies READ_ONLY; READ_ONLY does not imply
		/// IS_CONST.
		/// </summary>
		public readonly static int IS_CONST = 0x00400000;
		/// <summary> Indicates that a variable is a public member of its parent.
		/// 
		/// Note: the scope attributes are not bitfields.  To determine the scope
		/// of a variable, use variable.getScope() and compare the result to the
		/// various *_SCOPE values using ==.  For example:
		/// 
		/// <pre>
		/// if (myVar.getScope() == VariableAttribute.PUBLIC_SCOPE) ...
		/// </pre>
		/// </summary>
		public readonly static int PUBLIC_SCOPE = 0x00000000;
		/// <summary> Indicates that a variable is a private member of its parent.
		/// 
		/// Note: the scope attributes are not bitfields.  To determine the scope
		/// of a variable, use variable.getScope() and compare the result to the
		/// various *_SCOPE values using ==.  For example:
		/// 
		/// <pre>
		/// if (myVar.getScope() == VariableAttribute.PRIVATE_SCOPE) ...
		/// </pre>
		/// </summary>
		public readonly static int PRIVATE_SCOPE = 0x00800000;
		/// <summary> Indicates that a variable is a protected member of its parent.
		/// 
		/// Note: the scope attributes are not bitfields.  To determine the scope
		/// of a variable, use variable.getScope() and compare the result to the
		/// various *_SCOPE values using ==.  For example:
		/// 
		/// <pre>
		/// if (myVar.getScope() == VariableAttribute.PROTECTED_SCOPE) ...
		/// </pre>
		/// </summary>
		public readonly static int PROTECTED_SCOPE = 0x01000000;
		/// <summary> Indicates that a variable is an internal member of its parent.
		/// Internally scoped variables are visible to all classes that
		/// are in the same package.
		/// 
		/// Note: the scope attributes are not bitfields.  To determine the scope
		/// of a variable, use variable.getScope() and compare the result to the
		/// various *_SCOPE values using ==.  For example:
		/// 
		/// <pre>
		/// if (myVar.getScope() == VariableAttribute.INTERNAL_SCOPE) ...
		/// </pre>
		/// </summary>
		public readonly static int INTERNAL_SCOPE = 0x01800000;
		/// <summary> Indicates that a variable is scoped by a namespace.  For
		/// example, it may have been declared as:
		/// <code>my_namespace var x;</code>
		/// 
		/// Note: the scope attributes are not bitfields.  To determine the scope
		/// of a variable, use variable.getScope() and compare the result to the
		/// various *_SCOPE values using ==.  For example:
		/// 
		/// <pre>
		/// if (myVar.getScope() == VariableAttribute.NAMESPACE_SCOPE) ...
		/// </pre>
		/// </summary>
		public readonly static int NAMESPACE_SCOPE = 0x02000000;
		/// <summary> A mask which can be used to get back only the scope-related
		/// attributes.
		/// 
		/// Note: the scope attributes are not bitfields.  To determine the scope
		/// of a variable, use variable.getScope() and compare the result to the
		/// various *_SCOPE values using ==.  For example:
		/// 
		/// <pre>
		/// if (myVar.getScope() == VariableAttribute.PRIVATE_SCOPE) ...
		/// </pre>
		/// </summary>
		public readonly static int SCOPE_MASK;
		
		// 0x04000000 is reserved for IS_CLASS, which is now part of
		// ValueAttribute rather than VariableAttribute.
		static VariableAttribute()
		{
			SCOPE_MASK = PUBLIC_SCOPE | PRIVATE_SCOPE | PROTECTED_SCOPE | INTERNAL_SCOPE | NAMESPACE_SCOPE;
		}
		
		// 0x00040000 is reserved for IS_EXCEPTION, which is now part of
		// ValueAttribute rather than VariableAttribute.
		
	}
}
