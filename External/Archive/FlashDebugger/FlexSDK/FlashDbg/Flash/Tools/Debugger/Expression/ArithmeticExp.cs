// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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
	
	/// <summary> Arithmetic type of expression, e.g. Add, Div, etc.  This class was 
	/// created in order to categorize the types of non-terminals.
	/// 
	/// Additionally it understand how to convert the children
	/// results into long and then perform operate() within
	/// the subclass.
	/// </summary>
	public abstract class ArithmeticExp:NonTerminalExp
	{
		/* Sub-classes use this method to perform their specific operation */
		public abstract long operateOn(long a, long b);
		
		/* Sub-classes use this method to perform their specific operation (String manipulation is not supported by default)  */
		public virtual String operateOn(String a, String b)
		{
			throw new UnknownOperationException();
		}
		
		/// <summary> We override this in order to catch and convert the values returned 
		/// by our childrens evaluate method.
		/// 
		/// If we only take a single argument then don't play with the m_left.
		/// </summary>
		public override Object evaluate(Context context)
		{
			Object l = (this is SingleArgumentExp)?null:m_left.evaluate(context);
			Object r = m_right.evaluate(context);
			Object result = null;
			
			/*
			* Now convert each to a long and perform the operation 
			*/
			try
			{
				long lVal = (this is SingleArgumentExp)?0:toLong(l);
				long rVal = toLong(r);
				
				result = (long) operateOn(lVal, rVal);
			}
			catch (FormatException nfe)
			{
				// we could not perform arithmetic operation on these guys, 
				// so convert to a string and request operateOn to do its stuff.
				try
				{
					result = operateOn((this is SingleArgumentExp)?null:l.ToString(), r.ToString());
				}
				catch (UnknownOperationException)
				{
					// this form of operateOn is not supported, so throw the original exception
					throw nfe;
				}
			}
			
			return result;
		}
		
		/// <summary> The magic conversion function which provides a mapping
		/// from any object into a long!
		/// </summary>
		public static long toLong(Object o)
		{
			long value = 0;
			
			try
			{
				if (o is ValueType)
					value = Convert.ToInt64(((ValueType) o));
				else if (o is Boolean)
					value = (((Boolean) o))?1:0;
				else if (o is Variable)
				{
					Variable var = (Variable)o;
					if (var.getValue().getType() == VariableType.BOOLEAN) return (bool)var.getValue().ValueAsObject?1:0;
					if (var.getValue().getType() == VariableType.NULL) return 0;
					return var.getValue().ValueAsObject!=null?var.getValue().Id:0;
				}
				else
				{
					value = (long) Double.Parse(o.ToString());
				}
			}
			catch (FormatException n)
			{
				throw n;
			}
			
			return value;
		}
	}
}
