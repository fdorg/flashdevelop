// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2002-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
namespace Flash.Swf
{
	public class ActionConstants {
		// Flash 1 and 2 actions
		public const int sactionHasLength = 0x80;
        public const int sactionNone = 0x00;
        public const int sactionGotoFrame = 0x81; // frame num (int)
        public const int sactionGetURL = 0x83; // url (STR), window (STR)
        public const int sactionNextFrame = 0x04;
        public const int sactionPrevFrame = 0x05;
        public const int sactionPlay = 0x06;
        public const int sactionStop = 0x07;
        public const int sactionToggleQuality = 0x08;
        public const int sactionStopSounds = 0x09;
        public const int sactionWaitForFrame = 0x8A; // frame needed (int), actions to skip (BYTE)
		// Flash 3 Actions
        public const int sactionSetTarget = 0x8B; // name (STR)
        public const int sactionGotoLabel = 0x8C; // name (STR)
		// Flash 4 Actions
        public const int sactionAdd = 0x0A; // Stack IN: number, number, OUT: number
        public const int sactionSubtract = 0x0B; // Stack IN: number, number, OUT: number
        public const int sactionMultiply = 0x0C; // Stack IN: number, number, OUT: number
        public const int sactionDivide = 0x0D; // Stack IN: dividend, divisor, OUT: number
        public const int sactionEquals = 0x0E; // Stack IN: number, number, OUT: bool
        public const int sactionLess = 0x0F; // Stack IN: number, number, OUT: bool
        public const int sactionAnd = 0x10; // Stack IN: bool, bool, OUT: bool
        public const int sactionOr = 0x11; // Stack IN: bool, bool, OUT: bool
        public const int sactionNot = 0x12; // Stack IN: bool, OUT: bool
        public const int sactionStringEquals = 0x13; // Stack IN: string, string, OUT: bool
        public const int sactionStringLength = 0x14; // Stack IN: string, OUT: number
        public const int sactionStringAdd = 0x21; // Stack IN: string, strng, OUT: string
        public const int sactionStringExtract = 0x15; // Stack IN: string, index, count, OUT: substring
        public const int sactionPush = 0x96; // type (BYTE), value (STRING or FLOAT)
        public const int sactionPop = 0x17; // no arguments
        public const int sactionToInteger = 0x18; // Stack IN: number, OUT: integer
        public const int sactionJump = 0x99; // offset (int)
        public const int sactionIf = 0x9D; // offset (int) Stack IN: bool
        public const int sactionCall = 0x9E; // Stack IN: name
        public const int sactionGetVariable = 0x1C; // Stack IN: name, OUT: value
        public const int sactionSetVariable = 0x1D; // Stack IN: name, value
        public const int sactionGetURL2 = 0x9A; // method (BYTE) Stack IN: url, window
        public const int sactionGotoFrame2 = 0x9F; // flags (BYTE) Stack IN: frame
        public const int sactionSetTarget2 = 0x20; // Stack IN: target
        public const int sactionGetProperty = 0x22; // Stack IN: target, property, OUT: value
        public const int sactionSetProperty = 0x23; // Stack IN: target, property, value
        public const int sactionCloneSprite = 0x24; // Stack IN: source, target, depth
        public const int sactionRemoveSprite = 0x25; // Stack IN: target
        public const int sactionTrace = 0x26; // Stack IN: message
        public const int sactionStartDrag = 0x27; // Stack IN: no constraint: 0, center, target
		// constraint: x1, y1, x2, y2, 1, center, target
        public const int sactionEndDrag = 0x28; // no arguments
        public const int sactionStringLess = 0x29; // Stack IN: string, string, OUT: bool
        public const int sactionWaitForFrame2 = 0x8D; // skipCount (BYTE) Stack IN: frame
        public const int sactionRandomNumber = 0x30; // Stack IN: maximum, OUT: result
        public const int sactionMBStringLength = 0x31; // Stack IN: string, OUT: length
        public const int sactionCharToAscii = 0x32; // Stack IN: character, OUT: ASCII code
        public const int sactionAsciiToChar = 0x33; // Stack IN: ASCII code, OUT: character
        public const int sactionGetTime = 0x34; // Stack OUT: milliseconds since Player start
        public const int sactionMBStringExtract = 0x35; // Stack IN: string, index, count, OUT: substring
        public const int sactionMBCharToAscii = 0x36; // Stack IN: character, OUT: ASCII code
        public const int sactionMBAsciiToChar = 0x37; // Stack IN: ASCII code, OUT: character
		// Flash 5 actions
		//unused OK to reuse --> public static final int sactionGetLastKeyCode= 0x38; // Stack OUT: code for last key pressed
        public const int sactionDelete = 0x3A; // Stack IN: name of object to delete
		public const int sactionDefineFunction = 0x9B; // name (STRING), body (BYTES)
		public const int sactionDelete2 = 0x3B; // Stack IN: name
		public const int sactionDefineLocal = 0x3C; // Stack IN: name, value
		public const int sactionCallFunction = 0x3D; // Stack IN: function, numargs, arg1, arg2, ... argn
		public const int sactionReturn = 0x3E; // Stack IN: value to return
		public const int sactionModulo = 0x3F; // Stack IN: x, y Stack OUT: x % y
		public const int sactionNewObject = 0x40; // like CallFunction but constructs object
		public const int sactionDefineLocal2 = 0x41; // Stack IN: name
		public const int sactionInitArray = 0x42; // Stack IN: //# of elems, arg1, arg2, ... argn
		public const int sactionInitObject = 0x43; // Stack IN: //# of elems, arg1, name1, ...
		public const int sactionTypeOf = 0x44; // Stack IN: object, Stack OUT: type of object
		public const int sactionTargetPath = 0x45; // Stack IN: object, Stack OUT: target path
		public const int sactionEnumerate = 0x46; // Stack IN: name, Stack OUT: children ended by null
		public const int sactionStoreRegister = 0x87; // register number (BYTE, 0-31)
		public const int sactionAdd2 = 0x47; // Like sactionAdd, but knows about types
		public const int sactionLess2 = 0x48; // Like sactionLess, but knows about types
		public const int sactionEquals2 = 0x49; // Like sactionEquals, but knows about types
		public const int sactionToNumber = 0x4A; // Stack IN: object Stack OUT: number
		public const int sactionToString = 0x4B; // Stack IN: object Stack OUT: string
		public const int sactionPushDuplicate = 0x4C; // pushes duplicate of top of stack
		public const int sactionStackSwap = 0x4D; // swaps top two items on stack
		public const int sactionGetMember = 0x4E; // Stack IN: object, name, Stack OUT: value
		public const int sactionSetMember = 0x4F; // Stack IN: object, name, value
		public const int sactionIncrement = 0x50; // Stack IN: value, Stack OUT: value+1
		public const int sactionDecrement = 0x51; // Stack IN: value, Stack OUT: value-1
		public const int sactionCallMethod = 0x52; // Stack IN: object, name, numargs, arg1, arg2, ... argn
		public const int sactionNewMethod = 0x53; // Like sactionCallMethod but constructs object
		public const int sactionWith = 0x94; // body length: int, Stack IN: object
		public const int sactionConstantPool = 0x88; // Attaches constant pool
		public const int sactionStrictMode = 0x89; // Activate/deactivate strict mode
		public const int sactionBitAnd = 0x60; // Stack IN: number, number, OUT: number
		public const int sactionBitOr = 0x61; // Stack IN: number, number, OUT: number
		public const int sactionBitXor = 0x62; // Stack IN: number, number, OUT: number
		public const int sactionBitLShift = 0x63; // Stack IN: number, number, OUT: number
		public const int sactionBitRShift = 0x64; // Stack IN: number, number, OUT: number
		public const int sactionBitURShift = 0x65; // Stack IN: number, number, OUT: number
		// Flash 6 actions
		public const int sactionInstanceOf = 0x54; // Stack IN: object, class OUT: boolean
		public const int sactionEnumerate2 = 0x55; // Stack IN: object, Stack OUT: children ended by null
		public const int sactionStrictEquals = 0x66; // Stack IN: something, something, OUT: bool
		public const int sactionGreater = 0x67; // Stack IN: something, something, OUT: bool
		public const int sactionStringGreater = 0x68; // Stack IN: something, something, OUT: bool
		// Flash 7 actions
		public const int sactionDefineFunction2 = 0x8E; // name (STRING), numParams (WORD), registerCount (BYTE)
		public const int sactionTry = 0x8F;
		public const int sactionThrow = 0x2A;
		public const int sactionCastOp = 0x2B;
		public const int sactionImplementsOp = 0x2C;
		public const int sactionExtends = 0x69; // stack in: baseclass, classname, constructor
		public const int sactionNop = 0x77; // nop
		public const int sactionHalt = 0x5F; // halt script execution
		// Reserved for Quicktime
		public const int sactionQuickTime = 0xAA; // I think this is what they are using...
		public const int kPushStringType = 0;
		public const int kPushFloatType = 1;
		public const int kPushNullType = 2;
		public const int kPushUndefinedType = 3;
		public const int kPushRegisterType = 4;
		public const int kPushBooleanType = 5;
		public const int kPushDoubleType = 6;
		public const int kPushIntegerType = 7;
		public const int kPushConstant8Type = 8;
		public const int kPushConstant16Type = 9;
		// GetURL2 methods
		
