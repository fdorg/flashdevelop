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
using Tag = Flash.Swf.Tag;
using TagHandler = Flash.Swf.TagHandler;
using ArrayUtil = Flash.Util.ArrayUtil;
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class VideoFrame:Tag
	{
		public VideoFrame():base(Flash.Swf.TagValues.stagVideoFrame)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			h.videoFrame(this);
		}
		
		public override Tag SimpleReference
		{
            get
            {
                return stream;
            }
		}
		
		public DefineVideoStream stream;
		public int frameNum;
		public byte[] videoData;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is VideoFrame))
			{
				VideoFrame videoFrame = (VideoFrame) object_Renamed;
				
				if (equals(videoFrame.stream, this.stream) && (videoFrame.frameNum == this.frameNum) && ArrayUtil.equals(videoFrame.videoData, this.videoData))
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
