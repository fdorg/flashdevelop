// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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

namespace Flash.Tools.Debugger
{
	
	/// <summary> A SessionManager controls connection establishment and preferences 
	/// for all debugging sessions with the Flash Player.
	/// 
	/// To begin a new debugging session:
	/// 
	/// <ol>
	/// <li> Get a <code>SessionManager</code> from <code>Bootstrap.sessionManager()</code> </li>
	/// <li> Call <code>SessionManager.startListening()</code> </li>
	/// <li> If you want to have the API launch the Flash Player for you, call
	/// <code>SessionManager.launch()</code>.  If you want to launch the Flash Player
	/// and then have the API connect to it, then launch the Flash Player and then
	/// call <code>SessionManager.accept()</code>. <em>Note:</em> <code>launch()</code> 
	/// and <code>accept()</code> are both blocking calls, so you probably don't want
	/// to call them from your main UI thread. </li>
	/// <li> Finally, call <code>SessionManager.stopListening()</code></li>.
	/// </ol>
	/// </summary>
	public abstract class SessionManager
	{
		/// <summary> Is this object currently listening for Debug Player connections </summary>
		/// <returns> TRUE currently listening 
		/// </returns>
		public abstract bool Listening
		{
			get;
		}
		/// <summary> The preferences are set using the setPreference() method, and
		/// take effect immediately thereafter.
		/// 
		///
		/// -----------------------------------------------------------------
		/// The following are Session specific preferences.  These can be
		/// modified in this class, resulting in all future sessions using
		/// the values or they can be modified at the session level via
		/// Session.setPreference().
		/// -----------------------------------------------------------------
		/// 
		///
        /// The value used for <code>$accepttimeout</code> controls how long (in
        /// milliseconds) <code>accept()</code> waits before timing out. The
        /// default value for this preference is 120000 (2 minutes).
        /// </summary>
        public const String PREF_ACCEPT_TIMEOUT = "$accepttimeout"; //$NON-NLS-1$
        /// <summary> Valid values for <code>$urimodification</code> are 0 (off) and 1 (on).
        /// The default value is 1 (on), which allows this API to modify the URI
        /// passed to <code>launch()</code> as necessary for creating a debuggable
        /// version of an MXML file.
        /// </summary>
        public const String PREF_URI_MODIFICATION = "$urimodification"; //$NON-NLS-1$
        /// <summary> <code>$responsetimeout</code> is used to determine how long (in
        /// milliseconds) the session will wait, for a player response before giving
        /// up on the request and throwing an Exception.
        /// </summary>
        public const String PREF_RESPONSE_TIMEOUT = "$responsetimeout"; //$NON-NLS-1$
        /// <summary> <code>$contextresponsetimeout</code> is used to determine how long (in
        /// milliseconds) the session will wait for a player response from a request
        /// to get context, before giving up on the request and throwing an
        /// Exception.
        /// </summary>
        public const String PREF_CONTEXT_RESPONSE_TIMEOUT = "$contextresponsetimeout"; //$NON-NLS-1$
        /// <summary> <code>$getvarresponsetimeout</code> is used to determine how long (in
        /// milliseconds) the session will wait, for a player response to a get
        /// variable request before giving up on the request and throwing an
        /// Exception.
        /// </summary>
        public const String PREF_GETVAR_RESPONSE_TIMEOUT = "$getvarresponsetimeout"; //$NON-NLS-1$
        /// <summary> <code>$setvarresponsetimeout</code> is the amount of time (in
        /// milliseconds) that a setter in the user's code will be given to execute,
        /// before the player interrupts it with a ScriptTimeoutError. Default value
        /// is 5000 ms.
        /// </summary>
        public const String PREF_SETVAR_RESPONSE_TIMEOUT = "$setvarresponsetimeout"; //$NON-NLS-1$
        /// <summary> <code>$swfswdloadtimeout</code> is used to determine how long (in milliseconds)
        /// the session will wait, for a player response to a swf/swd load 
        /// request before giving up on the request and throwing an Exception.
        /// </summary>
        public const String PREF_SWFSWD_LOAD_TIMEOUT = "$swfswdloadtimeout"; //$NON-NLS-1$
        /// <summary> <code>$suspendwait</code> is the amount of time (in milliseconds) that
        /// a Session will wait for the Player to suspend, after a call to
        /// <code>suspend()</code>.
        /// </summary>
        public const String PREF_SUSPEND_WAIT = "$suspendwait"; //$NON-NLS-1$
        /// <summary> <code>$invokegetters</code> is used to determine whether a getter
        /// property is invoked or not when requested via <code>getVariable()</code>
        /// The default value is for this to be enabled.
        /// </summary>
        public const String PREF_INVOKE_GETTERS = "$invokegetters"; //$NON-NLS-1$
        public const String PLAYER_SUPPORTS_GET = "$playersupportsget"; //$NON-NLS-1$
        /// <summary> <code>$hiervars</code> is used to determine whether the members of
        /// a variable are shown in a hierchical way.
        /// </summary>
        public const String PREF_HIERARCHICAL_VARIABLES = "$hiervars"; //$NON-NLS-1$

