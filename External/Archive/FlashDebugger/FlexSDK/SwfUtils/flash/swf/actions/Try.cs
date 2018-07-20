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
using ActionConstants = Flash.Swf.ActionConstants;
namespace Flash.Swf.Actions
{
	
	public class Try:Action
	{
		public Try():base(Flash.Swf.ActionConstants.sactionTry)
		{
		}
		
		public override void  visit(ActionHandler h)
		{
			h.tryAction(this);
		}
		
		public int flags;
		
		/// <summary>name of variable holding the error object in catch body </summary>
		public String catchName;
		
		/// <summary>register that holds the catch variable </summary>
		public int catchReg;
		
		/// <summary>marks end of body and start of catch handler </summary>
		public Label endTry;
		/// <summary>marks end of catch handler and start of finally handler </summary>
		public Label endCatch;
		/// <summary>marks end of finally handler </summary>
		public Label endFinally;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is Try))
			{
				Try tryAction = (Try) object_Renamed;
				
				if ((tryAction.flags == this.flags) && equals(tryAction.catchName, this.catchName) && (tryAction.catchReg == this.catchReg) && tryAction.endTry == this.endTry && tryAction.endCatch == this.endCatch && tryAction.endFinally == this.endFinally)
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public virtual bool hasCatch()
		{
			return (flags & Flash.Swf.ActionConstants.kTryHasCatchFlag) != 0;
		}
		
		public virtual bool hasFinally()
		{
			return (flags & Flash.Swf.ActionConstants.kTryHasFinallyFlag) != 0;
		}
		
		public virtual bool hasRegister()
		{
			return (flags & Flash.Swf.ActionConstants.kTryCatchRegisterFlag) != 0;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
