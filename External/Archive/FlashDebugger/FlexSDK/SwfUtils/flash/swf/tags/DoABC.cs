// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
using Tag = Flash.Swf.Tag;
using TagHandler = Flash.Swf.TagHandler;
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class DoABC:Tag
	{
		public DoABC(String name, int flag):base(Flash.Swf.TagValues.stagDoABC2)
		{
			abc = new byte[0];
			this.name = name;
			this.flag = flag;
		}
		public DoABC():base(Flash.Swf.TagValues.stagDoABC)
		{
			abc = new byte[0];
			name = null;
			flag = 1;
		}
		
		public override void  visit(TagHandler h)
		{
			h.doABC(this);
		}
		
		public byte[] abc;
		public String name;
		public int flag;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DoABC))
			{
				DoABC doABC = (DoABC) object_Renamed;
				
				if (equals(doABC.abc, this.abc) && equals(doABC.name, this.name) && doABC.flag == this.flag)
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode() + DefineTag.PRIME * abc.GetHashCode() & name.GetHashCode() + flag;
		}
	}
}
