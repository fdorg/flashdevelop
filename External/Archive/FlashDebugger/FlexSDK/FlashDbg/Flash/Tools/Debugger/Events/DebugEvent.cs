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
	
	/// <summary> The top of the event hierarchy for debug events.  All debug events are of this type</summary>
	public abstract class DebugEvent
	{
		public String information;
		
		public DebugEvent()
		{
			information = "";
		}
		public DebugEvent(String info)
		{
			information = info;
		}
	}
}
