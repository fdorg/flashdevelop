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
using TagHandler = Flash.Swf.TagHandler;
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class SetBackgroundColor:Tag
	{
		public SetBackgroundColor():base(Flash.Swf.TagValues.stagSetBackgroundColor)
		{
		}
		
		public SetBackgroundColor(int color):this()
		{
			this.color = color;
		}
		
		public override void  visit(TagHandler h)
		{
			h.BackgroundColor = this;
		}
		
		/// <summary>color as int: 0x00RRGGBB </summary>
		public int color;
		
		public override int GetHashCode()
		{
			return base.GetHashCode() ^ color;
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is SetBackgroundColor))
			{
				SetBackgroundColor setBackgroundColor = (SetBackgroundColor) object_Renamed;
				
				if (setBackgroundColor.color == this.color)
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
	}
}
