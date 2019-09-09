// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2003-2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;

using Flash.Tools.Debugger.Events;

namespace Flash.Tools.Debugger
{
	/// <summary> The Session object manages all aspects of debugging session with 
	/// the Flash Player.  A program can be suspended, resumed, single
	/// stepping can be performed and state information can be obtained
	/// through this object.
	/// </summary>
	public interface Session
	{
		/// <summary> Returns the URL that identifies this Session.
		/// Note: this may not be unique across Sessions if
		/// the same launching mechanism and SWF are used.
		/// </summary>
		/// <returns> URI received from the connected Player.
		/// It identifies the debugging session 
		/// </returns>
		String URI
		{
			get;
		}
		/// <summary> Returns the Process object, if any, that triggered this Session.</summary>
		/// <returns> the Process object that was used to create this Session.
		/// If SessionManager.launch() was not used, then null is returned.
		/// </returns>
		System.Diagnostics.Process LaunchProcess
		{
			get;
		}
		/// <summary> Is the Player currently connected for this session.  This function
		/// must be thread-safe.
		/// 
		/// </summary>
		/// <returns> true if connection is alive
		/// </returns>
		bool Connected
		{
			get;
		}
		/// <summary> Is the Player currently halted awaiting requests, such as continue,
		/// stepOut, stepIn, stepOver. This function is guaranteed to be thread-safe.
		/// 
		/// </summary>
		/// <returns> true if player halted
		/// </returns>
		/// <throws>  NotConnectedException </throws>
		/// <summary>             if Player is disconnected from Session
		/// </summary>
		bool Suspended
		{
			get;
		}
		/// <summary> Returns an array of frames that identify the location and contain
		/// arguments, locals and 'this' information for each frame on the 
		/// function call stack.   The 0th frame contains the current location
		/// and context for the actionscript program.  Likewise 
		/// getFrames[getFrames().length] is the topmost (or outermost) frame
		/// of the call stack.
		/// </summary>
		/// <returns> array of call frames with 0th element representing the current frame.
		/// </returns>
		/// <throws>  NotConnectedException if Player is disconnected from Session </throws>
		Frame[] Frames
		{
			get;
		}
		/// <summary> Obtain information about the various SWF(s) that have been
		/// loaded into the Player, for this session.
		/// 
		/// Note: As SWFs are loaded by the Player a SwfLoadedEvent is 
		/// fired.  At this point, a call to getSwfInfo() will provide
		/// updated information.
		/// 
		/// </summary>
		/// <returns> array of records describing the SWFs
		/// </returns>
		/// <throws>  NoResponseException if times out </throws>
		SwfInfo[] Swfs
		{
			get;
		}
		/// <summary> Get a list of the current breakpoints.  No specific ordering
		/// of the breakpoints is implied by the array.
		/// </summary>
		/// <returns> breakpoints currently set.
		/// </returns>
		/// <throws>  NoResponseException if times out </throws>
		/// <throws>  NotConnectedException if Player is disconnected from Session </throws>
		Location[] BreakpointList
		{
			get;
		}
		/// <summary> Get a list of the current watchpoint.  No specific ordering
		/// of the watchpoints is implied by the array.  Also, the 
		/// list may contain watchpoints that are no longer relevant due
		/// to the variable going out of scope. 
		/// </summary>
		/// <returns> watchpoints currently set.
		/// </returns>
		/// <throws>  NoResponseException if times out </throws>
		/// <throws>  NotConnectedException if Player is disconnected from Session </throws>
		/// <since> Version 2
		/// </since>
		Watch[] WatchList
		{
			get;
		}
		/// <summary> Obtains a list of variables that are local to the current
		/// halted state.
		/// </summary>
		/// <deprecated> As of version 2. 
		/// </deprecated>
		/// <seealso cref="Frame.getLocals">
		/// </seealso>
		Variable[] VariableList
		{
			get;
		}
		/// <summary> Returns the number of events currently in the queue.  This function
		/// is guaranteed to be thread-safe.
		/// </summary>
		int EventCount
		{
			get;
		}
		/// <summary> Gets the SourceLocator for this session.  If none has been
		/// specified, returns null.
		/// </summary>
		/// <summary> Sets the SourceLocator for this session.  This can be used in order
		/// to override the default rules used for finding source files.
		/// </summary>
		SourceLocator SourceLocator
		{
			get;
			
			set;
		}

