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
namespace Flash.Tools.Debugger
{
	
	/// <summary> InProgressException is thrown when a request cannot
	/// be fulfilled because some other activity is currently
	/// taking place that will alter the result of the request.
	/// </summary>
	[Serializable]
	public class InProgressException:PlayerDebugException
	{
	}
}
