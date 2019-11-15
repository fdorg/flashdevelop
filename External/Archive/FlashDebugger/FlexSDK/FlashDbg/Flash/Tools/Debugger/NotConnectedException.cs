// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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
namespace Flash.Tools.Debugger
{
	
	/// <summary> NotConnectedException is thrown when the Session
	/// is no longer connnected to the Player
	/// </summary>
	[Serializable]
	public class NotConnectedException:PlayerDebugException
	{
		public override String Message
		{
			get
			{
				return Bootstrap.LocalizationManager.getLocalizedTextString("key3"); //$NON-NLS-1$
			}
			
		}
	}
}
