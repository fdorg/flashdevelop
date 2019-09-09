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
	
	/// <summary> De-reference a variable producing a string which contains a list of memebers
	/// and their values.
	/// </summary>
	public class IndirectionExp:NonTerminalExp, SingleArgumentExp
	{
		/// <summary> We need to evaluate our left child then request a list of 
		/// members from it
		/// </summary>
		public override System.Object evaluate(Context context)
		{
			// should eval to a variable id
			System.Object l = m_right.evaluate(context);
			
			// now request a lookup on this variable
			System.Object result = context.lookupMembers(l);
			
			return result;
		}
	}
}
