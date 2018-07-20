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
using ArrayUtil = Flash.Util.ArrayUtil;
namespace Flash.Swf.Tags
{
	
	/// <author>  Roger Gonzalez
	/// </author>
	public class DefineBinaryData:DefineTag
	{
		public DefineBinaryData():base(Flash.Swf.TagValues.stagDefineBinaryData)
		{
		}
		public override void  visit(Flash.Swf.TagHandler h)
		{
			h.defineBinaryData(this);
		}
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineBinaryData))
			{
				DefineBinaryData defineBinaryData = (DefineBinaryData) object_Renamed;
				
				if (ArrayUtil.equals(defineBinaryData.data, this.data))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public int reserved;
		public byte[] data;
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
