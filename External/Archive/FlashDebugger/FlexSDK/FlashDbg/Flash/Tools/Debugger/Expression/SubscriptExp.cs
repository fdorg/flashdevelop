// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2005-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;

namespace Flash.Tools.Debugger.Expression
{
	
	/// <summary> handles array indexing, e.g. a[k]
	/// 
	/// </summary>
	/// <author>  mmorearty
	/// </author>
	public class SubscriptExp:NonTerminalExp
	{
		
		public override System.Object evaluate(Context context)
		{
			// eval the left side and right side
			Object l = m_left.evaluate(context);
			VariableExp r = VariableExp.create(m_right.evaluate(context).ToString());
			
			// create a new context object using our left side, then ask 
			// the right to evaluate 
			Context current = context.createContext(l);
			
			System.Object result = r.evaluate(current);
			
			return result;
		}
	}
}
