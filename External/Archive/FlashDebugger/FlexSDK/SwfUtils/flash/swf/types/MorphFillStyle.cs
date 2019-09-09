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
using ArrayUtil = Flash.Util.ArrayUtil;
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class MorphFillStyle
	{
		public virtual bool hasBitmapId()
		{
			return type == 0x40 || type == 0x41;
		}
		
		public int type;
		/// <summary>colors as ints: 0xAARRGGBB </summary>
		public int startColor;
		public int endColor;
		public Matrix startGradientMatrix;
		public Matrix endGradientMatrix;
		public MorphGradRecord[] gradRecords;
		public DefineTag bitmap;
		public Matrix startBitmapMatrix;
		public Matrix endBitmapMatrix;
		
		// MorphFillStyle for DefineMorphShape2
		public int ratio1, ratio2;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is MorphFillStyle)
			{
				MorphFillStyle morphFillStyle = (MorphFillStyle) object_Renamed;
				
				if ((morphFillStyle.type == this.type) && (morphFillStyle.startColor == this.startColor) && (morphFillStyle.endColor == this.endColor) && (morphFillStyle.ratio1 == this.ratio1) && (morphFillStyle.ratio2 == this.ratio2) && (((morphFillStyle.startGradientMatrix == null) && (this.startGradientMatrix == null)) || ((morphFillStyle.startGradientMatrix != null) && (this.startGradientMatrix != null) && morphFillStyle.startGradientMatrix.Equals(this.startGradientMatrix))) && (((morphFillStyle.endGradientMatrix == null) && (this.endGradientMatrix == null)) || ((morphFillStyle.endGradientMatrix != null) && (this.endGradientMatrix != null) && morphFillStyle.endGradientMatrix.Equals(this.endGradientMatrix))) && ArrayUtil.equals(morphFillStyle.gradRecords, this.gradRecords) && (((morphFillStyle.bitmap == null) && (this.bitmap == null)) || ((morphFillStyle.bitmap != null) && (this.bitmap != null) && morphFillStyle.bitmap.Equals(this.bitmap))) && (((morphFillStyle.startBitmapMatrix == null) && (this.startBitmapMatrix == null)) || ((morphFillStyle.startBitmapMatrix != null) && (this.startBitmapMatrix != null) && morphFillStyle.startBitmapMatrix.Equals(this.startBitmapMatrix))) && (((morphFillStyle.endBitmapMatrix == null) && (this.endBitmapMatrix == null)) || ((morphFillStyle.endBitmapMatrix != null) && (this.endBitmapMatrix != null) && morphFillStyle.endBitmapMatrix.Equals(this.endBitmapMatrix))))
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
