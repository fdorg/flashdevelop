// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
using Watch = Flash.Tools.Debugger.Watch;
namespace Flash.Tools.Debugger.Concrete
{
	
	/// <summary> Holder of Watchpoint information</summary>
	public class DWatch : Watch
	{
		virtual public int ValueId
		{
			get
			{
				return m_parentValueId;
			}
			
		}
		virtual public String MemberName
		{
			get
			{
				return m_rawMemberName;
			}
			
		}
		virtual public int Kind
		{
			get
			{
				return m_kind;
			}
			
		}
		virtual public int Tag
		{
			get
			{
				return m_tag;
			}
			
		}
		internal int m_parentValueId;
		internal String m_rawMemberName; // corresponds to Variable.getRawName()
		internal int m_kind;
		internal int m_tag;
		
		public DWatch(int id, String name, int kind, int tag)
		{
			m_parentValueId = id;
			m_rawMemberName = name;
			m_kind = kind;
			m_tag = tag;
		}
	}
}