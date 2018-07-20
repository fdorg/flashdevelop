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

using Flash.Swf;

namespace Flash.Swf.Actions
{
	
	/// <author>  Clement Wong
	/// </author>
	public class Branch: Action
	{
		public Branch(int code):base(code)
		{
		}
		
		public override void  visit(ActionHandler h)
		{
			if (code == Flash.Swf.ActionConstants.sactionJump)
				h.jump(this);
			else
				h.ifAction(this);
		}
		
		/// <summary> branch offset relative to the next instruction after the JUMP</summary>
		public Label target;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is Branch))
			{
				Branch branch = (Branch) object_Renamed;
				
				if (branch.target == this.target)
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
