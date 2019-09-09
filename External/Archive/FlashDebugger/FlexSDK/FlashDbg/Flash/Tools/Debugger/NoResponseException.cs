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

namespace Flash.Tools.Debugger
{
	
	/// <summary> NoResponseException is thrown when the Player does
	/// not respond to the command that was issued.
	/// 
	/// The field m_waitedFor contains the number of
	/// milliseconds waited for the response.
	/// </summary>
	[Serializable]
	public class NoResponseException:PlayerDebugException
	{
		public String getLocalizedMessage()
		{
			System.Collections.IDictionary args = new System.Collections.Hashtable();
			String formatString;
			if (m_waitedFor != - 1 && m_waitedFor != 0)
			{
				formatString = "key2"; //$NON-NLS-1$
				args["time"] = System.Convert.ToString(m_waitedFor); //$NON-NLS-1$
			}
			else
			{
				formatString = "key1"; //$NON-NLS-1$
			}
			return Bootstrap.LocalizationManager.getLocalizedTextString(formatString, args);
		}
		/// <summary> Number of milliseconds that elapsed causing the timeout
		/// -1 means unknown.
		/// </summary>
		public int m_waitedFor;
		
		public NoResponseException(int t)
		{
			m_waitedFor = t;
		}
	}
}
