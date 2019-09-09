// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using Flash.Tools.Debugger.Concrete;
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
using LocalizationManager = Flash.Localization.LocalizationManager;

namespace Flash.Tools.Debugger
{
	
	/// <summary> Entry point for access to the general API.  A debugger uses this
	/// class to gain access to a SessionManager from which debugging
	/// sessions may be controlled or initiated.
	/// </summary>
	public class Bootstrap
	{
		internal static LocalizationManager LocalizationManager
		{
			get
			{
				return m_localizationManager;
			}
			
		}
		internal static SessionManager m_mgr = null;
		private static LocalizationManager m_localizationManager;
		
		private Bootstrap()
		{
		}
		
		public static SessionManager sessionManager()
		{
			if (m_mgr == null)
				m_mgr = new PlayerSessionManager();
			return m_mgr;
		}

		static Bootstrap()
		{
			{
				// set up for localizing messages
				m_localizationManager = new LocalizationManager();
				m_localizationManager.addLocalizer(new DebuggerLocalizer("Flash.Tools.Debugger.djapi.")); //$NON-NLS-1$
			}
		}
	}
}
