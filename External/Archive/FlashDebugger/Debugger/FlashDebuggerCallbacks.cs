// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Text;
using System.Collections.Generic;
using PluginCore.Localization;
using Flash.Tools.Debugger;
using Microsoft.Win32;

namespace FlashDebugger
{
	/// <author>
    /// 
	/// </author>
	public class FlashDebuggerCallbacks : IDebuggerCallbacks
	{
        private bool m_bComputedExeLocations;
        private System.IO.FileInfo m_httpExe;
        private System.IO.FileInfo m_playerExe;

        virtual public String HttpExeName
		{
			get { return TextHelper.GetString("Info.WebBrowserGenericName"); }
		}
		virtual public String PlayerExeName
		{
			get { return TextHelper.GetString("Info.FlashPlayerGenericName"); }
		}
		
		/**
		* @see flash.tools.debugger.IDebuggerCallbacks#getHttpExe()
		*/
		public virtual System.IO.FileInfo getHttpExe()
		{
			lock (this)
			{
				if (!m_bComputedExeLocations) recomputeExeLocations();
				return m_httpExe;
			}
		}
		
		/**
		* @see flash.tools.debugger.IDebuggerCallbacks#getPlayerExe()
		*/
		public virtual System.IO.FileInfo getPlayerExe()
		{
			lock (this)
			{
				if (!m_bComputedExeLocations) recomputeExeLocations();
				return m_playerExe;
			}
		}
		
		/**
		* @see flash.tools.debugger.IDebuggerCallbacks#recomputeExeLocations()
		*/
		public virtual void  recomputeExeLocations()
		{
			lock (this)
			{
				m_httpExe = determineExeForType("http"); //$NON-NLS-1$
				m_playerExe = determineExeForType("ShockwaveFlash.ShockwaveFlash"); //$NON-NLS-1$
				m_bComputedExeLocations = true;
			}
		}
		
		/// <summary> Note, this function is Windows-specific.</summary>
		private System.IO.FileInfo determineExeForType(String type)
		{
			try
			{
				RegistryKey typeKey = null;
				typeKey = Registry.ClassesRoot.OpenSubKey(type);
				if (typeKey == null)
				{
					return null;
				}
				RegistryKey shellKey = typeKey.OpenSubKey("shell");
				if (shellKey == null)
				{
					string typeValue = null;
					typeValue = (string)typeKey.GetValue(null);
					if (typeValue != null)
					{
						RegistryKey applicationKey = Registry.ClassesRoot.OpenSubKey(typeValue);
						if (applicationKey != null)
						{
							shellKey = applicationKey.OpenSubKey("shell");
						}
					}
				}
				if (shellKey != null)
				{
					RegistryKey exeKey = shellKey.OpenSubKey(@"open\command");
					string exeName = null;
					if (exeKey != null)
					{
						exeName = (string)exeKey.GetValue(null);
						if (exeName != null)
						{
							return new System.IO.FileInfo(extractExenameFromCommandString(exeName));
						}
					}
				}
			}
			catch {}
			return null;
		}
		
		/// <summary> 
        /// Given a command string of the form
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
				if (closingQuote == - 1) closingQuote = cmd.Length;
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
				for (; ;)
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
					if (System.IO.File.Exists(new System.IO.FileInfo(filename).FullName)) tmpBool = true;
					else tmpBool = System.IO.Directory.Exists(new System.IO.FileInfo(filename).FullName);
					if (tmpBool)
					{
						endOfFilename = nextSpace;
						break;
					}
					endOfFilename = nextSpace;
				}
                if (endOfFilename != -1 && endOfFilename < cmd.Length)
                {
                    cmd = cmd.Substring(0, (endOfFilename) - (0));
                }
			}
			return cmd;
		}
		
		/*
		* @see flash.tools.debugger.IDebuggerCallbacks#launchDebugTarget(String[])
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
				else p.StartInfo.Arguments += cmd[index];
			}
			p.Start();
			return p;
		}
		
		/// <summary> 
		/// </summary>
		public virtual String queryWindowsRegistry(String key, String value)
		{
			try
			{
				return (String)Registry.GetValue(key, value, null);
			}
			catch
			{
				return null;
			}
		}

	}

}
