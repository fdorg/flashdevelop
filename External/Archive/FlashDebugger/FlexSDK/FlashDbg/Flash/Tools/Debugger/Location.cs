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

namespace Flash.Tools.Debugger
{
	
	/// <summary> The Location object identifies a specific line number with a SourceFile.
	/// It is used for breakpoint manipulation and obtaining stack frame context.
	/// </summary>
	public interface Location
	{
		/// <summary> Source file for this location </summary>
		SourceFile File
		{
			get;
			
		}
		/// <summary> Line number within the source for this location </summary>
		int Line
		{
			get;
			
		}
	}
}
