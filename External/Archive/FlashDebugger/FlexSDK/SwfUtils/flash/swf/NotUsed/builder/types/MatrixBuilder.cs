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
using SwfConstants = flash.swf.SwfConstants;
using Matrix = flash.swf.types.Matrix;
namespace flash.swf.builder.types
{
	
	/// <author>  Peter Farland
	/// </author>
	public sealed class MatrixBuilder
	{
		private MatrixBuilder()
		{
		}
		
		public static Matrix build(System.Drawing.Drawing2D.Matrix at)
		{
			Matrix matrix = new Matrix();
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			matrix.scaleX = (int) System.Math.Round((double) ((float) at.Elements.GetValue(0) * flash.swf.SwfConstants_Fields.FIXED_POINT_MULTIPLE));
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			matrix.scaleY = (int) System.Math.Round((double) ((float) at.Elements.GetValue(3) * flash.swf.SwfConstants_Fields.FIXED_POINT_MULTIPLE));
			if (matrix.scaleX != 0 || matrix.scaleY != 0)
				matrix.hasScale = true;
			
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			matrix.rotateSkew0 = (int) System.Math.Round((double) ((float) at.Elements.GetValue(1) * flash.swf.SwfConstants_Fields.FIXED_POINT_MULTIPLE)); //Yes, these are supposed
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			matrix.rotateSkew1 = (int) System.Math.Round((double) ((float) at.Elements.GetValue(2) * flash.swf.SwfConstants_Fields.FIXED_POINT_MULTIPLE)); //to be flipped
			if (matrix.rotateSkew0 != 0 || matrix.rotateSkew1 != 0)
			{
				matrix.hasRotate = true;
				//UPGRADE_ISSUE: Method 'java.awt.geom.AffineTransform.getType' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtgeomAffineTransformgetType'"
				//UPGRADE_ISSUE: Field 'java.awt.geom.AffineTransform.TYPE_MASK_ROTATION' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtgeomAffineTransformTYPE_MASK_ROTATION_f'"
				if ((at.getType() & AffineTransform.TYPE_MASK_ROTATION) != 0)
					matrix.hasScale = true; //A rotation operation in Flash requires both rotate and scale components, even if zero scale.
			}
			
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			matrix.translateX = (int) System.Math.Round((double) ((System.Single) at.OffsetX * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL));
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			matrix.translateY = (int) System.Math.Round((double) ((System.Single) at.OffsetY * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL));
			
			return matrix;
		}
		
		public static Matrix getTranslateInstance(double tx, double ty)
		{
			Matrix matrix = new Matrix();
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			matrix.translateX = (int) System.Math.Round((double) (tx * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL));
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			matrix.translateY = (int) System.Math.Round((double) (ty * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL));
			return matrix;
		}
	}
}