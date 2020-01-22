// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
namespace Flash.Swf.Types
{
	
	/// <author>  Roger Gonzalez
	/// </author>
	public class DropShadowFilter:Filter
	{
		public const int ID = 0;
		public override int getID()
		{
			return ID;
		}
		public int color;
		public int blurX;
		public int blurY;
		public int angle; // really 8.8 fixedpoint, but we don't care yet
		public int distance;
		public int strength;
		public int flags;
	}
}
