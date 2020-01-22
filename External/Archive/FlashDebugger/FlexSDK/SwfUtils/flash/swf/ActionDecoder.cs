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
using Branch = Flash.Swf.Actions.Branch;
using ConstantPool = Flash.Swf.Actions.ConstantPool;
using DefineFunction = Flash.Swf.Actions.DefineFunction;
using GetURL = Flash.Swf.Actions.GetURL;
using GetURL2 = Flash.Swf.Actions.GetURL2;
using GotoFrame = Flash.Swf.Actions.GotoFrame;
using GotoFrame2 = Flash.Swf.Actions.GotoFrame2;
using GotoLabel = Flash.Swf.Actions.GotoLabel;
using Push = Flash.Swf.Actions.Push;
using SetTarget = Flash.Swf.Actions.SetTarget;
using StoreRegister = Flash.Swf.Actions.StoreRegister;
using StrictMode = Flash.Swf.Actions.StrictMode;
using Try = Flash.Swf.Actions.Try;
using Unknown = Flash.Swf.Actions.Unknown;
using WaitForFrame = Flash.Swf.Actions.WaitForFrame;
using With = Flash.Swf.Actions.With;
using DebugTable = Flash.Swf.Debug.DebugTable;
using LineRecord = Flash.Swf.Debug.LineRecord;
using RegisterRecord = Flash.Swf.Debug.RegisterRecord;
using ActionList = Flash.Swf.Types.ActionList;
using ClipActionRecord = Flash.Swf.Types.ClipActionRecord;
using ClipActions = Flash.Swf.Types.ClipActions;
namespace Flash.Swf
{
	
	public class ActionDecoder : ActionConstants
	{
		virtual public bool KeepOffsets
		{
			set
			{
				keepOffsets = value;
			}
			
		}
		private SwfDecoder reader;
		private DebugTable debug;
		private bool keepOffsets;
		private int actionCount;
		
		public ActionDecoder(SwfDecoder reader):this(reader, null)
		{
		}
		
		public ActionDecoder(SwfDecoder reader, DebugTable debug)
		{
			this.reader = reader;
			this.debug = debug;
		}
		
		/// <summary> consume actions until length bytes are used up. </summary>
		/// <param name="length">
		/// </param>
		/// <throws>  IOException </throws>
		public virtual ActionList decode(int length)
		{
			return decode(length, true);
		}
		
		/// <summary> consume actions until length bytes are used up</summary>
		/// <param name="length">
		/// </param>
		/// <param name="throwExceptions">- if false exceptions will NOT be thrown. This is 
		/// used for decoding a series of opcodes which may not be complete on their own.
		/// </param>
		/// <throws>  IOException </throws>
		public virtual ActionList decode(int length, bool throwExceptions)
		{
			int startOffset = reader.Offset;
			int end = startOffset + length;
			bool ending = false;
			
			ActionFactory factory = new ActionFactory(length, startOffset, actionCount);
			try
			{
				for (int offset = startOffset; offset < end; offset = reader.Offset)
				{
					int opcode = reader.readUI8();
					
					if (opcode > 0)
					{
						if (ending)
							throw new SwfFormatException("unexpected bytes after sactionEnd: " + opcode);
						factory.setActionOffset(actionCount, offset);
						decodeAction(opcode, offset, factory);
						actionCount++;
					}
					else if (opcode == 0)
					{
						ending = true;
					}
					else
					{
						break;
					}
				}
				// keep track of the end too
				factory.setActionOffset(actionCount, reader.Offset);
			}
			catch (System.IndexOutOfRangeException aio)
			{
				if (throwExceptions)
					throw aio;
			}
			catch (SwfFormatException swf)
			{
				if (throwExceptions)
					throw swf;
			}
			
			return factory.createActionList(keepOffsets);
		}
		
		public virtual ClipActions decodeClipActions(int length)
		{
			ClipActions a = new ClipActions();
			reader.readUI16(); // must be 0
			a.allEventFlags = decodeClipEventFlags(reader);
			
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			
			ClipActionRecord record = decodeClipActionRecord();
			while (record != null)
			{
				list.Add(record);
				record = decodeClipActionRecord();
			}
			
			a.clipActionRecords = list;
			
			return a;
		}
		
