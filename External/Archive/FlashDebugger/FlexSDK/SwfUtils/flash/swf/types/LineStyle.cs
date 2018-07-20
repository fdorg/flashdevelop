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
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class LineStyle
	{
		public LineStyle()
		{
		}
		
		public LineStyle(int color, int width)
		{
			this.color = color;
			this.width = width;
		}
		
		public int width;
		
		/// <summary>color as int: 0xAARRGGBB or 0x00RRGGBB </summary>
		public int color;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is LineStyle)
			{
				LineStyle lineStyle = (LineStyle) object_Renamed;
				
				if ((lineStyle.width == this.width) && (lineStyle.color == this.color))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		// Shape6 only:
		public int flags;
		// NOTE: As of this point, the Maelstrom SWF format spec documents the bitflags as being
		// UB[1], UB[2], etc.  This is incorrect.  Both authoring and the player apparently interpret
		// the flags as a UI16 (stored little-endian) and mask off the bits themselves.
		//
		// This isn't really in keeping with the way bitflags are stored in the rest of the format,
		// but since we've shipped, I think its now too late to remedy, and the spec will need to
		// be updated.
		//
		// Argh!  The bits in the spec are listed low-to-high, not left-to-right!
		
		// TODO - add getters/setters for the other bitflags.
		public virtual bool hasFillStyle()
		{
			return ((flags & 0x0008) != 0);
		}
		public virtual bool hasMiterJoint()
		{
			return ((flags & 0x0030) == 0x0020);
		}
		
		public int miterLimit; // UB[2] iff jointstyle = 2, 8.8 fixedpoint
		public FillStyle fillStyle; // if hasFillFlag;
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
