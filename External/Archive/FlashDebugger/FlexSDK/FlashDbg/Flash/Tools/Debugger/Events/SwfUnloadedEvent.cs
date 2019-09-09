// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
	
	/// <summary> This event is fired when the player has unloaded a swf</summary>
	public class SwfUnloadedEvent:DebugEvent
	{
		/// <summary>unique identifier for the SWF </summary>
		public long id;
		
		/// <summary>index of SWF in Session.getSwfs() array </summary>
		public int index;
		
		/// <summary>full path name for the SWF </summary>
		public String path;
		
		public SwfUnloadedEvent(long sId, String sPath, int sIndex)
		{
			id = sId;
			index = sIndex;
			path = sPath;
		}
	}
}
