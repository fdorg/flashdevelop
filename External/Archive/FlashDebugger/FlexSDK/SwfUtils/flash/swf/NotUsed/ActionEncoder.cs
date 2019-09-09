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
using Branch = flash.swf.actions.Branch;
using ConstantPool = flash.swf.actions.ConstantPool;
using DefineFunction = flash.swf.actions.DefineFunction;
using GetURL = flash.swf.actions.GetURL;
using GetURL2 = flash.swf.actions.GetURL2;
using GotoFrame = flash.swf.actions.GotoFrame;
using GotoFrame2 = flash.swf.actions.GotoFrame2;
using GotoLabel = flash.swf.actions.GotoLabel;
using Label = flash.swf.actions.Label;
using Push = flash.swf.actions.Push;
using SetTarget = flash.swf.actions.SetTarget;
using StoreRegister = flash.swf.actions.StoreRegister;
using StrictMode = flash.swf.actions.StrictMode;
using Try = flash.swf.actions.Try;
using Unknown = flash.swf.actions.Unknown;
using WaitForFrame = flash.swf.actions.WaitForFrame;
using With = flash.swf.actions.With;
using LineRecord = flash.swf.debug.LineRecord;
using RegisterRecord = flash.swf.debug.RegisterRecord;
using ActionList = flash.swf.types.ActionList;
using ClipActionRecord = flash.swf.types.ClipActionRecord;
using ClipActions = flash.swf.types.ClipActions;
namespace flash.swf
{
	
	public class ActionEncoder:ActionHandler
	{
		override public SetTarget Target
		{
			set
			{
				int updatePos = encodeActionHeader(value);
				writer.writeString(value.targetName);
				updateActionHeader(updatePos);
			}
			
		}
		private SwfEncoder writer;
		private DebugEncoder debug;
		
		// map label -> position
		//UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
		private System.Collections.Hashtable labels;
		
		private System.Collections.ArrayList updates;
		private int actionCount;
		
		public ActionEncoder(SwfEncoder writer, DebugEncoder debug)
		{
			this.writer = writer;
			this.debug = debug;
			//UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
			labels = new System.Collections.Hashtable();
			updates = new System.Collections.ArrayList();
		}
		
		private class UpdateEntry
		{
			internal int anchor; // location to subtract to compute delta
			internal int updatePos; // byte offset to update with delta value
			internal Action source;
			
			public UpdateEntry(int anchor, int updatePos, Action source)
			{
				this.anchor = anchor;
				this.updatePos = updatePos;
				this.source = source;
			}
		}
		
		private class LabelEntry
		{
			internal int offset; // byte offset in swf file
			internal int count; // action ordinal number (Nth action in current block)
			
			public LabelEntry(int offset, int count)
			{
				this.count = count;
				this.offset = offset;
			}
		}
		
		private int getLabelOffset(Label label)
		{
			Debug.Assert(labels.containsKey(label), "missing label");
			//UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
			return ((LabelEntry) labels[label]).offset;
		}
		
		private int getLabelCount(Label label)
		{
			Debug.Assert(labels.containsKey(label), "missing label");
			//UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
			return ((LabelEntry) labels[label]).count;
		}
		
		public virtual void  encode(ActionList actionList)
		{
			// write the actions
			for (int i = 0; i < actionList.size(); i++)
			{
				Action a = actionList.getAction(i);
				
				switch (a.code)
				{
					
					// don't update actionCount for synthetic opcodes
					case ActionList.sactionLabel: 
						a.visit(this);
						break;
					
					case ActionList.sactionLineRecord: 
						if (debug != null)
							debug.offset(writer.Pos, (LineRecord) a);
						break;
					
					
					case ActionList.sactionRegisterRecord: 
						if (debug != null)
							debug.registers(writer.Pos, (RegisterRecord) a);
						break;
						
						// the remaining types need counting
					
					case flash.swf.ActionConstants_Fields.sactionPush: 
						i = encodePush((Push) a, i, actionList);
						actionCount++;
						break;
					
					default: 
						if (a.code < 0x80)
							writer.writeUI8(a.code);
						else
							a.visit(this);
						actionCount++;
						break;
					
				}
			}
			
			patchForwardBranches();
		}
		
