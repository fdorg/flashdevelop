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
namespace Flash.Tools.Debugger
{
	
	/// <summary> A descriptor for the type of watchpoint.
	/// It may be one of three values; read, write or
	/// both read and write.
	/// </summary>
	/// <since> Version 2
	/// </since>
	public class WatchKind
	{
        /* kind of a watchpoint (one of) */
        public const int NONE = 0;
        public const int READ = 1;
        public const int WRITE = 2;
        public const int READWRITE = 3;
    }
}
