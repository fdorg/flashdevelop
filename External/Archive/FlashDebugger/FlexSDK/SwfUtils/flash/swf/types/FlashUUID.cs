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
	
	public sealed class FlashUUID
	{
		private const int kUUIDSize = 16;
		
		public FlashUUID(byte[] bytes)
		{
			if (bytes == null)
				throw new System.NullReferenceException();
			this.bytes = bytes;
		}
		
		public FlashUUID()
		{
			this.bytes = new byte[kUUIDSize];
		}
		
		public byte[] bytes;
		
		public override String ToString()
		{
			return MD5.stringify(bytes);
		}
		
		public override int GetHashCode()
		{
			int length = bytes.Length;
			int code = length;
			for (int i = 0; i < length; i++)
			{
				code = (code << 1) ^ bytes[i];
			}
			return code;
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is FlashUUID)
			{
				FlashUUID flashUUID = (FlashUUID) object_Renamed;
				if (ArrayUtil.equals(flashUUID.bytes, this.bytes))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
	}
}
