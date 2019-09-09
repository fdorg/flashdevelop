// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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
using Location = Flash.Tools.Debugger.Location;
using SourceFile = Flash.Tools.Debugger.SourceFile;
namespace Flash.Tools.Debugger.Concrete
{
	
	public class DLocation : Location
	{
		virtual public SourceFile File
		{
			/* getters/setters */
			
			get
			{
				return m_source;
			}
			
		}
		virtual public int Line
		{
			get
			{
				return m_line;
			}
			
		}
		virtual public bool Removed
		{
			get
			{
				return m_removed;
			}
			
			set
			{
				m_removed = value;
			}
			
		}
		virtual public int Id
		{
			get
			{
				return encodeId(File.Id, Line);
			}
			
		}
		internal SourceFile m_source;
		internal int m_line;
		internal bool m_removed;
		
		internal DLocation(SourceFile src, int line)
		{
			m_source = src;
			m_line = line;
			m_removed = false;
		}
		
		/* encode /decode */
		public static int encodeId(int fileId, int line)
		{
			return ((line << 16) | fileId);
		}
		
		public static int decodeFile(long id)
		{
			return (int) (id & 0xffff);
		}
		
		public static int decodeLine(long id)
		{
			return (int) (id >> 16 & 0xffff);
		}
		
		/// <summary>for debugging </summary>
		public override String ToString()
		{
			return m_source.ToString() + ":" + m_line; //$NON-NLS-1$
		}
	}
}