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
using ArrayUtil = Flash.Util.ArrayUtil;
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class ActionList:ActionHandler
	{
		// start numbering internal opcodes at 256 to make sure we wont
		// collide with a real player opcode.  player opcodes are 8-bit.
		public const int sactionLabel = 256;
		public const int sactionLineRecord = 257;
		public const int sactionRegisterRecord = 258;
		
		public ActionList():this(false)
		{
		}
		
		public ActionList(int capacity):this(false, capacity)
		{
		}
		
		public ActionList(bool keepOffsets):this(keepOffsets, 10)
		{
		}
		
		public ActionList(bool keepOffsets, int capacity)
		{
			if (keepOffsets)
				offsets = new int[capacity];
			actions = new Action[capacity];
			size_Renamed_Field = 0;
		}
		
		private int[] offsets;
		private Action[] actions;
		private int size_Renamed_Field;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is ActionList)
			{
				ActionList actionList = (ActionList) object_Renamed;
				
				if (ArrayUtil.equals(actionList.actions, this.actions))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public virtual void  visitAll(ActionHandler handler)
		{
			visit(handler, 0, size_Renamed_Field - 1);
		}
		
		public virtual void  visit(ActionHandler handler, int startIndex, int endIndex)
		{
			endIndex = (endIndex < 0)?size_Renamed_Field - 1:endIndex;
			for (int j = startIndex; j <= endIndex; j++)
			{
				Action a = actions[j];
				if (a.code != sactionLabel && a.code != sactionLineRecord)
				{
					// don't call this for labels
					if (offsets != null)
						handler.setActionOffset(offsets[j], a);
					else
						handler.setActionOffset(j, a);
				}
				a.visit(handler);
			}
		}
		
		public override void  setActionOffset(int offset, Action a)
		{
			insert(offset, a);
		}
		
		public virtual void  grow(int capacity)
		{
			if (offsets != null)
			{
				int[] newoffsets = new int[capacity];
				Array.Copy(offsets, 0, newoffsets, 0, size_Renamed_Field);
				offsets = newoffsets;
			}
			
			Action[] newactions = new Action[capacity];
			Array.Copy(actions, 0, newactions, 0, size_Renamed_Field);
			actions = newactions;
		}
		
		public virtual int size()
		{
			return size_Renamed_Field;
		}
		
		public virtual Action getAction(int i)
		{
			return actions[i];
		}
		
		public virtual int getOffset(int i)
		{
			return offsets[i];
		}
		
		public virtual void  remove(int i)
		{
			if (offsets != null)
				Array.Copy(offsets, i + 1, offsets, i, size_Renamed_Field - i - 1);
			Array.Copy(actions, i + 1, actions, i, size_Renamed_Field - i - 1);
			size_Renamed_Field--;
		}
		
		/// <summary> perform a binary search to find the requested offset.</summary>
		/// <param name="k">
		/// </param>
		/// <returns> the index where that offset is found, or -(ins+1) if
		/// the key is not found, and ins is the insertion index.
		/// 
		/// </returns>
		private int find(int k)
		{
			if (offsets != null)
			{
				int lo = 0;
				int hi = size_Renamed_Field - 1;
				
				while (lo <= hi)
				{
					int i = (lo + hi) / 2;
					int m = offsets[i];
					if (k > m)
						lo = i + 1;
					else if (k < m)
						hi = i - 1;
					else
						return i; // key found
				}
				return - (lo + 1); // key not found, lo is the insertion point
			}
			else
			{
				return k;
			}
		}
		
		public virtual void  insert(int offset, Action a)
		{
			if (size_Renamed_Field == actions.Length)
				grow(size_Renamed_Field * 2);
			int i;
			if (size_Renamed_Field == 0 || offsets == null && offset == size_Renamed_Field || offsets != null && offset > offsets[size_Renamed_Field - 1])
			{
				// appending.
				i = size_Renamed_Field;
			}
			else
			{
				i = find(offset);
				if (i < 0)
				{
					// offset not used yet.  compute insertion point
					i = - i - 1;
				}
				else
				{
					// offset already used.  if we are inserting a real action, make it be last
					if (a.code < 256)
					{
						// this is a real action, we want it to be last at this offset
						while (i < size_Renamed_Field && offsets[i] == offset)
							i++;
					}
				}
				if (offsets != null)
					Array.Copy(offsets, i, offsets, i + 1, size_Renamed_Field - i);
				Array.Copy(actions, i, actions, i + 1, size_Renamed_Field - i);
			}
			if (offsets != null)
				offsets[i] = offset;
			actions[i] = a;
			size_Renamed_Field++;
		}
		
		public virtual void  append(Action a)
		{
			int i = size_Renamed_Field;
			if (i == actions.Length)
				grow(size_Renamed_Field * 2);
			actions[i] = a;
			size_Renamed_Field = i + 1;
		}
		
		public override String ToString()
		{
			System.Text.StringBuilder stringBuffer = new System.Text.StringBuilder();
			
			stringBuffer.Append("ActionList: count = " + actions.Length);
			stringBuffer.Append(", actions = ");
			
			for (int i = 0; i < size_Renamed_Field; i++)
			{
				stringBuffer.Append(actions[i]);
			}
			
			return stringBuffer.ToString();
		}
		
		/// <summary> Return the index within this action list of the first
		/// occurence of the specified actionCode, searching foward
		/// starting at the given index
		/// </summary>
		public virtual int indexOf(int actionCode, int startAt)
		{
			int at = - 1;
			for (int i = startAt; at < 0 && i < actions.Length; i++)
			{
				Action a = getAction(i);
				if (a != null && a.code == actionCode)
					at = i;
			}
			return at;
		}
		
		/// <summary> Return the index within this action list of the first
		/// occurence of the specified actionCode, searching backward
		/// starting at the given index
		/// </summary>
		public virtual int lastIndexOf(int actionCode, int startAt)
		{
			int at = - 1;
			for (int i = startAt; at < 0 && i >= 0; i--)
			{
				Action a = getAction(i);
				if (a != null && a.code == actionCode)
					at = i;
			}
			return at;
		}
		
		// specialized indexOf and lastIndexOf that start at the beginning and end of the actions list respectively
		public virtual int indexOf(int actionCode)
		{
			return indexOf(actionCode, 0);
		}
		public virtual int lastIndexOf(int actionCode)
		{
			return lastIndexOf(actionCode, actions.Length - 1);
		}
		
		public virtual void  setAction(int i, Action action)
		{
			actions[i] = action;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
