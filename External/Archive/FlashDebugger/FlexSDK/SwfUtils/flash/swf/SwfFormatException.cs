////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2004-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;

namespace Flash.Swf
{
	
	[Serializable]
	public class SwfFormatException:IOException
	{
		public SwfFormatException()
		{
		}
		
		public SwfFormatException(String s):base(s)
		{
		}
	}
}
