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
using FillStyle = flash.swf.types.FillStyle;
using GradRecord = flash.swf.types.GradRecord;
using Matrix = flash.swf.types.Matrix;
using Gradient = flash.swf.types.Gradient;
using DefineBitsLossless = flash.swf.tags.DefineBitsLossless;
using DefineBitsLosslessBuilder = flash.swf.builder.tags.DefineBitsLosslessBuilder;
using SwfConstants = flash.swf.SwfConstants;
using SwfUtils = flash.swf.SwfUtils;
using LosslessImage = flash.graphics.images.LosslessImage;
//UPGRADE_TODO: The type 'org.apache.batik.ext.awt.LinearGradientPaint' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using LinearGradientPaint = org.apache.batik.ext.awt.LinearGradientPaint;
//UPGRADE_TODO: The type 'org.apache.batik.ext.awt.RadialGradientPaint' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using RadialGradientPaint = org.apache.batik.ext.awt.RadialGradientPaint;
namespace flash.swf.builder.types
{
	
	/// <author>  Peter Farland
	/// </author>
	public sealed class FillStyleBuilder
	{
		private FillStyleBuilder()
		{
		}
		
		/// <summary> Utility method to create an appropriate <code>FillStyle</code> from a <code>Paint</code>.</summary>
		/// <param name="paint">an AWT <code>Paint</code> instance
		/// </param>
		/// <param name="bounds">- required for gradient ratio calculation
		/// </param>
		/// <returns> a new <code>FillStyle</code> representing the given paint
		/// </returns>
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		public static FillStyle build(System.Drawing.Brush paint, ref System.Drawing.RectangleF bounds, System.Drawing.Drawing2D.Matrix transform)
		{
			FillStyle fs = null;
			
			if (paint != null)
			{
				double width = (double) bounds.Width;
				double height = (double) bounds.Height;
				
				if (paint is System.Drawing.Color)
				{
					//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
					fs = new FillStyle(SwfUtils.colorToInt(ref new System.Drawing.Color[]{(System.Drawing.Color) paint}[0]));
				}
				else if (paint is System.Drawing.Drawing2D.LinearGradientBrush)
				{
					System.Drawing.Drawing2D.LinearGradientBrush gp = (System.Drawing.Drawing2D.LinearGradientBrush) paint;
					//UPGRADE_ISSUE: Method 'java.awt.geom.AffineTransform.transform' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtgeomAffineTransformtransform_javaawtgeomPoint2D_javaawtgeomPoint2D'"
					System.Drawing.PointF tempAux = System.Drawing.PointF.Empty;
					System.Drawing.PointF tempAux2 = System.Drawing.PointF.Empty;
					System.Drawing.PointF tempAux3 = transform.transform(new System.Drawing.PointF(gp.Rectangle.X, gp.Rectangle.Y), tempAux);
					//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
					System.Drawing.PointF tempAux4 = transform.transform(new System.Drawing.PointF(gp.Rectangle.Height, gp.Rectangle.Width), tempAux2);
					System.Drawing.Drawing2D.Matrix gt = objectBoundingBoxTransform(ref tempAux3, ref tempAux4, width, height, width, height);
					fs = new FillStyle();
					fs.matrix = MatrixBuilder.build(gt);
					
					fs.type = FillStyle.FILL_LINEAR_GRADIENT;
					
					fs.gradient = new Gradient();
					fs.gradient.records = new GradRecord[2];
					System.Drawing.Color tempAux5 = (gp.LinearColors)[0];
					//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
					fs.gradient.records[0] = new GradRecord(0, SwfUtils.colorToInt(ref tempAux5)); //from left
					System.Drawing.Color tempAux6 = (gp.LinearColors)[1];
					//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
					fs.gradient.records[1] = new GradRecord(255, SwfUtils.colorToInt(ref tempAux6)); //to right
				}
				else if (paint is LinearGradientPaint)
				{
					LinearGradientPaint lgp = (LinearGradientPaint) paint;
					System.Drawing.PointF start = lgp.getStartPoint();
					System.Drawing.PointF end = lgp.getEndPoint();
					
					//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
					System.Drawing.Drawing2D.Matrix gt = objectBoundingBoxTransform(ref start, ref end, width, height, width, height);
					
					fs = new FillStyle();
					fs.matrix = MatrixBuilder.build(gt);
					
					System.Drawing.Color[] colors = lgp.getColors();
					float[] ratios = lgp.getFractions();
					
					if (colors.Length == 0 || colors.Length != ratios.Length)
					//Invalid fill so we skip
					{
						return null;
					}
					else if (colors.Length == 1)
					//Solid fill
					{
						//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
						return new FillStyle(SwfUtils.colorToInt(ref colors[0]));
					}
					else
					{
						fs.type = FillStyle.FILL_LINEAR_GRADIENT;
						
						//Maximum of 8 gradient control points records
						int len = ratios.Length;
						if (len > 8)
							len = 8;
						fs.gradient = new Gradient();
						fs.gradient.records = new GradRecord[len];
						
						for (int i = 0; i < len; i++)
						{
							//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
							//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
							fs.gradient.records[i] = new GradRecord((int) System.Math.Round((double) (255 * ratios[i])), SwfUtils.colorToInt(ref colors[i]));
						}
					}
				}
				else if (paint is RadialGradientPaint)
				{
					RadialGradientPaint rgp = (RadialGradientPaint) paint;
					
					//Note: Flash doesn't support the focal point of a radial gradient
					//Point2D cp = rgp.getCenterPoint();
					//Point2D fp = rgp.getFocusPoint();
					double diameter = rgp.getRadius() * 2.0;
					double outerX = diameter * rgp.getTransform().getScaleX();
					double outerY = diameter * rgp.getTransform().getScaleY();
					
					System.Drawing.PointF tempAux7 = System.Drawing.PointF.Empty;
					System.Drawing.PointF tempAux8 = System.Drawing.PointF.Empty;
					//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
					System.Drawing.Drawing2D.Matrix gt = objectBoundingBoxTransform(ref tempAux7, ref tempAux8, width, height, outerX, outerY);
					fs = new FillStyle();
					fs.matrix = MatrixBuilder.build(gt);
					
					fs.type = FillStyle.FILL_RADIAL_GRADIENT;
					
					System.Drawing.Color[] colors = rgp.getColors();
					float[] ratios = rgp.getFractions();
					
					fs.gradient = new Gradient();
					fs.gradient.records = new GradRecord[ratios.Length <= 8?ratios.Length:8];
					for (int i = 0; i < ratios.Length && i < 8; i++)
					{
						//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
						//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
						fs.gradient.records[i] = new GradRecord((int) System.Math.Round((double) (255 * ratios[i])), SwfUtils.colorToInt(ref colors[i]));
					}
				}
				else if (paint is System.Drawing.TextureBrush)
				{
					System.Drawing.TextureBrush tp = (System.Drawing.TextureBrush) paint;
					System.Drawing.Image image = (System.Drawing.Image) tp.Image;
					
					DefineBitsLossless tag = DefineBitsLosslessBuilder.build(new LosslessImage(image));
					
					//Apply Twips Scale of 20 x 20
					System.Drawing.Drawing2D.Matrix at = new System.Drawing.Drawing2D.Matrix();
					at.Scale((System.Single) flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL, (System.Single) flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL);
					Matrix matrix = MatrixBuilder.build(at);
					
					fs = new FillStyle(FillStyle.FILL_BITS, matrix, tag);
				}
			}
			
			return fs;
		}
		
		
		/// <summary> TODO: These methods need to be called based on which of two gradient transform methods we're applying.
		/// In SVG, these are known as:
		/// - userSpaceOnUse - apply gradient transform and use gradient points directly
		/// - objectBoundingBox - use the width/height from the target object then apply gradient transform
		/// </summary>
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		private static System.Drawing.Drawing2D.Matrix objectBoundingBoxTransform(ref System.Drawing.PointF gp1, ref System.Drawing.PointF gp2, double width, double height, double scaleWidth, double scaleHeight)
		{
			System.Drawing.Drawing2D.Matrix at = new System.Drawing.Drawing2D.Matrix();
			
			//Translate gradient to the center of the bounded geometry
			at.Translate((System.Single) (width / 2), (System.Single) (height / 2));
			
			//Scale to Gradient Square (in twips)
			at.Scale((System.Single) (scaleWidth * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL / flash.swf.SwfConstants_Fields.GRADIENT_SQUARE), (System.Single) (scaleHeight * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL / flash.swf.SwfConstants_Fields.GRADIENT_SQUARE));
			
			//Rotate gradient to match geometry
			if (!gp1.IsEmpty && !gp2.IsEmpty && ((double) gp2.X - (double) gp1.X) != 0)
			{
				double mx = (double) gp2.X - (double) gp1.X;
				double my = (double) gp2.Y - (double) gp1.Y;
				double gradient = my / mx;
				double angle = System.Math.Atan(gradient);
				
				/*
				Handle the "arctan" problem - get a standard angle so that it is positive within 360 degrees
				wrt the positive x-axis in a counter-clockwise direction
				*/
				if (mx < 0)
					angle += System.Math.PI;
				else if (my < 0)
					angle += (System.Math.PI * 2.0);
				
				if (angle != 0)
					at.Rotate((float) (angle * (180 / System.Math.PI)));
			}
			
			return at;
		}
		
