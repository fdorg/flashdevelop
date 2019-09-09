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
using DefineEditText = flash.swf.tags.DefineEditText;
using DefineTag = flash.swf.tags.DefineTag;
using Rect = flash.swf.types.Rect;
using SwfConstants = flash.swf.SwfConstants;
using SwfUtils = flash.swf.SwfUtils;
namespace flash.swf.builder.tags
{
	
	/// <author>  Peter Farland
	/// </author>
	public class EditTextBuilder : TagBuilder
	{
		virtual public bool AutoSize
		{
			set
			{
				tag.autoSize = value;
			}
			
		}
		virtual public bool Border
		{
			set
			{
				tag.border = value;
			}
			
		}
		virtual public System.String VarName
		{
			set
			{
				tag.varName = value;
			}
			
		}
		virtual public int MaxLength
		{
			set
			{
				tag.hasMaxLength = true;
				tag.maxLength = value;
			}
			
		}
		virtual public bool Html
		{
			set
			{
				tag.html = value;
			}
			
		}
		virtual public System.Drawing.Color Color
		{
			set
			{
				if (!value.IsEmpty)
				{
					tag.hasTextColor = true;
					//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
					tag.color = SwfUtils.colorToInt(ref value);
				}
			}
			
		}
		virtual public Rect Bounds
		{
			set
			{
				tag.bounds = value;
			}
			
		}
		virtual public System.String InitialText
		{
			set
			{
				if (value != null)
				{
					tag.hasText = true;
					tag.initialText = value;
					
					if (tag.bounds == null)
					{
						/*
						Font font = fontBuilder.getFont().deriveFont((float)tag.height/20);
						TextLayout layout = new TextLayout(text, font, fontBuilder.getFontRenderContext());
						Rectangle2D bs = layout.getBounds();
						tag.bounds = new Rect(SwfUtils.toTwips(bs.getMinX()),
						SwfUtils.toTwips(bs.getMaxX()),
						SwfUtils.toTwips(bs.getMinY()),
						SwfUtils.toTwips(bs.getMaxY()));*/
						
						
						//TODO: need to find a better way to set the text bounds 
						tag.bounds.yMax = tag.bounds.yMax * 3; //The logical bounds includes padding above and below of the actual bounding box
						tag.bounds.xMax = tag.bounds.xMax * 2;
					}
				}
			}
			
		}
		public EditTextBuilder(FontBuilder builder, float height, bool useOutlines, bool readOnly, bool noSelect)
		{
			tag = new DefineEditText();
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			tag.height = (int) System.Math.Round((double) (height * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL));
			
			if (builder != null)
			{
				fontBuilder = builder;
				tag.hasFont = true;
			}
			
			tag.useOutlines = useOutlines;
			tag.readOnly = readOnly;
			tag.noSelect = noSelect;
		}
		
		public virtual DefineTag build()
		{
			tag.font = fontBuilder.tag;
			if (tag.varName == null)
				tag.varName = "";
			
			if (tag.initialText == null)
				tag.initialText = "";
			
			if (tag.bounds == null)
				tag.bounds = new Rect();
			
			return tag;
		}
		
		public virtual void  setLayout(int align, int leftMargin, int rightMargin, int indent, int leading)
		{
			tag.hasLayout = true;
			if (align < 0 || align > 3)
				throw new System.SystemException("Invalid alignment.");
			tag.align = align;
			tag.leftMargin = leftMargin;
			tag.rightMargin = rightMargin;
			tag.ident = indent;
			tag.leading = leading;
		}
		
		private DefineEditText tag;
		private FontBuilder fontBuilder;
	}
}