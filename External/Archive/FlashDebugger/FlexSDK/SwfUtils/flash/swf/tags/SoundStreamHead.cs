// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class SoundStreamHead:Tag
	{
		public const int sndCompressNone = 0;
		public const int sndCompressADPCM = 1;
		public const int sndCompressMP3 = 2;
		public const int sndCompressNoneI = 3;
		
		public SoundStreamHead(int code):base(code)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			if (code == Flash.Swf.TagValues.stagSoundStreamHead)
				h.soundStreamHead(this);
			else
				h.soundStreamHead2(this);
		}
		
		public int playbackRate;
		public int playbackSize;
		public int playbackType;
		public int compression;
		public int streamRate;
		public int streamSize;
		public int streamType;
		public int streamSampleCount;
		public int latencySeek;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is SoundStreamHead))
			{
				SoundStreamHead soundStreamHead = (SoundStreamHead) object_Renamed;
				
				if ((soundStreamHead.playbackRate == this.playbackRate) && (soundStreamHead.playbackSize == this.playbackSize) && (soundStreamHead.playbackType == this.playbackType) && (soundStreamHead.compression == this.compression) && (soundStreamHead.streamRate == this.streamRate) && (soundStreamHead.streamSize == this.streamSize) && (soundStreamHead.streamType == this.streamType) && (soundStreamHead.streamSampleCount == this.streamSampleCount) && (soundStreamHead.latencySeek == this.latencySeek))
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
