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
using ArrayUtil = Flash.Util.ArrayUtil;
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class SoundInfo
	{
		public const int UNINITIALIZED = - 1;
		
		public bool syncStop;
		public bool syncNoMultiple;
		
		// they are unsigned, so if they're -1, they're not initialized.
		public long inPoint = UNINITIALIZED;
		public long outPoint = UNINITIALIZED;
		public int loopCount = UNINITIALIZED;
		
		/// <summary>pos44:32, leftLevel:16, rightLevel:16 </summary>
		public long[] records = new long[0];
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is SoundInfo)
			{
				SoundInfo soundInfo = (SoundInfo) object_Renamed;
				
				if ((soundInfo.syncStop == this.syncStop) && (soundInfo.syncNoMultiple == this.syncNoMultiple) && (soundInfo.inPoint == this.inPoint) && (soundInfo.outPoint == this.outPoint) && (soundInfo.loopCount == this.loopCount) && ArrayUtil.equals(soundInfo.records, this.records))
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
