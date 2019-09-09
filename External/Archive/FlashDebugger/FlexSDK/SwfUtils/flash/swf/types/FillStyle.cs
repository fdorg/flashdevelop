////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2003-2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using DefineTag = Flash.Swf.Tags.DefineTag;

namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class FillStyle
	{
		virtual public int Type
		{
			get
			{
				return type;
			}
			
			set
			{
				this.type = value;
				System.Diagnostics.Debug.Assert((value == FILL_SOLID) || (value == FILL_GRADIENT) || (value == FILL_LINEAR_GRADIENT) || (value == FILL_RADIAL_GRADIENT) || (value == FILL_VECTOR_PATTERN) || (value == FILL_RAGGED_CROSSHATCH) || (value == FILL_STIPPLE) || (value == FILL_BITS) || (value == (FILL_BITS | FILL_BITS_CLIP)) || (value == (FILL_BITS | FILL_BITS_NOSMOOTH)) || (value == (FILL_BITS | FILL_BITS_NOSMOOTH | FILL_BITS_CLIP)));
			}
			
		}
		public const int FILL_SOLID = 0;
		
		public const int FILL_GRADIENT = 0x10;
		public const int FILL_LINEAR_GRADIENT = 0x10;
		public const int FILL_RADIAL_GRADIENT = 0x12;
		public const int FILL_FOCAL_RADIAL_GRADIENT = 0x13;
		
		public const int FILL_VECTOR_PATTERN = 0x20;
		public const int FILL_RAGGED_CROSSHATCH = 0x20;
		public const int FILL_DIAGONAL_LINES = 0x21;
		public const int FILL_CROSSHATCHED_LINES = 0x22;
		public const int FILL_STIPPLE = 0x23;
		
		public const int FILL_BITS = 0x40;
		public const int FILL_BITS_CLIP = 0x01; // set if bitmap is clipped. otherwise repeating
		public const int FILL_BITS_NOSMOOTH = 0x02; // set if bitmap should not be smoothed
		
		public FillStyle()
		{
		}
		
		public FillStyle(int type, Matrix matrix, DefineTag bitmap)
		{
			Type = type;
			this.matrix = matrix;
			this.bitmap = bitmap;
		}
		
		public FillStyle(int color)
		{
			this.type = FILL_SOLID;
			this.color = color;
		}
		
		public int type;
		
		/// <summary>color as int: 0xAARRGGBB or 0x00RRGGBB </summary>
		public int color;
		public Gradient gradient;
		public Matrix matrix;
		public DefineTag bitmap;
		
		public virtual bool hasBitmapId()
		{
			return ((type & FILL_BITS) != 0);
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is FillStyle)
			{
				FillStyle fillStyle = (FillStyle) object_Renamed;
				
				if ((fillStyle.type == this.type) && (fillStyle.color == this.color) && (((fillStyle.gradient == null) && (this.gradient == null)) || (fillStyle.gradient.Equals(this.gradient))) && (((fillStyle.matrix == null) && (this.matrix == null)) || ((fillStyle.matrix != null) && (this.matrix != null) && fillStyle.matrix.Equals(this.matrix))) && (((fillStyle.bitmap == null) && (this.bitmap == null)) || ((fillStyle.bitmap != null) && (this.bitmap != null) && fillStyle.bitmap.Equals(this.bitmap))))
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