		private void  patchForwardBranches()
		{
			// now patch forward branches
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			for (System.Collections.IEnumerator i = updates.GetEnumerator(); i.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				UpdateEntry entry = (UpdateEntry) i.Current;
				switch (entry.source.code)
				{
					
					case flash.swf.ActionConstants_Fields.sactionIf: 
					case flash.swf.ActionConstants_Fields.sactionJump: 
						int target = getLabelOffset(((Branch) entry.source).target);
						writer.writeSI16at(entry.updatePos, target - entry.anchor);
						break;
					
					case flash.swf.ActionConstants_Fields.sactionWith: 
						int endWith = getLabelOffset(((With) entry.source).endWith);
						writer.writeUI16at(entry.updatePos, endWith - entry.anchor);
						break;
					
					case flash.swf.ActionConstants_Fields.sactionWaitForFrame: 
					case flash.swf.ActionConstants_Fields.sactionWaitForFrame2: 
						int skipTarget = getLabelCount(((WaitForFrame) entry.source).skipTarget);
						writer.writeUI8at(entry.updatePos, skipTarget - entry.anchor);
						break;
					
					case flash.swf.ActionConstants_Fields.sactionTry: 
						Try t = (Try) entry.source;
						int endTry = getLabelOffset(t.endTry);
						writer.writeUI16at(entry.updatePos, endTry - entry.anchor);
						entry.anchor = endTry;
						if (t.hasCatch())
						{
							int endCatch = getLabelOffset(t.endCatch);
							writer.writeUI16at(entry.updatePos + 2, endCatch - entry.anchor);
							entry.anchor = endCatch;
						}
						if (t.hasFinally())
						{
							int endFinally = getLabelOffset(t.endFinally);
							writer.writeUI16at(entry.updatePos + 4, endFinally - entry.anchor);
						}
						break;
					
					default: 
						Debug.Assert(false, "invalid action in UpdateEntry");
						break;
					
				}
			}
		}
		
		//    public void inlineBinaryOp(InlineBinaryOp op)
		//    {
		//        writer.writeU8(op.code);
		//        writer.writeU8(op.dst);
		//        encodeOperand(op.lhs, op.rhs);
		//    }
		
		//    private void encodeOperand(Object lhs, Object rhs)
		//    {
		//        int lhsType = operandType(lhs);
		//        int rhsType = operandType(rhs);
		//        writer.writeU8(lhsType >> 4 | rhsType);
		//        encodeOperand(lhsType, lhs);
		//        encodeOperand(rhsType, rhs);
		//    }
		//
		//    private void encodeOperand(Object operand)
		//    {
		//        int type = operandType(operand);
		//        writer.writeU8(type);
		//        encodeOperand(type, operand);
		//    }
		//
		//    public void inlineBranchWhenFalse(InlineBranchWhenFalse op)
		//    {
		//        writer.writeU8(op.code);
		//        int pos = writer.getPos();
		//        writer.writeU8(op.cond);
		//        encodeOperand(op.lhs, op.rhs);
		//        Integer offset = (Integer) labels.get(op.target);
		//        if (offset != null)
		//        {
		//            // label came earlier
		//            writer.writeU16(offset.intValue() - pos - 2);
		//        }
		//        else
		//        {
		//            // label comes later. don't know the offset yet.
		//			updates.add(new UpdateEntry(pos+2, pos, op));
		//            writer.writeU16(0);
		//        }
		//    }
		
		//    public void inlineGetMember(InlineGetMember op)
		//    {
		//        writer.writeU8(op.code);
		//        writer.writeU8(op.dst);
		//        writer.writeU8(op.src);
		//        encodeOperand(op.member);
		//    }
		//
		//    public void inlineSetMember(InlineSetMember op)
		//    {
		//        writer.writeU8(op.code);
		//        writer.writeU8(op.dst);
		//        encodeOperand(op.member, op.src);
		//    }
		
		//    public void inlineSetRegister(InlineSetRegister op)
		//    {
		//        writer.writeU8(op.code);
		//        writer.writeU8(op.dst);
		//        encodeOperand(op.value);
		//    }
		//
		//    public void inlineUnaryRegOp(InlineUnaryRegOp op)
		//    {
		//        writer.writeU8(op.code);
		//        writer.writeU8(op.register);
		//    }
		
