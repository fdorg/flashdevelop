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
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class EnableDebugger:Flash.Swf.Tag
	{
		public EnableDebugger(int code):base(code)
		{
		}
		
		public EnableDebugger(String password):base(Flash.Swf.TagValues.stagEnableDebugger2)
		{
			this.password = password;
		}

        public override void visit(Flash.Swf.TagHandler h)
		{
			if (code == Flash.Swf.TagValues.stagEnableDebugger)
				h.enableDebugger(this);
			else
				h.enableDebugger2(this);
		}
		
		public String password;
		public int reserved;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is EnableDebugger))
			{
				EnableDebugger enableDebugger = (EnableDebugger) object_Renamed;
				
				if (equals(enableDebugger.password, this.password))
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
