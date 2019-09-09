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
using Action = Flash.Swf.Action;
using ActionHandler = Flash.Swf.ActionHandler;
using ActionConstants = Flash.Swf.ActionConstants;
namespace Flash.Swf.Actions
{
	
	/// <author>  Clement Wong
	/// </author>
	public class SetTarget:Action
	{
		public SetTarget():base(Flash.Swf.ActionConstants.sactionSetTarget)
		{
		}
		
		public override void  visit(ActionHandler h)
		{
			h.Target = this;
		}
		
		/// <summary> name of target object for subsequent actions.  Empty string ("") means
		/// the current movie.
		/// </summary>
		public String targetName;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is SetTarget))
			{
				SetTarget setTarget = (SetTarget) object_Renamed;
				
				if (equals(setTarget.targetName, this.targetName))
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
