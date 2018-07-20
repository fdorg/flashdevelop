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
using Action = Flash.Swf.Action;
using ActionHandler = Flash.Swf.ActionHandler;
using ActionConstants = Flash.Swf.ActionConstants;
namespace Flash.Swf.Actions
{
	
	/// <author>  Clement Wong
	/// </author>
	public class WaitForFrame:Action
	{
		public WaitForFrame(int code):base(code)
		{
		}
		
		public override void  visit(ActionHandler h)
		{
			if (code == Flash.Swf.ActionConstants.sactionWaitForFrame)
				h.waitForFrame(this);
			else
				h.waitForFrame2(this);
		}
		
		/// <summary> Frame number to wait for (WaitForFrame only).  WaitForFrame2 takes
		/// its frame argument from the stack.
		/// </summary>
		public int frame;
		
		/// <summary>  label marking the number of actions to skip if frame is not loaded</summary>
		public Label skipTarget;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is WaitForFrame))
			{
				WaitForFrame waitForFrame = (WaitForFrame) object_Renamed;
				
				if ((waitForFrame.frame == this.frame) && (waitForFrame.skipTarget == this.skipTarget))
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
