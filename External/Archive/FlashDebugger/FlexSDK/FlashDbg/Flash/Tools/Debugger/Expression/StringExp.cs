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
	
	/// <summary> A class which contains a fixed string.</summary>
	public class StringExp:TerminalExp
	{
		internal String m_text;
		
		internal StringExp(String text)
		{
			m_text = text;
		} /* use static to create nodes */
		
		public override System.Object evaluate(Context context)
		{
			return m_text;
		}
		
		/* we put this here in case we later want to refine object construction */
		public static StringExp create(String text)
		{
			return new StringExp(text);
		}
	}
}
