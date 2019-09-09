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
	public class DefineSound:DefineTag
	{
		public DefineSound():base(Flash.Swf.TagValues.stagDefineSound)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			h.defineSound(this);
		}
		
		public int format;
		public int rate;
		public int size;
		public int type;
		public long sampleCount; // U32
		public byte[] data;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineSound))
			{
				DefineSound defineSound = (DefineSound) object_Renamed;
				
				if ((defineSound.format == this.format) && (defineSound.rate == this.rate) && (defineSound.size == this.size) && (defineSound.type == this.type) && (defineSound.sampleCount == this.sampleCount) && ArrayUtil.equals(defineSound.data, this.data))
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
