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
using System.IO;

namespace Flash.Tools.Debugger.Concrete
{
	
	/// <author>  mmorearty
	/// </author>
	public abstract class AbstractPlayer : Player
	{
		public override FileInfo Path
		{
			/*
			* @see Flash.Tools.Debugger.Player#getPath()
			*/
			
			get
			{
				return m_flashPlayer;
			}
			
		}
        public override Browser Browser
		{
			/*
			* @see Flash.Tools.Debugger.Player#getBrowser()
			*/
			
			get
			{
				return m_browser; // this is null if we're using the standalone player
			}
			
		}
        public override abstract int Type { get; }
		private Browser m_browser;
		private FileInfo m_flashPlayer;
		
		public AbstractPlayer(FileInfo webBrowser, FileInfo flashPlayer)
		{
			if (webBrowser != null)
				m_browser = new DBrowser(webBrowser);
			m_flashPlayer = flashPlayer;
		}
	}
}
