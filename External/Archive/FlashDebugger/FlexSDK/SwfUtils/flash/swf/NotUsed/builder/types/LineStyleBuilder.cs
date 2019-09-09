// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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
using LineStyle = flash.swf.types.LineStyle;
using SwfConstants = flash.swf.SwfConstants;
using SwfUtils = flash.swf.SwfUtils;
namespace flash.swf.builder.types
{
	
	/// <author>  Peter Farland
	/// </author>
	public sealed class LineStyleBuilder
	{
		private LineStyleBuilder()
		{
		}
		
		public static LineStyle build(System.Drawing.Brush paint, System.Drawing.Pen thickness)
		{
			LineStyle ls = new LineStyle();
			
			if (paint != null && paint is System.Drawing.Color)
			{
				//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
				ls.color = SwfUtils.colorToInt(ref new System.Drawing.Color[]{(System.Drawing.Color) paint}[0]);
			}
			
			if (thickness != null && thickness is System.Drawing.Pen)
			{
				//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
				ls.width = (int) (System.Math.Round((double) (((System.Drawing.Pen) thickness).Width * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL)));
			}
			
			return ls;
		}
	}
}