        /// <summary> Set preference for this manager and for subsequent Sessions 
		/// that are initiated after this call.
		/// 
		/// If an invalid preference is passed, it will be silently ignored.
		/// </summary>
		/// <param name="pref">preference name, one of the strings listed above
		/// </param>
		/// <param name="value">value to set for preference
		/// </param>
        public abstract void setPreference(String pref, int value);
		
		/// <summary> Set preference for this manager and for subsequent Sessions 
		/// that are initiated after this call.
		/// 
		/// If an invalid preference is passed, it will be silently ignored.
		/// </summary>
		/// <param name="pref">preference name, one of the strings listed above
		/// </param>
		/// <param name="value">value to set for preference
		/// </param>
        public abstract void setPreference(String pref, String value);
		
		/// <summary> Return the value of a particular preference item
		/// 
		/// </summary>
		/// <param name="pref">preference name, one of the strings listed above
		/// </param>
		/// <throws>  NullPointerException if pref does not exist </throws>
        public abstract int getPreference(String pref);
		
		/// <summary> Listens for Player attempts to open a debug session. This method must be
		/// called prior to <code>accept()</code> being invoked.
		/// 
		/// </summary>
		/// <throws>  IOException </throws>
		/// <summary>             if opening the server side socket fails
		/// </summary>
        public abstract void startListening();

        /// <summary>
        /// 
        /// </summary>
        public abstract void startListening(Boolean useAny);

		/// <summary> Stops listening for new Player attempts to open a debug session. The
		/// method DOES NOT terminate currently connected sessions, but will cause
		/// threads blocked in <code>accept</code> to throw SocketExceptions.
		/// </summary>
        public abstract void stopListening();
		
