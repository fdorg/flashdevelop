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
namespace Flash.Tools.Debugger.Events
{
	
	/// <summary> Signals that a bad target name was provided while executing 
	/// a ActionSetTarget instruction.
	/// </summary>
	public class InvalidTargetFault:FaultEvent
	{
		public static String s_name = "invalid_target"; //$NON-NLS-1$
		
		public InvalidTargetFault(String target):base(target)
		{
		}
		
		public override String name()
		{
            return s_name;
		}
	}
}
