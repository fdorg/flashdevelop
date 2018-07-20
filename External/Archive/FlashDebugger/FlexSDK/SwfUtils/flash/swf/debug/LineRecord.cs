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
namespace Flash.Swf.Debug
{
	
	public class LineRecord:Action
	{
		public LineRecord(int lineno, DebugModule module):base(ActionList.sactionLineRecord)
		{
			this.lineno = lineno;
			this.module = module;
		}
		
		public int lineno;
		public DebugModule module;
		
		public override void  visit(ActionHandler h)
		{
			h.lineRecord(this);
		}
		
		public override String ToString()
		{
			return module.name + ":" + lineno;
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			if (object_Renamed is LineRecord)
			{
				LineRecord other = (LineRecord) object_Renamed;
				return base.Equals(other) && other.lineno == this.lineno && equals(other.module, this.module);
			}
			else
			{
				return false;
			}
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
