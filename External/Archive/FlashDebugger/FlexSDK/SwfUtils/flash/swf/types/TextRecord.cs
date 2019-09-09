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
using DefineFont = Flash.Swf.Tags.DefineFont;
using ArrayUtil = Flash.Util.ArrayUtil;
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class TextRecord
	{
		virtual public DefineFont Font
		{
			set
			{
				this.font = value;
				flags |= HAS_FONT;
			}
			
		}
		virtual public int Height
		{
			set
			{
				this.height = value;
				flags |= HAS_HEIGHT;
			}
			
		}
		virtual public int Color
		{
			set
			{
				flags |= HAS_COLOR;
				this.color = value;
			}
			
		}
		virtual public int X
		{
			set
			{
				this.xOffset = value;
				flags |= HAS_X;
			}
			
		}
		virtual public int Y
		{
			set
			{
				this.yOffset = value;
				flags |= HAS_Y;
			}
			
		}
		private const int HAS_FONT = 8;
		private const int HAS_COLOR = 4;
		private const int HAS_X = 1;
		private const int HAS_Y = 2;
		private const int HAS_HEIGHT = 8; // yep, same as HAS_FONT.  see player/core/stags.h
		public int flags = 128;
		
		/// <summary>color as integer 0x00RRGGBB or 0xAARRGGBB </summary>
		public int color;
		
		public int xOffset;
		public int yOffset;
		public int height;
		public DefineFont font;
		public GlyphEntry[] entries;
		
		public virtual void  getReferenceList(System.Collections.IList refs)
		{
			if (hasFont() && font != null)
				refs.Add(font);
		}
		
		public virtual bool hasFont()
		{
			return (flags & HAS_FONT) != 0;
		}
		
		public virtual bool hasColor()
		{
			return (flags & HAS_COLOR) != 0;
		}
		
		public virtual bool hasX()
		{
			return (flags & HAS_X) != 0;
		}
		
		public virtual bool hasY()
		{
			return (flags & HAS_Y) != 0;
		}
		
		public virtual bool hasHeight()
		{
			return (flags & HAS_HEIGHT) != 0;
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is TextRecord)
			{
				TextRecord textRecord = (TextRecord) object_Renamed;
				
				if ((textRecord.flags == this.flags) && (textRecord.color == this.color) && (textRecord.xOffset == this.xOffset) && (textRecord.yOffset == this.yOffset) && (textRecord.height == this.height) && (textRecord.font == this.font) && (ArrayUtil.equals(textRecord.entries, this.entries)))
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
