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

namespace Flash.Tools.Debugger
{
	
	/// <summary> Miscellaneous callbacks from the DJAPI to the debugger which is using it.
	/// 
	/// </summary>
	/// <author>  mmorearty
	/// </author>
	public interface IDebuggerCallbacks
	{
		/// <summary> Returns a name such as "firefox" or "Web browser", the name of the
		/// browser, useful for error messages. Never returns <code>null</code>.
		/// </summary>
		String HttpExeName
		{
			get;
			
		}
		/// <summary> Returns a name such as "SAFlashPlayer.exe" or "gflashplayer" or "Flash
		/// player", the name of the standalone player, useful for error messages.
		/// Never returns <code>null</code>.
		/// </summary>
		String PlayerExeName
		{
			get;
			
		}
		/// <summary> Tells the debugger to recompute the values which will be returned by
		/// getHttpExe() and getPlayerExe().
		/// 
		/// This does NOT need to be called before the first call to either of
		/// those functions.  The intent of this function is to allow the debugger
		/// to cache any expensive calculations, but still allow for the possibility
		/// of recalculating the values from time to time (e.g. when a new launch
		/// is going to happen).
		/// </summary>
		void  recomputeExeLocations();
		
		/// <summary> Returns the executable of the browser to launch for http: URLs, or
		/// <code>null</code> if not known.
		/// </summary>
		FileInfo getHttpExe();
		
		/// <summary> Returns the executable for the standalone Flash player, or <code>null</code>
		/// if not known.
		/// </summary>
		FileInfo getPlayerExe();
		
		/// <summary> Launches a debug target.  The arguments are the same as those of
		/// Runtime.exec().
		/// </summary>
		System.Diagnostics.Process launchDebugTarget(String[] cmd);
		
		/// <summary> Query the Windows registry.
		/// 
		/// </summary>
		/// <param name="key">The registry key, in a format suitable for the REG.EXE
		/// program.
		/// </param>
		/// <param name="value">The value within that key, or null for the unnamed ("empty")
		/// value
		/// </param>
		/// <returns> the value stored at the location, or null if key or value was not
		/// found
		/// </returns>
		/// <throws>  IOException </throws>
		/// <summary>             indicates the registry query failed -- warning, this can
		/// really happen! Some implementations of this function don't
		/// work on Windows 2000. So, this function should not be counted
		/// on too heavily -- you should have a backup plan.
		/// </summary>
		String queryWindowsRegistry(String key, String value);
	}
}
