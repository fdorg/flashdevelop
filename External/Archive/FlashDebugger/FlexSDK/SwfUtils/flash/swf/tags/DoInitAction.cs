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

using Flash.Swf;
using ActionList = Flash.Swf.Types.ActionList;

namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class DoInitAction:Tag
	{
		public DoInitAction():base(Flash.Swf.TagValues.stagDoInitAction)
		{
		}
		
		public DoInitAction(DefineSprite sprite):this()
		{
			this.sprite = sprite;
			sprite.initAction = this;
		}
		
		public override void  visit(TagHandler h)
		{
			h.doInitAction(this);
		}
		
		public override Tag SimpleReference
		{
            get
            {
                return sprite;
            }
		}
		
		public DefineSprite sprite;
		public ActionList actionList;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DoInitAction))
			{
				DoInitAction doInitAction = (DoInitAction) object_Renamed;

				System.Diagnostics.Debug.Assert(doInitAction.sprite.initAction == doInitAction);
				
				// [paul] Checking that the sprite fields are equal would
				// lead to an infinite loop, because DefineSprite contains
				// a reference to it's DoInitAction.  Also don't compare
				// the order fields, because they are never the same.
				if (equals(doInitAction.actionList, this.actionList))
				{
					isEqual = true;
				}
			}

			System.Diagnostics.Debug.Assert(sprite.initAction == this);
			
			return isEqual;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
