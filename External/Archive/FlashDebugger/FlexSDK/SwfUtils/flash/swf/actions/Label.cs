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
using ActionList = Flash.Swf.Types.ActionList;
namespace Flash.Swf.Actions
{
	
	/// <author>  Edwin Smith
	/// </author>
	public class Label:Action
	{
		public Label():base(ActionList.sactionLabel)
		{
		}
		
		public override void  visit(ActionHandler h)
		{
			h.label(this);
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			// labels should always be unique unless they really are the same object
			return this == object_Renamed;
		}
		
		public override int GetHashCode()
		{
			// Action.hashCode() allways returns the code, but we want a real hashcode
			// since every instance of Label needs to be unique
			return base.objectHashCode();
		}
	}
}
