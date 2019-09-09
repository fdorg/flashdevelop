// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
using System.IO;
using Trace = Flash.Util.Trace;

namespace Flash.Tools.Debugger
{
	
	/// <author>  mmorearty
	/// </author>
	public class DefaultDebuggerCallbacks : IDebuggerCallbacks
	{
		/// <summary> Returns WINDOWS, MAC, or UNIX</summary>
		private static int OS
		{
			get
			{
				if (Environment.OSVersion.Platform == PlatformID.Unix)
                    return UNIX;
                else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
					return MAC;
				else
					return WINDOWS;
			}
			
		}
		virtual public String HttpExeName
		{
			get
			{
				if (OS == UNIX)
					return UNIX_DEFAULT_BROWSER;
				else
					return Bootstrap.LocalizationManager.getLocalizedTextString("webBrowserGenericName"); //$NON-NLS-1$
			}
			
		}
		virtual public String PlayerExeName
		{
			get
			{
				if (OS == UNIX)
					return UNIX_FLASH_PLAYER;
				else
					return Bootstrap.LocalizationManager.getLocalizedTextString("flashPlayerGenericName"); //$NON-NLS-1$
			}
			
		}
		private bool m_computedExeLocations;
		private FileInfo m_httpExe;
		private FileInfo m_playerExe;
		
		private const String UNIX_DEFAULT_BROWSER = "firefox"; //$NON-NLS-1$
		private const String UNIX_FLASH_PLAYER = "flashplayer"; //$NON-NLS-1$
		
		private const int WINDOWS = 0;
		private const int MAC = 1;
		private const int UNIX = 2;
		
		/*
		* @see Flash.Tools.Debugger.IDebuggerCallbacks#getHttpExe()
		*/
		public virtual FileInfo getHttpExe()
		{
			lock (this)
			{
				if (!m_computedExeLocations)
					recomputeExeLocations();
				return m_httpExe;
			}
		}
		
		/*
		* @see Flash.Tools.Debugger.IDebuggerCallbacks#getPlayerExe()
		*/
		public virtual FileInfo getPlayerExe()
		{
			lock (this)
			{
				if (!m_computedExeLocations)
					recomputeExeLocations();
				return m_playerExe;
			}
		}
		
		/*
		* @see Flash.Tools.Debugger.IDebuggerCallbacks#recomputeExeLocations()
		*/
		public virtual void  recomputeExeLocations()
		{
			lock (this)
			{
				int os = OS;
				if (os == WINDOWS)
				{
					m_httpExe = determineExeForType("http"); //$NON-NLS-1$
					m_playerExe = determineExeForType("ShockwaveFlash.ShockwaveFlash"); //$NON-NLS-1$
				}
				else if (os == MAC)
				{
					m_httpExe = null;
					m_playerExe = null;
				}
				// probably Unix
				else
				{
					// "firefox" is default browser for unix
					m_httpExe = findUnixProgram(UNIX_DEFAULT_BROWSER);
					
					// "flashplayer" is standalone flash player on unix
					m_playerExe = findUnixProgram(UNIX_FLASH_PLAYER);
				}
				m_computedExeLocations = true;
			}
		}
		
		/// <summary> Looks for a Unix program.  Checks the PATH, and if not found there,
		/// checks the directory specified by the "application.home" Java property.
		/// ("application.home" was set by the "fdb" shell script.)
		/// 
		/// </summary>
		/// <param name="program">program to find, e.g. "firefox"
		/// </param>
		/// <returns> path, or <code>null</code> if not found.
		/// </returns>
		private FileInfo findUnixProgram(String program)
		{
#if false
			String[] cmd = new String[]{"/bin/sh", "-c", "which " + program}; //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
			try
			{
				System.Diagnostics.Process process = SupportClass.ExecSupport(cmd);

                StreamReader reader = process.StandardOutput;
				String line = reader.ReadLine();
				if (line != null)
				{
					FileInfo f = new FileInfo(line);
					bool tmpBool;
					if (File.Exists(f.FullName))
						tmpBool = true;
					else
						tmpBool = Directory.Exists(f.FullName);
					if (tmpBool)
					{
						return f;
					}
				}
				
				// Check in the Flex SDK's "bin" directory.  The "application.home"
				// property is set by the "fdb" shell script.
				String flexHome = SystemProperties.getProperty("application.home"); //$NON-NLS-1$
				if (flexHome != null)
				{
					FileInfo f = new FileInfo(flexHome + "\\" + "bin/" + program); //$NON-NLS-1$
					bool tmpBool2;
					if (File.Exists(f.FullName))
						tmpBool2 = true;
					else
						tmpBool2 = Directory.Exists(f.FullName);
					if (tmpBool2)
					{
						return f;
					}
				}
			}
			catch (IOException)
			{
				// ignore
			}
#endif
			return null;
		}
		