		/// <summary> Adjust the preferences for this session; see SessionManager
		/// for a list of valid preference strings.
		/// 
		/// If an invalid preference is passed, it will be silently ignored.
		/// </summary>
		/// <param name="pref">preference name, one of the strings listed above
		/// </param>
		/// <param name="value">value to set for preference
		/// </param>
		void  setPreference(String pref, int value);
		
		/// <summary> Return the value of a particular preference item
		/// 
		/// </summary>
		/// <param name="pref">preference name, one of the strings listed in <code>SessionManager</code>
		/// </param>
		/// <throws>  NullPointerException if pref does not exist </throws>
		/// <seealso cref="SessionManager">
		/// </seealso>
		int getPreference(String pref);
		
		/// <summary> Allow the session to start communicating with the player.  This
		/// call must be made PRIOR to any other Session method call.
		/// </summary>
		/// <returns> true if bind was successful.
		/// </returns>
		/// <throws>  VersionException connected to Player which does not support all API completely </throws>
		bool bind();
		
		/// <summary> Permanently stops the debugging session and breaks the 
		/// connection.  If this Session is used for any subsequent
		/// calls exceptions will be thrown.
		/// <p />
		/// Note: this method allows the caller to disconnect
		/// from the debugging session (and Player) without 
		/// terminating the Player.  A subsequent call to terminate() 
		/// will destroy the Player process.
		/// <p />
		/// Under normal circumstances this method need not be
		/// called since a call to terminate() performs both 
		/// actions of disconnecting from the Player and destroying
		/// the Player process.
		/// </summary>
		void  unbind();
		
		/// <summary> Permanently stops the debugging session and breaks the connection. If
		/// this session ID is used for any subsequent calls exceptions will be
		/// thrown.
		/// <p />
		/// Note that due to platform and browser differences, it should not be
		/// assumed that this function will necessarily kill the process being
		/// debugged. For example:
		/// 
		/// <ul>
		/// <li> On all platforms, Firefox cannot be terminated. This is because when
		/// we launch a new instance of Firefox, Firefox actually checks to see if
		/// there is another already-running instance. If there is, then the new
		/// instance just passes control to that old instance. So, the debugger
		/// doesn't know the process ID of the browser. It would be bad to attempt to
		/// figure out the PID and then kill that process, because the user might
		/// have other browser windows open that they don't want to lose. </li>
		/// <li> On Mac, similar problems apply to the Safari and Camino browsers:
		/// all browsers are launched with /usr/bin/open, so we never know the
		/// process ID, and we can't kill it. However, for Safari and Camino, what we
		/// do attempt to do is communicate with the browser via AppleScript, and
		/// tell it to close the window of the program that is being debugged. </li>
		/// </ul>
		/// 
		/// <p />
		/// If SessionManager.launch() was used to initiate the Session then calling
		/// this function also causes getLaunchProcess().destroy() to be called.
		/// <p />
		/// Note: this method first calls unbind() if needed.
		/// </summary>
		void  terminate();
		
		/// <summary> Continue a halted session.  Execution of the ActionScript
		/// will commence until a reason for halting exists. That
		/// is, a breakpoint is reached or the <code>suspend()</code> method is called.
		/// <p />
		/// This method will NOT block.  It will return immediately 
		/// after the Player resumes execution.  Use the isSuspended
		/// method to determine when the Player has halted.
		/// 
		/// </summary>
		/// <throws>  NoResponseException if times out </throws>
		/// <throws>  NotSuspendedException if Player is already running </throws>
		/// <throws>  NotConnectedException if Player is disconnected from Session </throws>
		void  resume();
		
		/// <summary> Halt a running session.  Execution of the ActionScript
		/// will stop at the next possible breakpoint.
		/// <p /> 
		/// This method WILL BLOCK until the Player halts for some
		/// reason or an error occurs. During this period, one or
		/// more callbacks may be initiated.
		/// 
		/// </summary>
		/// <throws>  NoResponseException if times out </throws>
		/// <throws>  SuspendedException if Player is already suspended </throws>
		/// <throws>  NotConnectedException if Player is disconnected from Session </throws>
		void  suspend();
		
