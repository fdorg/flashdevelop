// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2006-2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;

namespace Flash.Tools.Debugger
{
	
	/// <summary> Represents an error that occurred while invoking a command-line
	/// program.  Saves the text error message that was reported
	/// by the command-line program.
	/// 
	/// </summary>
	/// <author>  mmorearty
	/// </author>
	[Serializable]
	public class CommandLineException : IOException
	{
		virtual public String[] CommandLine
		{
			get
			{
				return m_commandLine;
			}
			
		}
		/// <returns> command line message, often multi-line, never <code>null</code>
		/// </returns>
		virtual public String CommandOutput
		{
			get
			{
				return m_commandOutput;
			}
			
		}
		/// <returns> the exit value that was returned by the command-line program.
		/// </returns>
		virtual public int ExitValue
		{
			get
			{
				return m_exitValue;
			}
			
		}
		private String[] m_commandLine;
		private String m_commandOutput;
		private int m_exitValue;
		
		/// <param name="detailMessage">the detail message, e.g. "Program failed" or whatever
		/// </param>
		/// <param name="commandLine">the command and arguments that were executed, e.g.
		/// <code>{ "ls", "-l" }</code>
		/// </param>
		/// <param name="commandOutput">the text error message that was reported by the command-line
		/// program. It is common for this message to be more than one
		/// line.
		/// </param>
		/// <param name="exitValue">the exit value that was returned by the command-line program.
		/// </param>
		public CommandLineException(String detailMessage, String[] commandLine, String commandOutput, int exitValue):base(detailMessage)
		{
			
			m_commandLine = commandLine;
			m_commandOutput = commandOutput;
			m_exitValue = exitValue;
		}
	}
}
