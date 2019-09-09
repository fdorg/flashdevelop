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
using Tag = Flash.Swf.Tag;
using TagHandler = Flash.Swf.TagHandler;
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class ImportAssets:Tag
	{
		public ImportAssets(int code):base(code)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			if (code == Flash.Swf.TagValues.stagImportAssets)
				h.importAssets(this);
			else
				h.importAssets2(this);
		}
		
		public String url;
		public System.Collections.IList importRecords;
		
		public bool downloadNow;
		public byte[] SHA1;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is ImportAssets))
			{
				ImportAssets importAssets = (ImportAssets) object_Renamed;
				
				if (equals(importAssets.url, this.url) && (importAssets.downloadNow == this.downloadNow) && digestEquals(importAssets.SHA1, this.SHA1) && equals(importAssets.importRecords, this.importRecords))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		private bool digestEquals(byte[] d1, byte[] d2)
		{
			if (d1 == null && d2 == null)
			{
				return true;
			}
			else
			{
				for (int i = 0; i < 20; i++)
				{
					if (d1[i] != d2[i])
					{
						return false;
					}
				}
				return true;
			}
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