		public const int kHttpDontSend = 0x0000;
		public const int kHttpSendUseGet = 0x0001;
		public const int kHttpSendUsePost = 0x0002;
		public const int kHttpMethodMask = 0x0003;
		public const int kHttpLoadTarget = 0x0040;
		public const int kHttpLoadVariables = 0x0080;
		//    //#ifdef FAP
		//    int kHttpIsFAP = 0x0200;
		//#endif
		public const int kClipEventLoad = 0x0001;
		public const int kClipEventEnterFrame = 0x0002;
		public const int kClipEventUnload = 0x0004;
		public const int kClipEventMouseMove = 0x0008;
		public const int kClipEventMouseDown = 0x0010;
		public const int kClipEventMouseUp = 0x0020;
		public const int kClipEventKeyDown = 0x0040;
		public const int kClipEventKeyUp = 0x0080;
		public const int kClipEventData = 0x0100;
		public const int kClipEventInitialize = 0x00200;
		public const int kClipEventPress = 0x00400;
		public const int kClipEventRelease = 0x00800;
		public const int kClipEventReleaseOutside = 0x01000;
		public const int kClipEventRollOver = 0x02000;
		public const int kClipEventRollOut = 0x04000;
		public const int kClipEventDragOver = 0x08000;
		public const int kClipEventDragOut = 0x10000;
		public const int kClipEventKeyPress = 0x20000;
		public const int kClipEventConstruct = 0x40000;
		// #ifdef FEATURE_EXCEPTIONS
		public const int kTryHasCatchFlag = 1;
		public const int kTryHasFinallyFlag = 2;
		public const int kTryCatchRegisterFlag = 4;
		// #endif /* FEATURE_EXCEPTIONS */
	}
}
