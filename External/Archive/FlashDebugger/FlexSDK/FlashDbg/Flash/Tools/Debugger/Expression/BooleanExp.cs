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

namespace Flash.Tools.Debugger.Expression
{
	
	/// <summary> Boolean type of expression, e.g. And, Not, Or.  This class was 
	/// created in order to categorize the types of non-terminals.
	/// 
	/// Additionally it understand how to convert the children
	/// results into booleans and then perform operate() within
	/// the subclass.
	/// </summary>
	public abstract class BooleanExp:NonTerminalExp
	{
		/* Sub-classes use this method to perform their specific operation */
		public abstract bool operateOn(bool a, bool b);
		
		/// <summary> We override this in order to catch and convert the values returned 
		/// by our childrens evaluate method.
		/// </summary>
		public override Object evaluate(Context context)
		{
			Object l = (this is SingleArgumentExp)?null:m_left.evaluate(context);
			Object r = m_right.evaluate(context);
			
			/*
			* Now convert each to a long and perform the operation 
			*/
			bool lVal = (this is SingleArgumentExp)?false:toBoolean(l);
			bool rVal = toBoolean(r);
			
			bool result = operateOn(lVal, rVal);
			
			return result;
		}
		
		/// <summary> The magic conversion function which provides a mapping
		/// from any object into a boolean!
		/// </summary>
		public static bool toBoolean(Object o)
		{
			bool value = false;
			
			try
			{
				if (o is Boolean)
					value = ((Boolean)o);
				else if (o is ValueType)
					value = (Convert.ToInt64(((ValueType)o)) != 0) ? true : false;
				else if (o is Variable)
					value = ((Variable)o).getValue().ValueAsObject != null;
				else
				{
					String v = o.ToString().ToLower();
					value = "true".Equals(v); //$NON-NLS-1$
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
