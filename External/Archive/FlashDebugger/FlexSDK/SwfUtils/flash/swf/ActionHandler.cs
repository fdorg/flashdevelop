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
using Flash.Swf.Actions;
using Flash.Swf.Debug;

namespace Flash.Swf
{
	
	/// <author>  Clement Wong
	/// </author>
	public class ActionHandler
	{
		virtual public Action Target2
		{
			set
			{
			}
			
		}
		virtual public SetTarget Target
		{
			set
			{
			}
			
		}
		/// <summary> called before visiting each action, to indicate the offset of this
		/// action from the start of the SWF file.
		/// </summary>
		/// <param name="offset">
		/// </param>
		public virtual void  setActionOffset(int offset, Action a)
		{
		}
		
		public virtual void  nextFrame(Action action)
		{
		}
		
		public virtual void  prevFrame(Action action)
		{
		}
		
		public virtual void  play(Action action)
		{
		}
		
		public virtual void  stop(Action action)
		{
		}
		
		public virtual void  toggleQuality(Action action)
		{
		}
		
		public virtual void  stopSounds(Action action)
		{
		}
		
		public virtual void  add(Action action)
		{
		}
		
		public virtual void  subtract(Action action)
		{
		}
		
		public virtual void  multiply(Action action)
		{
		}
		
		public virtual void  divide(Action action)
		{
		}

        public virtual void  equals(Action action)
		{
		}
		
		public virtual void  less(Action action)
		{
		}
		
		public virtual void  and(Action action)
		{
		}
		
		public virtual void  or(Action action)
		{
		}
		
		public virtual void  not(Action action)
		{
		}
		
		public virtual void  stringEquals(Action action)
		{
		}
		
		public virtual void  stringLength(Action action)
		{
		}
		
		public virtual void  stringExtract(Action action)
		{
		}
		
		public virtual void  pop(Action action)
		{
		}
		
		public virtual void  toInteger(Action action)
		{
		}
		
		public virtual void  getVariable(Action action)
		{
		}
		
		public virtual void  setVariable(Action action)
		{
		}
		
		public virtual void  stringAdd(Action action)
		{
		}
		
		public virtual void  getProperty(Action action)
		{
		}
		
		public virtual void  setProperty(Action action)
		{
		}
		
		public virtual void  cloneSprite(Action action)
		{
		}
		
		public virtual void  removeSprite(Action action)
		{
		}
		
		public virtual void  trace(Action action)
		{
		}
		
		public virtual void  startDrag(Action action)
		{
		}
		
		public virtual void  endDrag(Action action)
		{
		}
		
		public virtual void  stringLess(Action action)
		{
		}
		
		public virtual void  randomNumber(Action action)
		{
		}
		
		public virtual void  mbStringLength(Action action)
		{
		}
		
		public virtual void  charToASCII(Action action)
		{
		}
		
		public virtual void  asciiToChar(Action action)
		{
		}
		
		public virtual void  getTime(Action action)
		{
		}
		
		public virtual void  mbStringExtract(Action action)
		{
		}
		
		public virtual void  mbCharToASCII(Action action)
		{
		}
		
		public virtual void  mbASCIIToChar(Action action)
		{
		}
		
		public virtual void  delete(Action action)
		{
		}
		
		public virtual void  delete2(Action action)
		{
		}
		
		public virtual void  defineLocal(Action action)
		{
		}
		
		public virtual void  callFunction(Action action)
		{
		}
		
		public virtual void  returnAction(Action action)
		{
		}
		
		public virtual void  modulo(Action action)
		{
		}
		
		public virtual void  newObject(Action action)
		{
		}
		
		public virtual void  defineLocal2(Action action)
		{
		}
		
		public virtual void  initArray(Action action)
		{
		}
		
		public virtual void  initObject(Action action)
		{
		}
		
		public virtual void  typeOf(Action action)
		{
		}
		
		public virtual void  targetPath(Action action)
		{
		}
		
		public virtual void  enumerate(Action action)
		{
		}
		
		public virtual void  add2(Action action)
		{
		}
		
		public virtual void  less2(Action action)
		{
		}
		
		public virtual void  equals2(Action action)
		{
		}
		
		public virtual void  toNumber(Action action)
		{
		}
		
		public virtual void  toString(Action action)
		{
		}
		
		public virtual void  pushDuplicate(Action action)
		{
		}
		
		public virtual void  stackSwap(Action action)
		{
		}
		
		public virtual void  getMember(Action action)
		{
		}
		
		public virtual void  setMember(Action action)
		{
		}
		
		public virtual void  increment(Action action)
		{
		}
		
		public virtual void  decrement(Action action)
		{
		}
		
		public virtual void  callMethod(Action action)
		{
		}
		
		public virtual void  newMethod(Action action)
		{
		}
		
		public virtual void  instanceOf(Action action)
		{
		}
		
		public virtual void  enumerate2(Action action)
		{
		}
		
		public virtual void  bitAnd(Action action)
		{
		}
		
		public virtual void  bitOr(Action action)
		{
		}
		
		public virtual void  bitXor(Action action)
		{
		}
		
		public virtual void  bitLShift(Action action)
		{
		}
		
		public virtual void  bitRShift(Action action)
		{
		}
		
		public virtual void  bitURShift(Action action)
		{
		}
		
		public virtual void  strictEquals(Action action)
		{
		}
		
		public virtual void  greater(Action action)
		{
		}
		
		public virtual void  stringGreater(Action action)
		{
		}
		
		public virtual void  gotoFrame(GotoFrame action)
		{
		}
		
		public virtual void  getURL(GetURL action)
		{
		}
		
		public virtual void  storeRegister(StoreRegister action)
		{
		}
		
		public virtual void  constantPool(ConstantPool action)
		{
		}
		
		public virtual void  strictMode(StrictMode action)
		{
		}
		
		public virtual void  waitForFrame(WaitForFrame action)
		{
		}
		
		public virtual void  gotoLabel(GotoLabel action)
		{
		}
		
		public virtual void  waitForFrame2(WaitForFrame action)
		{
		}
		
		public virtual void  with(With action)
		{
		}
		
		public virtual void  push(Push action)
		{
		}
		
		public virtual void  jump(Branch action)
		{
		}
		
		public virtual void  getURL2(GetURL2 action)
		{
		}
		
		public virtual void  defineFunction(DefineFunction action)
		{
		}
		
		public virtual void  defineFunction2(DefineFunction action)
		{
		}
		
		public virtual void  ifAction(Branch action)
		{
		}
		
		public virtual void  label(Label label)
		{
		}
		
		public virtual void  call(Action action)
		{
		}
		
		public virtual void  gotoFrame2(GotoFrame2 action)
		{
		}
		
		public virtual void  quickTime(Action action)
		{
		}
		
		public virtual void  unknown(Unknown action)
		{
		}
		
		public virtual void  tryAction(Try aTry)
		{
		}
		
		public virtual void  throwAction(Action aThrow)
		{
		}
		
		public virtual void  castOp(Action action)
		{
		}
		
		public virtual void  implementsOp(Action action)
		{
		}
		
		public virtual void  lineRecord(LineRecord line)
		{
		}
		
		public virtual void  registerRecord(RegisterRecord line)
		{
		}
		
		public virtual void  extendsOp(Action action)
		{
		}
		
		public virtual void  nop(Action action)
		{
		}
		
		public virtual void  halt(Action action)
		{
		}
	}
}
