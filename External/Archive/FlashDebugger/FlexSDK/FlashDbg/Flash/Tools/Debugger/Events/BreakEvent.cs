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

namespace Flash.Tools.Debugger.Events
{
	
	/// <summary> Break event is received when the player halts execution</summary>
	public class BreakEvent:DebugEvent
	{
		/// <summary>unique identifier for the source file where the Player has suspened. </summary>
		public int fileId;
		
		/// <summary>line number in the source file where the Player has suspended. </summary>
		public int line;
		
		public BreakEvent(int fId, int l)
		{
			fileId = fId;
			line = l;
		}
	}
}
