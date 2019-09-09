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
namespace Flash.Swf.Tags
{
	
	/// <summary> Tag that just contains a byte[] payload.  We can use this to hold any tag
	/// in its packed format, and also to hold tags that don't need any unpacking.
	/// </summary>
	/// <author>  Clement Wong
	/// </author>
	public class GenericTag:Flash.Swf.Tag
	{
		public GenericTag(int code):base(code)
		{
		}
		
		public override void  visit(Flash.Swf.TagHandler h)
		{
			switch (code)
			{
				
				case Flash.Swf.TagValues.stagJPEGTables: 
					h.jpegTables(this);
					break;
				
				case Flash.Swf.TagValues.stagProtect: 
					h.protect(this);
					break;
				
				case Flash.Swf.TagValues.stagSoundStreamBlock: 
					h.soundStreamBlock(this);
					break;
				
				default: 
					h.unknown(this);
					break;
				
			}
		}
		
		public byte[] data;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is GenericTag))
			{
				GenericTag genericTag = (GenericTag) object_Renamed;
				
				if (equals(genericTag.data, this.data))
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
