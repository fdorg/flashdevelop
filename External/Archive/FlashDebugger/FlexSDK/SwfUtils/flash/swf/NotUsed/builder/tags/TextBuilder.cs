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
using DefineTag = flash.swf.tags.DefineTag;
using DefineText = flash.swf.tags.DefineText;
using GlyphEntry = flash.swf.types.GlyphEntry;
using Matrix = flash.swf.types.Matrix;
using Rect = flash.swf.types.Rect;
using TextRecord = flash.swf.types.TextRecord;
using SwfConstants = flash.swf.SwfConstants;
using SwfUtils = flash.swf.SwfUtils;
namespace flash.swf.builder.tags
{
	
	/// <author>  Peter Farland
	/// <p/>
	/// Modified by s. gong
	/// </author>
	public class TextBuilder : TagBuilder
	{
		public TextBuilder(int code)
		{
			tag = new DefineText(code);
			fontBuilders = new System.Collections.ArrayList();
		}
		
		public virtual DefineTag build()
		{
			if (tag.matrix == null)
				tag.matrix = new Matrix();
			
			return tag;
		}
		/// <summary> </summary>
		/// <param name="fontBuilder">FontBuilder
		/// </param>
		/// <param name="height">double
		/// </param>
		/// <param name="text">String
		/// </param>
		/// <param name="color">Color
		/// </param>
		/// <param name="xOffset">int
		/// </param>
		/// <param name="yOffset">int
		/// </param>
		/// <throws>  IOException </throws>
		/// <summary> add text</summary>
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		public virtual void  add(FontBuilder fontBuilder, double height, System.String text, ref System.Drawing.Color color, int xOffset, int yOffset)
		{
			fontBuilders.Add(fontBuilder);
			
			//UPGRADE_ISSUE: Constructor 'java.io.BufferedReader.BufferedReader' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaioBufferedReaderBufferedReader_javaioReader'"
			System.IO.StreamReader reader = new BufferedReader(new System.IO.StringReader(text));
			System.String line;
			char[] chars;
			int yCount = 0;
			double t_width = 0.0f;
			double t_height = 0.0f;
			while (true)
			{
				line = reader.ReadLine();
				// Make sure we don't create empty TextRecords
				// A empty textRecord will crash Flash Player. Player Bug#57644
				if ((line == null) || (line.Length == 0))
					break;
				
				//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
				//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
				TextRecord tr = getStyleRecord(fontBuilder, height, ref color, xOffset, (int) (yOffset + yCount * height * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL));
				
				chars = line.ToCharArray();
				tr.entries = new GlyphEntry[chars.Length];
				double w = 0;
				for (int i = 0; i < chars.Length; i++)
				{
					char c = chars[i];
					// preilly: According to Sherman Gong, we need to clone the font GlyphEntry, so
					// that the advance value can be mapped from the font's logical scale to the
					// text's physical scale.
					GlyphEntry ge = (GlyphEntry) fontBuilder.getGlyph(c).Clone();
					//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
					ge.advance = (int) ((ge.advance / 1024f) * tr.height);
					tr.entries[i] = ge;
					w += ge.advance;
				}
				if (w > t_width)
					t_width = w;
				tag.records.Add(tr);
				yCount++;
			}
			t_height = yCount * height;
			
			double x1 = 0;
			double y1 = 0;
			double x2 = x1 + t_width;
			double y2 = y1 + t_height;
			x1 = x1 * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL;
			x2 = x2 * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL;
			y1 = y1 * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL;
			y2 = y2 * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL;
			/**
			*  If the values are greater than Max_value, then
			*  the results are not to be trusted.
			*/
			if (x1 > System.Int32.MaxValue)
				x1 = 0;
			if (x2 > System.Int32.MaxValue)
				x2 = 0;
			if (y1 > System.Int32.MaxValue)
				y1 = 0;
			if (y2 > System.Int32.MaxValue)
				y2 = 0;
			
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			tag.bounds = new Rect((int) x1, (int) x2, (int) y1, (int) y2);
		}
		
