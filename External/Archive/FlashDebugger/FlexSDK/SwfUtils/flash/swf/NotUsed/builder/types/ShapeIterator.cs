// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
namespace flash.swf.builder.types
{
	
	/// <author>  Peter Farland
	/// </author>
	public struct ShapeIterator_Fields{
		public readonly static short MOVE_TO = 0;
		public readonly static short LINE_TO = 1;
		public readonly static short QUAD_TO = 2;
		public readonly static short CUBIC_TO = 3;
		public readonly static short CLOSE = 4;
	}
	public interface ShapeIterator
	{
		//UPGRADE_NOTE: Members of interface 'ShapeIterator' were extracted into structure 'ShapeIterator_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
		bool Done
		{
			get;
			
		}
		short currentSegment(double[] coords);
		void  next();
		
	}
}