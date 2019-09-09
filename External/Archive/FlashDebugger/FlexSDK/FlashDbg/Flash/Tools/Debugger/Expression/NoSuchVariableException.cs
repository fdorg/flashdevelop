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
	/// <summary> Thrown when a variable name cannot be resolved in the current scope</summary>
	[Serializable]
	public class NoSuchVariableException : ExpressionException
	{
		public override String getLocalizedMessage()
		{
			System.Collections.IDictionary args = new System.Collections.Hashtable();
			args["arg1"] = base.Message; //$NON-NLS-1$
			return ASTBuilder.LocalizationManager.getLocalizedTextString("key4", args); //$NON-NLS-1$
		}

        public override string Message
        {
	        get 
	        { 
		         return base.Message;
	        }
        }

        public NoSuchVariableException(String s)
			: base(s)
		{
		}

		public NoSuchVariableException(Object o)
			: base(o.ToString())
		{
		}
	}
}
