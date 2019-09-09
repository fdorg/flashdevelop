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
	
	/// <summary> Notification that function information for
	/// all SourceFiles is now available for access.
	/// 
	/// Prior to this notification the following 
	/// calls to SourceFile will return null or 0 values:
	/// 
	/// public String getFunctionNameForLine(int lineNum);
	/// public int getLineForFunctionName(String name);
	/// public String[] getFunctionNames();
	/// 
	/// This is due to the fact the function data is processed
	/// by a background thread and may take many hundreds of 
	/// milliseconds to complete.
	/// </summary>
	/// <deprecated> As of Version 2.  No replacement
	/// </deprecated>
	public class FunctionMetaDataAvailableEvent:DebugEvent
	{
	}
}
