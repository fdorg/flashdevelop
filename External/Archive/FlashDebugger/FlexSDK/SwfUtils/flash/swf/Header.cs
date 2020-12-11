// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
using SystemProperties = JavaCompatibleClasses.SystemProperties;

using Rect = Flash.Swf.Types.Rect;
namespace Flash.Swf
{
	
	/// <author>  Clement Wong
	/// </author>
	public class Header
	{
		public Header()
		{
		}
		
		public static bool useCompression(int version)
		{
			if ((SystemProperties.getProperty("flex.swf.uncompressed") != null))
				return false;
			
			return version >= 6;
		}
		
		public bool compressed;
		public int version;
		public long length;
		public Rect size;
		public int rate;
		public int framecount;
	}
}
