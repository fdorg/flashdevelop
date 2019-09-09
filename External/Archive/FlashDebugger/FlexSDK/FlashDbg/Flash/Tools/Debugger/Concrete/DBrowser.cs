////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;

using Browser = Flash.Tools.Debugger.Browser;

namespace Flash.Tools.Debugger.Concrete
{
	
	/// <author>  mmorearty
	/// </author>
	public class DBrowser : Browser
	{
		public override int Type
		{
			/*
			* @see Flash.Tools.Debugger.Browser#getType()
			*/
			
			get
			{
				return m_type;
			}
			
		}
        public override FileInfo Path
		{
			/*
			* @see Flash.Tools.Debugger.Browser#getPath()
			*/
			
			get
			{
				return m_path;
			}
			
		}
		private FileInfo m_path;
		private int m_type;
		
		public DBrowser(FileInfo exepath)
		{
			m_path = exepath;
			String exename = exepath.Name.ToLower();
			if (exename.Equals("iexplore.exe"))         //$NON-NLS-1$
				m_type = Flash.Tools.Debugger.Browser.INTERNET_EXPLORER;
			else if (exename.Equals("mozilla.exe"))     //$NON-NLS-1$
				m_type = Flash.Tools.Debugger.Browser.MOZILLA;
			else if (exename.Equals("firefox.exe"))     //$NON-NLS-1$
				m_type = Flash.Tools.Debugger.Browser.MOZILLA_FIREFOX;
			else if (exename.Equals("opera.exe"))       //$NON-NLS-1$
				m_type = Flash.Tools.Debugger.Browser.OPERA;
			else if (exename.Equals("netscape.exe"))    //$NON-NLS-1$
				m_type = Flash.Tools.Debugger.Browser.NETSCAPE_NAVIGATOR;
			else
				m_type = Flash.Tools.Debugger.Browser.UNKNOWN;
		}
	}
}