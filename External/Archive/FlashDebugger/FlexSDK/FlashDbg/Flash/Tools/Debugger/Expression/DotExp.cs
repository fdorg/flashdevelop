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
using System.Text;

namespace Flash.Tools.Debugger.Expression
{
	/// <summary> Implements the dot operator.</summary>
	public class DotExp:NonTerminalExp
	{
		private String Path(ValueExp exp)
		{
			StringBuilder sb = new StringBuilder();

			if (exp is DotExp)
			{
				sb.Append(Path((exp as DotExp).m_left));
				sb.Append('.');
				sb.Append(Path((exp as DotExp).m_right));
			}
			else if (exp is VariableExp)
			{
				sb.Append((exp as VariableExp).m_name);
			}
			else
			{
				sb.Append(exp.ToString());
			}

			return sb.ToString();
		}

		/* perform your evaluation */
		public override Object evaluate(Context context)
		{
			// eval the left side 
			
			Object l = m_left.evaluate(context);
			
			// create a new context object using our left side then ask 
			// the right to evaluate 
			Context current = context.createContext(l);
			
			Object result;

			try
			{
				result = m_right.evaluate(current);
			}
			catch (NoSuchVariableException)
			{
				throw new NoSuchVariableException(Path(this));
			}
			
			return result;
		}
	}
}
