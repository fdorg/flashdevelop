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
	
	/// <summary> A class which contains a constant.  Currently only
	/// integers are supported. 
	/// </summary>
	public class ConstantExp:TerminalExp
	{
		internal System.Object m_value;
		
		internal ConstantExp(long value)
		{
			m_value = (long) value;
		} /* use static to create nodes */
		
		public override System.Object evaluate(Context context)
		{
			return m_value;
		}
		
		/* we put this here in case we later want to refine object construction */
		public static ConstantExp create(long value)
		{
			return new ConstantExp(value);
		}
	}
}