		/// <summary> Launches a Player using the given string as a URI, as defined by RFC2396.
		/// It is expected that the operating system will be able to launch the
		/// appropriate player application given this URI.
		/// <p />
		/// For example "http://localhost:8100/flex/my.mxml" or for a local file on
		/// Windows, "file://c:/my.swf"
		/// <p />
		/// This call will block until a session with the newly launched player is
		/// created.
		/// <p />
		/// It is the caller's responsibility to ensure that no other thread is
		/// blocking in <code>accept()</code>, since that thread will gain control
		/// of this session.
		/// <p />
		/// Before calling <code>launch()</code>, you should first call
		/// <code>supportsLaunch()</code>. If <code>supportsLaunch()</code>
		/// returns false, then you will have to tell the user to manually launch the
		/// Flash player.
		/// <p />
		/// Also, before calling <code>launch()</code>, you must call
		/// <code>startListening()</code>.
		/// 
		/// </summary>
		/// <param name="uri">which will launch a Flash player under running OS. For
		/// Flash/Flex apps, this can point to either a SWF or an HTML
		/// file. For AIR apps, this must point to the application.xml
		/// file for the application.
		/// </param>
		/// <param name="airLaunchInfo">If trying to launch an AIR application, this argument must be
		/// specified; it gives more information about how to do the
		/// launch. If trying to launch a regular web-based Flash or Flex
		/// application, such as one that will be in a browser or in the
		/// standalone Flash Player, this argument should be
		/// <code>null</code>.
		/// </param>
		/// <param name="forDebugging">if <code>true</code>, then the launch is for the purposes
		/// of debugging. If <code>false</code>, then the launch is
		/// simply because the user wants to run the movie but not debug
		/// it; in that case, the return value of this function will be
		/// <code>null</code>.
		/// </param>
		/// <param name="waitReporter">a progress monitor to allow accept() to notify its parent how
		/// long it has been waiting for the Flash player to connect to
		/// it. May be <code>null</code> if the caller doesn't need to
		/// know how long it's been waiting.
		/// </param>
		/// <returns> a Session to use for debugging, or null if forDebugging==false.
		/// The return value is not used to indicate an error -- exceptions
		/// are used for that. If this function returns without throwing an
		/// exception, then the return value will always be non-null if
		/// forDebugging==true, or null if forDebugging==false.
		/// </returns>
		/// <throws>  BindException </throws>
		/// <summary>             if <code>isListening()</code> == false
		/// </summary>
		/// <throws>  FileNotFoundException </throws>
		/// <summary>             if file cannot be located
		/// </summary>
		/// <throws>  CommandLineException </throws>
		/// <summary>             if the program that was launched exited unexpectedly. This
		/// will be returned, for example, when launching an AIR
		/// application, if adl exits with an error code.
		/// CommandLineException includes functions to return any error
		/// text that may have been sent to stdout/stderr, and the exit
		/// code of the program.
		/// </summary>
		/// <throws>  IOException </throws>
		/// <summary>             see Runtime.exec()
		/// </summary>
        public abstract Session launch(String uri, AIRLaunchInfo airLaunchInfo, bool forDebugging, IProgress waitReporter);
		
		/// <summary> Returns information about the Flash player which will be used to run the
		/// given URI.
		/// 
		/// </summary>
		/// <param name="uri">The URI which will be passed to <code>launch()</code> -- for
		/// example, <code>http://flexserver/mymovie.mxml</code> or
		/// <code>c:\mymovie.swf</code>
		/// </param>
		/// <returns> a {@link Player} which can be used to determine information about
		/// the player -- for example, whether it is a debugger-enabled
		/// player. Returns <code>null</code> if the player cannot be
		/// determined. <em>Important:</em> There are valid situations in
		/// which this will return <code>null</code>
		/// </returns>
        public abstract Player playerForUri(String uri);
		
		/// <summary> Returns whether this platform supports the <code>launch()</code>
		/// command; that is, whether the debugger can programmatically launch the
		/// Flash player. If this function returns false, then the debugger will have
		/// to tell the user to manually launch the Flash player.
		/// 
		/// </summary>
		/// <returns> true if this platform supports the <code>launch()</code>
		/// command.
		/// </returns>
        public abstract bool supportsLaunch();
		
		/// <summary> Blocks until the next available player debug session commences, or until
		/// <code>getPreference(PREF_ACCEPT_TIMEOUT)</code> milliseconds pass.
		/// <p />
		/// Before calling <code>launch()</code>, you must call
		/// <code>startListening()</code>.
		/// <p />
		/// Once a Session is obtained, Session.bind() must be called prior to any
		/// other Session method.
		/// 
		/// </summary>
		/// <param name="waitReporter">a progress monitor to allow accept() to notify its parent how
		/// long it has been waiting for the Flash player to connect to it.
		/// May be <code>null</code> if the caller doesn't need to know how
		/// long it's been waiting.
		/// </param>
		/// <throws>  BindException </throws>
		/// <summary>             if isListening() == false
		/// </summary>
		/// <throws>  IOException - </throws>
		/// <summary>             see java.net.ServerSocket.accept()
		/// </summary>
        public abstract Session accept(IProgress waitReporter);
		
		/// <summary> Tells the session manager to use the specified IDebuggerCallbacks for
		/// performing certain operatios, such as finding the Flash Player and
		/// launching the debug target. If you do not call this, the session manager
		/// will use a <code>DefaultDebuggerCallbacks</code> object.
		/// </summary>
        public abstract void setDebuggerCallbacks(IDebuggerCallbacks debugger);
	}
}
