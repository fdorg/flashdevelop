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

namespace Flash.Tools.Debugger.Events
{
	
	/// <summary> Notification that the file list has been 
	/// modified since the last query was performed,
	/// that is, since the last call of Session.getFileList
	/// </summary>
	/// <deprecated> As of Version 2.  
	/// </deprecated>
	/// <seealso cref="SwfLoadedEvent">
	/// </seealso>
	public class FileListModifiedEvent:DebugEvent
	{
	}
}
