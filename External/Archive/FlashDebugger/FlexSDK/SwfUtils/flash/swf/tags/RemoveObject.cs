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
using Tag = Flash.Swf.Tag;
using TagHandler = Flash.Swf.TagHandler;
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class RemoveObject:Tag
	{
		public RemoveObject(int code):base(code)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			if (code == Flash.Swf.TagValues.stagRemoveObject)
				h.removeObject(this);
			else
				h.removeObject2(this);
		}
		
		public override Tag SimpleReference
		{
            get
            {
                return ref_Renamed;
            }
		}
		
		public int depth;
		public DefineTag ref_Renamed;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is RemoveObject))
			{
				RemoveObject removeObject = (RemoveObject) object_Renamed;
				
				if ((removeObject.depth == this.depth) && equals(removeObject.ref_Renamed, this.ref_Renamed))
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
