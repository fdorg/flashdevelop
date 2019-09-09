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
using Action = Flash.Swf.Action;
using ActionHandler = Flash.Swf.ActionHandler;
using ActionConstants = Flash.Swf.ActionConstants;
namespace Flash.Swf.Actions
{
	
	/// <author>  Clement Wong
	/// </author>
	public class GotoLabel:Action
	{
		public GotoLabel():base(Flash.Swf.ActionConstants.sactionGotoLabel)
		{
		}
		
		public override void  visit(ActionHandler h)
		{
			h.gotoLabel(this);
		}
		
		/// <summary> Frame label, as attached to a frame using the FrameLabel tag.</summary>
		public String label;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is GotoLabel))
			{
				GotoLabel gotoLabel = (GotoLabel) object_Renamed;
				
				if (equals(gotoLabel.label, this.label))
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
