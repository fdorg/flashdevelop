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
//UPGRADE_TODO: The type 'macromedia.abc.AbcParser' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using AbcParser = macromedia.abc.AbcParser;
//UPGRADE_TODO: The type 'macromedia.asc.embedding.CompilerHandler' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using CompilerHandler = macromedia.asc.embedding.CompilerHandler;
//UPGRADE_TODO: The type 'macromedia.asc.embedding.avmplus.ActionBlockEmitter' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using ActionBlockEmitter = macromedia.asc.embedding.avmplus.ActionBlockEmitter;
//UPGRADE_TODO: The type 'macromedia.asc.parser.ProgramNode' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using ProgramNode = macromedia.asc.parser.ProgramNode;
//UPGRADE_TODO: The type 'macromedia.asc.semantics.ObjectValue' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using ObjectValue = macromedia.asc.semantics.ObjectValue;
//UPGRADE_TODO: The type 'macromedia.asc.semantics.TypeValue' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using TypeValue = macromedia.asc.semantics.TypeValue;
//UPGRADE_TODO: The type 'macromedia.asc.util.Context' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using Context = macromedia.asc.util.Context;
//UPGRADE_TODO: The type 'macromedia.asc.util.ContextStatics' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using ContextStatics = macromedia.asc.util.ContextStatics;
//UPGRADE_TODO: The type 'macromedia.asc.util.StringPrintWriter' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using StringPrintWriter = macromedia.asc.util.StringPrintWriter;
using ActionDecoder = flash.swf.ActionDecoder;
using Dictionary = flash.swf.Dictionary;
using Header = flash.swf.Header;
using SwfDecoder = flash.swf.SwfDecoder;
using Tag = flash.swf.Tag;
using TagDecoder = flash.swf.TagDecoder;
using TagEncoder = flash.swf.TagEncoder;
using TagHandler = flash.swf.TagHandler;
using TagValues = flash.swf.TagValues;
using flash.swf.tags;
using ActionList = flash.swf.types.ActionList;
using ButtonCondAction = flash.swf.types.ButtonCondAction;
using ButtonRecord = flash.swf.types.ButtonRecord;
using ClipActionRecord = flash.swf.types.ClipActionRecord;
using CurvedEdgeRecord = flash.swf.types.CurvedEdgeRecord;
using EdgeRecord = flash.swf.types.EdgeRecord;
using FillStyle = flash.swf.types.FillStyle;
using Filter = flash.swf.types.Filter;
using GlyphEntry = flash.swf.types.GlyphEntry;
using GradRecord = flash.swf.types.GradRecord;
using ImportRecord = flash.swf.types.ImportRecord;
using LineStyle = flash.swf.types.LineStyle;
using MorphFillStyle = flash.swf.types.MorphFillStyle;
using MorphGradRecord = flash.swf.types.MorphGradRecord;
using MorphLineStyle = flash.swf.types.MorphLineStyle;
using Shape = flash.swf.types.Shape;
using ShapeRecord = flash.swf.types.ShapeRecord;
using ShapeWithStyle = flash.swf.types.ShapeWithStyle;
using SoundInfo = flash.swf.types.SoundInfo;
using StraightEdgeRecord = flash.swf.types.StraightEdgeRecord;
using StyleChangeRecord = flash.swf.types.StyleChangeRecord;
using TextRecord = flash.swf.types.TextRecord;
using FocalGradient = flash.swf.types.FocalGradient;
using KerningRecord = flash.swf.types.KerningRecord;
using Base64 = flash.util.Base64;
using FileUtils = flash.util.FileUtils;
using SwfImageUtils = flash.util.SwfImageUtils;
using Trace = flash.util.Trace;
namespace flash.swf.tools
{
	
	/// <author>  Clement Wong
	/// </author>
	/// <author>  Edwin Smith
	/// </author>
	public sealed class SwfxPrinter:TagHandler
	{
		override public Dictionary DecoderDictionary
		{
			set
			{
				this.dict = value;
			}
			
		}
		override public SetBackgroundColor BackgroundColor
		{
			set
			{
				open(value);
				out_Renamed.Write(" color='" + printRGB(value.color) + "'");
				close();
			}
			
		}
		override public SetTabIndex TabIndex
		{
			set
			{
				open(value);
				out_Renamed.Write(" depth='" + value.depth + "'");
				out_Renamed.Write(" index='" + value.index + "'");
				close();
			}
			
		}
		/// <summary> this value should get set after the header is parsed</summary>
		//UPGRADE_TODO: The 'System.Int32' structure does not have an equivalent to NULL. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1291'"
		private System.Int32 swfVersion = null;
		
		private bool abc = false;
		private bool showActions = true;
		private bool showOffset = false;
		private bool showDebugSource = false;
		private bool glyphs = true;
		private bool external = false;
		private System.String externalPrefix = null;
		private System.String externalDirectory = null;
		private bool decompile;
		private bool defunc;
		private int indent_Renamed_Field = 0;
		private bool tabbedGlyphs = false;
		
		public SwfxPrinter(System.IO.StreamWriter out_Renamed)
		{
			this.out_Renamed = out_Renamed;
		}
		
		private void  printActions(ActionList list)
		{
			if (decompile)
			{
				/*
				AsNode node;
				try
				{
				node = new Decompiler(defunc).decompile(list);
				new PrettyPrinter(out, indent).list(node);
				return;
				}
				catch (Exception e)
				{
				indent();
				out.println("// error while decompiling.  falling back to disassembler");
				}
				*/
			}
			
			Disassembler disassembler = new Disassembler(out_Renamed, showOffset, indent_Renamed_Field);
			if (showDebugSource)
			{
				disassembler.ShowDebugSource = showDebugSource;
				disassembler.Comment = "// ";
			}
			list.visitAll(disassembler);
		}
		
		private void  setExternal(bool b, System.String path)
		{
			external = b;
			
			if (external)
			{
				if (path != null)
				{
					externalPrefix = baseName(path);
					externalDirectory = dirName(path);
				}
				
				if (externalPrefix == null)
					externalPrefix = "";
				else
					externalPrefix += "-";
				if (externalDirectory == null)
					externalDirectory = "";
			}
		}
		
		private void  indent()
		{
			for (int i = 0; i < indent_Renamed_Field; i++)
			{
				out_Renamed.Write("  ");
			}
		}
		
		public override void  header(Header h)
		{
			swfVersion = (System.Int32) h.version;
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("<!-- ?xml version=\"1.0\" encoding=\"UTF-8\"? -->");
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("<swf xmlns='http://macromedia/2003/swfx'" + " version='" + h.version + "'" + " framerate='" + h.rate + "'" + " size='" + h.size + "'" + " compressed='" + h.compressed + "'" + " >");
			indent_Renamed_Field++;
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("<!-- framecount=" + h.framecount + " length=" + h.length + " -->");
		}
		
		public override void  productInfo(ProductInfo productInfo)
		{
			open(productInfo);
			out_Renamed.Write(" product='" + productInfo.ProductString + "'");
			out_Renamed.Write(" edition='" + productInfo.EditionString + "'");
			out_Renamed.Write(" version='" + productInfo.MajorVersion + "." + productInfo.MinorVersion + "'");
			out_Renamed.Write(" build='" + productInfo.Build + "'");
			//UPGRADE_TODO: Constructor 'java.util.Date.Date' was converted to 'System.DateTime.DateTime' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilDateDate_long'"
			out_Renamed.Write(" compileDate='" + SupportClass.FormatDateTime(SupportClass.GetDateTimeFormatInstance(3, 3, System.Globalization.CultureInfo.CurrentCulture), new System.DateTime(productInfo.CompileDate)) + "'");
			close();
		}
		
		public override void  metadata(Metadata tag)
		{
			open(tag);
			end();
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(tag.xml);
			close(tag);
		}
		
		public override void  fileAttributes(FileAttributes tag)
		{
			open(tag);
			out_Renamed.Write(" hasMetadata='" + tag.hasMetadata + "'");
			out_Renamed.Write(" actionScript3='" + tag.actionScript3 + "'");
			out_Renamed.Write(" suppressCrossDomainCaching='" + tag.suppressCrossDomainCaching + "'");
			out_Renamed.Write(" swfRelativeUrls='" + tag.swfRelativeUrls + "'");
			out_Renamed.Write(" useNetwork='" + tag.useNetwork + "'");
			close();
		}
		
		
		//UPGRADE_NOTE: Final was removed from the declaration of 'out '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		private System.IO.StreamWriter out_Renamed;
		
		private Dictionary dict;
		
		public override void  setOffsetAndSize(int offset, int size)
		{
			// Note: 'size' includes the size of the tag's header
			// so it is either length + 2 or length + 6.
			
			if (showOffset)
			{
				indent();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("<!--" + " offset=" + offset + " size=" + size + " -->");
			}
		}
		
		private void  open(Tag tag)
		{
			indent();
			out_Renamed.Write("<" + flash.swf.TagValues_Fields.names[tag.code]);
		}
		
		private void  end()
		{
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(">");
			indent_Renamed_Field++;
		}
		
		private void  openCDATA()
		{
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("<![CDATA[");
			indent_Renamed_Field++;
		}
		
		private void  closeCDATA()
		{
			indent_Renamed_Field--;
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("]]>");
		}
		
		private void  close()
		{
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("/>");
		}
		
		private void  close(Tag tag)
		{
			indent_Renamed_Field--;
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("</" + flash.swf.TagValues_Fields.names[tag.code] + ">");
		}
		
		public override void  error(System.String s)
		{
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("<!-- error: " + s + " -->");
		}
		
		public override void  unknown(GenericTag tag)
		{
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("<!-- unknown tag=" + tag.code + " length=" + (tag.data != null?tag.data.Length:0) + " -->");
		}
		
