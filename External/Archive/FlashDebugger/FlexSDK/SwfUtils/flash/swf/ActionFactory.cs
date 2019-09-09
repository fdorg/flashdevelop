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
using Push = Flash.Swf.Actions.Push;
using StoreRegister = Flash.Swf.Actions.StoreRegister;
using StrictMode = Flash.Swf.Actions.StrictMode;
using Label = Flash.Swf.Actions.Label;
using WaitForFrame = Flash.Swf.Actions.WaitForFrame;
using ActionList = Flash.Swf.Types.ActionList;
using LineRecord = Flash.Swf.Debug.LineRecord;
using RegisterRecord = Flash.Swf.Debug.RegisterRecord;
namespace Flash.Swf
{
	
	/// <summary> This is a factory for decoding ActionScript bytecode.  It keeps track of temporary
	/// information we need while decoding but can discard once we are done.
	/// </summary>
	/// <author>  Edwin Smith
	/// </author>
	sealed public class ActionFactory
	{
		internal class AnonymousClassObject:System.Object
		{
			public override String ToString()
			{
				return "undefined";
			}
		}
		internal class AnonymousClassObject1:System.Object
		{
			public override String ToString()
			{
				return "stack";
			}
		}
		public static readonly System.Object UNDEFINED;
		
		public static readonly System.Object STACKTOP;
		
		/// <summary>flyweight action objects for 1-byte opcodes 0..7F </summary>
		private static readonly Action[] actionFlyweights = new Action[0x80];
		private static readonly Push[] pushCpoolFlyweights = new Push[256];
		private static readonly Push[] pushRegisterFlyweights = new Push[256];
		private static readonly StoreRegister[] storeRegisterFlyweights = new StoreRegister[256];
		private static readonly Push pushTrueFlyweight = new Push((System.Object) true);
		private static readonly Push pushFalseFlyweight = new Push((System.Object) false);
		private static readonly Push pushUndefinedFlyweight = new Push(UNDEFINED);
		private static readonly Push pushNullFlyweight = new Push((System.Object) null);
		private static readonly Push pushFloat0Flyweight = new Push((System.Object) 0);
		private static readonly Push pushInteger0Flyweight = new Push((System.Object) 0);
		private static readonly Push pushDouble0Flyweight = new Push((System.Object) 0);
		private static readonly Action callFlyweight = new Action(Flash.Swf.ActionConstants.sactionCall);
		private static readonly StrictMode strictTrueFlyweight = new StrictMode(true);
		private static readonly StrictMode strictFalseFlyweight = new StrictMode(false);
		
		public static Action createAction(int code)
		{
			return actionFlyweights[code];
		}
		
		public static Push createPushCpool(int index)
		{
			return (index < pushCpoolFlyweights.Length)?pushCpoolFlyweights[index]:new Push((System.Object) index);
		}
		
		public static Push createPush(String s)
		{
			return new Push(s);
		}
		
		public static Push createPush(float fvalue)
		{
			return fvalue == 0?pushFloat0Flyweight:new Push((System.Object) fvalue);
		}
		
		public static Push createPushNull()
		{
			return pushNullFlyweight;
		}
		
		public static Push createPushUndefined()
		{
			return pushUndefinedFlyweight;
		}
		
		public static Push createPushRegister(int regno)
		{
			return pushRegisterFlyweights[regno];
		}
		
		public static Push createPush(bool b)
		{
			return (b?pushTrueFlyweight:pushFalseFlyweight);
		}
		
		public static Push createPush(double dvalue)
		{
			return dvalue == 0?pushDouble0Flyweight:new Push((System.Object) dvalue);
		}
		
		public static Push createPush(int ivalue)
		{
			return ivalue == 0?pushInteger0Flyweight:new Push((System.Object) ivalue);
		}
		
		public static StoreRegister createStoreRegister(int register)
		{
			return storeRegisterFlyweights[register];
		}
		
		public static Action createCall()
		{
			return callFlyweight;
		}
		
		public static StrictMode createStrictMode(bool mode)
		{
			return mode?strictTrueFlyweight:strictFalseFlyweight;
		}
		
