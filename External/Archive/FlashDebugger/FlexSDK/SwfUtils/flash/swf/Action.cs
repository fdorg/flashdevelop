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

namespace Flash.Swf
{
	
	/// <summary> Base class for all actionscript opcodes</summary>
	/// <author>  Clement Wong
	/// </author>
	public class Action : ActionConstants
	{
		public Action(int code)
		{
			this.code = code;
		}
		
		public int code;
		
		/// <summary> Subclasses implement this method to callback one of the methods in ActionHandler...</summary>
		/// <param name="h">
		/// </param>
		public virtual void  visit(ActionHandler h)
		{
			switch (code)
			{
				
				case Flash.Swf.ActionConstants.sactionNextFrame:  h.nextFrame(this); break;
				
				case Flash.Swf.ActionConstants.sactionPrevFrame:  h.prevFrame(this); break;
				
				case Flash.Swf.ActionConstants.sactionPlay:  h.play(this); break;
				
				case Flash.Swf.ActionConstants.sactionStop:  h.stop(this); break;
				
				case Flash.Swf.ActionConstants.sactionToggleQuality:  h.toggleQuality(this); break;
				
				case Flash.Swf.ActionConstants.sactionStopSounds:  h.stopSounds(this); break;
				
				case Flash.Swf.ActionConstants.sactionAdd:  h.add(this); break;
				
				case Flash.Swf.ActionConstants.sactionSubtract:  h.subtract(this); break;
				
				case Flash.Swf.ActionConstants.sactionMultiply:  h.multiply(this); break;
				
				case Flash.Swf.ActionConstants.sactionDivide:  h.divide(this); break;
				
				case Flash.Swf.ActionConstants.sactionEquals:  h.equals(this); break;
				
				case Flash.Swf.ActionConstants.sactionLess:  h.less(this); break;
				
				case Flash.Swf.ActionConstants.sactionAnd:  h.and(this); break;
				
				case Flash.Swf.ActionConstants.sactionOr:  h.or(this); break;
				
				case Flash.Swf.ActionConstants.sactionNot:  h.not(this); break;
				
				case Flash.Swf.ActionConstants.sactionStringEquals:  h.stringEquals(this); break;
				
				case Flash.Swf.ActionConstants.sactionStringLength:  h.stringLength(this); break;
				
				case Flash.Swf.ActionConstants.sactionStringExtract:  h.stringExtract(this); break;
				
				case Flash.Swf.ActionConstants.sactionPop:  h.pop(this); break;
				
				case Flash.Swf.ActionConstants.sactionToInteger:  h.toInteger(this); break;
				
				case Flash.Swf.ActionConstants.sactionGetVariable:  h.getVariable(this); break;
				
				case Flash.Swf.ActionConstants.sactionSetVariable:  h.setVariable(this); break;
				
				case Flash.Swf.ActionConstants.sactionSetTarget2:  h.Target2 = this; break;
				
				case Flash.Swf.ActionConstants.sactionStringAdd:  h.stringAdd(this); break;
				
				case Flash.Swf.ActionConstants.sactionGetProperty:  h.getProperty(this); break;
				
				case Flash.Swf.ActionConstants.sactionSetProperty:  h.setProperty(this); break;
				
				case Flash.Swf.ActionConstants.sactionCloneSprite:  h.cloneSprite(this); break;
				
				case Flash.Swf.ActionConstants.sactionRemoveSprite:  h.removeSprite(this); break;
				
				case Flash.Swf.ActionConstants.sactionTrace:  h.trace(this); break;
				
				case Flash.Swf.ActionConstants.sactionStartDrag:  h.startDrag(this); break;
				
				case Flash.Swf.ActionConstants.sactionEndDrag:  h.endDrag(this); break;
				
				case Flash.Swf.ActionConstants.sactionStringLess:  h.stringLess(this); break;
				
				case Flash.Swf.ActionConstants.sactionThrow:  h.throwAction(this); break;
				
				case Flash.Swf.ActionConstants.sactionCastOp:  h.castOp(this); break;
				
				case Flash.Swf.ActionConstants.sactionImplementsOp:  h.implementsOp(this); break;
				
				case Flash.Swf.ActionConstants.sactionRandomNumber:  h.randomNumber(this); break;
				
				case Flash.Swf.ActionConstants.sactionMBStringLength:  h.mbStringLength(this); break;
				
				case Flash.Swf.ActionConstants.sactionCharToAscii:  h.charToASCII(this); break;
				
				case Flash.Swf.ActionConstants.sactionAsciiToChar:  h.asciiToChar(this); break;
				
				case Flash.Swf.ActionConstants.sactionGetTime:  h.getTime(this); break;
				
				case Flash.Swf.ActionConstants.sactionMBStringExtract:  h.mbStringExtract(this); break;
				
				case Flash.Swf.ActionConstants.sactionMBCharToAscii:  h.mbCharToASCII(this); break;
				
				case Flash.Swf.ActionConstants.sactionMBAsciiToChar:  h.mbASCIIToChar(this); break;
				
				case Flash.Swf.ActionConstants.sactionDelete:  h.delete(this); break;
				
				case Flash.Swf.ActionConstants.sactionDelete2:  h.delete2(this); break;
				
				case Flash.Swf.ActionConstants.sactionDefineLocal:  h.defineLocal(this); break;
				
				case Flash.Swf.ActionConstants.sactionCallFunction:  h.callFunction(this); break;
				
				case Flash.Swf.ActionConstants.sactionReturn:  h.returnAction(this); break;
				
				case Flash.Swf.ActionConstants.sactionModulo:  h.modulo(this); break;
				
				case Flash.Swf.ActionConstants.sactionNewObject:  h.newObject(this); break;
				
				case Flash.Swf.ActionConstants.sactionDefineLocal2:  h.defineLocal2(this); break;
				
				case Flash.Swf.ActionConstants.sactionInitArray:  h.initArray(this); break;
				
				case Flash.Swf.ActionConstants.sactionInitObject:  h.initObject(this); break;
				
				case Flash.Swf.ActionConstants.sactionTypeOf:  h.typeOf(this); break;
				
				case Flash.Swf.ActionConstants.sactionTargetPath:  h.targetPath(this); break;
				
				case Flash.Swf.ActionConstants.sactionEnumerate:  h.enumerate(this); break;
				
				case Flash.Swf.ActionConstants.sactionAdd2:  h.add2(this); break;
				
				case Flash.Swf.ActionConstants.sactionLess2:  h.less2(this); break;
				
				case Flash.Swf.ActionConstants.sactionEquals2:  h.equals2(this); break;
				
				case Flash.Swf.ActionConstants.sactionToNumber:  h.toNumber(this); break;
				
				case Flash.Swf.ActionConstants.sactionToString:  h.toString(this); break;
				
				case Flash.Swf.ActionConstants.sactionPushDuplicate:  h.pushDuplicate(this); break;
				
				case Flash.Swf.ActionConstants.sactionStackSwap:  h.stackSwap(this); break;
				
				case Flash.Swf.ActionConstants.sactionGetMember:  h.getMember(this); break;
				
				case Flash.Swf.ActionConstants.sactionSetMember:  h.setMember(this); break;
				
				case Flash.Swf.ActionConstants.sactionIncrement:  h.increment(this); break;
				
				case Flash.Swf.ActionConstants.sactionDecrement:  h.decrement(this); break;
				
				case Flash.Swf.ActionConstants.sactionCallMethod:  h.callMethod(this); break;
				
				case Flash.Swf.ActionConstants.sactionNewMethod:  h.newMethod(this); break;
				
				case Flash.Swf.ActionConstants.sactionInstanceOf:  h.instanceOf(this); break;
				
				case Flash.Swf.ActionConstants.sactionEnumerate2:  h.enumerate2(this); break;
				
				case Flash.Swf.ActionConstants.sactionBitAnd:  h.bitAnd(this); break;
				
				case Flash.Swf.ActionConstants.sactionBitOr:  h.bitOr(this); break;
				
				case Flash.Swf.ActionConstants.sactionBitXor:  h.bitXor(this); break;
				
				case Flash.Swf.ActionConstants.sactionBitLShift:  h.bitLShift(this); break;
				
				case Flash.Swf.ActionConstants.sactionBitRShift:  h.bitRShift(this); break;
				
				case Flash.Swf.ActionConstants.sactionBitURShift:  h.bitURShift(this); break;
				
				case Flash.Swf.ActionConstants.sactionStrictEquals:  h.strictEquals(this); break;
				
				case Flash.Swf.ActionConstants.sactionGreater:  h.greater(this); break;
				
				case Flash.Swf.ActionConstants.sactionStringGreater:  h.stringGreater(this); break;
				
				case Flash.Swf.ActionConstants.sactionCall:  h.call(this); break;
				
				case Flash.Swf.ActionConstants.sactionQuickTime:  h.quickTime(this); break;
				
				case Flash.Swf.ActionConstants.sactionExtends:  h.extendsOp(this); break;
				
				case Flash.Swf.ActionConstants.sactionNop:  h.nop(this); break;
				
				case Flash.Swf.ActionConstants.sactionHalt:  h.halt(this); break;
				
				default: 
					System.Diagnostics.Debug.Assert(false, "unexpected action " + code); // should not get here
					break;
				
			}
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is Action)
			{
				Action action = (Action) object_Renamed;
				
				if (action.code == this.code)
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		protected internal virtual bool equals(System.Object a, System.Object b)
		{
			return a == b || a != null && a.Equals(b);
		}
		
		public override int GetHashCode()
		{
			return code;
		}
		
		public virtual int objectHashCode()
		{
			return base.GetHashCode();
		}
		
		public override String ToString()
		{
			return GetType().FullName + "[ code = " + code + " ]";
		}
	}
}
