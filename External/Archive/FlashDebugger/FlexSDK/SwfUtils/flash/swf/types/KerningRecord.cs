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
	public class KerningRecord
	{
		public int code1;
		public int code2;
		public int adjustment;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is KerningRecord)
			{
				KerningRecord kerningRecord = (KerningRecord) object_Renamed;
				
				if ((kerningRecord.code1 == this.code1) && (kerningRecord.code2 == this.code2) && (kerningRecord.adjustment == this.adjustment))
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