		public override void  showFrame(ShowFrame tag)
		{
			open(tag);
			close();
		}
		
		public override void  defineShape(DefineShape tag)
		{
			printDefineShape(tag, false);
		}
		
		private void  printDefineShape(DefineShape tag, bool alpha)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			out_Renamed.Write(" bounds='" + tag.bounds + "'");
			if (tag.code == flash.swf.TagValues_Fields.stagDefineShape6)
			{
				out_Renamed.Write(" edgebounds='" + tag.edgeBounds + "'");
				out_Renamed.Write(" usesNonScalingStrokes='" + tag.usesNonScalingStrokes + "'");
				out_Renamed.Write(" usesScalingStrokes='" + tag.usesScalingStrokes + "'");
			}
			
			end();
			
			printShapeWithStyles(tag.shapeWithStyle, alpha);
			
			close(tag);
		}
		
		private System.String id(DefineTag tag)
		{
			//UPGRADE_NOTE: Final was removed from the declaration of 'id '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
			int id = dict.getId(tag);
			return System.Convert.ToString(id);
		}
		
		//UPGRADE_NOTE: Final was removed from the declaration of 'digits '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal static readonly char[] digits = new char[]{'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
		
		/// <param name="rgb">as an integer, 0x00RRGGBB
		/// </param>
		/// <returns> string formatted as #RRGGBB
		/// </returns>
		public System.String printRGB(int rgb)
		{
			System.Text.StringBuilder b = new System.Text.StringBuilder();
			b.Append('#');
			int red = (rgb >> 16) & 255;
			b.Append(digits[(red >> 4) & 15]);
			b.Append(digits[red & 15]);
			int green = (rgb >> 8) & 255;
			b.Append(digits[(green >> 4) & 15]);
			b.Append(digits[green & 15]);
			int blue = rgb & 255;
			b.Append(digits[(blue >> 4) & 15]);
			b.Append(digits[blue & 15]);
			return b.ToString();
		}
		
		/// <param name="rgb">as an integer, 0xAARRGGBB
		/// </param>
		/// <returns> string formatted as #RRGGBBAA
		/// </returns>
		public System.String printRGBA(int rgb)
		{
			System.Text.StringBuilder b = new System.Text.StringBuilder();
			b.Append('#');
			int red = (rgb >> 16) & 255;
			b.Append(digits[(red >> 4) & 15]);
			b.Append(digits[red & 15]);
			int green = (rgb >> 8) & 255;
			b.Append(digits[(green >> 4) & 15]);
			b.Append(digits[green & 15]);
			int blue = rgb & 255;
			b.Append(digits[(blue >> 4) & 15]);
			b.Append(digits[blue & 15]);
			int alpha = (rgb >> 24) & 255;
			b.Append(digits[(alpha >> 4) & 15]);
			b.Append(digits[alpha & 15]);
			return b.ToString();
		}
		
		public override void  placeObject(PlaceObject tag)
		{
			open(tag);
			out_Renamed.Write(" idref='" + idRef(tag.ref_Renamed) + "'");
			out_Renamed.Write(" depth='" + tag.depth + "'");
			out_Renamed.Write(" matrix='" + tag.matrix + "'");
			if (tag.colorTransform != null)
			{
				out_Renamed.Write(" colorXform='" + tag.colorTransform + "'");
			}
			close();
		}
		
		public override void  removeObject(RemoveObject tag)
		{
			open(tag);
			out_Renamed.Write(" idref='" + idRef(tag.ref_Renamed) + "'");
			close();
		}
		
		public void  outputBase64(sbyte[] data)
		{
			Base64.Encoder e = new Base64.Encoder(1024);
			
			indent();
			int remain = data.Length;
			while (remain > 0)
			{
				int block = 1024;
				if (block > remain)
					block = remain;
				e.encode(data, data.Length - remain, block);
				out_Renamed.Write(e.drain());
				remain -= block;
			}
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(e.flush());
		}
		//private byte[]  jpegTable = null;
		
		public override void  defineBits(DefineBits tag)
		{
			if (tag.jpegTables == null)
			{
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("<!-- warning: no JPEG table tag found. -->");
			}
			
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			
			if (external)
			{
				System.String path = externalDirectory + externalPrefix + "image" + dict.getId(tag) + ".jpg";
				
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(" src='" + path + "' />");
				try
				{
					//UPGRADE_TODO: Constructor 'java.io.FileOutputStream.FileOutputStream' was converted to 'System.IO.FileStream.FileStream' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileOutputStreamFileOutputStream_javalangString_boolean'"
					System.IO.FileStream image = SupportClass.GetFileStream(path, false);
					SwfImageUtils.JPEG jpeg = new SwfImageUtils.JPEG(tag.jpegTables.data, tag.data);
					jpeg.write(image);
					image.Close();
				}
				catch (System.IO.IOException e)
				{
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("<!-- error: unable to write external asset file " + path + "-->");
				}
			}
			else
			{
				out_Renamed.Write(" encoding='base64'");
				end();
				outputBase64(tag.data);
				close(tag);
			}
		}
		
