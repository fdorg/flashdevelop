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
	public class GradientGlowFilter:GlowFilter
	{
		new public const int ID = 4;
		public override int getID()
		{
			return ID;
		}
		
		public int numcolors;
		public int[] gradientColors;
		public int[] gradientRatio;
		public int angle;
		public int distance;
	}
}