		/// <summary> Returns a SuspendReason integer which indicates
		/// why the Player has suspended execution.
		/// </summary>
		/// <returns> see SuspendReason
		/// </returns>
		/// <throws>  NotConnectedException if Player is disconnected from Session </throws>
		int suspendReason();
		
		/// <summary> Step to the next executable source line within the 
		/// program, will enter into functions.
		/// <p />
		/// This method will NOT block.  It will return immediately 
		/// after the Player resumes execution.  Use the isSuspended
		/// method to determine when the Player has halted.
		/// 
		/// </summary>
		/// <throws>  NoResponseException if times out </throws>
		/// <throws>  NotSuspendedException if Player is running </throws>
		/// <throws>  NotConnectedException if Player is disconnected from Session </throws>
		void  stepInto();
		
		/// <summary> Step out of the current method/function onto the 
		/// next executable soruce line.
		/// <p />
		/// This method will NOT block.  It will return immediately 
		/// after the Player resumes execution.  Use the isSuspended
		/// method to determine when the Player has halted.
		/// 
		/// </summary>
		/// <throws>  NoResponseException if times out </throws>
		/// <throws>  NotSuspendedException if Player is running </throws>
		/// <throws>  NotConnectedException if Player is disconnected from Session </throws>
		void  stepOut();
		
		/// <summary> Step to the next executable source line within
		/// the program, will NOT enter into functions. 
		/// <p />
		/// This method will NOT block.  It will return immediately 
		/// after the Player resumes execution.  Use the isSuspended
		/// method to determine when the Player has halted.
		/// 
		/// </summary>
		/// <throws>  NoResponseException if times out </throws>
		/// <throws>  NotSuspendedException if Player is running </throws>
		/// <throws>  NotConnectedException if Player is disconnected from Session </throws>
		void  stepOver();
		
		/// <summary> Continue the process of stepping.  
		/// This call should only be issued if a previous
		/// stepXXX() call was made and the Player suspended
		/// execution due to a breakpoint being hit.
		/// That is getSuspendReason() == SuspendReason.Break
		/// This operation can be used for assisting with 
		/// the processing of conditional breakpoints.
		/// </summary>
		/// <throws>  NoResponseException if times out </throws>
		/// <throws>  NotSuspendedException if Player is running </throws>
		/// <throws>  NotConnectedException if Player is disconnected from Session </throws>
		void  stepContinue();
		
		/// <summary> Set a breakpoint on a line within the given file.
		/// <p />
		/// <em>Warning:</em> <code>setBreakpoint()</code> and
		/// <code>clearBreakpoint()</code> do not keep track of how many times they
		/// have been called for a given Location. For example, if you make two calls
		/// to <code>setBreakpoint()</code> for file X.as line 10, and then one
		/// call to <code>clearBreakpoint()</code> for that same file and line,
		/// then the breakpoint is gone. So, the caller is responsible for keeping
		/// track of whether the user has set two breakpoints at the same location.
		/// 
		/// </summary>
		/// <returns> null if breakpoint not set, otherwise
		/// Location of breakpoint.
		/// </returns>
		/// <throws>  NoResponseException if times out </throws>
		/// <throws>  NotConnectedException if Player is disconnected from Session </throws>
		Location setBreakpoint(int fileId, int lineNum);
		
		/// <summary> Remove a breakpoint at given location. The Location obtain can be a
		/// clone/copy of a Location object returned from a previous call to
		/// getBreakpointList().
		/// <p />
		/// <em>Warning:</em> <code>setBreakpoint()</code> and
		/// <code>clearBreakpoint()</code> do not keep track of how many times they
		/// have been called for a given Location. For example, if you make two calls
		/// to <code>setBreakpoint()</code> for file X.as line 10, and then one
		/// call to <code>clearBreakpoint()</code> for that same file and line,
		/// then the breakpoint is gone. So, the caller is responsible for keeping
		/// track of whether the user has set two breakpoints at the same location.
		/// 
		/// </summary>
		/// <returns> null if breakpoint was not removed.
		/// </returns>
		/// <throws>  NoResponseException </throws>
		/// <summary>             if times out
		/// </summary>
		/// <throws>  NotConnectedException </throws>
		/// <summary>             if Player is disconnected from Session
		/// </summary>
		Location clearBreakpoint(Location location);
		