		private int startOffset;
		private int startCount;
		private Action[] actions;
		private Label[] labels;
		private LineRecord[] lines;
		private RegisterRecord[] registers;
		private int[] actionOffsets;
		private int count;
		private System.Collections.IList skipRecords;
		
		public ActionFactory(int length, int startOffset, int startCount)
		{
			this.startOffset = startOffset;
			this.startCount = startCount;
			
			labels = new Label[length + 1]; // length+1 to handle labels after last action
			lines = new LineRecord[length];
			registers = new RegisterRecord[length];
			actions = new Action[length];
			actionOffsets = new int[length + 1];
			skipRecords = new System.Collections.ArrayList();
		}
		
		public void  setLine(int offset, LineRecord line)
		{
			int i = offset - startOffset;
			if (lines[i] == null)
				count++;
			lines[i] = line;
		}
		
		public void  setRegister(int offset, RegisterRecord record)
		{
			int i = offset - startOffset;
			if (registers[i] == null)
				count++;
			registers[i] = record;
		}
		
		public void  setAction(int offset, Action a)
		{
			int i = offset - startOffset;
			if (actions[i] == null)
				count++;
			actions[i] = a;
		}
		
		public Label getLabel(int target)
		{
			int i = target - startOffset;
			Label label = labels[i];
			if (label == null)
			{
				labels[i] = label = new Label();
				count++;
			}
			return label;
		}
		
		public void  setActionOffset(int actionCount, int offset)
		{
			actionOffsets[actionCount - startCount] = offset;
		}
		
		/// <summary> now that everything has been decoded, build a single actionlist
		/// with the labels and jump targets merged in.
		/// </summary>
		/// <param name="keepOffsets">
		/// </param>
		/// <returns>
		/// </returns>
		public ActionList createActionList(bool keepOffsets)
		{
			processSkipEntries();
			
			ActionList list = new ActionList(keepOffsets);
			list.grow(count);
			Action a;
			int length = actions.Length;
			if (keepOffsets)
			{
				for (int i = 0; i < length; i++)
				{
					int offset = startOffset + i;
					if ((a = actions[i]) != null)
						list.insert(offset, a);
					if ((a = lines[i]) != null)
						list.insert(offset, a);
					if ((a = registers[i]) != null)
						list.insert(offset, a);
					if ((a = labels[i]) != null)
						list.insert(offset, a);
				}
				if ((a = labels[length]) != null)
					list.insert(startOffset + length, a);
			}
			else
			{
				for (int i = 0; i < length; i++)
				{
					if ((a = labels[i]) != null)
						list.append(a);
					if ((a = lines[i]) != null)
						list.append(a);
					if ((a = registers[i]) != null)
						list.append(a);
					if ((a = actions[i]) != null)
						list.append(a);
				}
				if ((a = labels[length]) != null)
					list.append(a);
			}
			return list;
		}
		
		private class SkipEntry
		{
			internal WaitForFrame action;
			internal int skipTarget;
			
			public SkipEntry(WaitForFrame action, int skipTarget)
			{
				this.action = action;
				this.skipTarget = skipTarget;
			}
		}
		
		/// <summary> postprocess skip records now that we now the offset of each encoded action</summary>
		private void  processSkipEntries()
		{
            foreach (SkipEntry skipRecord in skipRecords)
			{
				int labelOffset = actionOffsets[skipRecord.skipTarget - startCount];
				skipRecord.action.skipTarget = getLabel(labelOffset);
			}
		}
		
		public void  addSkipEntry(WaitForFrame a, int skipTarget)
		{
			skipRecords.Add(new SkipEntry(a, skipTarget));
		}
		static ActionFactory()
		{
			UNDEFINED = new AnonymousClassObject();
			STACKTOP = new AnonymousClassObject1();
			{
				for (int i = 0; i < 0x80; i++)
				{
					ActionFactory.actionFlyweights[i] = new Action(i);
				}
				
				for (int i = 0; i < 256; i++)
				{
					ActionFactory.pushRegisterFlyweights[i] = new Push((System.Object) i);
					ActionFactory.pushCpoolFlyweights[i] = new Push((System.Object) i);
					ActionFactory.storeRegisterFlyweights[i] = new StoreRegister(i);
				}
			}
		}
	}
}
