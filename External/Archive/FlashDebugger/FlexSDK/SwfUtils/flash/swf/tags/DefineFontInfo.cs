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
using Tag = Flash.Swf.Tag;
using ArrayUtil = Flash.Util.ArrayUtil;
namespace Flash.Swf.Tags
{
	
	/// <since> SWF1
	/// </since>
	/// <author>  Clement Wong
	/// </author>
	public class DefineFontInfo:Tag
	{
		public DefineFontInfo(int code):base(code)
		{
		}
		
		public override void  visit(Flash.Swf.TagHandler h)
		{
			if (code == Flash.Swf.TagValues.stagDefineFontInfo)
				h.defineFontInfo(this);
			else
				h.defineFontInfo2(this);
		}
		
		public override Tag SimpleReference
		{
            get
            {
                return font;
            }
		}
		
		public DefineFont1 font;
		public String name;
		public bool shiftJIS;
		public bool ansi;
		public bool italic;
		public bool bold;
		public bool wideCodes; // not in equality check- sometimes determined from other vars at encoding time
		
		/// <summary>U16 if widecodes == true, U8 otherwise.  provides the character
		/// values for each glyph in the font. 
		/// </summary>
		public char[] codeTable;
		
		/// <summary>langcode - valid for DefineFont2 only </summary>
		public int langCode;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineFontInfo))
			{
				DefineFontInfo defineFontInfo = (DefineFontInfo) object_Renamed;
				
				// [paul] Checking that the font fields are equal would
				// lead to an infinite loop, because DefineFont contains a
				// reference to it's DefineFontInfo.
				if (equals(defineFontInfo.name, this.name) && (defineFontInfo.shiftJIS == this.shiftJIS) && (defineFontInfo.ansi == this.ansi) && (defineFontInfo.italic == this.italic) && (defineFontInfo.bold == this.bold) && ArrayUtil.equals(defineFontInfo.codeTable, this.codeTable) && (defineFontInfo.langCode == this.langCode))
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
