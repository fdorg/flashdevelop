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
	
	/// <author>  mmorearty
	/// </author>
	public class AIRLaunchInfo
	{
		/// <summary> Full path to the AIR Debug Launcher, <code>adl.exe</code> (Windows) or
		/// <code>adl</code> (Mac/Linux).  This is mandatory.
		/// </summary>
		public FileInfo airDebugLauncher;
		
		/// <summary> The directory that has runtime.dll, or <code>null</code> to
		/// use the default.
		/// </summary>
		public FileInfo airRuntimeDir;
		
		/// <summary> The filename of the security policy to use, or <code>null</code> to
		/// use the default.
		/// </summary>
		public FileInfo airSecurityPolicy;
		
		/// <summary> The directory to specify as the application's content root, or
		/// <code>null</code> to not tell ADL where the content root is, in which
		/// case ADL will use the directory of the application.xml file as the
		/// content root.
		/// </summary>
		public FileInfo applicationContentRootDir;
		
		/// <summary> Command-line arguments for the user's program.  These are specific
		/// to the user's program; they are not processed by AIR itself,
		/// just passed on to the user's app.
		/// </summary>
		public String applicationArguments;
		
		/// <summary> The publisher ID to use; passed to adl's "-pubid" option.  If
		/// null, no pubid is passed to adl.
		/// </summary>
		public String airPublisherID;
	}
}
