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
using ShapeWithStyle = flash.swf.types.ShapeWithStyle;
using FillStyle = flash.swf.types.FillStyle;
using LineStyle = flash.swf.types.LineStyle;
using GraphicContext = flash.graphics.g2d.GraphicContext;
namespace flash.swf.builder.types
{
	
	/// <author>  Peter Farland
	/// </author>
	public sealed class ShapeWithStyleBuilder
	{
		//UPGRADE_TODO: Interface 'java.awt.Shape' was converted to 'System.Drawing.Drawing2D.GraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
		public ShapeWithStyleBuilder(System.Drawing.Drawing2D.GraphicsPath shape, GraphicContext graphicContext, bool outline, bool fill)
		{
			Point origin = new Point((double) graphicContext.getPen().X, (double) graphicContext.getPen().Y);
			System.Drawing.Brush paint = graphicContext.Paint;
			System.Drawing.Pen stroke = graphicContext.Stroke;
			
			FillStyle fs = null;
			LineStyle ls = null;
			
			if (fill && paint != null)
			{
				System.Drawing.RectangleF tempAux = shape.GetBounds();
				//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
				fs = FillStyleBuilder.build(paint, ref tempAux, graphicContext.Transform);
			}
			
			if (outline && stroke != null)
				ls = LineStyleBuilder.build(paint, stroke);
			
			init(shape, origin, fs, ls, fill);
		}
		
		//UPGRADE_TODO: Interface 'java.awt.Shape' was converted to 'System.Drawing.Drawing2D.GraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
		public ShapeWithStyleBuilder(System.Drawing.Drawing2D.GraphicsPath shape, Point origin, FillStyle fs, LineStyle ls, bool fill)
		{
			init(shape, origin, fs, ls, fill);
		}
		
		//UPGRADE_TODO: Interface 'java.awt.Shape' was converted to 'System.Drawing.Drawing2D.GraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
		private void  init(System.Drawing.Drawing2D.GraphicsPath shape, Point origin, FillStyle fs, LineStyle ls, bool fill)
		{
			builder = new ShapeBuilder(origin);
			linestyles = new System.Collections.ArrayList(2);
			fillstyles = new System.Collections.ArrayList(2);
			
			if (fill && fs != null)
			{
				builder.UseFillStyle0 = true;
				builder.CurrentFillStyle0 = addFillStyle(fs);
			}
			
			if (ls != null)
				builder.CurrentLineStyle = addLineStyle(ls);
			
			//UPGRADE_TODO: Method 'java.awt.Shape.getPathIterator' was converted to 'System.Drawing.Drawing2D.GraphicsPathIterator.GraphicsPathIterator' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtShapegetPathIterator_javaawtgeomAffineTransform'"
			builder.processShape(new PathIteratorWrapper(new System.Drawing.Drawing2D.GraphicsPathIterator(shape)));
		}
		
		public ShapeWithStyle build()
		{
			ShapeWithStyle sws = new ShapeWithStyle();
			sws.shapeRecords = builder.build().shapeRecords;
			sws.fillstyles = (System.Collections.ArrayList) fillstyles;
			sws.linestyles = (System.Collections.ArrayList) linestyles;
			
			return sws;
		}
		
		//UPGRADE_TODO: Interface 'java.awt.Shape' was converted to 'System.Drawing.Drawing2D.GraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
		public void  join(System.Drawing.Drawing2D.GraphicsPath shape)
		{
			//UPGRADE_TODO: Method 'java.awt.Shape.getPathIterator' was converted to 'System.Drawing.Drawing2D.GraphicsPathIterator.GraphicsPathIterator' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtShapegetPathIterator_javaawtgeomAffineTransform'"
			builder.processShape(new PathIteratorWrapper(new System.Drawing.Drawing2D.GraphicsPathIterator(shape)));
		}
		
		/// <summary> Adds a new fill style to the <code>FillStyleArray</code>. If the given <code>FillStyle</code>
		/// is <code>null</code>, the style is ignored and an index of 0 is returned.
		/// </summary>
		/// <param name="fs">the new <code>FillStyle</code>
		/// </param>
		/// <returns> index pointing to this fill style in the <code>FillStyleArray</code>
		/// </returns>
		public int addFillStyle(FillStyle fs)
		{
			//UPGRADE_TODO: The equivalent in .NET for method 'java.util.List.add' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
			return fillstyles.Add(fs) >= 0?((System.Collections.ArrayList) fillstyles).LastIndexOf(fs) + 1:0; //Index in a 1-based array, 0 = none
		}
		
		/// <summary> Adds a new fill style to the <code>LineStyleArray</code></summary>
		/// <param name="ls">the new <code>LineStyle</code>
		/// </param>
		/// <returns> index pointing to this line style in the <code>LineStyleArray</code>
		/// </returns>
		public int addLineStyle(LineStyle ls)
		{
			//UPGRADE_TODO: The equivalent in .NET for method 'java.util.List.add' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
			return linestyles.Add(ls) >= 0?((System.Collections.ArrayList) linestyles).LastIndexOf(ls) + 1:0; //Index in a 1-based array, 0 = none
		}
		
		private ShapeBuilder builder;
		private System.Collections.IList linestyles;
		private System.Collections.IList fillstyles;
	}
}