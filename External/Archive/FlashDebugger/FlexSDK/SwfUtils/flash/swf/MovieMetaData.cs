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

using Flash.Swf.Actions;
using Flash.Swf.Debug;
using Flash.Swf.Tags;
using Flash.Swf.Types;
using IntMap = Flash.Util.IntMap;

namespace Flash.Swf
{
	
	/// <author>  Clement Wong
	/// </author>
	public sealed class MovieMetaData : TagHandler
	{
		public System.Collections.IEnumerator FunctionLines
		{
			get
			{
				return preciseLines.GetEnumerator();
			}
			
		}
		override public Dictionary DecoderDictionary
		{
			set
			{
				this.dict = value;
			}
			
		}
		public MovieMetaData(byte[] swf, byte[] swd):this(new MemoryStream(swf), new MemoryStream(swd))
		{
		}
		
		
		public MovieMetaData(Stream swf, Stream swd)
		{
			try
			{
				init();
				TagDecoder p = new TagDecoder(swf, swd);
				parse(p);
			}
			catch (IOException)
			{
			}
		}
		
		public MovieMetaData(String u)
		{
			try
			{
				init();
				Uri url = new Uri(u);
				Stream in_Renamed = System.Net.WebRequest.Create(url).GetResponse().GetResponseStream();
				TagDecoder p = new TagDecoder(in_Renamed, url);
				parse(p);
			}
			catch (UriFormatException)
			{
			}
			catch (IOException)
			{
			}
		}
		
		private void  init()
		{
			actions = new IntMap();
			modules = new IntMap();
			functionNames = new IntMap();
			functionSizes = new IntMap();
			functionLines = new IntMap();
			preciseLines = new IntMap();
			mxml = new System.Collections.Hashtable();
			
			pool = null;
			skipOffsets = new System.Collections.ArrayList();
		}
		
		private void  parse(TagDecoder p)
		{
			p.KeepOffsets = true;
			p.parse(this);
			
			SupportClass.CollectionsSupport.Sort(skipOffsets, null);
			className = null;
		}
		
		private Dictionary dict;
		private Header header_field;
		
		// given an offset, what's the bytecode?
		public IntMap actions;
		
		// given an offset, what debug module it's in?
		public IntMap modules;
		
		// given an offset, what function it's in?
		public IntMap functionNames;
		public IntMap functionSizes;
		public IntMap functionLines;
		public IntMap preciseLines;
		
		// MXML DebugModule
		public System.Collections.IDictionary mxml;
		
		// offsets that we don't want to profile
		public System.Collections.IList skipOffsets;
		
		// temporarily store AS2 class name...
		private String className;
		
		private String[] pool;
		
		public DebugModule getDebugModule(int offset)
		{
			DebugModule d = (DebugModule) modules.get_Renamed(offset);
			if (d == null)
			{
				return null;
			}
			else
			{
				return d;
			}
		}
		
		public String getFunctionName(int offset)
		{
			return (String) functionNames.get_Renamed(offset);
		}
		
		public Int32 getOpCode(int offset)
		{
			return (Int32) actions.get_Renamed(offset);
		}
		
		internal Int32 getFunctionLineNumber(int offset)
		{
			return (Int32) functionLines.get_Renamed(offset);
		}
		
		internal bool isFunction(int offset)
		{
			String s = getFunctionName(offset);
			return (s != null);
		}
		
		public override void  header(Header h)
		{
			header_field = h;
		}
		
		public override void  defineButton(DefineButton tag)
		{
			String[] temp = pool;
			collectActions(tag.condActions[0].actionList);
			pool = temp;
		}
		
		public override void  doAction(DoAction tag)
		{
			String[] temp = pool;
			collectActions(tag.actionList);
			pool = temp;
		}
		
		public override void  placeObject2(PlaceObject tag)
		{
			collectClipActions(tag.clipActions);
		}
		
		public override void  placeObject3(PlaceObject tag)
		{
			collectClipActions(tag.clipActions);
		}
		
		public override void  defineButton2(DefineButton tag)
		{
			collectCondActions(tag.condActions);
		}
		
