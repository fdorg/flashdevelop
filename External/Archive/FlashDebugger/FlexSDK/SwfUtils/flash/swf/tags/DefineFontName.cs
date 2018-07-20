// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
namespace Flash.Swf.Tags
{
	
	/// <summary> Stores the name and copyright information for a font.
	/// 
	/// </summary>
	/// <author>  Brian Deitte
	/// </author>
	public class DefineFontName:DefineTag
	{
		public DefineFontName():base(Flash.Swf.TagValues.stagDefineFontName)
		{
		}
		
		public override void  visit(Flash.Swf.TagHandler h)
		{
			h.defineFontName(this);
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineFontName))
			{
				DefineFontName defineFontName = (DefineFontName) object_Renamed;
				isEqual = (equals(font, defineFontName.font) && (equals(fontName, defineFontName.fontName)) && (equals(copyright, defineFontName.copyright)));
			}
			
			return isEqual;
		}
		
		public DefineFont font;
		public String fontName;
		public String copyright;
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
