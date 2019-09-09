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
	
	/// <summary> Relational type of expression, e.g. Eq, GTEq, Neq, etc.  This class was 
	/// created in order to categorize the types of non-terminals.
	/// 
	/// Additionally it understand how to convert the children
	/// results into long and then perform operate() within
	/// the subclass.
	/// </summary>
	public abstract class RelationalExp:NonTerminalExp
	{
		/* Sub-classes use these method to perform their specific operation */
		public abstract bool operateOn(long a, long b);
		public abstract bool operateOn(String a, String b);
		
		/// <summary> We override this in order to catch and convert the values returned 
		/// by our childrens evaluate method.
		/// </summary>
		public override Object evaluate(Context context)
		{
			Object l = m_left.evaluate(context);
			Object r = m_right.evaluate(context);
			
			bool result = false;
			
			/*
			* Now if either are strings force them both to strigs and compute 
			*/
			if ((l is String) || (r is String))
			{
				String lhs = l.ToString();
				String rhs = r.ToString();
				
				result = operateOn(lhs, rhs);
			}
			else
			{
				/* we are some form of number or boolean */
				long lVal = ArithmeticExp.toLong(l);
				long rVal = ArithmeticExp.toLong(r);
				
				result = operateOn(lVal, rVal);
			}
			return result;
		}
	}
}
