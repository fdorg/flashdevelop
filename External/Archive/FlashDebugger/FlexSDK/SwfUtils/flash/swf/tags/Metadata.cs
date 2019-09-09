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
	
	/// <author>  Roger Gonzalez
	/// </author>
	public class Metadata:Tag
	{
		public Metadata():base(Flash.Swf.TagValues.stagMetadata)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			h.metadata(this);
		}
		
		public  override bool Equals(System.Object o)
		{
			return ((o != null) && (o is Metadata) && (((Metadata) o).xml).Equals(xml));
		}
		
		public String xml;
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