		private ClipActionRecord decodeClipActionRecord()
		{
			int flags = decodeClipEventFlags(reader);
			if (flags != 0)
			{
				ClipActionRecord c = new ClipActionRecord();
				
				c.eventFlags = flags;
				
				// this tells us how big the action block is
				int size = (int) reader.readUI32();
				
				if ((flags & ClipActionRecord.keyPress) != 0)
				{
					size--;
					c.keyCode = reader.readUI8();
				}
				
				c.actionList = decode(size);
				
				return c;
			}
			else
			{
				return null;
			}
		}
		
		private int decodeClipEventFlags(SwfDecoder r)
		{
			int flags;
			if (r.swfVersion >= 6)
				flags = (int) r.readUI32();
			else
				flags = r.readUI16();
			return flags;
		}
		
		private void  decodeAction(int opcode, int offset, ActionFactory factory)
		{
			LineRecord line = debug != null?debug.getLine(offset):null;
			if (line != null)
			{
				factory.setLine(offset, line);
			}
			
			// interleave register records in the action list
			RegisterRecord record = (debug != null)?debug.getRegisters(offset):null;
			if (record != null)
			{
				factory.setRegister(offset, record);
			}
			
			Action a;
			if (opcode < 0x80)
			{
				a = ActionFactory.createAction(opcode);
				factory.setAction(offset, a);
				return ;
			}
			
			int len = reader.readUI16();
			int pos = offset + 3;
			
			switch (opcode)
			{
				
				case Flash.Swf.ActionConstants.sactionDefineFunction: 
					a = decodeDefineFunction(pos, len);
					factory.setAction(offset, a);
					return ;
				
				
				case Flash.Swf.ActionConstants.sactionDefineFunction2: 
					a = decodeDefineFunction2(pos, len);
					factory.setAction(offset, a);
					return ;
				
				
				case Flash.Swf.ActionConstants.sactionWith: 
					a = decodeWith(factory);
					break;
				
				
				case Flash.Swf.ActionConstants.sactionTry: 
					a = decodeTry(factory);
					break;
				
				
				case Flash.Swf.ActionConstants.sactionPush: 
					Push p = decodePush(offset, pos + len, factory);
					checkConsumed(pos, len, p);
					return ;
				
				
				case Flash.Swf.ActionConstants.sactionStrictMode: 
					a = decodeStrictMode();
					break;
				
				
				case Flash.Swf.ActionConstants.sactionCall: 
					// this actions opcode has the high bit set, but there is no length.  considered a permanent bug.
					a = ActionFactory.createCall();
					break;
				
				
				case Flash.Swf.ActionConstants.sactionGotoFrame: 
					a = decodeGotoFrame();
					break;
				
				
				case Flash.Swf.ActionConstants.sactionGetURL: 
					a = decodeGetURL();
					break;
				
				
				case Flash.Swf.ActionConstants.sactionStoreRegister: 
					a = decodeStoreRegister();
					break;
				
				
				case Flash.Swf.ActionConstants.sactionConstantPool: 
					a = decodeConstantPool();
					break;
				
				
				case Flash.Swf.ActionConstants.sactionWaitForFrame: 
					a = decodeWaitForFrame(opcode, factory);
					break;
				
				
				case Flash.Swf.ActionConstants.sactionSetTarget: 
					a = decodeSetTarget();
					break;
				
				
				case Flash.Swf.ActionConstants.sactionGotoLabel: 
					a = decodeGotoLabel();
					break;
				
				
				case Flash.Swf.ActionConstants.sactionWaitForFrame2: 
					a = decodeWaitForFrame(opcode, factory);
					break;
				
				
				case Flash.Swf.ActionConstants.sactionGetURL2: 
					a = decodeGetURL2();
					break;
				
				
				case Flash.Swf.ActionConstants.sactionJump: 
				case Flash.Swf.ActionConstants.sactionIf: 
					a = decodeBranch(opcode, factory);
					break;
				
				
				case Flash.Swf.ActionConstants.sactionGotoFrame2: 
					a = decodeGotoFrame2();
					break;
				
				
				default: 
					a = decodeUnknown(opcode, len);
					break;
				
			}
			checkConsumed(pos, len, a);
			factory.setAction(offset, a);
		}
		
