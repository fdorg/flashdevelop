////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2005-2007 Adobe Systems Incorporated
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
	
	/// <summary> Describes a Flash player.
	/// 
	/// </summary>
	/// <author>  mmorearty
	/// </author>
	public abstract class Player
	{
        /// <summary> Indicates a standalone Flash player, e.g. FlashPlayer.exe.
        /// 
        /// </summary>
        /// <seealso cref="Type">
        /// </seealso>
        public readonly static int STANDALONE = 1;
        /// <summary> Indicates a Netscape-plugin Flash player, e.g. NPSWF32.dll.  Used on Windows
        /// by all Netscape-based browsers (e.g. Firefox etc.).
        /// 
        /// </summary>
        /// <seealso cref="Type">
        /// </seealso>
        public readonly static int NETSCAPE_PLUGIN = 2;
        /// <summary> Indicates an ActiveX-control Flash player, e.g. Flash.ocx.  Used on Windows
        /// by Internet Explorer.
        /// 
        /// </summary>
        /// <seealso cref="Type">
        /// </seealso>
        public readonly static int ACTIVEX = 3;
        /// <summary> Indicates the Flash player inside AIR.</summary>
        public readonly static int AIR = 4;
        
        /// <summary> Returns what type of Player this is: <code>STANDALONE</code>, <code>NETSCAPE_PLUGIN</code>,
		/// <code>ACTIVEX</code>, or <code>AIR</code>.
		/// </summary>
        public abstract int Type
		{
			get;
			
		}
		/// <summary> Returns the path to the Flash player file -- e.g. the path to
		/// SAFlashPlayer.exe, NPSWF32.dll, or Flash.ocx.  (Filenames are
		/// obviously platform-specific.)
		/// 
		/// Note that the file is not guaranteed to exist.  You can use
		/// File.exists() to test that.
		/// </summary>
        public abstract FileInfo Path
		{
			get;
			
		}
		/// <summary> Returns the web browser with which this player is associated,
		/// or <code>null</code> if this is the standalone player or if
		/// we're not sure which browser will be run.
		/// </summary>
        public abstract Browser Browser
		{
			get;
			
		}
	}
}
