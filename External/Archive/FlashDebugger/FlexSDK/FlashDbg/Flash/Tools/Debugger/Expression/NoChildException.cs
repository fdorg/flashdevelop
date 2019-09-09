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
	public class NoChildException : ExpressionException
	{
		public override String getLocalizedMessage()
		{
			System.Collections.IDictionary args = new System.Collections.Hashtable();
			args["arg1"] = base.Message; //$NON-NLS-1$
			return ASTBuilder.LocalizationManager.getLocalizedTextString("key6", args); //$NON-NLS-1$
		}

		public NoChildException()
			: base()
		{
		}
		public NoChildException(String s)
			: base(s)
		{
		}
	}
}
