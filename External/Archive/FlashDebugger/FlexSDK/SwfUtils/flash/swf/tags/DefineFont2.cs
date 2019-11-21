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
	
	/// <summary> DefineFont2 improves on the functionality of the DefineFont tag. Enhancements
	/// include:
	/// <ul>
	/// <li>32-bit entries in the offset table for fonts with more than 65535
	/// glyphs.</li>
	/// <li>Mapping to device fonts by incorporating all of the functionality of
	/// DefineFontInfo.</li>
	/// <li>Font metrics for improved layout of dynamic glyph text.</li>
	/// </ul>
	/// Note that DefineFont2 reserves space for a font bounds table and kerning
	/// table. This information is not used through Flash Player 7, though some
	/// minimal values must be present for these entries to define a well formed tag.
	/// A minimal Rect can be supplied for the font bounds table and the kerning
	/// count can be set to 0 to omit the kerning table. DefineFont2 was introduced
	/// in SWF version 3.
	/// 
	/// </summary>
	/// <author>  Clement Wong
	/// </author>
	/// <author>  Peter Farland
	/// </author>
	public class DefineFont2:DefineFont
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
		public DefineFont2():this(Flash.Swf.TagValues.stagDefineFont2)
		{
		}
		
		protected internal DefineFont2(int code):base(code)
		{
		}
		
		//--------------------------------------------------------------------------
		//
		// Fields and Bean Properties
		//
		//--------------------------------------------------------------------------
		
		public bool smallText;
		public bool hasLayout;
		public bool shiftJIS;
		public bool ansi;
		public bool wideOffsets;
		public bool wideCodes;
		public bool italic;
		public bool bold;
		public int langCode;
		public String fontName;
		
		// U16 if wideOffsets == true, U8 otherwise
		public char[] codeTable;
		public int ascent;
		public int descent;
		public int leading;
		
		public Shape[] glyphShapeTable;
		public short[] advanceTable;
		public Rect[] boundsTable;
		public int kerningCount;
		public KerningRecord[] kerningTable;
		
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
			if (code == Flash.Swf.TagValues.stagDefineFont2)
				handler.defineFont2(this);
		}
		
		//--------------------------------------------------------------------------
		//
		// Utility Methods
		//
		//--------------------------------------------------------------------------
		
		/// <summary> Tests whether this DefineFont2 tag is equivalent to another DefineFont2
		/// tag instance.
		/// 
		/// </summary>
		/// <param name="obj">Another DefineFont2 instance to test for equality.
		/// </param>
		/// <returns> true if the given instance is considered equal to this instance
		/// </returns>
		public  override bool Equals(System.Object obj)
		{
			bool isEqual = false;
			
			if (obj is DefineFont2 && base.Equals(obj))
			{
				DefineFont2 defineFont = (DefineFont2) obj;
				
				// wideOffsets and wideCodes not considered in the equality check
				// as these are determined at encoding time
				
				if ((defineFont.hasLayout == this.hasLayout) && (defineFont.shiftJIS == this.shiftJIS) && (defineFont.ansi == this.ansi) && (defineFont.italic == this.italic) && (defineFont.bold == this.bold) && (defineFont.langCode == this.langCode) && (defineFont.ascent == this.ascent) && (defineFont.descent == this.descent) && (defineFont.leading == this.leading) && (defineFont.kerningCount == this.kerningCount) && equals(defineFont.name, this.name) && equals(defineFont.fontName, this.fontName) && SupportClass.ArraySupport.Equals(defineFont.glyphShapeTable, this.glyphShapeTable) && SupportClass.ArraySupport.Equals(defineFont.codeTable, this.codeTable) && SupportClass.ArraySupport.Equals(defineFont.advanceTable, this.advanceTable) && SupportClass.ArraySupport.Equals(defineFont.boundsTable, this.boundsTable) && SupportClass.ArraySupport.Equals(defineFont.kerningTable, this.kerningTable))
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
