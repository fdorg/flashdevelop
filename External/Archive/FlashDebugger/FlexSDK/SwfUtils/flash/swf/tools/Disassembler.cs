// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
using System.IO;

using Flash.Swf;
using Flash.Swf.Actions;
using Flash.Swf.Debug;
using Flash.Swf.Types;
using FieldFormat = Flash.Util.FieldFormat;

namespace Flash.Swf.tools
{
	
	/// <author>  Edwin Smith
	/// </author>
	public class Disassembler:ActionHandler
	{
		virtual public String Comment
		{
			set
			{
				this.comment = value;
			}
			
		}
		virtual public String Format
		{
			set
			{
				this.format = value;
			}
			
		}
		virtual public bool ShowDebugSource
		{
			set
			{
				this.showDebugSource = value;
			}
			
		}
		virtual public bool ShowLineRecord
		{
			set
			{
				this.showLineRecord = value;
			}
			
		}
		override public Action Target2
		{
			set
			{
				print(value);
			}
			
		}
		override public SetTarget Target
		{
			set
			{
				start(value);
				out_Renamed.WriteLine(" " + value.targetName);
			}
			
		}
		protected internal ConstantPool cpool;
		protected internal int start_Renamed_Field;
		protected internal int offset;
		protected internal StreamWriter out_Renamed;
		private bool showOffset = false;
		private bool showDebugSource = false;
		private bool showLineRecord = true;
		private RegisterRecord registerRecord_Renamed_Field = null;
		private int indent_Renamed_Field;
		private int initialIndent;
		private String comment;
		private String format;
		
		public Disassembler(StreamWriter out_Renamed, ConstantPool cpool, String comment):this(out_Renamed, false, 0)
		{
			this.cpool = cpool;
			this.comment = comment;
		}
		
		public Disassembler(StreamWriter out_Renamed, bool showOffset, int indent)
		{
			this.out_Renamed = out_Renamed;
			this.showOffset = showOffset;
			this.indent_Renamed_Field = indent;
			this.initialIndent = indent;
			this.comment = "";
		}
		
		public static void  disassemble(ActionList list, ConstantPool pool, int startIndex, int endIndex, StreamWriter out_Renamed)
		{
			Disassembler d = new Disassembler(out_Renamed, pool, "    ");
			d.Format = "    0x%08O  %a";
			d.ShowLineRecord = false;
			
			// probe backward for a register record if any to set up register to variable name mapping
			int at = list.lastIndexOf(ActionList.sactionRegisterRecord, startIndex);
			if (at > - 1)
				d.registerRecord_Renamed_Field = (RegisterRecord) list.getAction(at);
			
			// now dump the contents of our request
			list.visit(d, startIndex, endIndex);
			out_Renamed.Flush();
		}
		
		protected internal virtual void  print(Action action)
		{
			start(action);
			out_Renamed.WriteLine();
		}
		
		public override void  setActionOffset(int offset, Action a)
		{
			if (this.offset == 0)
			{
				this.start_Renamed_Field = offset;
			}
			this.offset = offset;
		}
		
		protected internal virtual void  indent()
		{
			for (int i = 0; i < initialIndent; i++)
				out_Renamed.Write("  ");
			out_Renamed.Write(comment);
			for (int i = initialIndent; i < indent_Renamed_Field; i++)
				out_Renamed.Write("  ");
		}
		
		public override void  registerRecord(RegisterRecord record)
		{
			// set the active record
			registerRecord_Renamed_Field = record;
		}
		
		protected internal virtual String variableNameForRegister(int regNbr)
		{
			int at = (registerRecord_Renamed_Field == null)?- 1:registerRecord_Renamed_Field.indexOf(regNbr);
			if (at > - 1)
				return registerRecord_Renamed_Field.variableNames[at];
			else
				return null;
		}
		
		public override void  lineRecord(LineRecord line)
		{
			if (!showLineRecord)
			{
			}
			else if (showDebugSource)
			{
				printLines(line, out_Renamed);
			}
			else
			{
				start(line);
				out_Renamed.WriteLine(" " + line.module.name + ":" + line.lineno);
			}
		}
		