		//    private void encodeOperand(int opType, Object operand)
		//    {
		//        switch (opType)
		//        {
		//        case ActionConstants.kInlineTrue:
		//        case ActionConstants.kInlineFalse:
		//        case ActionConstants.kInlineNull:
		//        case ActionConstants.kInlineUndefined:
		//            // do nothing, type implies value
		//            break;
		//        case ActionConstants.kInlineConstantByte:
		//        case ActionConstants.kInlineChar:
		//        case ActionConstants.kInlineRegister:
		//            int i = ((Number)operand).intValue();
		//            writer.writeU8(i);
		//            break;
		//        case ActionConstants.kInlineConstantWord:
		//        case ActionConstants.kInlineShort:
		//            i = ((Number)operand).intValue();
		//            writer.writeU16(i);
		//            break;
		//        case ActionConstants.kInlineLong:
		//            i = ((Number)operand).intValue();
		//            writer.write32(i);
		//            break;
		//        case ActionConstants.kInlineDouble:
		//            long num = Double.doubleToLongBits(((Number)operand).doubleValue());
		//            writer.write32((int)(num>>32));
		//            writer.write32((int)num);
		//            break;
		//        }
		//    }
		
		//    private int operandType(Object operand)
		//    {
		//        if (operand == Boolean.TRUE)
		//            return ActionConstants.kInlineTrue;
		//        else if (operand == Boolean.FALSE)
		//            return ActionConstants.kInlineFalse;
		//        else if (operand == ActionFactory.UNDEFINED)
		//            return ActionConstants.kInlineUndefined;
		//        else if (operand == ActionFactory.STACKTOP)
		//            return ActionConstants.kInlineStack;
		//        else if (operand == null)
		//            return ActionConstants.kInlineNull;
		//        else if (operand instanceof Short)
		//            return ((Short)operand).intValue() < 256 ? ActionConstants.kInlineConstantByte : ActionConstants.kInlineConstantWord;
		//        else if (operand instanceof Double)
		//            return ActionConstants.kInlineDouble;
		//        else if (operand instanceof Integer)
		//            return (((Integer)operand).intValue() & ~0xFF) == 0 ? ActionConstants.kInlineChar :
		//                   (((Integer)operand).intValue() & ~0xFFFF) == 0 ? ActionConstants.kInlineShort :
		//                    ActionConstants.kInlineLong;
		//        else if (operand instanceof Byte)
		//            return ActionConstants.kInlineRegister;
		//        else
		//            throw new IllegalArgumentException("unknown operand type " + operand.getClass().getName());
		//    }
		
		public override void  call(Action action)
		{
			writer.writeUI8(action.code);
			// this action's opcode 0x9E has hi bit set, but it never has data.  considered a permanent minor bug.
			writer.writeUI16(0);
		}
		
		public override void  constantPool(ConstantPool action)
		{
			int updatePos = encodeActionHeader(action);
			writer.writeUI16(action.pool.Length);
			for (int i = 0; i < action.pool.Length; i++)
			{
				writer.writeString(action.pool[i]);
			}
			updateActionHeader(updatePos);
		}
		
		private void  updateActionHeader(int updatePos)
		{
			int length = (writer.Pos - updatePos) - 2;
			if (length >= 0x10000)
			{
				Debug.Assert(false, "action length (" + length + ") exceeds 64K");
			}
			writer.writeUI16at(updatePos, length);
		}
		
		private int encodeActionHeader(Action action)
		{
			writer.writeUI8(action.code);
			int updatePos = writer.Pos;
			writer.writeUI16(0);
			return updatePos;
		}
		
		public override void  defineFunction(DefineFunction action)
		{
			int updatePos = encodeActionHeader(action);
			writer.writeString(action.name);
			writer.writeUI16(action.params_Renamed.Length);
			for (int i = 0; i < action.params_Renamed.Length; i++)
			{
				writer.writeString(action.params_Renamed[i]);
			}
			int pos = writer.Pos;
			writer.writeUI16(0); // codesize placeholder
			updateActionHeader(updatePos);
			
			new ActionEncoder(writer, debug).encode(action.actionList);
			
			writer.writeUI16at(pos, (writer.Pos - pos) - 2);
		}
		
