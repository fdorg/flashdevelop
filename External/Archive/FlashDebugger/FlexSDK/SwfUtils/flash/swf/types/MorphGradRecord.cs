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
	public class MorphGradRecord
	{
		public int startRatio;
		public int endRatio;
		
		/// <summary>colors as ints: 0xAARRGGBB </summary>
		public int startColor;
		public int endColor;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is MorphGradRecord)
			{
				MorphGradRecord morphGradRecord = (MorphGradRecord) object_Renamed;
				
				if ((morphGradRecord.startRatio == this.startRatio) && (morphGradRecord.startColor == this.startColor) && (morphGradRecord.endRatio == this.endRatio) && (morphGradRecord.endColor == this.endColor))
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
