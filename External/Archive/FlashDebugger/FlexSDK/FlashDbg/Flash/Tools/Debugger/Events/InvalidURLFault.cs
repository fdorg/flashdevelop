// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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
	
	/// <summary> Signals that a request to open a URL failed. </summary>
	public class InvalidURLFault:FaultEvent
	{
		public static String s_name = "invalid_url"; //$NON-NLS-1$
		
		public InvalidURLFault(String url):base(url)
		{
		}
		
		public override String name()
		{
			return s_name;
		}
	}
}