		/// <summary> Set a watchpoint on a given variable.  A watchpoint is used 
		/// to suspend Player execution upon access of a particular variable.
		/// If the variable upon which the watchpoint is set goes out of scope,
		/// the watchpoint will NOT be automatically removed. 
		/// <p />
		/// Specification of the variable item to be watched requires two
		/// pieces of information (similar to setScalarMember())
		/// The Variable and the name of the particular member to be watched 
		/// within the variable.
		/// For example if the watchpoint is to be applied to 'a.b.c'.  First the
		/// Value for object 'a.b' must be obtained and then the call
		/// setWatch(v, "c", ...) can be issued.
		/// The watchpoint can be triggered (i.e. the Player suspended) when either a read
		/// or write (or either) occurs on the variable.  If the Player is suspended 
		/// due to a watchpoint being fired, then the suspendReason() call will
		/// return SuspendReason.WATCH.
		/// <p />
		/// Setting a watchpoint multiple times on the same variable will result 
		/// in the old watchpoint being removed from the list and a new watchpoint
		/// being added to the end of the list.
		/// <p />
		/// Likewise, if a previously existing watchpoint is modified by
		/// specifiying a different kind variable then the old watchpoint
		/// will be removed from the list and a new watchpoint will be added
		/// to the end of the list.
		/// 
		/// </summary>
		/// <param name="v">the variable, upon whose member, the watch is to be placed.
		/// </param>
		/// <param name="memberName">is the mmeber name upon which the watch 
		/// should be placed.  This variable name may NOT contain the dot ('.')
		/// character and MUST be a member of v.
		/// </param>
		/// <param name="kind">access type that will trigger the watchpoint to fire --
		/// read, write, or read/write.  See <code>WatchKind</code>.
		/// </param>
		/// <returns> null if watchpoint was not created.
		/// </returns>
		/// <throws>  NoResponseException if times out </throws>
		/// <throws>  NotConnectedException if Player is disconnected from Session </throws>
		/// <since> Version 2
		/// </since>
		/// <seealso cref="WatchKind">
		/// </seealso>
		Watch setWatch(Value v, String memberName, int kind);
		
		/// <summary> Remove a previously created watchpoint.  The watchpoint
		/// that was removed will be returned upon a sucessful call.
		/// </summary>
		/// <returns> null if watchpoint was not removed.
		/// </returns>
		/// <throws>  NoResponseException if times out </throws>
		/// <throws>  NotConnectedException if Player is disconnected from Session </throws>
		/// <since> Version 2
		/// </since>
		Watch clearWatch(Watch watch);
		
		/// <summary> From a given value identifier return a Value.  This call
		/// allows tools to access a specific value whenever the Player has
		/// suspended.  A Value's id is maintained for the life of the 
		/// Value and is guaranteed not to change.  Values that
		/// go out of scope are no longer accessible and will result
		/// in a null being returned.   Also note, that scalar
		/// variables do not contain an id that can be referenced in 
		/// this manner.  Therefore the caller must also maintain the
		/// 'context' in which the variable was obtained.  For example
		/// if a Number b exists on a, then the reference 'a.b' must be
		/// managed, as the id of 'a' will be needed to obtain the
		/// value of 'b'.
		/// </summary>
		/// <param name="valueId">identifier from Value class or
		/// from a call to Value.getId()
		/// </param>
		/// <returns> null, if value cannot be found or 
		/// value with the specific id.
		/// </returns>
		/// <throws>  NoResponseException if times out </throws>
		/// <throws>  NotSuspendedException if Player is running </throws>
		/// <throws>  NotConnectedException if Player is disconnected from Session </throws>
		Value getValue(int valueId);
		
		/// <summary> Events provide a mechanism whereby status information is provided from
		/// the Player in a timely fashion.
		/// <p />
		/// The caller has the option of either polling the event queue via
		/// <code>nextEvent()</code> or calling <code>waitForEvent()</code> which
		/// blocks the calling thread until one or more events exist in the queue.
		/// 
		/// </summary>
		/// <throws>  NotConnectedException </throws>
		/// <summary>             if Session is disconnected from Player
		/// </summary>
		/// <throws>  InterruptedException </throws>
		void  waitForEvent();
		
		/// <summary> Removes and returns the next event from queue</summary>
		DebugEvent nextEvent();
	}
}
