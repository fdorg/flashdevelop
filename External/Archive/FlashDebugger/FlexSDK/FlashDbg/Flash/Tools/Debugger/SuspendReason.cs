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
namespace Flash.Tools.Debugger
{
	
	/// <summary> Reasons for which the Flash Player will suspend itself</summary>
	public class SuspendReason
	{
        public const int Unknown = 0;
        /// <summary>We hit a breakpoint </summary>
        public const int Breakpoint = 1;
        /// <summary>A watchpoint was triggered </summary>
        public const int Watch = 2;
        /// <summary>A fault occurred </summary>
        public const int Fault = 3;
        public const int StopRequest = 4;
        /// <summary>A step completed </summary>
        public const int Step = 5;
        public const int HaltOpcode = 6;
        /// <summary> Either a new SWF was loaded, or else one or more scripts (ABCs)
        /// from an existing SWF were loaded.
        /// </summary>
        public const int ScriptLoaded = 7;
    }
}