		public override void  defineSprite(DefineSprite tag)
		{
			collectSpriteActions(tag.tagList);
		}
		
		public override void  doInitAction(DoInitAction tag)
		{
			if (header_field.version > 6 && tag.sprite != null)
			{
				String __Packages = idRef(tag.sprite);
				className = (__Packages != null && __Packages.StartsWith("__Packages"))?__Packages.Substring(11):null; // length("__Packages.") = 11
				
				if (isRegisterClass(tag.actionList))
				{
					DebugModule dm = new DebugModule();
					// C: We actually want the class name here, not the linkage ID.
					dm.name = "<" + __Packages + ".2>";
					// C: We want the class name as the second input argument. Fortunately, we don't
					//    really do anything with the source, so it's okay.
					dm.Text = "Object.registerClass(" + __Packages + ", " + __Packages + ");";
					dm.bitmap = 1;
					
					LineRecord lr = new LineRecord(1, dm);
					
					int startOffset = tag.actionList.getOffset(0);
					dm.addOffset(lr, startOffset);
					
					tag.actionList.insert(startOffset, lr);
					modules.put((int) (SupportClass.Random.NextDouble() * System.Int32.MaxValue), dm);
				}
			}
			
			String[] temp = pool;
			collectActions(tag.actionList);
			pool = temp;
			
			className = null;
		}
		
		private static readonly int[] regClassCall9 = new int[]{ActionConstants.sactionPush, ActionConstants.sactionGetVariable, ActionConstants.sactionPush, ActionConstants.sactionPush, ActionConstants.sactionPush, ActionConstants.sactionGetVariable, ActionConstants.sactionPush, ActionConstants.sactionCallMethod, ActionConstants.sactionPop};
		
		private static readonly int[] regClassCall10 = new int[]{ActionConstants.sactionConstantPool, ActionConstants.sactionPush, ActionConstants.sactionGetVariable, ActionConstants.sactionPush, ActionConstants.sactionPush, ActionConstants.sactionPush, ActionConstants.sactionGetVariable, ActionConstants.sactionPush, ActionConstants.sactionCallMethod, ActionConstants.sactionPop};
		
		// TODO: Use an evaluation stack to figure out the Object.registerClass() call.
		public static bool isRegisterClass(ActionList actionList)
		{
			if (!hasLineRecord(actionList))
			{
				int[] opcodes;
				
				if (actionList.size() == 9)
				{
					opcodes = regClassCall9;
				}
				else if (actionList.size() == 10)
				{
					opcodes = regClassCall10;
				}
				else
				{
					return false;
				}
				
				for (int i = 0; i < opcodes.Length; i++)
				{
					if (actionList.getAction(i).code != opcodes[i])
					{
						return false;
					}
					else
					{
						// TODO: need to check the PUSH values...
					}
				}
				
				return true;
			}
			
			return false;
		}
		
		internal String idRef(DefineTag tag)
		{
			return idRef(tag, dict);
		}
		
		public static String idRef(DefineTag tag, Dictionary d)
		{
			if (tag == null)
			{
				// if tag is null then it isn't in the dict -- the SWF is invalid.
				// lets be lax and print something; Matador generates invalid SWF sometimes.
				return "-1";
			}
			else if (tag.name == null)
			{
				// just print the character id since no name was exported
				return System.Convert.ToString(d.getId(tag));
			}
			else
			{
				return tag.name;
			}
		}
		
		private static bool hasLineRecord(ActionList c)
		{
			if (c == null || c.size() == 0)
			{
				return true;
			}
			
			bool result = false;
			
			for (int i = 0; i < c.size() && !result; i++)
			{
				Action action = c.getAction(i);
				
				switch (action.code)
				{
					
					case ActionConstants.sactionDefineFunction: 
					case ActionConstants.sactionDefineFunction2: 
						result = result || hasLineRecord(((DefineFunction) action).actionList);
						break;
					
					case ActionList.sactionLineRecord: 
						result = true;
						break;
					}
			}
			
			return result;
		}
		
