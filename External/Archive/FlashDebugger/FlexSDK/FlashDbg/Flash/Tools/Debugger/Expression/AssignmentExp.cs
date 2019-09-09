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
	
	/// <summary> Provides the ability to do assignment of a rhs to a lhs
	/// The expectation is that the rhs can resolve to a variable.
	/// </summary>
	public class AssignmentExp:NonTerminalExp
	{
		/// <summary> We need to evaluate both left and right children and then request an assignment</summary>
		/// <throws>  PlayerFaultException  </throws>
		public override System.Object evaluate(Context context)
		{
			// should eval to a variable, but in order to create some that 
			// may not exist we enable dummies to be made during this process
			context.createPseudoVariables(true);
			
			System.Object l = m_left.evaluate(context);
			
			context.createPseudoVariables(false);
			
			// should eval to a scalar (string)
			System.Object r = m_right.evaluate(context);
			
			// now request a assignment on this variable 
			System.Object result = context.assign(l, r);
			
			return result;
		}
	}
}
