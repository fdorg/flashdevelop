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
using System;

namespace Flash.Tools.Debugger.Events
{
	
	/// <summary> This event is fired when the player has completed the loading of 
	/// the specified SWF.
	/// </summary>
	public class SwfLoadedEvent:DebugEvent
	{
		/// <summary>unique identifier for the SWF </summary>
		public long id;
		
		/// <summary>index of swf in Session.getSwfs() array </summary>
		public int index;
		
		/// <summary>full path name for  SWF </summary>
		public String path;
		
		/// <summary>size of the loaded SWF in bytes </summary>
		public long swfSize;
		
		/// <summary>URL of the loaded SWF </summary>
		public String url;
		
		/// <summary>port number related to the URL </summary>
		public long port;
		
		/// <summary>name of host in which the SWF was loaded </summary>
		public String host;
		
		public SwfLoadedEvent(long sId, int sIndex, String sPath, String sUrl, String sHost, long sPort, long sSwfSize)
		{
			id = sId;
			index = sIndex;
			swfSize = sSwfSize;
			port = sPort;
			path = sPath;
			url = sUrl;
			host = sHost;
		}
	}
}
