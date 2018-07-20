// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2003-2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;

namespace Flash.Tools.Debugger.Expression
{
	
	/// <summary> Abstract implementation of the ValueExp interface which 
	/// does not allow the setting of children.
	/// </summary>
	public abstract class TerminalExp : ValueExp
	{
		virtual public ValueExp LeftChild
		{
			/* sets the left hand side child to the given node */
			
			set
			{
				throw new NoChildException("left");
			}
			
		}
		virtual public ValueExp RightChild
		{
			/* sets the right hand side child to the given node */
			
			set
			{
				throw new NoChildException("right");
			}
			
		}
		
		/* are we an instanceof something */
		public virtual bool containsInstanceOf(Type c)
		{
			return (c.IsInstanceOfType(this));
		}
		
		public virtual bool hasSideEffectsOtherThanGetters()
		{
			return false;
		}
		public abstract Object evaluate(Context param1);
	}
}
