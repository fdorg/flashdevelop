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
using Tag = Flash.Swf.Tag;
using TagHandler = Flash.Swf.TagHandler;
namespace Flash.Swf.Tags
{
	
	/// <author>  Roger Gonzalez
	/// </author>
	public class DefineSceneAndFrameLabelData:Tag
	{
		public DefineSceneAndFrameLabelData():base(Flash.Swf.TagValues.stagDefineSceneAndFrameLabelData)
		{
		}
		public override void  visit(TagHandler h)
		{
			h.defineSceneAndFrameLabelData(this);
		}
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineSceneAndFrameLabelData))
			{
				isEqual = equals(((DefineSceneAndFrameLabelData) object_Renamed).data, this.data);
			}
			return isEqual;
		}
		public override int GetHashCode()
		{
			return data.GetHashCode();
		}
		// todo: once we care about this tag, break out the fields
		// for now, just allow round-tripping
		public byte[] data;
	}
}
