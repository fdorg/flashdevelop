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
	
	/// <summary> Arithmetic add</summary>
	public class AddExp:ArithmeticExp
	{
		public override long operateOn(long a, long b)
		{
			return a + b;
		}
		
		/* String concatenation */
		public override String operateOn(String a, String b)
		{
			// we may be getting double values coming through with trailing ".0", so trim them 
			if (a.EndsWith(".0"))
			//$NON-NLS-1$
				a = a.Substring(0, (a.Length - 2) - (0));
			if (b.EndsWith(".0"))
			//$NON-NLS-1$
				b = b.Substring(0, (b.Length - 2) - (0));
			
			return a + b;
		}
	}
}
