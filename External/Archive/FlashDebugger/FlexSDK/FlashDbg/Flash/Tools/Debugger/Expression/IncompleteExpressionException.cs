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
	
	/// <summary> Thrown when an attempt is made to set the child of a node 
	/// that does not exist
	/// </summary>
	[Serializable]
	public class IncompleteExpressionException : ExpressionException
	{
		public override String getLocalizedMessage()
		{
			return ASTBuilder.LocalizationManager.getLocalizedTextString("key3"); //$NON-NLS-1$
		}
	}
}
