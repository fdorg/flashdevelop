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
using SwfConstants = flash.swf.SwfConstants;
using Rect = flash.swf.types.Rect;
namespace flash.swf.builder.types
{
	
	/// <author>  Peter Farland
	/// </author>
	public sealed class RectBuilder
	{
		private RectBuilder()
		{
		}
		
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		public static Rect build(ref System.Drawing.RectangleF r)
		{
			Rect rect = new Rect();
			
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			rect.xMin = (int) System.Math.Round((double) ((double) r.X * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL));
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			rect.yMin = (int) System.Math.Round((double) ((double) r.Y * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL));
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			rect.xMax = (int) System.Math.Round((double) ((double) (r.X + r.Width) * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL));
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			rect.yMax = (int) System.Math.Round((double) ((double) (r.Y + r.Height) * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL));
			
			return rect;
		}
	}
}