		private void  collectSpriteActions(TagList s)
		{
			String[] temp;
			
			int len = s.tags.Count;
			for (int i = 0; i < len; i++)
			{
				Tag t = (Tag) s.tags[i];
				switch (t.code)
				{
					case TagValues.stagDoAction: 
						temp = pool;
						collectActions(((DoAction) t).actionList);
						pool = temp;
						break;
					
					case TagValues.stagDefineButton2: 
						collectCondActions(((DefineButton) t).condActions);
						break;
					
					case TagValues.stagDefineButton: 
						temp = pool;
						collectActions(((DefineButton) t).condActions[0].actionList);
						pool = temp;
						break;
					
					case TagValues.stagDoInitAction: 
						temp = pool;
						collectActions(((DoInitAction) t).actionList);
						pool = temp;
						break;
					
					case TagValues.stagDefineSprite: 
						collectSpriteActions(((DefineSprite) t).tagList);
						break;
					
					case TagValues.stagPlaceObject2: 
						collectClipActions(((PlaceObject) t).clipActions);
						break;
				}
			}
		}
		
		private DebugModule findDebugModule(ActionList c)
		{
			MFUCache modules = new MFUCache();
			
			for (int i = 0; i < c.size(); i++)
			{
				Action a = c.getAction(i);
				
				DebugModule temp = null;
				
				switch (a.code)
				{
					
					case ActionConstants.sactionDefineFunction: 
					case ActionConstants.sactionDefineFunction2: 
						temp = findDebugModule(((DefineFunction) a).actionList);
						break;
					
					case ActionList.sactionLineRecord: 
						if (((LineRecord) a).module != null)
						{
							temp = ((LineRecord) a).module;
						}
						break;
					}
				
				if (temp != null)
				{
					modules.add(temp);
				}
			}
			
			// ActionList may have actions pointing to more than one debug module because of #include, etc.
			// The majority wins.
			
			return modules.topModule;
		}
		
		private static Int32[] codes = new Int32[256];
		
		private void  collectActions(ActionList c)
		{
			// assumption: ActionContext c is always not null! try-catch-finally may be busted.
			if (c == null)
			{
				return ;
			}
			
			// interprets the actions. try to assign names to anonymous functions...
			evalActions(c);
			
			DebugModule d = findDebugModule(c);
			
			String emptyMethodName = null;
			
			// loop again, this time, we register all the actions...
			for (int i = 0; i < c.size(); i++)
			{
				int ioffset = c.getOffset(i);
				Action a = c.getAction(i);
				
				if (emptyMethodName != null && emptyMethodName.Length != 0)
				{
					functionNames.put(ioffset, emptyMethodName);
					emptyMethodName = null;
				}
				
				if (a.code == ActionList.sactionLineRecord)
				{
					LineRecord line = (LineRecord) a;
					if (line.module != null)
					{
						d = line.module;
						if (d.name.EndsWith(".mxml"))
						{
							mxml[d.name] = d;
						}
					}
					
					continue;
				}
				
				if (a.code >= 256)
				{
					// something synthetic we don't care about
					continue;
				}
				
				actions.put(ioffset, (Object) codes[a.code]);
				modules.put(ioffset, d);
				
				switch (a.code)
				{
					
					case ActionConstants.sactionDefineFunction: 
					case ActionConstants.sactionDefineFunction2: 
						DefineFunction f = (DefineFunction) a;
						Int32 size = (Int32) f.codeSize;
						
						if (f.actionList.size() == 0)
						{
							emptyMethodName = f.name;
						}
						else
						{
							Int32 lineno = -1;
							
							// map all the offsets in this function to the function name
							for (int j = 0; j < f.actionList.size(); j++)
							{
								int o = f.actionList.getOffset(j);
								Action child = f.actionList.getAction(j);
								if (child.code == ActionList.sactionLineRecord)
								{
									if (lineno == -1)
										lineno = (Int32) ((LineRecord) child).lineno;
									
									preciseLines.put(o, (Object) ((LineRecord) child).lineno);
								}
								functionNames.put(o, f.name);
								functionSizes.put(o, (Object) size);
							}
							
							
							// map all the offsets in this function to the first line number of this function.
							for (int j = 0; j < f.actionList.size(); j++)
							{
								int o = f.actionList.getOffset(j);
								functionLines.put(o, (Object) lineno);
							}
						}
						
						collectActions(f.actionList);
						break;
					}
			}
		}
		
