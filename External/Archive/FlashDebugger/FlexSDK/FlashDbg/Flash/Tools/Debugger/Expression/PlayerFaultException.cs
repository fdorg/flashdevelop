////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2005-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using FaultEvent = Flash.Tools.Debugger.Events.FaultEvent;
namespace Flash.Tools.Debugger.Expression
{
	
	/// <summary> Thrown when the player generates a fault.  For example, if
	/// an attempt to assign a value to a variable results in the player
	/// generating a fault because that value has no setter, or because
	/// the setter throws an exception for any other reason, then this
	/// exception will be generated.
	/// </summary>
	[Serializable]
	public class PlayerFaultException : ExpressionException
	{
		public override String getLocalizedMessage()
		{
			System.Collections.IDictionary args = new System.Collections.Hashtable();
			args["arg1"] = base.Message; //$NON-NLS-1$
			return ASTBuilder.LocalizationManager.getLocalizedTextString("key7", args); //$NON-NLS-1$
		}

		virtual public FaultEvent FaultEvent
		{
			get
			{
				return m_event;
			}
			
		}
		public override String Message
		{
			get
			{
				return m_event.information;
			}
			
		}

		private FaultEvent m_event;
		
		public PlayerFaultException(FaultEvent eventType)
		{
            m_event = eventType;
		}
	}
}
