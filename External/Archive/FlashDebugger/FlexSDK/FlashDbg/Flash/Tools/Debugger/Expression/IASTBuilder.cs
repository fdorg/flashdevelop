// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;

namespace Flash.Tools.Debugger.Expression
{
	public interface IASTBuilder
	{
		/// <summary> A rather stupid parser that should do a fairly good job at
		/// parsing a general expression string 
		/// 
		/// Special mode for processing '.' since a.0 is legal in ActionScript
		/// 
		/// Exceptions:
		/// EmptyStackException - no expression was built!
		/// UnknownOperationException - there was an unknown operation placed into the expression
		/// IncompleteExpressionException - most likely missing parenthesis.
		/// ParseException - a general parsing error occurred.
		/// 
		/// </summary>
		ValueExp parse(TextReader inStream);

        ValueExp parse(TextReader inStream, bool ignoreUnknownCharacters);
	}
}
