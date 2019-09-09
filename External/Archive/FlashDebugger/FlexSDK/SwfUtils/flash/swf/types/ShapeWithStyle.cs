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
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class ShapeWithStyle:Shape
	{
		public System.Collections.ArrayList fillstyles;
		public System.Collections.ArrayList linestyles;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is ShapeWithStyle))
			{
				ShapeWithStyle shapeWithStyle = (ShapeWithStyle) object_Renamed;
				
				if ((((shapeWithStyle.fillstyles == null) && (this.fillstyles == null)) || ((shapeWithStyle.fillstyles != null) && (this.fillstyles != null) && ArrayLists.equals(shapeWithStyle.fillstyles, this.fillstyles))) && (((shapeWithStyle.linestyles == null) && (this.linestyles == null)) || ((shapeWithStyle.linestyles != null) && (this.linestyles != null) && ArrayLists.equals(shapeWithStyle.linestyles, this.linestyles))))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override void  getReferenceList(System.Collections.IList refs)
		{
			base.getReferenceList(refs);
			
            foreach (FillStyle style in fillstyles)
			{
				if (style.hasBitmapId() && style.bitmap != null)
				{
					refs.Add(style.bitmap);
				}
			}
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