		public override void  defineButton(DefineButton tag)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			end();
			if (showActions)
			{
				openCDATA();
				// todo print button records
				printActions(tag.condActions[0].actionList);
				closeCDATA();
			}
			else
			{
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("<!-- " + tag.condActions[0].actionList.size() + " action(s) elided -->");
			}
			close(tag);
		}
		
		public override void  jpegTables(GenericTag tag)
		{
			open(tag);
			out_Renamed.Write(" encoding='base64'");
			end();
			outputBase64(tag.data);
			close(tag);
		}
		
		public override void  defineFont(DefineFont1 tag)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			end();
			
			if (glyphs)
			{
				for (int i = 0; i < tag.glyphShapeTable.Length; i++)
				{
					indent();
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("<glyph>");
					
					Shape shape = tag.glyphShapeTable[i];
					indent_Renamed_Field++;
					printShapeWithTabs(shape);
					indent_Renamed_Field--;
					
					indent();
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("</glyph>");
				}
			}
			close(tag);
		}
		
		public override void  defineText(DefineText tag)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			out_Renamed.Write(" bounds='" + tag.bounds + "'");
			out_Renamed.Write(" matrix='" + tag.matrix + "'");
			
			end();
			
			System.Collections.IEnumerator it = tag.records.GetEnumerator();
			
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				TextRecord tr = (TextRecord) it.Current;
				printTextRecord(tr, tag.code);
			}
			
			close(tag);
		}
		
		public override void  doAction(DoAction tag)
		{
			open(tag);
			end();
			
			if (showActions)
			{
				openCDATA();
				printActions(tag.actionList);
				closeCDATA();
			}
			else
			{
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("<!-- " + tag.actionList.size() + " action(s) elided -->");
			}
			close(tag);
		}
		
		public override void  defineFontInfo(DefineFontInfo tag)
		{
			open(tag);
			out_Renamed.Write(" idref='" + idRef(tag.font) + "'");
			out_Renamed.Write(" ansi='" + tag.ansi + "'");
			out_Renamed.Write(" italic='" + tag.italic + "'");
			out_Renamed.Write(" bold='" + tag.bold + "'");
			out_Renamed.Write(" wideCodes='" + tag.wideCodes + "'");
			out_Renamed.Write(" langCold='" + tag.langCode + "'");
			out_Renamed.Write(" name='" + tag.name + "'");
			out_Renamed.Write(" shiftJIS='" + tag.shiftJIS + "'");
			end();
			indent();
			for (int i = 0; i < tag.codeTable.Length; i++)
			{
				out_Renamed.Write((int) tag.codeTable[i]);
				if ((i + 1) % 16 == 0)
				{
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln'"
					out_Renamed.WriteLine();
					indent();
				}
				else
				{
					out_Renamed.Write(' ');
				}
			}
			if (tag.codeTable.Length % 16 != 0)
			{
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln'"
				out_Renamed.WriteLine();
				indent();
			}
			close(tag);
		}
		
		public override void  defineSound(DefineSound tag)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			out_Renamed.Write(" format='" + tag.format + "'");
			out_Renamed.Write(" rate='" + tag.rate + "'");
			out_Renamed.Write(" size='" + tag.size + "'");
			out_Renamed.Write(" type='" + tag.type + "'");
			out_Renamed.Write(" sampleCount='" + tag.sampleCount + "'");
			out_Renamed.Write(" soundDataSize='" + tag.data.Length + "'");
			end();
			openCDATA();
			outputBase64(tag.data);
			closeCDATA();
			close(tag);
		}
		
		public override void  startSound(StartSound tag)
		{
			open(tag);
			out_Renamed.Write(" soundid='" + idRef(tag.sound) + "'");
			printSoundInfo(tag.soundInfo);
			close(tag);
		}
		
		private void  printSoundInfo(SoundInfo info)
		{
			out_Renamed.Write(" syncStop='" + info.syncStop + "'");
			out_Renamed.Write(" syncNoMultiple='" + info.syncNoMultiple + "'");
			if (info.inPoint != SoundInfo.UNINITIALIZED)
			{
				out_Renamed.Write(" inPoint='" + info.inPoint + "'");
			}
			if (info.outPoint != SoundInfo.UNINITIALIZED)
			{
				out_Renamed.Write(" outPoint='" + info.outPoint + "'");
			}
			if (info.loopCount != SoundInfo.UNINITIALIZED)
			{
				out_Renamed.Write(" loopCount='" + info.loopCount + "'");
			}
			end();
			if (info.records != null && info.records.Length > 0)
			{
				openCDATA();
				for (int i = 0; i < info.records.Length; i++)
				{
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_long'"
					out_Renamed.WriteLine(info.records[i]);
				}
				closeCDATA();
			}
		}
		
		public override void  defineButtonSound(DefineButtonSound tag)
		{
			open(tag);
			out_Renamed.Write(" buttonId='" + idRef(tag.button) + "'");
			close();
		}
		
		public override void  soundStreamHead(SoundStreamHead tag)
		{
			open(tag);
			close();
		}
		
		public override void  soundStreamBlock(GenericTag tag)
		{
			open(tag);
			close();
		}
		
		public override void  defineBinaryData(DefineBinaryData tag)
		{
			open(tag);
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(" id='" + id(tag) + "' length='" + tag.data.Length + "' />");
		}
		public override void  defineBitsLossless(DefineBitsLossless tag)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "' width='" + tag.width + "' height='" + tag.height + "'");
			
			if (external)
			{
				System.String path = externalDirectory + externalPrefix + "image" + dict.getId(tag) + ".bitmap";
				
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(" src='" + path + "' />");
				try
				{
					//UPGRADE_TODO: Constructor 'java.io.FileOutputStream.FileOutputStream' was converted to 'System.IO.FileStream.FileStream' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileOutputStreamFileOutputStream_javalangString_boolean'"
					System.IO.FileStream image = SupportClass.GetFileStream(path, false);
					SupportClass.WriteOutput(image, tag.data);
					image.Close();
				}
				catch (System.IO.IOException e)
				{
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("<!-- error: unable to write external asset file " + path + "-->");
				}
			}
			else
			{
				out_Renamed.Write(" encoding='base64'");
				end();
				outputBase64(tag.data);
				close(tag);
			}
		}
		
		public override void  defineBitsJPEG2(DefineBits tag)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			
			if (external)
			{
				System.String path = externalDirectory + externalPrefix + "image" + dict.getId(tag) + ".jpg";
				
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(" src='" + path + "' />");
				try
				{
					//UPGRADE_TODO: Constructor 'java.io.FileOutputStream.FileOutputStream' was converted to 'System.IO.FileStream.FileStream' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileOutputStreamFileOutputStream_javalangString_boolean'"
					System.IO.FileStream image = SupportClass.GetFileStream(path, false);
					SupportClass.WriteOutput(image, tag.data);
					image.Close();
				}
				catch (System.IO.IOException e)
				{
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("<!-- error: unable to write external asset file " + path + "-->");
				}
			}
			else
			{
				out_Renamed.Write(" encoding='base64'");
				end();
				outputBase64(tag.data);
				close(tag);
			}
		}
		
		public override void  defineShape2(DefineShape tag)
		{
			printDefineShape(tag, false);
		}
		
		public override void  defineButtonCxform(DefineButtonCxform tag)
		{
			open(tag);
			out_Renamed.Write(" buttonId='" + idRef(tag.button) + "'");
			close();
		}
		
		public override void  protect(GenericTag tag)
		{
			open(tag);
			if (tag.data != null)
				out_Renamed.Write(" password='" + hexify(tag.data) + "'");
			close();
		}
		
		public override void  placeObject2(PlaceObject tag)
		{
			placeObject23(tag);
		}
		
		public override void  placeObject3(PlaceObject tag)
		{
			placeObject23(tag);
		}
		
		public void  placeObject23(PlaceObject tag)
		{
			if (tag.hasCharID())
			{
				if (tag.ref_Renamed.name != null)
				{
					indent();
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("<!-- instance of " + tag.ref_Renamed.name + " -->");
				}
			}
			
			open(tag);
			if (tag.hasClassName())
				out_Renamed.Write(" className='" + tag.className + "'");
			if (tag.hasImage())
				out_Renamed.Write(" hasImage='true' ");
			if (tag.hasCharID())
				out_Renamed.Write(" idref='" + idRef(tag.ref_Renamed) + "'");
			if (tag.hasName())
				out_Renamed.Write(" name='" + tag.name + "'");
			out_Renamed.Write(" depth='" + tag.depth + "'");
			if (tag.hasClipDepth())
				out_Renamed.Write(" clipDepth='" + tag.clipDepth + "'");
			if (tag.hasRatio())
				out_Renamed.Write(" ratio='" + tag.ratio + "'");
			if (tag.hasCxform())
			{
				out_Renamed.Write(" cxform='" + tag.colorTransform + "'");
			}
			if (tag.hasMatrix())
			{
				out_Renamed.Write(" matrix='" + tag.matrix + "'");
			}
			if (tag.hasBlendMode())
				out_Renamed.Write(" blendmode='" + tag.blendMode + "'");
			if (tag.hasFilterList())
			{
				// todo - pretty print this once we actually care
				out_Renamed.Write(" filters='");
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
				for (System.Collections.IEnumerator it = tag.filters.GetEnumerator(); it.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
					out_Renamed.Write((((Filter) it.Current).getID()) + " ");
				}
				out_Renamed.Write("'");
			}
			
			if (tag.hasClipAction())
			{
				end();
				System.Collections.IEnumerator it = tag.clipActions.clipActionRecords.GetEnumerator();
				
				openCDATA();
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
				while (it.MoveNext())
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
					ClipActionRecord record = (ClipActionRecord) it.Current;
					indent();
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("onClipEvent(" + printClipEventFlags(record.eventFlags) + (record.hasKeyPress()?"<" + record.keyCode + ">":"") + ") {");
					indent_Renamed_Field++;
					if (showActions)
					{
						printActions(record.actionList);
					}
					else
					{
						indent();
						//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
						out_Renamed.WriteLine("// " + record.actionList.size() + " action(s) elided");
					}
					indent_Renamed_Field--;
					indent();
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("}");
				}
				closeCDATA();
				close(tag);
			}
			else
			{
				close();
			}
		}
		
		public override void  removeObject2(RemoveObject tag)
		{
			open(tag);
			out_Renamed.Write(" depth='" + tag.depth + "'");
			close();
		}
		
		public override void  defineShape3(DefineShape tag)
		{
			printDefineShape(tag, true);
		}
		
		public override void  defineShape6(DefineShape tag)
		{
			printDefineShape(tag, true);
		}
		
		private void  printShapeWithStyles(ShapeWithStyle shapes, bool alpha)
		{
			printFillStyles(shapes.fillstyles, alpha);
			printLineStyles(shapes.linestyles, alpha);
			printShape(shapes, alpha);
		}
		
		private void  printMorphLineStyles(MorphLineStyle[] lineStyles)
		{
			for (int i = 0; i < lineStyles.Length; i++)
			{
				MorphLineStyle lineStyle = lineStyles[i];
				indent();
				out_Renamed.Write("<linestyle ");
				out_Renamed.Write("startColor='" + printRGBA(lineStyle.startColor) + "' ");
				out_Renamed.Write("endColor='" + printRGBA(lineStyle.startColor) + "' ");
				out_Renamed.Write("startWidth='" + lineStyle.startWidth + "' ");
				out_Renamed.Write("endWidth='" + lineStyle.endWidth + "' ");
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("/>");
			}
		}
		
		private void  printLineStyles(System.Collections.ArrayList linestyles, bool alpha)
		{
			System.Collections.IEnumerator it = linestyles.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				LineStyle lineStyle = (LineStyle) it.Current;
				indent();
				out_Renamed.Write("<linestyle ");
				System.String color = alpha?printRGBA(lineStyle.color):printRGB(lineStyle.color);
				out_Renamed.Write("color='" + color + "' ");
				out_Renamed.Write("width='" + lineStyle.width + "' ");
				if (lineStyle.flags != 0)
					out_Renamed.Write("flags='" + lineStyle.flags + "' ");
				if (lineStyle.hasMiterJoint())
				{
					out_Renamed.Write("miterLimit='" + lineStyle.miterLimit + "' ");
				}
				if (lineStyle.hasFillStyle())
				{
					//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
					out_Renamed.Write("fillStyle='" + lineStyle.fillStyle + "' ");
				}
				
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("/>");
			}
		}
		
		private void  printFillStyles(System.Collections.ArrayList fillstyles, bool alpha)
		{
			System.Collections.IEnumerator it = fillstyles.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				FillStyle fillStyle = (FillStyle) it.Current;
				indent();
				out_Renamed.Write("<fillstyle");
				out_Renamed.Write(" type='" + fillStyle.Type + "'");
				if (fillStyle.Type == FillStyle.FILL_SOLID)
				{
					out_Renamed.Write(" color='" + (alpha?printRGBA(fillStyle.color):printRGB(fillStyle.color)) + "'");
				}
				if ((fillStyle.Type & FillStyle.FILL_LINEAR_GRADIENT) != 0)
				{
					if (fillStyle.Type == FillStyle.FILL_RADIAL_GRADIENT)
						out_Renamed.Write("type='radial'");
					else if (fillStyle.Type == FillStyle.FILL_FOCAL_RADIAL_GRADIENT)
						out_Renamed.Write("type='focal' focalPoint='" + ((FocalGradient) fillStyle.gradient).focalPoint + "'");
					// todo print linear or radial or focal
					out_Renamed.Write(" gradient='" + formatGradient(fillStyle.gradient.records, alpha) + "'");
					out_Renamed.Write(" matrix='" + fillStyle.matrix + "'");
				}
				if ((fillStyle.Type & FillStyle.FILL_BITS) != 0)
				{
					// todo print tiled or clipped
					out_Renamed.Write(" idref='" + idRef(fillStyle.bitmap) + "'");
					out_Renamed.Write(" matrix='" + fillStyle.matrix + "'");
				}
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(" />");
			}
		}
		
		private void  printMorphFillStyles(MorphFillStyle[] fillStyles)
		{
			for (int i = 0; i < fillStyles.Length; i++)
			{
				MorphFillStyle fillStyle = fillStyles[i];
				indent();
				out_Renamed.Write("<fillstyle");
				out_Renamed.Write(" type='" + fillStyle.type + "'");
				if (fillStyle.type == FillStyle.FILL_SOLID)
				{
					out_Renamed.Write(" startColor='" + printRGBA(fillStyle.startColor) + "'");
					out_Renamed.Write(" endColor='" + printRGBA(fillStyle.endColor) + "'");
				}
				if ((fillStyle.type & FillStyle.FILL_LINEAR_GRADIENT) != 0)
				{
					// todo print linear or radial
					out_Renamed.Write(" gradient='" + formatMorphGradient(fillStyle.gradRecords) + "'");
					out_Renamed.Write(" startMatrix='" + fillStyle.startGradientMatrix + "'");
					out_Renamed.Write(" endMatrix='" + fillStyle.endGradientMatrix + "'");
				}
				if ((fillStyle.type & FillStyle.FILL_BITS) != 0)
				{
					// todo print tiled or clipped
					out_Renamed.Write(" idref='" + idRef(fillStyle.bitmap) + "'");
					out_Renamed.Write(" startMatrix='" + fillStyle.startBitmapMatrix + "'");
					out_Renamed.Write(" endMatrix='" + fillStyle.endBitmapMatrix + "'");
				}
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(" />");
			}
		}
		
		private System.String formatGradient(GradRecord[] records, bool alpha)
		{
			System.Text.StringBuilder b = new System.Text.StringBuilder();
			for (int i = 0; i < records.Length; i++)
			{
				b.Append(records[i].ratio);
				b.Append(' ');
				b.Append(alpha?printRGBA(records[i].color):printRGB(records[i].color));
				if (i + 1 < records.Length)
					b.Append(' ');
			}
			return b.ToString();
		}
		
		private System.String formatMorphGradient(MorphGradRecord[] records)
		{
			System.Text.StringBuilder b = new System.Text.StringBuilder();
			for (int i = 0; i < records.Length; i++)
			{
				b.Append(records[i].startRatio);
				b.Append(',');
				b.Append(records[i].endRatio);
				b.Append(' ');
				b.Append(printRGBA(records[i].startColor));
				b.Append(',');
				b.Append(printRGBA(records[i].endColor));
				if (i + 1 < records.Length)
					b.Append(' ');
			}
			return b.ToString();
		}
		
		private void  printShape(Shape shapes, bool alpha)
		{
			System.Collections.IEnumerator it = shapes.shapeRecords.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext())
			{
				indent();
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				ShapeRecord shape = (ShapeRecord) it.Current;
				if (shape is StyleChangeRecord)
				{
					StyleChangeRecord styleChange = (StyleChangeRecord) shape;
					out_Renamed.Write("<styleChange ");
					if (styleChange.stateMoveTo)
					{
						out_Renamed.Write("dx='" + styleChange.moveDeltaX + "' dy='" + styleChange.moveDeltaY + "' ");
					}
					if (styleChange.stateFillStyle0)
					{
						out_Renamed.Write("fillStyle0='" + styleChange.fillstyle0 + "' ");
					}
					if (styleChange.stateFillStyle1)
					{
						out_Renamed.Write("fillStyle1='" + styleChange.fillstyle1 + "' ");
					}
					if (styleChange.stateLineStyle)
					{
						out_Renamed.Write("lineStyle='" + styleChange.linestyle + "' ");
					}
					if (styleChange.stateNewStyles)
					{
						//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
						out_Renamed.WriteLine(">");
						indent_Renamed_Field++;
						printFillStyles(styleChange.fillstyles, alpha);
						printLineStyles(styleChange.linestyles, alpha);
						indent_Renamed_Field--;
						indent();
						//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
						out_Renamed.WriteLine("</styleChange>");
					}
					else
					{
						//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
						out_Renamed.WriteLine("/>");
					}
				}
				else
				{
					EdgeRecord edge = (EdgeRecord) shape;
					if (edge is StraightEdgeRecord)
					{
						StraightEdgeRecord straightEdge = (StraightEdgeRecord) edge;
						//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
						out_Renamed.WriteLine("<line dx='" + straightEdge.deltaX + "' dy='" + straightEdge.deltaY + "' />");
					}
					else
					{
						CurvedEdgeRecord curvedEdge = (CurvedEdgeRecord) edge;
						out_Renamed.Write("<curve ");
						out_Renamed.Write("cdx='" + curvedEdge.controlDeltaX + "' cdy='" + curvedEdge.controlDeltaY + "' ");
						out_Renamed.Write("dx='" + curvedEdge.anchorDeltaX + "' dy='" + curvedEdge.anchorDeltaY + "' ");
						//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
						out_Renamed.WriteLine("/>");
					}
				}
			}
		}
		
		private void  printShapeWithTabs(Shape shapes)
		{
			System.Collections.IEnumerator it = shapes.shapeRecords.GetEnumerator();
			int startX = 0;
			int startY = 0;
			
			int x = 0;
			int y = 0;
			
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext())
			{
				indent();
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				ShapeRecord shape = (ShapeRecord) it.Current;
				if (shape is StyleChangeRecord)
				{
					StyleChangeRecord styleChange = (StyleChangeRecord) shape;
					out_Renamed.Write("SSCR" + styleChange.nMoveBits() + "\t");
					if (styleChange.stateMoveTo)
					{
						out_Renamed.Write(styleChange.moveDeltaX + "\t" + styleChange.moveDeltaY);
						
						if (startX == 0 && startY == 0)
						{
							startX = styleChange.moveDeltaX;
							startY = styleChange.moveDeltaY;
						}
						
						x = styleChange.moveDeltaX;
						y = styleChange.moveDeltaY;
						
						out_Renamed.Write("\t\t");
					}
				}
				else
				{
					EdgeRecord edge = (EdgeRecord) shape;
					if (edge is StraightEdgeRecord)
					{
						StraightEdgeRecord straightEdge = (StraightEdgeRecord) edge;
						out_Renamed.Write("SER" + "\t");
						out_Renamed.Write(straightEdge.deltaX + "\t" + straightEdge.deltaY);
						x += straightEdge.deltaX;
						y += straightEdge.deltaY;
						out_Renamed.Write("\t\t");
					}
					else
					{
						CurvedEdgeRecord curvedEdge = (CurvedEdgeRecord) edge;
						out_Renamed.Write("CER" + "\t");
						out_Renamed.Write(curvedEdge.controlDeltaX + "\t" + curvedEdge.controlDeltaY + "\t");
						out_Renamed.Write(curvedEdge.anchorDeltaX + "\t" + curvedEdge.anchorDeltaY);
						x += (curvedEdge.controlDeltaX + curvedEdge.anchorDeltaX);
						y += (curvedEdge.controlDeltaY + curvedEdge.anchorDeltaY);
					}
				}
				
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("\t\t" + x + "\t" + y);
			}
		}
		
		private System.String printClipEventFlags(int flags)
		{
			System.Text.StringBuilder b = new System.Text.StringBuilder();
			
			if ((flags & ClipActionRecord.unused31) != 0)
				b.Append("res31,");
			if ((flags & ClipActionRecord.unused30) != 0)
				b.Append("res30,");
			if ((flags & ClipActionRecord.unused29) != 0)
				b.Append("res29,");
			if ((flags & ClipActionRecord.unused28) != 0)
				b.Append("res28,");
			if ((flags & ClipActionRecord.unused27) != 0)
				b.Append("res27,");
			if ((flags & ClipActionRecord.unused26) != 0)
				b.Append("res26,");
			if ((flags & ClipActionRecord.unused25) != 0)
				b.Append("res25,");
			if ((flags & ClipActionRecord.unused24) != 0)
				b.Append("res24,");
			
			if ((flags & ClipActionRecord.unused23) != 0)
				b.Append("res23,");
			if ((flags & ClipActionRecord.unused22) != 0)
				b.Append("res22,");
			if ((flags & ClipActionRecord.unused21) != 0)
				b.Append("res21,");
			if ((flags & ClipActionRecord.unused20) != 0)
				b.Append("res20,");
			if ((flags & ClipActionRecord.unused19) != 0)
				b.Append("res19,");
			if ((flags & ClipActionRecord.construct) != 0)
				b.Append("construct,");
			if ((flags & ClipActionRecord.keyPress) != 0)
				b.Append("keyPress,");
			if ((flags & ClipActionRecord.dragOut) != 0)
				b.Append("dragOut,");
			
			if ((flags & ClipActionRecord.dragOver) != 0)
				b.Append("dragOver,");
			if ((flags & ClipActionRecord.rollOut) != 0)
				b.Append("rollOut,");
			if ((flags & ClipActionRecord.rollOver) != 0)
				b.Append("rollOver,");
			if ((flags & ClipActionRecord.releaseOutside) != 0)
				b.Append("releaseOutside,");
			if ((flags & ClipActionRecord.release) != 0)
				b.Append("release,");
			if ((flags & ClipActionRecord.press) != 0)
				b.Append("press,");
			if ((flags & ClipActionRecord.initialize) != 0)
				b.Append("initialize,");
			if ((flags & ClipActionRecord.data) != 0)
				b.Append("data,");
			
			if ((flags & ClipActionRecord.keyUp) != 0)
				b.Append("keyUp,");
			if ((flags & ClipActionRecord.keyDown) != 0)
				b.Append("keyDown,");
			if ((flags & ClipActionRecord.mouseUp) != 0)
				b.Append("mouseUp,");
			if ((flags & ClipActionRecord.mouseDown) != 0)
				b.Append("mouseDown,");
			if ((flags & ClipActionRecord.mouseMove) != 0)
				b.Append("moseMove,");
			if ((flags & ClipActionRecord.unload) != 0)
				b.Append("unload,");
			if ((flags & ClipActionRecord.enterFrame) != 0)
				b.Append("enterFrame,");
			if ((flags & ClipActionRecord.load) != 0)
				b.Append("load,");
			if (b.Length > 1)
			{
				b.Length -= 1;
			}
			return b.ToString();
		}
		
		public override void  defineText2(DefineText tag)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			end();
			
			System.Collections.IEnumerator it = tag.records.GetEnumerator();
			
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				TextRecord tr = (TextRecord) it.Current;
				printTextRecord(tr, tag.code);
			}
			
			close(tag);
		}
		
		public void  printTextRecord(TextRecord tr, int tagCode)
		{
			indent();
			out_Renamed.Write("<textRecord ");
			if (tr.hasFont())
				out_Renamed.Write(" font='" + tr.font.FontName + "'");
			
			if (tr.hasHeight())
				out_Renamed.Write(" height='" + tr.height + "'");
			
			if (tr.hasX())
				out_Renamed.Write(" xOffset='" + tr.xOffset + "'");
			
			if (tr.hasY())
				out_Renamed.Write(" yOffset='" + tr.yOffset + "'");
			
			if (tr.hasColor())
				out_Renamed.Write(" color='" + (tagCode == flash.swf.TagValues_Fields.stagDefineEditText?printRGB(tr.color):printRGBA(tr.color)) + "'");
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(">");
			
			indent_Renamed_Field++;
			printGlyphEntries(tr);
			indent_Renamed_Field--;
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("</textRecord>");
		}
		
		private void  printGlyphEntries(TextRecord tr)
		{
			indent();
			for (int i = 0; i < tr.entries.Length; i++)
			{
				GlyphEntry ge = tr.entries[i];
				out_Renamed.Write(ge.Index);
				if (ge.advance >= 0)
					out_Renamed.Write('+');
				out_Renamed.Write(ge.advance);
				out_Renamed.Write(' ');
				if ((i + 1) % 10 == 0)
				{
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln'"
					out_Renamed.WriteLine();
					indent();
				}
			}
			if (tr.entries.Length % 10 != 0)
			{
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln'"
				out_Renamed.WriteLine();
			}
		}
		
		
		public override void  defineButton2(DefineButton tag)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			out_Renamed.Write(" trackAsMenu='" + tag.trackAsMenu + "'");
			end();
			
			for (int i = 0; i < tag.buttonRecords.Length; i++)
			{
				ButtonRecord record = tag.buttonRecords[i];
				indent();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("<buttonRecord " + "idref='" + idRef(record.characterRef) + "' " + "depth='" + record.placeDepth + "' " + "matrix='" + record.placeMatrix + "' " + "states='" + record.Flags + "'/>");
				// todo print optional cxforma
			}
			
			// print conditional actions
			if (tag.condActions.Length > 0 && showActions)
			{
				indent();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("<buttonCondAction>");
				openCDATA();
				for (int i = 0; i < tag.condActions.Length; i++)
				{
					ButtonCondAction cond = tag.condActions[i];
					indent();
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("on(" + cond + ") {");
					indent_Renamed_Field++;
					printActions(cond.actionList);
					indent_Renamed_Field--;
					indent();
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("}");
				}
				closeCDATA();
				indent();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("</buttonCondAction>");
			}
			
			close(tag);
		}
		
		public override void  defineBitsJPEG3(DefineBitsJPEG3 tag)
		{
			// We don't support
			// FIXME
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			close();
		}
		
		public override void  defineBitsLossless2(DefineBitsLossless tag)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			
			if (external)
			{
				System.String path = externalDirectory + externalPrefix + "image" + dict.getId(tag) + ".bitmap";
				
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(" src='" + path + "' />");
				try
				{
					//UPGRADE_TODO: Constructor 'java.io.FileOutputStream.FileOutputStream' was converted to 'System.IO.FileStream.FileStream' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileOutputStreamFileOutputStream_javalangString_boolean'"
					System.IO.FileStream image = SupportClass.GetFileStream(path, false);
					SupportClass.WriteOutput(image, tag.data);
					image.Close();
				}
				catch (System.IO.IOException e)
				{
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("<!-- error: unable to write external asset file " + path + "-->");
				}
			}
			else
			{
				out_Renamed.Write(" encoding='base64'");
				end();
				outputBase64(tag.data);
				close(tag);
			}
		}
		
		internal System.String escape(System.String s)
		{
			if (s == null)
				return null;
			
			System.Text.StringBuilder b = new System.Text.StringBuilder(s.Length);
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				switch (c)
				{
					
					case '<': 
						b.Append("&lt;");
						break;
					
					case '>': 
						b.Append("&gt;");
						break;
					
					case '&': 
						b.Append("&amp;");
						break;
					}
			}
			
			return b.ToString();
		}
		
		public override void  defineEditText(DefineEditText tag)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			
			if (tag.hasText)
				out_Renamed.Write(" text='" + escape(tag.initialText) + "'");
			
			if (tag.hasFont)
			{
				out_Renamed.Write(" fontId='" + id(tag.font) + "'");
				out_Renamed.Write(" fontName='" + tag.font.FontName + "'");
				out_Renamed.Write(" fontHeight='" + tag.height + "'");
			}
			
			out_Renamed.Write(" bounds='" + tag.bounds + "'");
			
			if (tag.hasTextColor)
				out_Renamed.Write(" color='" + printRGBA(tag.color) + "'");
			
			out_Renamed.Write(" html='" + tag.html + "'");
			out_Renamed.Write(" autoSize='" + tag.autoSize + "'");
			out_Renamed.Write(" border='" + tag.border + "'");
			
			if (tag.hasMaxLength)
				out_Renamed.Write(" maxLength='" + tag.maxLength + "'");
			
			out_Renamed.Write(" multiline='" + tag.multiline + "'");
			out_Renamed.Write(" noSelect='" + tag.noSelect + "'");
			out_Renamed.Write(" password='" + tag.password + "'");
			out_Renamed.Write(" readOnly='" + tag.readOnly + "'");
			out_Renamed.Write(" useOutlines='" + tag.useOutlines + "'");
			out_Renamed.Write(" varName='" + tag.varName + "'");
			out_Renamed.Write(" wordWrap='" + tag.wordWrap + "'");
			
			if (tag.hasLayout)
			{
				out_Renamed.Write(" align='" + tag.align + "'");
				out_Renamed.Write(" indent='" + tag.ident + "'");
				out_Renamed.Write(" leading='" + tag.leading + "'");
				out_Renamed.Write(" leftMargin='" + tag.leftMargin + "'");
				out_Renamed.Write(" rightMargin='" + tag.rightMargin + "'");
			}
			close();
		}
		
		
		public override void  defineSprite(DefineSprite tag)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			end();
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("<!-- sprite framecount=" + tag.framecount + " -->");
			
			tag.tagList.visitTags(this);
			
			close(tag);
		}
		
		public override void  finish()
		{
			--indent_Renamed_Field;
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("</swf>");
		}
		
		public override void  frameLabel(FrameLabel tag)
		{
			open(tag);
			out_Renamed.Write(" label='" + tag.label + "'");
			if (tag.anchor)
				out_Renamed.Write(" anchor='" + "true" + "'");
			close();
		}
		
		public override void  soundStreamHead2(SoundStreamHead tag)
		{
			open(tag);
			out_Renamed.Write(" playbackRate='" + tag.playbackRate + "'");
			out_Renamed.Write(" playbackSize='" + tag.playbackSize + "'");
			out_Renamed.Write(" playbackType='" + tag.playbackType + "'");
			out_Renamed.Write(" compression='" + tag.compression + "'");
			out_Renamed.Write(" streamRate='" + tag.streamRate + "'");
			out_Renamed.Write(" streamSize='" + tag.streamSize + "'");
			out_Renamed.Write(" streamType='" + tag.streamType + "'");
			out_Renamed.Write(" streamSampleCount='" + tag.streamSampleCount + "'");
			
			if (tag.compression == 2)
			{
				out_Renamed.Write(" latencySeek='" + tag.latencySeek + "'");
			}
			close();
		}
		
		public override void  defineScalingGrid(DefineScalingGrid tag)
		{
			open(tag);
			out_Renamed.Write(" idref='" + id(tag.scalingTarget) + "'");
			out_Renamed.Write(" grid='" + tag.rect + "'");
			close();
		}
		
		public override void  defineMorphShape(DefineMorphShape tag)
		{
			defineMorphShape2(tag);
		}
		
		public override void  defineMorphShape2(DefineMorphShape tag)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			out_Renamed.Write(" startBounds='" + tag.startBounds + "'");
			out_Renamed.Write(" endBounds='" + tag.endBounds + "'");
			if (tag.code == flash.swf.TagValues_Fields.stagDefineMorphShape2)
			{
				out_Renamed.Write(" startEdgeBounds='" + tag.startEdgeBounds + "'");
				out_Renamed.Write(" endEdgeBounds='" + tag.endEdgeBounds + "'");
				out_Renamed.Write(" usesNonScalingStrokes='" + tag.usesNonScalingStrokes + "'");
				out_Renamed.Write(" usesScalingStrokes='" + tag.usesScalingStrokes + "'");
			}
			end();
			printMorphLineStyles(tag.lineStyles);
			printMorphFillStyles(tag.fillStyles);
			
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("<start>");
			indent_Renamed_Field++;
			printShape(tag.startEdges, true);
			indent_Renamed_Field--;
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("</start>");
			
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("<end>");
			indent_Renamed_Field++;
			printShape(tag.endEdges, true);
			indent_Renamed_Field--;
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("</end>");
			
			close(tag);
		}
		
		public override void  defineFont2(DefineFont2 tag)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			out_Renamed.Write(" font='" + tag.fontName + "'");
			out_Renamed.Write(" numGlyphs='" + tag.glyphShapeTable.Length + "'");
			out_Renamed.Write(" italic='" + tag.italic + "'");
			out_Renamed.Write(" bold='" + tag.bold + "'");
			out_Renamed.Write(" ansi='" + tag.ansi + "'");
			out_Renamed.Write(" wideOffsets='" + tag.wideOffsets + "'");
			out_Renamed.Write(" wideCodes='" + tag.wideCodes + "'");
			out_Renamed.Write(" shiftJIS='" + tag.shiftJIS + "'");
			out_Renamed.Write(" langCode='" + tag.langCode + "'");
			out_Renamed.Write(" hasLayout='" + tag.hasLayout + "'");
			out_Renamed.Write(" ascent='" + tag.ascent + "'");
			out_Renamed.Write(" descent='" + tag.descent + "'");
			out_Renamed.Write(" leading='" + tag.leading + "'");
			out_Renamed.Write(" kerningCount='" + tag.kerningCount + "'");
			
			out_Renamed.Write(" codepointCount='" + tag.codeTable.Length + "'");
			
			if (tag.hasLayout)
			{
				out_Renamed.Write(" advanceCount='" + tag.advanceTable.Length + "'");
				out_Renamed.Write(" boundsCount='" + tag.boundsTable.Length + "'");
			}
			end();
			
			if (glyphs)
			{
				for (int i = 0; i < tag.kerningCount; i++)
				{
					KerningRecord rec = tag.kerningTable[i];
					indent();
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("<kerningRecord adjustment='" + rec.adjustment + "' code1='" + rec.code1 + "' code2='" + rec.code2 + "' />");
				}
				
				for (int i = 0; i < tag.glyphShapeTable.Length; i++)
				{
					indent();
					out_Renamed.Write("<glyph");
					out_Renamed.Write(" codepoint='" + ((int) tag.codeTable[i]) + (isPrintable(tag.codeTable[i])?("(" + tag.codeTable[i] + ")"):"(?)") + "'");
					if (tag.hasLayout)
					{
						out_Renamed.Write(" advance='" + tag.advanceTable[i] + "'");
						out_Renamed.Write(" bounds='" + tag.boundsTable[i] + "'");
					}
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine(">");
					
					Shape shape = tag.glyphShapeTable[i];
					indent_Renamed_Field++;
					if (tabbedGlyphs)
						printShapeWithTabs(shape);
					else
						printShape(shape, true);
					indent_Renamed_Field--;
					indent();
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("</glyph>");
				}
			}
			
			close(tag);
		}
		
		public override void  defineFont3(DefineFont3 tag)
		{
			defineFont2(tag);
		}
		
		public override void  defineFont4(DefineFont4 tag)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			out_Renamed.Write(" font='" + tag.fontName + "'");
			out_Renamed.Write(" hasFontData='" + tag.hasFontData + "'");
			out_Renamed.Write(" smallText='" + tag.smallText + "'");
			out_Renamed.Write(" italic='" + tag.italic + "'");
			out_Renamed.Write(" bold='" + tag.bold + "'");
			out_Renamed.Write(" langCode='" + tag.langCode + "'");
			end();
			
			if (tag.hasFontData)
			{
				outputBase64(tag.data);
			}
			
			close(tag);
		}
		
		public override void  defineFontAlignZones(DefineFontAlignZones tag)
		{
			open(tag);
			if (tag.name != null)
				out_Renamed.Write(" id='" + id(tag) + "'");
			out_Renamed.Write(" fontID='" + id(tag.font) + "'");
			out_Renamed.Write(" CSMTableHint='" + tag.csmTableHint + "'");
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(">");
			indent_Renamed_Field++;
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("<ZoneTable length='" + tag.zoneTable.Length + "'>");
			indent_Renamed_Field++;
			if (glyphs)
			{
				for (int i = 0; i < tag.zoneTable.Length; i++)
				{
					ZoneRecord record = tag.zoneTable[i];
					indent();
					out_Renamed.Write("<ZoneRecord num='" + record.numZoneData + "' mask='" + record.zoneMask + "'>");
					for (int j = 0; j < record.zoneData.Length; j++)
					{
						out_Renamed.Write(record.zoneData[j] + " ");
					}
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("</ZoneRecord>");
				}
			}
			indent_Renamed_Field--;
			indent();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("</ZoneTable>");
			close(tag);
		}
		
		public override void  csmTextSettings(CSMTextSettings tag)
		{
			open(tag);
			if (tag.name != null)
				out_Renamed.Write(" id='" + id(tag) + "'");
			
			System.String textID = tag.textReference == null?"0":id(tag.textReference);
			out_Renamed.Write(" textID='" + textID + "'");
			out_Renamed.Write(" styleFlagsUseSaffron='" + tag.styleFlagsUseSaffron + "'");
			out_Renamed.Write(" gridFitType='" + tag.gridFitType + "'");
			out_Renamed.Write(" thickness='" + tag.thickness + "'");
			out_Renamed.Write(" sharpness='" + tag.sharpness + "'");
			close();
		}
		
		public override void  defineFontName(DefineFontName tag)
		{
			open(tag);
			if (tag.name != null)
				out_Renamed.Write(" id='" + id(tag) + "'");
			out_Renamed.Write(" fontID='" + id(tag.font) + "'");
			if (tag.fontName != null)
			{
				out_Renamed.Write(" name='" + tag.fontName + "'");
			}
			if (tag.copyright != null)
			{
				out_Renamed.Write(" copyright='" + tag.copyright + "'");
			}
			
			close();
		}
		
		private bool isPrintable(char c)
		{
			int i = c & 0xFFFF;
			if (i < ' ' || i == '<' || i == '&' || i == '\'')
				return false;
			else
				return true;
		}
		
		
		public override void  exportAssets(ExportAssets tag)
		{
			open(tag);
			end();
			
			System.Collections.IEnumerator it = tag.exports.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				DefineTag ref_Renamed = (DefineTag) it.Current;
				indent();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("<Export idref='" + dict.getId(ref_Renamed) + "' name='" + ref_Renamed.name + "' />");
			}
			
			close(tag);
		}
		
		public override void  symbolClass(SymbolClass tag)
		{
			open(tag);
			end();
			
			//UPGRADE_TODO: Method 'java.util.Map.entrySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilMapentrySet'"
			System.Collections.IEnumerator it = new SupportClass.HashSetSupport(tag.class2tag).GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				System.Collections.DictionaryEntry e = (System.Collections.DictionaryEntry) it.Current;
				System.String className = (System.String) e.Key;
				DefineTag ref_Renamed = (DefineTag) e.Value;
				indent();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("<Symbol idref='" + dict.getId(ref_Renamed) + "' className='" + className + "' />");
			}
			
			if (tag.topLevelClass != null)
			{
				indent();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("<Symbol idref='0' className='" + tag.topLevelClass + "' />");
			}
			
			
			close(tag);
		}
		
		public override void  importAssets(ImportAssets tag)
		{
			open(tag);
			out_Renamed.Write(" url='" + tag.url + "'");
			end();
			
			System.Collections.IEnumerator it = tag.importRecords.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				ImportRecord record = (ImportRecord) it.Current;
				indent();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("<Import name='" + record.name + "' id='" + dict.getId(record) + "' />");
			}
			
			close(tag);
		}
		
		public override void  importAssets2(ImportAssets tag)
		{
			// TODO: add support for tag.downloadNow and SHA1...
			importAssets(tag);
		}
		
		public override void  enableDebugger(EnableDebugger tag)
		{
			open(tag);
			out_Renamed.Write(" password='" + tag.password + "'");
			close();
		}
		
		public override void  doInitAction(DoInitAction tag)
		{
			if (tag.sprite != null && tag.sprite.name != null)
			{
				indent();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("<!-- init " + tag.sprite.name + " " + dict.getId(tag.sprite) + " -->");
			}
			
			open(tag);
			if (tag.sprite != null)
				out_Renamed.Write(" idref='" + idRef(tag.sprite) + "'");
			end();
			
			if (showActions)
			{
				openCDATA();
				printActions(tag.actionList);
				closeCDATA();
			}
			else
			{
				indent();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("<!-- " + tag.actionList.size() + " action(s) elided -->");
			}
			close(tag);
		}
		
		private System.String idRef(DefineTag tag)
		{
			if (tag == null)
			{
				// if tag is null then it isn't in the dict -- the SWF is invalid.
				// lets be lax and print something; Matador generates invalid SWF sometimes.
				return "-1";
			}
			else if (tag.name == null)
			{
				// just print the character id since no name was exported
				return System.Convert.ToString(dict.getId(tag));
			}
			else
			{
				return tag.name;
			}
		}
		
		public override void  defineVideoStream(DefineVideoStream tag)
		{
			open(tag);
			out_Renamed.Write(" id='" + id(tag) + "'");
			close();
		}
		
		public override void  videoFrame(VideoFrame tag)
		{
			open(tag);
			out_Renamed.Write(" streamId='" + idRef(tag.stream) + "'");
			out_Renamed.Write(" frame='" + tag.frameNum + "'");
			close();
		}
		
		public override void  defineFontInfo2(DefineFontInfo tag)
		{
			defineFontInfo(tag);
		}
		
		public override void  enableDebugger2(EnableDebugger tag)
		{
			open(tag);
			out_Renamed.Write(" password='" + tag.password + "'");
			out_Renamed.Write(" reserved='0x" + System.Convert.ToString(tag.reserved, 16) + "'");
			close();
		}
		
		public override void  debugID(DebugID tag)
		{
			open(tag);
			out_Renamed.Write(" uuid='" + tag.uuid + "'");
			close();
		}
		
		public override void  scriptLimits(ScriptLimits tag)
		{
			open(tag);
			out_Renamed.Write(" scriptRecursionLimit='" + tag.scriptRecursionLimit + "'" + " scriptTimeLimit='" + tag.scriptTimeLimit + "'");
			close();
		}
		
		public override void  doABC(DoABC tag)
		{
			if (abc)
			{
				open(tag);
				end();
				AbcPrinter abcPrinter = new AbcPrinter(tag.abc, out_Renamed, showOffset, indent_Renamed_Field);
				abcPrinter.print();
				close(tag);
			}
			else if (showActions)
			{
				open(tag);
				if (tag.code == flash.swf.TagValues_Fields.stagDoABC2)
					out_Renamed.Write(" name='" + tag.name + "'");
				end();
				
				ContextStatics contextStatics = new ContextStatics();
				contextStatics.use_static_semantics = true;
				contextStatics.dialect = 9;
				
				//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
				Debug.Assert(swfVersion != null, "header should have been parsed already, but wasn't");
				contextStatics.setAbcVersion(ContextStatics.getTargetAVM(swfVersion));
				contextStatics.use_namespaces.addAll(ContextStatics.getRequiredUseNamespaces(swfVersion));
				
				Context context = new Context(contextStatics);
				context.setHandler(new CompilerHandler());
				AbcParser abcParser = new AbcParser(context, tag.abc);
				context.setEmitter(new ActionBlockEmitter(context, tag.name, new StringPrintWriter(), new StringPrintWriter(), false, false, false, false));
				ProgramNode programNode = abcParser.parseAbc();
				
				if (programNode == null)
				{
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("<!-- Error: could not parse abc -->");
				}
				else if (decompile)
				{
					//                PrettyPrinter prettyPrinter = new PrettyPrinter(out);
					//                programNode.evaluate(context, prettyPrinter);
				}
				else
				{
					SyntaxTreeDumper syntaxTreeDumper = new SyntaxTreeDumper(out_Renamed, indent_Renamed_Field);
					programNode.evaluate(context, syntaxTreeDumper);
				}
				
				close(tag);
			}
			else
			{
				open(tag);
				close();
			}
		}
		
		private System.String hexify(sbyte[] id)
		{
			System.Text.StringBuilder b = new System.Text.StringBuilder(id.Length * 2);
			for (int i = 0; i < id.Length; i++)
			{
				//UPGRADE_ISSUE: Method 'java.lang.Character.forDigit' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangCharacterforDigit_int_int'"
				b.Append(Character.forDigit((id[i] >> 4) & 15, 16));
				//UPGRADE_ISSUE: Method 'java.lang.Character.forDigit' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangCharacterforDigit_int_int'"
				b.Append(Character.forDigit(id[i] & 15, 16));
			}
			return b.ToString().ToUpper();
		}
		
		public static System.String baseName(System.String path)
		{
			int start = path.LastIndexOf((System.Char) System.IO.Path.DirectorySeparatorChar);
			
			if (System.IO.Path.DirectorySeparatorChar != '/')
			{
				// some of us are grouchy about unix paths not being
				// parsed since they are totally legit at the system
				// level of win32.
				int altstart = path.LastIndexOf('/');
				if ((start == - 1) || (altstart > start))
					start = altstart;
			}
			
			if (start == - 1)
				start = 0;
			else
				++start;
			
			
			int end = path.LastIndexOf('.');
			
			if (end == - 1)
				end = path.Length;
			
			
			if (start > end)
				end = path.Length;
			
			return path.Substring(start, (end) - (start));
		}
		
		public static System.String dirName(System.String path)
		{
			int end = path.LastIndexOf((System.Char) System.IO.Path.PathSeparator);
			
			
			if (System.IO.Path.PathSeparator != '/')
			{
				// some of us are grouchy about unix paths not being
				// parsed since they are totally legit at the system
				// level of win32.
				int altend = path.LastIndexOf('/');
				if ((end == - 1) || (altend < end))
					end = altend;
			}
			
			if (end == - 1)
				return "";
			else
				++end;
			
			return path.Substring(0, (end) - (0));
		}
		
		// options
		internal static bool abcOption = false;
		internal static bool encodeOption = false;
		internal static bool showActionsOption = true;
		internal static bool showOffsetOption = false;
		internal static bool showDebugSourceOption = false;
		internal static bool glyphsOption = true;
		internal static bool externalOption = false;
		internal static bool decompileOption = true;
		internal static bool defuncOption = true;
		internal static bool saveOption = false;
		internal static bool tabbedGlyphsOption = true;
		
		
		/// <summary> swfdump usage:  swfdump [-encode] [-noactions] [-showoffset] files ...
		/// -encode       ?
		/// -noactions    don't output ActionScript byte code
		/// -showoffset   output an XML comment line in the output before each
		/// tag, displaying the tag's byte offset and size in the file
		/// <p/>
		/// Swfdump will dump a SWF file as XML.  Swf tags are shown as XML tags.  Swf Actions are shown
		/// commented out assembly language.  If a SWD file is found that matches this SWF file, then
		/// we will show intermixed source code and assembly language.
		/// <p/>
		/// The format of the output (swfx) is according to the SWFX doctype.  The optional -dtd flag will
		/// include the doctype declaration before the actual content.  This format can be edited in any
		/// text or xml editor, and then converted back into SWF using the Swfxc utility.
		/// </summary>
		[STAThread]
		public static void  Main(System.String[] args)
		{
			if (args.Length == 0)
			{
				System.Console.Error.WriteLine("Usage: java tools.SwfxPrinter [-encode] [-asm] [-abc] [-noactions] [-showdebugsource] [-showoffset] [-noglyphs] [-external] [-save file.swf] [-nofunctions] [-out file.swfx] file1.swf ...");
				System.Environment.Exit(1);
			}
			
			int index = 0;
			System.IO.StreamWriter out_Renamed = null;
			System.String outfile = null;
			
			while ((index < args.Length) && (args[index].StartsWith("-")))
			{
				if (args[index].Equals("-encode"))
				{
					encodeOption = true;
					++index;
				}
				else if (args[index].Equals("-save"))
				{
					++index;
					saveOption = true;
					outfile = args[index++];
				}
				else if (args[index].Equals("-decompile"))
				{
					decompileOption = true;
					++index;
				}
				else if (args[index].Equals("-nofunctions"))
				{
					defuncOption = false;
					++index;
				}
				else if (args[index].Equals("-asm"))
				{
					decompileOption = false;
					++index;
				}
				else if (args[index].Equals("-abc"))
				{
					abcOption = true;
					++index;
				}
				else if (args[index].Equals("-noactions"))
				{
					showActionsOption = false;
					++index;
				}
				else if (args[index].Equals("-showoffset"))
				{
					showOffsetOption = true;
					++index;
				}
				else if (args[index].Equals("-showdebugsource"))
				{
					showDebugSourceOption = true;
					++index;
				}
				else if (args[index].Equals("-noglyphs"))
				{
					glyphsOption = false;
					++index;
				}
				else if (args[index].Equals("-out"))
				{
					if (index + 1 == args.Length)
					{
						System.Console.Error.WriteLine("-out requires a filename or - for stdout");
						System.Environment.Exit(1);
					}
					if (!args[index + 1].Equals("-"))
					{
						
						outfile = args[index + 1];
						//UPGRADE_TODO: Constructor 'java.io.FileOutputStream.FileOutputStream' was converted to 'System.IO.FileStream.FileStream' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileOutputStreamFileOutputStream_javalangString_boolean'"
						out_Renamed = new System.IO.StreamWriter(SupportClass.GetFileStream(outfile, false), System.Text.Encoding.Default);
					}
					index += 2;
				}
				else if (args[index].Equals("-external"))
				{
					externalOption = true;
					++index;
				}
				else if (args[index].ToUpper().Equals("-tabbedGlyphs".ToUpper()))
				{
					tabbedGlyphsOption = true;
					++index;
				}
				else
				{
					System.Console.Error.WriteLine("unknown argument " + args[index]);
					++index;
				}
			}
			
			if (out_Renamed == null)
			{
				System.IO.StreamWriter temp_writer;
				temp_writer = new System.IO.StreamWriter(System.Console.OpenStandardOutput(), System.Text.Encoding.Default);
				temp_writer.AutoFlush = true;
				out_Renamed = temp_writer;
			}
			
			System.IO.FileInfo f = new System.IO.FileInfo(args[index]);
			System.Uri[] urls;
			bool tmpBool;
			if (System.IO.File.Exists(f.FullName))
				tmpBool = true;
			else
				tmpBool = System.IO.Directory.Exists(f.FullName);
			if (!tmpBool)
			{
				//UPGRADE_TODO: Class 'java.net.URL' was converted to a 'System.Uri' which does not throw an exception if a URL specifies an unknown protocol. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1132'"
				urls = new System.Uri[]{new System.Uri(args[index])};
			}
			else
			{
				if (System.IO.Directory.Exists(f.FullName))
				{
					System.IO.FileInfo[] list = FileUtils.listFiles(f);
					urls = new System.Uri[list.Length];
					for (int i = 0; i < list.Length; i++)
					{
						urls[i] = FileUtils.toURL(list[i]);
					}
				}
				else
				{
					urls = new System.Uri[]{FileUtils.toURL(f)};
				}
			}
			
			for (int i = 0; i < urls.Length; i++)
			{
				try
				{
					System.Uri url = urls[i];
					if (saveOption)
					{
						System.IO.Stream in_Renamed = new System.IO.BufferedStream(System.Net.WebRequest.Create(url).GetResponse().GetResponseStream());
						try
						{
							//UPGRADE_TODO: Constructor 'java.io.FileOutputStream.FileOutputStream' was converted to 'System.IO.FileStream.FileStream' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileOutputStreamFileOutputStream_javalangString'"
							System.IO.Stream fileOut = new System.IO.BufferedStream(new System.IO.FileStream(outfile, System.IO.FileMode.Create));
							try
							{
								int c;
								while ((c = in_Renamed.ReadByte()) != - 1)
								{
									fileOut.WriteByte((System.Byte) c);
								}
							}
							finally
							{
								fileOut.Close();
							}
						}
						finally
						{
							in_Renamed.Close();
						}
					}
					
					if (isSwf(url))
					{
						dumpSwf(out_Renamed, url, outfile);
					}
					else if (isZip(url) && !url.ToString().EndsWith(".abj"))
					{
						dumpZip(out_Renamed, url, outfile);
					}
					else
					{
						//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
						out_Renamed.WriteLine("<!-- Parsing actions from " + url + " -->");
						// we have no way of knowing the swf version, so assume latest
						System.Net.HttpWebRequest connection = (System.Net.HttpWebRequest) System.Net.WebRequest.Create(url);
						ActionDecoder actionDecoder = new ActionDecoder(new SwfDecoder(connection.GetResponse().GetResponseStream(), 7));
						actionDecoder.KeepOffsets = true;
						int ContentLength;
						try
						{
							ContentLength = System.Int32.Parse(connection.GetResponse().Headers.Get("Content-Length"));
						}
						catch (System.IO.IOException e)
						{
							ContentLength = -1;
						}
						ActionList actions = actionDecoder.decode(ContentLength);
						SwfxPrinter printer = new SwfxPrinter(out_Renamed);
						printer.decompile = decompileOption;
						printer.defunc = defuncOption;
						printer.printActions(actions);
					}
					out_Renamed.Flush();
				}
				catch (System.ApplicationException e)
				{
					if (Trace.error)
						SupportClass.WriteStackTrace(e, Console.Error);
					
					System.Console.Error.WriteLine("");
					System.Console.Error.WriteLine("An unrecoverable error occurred.  The given file " + urls[i] + " may not be");
					System.Console.Error.WriteLine("a valid swf.");
				}
				catch (System.IO.FileNotFoundException e)
				{
					//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
					System.Console.Error.WriteLine("Error: " + e.Message);
					System.Environment.Exit(1);
				}
			}
		}
		
		private static void  dumpZip(System.IO.StreamWriter out_Renamed, System.Uri url, System.String outfile)
		{
			System.IO.Stream in_Renamed = new System.IO.BufferedStream(System.Net.WebRequest.Create(url).GetResponse().GetResponseStream());
			try
			{
				//UPGRADE_ISSUE: Class 'java.util.zip.ZipInputStream' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilzipZipInputStream'"
				//UPGRADE_ISSUE: Constructor 'java.util.zip.ZipInputStream.ZipInputStream' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilzipZipInputStream'"
				ZipInputStream zipIn = new ZipInputStream(in_Renamed);
				//UPGRADE_ISSUE: Class 'java.util.zip.ZipEntry' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilzipZipEntry'"
				//UPGRADE_ISSUE: Method 'java.util.zip.ZipInputStream.getNextEntry' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilzipZipInputStream'"
				ZipEntry zipEntry = zipIn.getNextEntry();
				while ((zipEntry != null))
				{
					//UPGRADE_TODO: Class 'java.net.URL' was converted to a 'System.Uri' which does not throw an exception if a URL specifies an unknown protocol. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1132'"
					//UPGRADE_ISSUE: Method 'java.util.zip.ZipEntry.getName' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilzipZipEntry'"
					System.Uri fileUrl = new System.Uri("jar:" + url.ToString() + "!/" + zipEntry.getName());
					if (isSwf(fileUrl))
						dumpSwf(out_Renamed, fileUrl, outfile);
					//UPGRADE_ISSUE: Method 'java.util.zip.ZipInputStream.getNextEntry' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilzipZipInputStream'"
					zipEntry = zipIn.getNextEntry();
				}
			}
			finally
			{
				in_Renamed.Close();
			}
		}
		
		private static void  dumpSwf(System.IO.StreamWriter out_Renamed, System.Uri url, System.String outfile)
		{
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine("<!-- Parsing swf " + url + " -->");
			System.IO.Stream in_Renamed;
			SwfxPrinter debugPrinter = new SwfxPrinter(out_Renamed);
			
			debugPrinter.showActions = showActionsOption;
			debugPrinter.showOffset = showOffsetOption;
			debugPrinter.showDebugSource = showDebugSourceOption;
			debugPrinter.glyphs = glyphsOption;
			debugPrinter.setExternal(externalOption, outfile);
			debugPrinter.decompile = decompileOption;
			debugPrinter.abc = abcOption;
			debugPrinter.defunc = defuncOption;
			debugPrinter.tabbedGlyphs = tabbedGlyphsOption;
			
			if (encodeOption)
			{
				// decode -> encode -> decode -> print
				TagEncoder encoder = new TagEncoder();
				in_Renamed = System.Net.WebRequest.Create(url).GetResponse().GetResponseStream();
				new TagDecoder(in_Renamed, url).parse(encoder);
				encoder.finish();
				in_Renamed = new System.IO.MemoryStream(SupportClass.ToByteArray(encoder.toByteArray()));
			}
			else
			{
				// decode -> print
				in_Renamed = System.Net.WebRequest.Create(url).GetResponse().GetResponseStream();
			}
			TagDecoder t = new TagDecoder(in_Renamed, url);
			t.KeepOffsets = debugPrinter.showOffset;
			t.parse(debugPrinter);
		}
		
		private static bool isSwf(System.Uri url)
		{
			System.IO.Stream in_Renamed = new System.IO.BufferedStream(System.Net.WebRequest.Create(url).GetResponse().GetResponseStream());
			try
			{
				return isSwf(in_Renamed);
			}
			finally
			{
				in_Renamed.Close();
			}
		}
		
		public static bool isSwf(System.IO.Stream in_Renamed)
		{
			try
			{
				//UPGRADE_TODO: Class 'java.io.DataInputStream' was converted to 'System.IO.BinaryReader' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioDataInputStream'"
				System.IO.BinaryReader data = new System.IO.BinaryReader(in_Renamed);
				sbyte[] b = new sbyte[3];
				//UPGRADE_ISSUE: Method 'java.io.FilterInputStream.mark' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaioFilterInputStreammark_int'"
				data.mark(b.Length);
				SupportClass.ReadInput(data.BaseStream, b, 0, b.Length);
				if (b[0] == 'C' && b[1] == 'W' && b[2] == 'S' || b[0] == 'F' && b[1] == 'W' && b[2] == 'S')
				{
					//UPGRADE_ISSUE: Method 'java.io.FilterInputStream.reset' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaioFilterInputStreamreset'"
					data.reset();
					return true;
				}
				else
				{
					//UPGRADE_ISSUE: Method 'java.io.FilterInputStream.reset' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaioFilterInputStreamreset'"
					data.reset();
					return false;
				}
			}
			catch (System.IO.IOException e)
			{
				return false;
			}
		}
		
		private static bool isZip(System.Uri url)
		{
			System.IO.Stream in_Renamed = new System.IO.BufferedStream(System.Net.WebRequest.Create(url).GetResponse().GetResponseStream());
			try
			{
				return isZip(in_Renamed);
			}
			finally
			{
				in_Renamed.Close();
			}
		}
		
		public static bool isZip(System.IO.Stream in_Renamed)
		{
			try
			{
				//UPGRADE_ISSUE: Class 'java.util.zip.ZipInputStream' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilzipZipInputStream'"
				//UPGRADE_ISSUE: Constructor 'java.util.zip.ZipInputStream.ZipInputStream' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilzipZipInputStream'"
				ZipInputStream swcZipInputStream = new ZipInputStream(in_Renamed);
				//UPGRADE_ISSUE: Method 'java.util.zip.ZipInputStream.getNextEntry' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilzipZipInputStream'"
				swcZipInputStream.getNextEntry();
				return true;
			}
			catch (System.IO.IOException e)
			{
				return false;
			}
		}
		
		// Handy dandy for dumping an action list during debugging
		public static System.String actionListToString(ActionList al, System.String[] args)
		{
			// cut and paste arg code from main() could be better but it works
			bool showActions = true;
			bool showOffset = false;
			bool showDebugSource = false;
			bool decompile = false;
			bool defunc = true;
			bool tabbedGlyphs = true;
			int index = 0;
			
			while (args != null && (index < args.Length) && (args[index].StartsWith("-")))
			{
				if (args[index].Equals("-decompile"))
				{
					decompile = true;
					++index;
				}
				else if (args[index].Equals("-nofunctions"))
				{
					defunc = false;
					++index;
				}
				else if (args[index].Equals("-asm"))
				{
					decompile = false;
					++index;
				}
				else if (args[index].Equals("-noactions"))
				{
					showActions = false;
					++index;
				}
				else if (args[index].Equals("-showoffset"))
				{
					showOffset = true;
					++index;
				}
				else if (args[index].Equals("-showdebugsource"))
				{
					showDebugSource = true;
					++index;
				}
				else if (args[index].ToUpper().Equals("-tabbedGlyphs".ToUpper()))
				{
					tabbedGlyphs = true;
					++index;
				}
			}
			
			System.IO.StringWriter sw = new System.IO.StringWriter();
			//UPGRADE_ISSUE: Constructor 'java.io.PrintWriter.PrintWriter' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaioPrintWriterPrintWriter_javaioWriter'"
			System.IO.StreamWriter out_Renamed = new PrintWriter(sw);
			SwfxPrinter printer = new SwfxPrinter(out_Renamed);
			printer.showActions = showActions;
			printer.showOffset = showOffset;
			printer.showDebugSource = showDebugSource;
			printer.decompile = decompile;
			printer.defunc = defunc;
			printer.tabbedGlyphs = tabbedGlyphs;
			
			printer.printActions(al);
			out_Renamed.Flush();
			return sw.ToString();
		}
		static SwfxPrinter()
		{
			{
				TypeValue.init();
				ObjectValue.init();
			}
		}
	}
}