		/// <summary> Note, this function is Windows-specific.</summary>
		private FileInfo determineExeForType(String type)
		{
			String it = null;
			try
			{
				System.Diagnostics.Process p = new System.Diagnostics.Process();

				p.StartInfo.FileName = "cmd";
				p.StartInfo.Arguments = "/c ftype " + type;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.UseShellExecute = false;

				try
				{
					p.Start();
				}
				catch (System.ComponentModel.Win32Exception e)
				{
					Console.WriteLine("Exception = " + e.Message);
				}

                String line = null;
				type += "="; //$NON-NLS-1$

                while (it == null && (line = p.StandardOutput.ReadLine()) != null)
				{
					if (String.Compare(line.Substring(0, (type.Length) - (0)), type, true) == 0)
					{
						it = line;
						break;
					}
				}
				//p.Kill();
				
				// if we have one extract cmd = " "
				if (it != null)
				{
					int equalSign = it.IndexOf('=');
					if (equalSign != - 1)
						it = it.Substring(equalSign + 1);
					
					it = extractExenameFromCommandString(it);
				}
			}
			catch (IOException)
			{
				// means it didn't work
			}
			
			if (it != null)
				return new FileInfo(it);
			else
				return null;
		}
		
		/// <summary> Given a command string of the form
		/// "path_to_exe" args
		/// or
		/// path_to_exe args
		/// 
		/// return the path_to_exe.  Note that path_to_exe may contain spaces.
		/// </summary>
		protected internal virtual String extractExenameFromCommandString(String cmd)
		{
			// now strip trailing junk if any
			if (cmd.StartsWith("\""))
			{
				//$NON-NLS-1$
				// ftype is enclosed in quotes
				int closingQuote = cmd.IndexOf('"', 1);
				if (closingQuote == - 1)
					closingQuote = cmd.Length;
				cmd = cmd.Substring(1, (closingQuote) - (1));
			}
			else
			{
				// Some ftypes don't use enclosing quotes.  This is tricky -- we have to
				// scan through the string, stopping at each space and checking whether
				// the filename up to that point refers to a valid filename.  For example,
				// if the input string is
				//
				//     C:\Program Files\Macromedia\Flash 9\Players\SAFlashPlayer.exe %1
				//
				// then we need to stop at each space and see if that is an EXE name:
				//
				//     C:\Program.exe
				//     C:\Program Files\Macromedia\Flash.exe
				//     C:\Program Files\Macromedia\Flash 9\Players\SAFlashPlayer.exe
				
				int endOfFilename = - 1;
				for (; ; )
				{
					int nextSpace = cmd.IndexOf(' ', endOfFilename + 1);
					if (nextSpace == - 1)
					{
						endOfFilename = - 1;
						break;
					}
					String filename = cmd.Substring(0, (nextSpace) - (0));
					if (!filename.ToLower().EndsWith(".exe"))
					//$NON-NLS-1$
						filename += ".exe"; //$NON-NLS-1$
					bool tmpBool;
					if (File.Exists(new FileInfo(filename).FullName))
						tmpBool = true;
					else
						tmpBool = Directory.Exists(new FileInfo(filename).FullName);
					if (tmpBool)
					{
						endOfFilename = nextSpace;
						break;
					}
					endOfFilename = nextSpace;
				}
				if (endOfFilename != - 1 && endOfFilename < cmd.Length)
					cmd = cmd.Substring(0, (endOfFilename) - (0));
			}
			return cmd;
		}
		
		/*
		* @see Flash.Tools.Debugger.IDebuggerCallbacks#launchDebugTarget(java.lang.String[])
		*/
		public virtual System.Diagnostics.Process launchDebugTarget(String[] cmd)
		{
			System.Diagnostics.Process	p = new System.Diagnostics.Process();

			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.FileName = cmd[0];

			for (int index = 1; index < cmd.Length; index++)
			{
				if (index > 1)
				{
					p.StartInfo.Arguments += " ";
				}

				if (cmd[index].Contains(" "))
				{
					p.StartInfo.Arguments += "\"" + cmd[index] + "\"";
				}
				else
				{
					p.StartInfo.Arguments += cmd[index];
				}
			}

			p.Start();

			return p;
		}
		
		/// <summary> This implementation of queryWindowsRegistry() does not make any native
		/// calls.  I had to do it this way because it is too hard, at this point,
		/// to add native code to the Flex code tree.
		/// </summary>
		public virtual String queryWindowsRegistry(String key, String value)
		{
			String result = null;

			System.Diagnostics.Process p = new System.Diagnostics.Process();

			p.StartInfo.FileName = "reg.exe";
			p.StartInfo.Arguments = "query " + key;
			if (value == null || value.Length == 0)
			{
				p.StartInfo.Arguments += "/ve"; //$NON-NLS-1$
			}
			else
			{
				p.StartInfo.Arguments += "/v" + value; //$NON-NLS-1$
			}
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.UseShellExecute = false;

			try
			{
				p.Start();
			}
			catch (System.ComponentModel.Win32Exception e)
			{
				Console.WriteLine("Exception = " + e.Message);
			}
			
			try
			{
				StreamReader reader = p.StandardOutput;
				
				String line;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.Equals(key))
					{
						line = reader.ReadLine();
						if (line != null)
						{
							int lastTab = line.LastIndexOf('\t');
							if (lastTab != - 1)
								result = line.Substring(lastTab + 1);
						}
						break;
					}
				}
			}
			catch (IOException e)
			{
                if (Trace.error)
                {
                    Console.Error.Write(e.StackTrace);
                    Console.Error.Flush();
                }
			}
			finally
			{
				if (p != null)
				{
					// p.Kill();
					p = null;
				}
			}
			
			return result;
		}
	}
}
