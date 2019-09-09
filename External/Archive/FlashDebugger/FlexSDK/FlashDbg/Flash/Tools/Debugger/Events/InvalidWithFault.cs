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
	
	/// <summary> Signals that a ActionWith instruction could not be executed becuase
	/// the target of the operation is not an object. 
	/// </summary>
	public class InvalidWithFault:FaultEvent
	{
		public static String s_name = "invalid_with"; //$NON-NLS-1$
		
		public override String name()
		{
			return s_name;
		}
	}
}
