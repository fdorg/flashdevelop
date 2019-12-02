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
using Action = Flash.Swf.Action;
using ActionHandler = Flash.Swf.ActionHandler;
using ActionConstants = Flash.Swf.ActionConstants;
using ArrayUtil = Flash.Util.ArrayUtil;
namespace Flash.Swf.Actions
{
	
	/// <author>  Clement Wong
	/// </author>
	public class ConstantPool:Action
	{
		public ConstantPool():base(Flash.Swf.ActionConstants.sactionConstantPool)
		{
		}
		
		public ConstantPool(String[] poolData):this()
		{
			pool = poolData;
		}
		
		public override void  visit(ActionHandler h)
		{
			h.constantPool(this);
		}
		
		public String[] pool;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is ConstantPool))
			{
				ConstantPool constantPool = (ConstantPool) object_Renamed;
				
				if (ArrayUtil.equals(constantPool.pool, this.pool))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override String ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("ConstantPool[ pool = { ");
			for (int i = 0; i < pool.Length; i++)
			{
				sb.Append(pool[i]);
				sb.Append(", ");
			}
			sb.Append("} ]");
			return sb.ToString();
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
