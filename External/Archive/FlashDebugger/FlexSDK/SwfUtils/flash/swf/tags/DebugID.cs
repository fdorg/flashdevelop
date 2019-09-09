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
using Tag = Flash.Swf.Tag;
using TagValues = Flash.Swf.TagValues;
using FlashUUID = Flash.Swf.Types.FlashUUID;
namespace Flash.Swf.Tags
{
	
	public class DebugID:Tag
	{
		public DebugID(int code):base(code)
		{
		}
		
		public DebugID(FlashUUID uuid):base(Flash.Swf.TagValues.stagDebugID)
		{
			this.uuid = uuid;
		}

        public override void visit(Flash.Swf.TagHandler h)
		{
			h.debugID(this);
		}
		
		public FlashUUID uuid;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DebugID))
			{
				DebugID debugID = (DebugID) object_Renamed;
				
				if (equals(debugID.uuid, this.uuid))
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
