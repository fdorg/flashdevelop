// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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
using FontFace = flash.fonts.FontFace;
using FontManager = flash.fonts.FontManager;
using FSType = flash.fonts.FSType;
using Tag = flash.swf.Tag;
using TagValues = flash.swf.TagValues;
using ZoneRecordBuilder = flash.swf.builder.types.ZoneRecordBuilder;
using DefineFont2 = flash.swf.tags.DefineFont2;
using DefineFont3 = flash.swf.tags.DefineFont3;
using DefineTag = flash.swf.tags.DefineTag;
using DefineFontName = flash.swf.tags.DefineFontName;
using DefineFontAlignZones = flash.swf.tags.DefineFontAlignZones;
using ZoneRecord = flash.swf.tags.ZoneRecord;
using GlyphEntry = flash.swf.types.GlyphEntry;
using KerningRecord = flash.swf.types.KerningRecord;
using Rect = flash.swf.types.Rect;
using Shape = flash.swf.types.Shape;
using IntMap = flash.util.IntMap;
using Trace = flash.util.Trace;
namespace flash.swf.builder.tags
{
	
	/// <summary> A utility class to build a DefineFont2, or DefineFont3 tag. One
	/// must supply a font family name and style to establish a default font face
	/// and a <code>FontManager</code> to locate and cache fonts and glyphs.
	/// 
	/// </summary>
	/// <author>  Peter Farland
	/// </author>
	public sealed class FontBuilder : TagBuilder
	{
		public System.String Copyright
		{
			get
			{
				return defaultFace.Copyright;
			}
			
		}
		public System.String Name
		{
			get
			{
				return defaultFace.getFamily();
			}
			
		}
		public FSType FSType
		{
			get
			{
				return defaultFace.FSType;
			}
			
		}
		public int Langcode
		{
			set
			{
				if (value >= 0 && value < 6)
					tag.langCode = value;
			}
			
		}
		public double FontHeight
		{
			get
			{
				return fontHeight;
			}
			
		}
		public DefineFont2 tag;
		
		private bool flashType;
		private IntMap glyphEntryMap; // Code-point ordered collection of glyphs
		private FontFace defaultFace;
		private double fontHeight;
		private ZoneRecordBuilder zoneRecordBuilder;
		
		//UPGRADE_NOTE: Final was removed from the declaration of 'IDENTITY_RECT '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		private static readonly Rect IDENTITY_RECT = new Rect();
		private static bool useLicenseTag = true;
		
		private FontBuilder(int code, bool hasLayout, bool useFlashType)
		{
			if (code == flash.swf.TagValues_Fields.stagDefineFont2)
				tag = new DefineFont2();
			else if (code == flash.swf.TagValues_Fields.stagDefineFont3)
				tag = new DefineFont3();
			else
				throw new SWFFontNotSupportedException("Cannot build DefineFont for SWF tag code " + code);
			
			tag.hasLayout = hasLayout;
			glyphEntryMap = new IntMap(100); //Sorted by code point order
			flashType = useFlashType;
		}
		
		/// <summary> Build a DefineFont2 or DefineFont3 tag for a given FontFace.
		/// 
		/// Note that with this constructor, flashType is assumed to be false.
		/// 
		/// </summary>
		/// <param name="code">Determines the version of DefineFont SWF tag to use.
		/// </param>
		/// <param name="fontFace">The FontFace to build into a DefineFont tag.
		/// </param>
		/// <param name="alias">The name used to bind a DefineFont tag to other SWF tags
		/// (such as DefineEditText).
		/// </param>
		public FontBuilder(int code, FontFace fontFace, System.String alias):this(code, true, false)
		{
			defaultFace = fontFace;
			init(alias);
		}
		
