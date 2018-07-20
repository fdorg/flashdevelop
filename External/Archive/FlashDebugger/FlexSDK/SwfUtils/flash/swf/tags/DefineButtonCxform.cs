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
using CXForm = Flash.Swf.Types.CXForm;
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class DefineButtonCxform:Tag
	{
		public DefineButtonCxform():base(Flash.Swf.TagValues.stagDefineButtonCxform)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			h.defineButtonCxform(this);
		}
		
		public override Tag SimpleReference
		{
            get
            {
                return button;
            }
		}
		
		public DefineButton button;
		public CXForm colorTransform;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineButtonCxform))
			{
				DefineButtonCxform defineButtonCxform = (DefineButtonCxform) object_Renamed;
				
				if (equals(defineButtonCxform.button, this.button) && equals(defineButtonCxform.colorTransform, this.colorTransform))
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
