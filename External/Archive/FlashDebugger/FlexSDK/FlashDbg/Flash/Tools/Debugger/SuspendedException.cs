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
namespace Flash.Tools.Debugger
{
	
	/// <summary> SuspendedException is thrown when the Player 
	/// is in a state for which the action cannot be performed.
	/// </summary>
	[Serializable]
	public class SuspendedException:PlayerDebugException
	{
		public override String Message
		{
			get
			{
				return Bootstrap.LocalizationManager.getLocalizedTextString("key5"); //$NON-NLS-1$
			}
			
		}
	}
}
