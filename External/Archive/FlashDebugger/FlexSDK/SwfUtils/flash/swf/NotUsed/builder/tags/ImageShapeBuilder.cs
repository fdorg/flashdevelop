////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2004-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using Tag = flash.swf.Tag;
using SwfConstants = flash.swf.SwfConstants;
using Point = flash.swf.builder.types.Point;
using ShapeBuilder = flash.swf.builder.types.ShapeBuilder;
using DefineBits = flash.swf.tags.DefineBits;
using DefineShape = flash.swf.tags.DefineShape;
using FillStyle = flash.swf.types.FillStyle;
using Matrix = flash.swf.types.Matrix;
using Rect = flash.swf.types.Rect;
using ShapeWithStyle = flash.swf.types.ShapeWithStyle;
namespace flash.swf.builder.tags
{
	
	/// <summary> Simple utility class for building an Image as a Shape with a
	/// bitmap fill style. This is a separate class to decouple image processing
	/// from the main Shape/Graphics2D processing required by more complicated SWF
	/// entities.
	/// 
	/// </summary>
	/// <author>  Peter Farland
	/// </author>
	public class ImageShapeBuilder
	{
		private ImageShapeBuilder()
		{
		}
		
		public static DefineShape buildImage(DefineBits tag, int width, int height)
		{
			// Create Fill Style
			Matrix matrix = new Matrix();
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			matrix.scaleX = (int) System.Math.Round((double) (flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL * flash.swf.SwfConstants_Fields.FIXED_POINT_MULTIPLE));
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			matrix.scaleY = (int) System.Math.Round((double) (flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL * flash.swf.SwfConstants_Fields.FIXED_POINT_MULTIPLE));
			matrix.hasScale = true; //Apply runtime scale of 20 (for twips)
			FillStyle fs = new FillStyle(FillStyle.FILL_BITS | FillStyle.FILL_BITS_CLIP, matrix, tag);
			
			// Apply Fill Styles
			ShapeWithStyle sws = new ShapeWithStyle();
			sws.fillstyles = new System.Collections.ArrayList();
			//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.add' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
			int fsIndex = sws.fillstyles.Add(fs) >= 0?sws.fillstyles.LastIndexOf(fs) + 1:0;
			sws.linestyles = new System.Collections.ArrayList();
			
			// Build Raw SWF Shape
			ShapeBuilder builder = new ShapeBuilder(new Point());
			builder.UseFillStyle0 = true;
			builder.CurrentFillStyle0 = fsIndex;
			builder.move(0, 0);
			builder.straight(width, 0);
			builder.straight(width, height);
			builder.straight(0, height);
			builder.straight(0, 0);
			builder.correctRoundingErrors();
			sws.shapeRecords = builder.build().shapeRecords;
			
			// Wrap up into a SWF DefineShape Tag
			DefineShape defineShape = new DefineShape(flash.swf.TagValues_Fields.stagDefineShape3);
			defineShape.bounds = new Rect(width * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL, height * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL);
			defineShape.shapeWithStyle = sws;
			
			return defineShape;
		}
	}
}