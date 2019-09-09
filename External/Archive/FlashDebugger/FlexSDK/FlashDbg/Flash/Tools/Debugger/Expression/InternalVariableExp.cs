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
	
	/// <summary> A class which contains an internal variable that needs 
	/// to be resolved.
	/// </summary>
	public class InternalVariableExp:VariableExp
	{
		internal InternalVariableExp(String name):base(name)
		{
		} /* use static to create nodes */
		
		public override System.Object evaluate(Context context)
		{
			/* perform a lookup on this dude */
			return context.lookup(m_name);
		}
		
		/* we put this here in case we later want to refine object construction */
		public static new VariableExp create(String name)
		{
			return new InternalVariableExp(name);
		}
	}
}