		public override void  defineFunction2(DefineFunction action)
		{
			int updatePos = encodeActionHeader(action);
			writer.writeString(action.name);
			writer.writeUI16(action.params_Renamed.Length);
			writer.writeUI8(action.regCount);
			writer.writeUI16(action.flags);
			
			for (int i = 0; i < action.params_Renamed.Length; i++)
			{
				writer.writeUI8(action.paramReg[i]);
				writer.writeString(action.params_Renamed[i]);
			}
			
			int pos = writer.Pos;
			writer.writeUI16(0); // placeholder
			updateActionHeader(updatePos);
			
			new ActionEncoder(writer, debug).encode(action.actionList);
			
			writer.writeUI16at(pos, (writer.Pos - pos) - 2);
		}
		
		public override void  getURL(GetURL action)
		{
			int updatePos = encodeActionHeader(action);
			writer.writeString(action.url);
			writer.writeString(action.target);
			updateActionHeader(updatePos);
		}
		
		public override void  getURL2(GetURL2 action)
		{
			int updatePos = encodeActionHeader(action);
			writer.writeUI8(action.method);
			updateActionHeader(updatePos);
		}
		
		public override void  gotoFrame(GotoFrame action)
		{
			int updatePos = encodeActionHeader(action);
			writer.writeUI16(action.frame);
			updateActionHeader(updatePos);
		}
		
		public override void  gotoFrame2(GotoFrame2 action)
		{
			int updatePos = encodeActionHeader(action);
			writer.writeUI8(action.playFlag);
			updateActionHeader(updatePos);
		}
		
		public override void  gotoLabel(GotoLabel action)
		{
			int updatePos = encodeActionHeader(action);
			writer.writeString(action.label);
			updateActionHeader(updatePos);
		}
		
		public override void  ifAction(Branch action)
		{
			encodeBranch(action);
		}
		
		public override void  jump(Branch action)
		{
			encodeBranch(action);
		}
		
		private void  encodeBranch(Branch branch)
		{
			writer.writeUI8(branch.code);
			writer.writeUI16(2);
			int pos = writer.Pos;
			if (labels.ContainsKey(branch.target))
			{
				// label came earlier
				writer.writeSI16(getLabelOffset(branch.target) - pos - 2);
			}
			else
			{
				// label comes later. don't know the offset yet.
				updates.Add(new UpdateEntry(pos + 2, pos, branch));
				writer.writeSI16(0);
			}
		}
		
		public override void  with(With action)
		{
			writer.writeUI8(action.code);
			writer.writeUI16(2);
			// label comes later, don't know offset yet
			int pos = writer.Pos;
			updates.Add(new UpdateEntry(pos + 2, pos, action));
			writer.writeUI16(0);
		}
		
		public override void  waitForFrame(WaitForFrame action)
		{
			writer.writeUI8(action.code);
			writer.writeUI16(3);
			writer.writeUI16(action.frame);
			int pos = writer.Pos;
			updates.Add(new UpdateEntry(actionCount + 1, pos, action));
			writer.writeUI8(0);
		}
		
		public override void  waitForFrame2(WaitForFrame action)
		{
			writer.writeUI8(action.code);
			writer.writeUI16(1);
			int pos = writer.Pos;
			updates.Add(new UpdateEntry(actionCount + 1, pos, action));
			writer.writeUI8(0);
		}
		
		public override void  label(Label label)
		{
			Debug.Assert(!labels.containsKey(label), "found duplicate label");
			int labelPos = writer.Pos;
			labels[label] = new LabelEntry(labelPos, actionCount);
		}
		
