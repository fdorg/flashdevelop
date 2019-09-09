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
using Tag = Flash.Swf.Tag;
using TagHandler = Flash.Swf.TagHandler;
namespace Flash.Swf.Tags
{
	
	/// <summary> ExportAssets makes portions of a SWF file available for import by other SWF files (see
	/// ImportAssets). For example, ten Flash movies that are all part of the same website can share an
	/// embedded custom font if one movie embeds the font and exports the font character. Each
	/// exported character is identified by a string. Any type of character can be exported.
	/// </summary>
	/// <author>  Clement Wong
	/// </author>
	/// <since> SWF5
	/// </since>
	public class ExportAssets:Tag
	{
		override public System.Collections.IEnumerator References
		{
			get
			{
				return exports.GetEnumerator();
			}
			
		}
		public ExportAssets():base(Flash.Swf.TagValues.stagExportAssets)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			h.exportAssets(this);
		}
		
		/// <summary>list of DefineTags exported by this ExportTag </summary>
		public System.Collections.IList exports = new System.Collections.ArrayList();
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is ExportAssets))
			{
				ExportAssets exportAssets = (ExportAssets) object_Renamed;
				
				if (equals(exportAssets.exports, this.exports))
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
