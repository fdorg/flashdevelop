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

namespace Flash.Tools.Debugger.Expression
{
	/// <summary> All objects in the abstract syntax tree must provide 
	/// this interface.  It allows the tree to resolve down
	/// to a single value.
	/// 
	/// The tree nodes are terminal and non-terminal.  Terminals
	/// are constants or variables, non-terminals are everything 
	/// else.  Each non-terminal is an operation which takes 
	/// its left hand child and right hand child as input
	/// and produces a result.  Performing evaluate() at the root of 
	/// the tree results in a single Object being returned.
	/// </summary>
	public interface ValueExp
	{
		/// <summary> sets the left hand side child to the given node</summary>
		ValueExp LeftChild
		{
			set;
			
		}
		/// <summary> sets the right hand side child to the given node</summary>
		ValueExp RightChild
		{
			set;
			
		}
		/// <summary> perform your evaluation</summary>
		Object evaluate(Context context);
		
		/// <summary> sees if any nodes within this expression are equal to the type of o</summary>
		bool containsInstanceOf(Type c);
		
		/// <summary> Returns whether the expression would have any side effects other than
		/// executing getters -- e.g. assignment, ++, or function calls.
		/// </summary>
		bool hasSideEffectsOtherThanGetters();
	}
}