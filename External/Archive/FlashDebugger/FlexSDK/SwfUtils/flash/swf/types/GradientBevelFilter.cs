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
	public class GradientBevelFilter:BevelFilter
	{
		new public const int ID = 7;
		public override int getID()
		{
			return ID;
		}
		
		public int numcolors;
		public int[] gradientColors;
		public int[] gradientRatio;
	}
}