		/// <summary> encode a run of push actions into one action record.  The player
		/// supports this compact encoding since push is such a common
		/// opcode.  the format is:
		/// 
		/// sactionPush type1 value1 type2 value2 ...
		/// 
		/// </summary>
		/// <param name="push">
		/// </param>
		/// <param name="j">the index of the starting push action
		/// </param>
		/// <param name="actions">
		/// </param>
		/// <returns> the index of the last push action encoded.  the next action will
		/// not be a push action.
		/// </returns>
		public virtual int encodePush(Push push, int j, ActionList actions)
		{
			int updatePos = encodeActionHeader(push);
			do 
			{
				System.Object value_Renamed = push.value_Renamed;
				int type = Push.getTypeCode(value_Renamed);
				writer.writeUI8(type);
				
				switch (type)
				{
					
					case 0:  // string
						//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
						writer.writeString(value_Renamed.ToString());
						break;
					
					case 1:  // float
						//UPGRADE_ISSUE: Method 'java.lang.Float.floatToIntBits' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangFloatfloatToIntBits_float'"
						//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Float.floatValue' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
						int bits = Float.floatToIntBits((float) ((System.Single) value_Renamed));
						writer.write32(bits);
						break;
					
					case 2:  // null
						break;
					
					case 3:  // undefined
						break;
					
					case 4:  // register
						writer.writeUI8((int) ((System.SByte) value_Renamed) & 0xFF);
						break;
					
					case 5:  // boolean
						writer.writeUI8(((System.Boolean) value_Renamed)?1:0);
						break;
					
					case 6:  // double
						double d = ((System.Double) value_Renamed);
						//UPGRADE_ISSUE: Method 'java.lang.Double.doubleToLongBits' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangDoubledoubleToLongBits_double'"
						long num = Double.doubleToLongBits(d);
						writer.write32((int) (num >> 32));
						writer.write32((int) num);
						break;
					
					case 7:  // integer
						writer.write32(((System.Int32) value_Renamed));
						break;
					
					case 8:  // const8
						writer.writeUI8((int) ((System.Int16) value_Renamed));
						break;
					
					case 9:  // const16
						writer.writeUI16((int) ((System.Int16) value_Renamed) & 0xFFFF);
						break;
					}
				
				if (debug == null)
				{
					// ignore line records if we aren't debugging
					while (j + 1 < actions.size() && actions.getAction(j + 1).code == ActionList.sactionLineRecord)
						j++;
				}
				
				Action a;
				if (++j < actions.size() && (a = actions.getAction(j)).code == flash.swf.ActionConstants_Fields.sactionPush)
				{
					push = (Push) a;
				}
				else
				{
					push = null;
				}
			}
			while (push != null);
			updateActionHeader(updatePos);
			return j - 1;
		}
		
		public override void  storeRegister(StoreRegister action)
		{
			int updatePos = encodeActionHeader(action);
			writer.writeUI8(action.register);
			updateActionHeader(updatePos);
		}
		
		public override void  strictMode(StrictMode action)
		{
			int updatePos = encodeActionHeader(action);
			writer.writeUI8(action.mode?1:0);
			updateActionHeader(updatePos);
		}
		
		public override void  tryAction(Try a)
		{
			int updatePos = encodeActionHeader(a);
			
			writer.writeUI8(a.flags);
			int trySizePos = writer.Pos;
			writer.writeUI16(0); // try size
			writer.writeUI16(0); // catch size
			writer.writeUI16(0); // finally size
			
			if (a.hasRegister())
				writer.writeUI8(a.catchReg);
			else
				writer.writeString(a.catchName);
			
			// we have emitted the try action, what follows is label mgmt
			updateActionHeader(updatePos);
			
			int tryStart = writer.Pos;
			updates.Add(new UpdateEntry(tryStart, trySizePos, a));
		}
		
		public override void  unknown(Unknown action)
		{
			int updatePos = encodeActionHeader(action);
			writer.write(action.data);
			updateActionHeader(updatePos);
		}
		
		public virtual void  encodeClipActions(ClipActions clipActions)
		{
			writer.writeUI16(0);
			
			encodeClipEventFlags(clipActions.allEventFlags, writer);
			
			System.Collections.IEnumerator it = clipActions.clipActionRecords.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				ClipActionRecord r = (ClipActionRecord) it.Current;
				encodeClipActionRecord(r);
			}
			
			if (writer.swfVersion >= 6)
				writer.write32(0);
			else
				writer.writeUI16(0);
		}
		
		
		private void  encodeClipActionRecord(ClipActionRecord r)
		{
			encodeClipEventFlags(r.eventFlags, writer);
			
			int pos = writer.Pos;
			writer.write32(0); // offset placeholder
			
			if ((r.eventFlags & ClipActionRecord.keyPress) != 0)
			{
				writer.writeUI8(r.keyCode);
			}
			
			encode(r.actionList);
			
			writer.write32at(pos, (writer.Pos - pos) - 4);
		}
		
		private void  encodeClipEventFlags(int flags, SwfEncoder w)
		{
			if (w.swfVersion >= 6)
				w.write32(flags);
			else
				w.writeUI16(flags);
		}
	}
}