		private Try decodeTry(ActionFactory factory)
		{
			Try a = new Try();
			
			a.flags = reader.readUI8();
			int trySize = reader.readUI16();
			int catchSize = reader.readUI16();
			int finallySize = reader.readUI16();
			
			if (a.hasRegister())
				a.catchReg = reader.readUI8();
			else
				a.catchName = reader.readString();
			
			// we have now consumed the try action.  what follows is label mgmt
			
			int tryEnd = reader.Offset + trySize;
			a.endTry = factory.getLabel(tryEnd);
			
			// place the catchLabel to mark the end point of the catch handler
			if (a.hasCatch())
				a.endCatch = factory.getLabel(tryEnd + catchSize);
			
			// place the finallyLabel to mark the end point of the finally handler
			if (a.hasFinally())
				a.endFinally = factory.getLabel(tryEnd + finallySize + (a.hasCatch()?catchSize:0));
			
			return a;
		}
		
		private GotoFrame2 decodeGotoFrame2()
		{
			GotoFrame2 a = new GotoFrame2();
			a.playFlag = reader.readUI8();
			return a;
		}
		
		private Branch decodeBranch(int code, ActionFactory factory)
		{
			Branch a = new Branch(code);
			int offset = reader.readSI16();
			int target = offset + reader.Offset;
			a.target = factory.getLabel(target);
			return a;
		}
		
		private WaitForFrame decodeWaitForFrame(int opcode, ActionFactory factory)
		{
			WaitForFrame a = new WaitForFrame(opcode);
			if (opcode == Flash.Swf.ActionConstants.sactionWaitForFrame)
				a.frame = reader.readUI16();
			int skipCount = reader.readUI8();
			int skipTarget = actionCount + 1 + skipCount;
			factory.addSkipEntry(a, skipTarget);
			return a;
		}
		
		private GetURL2 decodeGetURL2()
		{
			GetURL2 a = new GetURL2();
			a.method = reader.readUI8();
			return a;
		}
		
		private GotoLabel decodeGotoLabel()
		{
			GotoLabel a = new GotoLabel();
			a.label = reader.readString();
			return a;
		}
		
		private SetTarget decodeSetTarget()
		{
			SetTarget a = new SetTarget();
			a.targetName = reader.readString();
			return a;
		}
		
		private ConstantPool decodeConstantPool()
		{
			ConstantPool cpool = new ConstantPool();
			int count = reader.readUI16();
			cpool.pool = new String[count];
			for (int i = 0; i < count; i++)
			{
				cpool.pool[i] = reader.readString();
			}
			return cpool;
		}
		
		private StoreRegister decodeStoreRegister()
		{
			int register = reader.readUI8();
			return ActionFactory.createStoreRegister(register);
		}
		
		private GetURL decodeGetURL()
		{
			GetURL a = new GetURL();
			a.url = reader.readString();
			a.target = reader.readString();
			return a;
		}
		
		private GotoFrame decodeGotoFrame()
		{
			GotoFrame a = new GotoFrame();
			a.frame = reader.readUI16();
			return a;
		}
		
		private Unknown decodeUnknown(int opcode, int length)
		{
			Unknown a = new Unknown(opcode);
			a.data = new byte[length];
			reader.readFully(a.data);
			return a;
		}
		
		private StrictMode decodeStrictMode()
		{
			bool mode = reader.readUI8() != 0;
			return ActionFactory.createStrictMode(mode);
		}
		
