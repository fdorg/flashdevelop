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
	
	/// <summary>    Logical NOT of right child only.</summary>
	public class LogicNotExp:BooleanExp, SingleArgumentExp
	{
		public override bool operateOn(bool a, bool b)
		{
			return !b;
		}
	}
}
