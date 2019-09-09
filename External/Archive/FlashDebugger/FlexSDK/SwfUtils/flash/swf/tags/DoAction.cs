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
using Tag = Flash.Swf.Tag;
using TagHandler = Flash.Swf.TagHandler;
using ActionList = Flash.Swf.Types.ActionList;
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class DoAction:Tag
	{
		public DoAction():base(Flash.Swf.TagValues.stagDoAction)
		{
		}
		
		public DoAction(ActionList actions):base(Flash.Swf.TagValues.stagDoAction)
		{
			this.actionList = actions;
		}
		
		public override void  visit(TagHandler h)
		{
			h.doAction(this);
		}
		
		public ActionList actionList;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DoAction))
			{
				DoAction doAction = (DoAction) object_Renamed;
				
				if (equals(doAction.actionList, this.actionList))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode += DefineTag.PRIME * actionList.size();
			return hashCode;
		}
	}
}