		private Push decodePush(int offset, int end, ActionFactory factory)
		{
			Push p;
			do 
			{
				int pushType = reader.readUI8();
				switch (pushType)
				{
					case Flash.Swf.ActionConstants.kPushStringType:  // string
						p = ActionFactory.createPush(reader.readString());
						break;
					
					case Flash.Swf.ActionConstants.kPushFloatType:  // float
                        byte[] floatBytes = new byte[4];
                        reader.read(floatBytes, 0, 4);
						p = ActionFactory.createPush(BitConverter.ToSingle(floatBytes, 0)); // value
						break;
					
					case Flash.Swf.ActionConstants.kPushNullType:  // null
						p = ActionFactory.createPushNull();
						break;
					
					case Flash.Swf.ActionConstants.kPushUndefinedType:  // undefined
						p = ActionFactory.createPushUndefined();
						break;
					
					case Flash.Swf.ActionConstants.kPushRegisterType:  // register
						p = ActionFactory.createPushRegister(reader.readUI8());
						break;
					
					case Flash.Swf.ActionConstants.kPushBooleanType:  // boolean
						p = ActionFactory.createPush(reader.readUI8() != 0);
						break;
					
					case Flash.Swf.ActionConstants.kPushDoubleType:  // double
						// read two 32 bit little-endian values in big-endian order.  weird.
                        byte[] doubleBytes = new byte[8];
                        reader.read(doubleBytes, 4, 4);
                        reader.read(doubleBytes, 0, 4);
                        p = ActionFactory.createPush(BitConverter.ToDouble(doubleBytes, 0));
						break;
					
					case Flash.Swf.ActionConstants.kPushIntegerType:  // integer
						p = ActionFactory.createPush((int) reader.readUI32());
						break;
					
					case Flash.Swf.ActionConstants.kPushConstant8Type:  // 8-bit cpool reference
						p = ActionFactory.createPushCpool(reader.readUI8());
						break;
					
					case Flash.Swf.ActionConstants.kPushConstant16Type:  // 16-bit cpool reference
						p = ActionFactory.createPushCpool(reader.readUI16());
						break;
					
					default: 
						throw new SwfFormatException("Unknown push data type " + pushType);
					
				}
				factory.setAction(offset, p);
				offset = reader.Offset;
			}
			while (offset < end);
			return p;
		}
		
		private DefineFunction decodeDefineFunction(int pos, int len)
		{
			DefineFunction a = new DefineFunction(Flash.Swf.ActionConstants.sactionDefineFunction);
			a.name = reader.readString();
			int number = reader.readUI16();
			a.params_Renamed = new String[number];
			
			for (int i = 0; i < number; i++)
			{
				a.params_Renamed[i] = reader.readString();
			}
			
			a.codeSize = reader.readUI16();
			
			checkConsumed(pos, len, a);
			
			a.actionList = decode(a.codeSize);
			
			return a;
		}
		
		private DefineFunction decodeDefineFunction2(int pos, int len)
		{
			DefineFunction a = new DefineFunction(Flash.Swf.ActionConstants.sactionDefineFunction2);
			a.name = reader.readString();
			int number = reader.readUI16();
			a.params_Renamed = new String[number];
			a.paramReg = new int[number];
			
			a.regCount = reader.readUI8();
			a.flags = reader.readUI16();
			
			for (int i = 0; i < number; i++)
			{
				a.paramReg[i] = reader.readUI8();
				a.params_Renamed[i] = reader.readString();
			}
			
			a.codeSize = reader.readUI16();
			
			checkConsumed(pos, len, a);
			
			a.actionList = decode(a.codeSize);
			
			return a;
		}
		
		private void  checkConsumed(int pos, int len, Action a)
		{
			int consumed = reader.Offset - pos;
			if (consumed != len)
			{
				throw new SwfFormatException(a.GetType().FullName + ": " + consumed + " was read. " + len + " was required.");
			}
		}
		
		private With decodeWith(ActionFactory factory)
		{
			With a = new With();
			int size = reader.readUI16();
			int target = size + reader.Offset;
			a.endWith = factory.getLabel(target);
			return a;
		}
	}
}