		private void  collectCondActions(ButtonCondAction[] actions)
		{
			for (int i = 0; i < actions.Length; i++)
			{
				collectActions(actions[i].actionList);
			}
		}
		
		private void  collectClipActions(ClipActions actions)
		{
			if (actions != null)
			{
                foreach (ClipActionRecord record in actions.clipActionRecords)
				{
					collectActions(record.actionList);
				}
			}
		}
		
		private static Object pop(System.Collections.Stack stack)
		{
			return (stack.Count != 0) ? stack.Pop() : null;
		}
		
		private void  evalActions(ActionList c)
		{
			try
			{
				walkActions(c, header_field.version, pool, className, skipOffsets);
			}
			catch (Exception)
			{
			}
		}
		
		// data used in our walkActions routine
		private static Object dummy = new Object();
		private static Object[] registers = new Object[256];
		
		/// <summary> Walk the actions filling in the names of functions as we go.
		/// This is done by looking for DefineFunction's actions and then
		/// examining the content of the stack for a name.
		/// 
		/// </summary>
		/// <param name="c">list of actions to be traversed
		/// </param>
		/// <param name="swfVersion">version of swf file that housed the ActionList (just use 7 if you don't know)
		/// </param>
		/// <param name="pool">optional; constant pool for the list of actions
		/// </param>
		/// <param name="className">optional; used to locate a constructor function (i.e if funcName == className)
		/// </param>
		/// <param name="profileOffsets">optional; is filled with offsets if a call to a 
		/// function named 'profile' is encountered.  Can be null if caller is not
		/// interested in obtaining this information.
		/// </param>
		public static void walkActions(ActionList c, int swfVersion, String[] pool, String className, System.Collections.IList profileOffsets)
		{
			// assumption: ActionContext c is always not null! try-catch-finally may be busted.
			if (c == null)
				return ;
			
			System.Collections.Stack evalStack = new System.Collections.Stack();
			System.Collections.Hashtable variables = new System.Collections.Hashtable();
			
			// loop again, this time, we register all the actions...
			int offset;
			Action a;
			
			for (int i = 0; i < c.size(); i++)
			{
				offset = c.getOffset(i);
				a = c.getAction(i);
				
				switch (a.code)
				{
					
					// Flash 1 and 2 actions
					case ActionConstants.sactionHasLength: 
					case ActionConstants.sactionNone: 
					case ActionConstants.sactionGotoFrame: 
					case ActionConstants.sactionGetURL: 
					case ActionConstants.sactionNextFrame: 
					case ActionConstants.sactionPrevFrame: 
					case ActionConstants.sactionPlay: 
					case ActionConstants.sactionStop: 
					case ActionConstants.sactionToggleQuality: 
					case ActionConstants.sactionStopSounds: 
					case ActionConstants.sactionWaitForFrame: 
					// Flash 3 Actions
					case ActionConstants.sactionSetTarget: 
					case ActionConstants.sactionGotoLabel: 
						// no action
						break;
						
						// Flash 4 Actions
					
					case ActionConstants.sactionAdd: 
					case ActionConstants.sactionSubtract: 
					case ActionConstants.sactionMultiply: 
					case ActionConstants.sactionDivide: 
					case ActionConstants.sactionEquals: 
					case ActionConstants.sactionLess: 
					case ActionConstants.sactionAnd: 
					case ActionConstants.sactionOr: 
					case ActionConstants.sactionStringEquals: 
					case ActionConstants.sactionStringAdd: 
					case ActionConstants.sactionStringLess: 
					case ActionConstants.sactionMBStringLength: 
					case ActionConstants.sactionGetProperty: 
						// pop, pop, push
						pop(evalStack);
						break;
					
					case ActionConstants.sactionNot: 
					case ActionConstants.sactionStringLength: 
					case ActionConstants.sactionToInteger: 
					case ActionConstants.sactionCharToAscii: 
					case ActionConstants.sactionAsciiToChar: 
					case ActionConstants.sactionMBCharToAscii: 
					case ActionConstants.sactionMBAsciiToChar: 
					case ActionConstants.sactionRandomNumber: 
						// pop, push
						break;
					
					case ActionConstants.sactionGetVariable: 
						Object key = pop(evalStack);
						if (variables[key] == null)
						{
							evalStack.Push(key);
						}
						else
						{
							evalStack.Push(variables[key]);
						}
						break;
					
					case ActionConstants.sactionStringExtract: 
					case ActionConstants.sactionMBStringExtract: 
						// pop, pop, pop, push
						pop(evalStack);
						pop(evalStack);
						break;
					
					case ActionConstants.sactionPush: 
						Push p = (Push) a;
						System.Object o = p.value;
						int type = Push.getTypeCode(o);
						switch (type)
						{
							
							case ActionConstants.kPushStringType: 
								evalStack.Push(o);
								break;
							
							case ActionConstants.kPushNullType: 
								evalStack.Push("null");
								break;
							
							case ActionConstants.kPushUndefinedType: 
								evalStack.Push("undefined");
								break;
							
							case ActionConstants.kPushRegisterType: 
								evalStack.Push(registers[(int) ((SByte) o) & 0xFF]);
								break;
							
							case ActionConstants.kPushConstant8Type: 
							case ActionConstants.kPushConstant16Type: 
								evalStack.Push(pool[Convert.ToInt32(((ValueType) o)) & 0xFFFF]);
								break;
							
							case ActionConstants.kPushFloatType: 
								evalStack.Push(o + "F");
								break;
							
							case ActionConstants.kPushBooleanType: 
							case ActionConstants.kPushDoubleType: 
							case ActionConstants.kPushIntegerType: 
								evalStack.Push(o);
								break;
							
							default: 
								evalStack.Push("type" + type);
								break;
							
						}
						break;
					
					case ActionConstants.sactionIf: 
						pop(evalStack);
						break;
					
					case ActionConstants.sactionPop: 
					case ActionConstants.sactionCall: 
					case ActionConstants.sactionGotoFrame2: 
					case ActionConstants.sactionSetTarget2: 
					case ActionConstants.sactionRemoveSprite: 
					case ActionConstants.sactionWaitForFrame2: 
					case ActionConstants.sactionTrace: 
						// pop
						pop(evalStack);
						break;
					
					case ActionConstants.sactionJump: 
					case ActionConstants.sactionEndDrag: 
						// no action
						break;
					
					case ActionConstants.sactionSetVariable: 
						key = pop(evalStack);
						Object val = pop(evalStack);
						variables[key] = val;
						break;
					
					case ActionConstants.sactionGetURL2: 
						// pop, pop
						pop(evalStack);
						pop(evalStack);
						break;
					
					case ActionConstants.sactionSetProperty: 
					case ActionConstants.sactionCloneSprite: 
						// pop, pop, pop
						pop(evalStack);
						pop(evalStack);
						pop(evalStack);
						break;
					
					case ActionConstants.sactionStartDrag: 
						// pop, pop, pop, if the 3rd pop is non-zero, pop, pop, pop, pop
						pop(evalStack);
						pop(evalStack);
						Object obj = pop(evalStack);
						if (Int32.Parse(obj.ToString()) != 0)
						{
							pop(evalStack);
							pop(evalStack);
							pop(evalStack);
							pop(evalStack);
						}
						break;
					
					case ActionConstants.sactionGetTime: 
						// push
						evalStack.Push(dummy);
						break;
						
						// Flash 5 actions
					
					case ActionConstants.sactionDelete: 
						pop(evalStack);
						break;
					
					case ActionConstants.sactionDefineLocal: 
						// pop, pop
						val = pop(evalStack);
						key = pop(evalStack);
						variables[key] = val;
						break;
					
					case ActionConstants.sactionDefineFunction: 
					case ActionConstants.sactionDefineFunction2: 
						DefineFunction f = (DefineFunction) a;
						
						if (swfVersion > 6 && className != null)
						{
							if (f.name == null || f.name.Length == 0)
							{
								int depth = evalStack.Count;
								if (depth != 0)
								{
									o = evalStack.Peek();
									if (o == dummy)
									{
										f.name = "";
									}
									else if (o != null)
									{
										f.name = o.ToString();
									}
								}
								evalStack.Push(dummy);
							}
							
							if (f.name == "null")
							{
								f.name = "";
							}
							
							if (f.name == null || f.name.Length == 0)
							{
								// do nothing... it's an anonymous function!
							}
							else if (!className.EndsWith(f.name))
							{
								f.name = className + "." + f.name;
							}
							else
							{
								f.name = className + ".[constructor]";
							}
						}
						else
						{
							if (f.name == null || f.name.Length == 0)
							{
                                System.Text.StringBuilder buffer = new System.Text.StringBuilder();

                                Boolean bFirst = true;

								foreach (Object ob in evalStack)
								{
									if (ob == dummy)
									{
										break;
									}
									else if (bFirst)
									{
										buffer.Append(ob);
                                        bFirst = false;
									}
									else
									{
										buffer.Insert(0, '.');
										buffer.Insert(0, ob);
									}
								}
								f.name = buffer.ToString();
								
								if (f.name != null && f.name.IndexOf(".prototype.") == - 1)
								{
									f.name = "";
								}
								evalStack.Push(dummy);
							}
						}
						// evalActions(f.actions);
						break;
					
					case ActionConstants.sactionCallFunction: 
						Object function = pop(evalStack);
						if (profileOffsets != null && "profile".Equals(function))
						{
							profileOffsets.Add((Int32) (offset - 13)); // Push 1
							profileOffsets.Add((Int32) (offset - 5)); // Push 'profile'
							profileOffsets.Add((Int32) offset); // CallFunction
							profileOffsets.Add((Int32) (offset + 1)); // Pop
						}
						int n = Convert.ToInt32(((System.ValueType) pop(evalStack)));
						for (int k = 0; k < n; k++)
						{
							pop(evalStack);
						}
						evalStack.Push(dummy);
						break;
					
					case ActionConstants.sactionReturn: 
						// return function() { ... } doesn't push...
						pop(evalStack);
						break;
					
					case ActionConstants.sactionModulo: 
						// pop, push
						break;
					
					case ActionConstants.sactionNewObject: 
						pop(evalStack);
						int num = Convert.ToInt32(((ValueType) pop(evalStack)));
						for (int k = 0; k < num; k++)
						{
							pop(evalStack);
						}
						evalStack.Push(dummy);
						break;
					
					case ActionConstants.sactionDefineLocal2: 
					case ActionConstants.sactionDelete2: 
					case ActionConstants.sactionAdd2: 
					case ActionConstants.sactionLess2: 
						// pop
						pop(evalStack);
						break;
					
					case ActionConstants.sactionInitArray: 
						// pop, if the first pop is non-zero, keep popping
						num = Convert.ToInt32(((ValueType) pop(evalStack)));
						for (int k = 0; k < num; k++)
						{
							pop(evalStack);
						}
						evalStack.Push(dummy);
						break;
					
					case ActionConstants.sactionInitObject: 
						num = Convert.ToInt32(((ValueType) pop(evalStack))) * 2;
						for (int k = 0; k < num; k++)
						{
							pop(evalStack);
						}
						evalStack.Push(dummy);
						break;
					
					case ActionConstants.sactionTargetPath: 
					case ActionConstants.sactionEnumerate: 
					case ActionConstants.sactionToNumber: 
					case ActionConstants.sactionToString: 
					case ActionConstants.sactionTypeOf: 
						// no action
						break;
					
					case ActionConstants.sactionStoreRegister: 
						StoreRegister r = (StoreRegister) a;
						registers[r.register] = evalStack.Peek();
						break;
					
					case ActionConstants.sactionEquals2: 
						// pop, pop, push
						// if (evalStack.size() >= 2)
						{
							pop(evalStack);
						}
						break;
					
					case ActionConstants.sactionPushDuplicate: 
						evalStack.Push(dummy);
						break;
					
					case ActionConstants.sactionStackSwap: 
						// pop, pop, push, push
						break;
					
					case ActionConstants.sactionGetMember: 
						// pop, pop, concat, push
						Object o1 = pop(evalStack);
						Object o2 = pop(evalStack);
						if (pool != null)
						{
							try
							{
								evalStack.Push(pool[Int32.Parse(o2.ToString())] + "." + pool[Int32.Parse(o1.ToString())]);
							}
							catch (Exception)
							{
								if (o1 == dummy || o2 == dummy)
								{
									evalStack.Push(dummy);
								}
								else
								{
                                    evalStack.Push(o2 + "." + o1);
								}
							}
						}
						else
						{
							evalStack.Push(o2 + "." + o1);
						}
						break;
					
					case ActionConstants.sactionSetMember: 
						// pop, pop, pop
						pop(evalStack);
						pop(evalStack);
						pop(evalStack);
						break;
					
					case ActionConstants.sactionIncrement: 
					case ActionConstants.sactionDecrement: 
						break;
					
					case ActionConstants.sactionCallMethod: 
						pop(evalStack);
						pop(evalStack);
						Object obj2 = pop(evalStack);
						if (obj2 is String)
						{
							try
							{
								n = Int32.Parse((String) obj2);
							}
							catch (FormatException)
							{
								n = 1;
							}
						}
						else
						{
							n = Convert.ToInt32(((ValueType) obj2));
						}
						for (int k = 0; k < n; k++)
						{
							pop(evalStack);
						}
						evalStack.Push(dummy);
						break;
					
					case ActionConstants.sactionNewMethod: 
						/*Object meth =*/ pop(evalStack);
						/*Object cls =*/ pop(evalStack);
						num = Convert.ToInt32(((ValueType) pop(evalStack)));
						for (int k = 0; k < num; k++)
						{
							pop(evalStack);
						}
						evalStack.Push(dummy);
						break;
					
					case ActionConstants.sactionWith: 
						// pop
						pop(evalStack);
						break;
					
					case ActionConstants.sactionConstantPool: 
						pool = ((ConstantPool) a).pool;
						// no action
						break;
					
					case ActionConstants.sactionStrictMode: 
						break;
					
					
					case ActionConstants.sactionBitAnd: 
					case ActionConstants.sactionBitOr: 
					case ActionConstants.sactionBitLShift: 
						// pop, push
						break;
					
					case ActionConstants.sactionBitXor: 
					case ActionConstants.sactionBitRShift: 
					case ActionConstants.sactionBitURShift: 
						pop(evalStack);
						break;
						
						// Flash 6 actions
					
					case ActionConstants.sactionInstanceOf: 
						pop(evalStack);
						break;
					
					case ActionConstants.sactionEnumerate2: 
						// pop, push, more pushes?
						break;
					
					case ActionConstants.sactionStrictEquals: 
					case ActionConstants.sactionGreater: 
					case ActionConstants.sactionStringGreater: 
						pop(evalStack);
						break;
						
						// FEATURE_EXCEPTIONS
					
					case ActionConstants.sactionTry: 
						// do nothing
						break;
					
					case ActionConstants.sactionThrow: 
						pop(evalStack);
						break;
						
						// FEATURE_AS2_INTERFACES
					
					case ActionConstants.sactionCastOp: 
						break;
					
					case ActionConstants.sactionImplementsOp: 
						break;
						
						// Reserved for Quicktime
					
					case ActionConstants.sactionQuickTime: 
						break;
					
					default: 
						break;
					
				}
			}
		}
		static MovieMetaData()
		{
			{
				for (int i = 0; i < 256; i++)
				{
					codes[i] = (Int32) i;
				}
			}
			{
				for (int i = 0; i < 256; i++)
				{
					registers[i] = dummy;
				}
			}
		}
	}
	
	class MFUCache
	{
		virtual internal DebugModule TopModule
		{
			get
			{
				return topModule;
			}
			
		}

        internal System.Collections.Hashtable cache = new System.Collections.Hashtable(5);
		internal DebugModule topModule;
		internal int topCount;
		
		internal virtual void  add(DebugModule m)
		{
            int count = 0;

            if (cache.Contains(m))
            {
                count = (int)cache[m];
            }
 
			cache[m] = ++count;
			
			if (count > topCount)
			{
				topCount = count;
				topModule = m;
			}
		}
	}
}
