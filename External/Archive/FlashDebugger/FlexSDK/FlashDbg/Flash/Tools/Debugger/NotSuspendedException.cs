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
	
	/// <summary> NotSuspendedException is thrown when the Player 
	/// is in a state for which the action cannot be performed.
	/// </summary>
	[Serializable]
	public class NotSuspendedException:PlayerDebugException
	{
		public override String Message
		{
			get
			{
				return Bootstrap.LocalizationManager.getLocalizedTextString("key4"); //$NON-NLS-1$
			}
			
		}
	}
}
