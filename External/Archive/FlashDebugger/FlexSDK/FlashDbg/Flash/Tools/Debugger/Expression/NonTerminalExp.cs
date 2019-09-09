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
	/// allows the setting of children.
	/// </summary>
	public abstract class NonTerminalExp : ValueExp
	{
		virtual public ValueExp LeftChild
		{
			/* sets the left hand side child to the given node */
			
			set
			{
				m_left = value;
			}
			
		}
		virtual public ValueExp RightChild
		{
			/* sets the right hand side child to the given node */
			
			set
			{
				m_right = value;
			}
			
		}
		internal ValueExp m_right;
		internal ValueExp m_left;
		
		/* traverses the children asking each one if they are an instanceof something */
		public virtual bool containsInstanceOf(Type c)
		{
			bool yes = false;
			if (c.IsInstanceOfType(this))
				yes = true;
			else if (m_left != null && m_left.containsInstanceOf(c))
				yes = true;
			else if (m_right != null && m_right.containsInstanceOf(c))
				yes = true;
			return yes;
		}
		
		public virtual bool hasSideEffectsOtherThanGetters()
		{
			return containsInstanceOf(typeof(AssignmentExp));
		}
		public abstract Object evaluate(Context param1);
	}
}
