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
using TagHandler = Flash.Swf.TagHandler;
using ArrayUtil = Flash.Util.ArrayUtil;
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class DefineBitsLossless:DefineBits
	{
		public DefineBitsLossless(int code):base(code)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			if (code == Flash.Swf.TagValues.stagDefineBitsLossless)
				h.defineBitsLossless(this);
			else
				h.defineBitsLossless2(this);
		}
		
		public int format;
		public int width;
		public int height;
		
		/// <summary> DefineBitsLossLess:  array of 0x00RRGGBB
		/// DefineBitsLossLess2: array of 0xAARRGGBB
		/// </summary>
		public int[] colorData;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineBitsLossless))
			{
				DefineBitsLossless defineBitsLossless = (DefineBitsLossless) object_Renamed;
				
				if ((defineBitsLossless.format == this.format) && (defineBitsLossless.width == this.width) && (defineBitsLossless.height == this.height) && ArrayUtil.equals(defineBitsLossless.colorData, this.colorData))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
