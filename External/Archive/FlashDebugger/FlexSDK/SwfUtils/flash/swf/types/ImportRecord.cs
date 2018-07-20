// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
using TagHandler = Flash.Swf.TagHandler;
using DefineTag = Flash.Swf.Tags.DefineTag;

namespace Flash.Swf.Types
{
	
	/// <summary> this is an import record, which is serialized as a member of an ImportAssets
	/// tag.  We subclass DefineTag because definitions are the things that get
	/// imported; any tag that refers to a definition can also refer to an import
	/// of another definition.
	/// 
	/// </summary>
	/// <author>  Edwin Smith
	/// </author>
	public class ImportRecord:DefineTag
	{
		public ImportRecord():base(Flash.Swf.TagValues.stagImportAssets)
		{
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is ImportRecord))
			{
				ImportRecord importRecord = (ImportRecord) object_Renamed;
				
				if (equals(importRecord.name, this.name))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override void  visit(TagHandler h)
		{
			// this can't be visited, but you can visit the ImportAssets that owns this record.
            System.Diagnostics.Debug.Assert(false);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
