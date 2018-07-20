// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class MorphLineStyle
	{
		public int startWidth;
		public int endWidth;
		
		// MorphLineStyle2
		public int startCapsStyle;
		public int jointStyle;
		public bool hasFill;
		public bool noHScale;
		public bool noVScale;
		public bool pixelHinting;
		public bool noClose;
		public int endCapsStyle;
		public int miterLimit;
		
		public MorphFillStyle fillType;
		// end MorphLineStyle2
		
		/// <summary>colors as ints: 0xAARRGGBB </summary>
		public int startColor;
		public int endColor;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is MorphLineStyle)
			{
				MorphLineStyle morphLineStyle = (MorphLineStyle) object_Renamed;
				
				if ((morphLineStyle.startWidth == this.startWidth) && (morphLineStyle.endWidth == this.endWidth) && (morphLineStyle.startCapsStyle == this.startCapsStyle) && (morphLineStyle.jointStyle == this.jointStyle) && (morphLineStyle.hasFill == this.hasFill) && (morphLineStyle.noHScale == this.noHScale) && (morphLineStyle.noVScale == this.noVScale) && (morphLineStyle.pixelHinting == this.pixelHinting) && (morphLineStyle.noClose == this.noClose) && (morphLineStyle.endCapsStyle == this.endCapsStyle) && (morphLineStyle.miterLimit == this.miterLimit) && morphLineStyle.fillType.Equals(this.fillType) && (morphLineStyle.startColor == this.startColor) && (morphLineStyle.endColor == this.endColor))
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