		/// <summary> Description:  This version is the same as the straight add function
		/// The difference is that we use java layout class to
		/// calculate the bounding box.
		/// 
		/// </summary>
		/// <param name="fontBuilder">FontBuilder
		/// </param>
		/// <param name="height">     double
		/// </param>
		/// <param name="text">       String
		/// </param>
		/// <param name="color">      Color
		/// </param>
		/// <param name="xOffset">    int
		/// </param>
		/// <param name="yOffset">    int
		/// </param>
		/// <param name="bounds">     Rectangle2D
		/// </param>
		/// <throws>  IOException </throws>
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		public virtual void  addWithLayout(FontBuilder fontBuilder, double height, System.String text, ref System.Drawing.Color color, int xOffset, int yOffset, ref System.Drawing.RectangleF bounds)
		{
			fontBuilders.Add(fontBuilder);
			
			//UPGRADE_ISSUE: Constructor 'java.io.BufferedReader.BufferedReader' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaioBufferedReaderBufferedReader_javaioReader'"
			System.IO.StreamReader reader = new BufferedReader(new System.IO.StringReader(text));
			System.String line;
			char[] chars;
			int yCount = 0;
			while (true)
			{
				line = reader.ReadLine();
				// Make sure we don't create empty TextRecords
				// A empty textRecord will crash Flash Player. Player Bug #102948
				if ((line == null) || (line.Length == 0))
					break;
				//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
				//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
				TextRecord tr = getStyleRecord(fontBuilder, height, ref color, xOffset, (int) (yOffset + yCount * height * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL));
				
				chars = line.ToCharArray();
				tr.entries = new GlyphEntry[chars.Length];
				for (int i = 0; i < chars.Length; i++)
				{
					char c = chars[i];
					GlyphEntry ge = (GlyphEntry) fontBuilder.getGlyph(c).Clone();
					//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
					ge.advance = (int) ((ge.advance / 1024f) * tr.height);
					tr.entries[i] = ge;
				}
				tag.records.Add(tr);
				yCount++;
			}
			/**
			*  on JDK1.4.x the bounds.getMinX() can returns values > bounds.getMaxX()
			*  and also return values > Interger.MAX_VALUE which can cause many
			*  problems when we encode the the position valures in the tagEncoder.
			*
			*  So we stay away from the getMinx, getMinxY methods, and also
			*  double check everything here.
			*
			*/
			
			double x1 = (double) bounds.X;
			double y1 = (double) bounds.Y;
			double rect_width = (double) bounds.Width;
			double rect_height = (double) bounds.Height;
			double x2 = x1 + rect_width;
			double y2 = y1 + rect_height;
			x1 = x1 * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL;
			x2 = x2 * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL;
			y1 = y1 * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL;
			y2 = y2 * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL;
			/**
			*  If the values are greater than Max_value, then
			*  the results are not to be trusted.
			*/
			if (x1 > System.Int32.MaxValue)
				x1 = 0;
			if (x2 > System.Int32.MaxValue)
				x2 = 0;
			if (y1 > System.Int32.MaxValue)
				y1 = 0;
			if (y2 > System.Int32.MaxValue)
				y2 = 0;
			
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			tag.bounds = new Rect((int) x1, (int) x2, (int) y1, (int) y2);
		}
		
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		private TextRecord getStyleRecord(FontBuilder fontBuilder, double height, ref System.Drawing.Color color, int xOffset, int yOffset)
		{
			TextRecord tr = new TextRecord();
			if (fontBuilder != null)
			{
				tr.Font = fontBuilder.tag;
				tr.Height = SwfUtils.toTwips(height);
			}
			
			if (!color.IsEmpty)
			{
				//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
				int c = SwfUtils.colorToInt(ref color);
				tr.Color = c;
			}
			
			if (xOffset > 0)
			{
				tr.X = xOffset;
			}
			
			if (yOffset > 0)
			{
				tr.Y = yOffset;
			}
			return tr;
		}
		
		private DefineText tag;
		private System.Collections.IList fontBuilders;
	}
}