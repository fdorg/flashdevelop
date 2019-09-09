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
using System;
namespace Flash.Tools.Debugger.Expression
{
	
	/// <summary> Bitwise type of expression.  This class was 
	/// created in order to categorize the types of non-terminals.
	/// 
	/// Additionally it understand how to convert the children
	/// results into long and then perform operate() within
	/// the subclass.
	/// 
	/// It is used for bitwise operators.
	/// </summary>
	public abstract class BitwiseExp:ArithmeticExp
	{
		/* Arithmetic is good; nothing to change ! */
	}
}
