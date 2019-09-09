////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2004-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using Flash.Swf;
using Flash.Swf.Actions;
using Flash.Swf.Types;

namespace Flash.Tools
{
	
	/// <summary> ActionLocation record.  Used to contain
	/// information regarding a specific location
	/// within an action record.  
	/// 
	/// at and actions are typically guaranteed to 
	/// be filled out.  The others are optional.
	/// </summary>
	/// <seealso cref="SwfActionContainer">
	/// </seealso>
	public class ActionLocation
	{
		public ActionLocation()
		{
			init(- 1, null, null, null, null);
		}
		public ActionLocation(ActionLocation loc)
		{
			init(loc.at, loc.actions, loc.pool, loc.className, loc.function);
		}
		
		internal virtual void  init(int p1, ActionList p2, ConstantPool p3, String p4, DefineFunction p5)
		{
			at = p1;
			actions = p2;
			pool = p3;
			className = p4;
			function = p5;
		}
		
		public int at = - 1;
		public ActionList actions;
		public ConstantPool pool;
		public String className;
		public DefineFunction function;
	}
}
