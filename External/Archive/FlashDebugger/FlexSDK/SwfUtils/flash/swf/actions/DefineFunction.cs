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
using ActionList = Flash.Swf.Types.ActionList;
using ArrayUtil = Flash.Util.ArrayUtil;
namespace Flash.Swf.Actions
{
	
	/// <author>  Clement Wong
	/// </author>
	public class DefineFunction:Action
	{
		public const int kPreloadThis = 0x0001;
		public const int kSuppressThis = 0x0002;
		public const int kPreloadArguments = 0x0004;
		public const int kSuppressArguments = 0x0008;
		public const int kPreloadSuper = 0x0010;
		public const int kSuppressSuper = 0x0020;
		public const int kPreloadRoot = 0x0040;
		public const int kPreloadParent = 0x0080;
		public const int kPreloadGlobal = 0x0100;
		
		public DefineFunction(int code):base(code)
		{
		}
		
		public override void  visit(ActionHandler h)
		{
			if (code == Flash.Swf.ActionConstants.sactionDefineFunction)
				h.defineFunction(this);
			else
			{
				h.defineFunction2(this);
			}
		}
		
		public String name;
		public String[] params_Renamed;
		public ActionList actionList;
		
		// defineFunction2 only
		public int[] paramReg;
		public int regCount;
		public int flags;
		
		// C: I want to expose the code size to MovieMetaData so that the profiler can output the
		//    function code size vs. performance... ActionEncoder should not use this value.
		//    ActionDecoder should set this value.
		public int codeSize;
		
		public  override bool Equals(System.Object obj)
		{
			bool isEqual = false;
			
			if (base.Equals(obj) && (obj is DefineFunction))
			{
				DefineFunction defineFunction = (DefineFunction) obj;
				
				if (equals(defineFunction.name, this.name) && ArrayUtil.equals(defineFunction.params_Renamed, this.params_Renamed) && equals(defineFunction.actionList, this.actionList) && ArrayUtil.equals(defineFunction.paramReg, this.paramReg) && (defineFunction.regCount == this.regCount) && (defineFunction.flags == this.flags))
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
