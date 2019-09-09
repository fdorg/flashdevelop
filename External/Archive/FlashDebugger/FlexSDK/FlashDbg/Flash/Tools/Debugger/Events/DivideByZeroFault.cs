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
namespace Flash.Tools.Debugger.Events
{
	
	/// <summary> Signals that a divide by zero fault has occurred</summary>
	public class DivideByZeroFault:FaultEvent
	{
		public static String s_name = "zero_divide"; //$NON-NLS-1$
		
		public override String name()
		{
			return s_name;
		}
	}
}