		/// <summary> Build a DefineFont2 or DefineFont3 tag from a system font by family name.
		/// 
		/// </summary>
		/// <param name="code">Determines the version of DefineFont SWF tag to use.
		/// </param>
		/// <param name="manager">A FontManager resolves the fontFamily and style to 
		/// a FontFace.
		/// </param>
		/// <param name="alias">The name used to bind a DefineFont tag to other SWF tags
		/// (such as DefineEditText).
		/// </param>
		/// <param name="fontFamily">The name of the font family.
		/// </param>
		/// <param name="style">An integer describing the style variant of the FontFace, 
		/// either plain, bold, italic, or bolditalic.
		/// </param>
		/// <param name="hasLayout">Determines whether font layout metrics should be encoded.
		/// </param>
		/// <param name="flashType">Determines whether FlashType advanced anti-aliasing
		/// information should be included.
		/// </param>
		public FontBuilder(int code, FontManager manager, System.String alias, System.String fontFamily, int style, bool hasLayout, bool flashType):this(code, hasLayout, flashType)
		{
			
			if (manager == null)
				throw new NoFontManagerException();
			
			if (Trace.font)
				Trace.trace("Locating font using FontManager '" + manager.GetType().FullName + "'");
			
			bool useTwips = code != flash.swf.TagValues_Fields.stagDefineFont && code != flash.swf.TagValues_Fields.stagDefineFont2;
			FontFace fontFace = manager.getEntryFromSystem(fontFamily, style, useTwips);
			
			if (fontFace == null)
				throwFontNotFound(alias, fontFamily, style, null);
			
			if (Trace.font)
				Trace.trace("Initializing font '" + fontFamily + "' as '" + alias + "'");
			
			this.defaultFace = fontFace;
			
			init(alias);
		}
		
		/// <summary> Load a font from a URL
		/// 
		/// </summary>
		/// <param name="code">
		/// </param>
		/// <param name="alias">The name used to bind a DefineFont tag to a DefineEditText tag.
		/// </param>
		/// <param name="location">remote url or a relative, local file path
		/// </param>
		/// <param name="style">
		/// </param>
		/// <param name="hasLayout">
		/// </param>
		public FontBuilder(int code, FontManager manager, System.String alias, System.Uri location, int style, bool hasLayout, bool flashType):this(code, hasLayout, flashType)
		{
			
			if (manager == null)
				throw new NoFontManagerException();
			
			if (Trace.font)
				Trace.trace("Locating font using FontManager '" + manager.GetType().FullName + "'");
			
			bool useTwips = code != flash.swf.TagValues_Fields.stagDefineFont && code != flash.swf.TagValues_Fields.stagDefineFont2;
			FontFace fontFace = manager.getEntryFromLocation(location, style, useTwips);
			
			if (fontFace == null)
				throwFontNotFound(alias, null, style, location.ToString());
			
			if (Trace.font)
				Trace.trace("Initializing font at '" + location.ToString() + "' as '" + alias + "'");
			
			this.defaultFace = fontFace;
			
			init(alias);
		}
		
		private void  init(System.String alias)
		{
			fontHeight = defaultFace.PointSize;
			
			if (tag.code != flash.swf.TagValues_Fields.stagDefineFont)
			{
				tag.fontName = alias;
				tag.bold = defaultFace.isBold();
				tag.italic = defaultFace.isItalic();
				
				if (tag.hasLayout)
				{
					tag.ascent = defaultFace.Ascent;
					tag.descent = defaultFace.Descent;
					tag.leading = defaultFace.LineGap;
					
					if (Trace.font)
					{
						Trace.trace("\tBold: " + tag.bold);
						Trace.trace("\tItalic: " + tag.italic);
						Trace.trace("\tAscent: " + tag.ascent);
						Trace.trace("\tDescent: " + tag.descent);
						Trace.trace("\tLeading: " + tag.leading);
					}
				}
			}
			
			// If flashType enabled we must have z, Z, l, L
			if (flashType)
			{
				GlyphEntry adfGE = defaultFace.getGlyphEntry('z');
				if (adfGE == null)
					flashType = false;
				
				adfGE = defaultFace.getGlyphEntry('Z');
				if (adfGE == null)
					flashType = false;
				
				adfGE = defaultFace.getGlyphEntry('l');
				if (adfGE == null)
					flashType = false;
				
				adfGE = defaultFace.getGlyphEntry('L');
				if (adfGE == null)
					flashType = false;
			}
			
			if (flashType)
			{
				zoneRecordBuilder = ZoneRecordBuilder.createInstance();
				if (zoneRecordBuilder != null)
				{
					zoneRecordBuilder.FontAlias = alias;
					zoneRecordBuilder.FontBuilder = this;
					zoneRecordBuilder.FontFace = defaultFace;
				}
				else
				{
					// FlashType Zone Records are not available, so we should
					// disable flashType
					flashType = false;
				}
			}
			
			addChar(' '); // Add at least a space char by default
		}
		
