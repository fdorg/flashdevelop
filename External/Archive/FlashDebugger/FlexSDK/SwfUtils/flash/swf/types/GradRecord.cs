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
	public class GradRecord
	{
		public GradRecord()
		{
		}
		
		public GradRecord(int r, int c)
		{
			ratio = r;
			color = c;
		}
		
		public int ratio;
		
		/// <summary>color as int: 0xAARRGGBB or 0x00RRGGBB </summary>
		public int color;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is GradRecord)
			{
				GradRecord gradRecord = (GradRecord) object_Renamed;
				
				if ((gradRecord.ratio == this.ratio) && (gradRecord.color == this.color))
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
