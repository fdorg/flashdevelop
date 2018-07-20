// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2005-2006 Adobe Systems Incorporated
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
	
	/// <summary> FileAttributes defines whole-SWF attributes. It is a place to put information
	/// that belongs in the SWF header, but for which the SWF header has no space.
	/// FileAttributes must be the first tag in a SWF file, or it will be ignored.
	/// 
	/// It is our hope that FileAttributes will be the only tag ever to have this
	/// requirement of being located at a specific place in the file.
	/// (Otherwise, a complicated set of ordering rules could ensue.)
	/// 
	/// Any information that applies to the whole SWF should hopefully be incorporated
	/// into the FileAttributes tag.
	/// 
	/// </summary>
	/// <author>  Peter Farland
	/// </author>
	public class FileAttributes:Tag
	{
		public bool hasMetadata;
		public bool actionScript3;
		public bool suppressCrossDomainCaching;
		public bool swfRelativeUrls;
		public bool useNetwork;
		
		public FileAttributes():base(Flash.Swf.TagValues.stagFileAttributes)
		{
		}
		
		public override void  visit(TagHandler handler)
		{
			handler.fileAttributes(this);
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is FileAttributes))
			{
				FileAttributes tag = (FileAttributes) object_Renamed;
				
				if ((tag.hasMetadata == this.hasMetadata) && (tag.actionScript3 == this.actionScript3) && (tag.suppressCrossDomainCaching == this.suppressCrossDomainCaching) && (tag.swfRelativeUrls == this.swfRelativeUrls) && (tag.useNetwork == this.useNetwork))
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
