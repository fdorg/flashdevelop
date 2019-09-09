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
	public class ClipActions
	{
		/// <summary> All events used in these clip actions</summary>
		public int allEventFlags;
		
		/// <summary> Individual event handlers.  List of ClipActionRecord instances.</summary>
		public System.Collections.IList clipActionRecords;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is ClipActions)
			{
				ClipActions clipActions = (ClipActions) object_Renamed;
				
				if ((clipActions.allEventFlags == this.allEventFlags) && (((clipActions.clipActionRecords == null) && (this.clipActionRecords == null)) || ((clipActions.clipActionRecords != null) && (this.clipActionRecords != null) && ArrayLists.equals(clipActions.clipActionRecords, this.clipActionRecords))))
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
