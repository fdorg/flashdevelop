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
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class DefineVideoStream:DefineTag
	{
		public DefineVideoStream():base(Flash.Swf.TagValues.stagDefineVideoStream)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			h.defineVideoStream(this);
		}
		
		public int numFrames;
		public int width;
		public int height;
		public int deblocking;
		public bool smoothing;
		public int codecID;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineVideoStream))
			{
				DefineVideoStream defineVideoStream = (DefineVideoStream) object_Renamed;
				
				if ((defineVideoStream.numFrames == this.numFrames) && (defineVideoStream.width == this.width) && (defineVideoStream.height == this.height) && (defineVideoStream.deblocking == this.deblocking) && (defineVideoStream.smoothing == this.smoothing) && (defineVideoStream.codecID == this.codecID))
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
