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
namespace Flash.Tools.Debugger.Expression
{
	
	/// <summary> Relational != </summary>
	public class NeqExp:RelationalExp
	{
		public override bool operateOn(long a, long b)
		{
			return (a != b);
		}
		
		public override bool operateOn(String a, String b)
		{
			return (String.CompareOrdinal(a, b) == 0)?false:true;
		}
	}
}
