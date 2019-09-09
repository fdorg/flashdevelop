// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2005-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using TagHandler = Flash.Swf.TagHandler;
using ArrayUtil = Flash.Util.ArrayUtil;
namespace Flash.Swf.Tags
{
	
	/// <author>  Brian Deitte
	/// </author>
	public class DefineFontAlignZones:DefineTag
	{
		public DefineFontAlignZones():base(Flash.Swf.TagValues.stagDefineFontAlignZones)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			h.defineFontAlignZones(this);
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineFontAlignZones))
			{
				DefineFontAlignZones alignZones = (DefineFontAlignZones) object_Renamed;
				
				if (font.Equals(alignZones.font) && csmTableHint == alignZones.csmTableHint && ArrayUtil.equals(zoneTable, alignZones.zoneTable))
				{
					isEqual = true;
				}
			}
			return isEqual;
		}
		
		public DefineFont3 font;
		public int csmTableHint;
		public ZoneRecord[] zoneTable;
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
