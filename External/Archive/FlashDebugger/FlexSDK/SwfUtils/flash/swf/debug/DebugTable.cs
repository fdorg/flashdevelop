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
using DebugHandler = Flash.Swf.DebugHandler;
using FlashUUID = Flash.Swf.Types.FlashUUID;
using IntMap = Flash.Util.IntMap;
namespace Flash.Swf.Debug
{
	
	/// <summary> info gleaned from a debuggable flash movie (SWF+SWD)</summary>
	/// <author>  Edwin Smith
	/// </author>
	public class DebugTable : DebugHandler
	{
		public FlashUUID uuid_Renamed_Field;
		public int version;
		public IntMap lines;
		public IntMap registers_Renamed_Field;
		
		public DebugTable()
		{
			lines = new IntMap();
			registers_Renamed_Field = new IntMap();
		}

        public override void breakpoint(int offset)
		{
		}

        public override void header(int version)
		{
			this.version = version;
		}

        public override void module(DebugModule dm)
		{
		}

        public override void offset(int offset, LineRecord lr)
		{
			lines.put(offset, lr);
		}

        public override void registers(int offset, RegisterRecord r)
		{
			registers_Renamed_Field.put(offset, r);
		}

        public override void uuid(FlashUUID id)
		{
			this.uuid_Renamed_Field = id;
		}
		
		public virtual LineRecord getLine(int offset)
		{
			return (LineRecord) lines.get_Renamed(offset);
		}
		
		public virtual RegisterRecord getRegisters(int offset)
		{
			return (RegisterRecord) registers_Renamed_Field.get_Renamed(offset);
		}

        public override void error(String msg)
		{
		}
	}
}
