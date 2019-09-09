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
	public class DefineBitsJPEG3:DefineBits
	{
		public DefineBitsJPEG3():base(Flash.Swf.TagValues.stagDefineBitsJPEG3)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			h.defineBitsJPEG3(this);
		}
		
		public long alphaDataOffset;
		public byte[] alphaData;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineBitsJPEG3))
			{
				DefineBitsJPEG3 defineBitsJPEG3 = (DefineBitsJPEG3) object_Renamed;
				
				if ((defineBitsJPEG3.alphaDataOffset == this.alphaDataOffset) && ArrayUtil.equals(defineBitsJPEG3.alphaData, this.alphaData))
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
