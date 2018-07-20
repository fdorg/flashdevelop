// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2004-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
namespace Flash.Tools.Debugger.Events
{
	
	/// <summary> Signals that an ActionScript error has caused a fault</summary>
	public class ConsoleErrorFault:FaultEvent
	{
		public static String s_name = "console_error"; //$NON-NLS-1$
		
		public ConsoleErrorFault(String s):base(s)
		{
		}
		
		public override String name()
		{
			return s_name;
		}
	}
}
