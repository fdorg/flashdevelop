// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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
using TagValues = Flash.Swf.TagValues;
using TagHandler = Flash.Swf.TagHandler;
namespace Flash.Swf.Tags
{
	
	/// <author>  Edwin Smith
	/// </author>
	public class SetTabIndex:Tag
	{
		public SetTabIndex(int depth, int index):base(Flash.Swf.TagValues.stagSetTabIndex)
		{
			this.depth = depth;
			this.index = index;
		}
		
		public override void  visit(TagHandler tagHandler)
		{
			tagHandler.TabIndex = this;
		}
		
		public int depth;
		public int index;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			if (base.Equals(object_Renamed) && object_Renamed is SetTabIndex)
			{
				SetTabIndex other = (SetTabIndex) object_Renamed;
				return other.depth == this.depth && other.index == this.index;
			}
			else
			{
				return false;
			}
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
