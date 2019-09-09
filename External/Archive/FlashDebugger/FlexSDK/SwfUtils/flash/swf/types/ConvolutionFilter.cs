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
	public class ConvolutionFilter:Filter
	{
		public const int ID = 5;
		public override int getID()
		{
			return ID;
		}
		
		public int matrixX;
		public int matrixY;
		public float divisor;
		public float bias;
		public float[] matrix;
		public int color;
		public int flags;
	}
}
