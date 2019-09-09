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
namespace Flash.Swf.Tags
{
	
	/// <author>  Brian Deitte
	/// </author>
	public class CSMTextSettings:DefineTag
	{
		public CSMTextSettings():base(Flash.Swf.TagValues.stagCSMTextSettings)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			h.csmTextSettings(this);
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			if (base.Equals(object_Renamed) && (object_Renamed is CSMTextSettings))
			{
				CSMTextSettings settings = (CSMTextSettings) object_Renamed;
				if (textReference.Equals(settings.textReference) && styleFlagsUseSaffron == settings.styleFlagsUseSaffron && gridFitType == settings.gridFitType && thickness == settings.thickness && sharpness == settings.sharpness)
				{
					isEqual = true;
				}
			}
			return isEqual;
		}
		
		public DefineTag textReference;
		public int styleFlagsUseSaffron; // 0 if off, 1 if on
		public int gridFitType; // 0 if none, 1 if pixel, 2 if subpixel
		public long thickness;
		public long sharpness;
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
