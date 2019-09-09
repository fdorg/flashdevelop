// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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
	/// <summary> Used for optimized debugger protocol.</summary>
	public abstract class DebugTags
	{
        // Breakpoint info
        public readonly static int kDebugScript = 0;
        public readonly static int kDebugOffset = 1;
        public readonly static int kDebugBreakpoint = 2;
        public readonly static int kDebugID = 3;
        // --Recycle Bin (2/7/03 9:52 AM): public static final int kDebugTree = 4;
        public readonly static int kDebugRegisters = 5;

		// --Recycle Bin START (2/7/03 9:52 AM):
		//    // AT -> Player
		//    public static final int OutZoomIn = 1;
		// --Recycle Bin STOP (2/7/03 9:52 AM)
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutZoomOut = 2;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutZoom100 = 3;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutHome = 4;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutSetQuality = 5;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutPlay = 6;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutLoop = 7;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutRewind = 8;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutForward = 9;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutBack = 10;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutPrint = 11;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutSetVariable = 12;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutSetProperty = 13;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutExit = 14;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutSetFocus = 15;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutContinue = 16;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutStopDebug = 17;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutSetBreakpoints = 18;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutRemoveBreakpoints = 19;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutRemoveAllBreakpoints = 20;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutStepOver = 21;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutStepInto = 22;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutStepOut = 23;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int OutProcessedTags = 24;
		
		// --Recycle Bin START (2/7/03 9:52 AM):
		//    // Player -> AT
		//    public static final int InSetMenuState = 25;
		// --Recycle Bin STOP (2/7/03 9:52 AM)
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InSetProperty = 26;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InExit = 27;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InNewObject = 28;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InRemoveObject = 29;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InTrace = 30;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InErrorTarget = 31;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InErrorExecLimit = 32;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InErrorWith = 33;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InErrorProtoLimit = 34;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InSetVariable = 35;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InDeleteVariable = 36;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InParam = 37;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InPlaceObject = 38;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InScript = 39;
		// --Recycle Bin START (2/7/03 9:50 AM):
		//    // --Recycle Bin (2/7/03 9:50 AM): public static final int InAskBreakpoints = 40;
		//    public static final int InBreakAt = 41;
		// --Recycle Bin STOP (2/7/03 9:50 AM)
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InContinue = 42;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InSetLocalVariables = 43;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InSetBreakpoints = 44;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InSetJumpbar = 45;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InNumScripts = 46;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InRemoveScript = 47;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InRemoveBreakpoints = 48;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InNotSynced = 49;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InErrorURLOpen = 50;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InProcessTags = 51;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int InVersion = 52;
		
		// --Recycle Bin START (2/7/03 9:52 AM):
		//    // used in DebugRecorder (write), and DebuggerPanel (read)
		//    public static final int kTreeLine = 1;
		// --Recycle Bin STOP (2/7/03 9:52 AM)
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kTreeModule = 2;
		
		// --Recycle Bin START (2/7/03 9:52 AM):
		//    // Menu item indices
		//    public static final int kZoomInIndex = 1;
		// --Recycle Bin STOP (2/7/03 9:52 AM)
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kZoomOutIndex = 2;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kZoom100Index = 3;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kShowAllIndex = 4;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kLowQualityIndex = 5;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kMediumQualityIndex = 6;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kHighQualityIndex = 7;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kPlayIndex = 8;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kLoopIndex = 9;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kRewindIndex = 10;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kForwardIndex = 11;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kBackwardIndex = 12;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kPrintIndex = 13;
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kNumMenuItems = 14;
		
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kZoomInMask = (1 << kZoomInIndex);
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kZoomOutMask = (1 << kZoomOutIndex);
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kZoom100Mask = (1 << kZoom100Index);
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kShowAllMask = (1 << kShowAllIndex);
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kLowQualityMask = (1 << kLowQualityIndex);
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kMediumQualityMask = (1 << kMediumQualityIndex);
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kHighQualityMask = (1 << kHighQualityIndex);
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kPlayMask = (1 << kPlayIndex);
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kLoopMask = (1 << kLoopIndex);
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kRewindMask = (1 << kRewindIndex);
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kForwardMask = (1 << kForwardIndex);
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kBackwardMask = (1 << kBackwardIndex);
		// --Recycle Bin (2/7/03 9:52 AM): public static final int kPrintMask = (1 << kPrintIndex);
	}
}
