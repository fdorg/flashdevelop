// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2008 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using Flash.Swf;

namespace Flash.Swf.Tags
{
	
	/// <author>  Peter Farland
	/// </author>
	public class DefineFont4:DefineFont
	{
		/// <summary> The name of the font. This name is significant for embedded fonts at
		/// runtime as it determines how one refers to the font in CSS. In SWF 6 and
		/// later, font names are encoded using UTF-8.
		/// </summary>
		override public String FontName
		{
			get
			{
				return fontName;
			}
			
		}
		/// <summary> Reports whether the font face is bold.</summary>
		override public bool Bold
		{
			get
			{
				return bold;
			}
			
		}
		/// <summary> Reports whether the font face is italic.</summary>
		override public bool Italic
		{
			get
			{
				return italic;
			}
			
		}
		/// <summary> Constructor.</summary>
		public DefineFont4():base(Flash.Swf.TagValues.stagDefineFont4)
		{
		}
		
		//--------------------------------------------------------------------------
		//
		// Fields and Bean Properties
		//
		//--------------------------------------------------------------------------
		
		public bool hasFontData;
		public bool smallText;
		public bool italic;
		public bool bold;
		public int langCode;
		public String fontName;
		public byte[] data;
		
		//--------------------------------------------------------------------------
		//
		// Visitor Methods
		//
		//--------------------------------------------------------------------------
		
		public override void  visit(TagHandler handler)
		{
			if (code == Flash.Swf.TagValues.stagDefineFont4)
				handler.defineFont4(this);
		}
		
		//--------------------------------------------------------------------------
		//
		// Utility Methods
		//
		//--------------------------------------------------------------------------
		
		/// <summary> Tests whether this DefineFont4 tag is equivalent to another DefineFont4
		/// tag instance.
		/// 
		/// </summary>
		/// <param name="obj">Another DefineFont4 instance to test for equality.
		/// </param>
		/// <returns> true if the given instance is considered equal to this instance
		/// </returns>
		public  override bool Equals(System.Object obj)
		{
			bool isEqual = false;
			
			if (obj is DefineFont4 && base.Equals(obj))
			{
				DefineFont4 defineFont = (DefineFont4) obj;
				
				if ((defineFont.hasFontData == this.hasFontData) && (defineFont.italic == this.italic) && (defineFont.bold == this.bold) && (defineFont.langCode == this.langCode) && (defineFont.smallText == this.smallText) && equals(defineFont.fontName, this.fontName) && SupportClass.ArraySupport.Equals(defineFont.data, this.data))
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
