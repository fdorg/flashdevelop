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
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class ClipActionRecord
	{
		public const int unused31 = unchecked((int) 0x80000000);
		public const int unused30 = 0x40000000;
		public const int unused29 = 0x20000000;
		public const int unused28 = 0x10000000;
		public const int unused27 = 0x08000000;
		public const int unused26 = 0x04000000;
		public const int unused25 = 0x02000000;
		public const int unused24 = 0x01000000;
		
		public const int unused23 = 0x00800000;
		public const int unused22 = 0x00400000;
		public const int unused21 = 0x00200000;
		public const int unused20 = 0x00100000;
		public const int unused19 = 0x00080000;
		public const int construct = 0x00040000;
		public const int keyPress = 0x00020000;
		public const int dragOut = 0x00010000;
		
		public const int dragOver = 0x00008000;
		public const int rollOut = 0x00004000;
		public const int rollOver = 0x00002000;
		public const int releaseOutside = 0x00001000;
		public const int release = 0x00000800;
		public const int press = 0x00000400;
		public const int initialize = 0x00000200;
		public const int data = 0x00000100;
		
		public const int keyUp = 0x00000080;
		public const int keyDown = 0x00000040;
		public const int mouseUp = 0x00000020;
		public const int mouseDown = 0x00000010;
		public const int mouseMove = 0x00000008;
		public const int unload = 0x00000004;
		public const int enterFrame = 0x00000002;
		public const int load = 0x00000001;
		
		/// <summary> event(s) to which this handler applies</summary>
		public int eventFlags;
		
		/// <summary> if eventFlags.press is true, contains the key code to trap</summary>
		/// <seealso cref="ButtonCondAction">
		/// </seealso>
		public int keyCode;
		
		/// <summary> actions to perform</summary>
		public ActionList actionList;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is ClipActionRecord)
			{
				ClipActionRecord clipActionRecord = (ClipActionRecord) object_Renamed;
				
				if ((clipActionRecord.eventFlags == this.eventFlags) && (clipActionRecord.keyCode == this.keyCode) && (((clipActionRecord.actionList == null) && (this.actionList == null)) || ((clipActionRecord.actionList != null) && (this.actionList != null) && clipActionRecord.actionList.Equals(this.actionList))))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public virtual bool hasKeyPress()
		{
			return (eventFlags & keyPress) != 0;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
