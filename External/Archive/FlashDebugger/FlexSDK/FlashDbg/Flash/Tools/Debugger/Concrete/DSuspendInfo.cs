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
using SuspendReason = Flash.Tools.Debugger.SuspendReason;
namespace Flash.Tools.Debugger.Concrete
{
	
	/// <summary> The suspend information object returns information about the
	/// current halted state of the Player.
	/// </summary>
	public class DSuspendInfo
	{
		virtual public int Reason
		{
			get
			{
				return m_reason;
			}
			
		}
		virtual public int ActionIndex
		{
			get
			{
				return m_actionIndex;
			}
			
		}
		virtual public int Offset
		{
			get
			{
				return m_offset;
			}
			
		}
		virtual public int PreviousOffset
		{
			get
			{
				return m_previousOffset;
			}
			
		}
		virtual public int NextOffset
		{
			get
			{
				return m_nextOffset;
			}
			
		}
		internal int m_reason;
		internal int m_actionIndex; // which script caused the halt
		internal int m_offset; // offset into the actions that the player has halted
		internal int m_previousOffset; // previous offset, if any, which lies on the same source line (-1 means unknown)
		internal int m_nextOffset; // next offset, if any, which lies on the same source line (-1 means unknown)
		
		public DSuspendInfo()
		{
			m_reason = Flash.Tools.Debugger.SuspendReason.Unknown;
			m_actionIndex = - 1;
			m_offset = - 1;
			m_previousOffset = - 1;
			m_nextOffset = - 1;
		}
		
		public DSuspendInfo(int reason, int actionIndex, int offset, int previousOffset, int nextOffset)
		{
			m_reason = reason;
			m_actionIndex = actionIndex;
			m_offset = offset;
			m_previousOffset = previousOffset;
			m_nextOffset = nextOffset;
		}
	}
}