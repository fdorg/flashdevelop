// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
using TagHandler = Flash.Swf.TagHandler;
using Rect = Flash.Swf.Types.Rect;
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class DefineEditText:DefineTag
	{
		override public System.Collections.IEnumerator References
		{
			get
			{
				System.Collections.ArrayList refs = new System.Collections.ArrayList();
				if (font != null)
					refs.Add(font);
				
				return refs.GetEnumerator();
			}
			
		}
		public DefineEditText():base(Flash.Swf.TagValues.stagDefineEditText)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			h.defineEditText(this);
		}
		
		public Rect bounds;
		public bool hasText;
		public bool wordWrap;
		public bool multiline;
		public bool password;
		public bool readOnly;
		public bool hasTextColor;
		public bool hasMaxLength;
		public bool hasFont;
		public bool autoSize;
		public bool hasLayout;
		public bool noSelect;
		public bool border;
		public bool wasStatic;
		public bool html;
		public bool useOutlines;
		
		public DefineFont font;
		public int height;
		/// <summary>color as int: 0xAARRGGBB </summary>
		public int color;
		public int maxLength;
		public int align;
		public int leftMargin;
		public int rightMargin;
		public int ident;
		public int leading;
		public String varName;
		public String initialText;
		public CSMTextSettings csmTextSettings;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineEditText))
			{
				DefineEditText defineEditText = (DefineEditText) object_Renamed;
				
				if (equals(defineEditText.bounds, this.bounds) && (defineEditText.hasText == this.hasText) && (defineEditText.wordWrap == this.wordWrap) && (defineEditText.multiline == this.multiline) && (defineEditText.password == this.password) && (defineEditText.readOnly == this.readOnly) && (defineEditText.hasTextColor == this.hasTextColor) && (defineEditText.hasMaxLength == this.hasMaxLength) && (defineEditText.hasFont == this.hasFont) && (defineEditText.autoSize == this.autoSize) && (defineEditText.hasLayout == this.hasLayout) && (defineEditText.noSelect == this.noSelect) && (defineEditText.border == this.border) && (defineEditText.wasStatic == this.wasStatic) && (defineEditText.html == this.html) && (defineEditText.useOutlines == this.useOutlines) && equals(defineEditText.font, this.font) && (defineEditText.height == this.height) && (defineEditText.color == this.color) && (defineEditText.maxLength == this.maxLength) && (defineEditText.align == this.align) && (defineEditText.leftMargin == this.leftMargin) && (defineEditText.rightMargin == this.rightMargin) && (defineEditText.ident == this.ident) && (defineEditText.leading == this.leading) && equals(defineEditText.varName, this.varName) && equals(defineEditText.initialText, this.initialText))
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
