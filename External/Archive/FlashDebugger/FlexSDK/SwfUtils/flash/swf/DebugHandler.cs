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
using Flash.Swf.Debug;
using FlashUUID = Flash.Swf.Types.FlashUUID;

namespace Flash.Swf
{
	
	/// <summary> handler interface for SWD elements</summary>
	public abstract class DebugHandler : DebugTags
	{
        public abstract void header(int version);

        public abstract void uuid(FlashUUID id);
        public abstract void module(DebugModule dm);
        public abstract void offset(int offset, LineRecord lr);
        public abstract void registers(int offset, RegisterRecord r);

        public abstract void breakpoint(int offset);

        public abstract void error(String message);
	}
}
