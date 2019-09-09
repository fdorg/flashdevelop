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

namespace Flash.Tools.Debugger
{
	
	/// <summary> An identifier for the type of a Variable.</summary>
	public class VariableType {
		/* Types of a variable (one of) */
		public const int NUMBER = 0;
		public const int BOOLEAN = 1;
		public const int STRING = 2;
		public const int OBJECT = 3;
		public const int FUNCTION = 4;
		public const int MOVIECLIP = 5;
		public const int NULL = 6;
		public const int UNDEFINED = 7;
		public const int UNKNOWN = 8;
	}
}
