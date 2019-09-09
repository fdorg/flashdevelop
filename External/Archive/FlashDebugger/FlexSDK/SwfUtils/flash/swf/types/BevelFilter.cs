// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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
	public class BevelFilter:Filter
	{
		public const int ID = 3;
		public override int getID()
		{
			return ID;
		}
		
		public int shadowColor;
		public int highlightColor;
		public int blurX;
		public int blurY;
		public int angle;
		public int distance;
		public int strength;
		public int flags;
	}
}
