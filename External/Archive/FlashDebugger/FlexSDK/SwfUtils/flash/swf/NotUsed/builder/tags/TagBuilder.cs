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
using DefineTag = flash.swf.tags.DefineTag;
namespace flash.swf.builder.tags
{
	
	/// <author>  Peter Farland
	/// </author>
	public interface TagBuilder
	{
		DefineTag build();
	}
}