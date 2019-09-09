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
using TagHandler = Flash.Swf.TagHandler;
using Matrix = Flash.Swf.Types.Matrix;
using Rect = Flash.Swf.Types.Rect;
using TextRecord = Flash.Swf.Types.TextRecord;
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class DefineText:DefineTag
	{
		override public System.Collections.IEnumerator References
		{
			get
			{
				System.Collections.ArrayList refs = new System.Collections.ArrayList();
				for (int i = 0; i < records.Count; ++i)
					((TextRecord) records[i]).getReferenceList(refs);
				
				return refs.GetEnumerator();
			}
			
		}
		public DefineText(int code):base(code)
		{
			records = new System.Collections.ArrayList();
		}
		
		public override void  visit(TagHandler h)
		{
			if (code == Flash.Swf.TagValues.stagDefineText)
				h.defineText(this);
			else
				h.defineText2(this);
		}
		
		public Rect bounds;
		public Matrix matrix;
		public System.Collections.IList records;
		public CSMTextSettings csmTextSettings;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineText))
			{
				DefineText defineText = (DefineText) object_Renamed;
				
				if (equals(defineText.bounds, this.bounds) && equals(defineText.matrix, this.matrix) && equals(defineText.records, this.records))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			
			if (bounds != null)
			{
				hashCode += bounds.GetHashCode();
			}
			
			if (records != null)
			{
				hashCode += records.Count;
			}
			return hashCode;
		}
	}
}
