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

using Flash.Swf;

namespace Flash.Swf.Actions
{
	
	/// <author>  Clement Wong
	/// </author>
	public class Push:Action
	{
		public Push():base(Flash.Swf.ActionConstants.sactionPush)
		{
		}
		
		public Push(System.Object value):this()
		{
			this.value = value;
		}
		
		public override void  visit(ActionHandler h)
		{
			h.push(this);
		}
		
		/// <summary>the value to push </summary>
		public System.Object value;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is Push))
			{
				Push push = (Push) object_Renamed;
				
				if (equals(push.value, this.value))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public static int getTypeCode(System.Object value)
		{
			if (value == null)
				return Flash.Swf.ActionConstants.kPushNullType;
			if (value == ActionFactory.UNDEFINED)
				return Flash.Swf.ActionConstants.kPushUndefinedType;
			if (value is String)
				return Flash.Swf.ActionConstants.kPushStringType;
			if (value is System.Single)
				return Flash.Swf.ActionConstants.kPushFloatType;
			if (value is System.SByte)
				return Flash.Swf.ActionConstants.kPushRegisterType;
			if (value is System.Boolean)
				return Flash.Swf.ActionConstants.kPushBooleanType;
			if (value is System.Double)
				return Flash.Swf.ActionConstants.kPushDoubleType;
			if (value is System.Int32)
				return Flash.Swf.ActionConstants.kPushIntegerType;
			if (value is System.Int16)
				return (((int) ((System.Int16) value) & 0xFFFF) < 256)?Flash.Swf.ActionConstants.kPushConstant8Type:Flash.Swf.ActionConstants.kPushConstant16Type;
			System.Diagnostics.Debug.Assert(false);
			return Flash.Swf.ActionConstants.kPushStringType;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
