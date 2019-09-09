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
using Action = Flash.Swf.Action;
using ActionHandler = Flash.Swf.ActionHandler;
using ActionList = Flash.Swf.Types.ActionList;
namespace Flash.Swf.Debug
{
	
	public class RegisterRecord:Action
	{
		public RegisterRecord(int offset, int numRegisters):base(ActionList.sactionRegisterRecord)
		{
			int size = numRegisters;
			registerNumbers = new int[size];
			variableNames = new String[size];
			this.offset = offset;
			next = 0;
		}
		
		public int[] registerNumbers;
		public String[] variableNames;
		public int offset;
		
		// internal use for addRegister()
		internal int next;
		
		/// <summary> Used to add a register entry into this record</summary>
		public virtual void  addRegister(int regNbr, String variableName)
		{
			registerNumbers[next] = regNbr;
			variableNames[next] = variableName;
			next++;
		}
		
		public virtual int indexOf(int regNbr)
		{
			int at = - 1;
			for (int i = 0; at < 0 && i < registerNumbers.Length; i++)
			{
				if (registerNumbers[i] == regNbr)
					at = i;
			}
			return at;
		}
		
		public override void  visit(ActionHandler h)
		{
			h.registerRecord(this);
		}
		
		public override String ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(offset);
			sb.Append(" ");
			for (int i = 0; i < registerNumbers.Length; i++)
				sb.Append("$" + registerNumbers[i] + "='" + variableNames[i] + "' ");
			return (sb.ToString());
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isIt = (object_Renamed is RegisterRecord);
			if (isIt)
			{
				RegisterRecord other = (RegisterRecord) object_Renamed;
				isIt = base.Equals(other);
				for (int i = 0; isIt && i < registerNumbers.Length; i++)
				{
					isIt = ((other.registerNumbers[i] == this.registerNumbers[i]) && ((System.Object) other.variableNames[i] == (System.Object) this.variableNames[i]))?isIt:false;
				}
			}
			return isIt;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
