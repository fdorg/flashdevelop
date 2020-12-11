// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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
using Action = Flash.Swf.Action;
using ActionHandler = Flash.Swf.ActionHandler;
using ActionConstants = Flash.Swf.ActionConstants;
namespace Flash.Swf.Actions
{
	
	/// <author>  Clement Wong
	/// </author>
	public class GotoFrame2:Action
	{
		public GotoFrame2():base(Flash.Swf.ActionConstants.sactionGotoFrame2)
		{
		}
		
		public override void  visit(ActionHandler h)
		{
			h.gotoFrame2(this);
		}
		
		/// <summary> if the play flag is set, the action goes to the specified frame and
		/// begins playing the enclosed movie clip.  Otherwise, the action goes
		/// to the specified frame and stops.
		/// </summary>
		public int playFlag;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is GotoFrame2))
			{
				GotoFrame2 gotoFrame2 = (GotoFrame2) object_Renamed;
				
				if (gotoFrame2.playFlag == this.playFlag)
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