		public virtual void  printLines(LineRecord lr, StreamWriter out_Renamed)
		{
			DebugModule script = lr.module;
			
			if (script != null)
			{
				int lineno = lr.lineno;
				if (lineno > 0)
				{
					while (lineno - 1 > 0 && script.offsets[lineno - 1] == 0)
					{
						lineno--;
					}
					if (lineno == 1)
					{
						indent();
						out_Renamed.WriteLine(script.name);
					}
					int off = script.index[lineno - 1];
					int len = script.index[lr.lineno] - off;
					out_Renamed.Write(script.text.ToCharArray(), off, len);
				}
			}
		}
		
		protected internal virtual void  start(Action action)
		{
			String actionName;
			if ((action.code < 0) || (action.code > actionNames.Length))
			{
				actionName = "Unknown";
			}
			else
			{
				actionName = actionNames[action.code];
			}
			
			if (showOffset)
			{
				indent();
				out_Renamed.Write("absolute=" + offset + ",relative=" + (offset - start_Renamed_Field) + ",code=" + action.code + "\t" + actionName);
			}
			else
			{
				if (format == null)
				{
					indent();
					out_Renamed.Write(actionName);
				}
				else
				{
					startFormatted(actionName);
				}
			}
		}
		
		protected internal virtual void  startFormatted(String action)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			bool leadingZeros = false;
			int width = - 1;
			
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '%')
				{
					c = format[++i];
					if (System.Char.IsDigit(c))
					{
						// absorb a leading zero, if any
						if (c == '0')
						{
							leadingZeros = true;
							c = format[++i];
						}
						
						System.Text.StringBuilder number = new System.Text.StringBuilder();
						while (System.Char.IsDigit(c))
						{
							number.Append(c);
							c = format[++i];
						}
						try
						{
							width = System.Int32.Parse(number.ToString());
						}
						catch (System.FormatException)
						{
							width = - 1;
						}
					}
					
					if (c == 'O')
					{
						FieldFormat.formatLongToHex(sb, offset, width, leadingZeros);
					}
					else if (c == 'o')
					{
						FieldFormat.formatLong(sb, offset, width, leadingZeros);
					}
					else if (c == 'a')
					{
						sb.Append(action);
					}
				}
				else
					sb.Append(c);
			}
			out_Renamed.Write(sb.ToString());
		}
		
		public override void  nextFrame(Action action)
		{
			print(action);
		}
		
		public override void  prevFrame(Action action)
		{
			print(action);
		}
		
		public override void  play(Action action)
		{
			print(action);
		}
		
		public override void  stop(Action action)
		{
			print(action);
		}
		
		public override void  toggleQuality(Action action)
		{
			print(action);
		}
		
		public override void  stopSounds(Action action)
		{
			print(action);
		}
		
		public override void  add(Action action)
		{
			print(action);
		}
		
		public override void  subtract(Action action)
		{
			print(action);
		}
		
		public override void  multiply(Action action)
		{
			print(action);
		}
		
		public override void  divide(Action action)
		{
			print(action);
		}
		
		public override void  equals(Action action)
		{
			print(action);
		}
		
		public override void  less(Action action)
		{
			print(action);
		}
		
		public override void  and(Action action)
		{
			print(action);
		}
		
		public override void  or(Action action)
		{
			print(action);
		}
		
		public override void  not(Action action)
		{
			print(action);
		}
		
		public override void  stringEquals(Action action)
		{
			print(action);
		}
		
		public override void  stringLength(Action action)
		{
			print(action);
		}
		
		public override void  stringExtract(Action action)
		{
			print(action);
		}
		
		public override void  pop(Action action)
		{
			print(action);
		}
		
		public override void  toInteger(Action action)
		{
			print(action);
		}
		
		public override void  getVariable(Action action)
		{
			print(action);
		}
		
		public override void  setVariable(Action action)
		{
			print(action);
		}
		
		public override void  stringAdd(Action action)
		{
			print(action);
		}
		
		public override void  getProperty(Action action)
		{
			print(action);
		}
		
		public override void  setProperty(Action action)
		{
			print(action);
		}
		
		public override void  cloneSprite(Action action)
		{
			print(action);
		}
		
		public override void  removeSprite(Action action)
		{
			print(action);
		}
		
		public override void  trace(Action action)
		{
			print(action);
		}
		
		public override void  startDrag(Action action)
		{
			print(action);
		}
		
		public override void  endDrag(Action action)
		{
			print(action);
		}
		
		public override void  stringLess(Action action)
		{
			print(action);
		}
		
		public override void  randomNumber(Action action)
		{
			print(action);
		}
		
		public override void  mbStringLength(Action action)
		{
			print(action);
		}
		
		public override void  charToASCII(Action action)
		{
			print(action);
		}
		
		public override void  asciiToChar(Action action)
		{
			print(action);
		}
		
		public override void  getTime(Action action)
		{
			print(action);
		}
		
		public override void  mbStringExtract(Action action)
		{
			print(action);
		}
		
		public override void  mbCharToASCII(Action action)
		{
			print(action);
		}
		
		public override void  mbASCIIToChar(Action action)
		{
			print(action);
		}
		
		public override void  delete(Action action)
		{
			print(action);
		}
		
		public override void  delete2(Action action)
		{
			print(action);
		}
		
		public override void  defineLocal(Action action)
		{
			print(action);
		}
		
		public override void  callFunction(Action action)
		{
			print(action);
		}
		
		public override void  returnAction(Action action)
		{
			print(action);
		}
		
		public override void  modulo(Action action)
		{
			print(action);
		}
		
		public override void  newObject(Action action)
		{
			print(action);
		}
		
		public override void  defineLocal2(Action action)
		{
			print(action);
		}
		
		public override void  initArray(Action action)
		{
			print(action);
		}
		
		public override void  initObject(Action action)
		{
			print(action);
		}
		
		public override void  typeOf(Action action)
		{
			print(action);
		}
		
		public override void  targetPath(Action action)
		{
			print(action);
		}
		
		public override void  enumerate(Action action)
		{
			print(action);
		}
		
		public override void  add2(Action action)
		{
			print(action);
		}
		
		public override void  less2(Action action)
		{
			print(action);
		}
		
		public override void  equals2(Action action)
		{
			print(action);
		}
		
		public override void  toNumber(Action action)
		{
			print(action);
		}
		
		public override void  toString(Action action)
		{
			print(action);
		}
		
		public override void  pushDuplicate(Action action)
		{
			print(action);
		}
		
		public override void  stackSwap(Action action)
		{
			print(action);
		}
		
		public override void  getMember(Action action)
		{
			print(action);
		}
		
		public override void  setMember(Action action)
		{
			print(action);
		}
		
		public override void  increment(Action action)
		{
			print(action);
		}
		
		public override void  decrement(Action action)
		{
			print(action);
		}
		
		public override void  callMethod(Action action)
		{
			print(action);
		}
		
		public override void  newMethod(Action action)
		{
			print(action);
		}
		
		public override void  instanceOf(Action action)
		{
			print(action);
		} // only if object model enabled
		
		public override void  enumerate2(Action action)
		{
			print(action);
		}
		
		public override void  bitAnd(Action action)
		{
			print(action);
		}
		
		public override void  bitOr(Action action)
		{
			print(action);
		}
		
		public override void  bitXor(Action action)
		{
			print(action);
		}
		
		public override void  bitLShift(Action action)
		{
			print(action);
		}
		
		public override void  bitRShift(Action action)
		{
			print(action);
		}
		
		public override void  bitURShift(Action action)
		{
			print(action);
		}
		
		public override void  strictEquals(Action action)
		{
			print(action);
		}
		
		public override void  greater(Action action)
		{
			print(action);
		}
		
		public override void  stringGreater(Action action)
		{
			print(action);
		}
		
		public override void  gotoFrame(GotoFrame action)
		{
			start(action);
			out_Renamed.WriteLine(" " + action.frame);
		}
		
		public override void  getURL(GetURL action)
		{
			start(action);
			out_Renamed.WriteLine(" " + action.url + " " + action.target);
		}
		
		public override void  storeRegister(StoreRegister action)
		{
			start(action);
			String variableName = variableNameForRegister(action.register);
			out_Renamed.WriteLine(" $" + action.register + ((variableName == null)?"":"   \t\t; " + variableName));
		}
		
		public override void  constantPool(ConstantPool action)
		{
			cpool = action;
			start(action);
			out_Renamed.WriteLine(" [" + action.pool.Length + "]");
		}
		
		public override void  strictMode(StrictMode action)
		{
			print(action);
		}
		
		public override void  waitForFrame(WaitForFrame action)
		{
			start(action);
			out_Renamed.WriteLine(" " + action.frame + " {");
			indent_Renamed_Field++;
			labels.getLabelEntry(action.skipTarget).source = action;
		}
		
		public override void  gotoLabel(GotoLabel action)
		{
			start(action);
			out_Renamed.WriteLine(" " + action.label);
		}
		
		public override void  waitForFrame2(WaitForFrame action)
		{
			start(action);
			out_Renamed.WriteLine(" {");
			indent_Renamed_Field++;
			labels.getLabelEntry(action.skipTarget).source = action;
		}
		
		public override void  with(With action)
		{
			start(action);
			out_Renamed.WriteLine(" {");
			indent_Renamed_Field++;
			labels.getLabelEntry(action.endWith).source = action;
		}
		
		public override void  tryAction(Try action)
		{
			start(action);
			out_Renamed.WriteLine(" {");
			indent_Renamed_Field++;
			
			labels.getLabelEntry(action.endTry).source = action;
			if (action.hasCatch())
				labels.getLabelEntry(action.endCatch).source = action;
			if (action.hasFinally())
				labels.getLabelEntry(action.endFinally).source = action;
		}
		
		public override void  throwAction(Action action)
		{
			print(action);
		}
		
		public override void  castOp(Action action)
		{
			print(action);
		}
		
		public override void  implementsOp(Action action)
		{
			print(action);
		}
		
		public override void  extendsOp(Action action)
		{
			print(action);
		}
		
		public override void  nop(Action action)
		{
			print(action);
		}
		
		public override void  halt(Action action)
		{
			print(action);
		}
		
		public override void  push(Push action)
		{
			start(action);
			out_Renamed.Write(" ");
			System.Object value = action.value;
			int type = Push.getTypeCode(value);
			switch (type)
			{
				
				case Flash.Swf.ActionConstants.kPushStringType: 
					out_Renamed.Write(quoteString(value.ToString(), '"'));
					break;
				
				case Flash.Swf.ActionConstants.kPushNullType: 
					out_Renamed.Write("null");
					break;
				
				case Flash.Swf.ActionConstants.kPushUndefinedType: 
					out_Renamed.Write("undefined");
					break;
				
				case Flash.Swf.ActionConstants.kPushRegisterType: 
					String variableName = variableNameForRegister(((int) ((System.SByte) value) & 0xFF));
					out_Renamed.Write("$" + ((int) ((System.SByte) value) & 0xFF) + ((variableName == null)?"":"   \t\t; " + variableName));
					break;
				
				case Flash.Swf.ActionConstants.kPushConstant8Type: 
				case Flash.Swf.ActionConstants.kPushConstant16Type: 
					int index = System.Convert.ToInt32(((System.ValueType) value)) & 0xFFFF;
					out_Renamed.Write(((cpool == null)?System.Convert.ToString(index):quoteString(cpool.pool[index], '\'')));
					break;
				
				case Flash.Swf.ActionConstants.kPushFloatType: 
					out_Renamed.Write(value + "F");
					break;
				
				case Flash.Swf.ActionConstants.kPushBooleanType: 
				case Flash.Swf.ActionConstants.kPushDoubleType: 
				case Flash.Swf.ActionConstants.kPushIntegerType: 
					out_Renamed.Write(value);
					break;
				
				default:
					System.Diagnostics.Debug.Assert(false);
					break;
				
			}
			out_Renamed.WriteLine();
		}
		
		public override void  getURL2(GetURL2 action)
		{
			start(action);
			out_Renamed.WriteLine(" " + action.method);
		}
		
		public override void  defineFunction(DefineFunction action)
		{
			start(action);
			out_Renamed.Write(" " + action.name + "(");
			for (int i = 0; i < action.params_Renamed.Length; i++)
			{
				out_Renamed.Write(action.params_Renamed[i]);
				if (i + 1 < action.params_Renamed.Length)
				{
					out_Renamed.Write(", ");
				}
			}
			out_Renamed.WriteLine(") {");
			indent_Renamed_Field++;
			action.actionList.visitAll(this);
			indent_Renamed_Field--;
			indent();
			out_Renamed.WriteLine("} " + action.name);
		}
		
		public override void  defineFunction2(DefineFunction action)
		{
			start(action);
			out_Renamed.Write(" " + action.name + "(");
			for (int i = 0; i < action.params_Renamed.Length; i++)
			{
				out_Renamed.Write("$" + action.paramReg[i] + "=" + action.params_Renamed[i]);
				if (i + 1 < action.params_Renamed.Length)
				{
					out_Renamed.Write(", ");
				}
			}
			out_Renamed.Write(")");
			int regno = 1;
			if ((action.flags & DefineFunction.kPreloadThis) != 0)
				out_Renamed.Write(" $" + (regno++) + "=this");
			if ((action.flags & DefineFunction.kPreloadArguments) != 0)
				out_Renamed.Write(" $" + (regno++) + "=arguments");
			if ((action.flags & DefineFunction.kPreloadSuper) != 0)
				out_Renamed.Write(" $" + (regno++) + "=super");
			if ((action.flags & DefineFunction.kPreloadRoot) != 0)
				out_Renamed.Write(" $" + (regno++) + "=_root");
			if ((action.flags & DefineFunction.kPreloadParent) != 0)
				out_Renamed.Write(" $" + (regno++) + "=_parent");
			if ((action.flags & DefineFunction.kPreloadGlobal) != 0)
				out_Renamed.Write(" $" + (regno) + "=_global");
			out_Renamed.WriteLine(" {");
			indent_Renamed_Field++;
			action.actionList.visitAll(this);
			indent_Renamed_Field--;
			indent();
			out_Renamed.WriteLine("} " + action.name);
		}
		
		internal class LabelEntry
		{
			internal String name;
			internal Action source;
			
			public LabelEntry(String name, Action source)
			{
				this.name = name;
				this.source = source;
			}
		}
		[Serializable]
		private class LabelMap:System.Collections.Hashtable
		{
			internal virtual LabelEntry getLabelEntry(Label l)
			{
				LabelEntry entry = (LabelEntry) this[l];
				if (entry == null)
				{
					entry = new LabelEntry(null, null);
					System.Object tempObject;
					tempObject = this[l];
					this[l] = entry;
					System.Object generatedAux2 = tempObject;
				}
				return entry;
			}
		}

        private LabelMap labels = new LabelMap();
		internal int labelCount = 0;
		
		public override void  ifAction(Branch action)
		{
			printBranch(action);
		}
		
		public override void  jump(Branch action)
		{
			printBranch(action);
		}
		
		protected internal virtual void  printBranch(Branch action)
		{
			start(action);
			LabelEntry entry = labels.getLabelEntry(action.target);
			if (entry.name == null)
				entry.name = "L" + System.Convert.ToString(labelCount++);
			entry.source = action;
			out_Renamed.WriteLine(" " + entry.name);
		}
		
		public override void  label(Label label)
		{
			LabelEntry entry = labels.getLabelEntry(label);
			if (entry.source == null)
			{
				// have not seen any actions that target this label yet, and that
				// means the source can only be a backwards branch
				entry.name = "L" + System.Convert.ToString(labelCount++);
				indent();
				out_Renamed.WriteLine(entry.name + ":");
			}
			else
			{
				switch (entry.source.code)
				{
					
					case Flash.Swf.ActionConstants.sactionTry: 
						Try t = (Try) entry.source;
						indent_Renamed_Field--;
						indent();
						out_Renamed.WriteLine("}");
						indent();
						if (label == t.endTry && t.hasCatch())
						{
							out_Renamed.WriteLine("catch(" + (t.hasRegister()?"$" + t.catchReg:t.catchName) + ") {");
							indent_Renamed_Field++;
						}
						else if ((label == t.endTry || label == t.endCatch) && t.hasFinally())
						{
							out_Renamed.WriteLine("finally {");
							indent_Renamed_Field++;
						}
						break;
					
					case Flash.Swf.ActionConstants.sactionWaitForFrame: 
					case Flash.Swf.ActionConstants.sactionWaitForFrame2: 
					case Flash.Swf.ActionConstants.sactionWith: 
						// end of block
						indent_Renamed_Field--;
						indent();
						out_Renamed.WriteLine("}");
						break;
					
					case Flash.Swf.ActionConstants.sactionIf: 
					case Flash.Swf.ActionConstants.sactionJump: 
						indent();
						out_Renamed.WriteLine(entry.name + ":");
						break;
					
					default:
						System.Diagnostics.Debug.Assert(false);
						break;
					
				}
			}
		}
		
		public override void  call(Action action)
		{
			print(action);
		}
		
		public override void  gotoFrame2(GotoFrame2 action)
		{
			start(action);
			out_Renamed.WriteLine(" " + action.playFlag);
		}
		
		public override void  quickTime(Action action)
		{
			print(action);
		}
		
		public override void  unknown(Unknown action)
		{
			print(action);
		}
		
		
		public static String quoteString(String s, char qc)
		{
			System.Text.StringBuilder b = new System.Text.StringBuilder(s.Length + 2);
			
			b.Append(qc);
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				switch (c)
				{
					
					case (char) (8):  b.Append("\\v"); break;
					
					case '\f':  b.Append("\\f"); break;
					
					case '\r':  b.Append("\\r"); break;
					
					case '\t':  b.Append("\\t"); break;
					
					case '\n':  b.Append("\\n"); break;
					
					case '"':  b.Append("\\\""); break;
					
					case '\'':  b.Append("\\'"); break;
					
					default:  b.Append(c); break;
					
				}
			}
			b.Append(qc);
			return b.ToString();
		}
		
		
		public static readonly String[] actionNames = new String[]{"0x00", "0x01", "0x02", "0x03", "next", "prev", "play", "stop", "toggle", "stopsound", "add", "sub", "mul", "div", "eq", "lt", "and", "or", "not", "seq", "slen", "substr", "0x16", "pop", "toint", "0x19", "0x1A", "0x1B", "get", "set", "0x1E", "0x1F", "settarget2", "sadd", "getprop", "setprop", "csprite", "rsprite", "trace", "sdrag", "edrag", "slt", "0x2A", "0x2B", "0x2C", "0x2D", "0x2E", "0x2F", "rand", "wslen", "c2a", "a2c", "time", "wsubstr", "wc2a", "wa2c", "0x38", "0x39", "del", "del2", "var", "callfun", "return", "mod", "newobj", "var2", "initarr", "initobj", "typeof", "target", "enum", "add2", "lt2", "eq2", "tonum", "tostr", "dup", "swap", "getmem", "setmem", "inc", "dec", "callmethod", "newmethod", "instanceof", "enum2", "0x56", "0x57", "0x58", "0x59", "0x5A", "0x5B", "0x5C", "0x5D", "0x5E", "halt", "band", "bor", "bxor", "bls", "brs", "burs", "eqs", "gt", "sgt", "extends", "0x6A", "0x6B", "0x6C", "0x6D", "0x6E", "0x6F", "0x70", "0x71", "0x72", "0x73", "0x74", "0x75", "0x76", "nop", "0x78", "0x79", "0x7A", "0x7B", "0x7C", "0x7D", "0x7E", "0x7F", "0x80", "gotoframe", "0x82", "geturl", "0x84", "0x85", "0x86", "store", "cpool", "strict", "wait", "settarget", "gotolabel", "wait2", "function2", "try", "0x90", "0x91", "0x92", "0x93", "with", "0x95", "push", "0x97", "0x98", "jump", "geturl2", "function", "0x9C", "if", "call", "gotof2", "0xA0", "0xA1", "0xA2", "0xA3", "0xA4", "0xA5", "0xA6", "0xA7", "0xA8", "0xA9", "quicktime", "0xAB", "0xAC", "0xAD", "0xAE", "0xAF", "0xB0", "0xB1", "0xB2", "0xB3", "0xB4", "0xB5", "0xB6", "0xB7", "0xB8", "0xB9", "0xBA", "0xBB", "0xBC", "0xBD", "0xBE", "0xBF", "0xC0", "0xC1", "0xC2", "0xC3", "0xC4", "0xC5", "0xC6", "0xC7", "0xC8", "0xC9", "0xCA", "0xCB", "0xCC", "0xCD", "0xCE", "0xCF", "0xD0", "0xD1", "0xD2", "0xD3", "0xD4", "0xD5", "0xD6", "0xD7", "0xD8", "0xD9", "0xDA", "0xDB", "0xDC", "0xDD", "0xDE", "0xDF", "0xE0", "0xE1", "0xE2", "0xE3", "0xE4", "0xE5", "0xE6", "0xE7", "0xE8", "0xE9", "0xEA", 
			"0xEB", "0xEC", "0xED", "0xEE", "0xEF", "0xF0", "0xF1", "0xF2", "0xF3", "0xF4", "0xF5", "0xF6", "0xF7", "0xF8", "0xF9", "0xFA", "0xFB", "0xFC", "0xFD", "0xFE", "0xFF", "label", "line"};
	}
}
