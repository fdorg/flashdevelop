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
	
	/// <summary> A class which contains a variable that needs to be
	/// resolved in an appropriate context in order for 
	/// its value to be determined.
	/// </summary>
	public class VariableExp:TerminalExp
	{
		internal String m_name;
		
		internal VariableExp(String name)
		{
			m_name = name;
		} /* use static to create nodes */
		
		public override Object evaluate(Context context)
		{
			/* do a lookup in the current context for this variable */
			Object result = context.lookup(m_name);
			return result;
		}
		
		/* we put this here in case we later want to refine object construction */
		public static VariableExp create(String name)
		{
			return new VariableExp(name);
		}
	}
}