		/// <summary> Creates a DefineFont2 or DefineFont3 tag depending on the code specified
		/// on construction.
		/// </summary>
		public DefineTag build()
		{
			int count = glyphEntryMap.size();
			
			if (Trace.font)
				Trace.trace("Building font '" + tag.fontName + "' with " + count + " characters.");
			
			if (flashType && tag is DefineFont3)
			{
				DefineFont3 df3 = (DefineFont3) tag;
				df3.zones = new DefineFontAlignZones();
				df3.zones.font = df3;
				df3.zones.zoneTable = new ZoneRecord[count];
				df3.zones.csmTableHint = 1;
			}
			
			tag.glyphShapeTable = new Shape[count];
			
			if (tag.code != flash.swf.TagValues_Fields.stagDefineFont)
			{
				tag.codeTable = new char[count];
				
				if (tag.hasLayout)
				{
					tag.advanceTable = new short[count];
					tag.boundsTable = new Rect[count];
				}
			}
			
			// Process each GlyphEntry
			System.Collections.IEnumerator it = glyphEntryMap.iterator();
			int i = 0;
			
			// long flashTypeTime = 0;
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext() && i < count)
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				GlyphEntry ge = (GlyphEntry) ((System.Collections.DictionaryEntry) it.Current).Value;
				
				if (flashType && tag is DefineFont3)
				{
					((DefineFont3) tag).zones.zoneTable[i] = ge.zoneRecord;
				}
				
				// Note: offsets to shape table entries calculated on encoding
				tag.glyphShapeTable[i] = ge.shape;
				
				// IMPORTANT! Update GlyphEntry Index
				ge.Index = i;
				
				// DEFINEFONT2/3 specific properties
				if (tag.code != flash.swf.TagValues_Fields.stagDefineFont)
				{
					tag.codeTable[i] = ge.character; // unsigned code point
					
					// Layout information
					if (tag.hasLayout)
					{
						tag.advanceTable[i] = (short) ge.advance; //advance in emScale
						// The player doesn't need ge.bounds, so we ignore it. 
						// We must still generate this value, however, for ADF use.
						tag.boundsTable[i] = IDENTITY_RECT;
					}
					else
					{
						if (Trace.font)
							Trace.trace("Warning: font tag created without layout information.");
					}
				}
				
				i++;
			}
			
			if (tag.hasLayout)
			{
				tag.kerningTable = new KerningRecord[0];
			}
			
			// FIXME: we should allow the user to set the language code
			//tag.langCode = 1;
			
			// if we have any license info, create a DefineFontName tag
			if (useLicenseTag && ((FSType != null && !FSType.installable) || Copyright != null || Name != null))
			{
				tag.license = new DefineFontName();
				tag.license.font = tag;
				tag.license.fontName = Name;
				tag.license.copyright = Copyright;
			}
			