		/// <summary> TODO: These methods need to be called based on which of two gradient transform methods we're applying.
		/// In SVG, these are known as:
		/// - userSpaceOnUse - apply gradient transform and use gradient points directly
		/// - objectBoundingBox - use the width/height from the target object then apply gradient transform
		/// private static AffineTransform userSpaceOnUseTransform(Point2D gp1, Point2D gp2, AffineTransform gt)
		/// {
		/// double angle = 0.0;
		/// if (gt != null)
		/// {
		/// gp1 = gt.transform(gp1, null);
		/// gp2 = gt.transform(gp2, null);
		/// }
		/// //Rotate gradient to match geometry
		/// if (gp1 != null && gp2 != null && gp2.getX() - gp1.getX() != 0)
		/// {
		/// double mx = gp2.getX() - gp1.getX();
		/// double my = gp2.getY() - gp1.getY();
		/// double gradient = my / mx;
		/// angle = StrictMath.atan(gradient);
		/// //Handle the "arctan" problem - get a standard angle so that it is a positive value less than
		/// //360 degrees with respect to the positive x-axis (in a counter-clockwise direction).
		/// if (mx < 0)
		/// angle += StrictMath.PI;
		/// else if (my < 0)
		/// angle += (StrictMath.PI * 2.0);
		/// }
		/// double width = StrictMath.abs(gp2.getX() - gp1.getX());
		/// double height = StrictMath.abs(gp2.getY() - gp1.getY());
		/// AffineTransform at = new AffineTransform();
		/// //Translate gradient to the center of the bounded geometry
		/// at.translate(width / 2, height / 2);
		/// //Scale to Gradient Square (in twips)
		/// at.scale((width * SwfUtils.TWIPS_PER_PIXEL / SwfUtils.GRADIENT_SQUARE),
		/// (height * SwfUtils.TWIPS_PER_PIXEL / SwfUtils.GRADIENT_SQUARE));
		/// //Rotate if we have a significant angle
		/// if (angle != 0.0)
		/// at.rotate(angle);
		/// return at;
		/// }
		/// </summary>
	}
}