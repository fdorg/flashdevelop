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
using GraphicContext = flash.graphics.g2d.GraphicContext;
using DefineTag = flash.swf.tags.DefineTag;
using DefineShape = flash.swf.tags.DefineShape;
using DefineBits = flash.swf.tags.DefineBits;
using ShapeWithStyleBuilder = flash.swf.builder.types.ShapeWithStyleBuilder;
using Point = flash.swf.builder.types.Point;
using Tag = flash.swf.Tag;
using SwfConstants = flash.swf.SwfConstants;
using FillStyle = flash.swf.types.FillStyle;
using LineStyle = flash.swf.types.LineStyle;
using Rect = flash.swf.types.Rect;
using ShapeRecord = flash.swf.types.ShapeRecord;
using StyleChangeRecord = flash.swf.types.StyleChangeRecord;
using StraightEdgeRecord = flash.swf.types.StraightEdgeRecord;
using CurvedEdgeRecord = flash.swf.types.CurvedEdgeRecord;
namespace flash.swf.builder.tags
{
	
	/// <author>  Peter Farland
	/// </author>
	public sealed class DefineShapeBuilder : TagBuilder
	{
		private DefineShapeBuilder()
		{
			tag = new DefineShape(flash.swf.TagValues_Fields.stagDefineShape3);
		}
		
		//UPGRADE_TODO: Interface 'java.awt.Shape' was converted to 'System.Drawing.Drawing2D.GraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
		public DefineShapeBuilder(System.Drawing.Drawing2D.GraphicsPath shape, GraphicContext graphicContext, bool outline, bool fill):this()
		{
			sws = new ShapeWithStyleBuilder(shape, graphicContext, outline, fill);
		}
		
		//UPGRADE_TODO: Interface 'java.awt.Shape' was converted to 'System.Drawing.Drawing2D.GraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
		public DefineShapeBuilder(System.Drawing.Drawing2D.GraphicsPath shape, Point origin, FillStyle fs, LineStyle ls, bool fill):this()
		{
			sws = new ShapeWithStyleBuilder(shape, origin, fs, ls, fill);
		}
		
		//UPGRADE_TODO: Interface 'java.awt.Shape' was converted to 'System.Drawing.Drawing2D.GraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
		public void  join(System.Drawing.Drawing2D.GraphicsPath shape)
		{
			sws.join(shape);
		}
		
		public DefineTag build()
		{
			tag.shapeWithStyle = sws.build();
			tag.bounds = getBounds(tag.shapeWithStyle.shapeRecords, tag.shapeWithStyle.linestyles);
			return tag;
		}
		
		/// <summary> Utility method that calculates the minimum bounding rectangle that encloses a list
		/// of ShapeRecords, taking into account the possible maximum stroke width of any of the
		/// supplied linestyles.
		/// </summary>
		/// <param name="records">
		/// </param>
		/// <param name="lineStyles">
		/// </param>
		/// <returns>
		/// </returns>
		public static Rect getBounds(System.Collections.IList records, System.Collections.IList lineStyles)
		{
			if (records == null || records.Count == 0)
			{
				return new Rect();
			}
			else
			{
				int x1 = 0;
				int y1 = 0;
				int x2 = 0;
				int y2 = 0;
				int x = 0;
				int y = 0;
				bool firstMove = true;
				
				System.Collections.IEnumerator it = records.GetEnumerator();
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
				while (it.MoveNext())
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
					ShapeRecord r = (ShapeRecord) it.Current;
					if (r == null)
						continue;
					
					if (r is StyleChangeRecord)
					{
						StyleChangeRecord scr = (StyleChangeRecord) r;
						x = scr.moveDeltaX;
						y = scr.moveDeltaY;
						if (firstMove)
						{
							x1 = x;
							y1 = y;
							x2 = x;
							y2 = y;
							firstMove = false;
						}
					}
					else if (r is StraightEdgeRecord)
					{
						StraightEdgeRecord ser = (StraightEdgeRecord) r;
						x = x + ser.deltaX;
						y = y + ser.deltaY;
					}
					else if (r is CurvedEdgeRecord)
					{
						CurvedEdgeRecord cer = (CurvedEdgeRecord) r;
						x = x + cer.controlDeltaX + cer.anchorDeltaX;
						y = y + cer.controlDeltaY + cer.anchorDeltaY;
					}
					
					if (x < x1)
						x1 = x;
					if (y < y1)
						y1 = y;
					if (x > x2)
						x2 = x;
					if (y > y2)
						y2 = y;
				}
				
				if (lineStyles != null && lineStyles.Count > 0)
				{
					it = lineStyles.GetEnumerator();
					int width = flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL;
					//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
					while (it.MoveNext())
					{
						//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
						LineStyle ls = (LineStyle) it.Current;
						if (ls == null)
							continue;
						else
						{
							if (width < ls.width)
								width = ls.width;
						}
					}
					
					//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
					double stroke = (int) System.Math.Round((double) (width * 0.5));
					//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
					x1 = (int) System.Math.Round((double) (x1 - stroke));
					//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
					y1 = (int) System.Math.Round((double) (y1 - stroke));
					//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
					x2 = (int) System.Math.Round((double) (x2 + stroke));
					//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
					y2 = (int) System.Math.Round((double) (y2 + stroke));
				}
				
				return new Rect(x1, x2, y1, y2);
			}
		}
		
		public static DefineShape buildImage(DefineBits tag, int width, int height)
		{
			return ImageShapeBuilder.buildImage(tag, width, height);
		}
		
		private DefineShape tag;
		private ShapeWithStyleBuilder sws;
	}
}