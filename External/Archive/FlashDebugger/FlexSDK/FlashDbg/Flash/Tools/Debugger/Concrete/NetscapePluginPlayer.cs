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

namespace Flash.Tools.Debugger.Concrete
{
	
	/// <author>  mmorearty
	/// </author>
	public class NetscapePluginPlayer:AbstractPlayer
	{
		override public int Type
		{
			/*
			* @see Flash.Tools.Debugger.Player#getType()
			*/
			
			get
			{
				return Flash.Tools.Debugger.Player.NETSCAPE_PLUGIN;
			}
			
		}
		/// <param name="path">
		/// </param>
		public NetscapePluginPlayer(FileInfo browserExe, FileInfo path):base(browserExe, path)
		{
		}
	}
}
