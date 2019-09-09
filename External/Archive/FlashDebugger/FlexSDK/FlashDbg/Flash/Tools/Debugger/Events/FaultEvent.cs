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
namespace Flash.Tools.Debugger.Events
{
	
	/// <summary> An event type that signals a problem situation within the Player.
	/// Under normal conditions the Player will suspend execution, resulting
	/// in a following BreakEvent to be fired.  However, if this occurs
	/// while a getter or setter is executing, then the player will *not*
	/// suspend execution.
	/// </summary>
	public abstract class FaultEvent:DebugEvent
	{
        private String m_stackTrace = ""; //$NON-NLS-1$
		
		public FaultEvent(String info):base(getFirstLine(info))
		{
			int newline = info.IndexOf('\n');
			if (newline != - 1)
                m_stackTrace = info.Substring(newline + 1);
		}
		
		public FaultEvent():base()
		{
		}
		
		public abstract String name();
		
		private static String getFirstLine(String s)
		{
			int newline = s.IndexOf('\n');
			if (newline == - 1)
				return s;
			else
				return s.Substring(0, (newline) - (0));
		}
		
		/// <summary> Returns the callstack in exactly the format that it came back
		/// from the player.  That is, as a single string of the following
		/// form:
		/// 
		/// <pre>
		/// at functionName()[filename:lineNumber]
		/// at functionName()[filename:lineNumber]
		/// ...
		/// </pre>
		/// 
		/// Each line has a leading tab character.
		/// 
		/// </summary>
		/// <returns> callstack, or an empty string; never <code>null</code>
		/// </returns>
		public virtual String stackTrace()
		{
			return m_stackTrace;
		}
	}
}
