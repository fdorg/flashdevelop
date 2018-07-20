// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
using ArrayUtil = Flash.Util.ArrayUtil;
namespace Flash.Swf.Tags
{
	
	/// <author>  Brian Deitte
	/// </author>
	public class ZoneRecord
	{
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			if (base.Equals(object_Renamed) && (object_Renamed is ZoneRecord))
			{
				ZoneRecord record = (ZoneRecord) object_Renamed;
				if (numZoneData == record.numZoneData && ArrayUtil.equals(zoneData, record.zoneData) && zoneMask == record.zoneMask)
				{
					isEqual = true;
				}
			}
			return isEqual;
		}
		
		public int numZoneData;
		public long[] zoneData;
		public int zoneMask;
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
