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
namespace Flash.Swf.Tags
{
	
	/// <summary> A DefineFont tag defines the shape outlines of each glyph used in a
	/// particular font.
	/// 
	/// The DefineFont tag id can be used by subsequent DefineText tags to refer
	/// to the font definition. As with all SWF character ids, font ids must be
	/// unique amongst all character ids in a SWF file.
	/// 
	/// The first version of DefineFont was introduced in SWF 1.
	/// 
	/// The second version, DefineFont2, was introduced in SWF 3.
	/// 
	/// The third version, DefineFont3, was introduced in SWF 8, along with
	/// DefineFontAlignZones for advanced anti-aliasing.
	/// 
	/// In SWF 9, DefineFontName was introduced to record the formal font name
	/// and copyright information for an embedded font.
	/// 
	/// The fourth version, DefineFont4, was introduced in SWF 10.  
	/// 
	/// Note that DefineFont tags cannot be used for dynamic text. Dynamic text
	/// requires the DefineFont2 tag (or later).
	/// 
	/// </summary>
	/// <seealso cref="DefineFont1">
	/// </seealso>
	/// <seealso cref="DefineFont2">
	/// </seealso>
	/// <seealso cref="DefineFont3">
	/// </seealso>
	/// <seealso cref="DefineFont4">
	/// 
	/// </seealso>
	/// <author>  Clement Wong
	/// </author>
	/// <author>  Peter Farland
	/// </author>
	public abstract class DefineFont:DefineTag
	{
		public abstract String FontName{get;}
		public abstract bool Bold{get;}
		public abstract bool Italic{get;}
		protected internal DefineFont(int code):base(code)
		{
		}
		
		public DefineFontName license;
	}
}
