// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
// ADOBE SYSTEMS INCORPORATED
// Copyright 2003-2006 Adobe Systems Incorporated
// All Rights Reserved.
//
// NOTICE: Adobe permits you to use, modify, and distribute this file
// in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using Flash.Swf;
using Flash.Swf.Types;

namespace Flash.Swf.Tags
{
	
	/// <summary> A DefineFont tag defines the shape outlines of each glyph used in a
	/// particular font. Only the glyphs that are used by subsequent DefineText tags
	/// should be included. DefineFont tags cannot be used for dynamic text. Dynamic
	/// text requires the DefineFont2 tag. DefineFont was introduced in SWF version
	/// 1.
	/// 
	/// </summary>
	/// <seealso cref="DefineFontInfo">
	/// </seealso>
	/// <author>  Clement Wong
	/// </author>
	/// <author>  Peter Farland
	/// </author>
	public class DefineFont1:DefineFont
	{
		/// <summary> The name of the font. This name is significant for embedded fonts at
		/// runtime as it determines how one refers to the font in CSS. In SWF 6 and
		/// later, font names are encoded using UTF-8. In SWF 5 and earlier, font
		/// names are encoded in a platform specific manner in the codepage of the
		/// system they were authored.
		/// </summary>
		override public String FontName
		{
			get
			{
				if (fontInfo != null)
					return fontInfo.name;
				
				return null;
			}
			
		}
		/// <summary> Reports whether the font face is bold.</summary>
		override public bool Bold
		{
			get
			{
				if (fontInfo != null)
					return fontInfo.bold;
				
				return false;
			}
			
		}
		/// <summary> Reports whether the font face is italic.</summary>
		override public bool Italic
		{
			get
			{
				if (fontInfo != null)
					return fontInfo.italic;
				
				return false;
			}
			
		}
		/// <summary> Find the immediate, first order dependencies.
		/// 
		/// </summary>
		/// <returns> Iterator of immediate references of this DefineFont.
		/// </returns>
		override public System.Collections.IEnumerator References
		{
			get
			{
				System.Collections.IList refs = new System.Collections.ArrayList();
				
				for (int i = 0; i < glyphShapeTable.Length; i++)
					glyphShapeTable[i].getReferenceList(refs);
				
				return refs.GetEnumerator();
			}
			
		}
		/// <summary> Constructor.</summary>
		public DefineFont1():base(Flash.Swf.TagValues.stagDefineFont)
		{
		}
		
		//--------------------------------------------------------------------------
		//
		// Fields and Bean Properties
		//
		//--------------------------------------------------------------------------
		
		/// <summary> An Array of Shapes representing glyph outlines.</summary>
		public Shape[] glyphShapeTable;
		
		/// <summary> The DefineFontInfo associated with this DefineFont tag. DefineFontInfo
		/// provides a mapping from this glyph font (i.e. DefineFont) to a device
		/// font, as well as the font name and style information.
		/// </summary>
		public DefineFontInfo fontInfo;
		
		//--------------------------------------------------------------------------
		//
		// Visitor Methods
		//
		//--------------------------------------------------------------------------
		
		/// <summary> Invokes the defineFont visitor on the given TagHandler.
		/// 
		/// </summary>
		/// <param name="handler">The SWF TagHandler.
		/// </param>
		public override void  visit(TagHandler handler)
		{
			if (code == Flash.Swf.TagValues.stagDefineFont)
				handler.defineFont(this);
		}
		
		//--------------------------------------------------------------------------
		//
		// Utility Methods
		//
		//--------------------------------------------------------------------------
		
		/// <summary> Tests whether this DefineFont tag is equivalent to another DefineFont tag
		/// instance.
		/// 
		/// </summary>
		/// <param name="obj">Another DefineFont (version 1) instance to test for
		/// equality.
		/// </param>
		/// <returns> true if the given instance is considered equal to this instance
		/// </returns>
		public  override bool Equals(System.Object obj)
		{
			bool isEqual = false;
			
			if (obj is DefineFont1 && base.Equals(obj))
			{
				DefineFont1 defineFont = (DefineFont1) obj;
				
				if (equals(defineFont.fontInfo, this.fontInfo) && SupportClass.ArraySupport.Equals(defineFont.glyphShapeTable, this.glyphShapeTable))
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