			return tag;
		}
		
		/// <summary> Adds all supported characters from 0 to the highest glyph
		/// contained in the default font face.
		/// </summary>
		public void  addAllChars()
		{
			addAllChars(defaultFace);
		}
		
		/// <summary> Adds all supported characters from 0 to the highest glyph
		/// contained in the given font face.
		/// 
		/// </summary>
		/// <param name="face">
		/// </param>
		public void  addAllChars(FontFace face)
		{
			int min = face.FirstChar;
			int count = face.NumGlyphs;
			
			if (Trace.font)
				Trace.trace("\tAdding " + count + " chars, starting from " + min);
			
			addCharset(min, count);
		}
		
		/// <summary> Adds supported characters in the specified range from the default
		/// font face.
		/// 
		/// </summary>
		/// <param name="fromChar">
		/// </param>
		/// <param name="count">
		/// </param>
		public void  addCharset(int fromChar, int count)
		{
			addCharset(defaultFace, fromChar, count);
		}
		
		/// <summary> Adds supported characters in the specified range from the given
		/// font face.
		/// 
		/// </summary>
		/// <param name="face">
		/// </param>
		/// <param name="fromChar">
		/// </param>
		/// <param name="count">
		/// </param>
		public void  addCharset(FontFace face, int fromChar, int count)
		{
			int remaining = count;
			
			for (int i = fromChar; remaining > 0 && i < System.Char.MaxValue; i++)
			{
				char c = (char) i;
				GlyphEntry ge = addChar(face, c);
				if (ge != null)
				{
					remaining--;
				}
			}
		}
		
		/// <summary> Adds all supported characters in the given array from the
		/// default font face.
		/// 
		/// </summary>
		/// <param name="chars">
		/// </param>
		public void  addCharset(char[] chars)
		{
			addCharset(defaultFace, chars);
		}
		
		/// <summary> Adds all supported characters in array from the given font face.
		/// 
		/// </summary>
		/// <param name="face">
		/// </param>
		/// <param name="chars">
		/// </param>
		public void  addCharset(FontFace face, char[] chars)
		{
			//TODO: Sort before adding to optimize IntMap addition
			for (int i = 0; i < chars.Length; i++)
			{
				char c = chars[i];
				addChar(face, c);
			}
		}
		
		/// <summary> If supported, includes a given character from the default font face.
		/// 
		/// </summary>
		/// <param name="c">
		/// </param>
		public void  addChar(char c)
		{
			addChar(defaultFace, c);
		}
		
		/// <summary> If supported, includes a character from the given font face.
		/// 
		/// </summary>
		/// <param name="c">
		/// </param>
		public GlyphEntry addChar(FontFace face, char c)
		{
			GlyphEntry ge = (GlyphEntry) glyphEntryMap.get_Renamed(c);
			
			if (ge == null)
			{
				ge = face.getGlyphEntry(c);
				
				if (ge != null)
				{
					//Add to this tag's collection
					glyphEntryMap.put(c, ge);
				}
			}
			
			if (flashType && ge != null && ge.zoneRecord == null && zoneRecordBuilder != null)
			{
				ge.zoneRecord = zoneRecordBuilder.build(c);
			}
			
			return ge;
		}
		
		public GlyphEntry getGlyph(char c)
		{
			return (GlyphEntry) glyphEntryMap.get_Renamed(c);
		}
		
		public int size()
		{
			return glyphEntryMap.size();
		}
		
		private void  throwFontNotFound(System.String alias, System.String fontFamily, int style, System.String location)
		{
			System.Text.StringBuilder message = new System.Text.StringBuilder("Font for alias '");
			message.Append(alias).Append("' ");
			if (style == FontFace.BOLD)
			{
				message.Append("with bold weight ");
			}
			else if (style == FontFace.ITALIC)
			{
				message.Append("with italic style ");
			}
			else if (style == (FontFace.BOLD + FontFace.ITALIC))
			{
				message.Append("with bold weight and italic style ");
			}
			else
			{
				message.Append("with plain weight and style ");
			}
			
			if (location != null)
			{
				message.Append("was not found at: ").Append(location.ToString());
			}
			else
			{
				message.Append("was not found by family name '").Append(fontFamily).Append("'");
			}
			throw new FontNotFoundException(message.ToString());
		}
		
		[Serializable]
		public sealed class FontNotFoundException:System.SystemException
		{
			private const long serialVersionUID = - 2385779348825570473L;
			
			public FontNotFoundException(System.String message):base(message)
			{
			}
		}
		
		[Serializable]
		public sealed class NoFontManagerException:System.SystemException
		{
			private const long serialVersionUID = 755054716704678420L;
			
			public NoFontManagerException():base("No FontManager provided. Cannot build font.")
			{
			}
		}
		
		[Serializable]
		public sealed class SWFFontNotSupportedException:System.SystemException
		{
			private const long serialVersionUID = - 7381079883711386211L;
			
			public SWFFontNotSupportedException(System.String message):base(message)
			{
			}
		}
	}
}