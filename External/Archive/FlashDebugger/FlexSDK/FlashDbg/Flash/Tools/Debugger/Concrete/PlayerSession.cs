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
using System.IO;
using System.Text.RegularExpressions;
using Flash.Tools.Debugger.Events;
using JavaCompatibleClasses;
using Trace = Flash.Util.Trace;

namespace Flash.Tools.Debugger.Concrete
{
	public class PlayerSession : Session, DProtocolNotifierIF
	{
        virtual public DMessageCounter MessageCounter
		{
			/* getter */
			
			get
			{
				return m_protocol.MessageCounter;
			}
			
		}
		virtual public String URI
		{
			get
			{
				return m_manager.URI;
			}
			
		}
		virtual public SourceLocator SourceLocator
		{
			get
			{
				return m_manager.SourceLocator;
			}
			
			/*
			* @see Flash.Tools.Debugger.Session#setSourceLocator(Flash.Tools.Debugger.SourceLocator)
			*/
			
			set
			{
				m_manager.SourceLocator = value;
			}
			
		}
		/// <summary> If the manager started the process for us, then note it here. We will attempt to kill
		/// it when we go down
		/// </summary>
		virtual public System.Diagnostics.Process LaunchProcess
		{
			/*
			* @see Flash.Tools.Debugger.Session#getLaunchProcess()
			*/
			
			get
			{
				return m_process;
			}

            set
            {
                m_process = value;
            }
        }
		/// <summary> Set preference
		/// If an invalid preference is passed, it will be silently ignored.
		/// </summary>
		virtual public System.Collections.IDictionary Preferences
		{
			set
			{
				SupportClass.MapSupport.PutAll(m_prefs, value); mapBack();
			}
			
		}
		virtual public bool Connected
		{
			/*
			* @see Flash.Tools.Debugger.Session#isConnected()
			*/
			
			get
			{
				return m_isConnected;
			}
			
		}
		virtual public bool Suspended
		{
			/*
			* @see Flash.Tools.Debugger.Session#isSuspended()
			*/
			
			get
			{
				if (!Connected)
					throw new NotConnectedException();
				
				return m_isHalted;
			}
			
		}
		/// <summary> Obtain all the suspend information</summary>
		virtual public DSuspendInfo SuspendInfo
		{
			get
			{
				DSuspendInfo info = m_manager.SuspendInfo;
				if (info == null)
				{
					// request break information
					if (simpleRequestResponseMessage(DMessage.OutGetBreakReason, DMessage.InBreakReason))
						info = m_manager.SuspendInfo;
					
					// if we still can't get any info from the manager...
					if (info == null)
						info = new DSuspendInfo(); // empty unknown break information
				}
				return info;
			}
			
		}
		/// <summary> Return the offset in which the player has suspended itself.  The BreakReason
		/// message contains both reason and offset.
		/// </summary>
		virtual public int SuspendOffset
		{
			get
			{
				DSuspendInfo info = SuspendInfo;
				return info.Offset;
			}
			
		}
		/// <summary> Return the offset in which the player has suspended itself.  The BreakReason
		/// message contains both reason and offset.
		/// </summary>
		virtual public int SuspendActionIndex
		{
			get
			{
				DSuspendInfo info = SuspendInfo;
				return info.ActionIndex;
			}
			
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
		virtual public SwfInfo[] Swfs
		{
			get
			{
				if (m_manager.SwfInfoCount == 0)
				{
					// need to help out on the first one since the player
					// doesn't send it
					requestSwfInfo(0);
				}
				SwfInfo[] swfs = m_manager.SwfInfos;
				return swfs;
			}
			
		}
		/// <summary> Get a list of breakpoints</summary>
		virtual public Location[] BreakpointList
		{
			get
			{
				return m_manager.Breakpoints;
			}
			
		}
		virtual public Watch[] WatchList
		{
			/*
			* @see Flash.Tools.Debugger.Session#getWatchList()
			*/
			
			get
			{
				return m_manager.Watchpoints;
			}
			
		}
		virtual public Variable[] VariableList
		{
			/*
			* @see Flash.Tools.Debugger.Session#getVariableList()
			*/
			
			get
			{
				// make sure the player has stopped and send our message awaiting a response
				if (!Suspended)
					throw new NotSuspendedException();
				
				requestFrame(0); // our 0th frame gets our local context
				
				// now let's request all of the special variables too
				getValue(Value.GLOBAL_ID);
				getValue(Value.THIS_ID);
				getValue(Value.ROOT_ID);
				
				// request as many levels as we can get
				int i = 0;
				Value v = null;
				do 
				{
					v = getValue(Value.LEVEL_ID - i);
				}
				while (i++ < 128 && v != null);
				
				// now that we've primed the DManager we can request the base variable whose
				// children are the variables that are available
				v = m_manager.getValue(Value.BASE_ID);
				if (v == null)
					throw new VersionException();
				return v.getMembers(this);
			}
			
		}
		virtual public Frame[] Frames
		{
			/*
			* @see Flash.Tools.Debugger.Session#getFrames()
			*/
			
			get
			{
				return m_manager.Frames;
			}
			
		}
		virtual public int EventCount
		{
			/*
			* @see Flash.Tools.Debugger.Session#getEventCount()
			*/
			
			get
			{
				return m_manager.EventCount;
			}
			
		}
		virtual public String LaunchUrl
		{
			set
			{
				if (value.StartsWith("/"))
				{
					//$NON-NLS-1$
					value = "file://" + value; //$NON-NLS-1$
				}
				m_launchUrl = value;
			}
			
		}
		virtual public AIRLaunchInfo AIRLaunchInfo
		{
			set
			{
				m_airLaunchInfo = value;
			}
			
		}
		public const int MAX_STACK_DEPTH = 256;
		
		private System.Net.Sockets.TcpClient m_socket;
		private DProtocol m_protocol;
		private DManager m_manager;
		private System.Diagnostics.Process m_process;
		private System.Collections.IDictionary m_prefs; // WARNING -- accessed from multiple threads
		private static String s_newline = Environment.NewLine; //$NON-NLS-1$
		
		private volatile bool m_isConnected; // WARNING -- accessed from multiple threads
		private volatile bool m_isHalted; // WARNING -- accessed from multiple threds
		private volatile bool m_incoming; // WARNING -- accessed from multiple threads
		private volatile bool m_lastResponse; // whether there was a reponse from the last message to the Player
		private int m_watchTransactionTag;
		
		/// <summary> The URL that was launched, or <code>null</code> if not known.  Note:
		/// This is NOT the value returned by getURI().  getURI() returns the
		/// URL that came from the Player, and is therefore probably the URI of
		/// the SWF; but m_launchedUrl contains the URL that we tried to launch,
		/// which might be an HTML wrapper, e.g. http://localhost/myapp.html
		/// </summary>
		private String m_launchUrl;
		
		private AIRLaunchInfo m_airLaunchInfo; // null if this is not an AIR app
		
		internal static volatile bool m_debugMsgOn; // debug ONLY; turned on with "set $debug_messages = 1"
		internal volatile int m_debugMsgSize; // debug ONLY; controlled with "set $debug_message_size = NNN"
		internal static volatile bool m_debugMsgFileOn; // debug ONLY for file dump; turned on with "set $debug_message_file = 1"
		internal volatile int m_debugMsgFileSize; // debug ONLY for file dump; controlled with "set $debug_message_file_size = NNN"
		
		private const String DEBUG_MESSAGES = "$debug_messages"; //$NON-NLS-1$
		private const String DEBUG_MESSAGE_SIZE = "$debug_message_size"; //$NON-NLS-1$
		private const String DEBUG_MESSAGE_FILE = "$debug_message_file"; //$NON-NLS-1$
		private const String DEBUG_MESSAGE_FILE_SIZE = "$debug_message_file_size"; //$NON-NLS-1$
		
		private const String CONSOLE_ERRORS = "$console_errors"; //$NON-NLS-1$
		
		private const String FLASH_PREFIX = "$flash_"; //$NON-NLS-1$
		
		internal PlayerSession(System.Net.Sockets.TcpClient s, DProtocol proto, DManager manager)
		{
			m_isConnected = false;
			m_isHalted = false;
			m_socket = s;
			m_protocol = proto;
			m_manager = manager;
			m_prefs = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
			m_incoming = false;
			m_debugMsgOn = false;
			m_debugMsgSize = 16;
			m_debugMsgFileOn = false;
			m_debugMsgFileSize = 128;
			m_watchTransactionTag = 1; // number that is sent for each watch transaction that occurs
		}
		
		public static PlayerSession createFromSocket(System.Net.Sockets.TcpClient s)
		{
			DProtocol proto = DProtocol.createFromSocket(s);
			
			// let the manager hear incoming messages
			DManager manager = new DManager();
			manager.attach(proto);
			
			PlayerSession session = new PlayerSession(s, proto, manager);
			return session;
		}
		public virtual bool playerSupportsGet()
		{
			return m_manager.GetSupported;
		}
		public virtual int playerVersion()
		{
			return m_manager.Version;
		}

		public virtual SupportClass.SetSupport keySet()
		{
			return new SupportClass.HashSetSupport(m_prefs.Keys);
		}

		public virtual Object getPreferenceAsObject(String pref)
		{
			return m_prefs[pref];
		}
		
		/// <summary> Set a property. Special logic for debug message boolean</summary>
		public virtual void  setPreference(String pref, int value)
		{
			m_prefs[pref] = (Int32) value;
			mapBack();
			
			// change in console messages?
			if (pref.Equals(CONSOLE_ERRORS))
				sendConsoleErrorsAsTrace(value == 1);
			
			// generic message for flash player wherein "$flash_xxx" causes "xxx" to be sent
			if (pref.StartsWith(FLASH_PREFIX))
				sendOptionMessage(pref.Substring(FLASH_PREFIX.Length), Convert.ToString(value));
		}
		
		// helper for mapBack()
		private int mapBackOnePreference(String preferenceName, int defaultValue)
		{
			Object prefValue = getPreferenceAsObject(preferenceName);
			if (prefValue != null)
				return ((Int32) prefValue);
			else
				return defaultValue;
		}
		
		// helper for mapBack()
		private bool mapBackOnePreference(String preferenceName, bool defaultValue)
		{
			Object prefValue = getPreferenceAsObject(preferenceName);
			if (prefValue != null)
				return ((Int32) prefValue) != 0?true:false;
			else
				return defaultValue;
		}
		
		// look for preferences, that map back to variables
		private void  mapBack()
		{
			m_debugMsgOn = mapBackOnePreference(DEBUG_MESSAGES, m_debugMsgOn);
			m_debugMsgSize = mapBackOnePreference(DEBUG_MESSAGE_SIZE, m_debugMsgSize);
			
			m_debugMsgFileOn = mapBackOnePreference(DEBUG_MESSAGE_FILE, m_debugMsgFileOn);
			m_debugMsgFileSize = mapBackOnePreference(DEBUG_MESSAGE_FILE_SIZE, m_debugMsgFileSize);
		}
		
		public virtual int getPreference(String pref)
		{
            if (!m_prefs.Contains(pref))
            {
				throw new NullReferenceException();
            }

			return (int) m_prefs[pref];
		}
		
		/// <summary> Start up the session listening for incoming messages on the socket</summary>
		public virtual bool bind()
		{
			bool bound = false;
			
			// mark that we are connected
			m_isConnected = true;
			
			// attach us to the pipe (we are last to ensure that DManager and msg counter
			// get updated first
			m_protocol.addListener(this);
			
			// start up the receiving thread
			bound = m_protocol.bind();
			
			// transmit our first few adjustment messages
			sendStopWarning();
			sendStopOnFault();
			sendEnumerateOverride();
			sendFailureNotify();
			sendInvokeSetters();
			sendSwfloadNotify();
			sendGetterTimeout();
			sendSetterTimeout();
			bool responded = sendSquelch(true);
			
			// now note in our preferences whether get is working or not.
			setPreference(SessionManager.PLAYER_SUPPORTS_GET, playerSupportsGet()?1:0); //$NON-NLS-1$
			
			// Spawn a background thread which fetches the SWF and SWD
			// from the Player and uses them to build function name tables
			// for each source file
			System.Threading.Thread t = new System.Threading.Thread(Run);
			t.Name = "SWF/SWD reader"; //$NON-NLS-1$
			t.IsBackground = true;
			t.Start();
			
			// we're probably using a bad version
			if (!responded)
				throw new VersionException();
			
			return bound;
		}
		
		/// <summary> Permanently stops the debugging session and breaks the
		/// connection to the Player
		/// </summary>
		public virtual void  unbind()
		{
			unbind(false);
		}
		
		/// <param name="requestTerminate">if true, and if the player to which we are attached is capable
		/// of terminating itself (e.g. Adobe AIR), then the player will
		/// be told to terminate.
		/// </param>
		/// <returns> true if the player is capable of terminating itself and has been
		/// told to do so
		/// </returns>
		private bool unbind(bool requestTerminate)
		{
			// If the caller asked us to terminate the player, then we first check
			// whether the player to which we are connected is capable of that.
			// (The web-based players are not; Adobe AIR is.)
			requestTerminate = requestTerminate && playerCanTerminate();
			DMessage dm = DMessageCache.alloc(1);
			dm.Type = DMessage.OutExit;
			dm.putByte((byte) (requestTerminate?1:0));
			sendMessage(dm);
			
			// release the manager from the socket
			m_manager.release(m_protocol);
			
			// unbind from the socket, so that we don't receive any more messages
			m_protocol.unbind();
			
			// kill the socket
			try
			{
				m_socket.Close();
			}
			catch (IOException)
			{
			}
			
			m_isConnected = false;
			m_isHalted = false;
			
			return requestTerminate; // true if player was told to terminate
        }

#if false
		/// <summary> Execute the specified AppleScript by passing it to /usr/bin/osascript.
		/// 
		/// </summary>
		/// <param name="appleScript">the AppleScript to execute, as a series of lines
		/// </param>
		/// <param name="argv">any arguments; these can be accessed from within your
		/// AppleScript via "item 1 or argv", "item 2 of argv", etc.
		/// </param>
		/// <returns> any text which was sent to stdout by /usr/bin/osascript, with the
		/// trailing \n already removed
		/// </returns>
		private String executeAppleScript(String[] appleScript, String[] argv)
		{
			System.Text.StringBuilder retval = new System.Text.StringBuilder();
			try
			{
				System.Collections.IList execArgs = new System.Collections.ArrayList();
				// "osascript" is the command-line way of executing AppleScript.
				execArgs.Add("/usr/bin/osascript"); //$NON-NLS-1$
				execArgs.Add("-"); //$NON-NLS-1$
				if (argv != null)
				{
					for (int i = 0; i < argv.Length; ++i)
						execArgs.Add(argv[i]);
				}
				System.Diagnostics.Process osascript = SupportClass.ExecSupport((String[]) SupportClass.ICollectionSupport.ToArray(execArgs, new String[execArgs.Count]));
				// feed our AppleScript code to osascript's stdin
				Stream outputStream = osascript.StandardOutput.BaseStream;
				StreamWriter temp_writer;
				temp_writer = new StreamWriter(outputStream, System.Text.Encoding.Default);
				temp_writer.AutoFlush = true;
				StreamWriter writer = temp_writer;
				writer.WriteLine("on run argv"); //$NON-NLS-1$ // this gives the name "argv" to the command-line args
				for (int i = 0; i < appleScript.Length; ++i)
				{
					writer.WriteLine(appleScript[i]);
				}
				writer.WriteLine("end run"); //$NON-NLS-1$
				writer.Close();
				StreamReader reader = new StreamReader(osascript.StandardInput.BaseStream, System.Text.Encoding.Default);
				int ch;
				while ((ch = reader.Read()) != - 1)
					retval.Append((char) ch);
			}
			catch (IOException)
			{
				// ignore
			}
			return retval.ToString().Replace("\n$", ""); //$NON-NLS-1$ //$NON-NLS-2$
		}

		/// <summary> Execute the specified AppleScript by passing it to /usr/bin/osascript.
		/// 
		/// </summary>
		/// <param name="appleScriptFilename">The name of the file containing AppleScript to execute.  This
		/// must be relative to PlayerSession.java.
		/// </param>
		/// <param name="argv">any arguments; these can be accessed from within your
		/// AppleScript via "item 1 or argv", "item 2 of argv", etc.
		/// </param>
		/// <returns> any text which was sent to stdout by /usr/bin/osascript, with the
		/// trailing \n already removed
		/// </returns>
		/// <throws>  IOException  </throws>
		private String executeAppleScript(String appleScriptFilename, String[] argv)
		{
			Stream stm = null;
			try
			{
				//UPGRADE_ISSUE: Method 'java.lang.Class.getResourceAsStream' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangClassgetResourceAsStream_javalangString'"
				stm = typeof(PlayerSession).getResourceAsStream(appleScriptFilename);
				//UPGRADE_TODO: The differences in the expected value  of parameters for constructor 'java.io.BufferedReader.BufferedReader'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
				//UPGRADE_WARNING: At least one expression was used more than once in the target code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1181'"
				StreamReader reader = new StreamReader(new StreamReader(stm, System.Text.Encoding.Default).BaseStream, new StreamReader(stm, System.Text.Encoding.Default).CurrentEncoding);
				String line;
				System.Collections.IList appleScriptLines = new System.Collections.ArrayList();
				while ((line = reader.ReadLine()) != null)
					appleScriptLines.Add(line);
				String[] lines = (String[]) SupportClass.ICollectionSupport.ToArray(appleScriptLines, new String[appleScriptLines.Count]);
				return executeAppleScript(lines, argv);
			}
			finally
			{
				if (stm != null)
				{
					stm.Close();
				}
			}
		}
		
		/// <summary> Checks whether the specified Macintosh web browser is currently
		/// running.  You should only call this function if you have already
		/// checked that you are running on a Mac.
		/// 
		/// </summary>
		/// <param name="browserName">a name, e.g. "Safari", "Firefox", "Camino"
		/// </param>
		/// <returns> true if currently running
		/// </returns>
		private SupportClass.SetSupport runningApplications()
		{
			String running = executeAppleScript(new String[]{"tell application \"System Events\"", "	name of processes", "end tell"}, null);
			String[] apps = running.split(", "); //$NON-NLS-1$
			//UPGRADE_TODO: Class 'java.util.HashSet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashSet'"
			SupportClass.SetSupport retval = new SupportClass.HashSetSupport();
			for (int i = 0; i < apps.Length; ++i)
				retval.Add(apps[i]);
			return retval;
		}
#endif

        /// <summary> Destroys all objects related to the connection
		/// including the process that was tied to this
		/// session via SessionManager.launch(), if it
		/// exists.
		/// </summary>
		public virtual void  terminate()
		{
			bool playerWillTerminateItself = false;
			
			// unbind first
			try
			{
				// Tell player to end session.  Note that this is just a hint, and will often
				// do nothing.  For example, the Flash player running in a browser will
				// currently never terminate when you tell it to, but the AIR player will
				// terminate.
				playerWillTerminateItself = unbind(true);
			}
			catch (Exception)
			{
			}
			
			if (!playerWillTerminateItself)
			{
#if false
				//UPGRADE_TODO: Method 'java.lang.System.getProperty' was converted to 'System.Environment.GetEnvironmentVariable' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javalangSystemgetProperty_javalangString'"
				if (Environment.GetEnvironmentVariable("OS").ToLower().StartsWith("mac os x"))
				//$NON-NLS-1$ //$NON-NLS-2$
				{
					// In certain situations, AppleScript can hang for about two minutes (bug 193086).  Since all we're
					// trying to do here is close the browser window, we do the work in a separate thread.  If that
					// thread hangs for two minutes, no harm done.
					IThreadRunnable r = new AnonymousClassRunnable(this);
					new SupportClass.ThreadClass(new System.Threading.ThreadStart(r.Run), "Terminate Mac debug target").Start(); //$NON-NLS-1$
				}
#endif
				// if we have a process pop it
				if (m_process != null)
				{
					try
					{
						m_process.Kill();
					}
					catch (Exception)
					{
					}
				}
			}
			
			// now clear it all
			m_protocol = null;
			m_socket = null;
			m_manager = null;
			m_process = null;
			m_isConnected = false;
			m_isHalted = false;
		}
		
		/*
		* @see Flash.Tools.Debugger.Session#resume()
		*/
		public virtual void  resume()
		{
			// send a continue message then return
			if (!Suspended)
				throw new NotSuspendedException();
			
			if (!simpleRequestResponseMessage(DMessage.OutContinue, DMessage.InContinue))
				throw new NoResponseException(getPreference(SessionManager.PREF_RESPONSE_TIMEOUT));
		}
		
		/*
		* @see Flash.Tools.Debugger.Session#suspend()
		*/
		public virtual void  suspend()
		{
			// send a halt message
			int wait = getPreference(SessionManager.PREF_SUSPEND_WAIT);
			int every = 50; // wait this long for a response.  The lower the number the more aggressive we are!
			
			if (Suspended)
				throw new SuspendedException();
			
			while (!Suspended && wait > 0)
			{
				simpleRequestResponseMessage(DMessage.OutStopDebug, DMessage.InBreakAtExt, every);
				wait -= every;
			}
			
			if (!Suspended)
				throw new NoResponseException(wait);
		}
		
		/// <summary> Return the reason that the player has suspended itself.</summary>
		public virtual int suspendReason()
		{
			DSuspendInfo info = SuspendInfo;
			return info.Reason;
		}
		
		/// <summary> Request information on a particular swf, used by DSwfInfo
		/// to fill itself correctly
		/// </summary>
		public virtual void  requestSwfInfo(int at)
		{
			// nope don't have it...might as well go out and ask for all of them.
			DMessage dm = DMessageCache.alloc(4);
			dm.Type = DMessage.OutSwfInfo;
			dm.putWord(at);
			dm.putWord(0); // rserved
			
			int to = getPreference(SessionManager.PREF_CONTEXT_RESPONSE_TIMEOUT);
			
			if (!simpleRequestResponseMessage(dm, DMessage.InSwfInfo, to))
				throw new NoResponseException(to);
		}
		
		/// <summary> Request a set of actions from the player</summary>
		public virtual byte[] getActions(int which, int at, int len)
		{
			byte[] actions = null;
			
			// send a actions message
			DMessage dm = DMessageCache.alloc(12);
			dm.Type = DMessage.OutGetActions;
			dm.putWord(which);
			dm.putWord(0); // rsrvd
			dm.putDWord(at);
			dm.putDWord(len);
			
			// request action bytes
			int to = getPreference(SessionManager.PREF_CONTEXT_RESPONSE_TIMEOUT);
			if (simpleRequestResponseMessage(dm, DMessage.InGetActions, to))
				actions = m_manager.Actions;
			else
				throw new NoResponseException(to);
			
			return actions;
		}
		
		/*
		* @see Flash.Tools.Debugger.Session#stepInto()
		*/
		public virtual void  stepInto()
		{
			if (Suspended)
			{
				// send a step-into message and then wait for the Flash player to tell us that is has
				// resumed execution
				if (!simpleRequestResponseMessage(DMessage.OutStepInto, DMessage.InContinue))
					throw new NoResponseException(getPreference(SessionManager.PREF_RESPONSE_TIMEOUT));
			}
			else
				throw new NotSuspendedException();
		}
		
		/*
		* @see Flash.Tools.Debugger.Session#stepOut()
		*/
		public virtual void  stepOut()
		{
			if (Suspended)
			{
				// send a step-out message and then wait for the Flash player to tell us that is has
				// resumed execution
				if (!simpleRequestResponseMessage(DMessage.OutStepOut, DMessage.InContinue))
					throw new NoResponseException(getPreference(SessionManager.PREF_RESPONSE_TIMEOUT));
			}
			else
				throw new NotSuspendedException();
		}
		
		/*
		* @see Flash.Tools.Debugger.Session#stepOver()
		*/
		public virtual void  stepOver()
		{
			if (Suspended)
			{
				// send a step-over message and then wait for the Flash player to tell us that is has
				// resumed execution
				if (!simpleRequestResponseMessage(DMessage.OutStepOver, DMessage.InContinue))
					throw new NoResponseException(getPreference(SessionManager.PREF_RESPONSE_TIMEOUT));
			}
			else
				throw new NotSuspendedException();
		}
		
		/*
		* @see Flash.Tools.Debugger.Session#stepContinue()
		*/
		public virtual void  stepContinue()
		{
			if (!Suspended)
				throw new NotSuspendedException();
			
			// send a step-continue message and then wait for the Flash player to tell us that is has
			// resumed execution
			if (!simpleRequestResponseMessage(DMessage.OutStepContinue, DMessage.InContinue))
				throw new NoResponseException(getPreference(SessionManager.PREF_RESPONSE_TIMEOUT));
		}
		
		/// <summary> Sends a request to the player to obtain function names.
		/// The resultant message end up populating the function name array
		/// for the given DModule.
		/// 
		/// </summary>
		/// <param name="moduleId">
		/// </param>
		/// <param name="lineNbr">
		/// </param>
		/// <returns>
		/// </returns>
		public virtual void  requestFunctionNames(int moduleId, int lineNbr)
		{
			// only player 9 supports this message
			if (m_manager.Version >= 9)
			{
				DMessage dm = DMessageCache.alloc(8);
				dm.Type = DMessage.OutGetFncNames;
				dm.putDWord(moduleId);
				dm.putDWord(lineNbr);
				
				if (!simpleRequestResponseMessage(dm, DMessage.InGetFncNames))
					throw new NoResponseException(0);
			}
			else
			{
				throw new VersionException();
			}
		}
		
		/// <summary> From a given file identifier return a source file object</summary>
		public virtual SourceFile getFile(int fileId)
		{
			return m_manager.getSource(fileId);
		}
		
		/*
		* @see Flash.Tools.Debugger.Session#setBreakpoint(int, int)
		*/
		public virtual Location setBreakpoint(int fileId, int lineNum)
		{
			/* send the message to the player and await a response */
			Location l = null;
			int bp = DLocation.encodeId(fileId, lineNum);
			
			DMessage dm = DMessageCache.alloc(8);
			dm.Type = DMessage.OutSetBreakpoints;
			dm.putDWord(1);
			dm.putDWord(bp);
			
			bool gotResponse = simpleRequestResponseMessage(dm, DMessage.InSetBreakpoint);
			if (gotResponse)
			{
				/* even though we think we got an answer check that the breakpoint was added */
				l = m_manager.getBreakpoint(bp);
			}
			else
				throw new NoResponseException(getPreference(SessionManager.PREF_RESPONSE_TIMEOUT));
			
			return l;
		}
		
		/*
		* @see Flash.Tools.Debugger.Session#clearBreakpoint(Flash.Tools.Debugger.Location)
		*/
		public virtual Location clearBreakpoint(Location local)
		{
			/* first find it */
			SourceFile source = local.File;
			int fileId = source.Id;
			int lineNum = local.Line;
			int bp = DLocation.encodeId(fileId, lineNum);
			Location l = m_manager.getBreakpoint(bp);
			if (l != null)
			{
				/* send the message */
				DMessage dm = DMessageCache.alloc(8);
				dm.Type = DMessage.OutRemoveBreakpoints;
				dm.putDWord(1);
				dm.putDWord(bp);
				sendMessage(dm);
				
				/* no callback from the player so we remove it ourselves */
				m_manager.removeBreakpoint(bp);
			}
			return l;
		}
		
		/*
		* @see Flash.Tools.Debugger.Session#setWatch(Flash.Tools.Debugger.Variable, java.lang.String, int)
		*/
		public virtual Watch setWatch(Value v, String memberName, int kind)
		{
			// we really have two cases here, one where we add a completely new
			// watchpoint and the other where we modify an existing one.
			// In either case the DManager logic is such that the last watchpoint
			// in the list will contain our id if successful.
			Watch w = null;
			int tag = m_watchTransactionTag++;
			
			if (addWatch(v.Id, memberName, kind, tag))
			{
				// good that we got a response now let's check that
				// it actually worked.
				int count = m_manager.WatchpointCount;
				if (count > 0)
				{
					DWatch lastWatch = m_manager.getWatchpoint(count - 1);
					if (lastWatch.Tag == tag)
						w = lastWatch;
				}
			}
			return w;
		}
		
		/*
		* @see Flash.Tools.Debugger.Session#clearWatch(Flash.Tools.Debugger.Watch)
		*/
		public virtual Watch clearWatch(Watch watch)
		{
			Watch[] list = WatchList;
			Watch w = null;
			if (removeWatch(watch.ValueId, watch.MemberName))
			{
				// now let's first check the size of the list, it
				// should now be one less
				if (m_manager.WatchpointCount < list.Length)
				{
					// ok we made a change. So let's compare list and see which
					// one went away
					Watch[] newList = WatchList;
					for (int i = 0; i < newList.Length; i++)
					{
						// where they differ is the missing one
						if (list[i] != newList[i])
						{
							w = list[i];
							break;
						}
					}
					
					// might be the last one...
					if (w == null)
						w = list[list.Length - 1];
				}
			}
			return w;
		}
		
		/// <summary> Asks the player to return information regarding our current context which includes
		/// this pointer, arguments for current frame, locals, etc.
		/// </summary>
		public virtual void  requestFrame(int depth)
		{
			if (playerSupportsGet())
			{
				if (!Suspended)
					throw new NotSuspendedException();
				
				int timeout = getPreference(SessionManager.PREF_CONTEXT_RESPONSE_TIMEOUT);
				
				DMessage dm = DMessageCache.alloc(4);
				dm.Type = DMessage.OutGetFrame;
				dm.putDWord(depth); // depth of zero
				if (!simpleRequestResponseMessage(dm, DMessage.InFrame, timeout))
				{
					throw new NoResponseException(timeout);
				}
				
				pullUpActivationObjectVariables(depth);
			}
		}
		
		/// <summary> The compiler sometimes creates special local variables called
		/// "activation objects."  When it decides to do this (e.g. if the
		/// current function contains any anonymous functions, try/catch
		/// blocks, complicated E4X expressions, or "with" clauses), then
		/// all locals and arguments are actually stored as children of
		/// this activation object, rather than the usual way.
		/// 
		/// We need to hide this implementation detail from the user.  So,
		/// if we find any activation objects among the locals of the current
		/// function, then we will "pull up" its members, and represent them
		/// as if they were actually args/locals of the function itself.
		/// 
		/// </summary>
		/// <param name="depth">the depth of the stackframe we are fixing; 0 is topmost
		/// </param>
		private void  pullUpActivationObjectVariables(int depth)
		{
			DValue frame = m_manager.getValue(Value.BASE_ID - depth);
			DStackContext context = m_manager.getFrame(depth);
			DVariable[] frameVars = (DVariable[]) frame.getMembers(this);
			LinkedHashMap varmap = new LinkedHashMap(frameVars.Length); // preserves order
			System.Collections.IList activationObjects = new System.Collections.ArrayList();
			Regex activationObjectNamePattern = new Regex(@"^.*\$\d+$"); //$NON-NLS-1$
			
			// loop through all frame variables, and separate them into two
			// groups: activation objects, and all others (locals and arguments)
			for (int i = 0; i < frameVars.Length; ++i)
			{
				DVariable member = frameVars[i];
				Match match = activationObjectNamePattern.Match(member.getName());
				if (match.Success)
					activationObjects.Add(member);
				else
					varmap[member.getName()] = member;
			}
			
			// If there are no activation objects, then we don't need to do anything
			if (activationObjects.Count == 0)
				return ;
			
			// overwrite existing args and locals with ones pulled from the activation objects
			for (int i = 0; i < activationObjects.Count; ++i)
			{
				DVariable activationObject = (DVariable) activationObjects[i];
				DVariable[] activationMembers = (DVariable[]) activationObject.getValue().getMembers(this);
				for (int j = 0; j < activationMembers.Length; ++j)
				{
					DVariable member = activationMembers[j];
					int attributes = member.getAttributes();
					
					// For some odd reason, the activation object often contains a whole bunch of
					// other variables that we shouldn't be displaying.  I don't know what they
					// are, but I do know that they are all marked "static".
					if ((attributes & VariableAttribute.IS_STATIC) != 0)
						continue;
					
					// No matter what the activation object member's scope is, we want all locals
					// and arguments to be considered "public"
					attributes &= ~ (VariableAttribute.PRIVATE_SCOPE | VariableAttribute.PROTECTED_SCOPE | VariableAttribute.NAMESPACE_SCOPE);
					attributes |= VariableAttribute.PUBLIC_SCOPE;
					member.setAttributes(attributes);
					
					String name = member.getName();
					DVariable oldvar = (DVariable)(varmap.Contains(name) ? varmap[name] : null);
					int vartype;
					if (oldvar != null)
						vartype = oldvar.getAttributes() & (VariableAttribute.IS_ARGUMENT | VariableAttribute.IS_LOCAL);
					else
						vartype = VariableAttribute.IS_LOCAL;
					member.setAttributes(member.getAttributes() | vartype);
					varmap[name] = member;
				}
				
				context.convertLocalToActivationObject(activationObject);
			}

			foreach (DVariable next in varmap.Values)
            {
				frame.addMember(next);
				if (next.isAttributeSet(VariableAttribute.IS_LOCAL))
				{
					context.addLocal(next);
				}
				else if (next.isAttributeSet(VariableAttribute.IS_ARGUMENT))
				{
					if (next.getName().Equals("this"))
					//$NON-NLS-1$
						context.setThis(next);
					else
						context.addArgument(next);
				}
			}
		}
		
		/*
		* @see Flash.Tools.Debugger.Session#getValue(int)
		*/
		public virtual Value getValue(int valueId)
		{
			DValue val = null;
			
			if (!Suspended)
				throw new NotSuspendedException();
			
			// get it from cache if we can
			val = m_manager.getValue(valueId);
			
			if (val == null)
			{
				// if a special variable, then we need to trigger a local frame call, otherwise just use id to get it
				if (valueId < Value.UNKNOWN_ID)
				{
					requestFrame(0); // force our current frame to get populated, BASE_ID will be available
				}
				else if (valueId > Value.UNKNOWN_ID)
				{
					requestVariable(valueId, null);
				}
				
				// after all this we should have our variable cache'd so try again if it wasn't there the first time
				val = m_manager.getValue(valueId);
			}
			
			return val;
		}
		
		/// <summary> Returns the current value object for the given id; never requests it from the player.</summary>
		public virtual Value getRawValue(int valueId)
		{
			return m_manager.getValue(valueId);
		}
		
		/// <summary> Returns the previous value object for the given id -- that is, the value that that
		/// object had the last time the player was suspended.  Never requests it from the
		/// player (because it can't, of course).  Returns <code>null</code> if we don't have
		/// a value for that id.
		/// </summary>
		public virtual Value getPreviousValue(int valueId)
		{
			return m_manager.getPreviousValue(valueId);
		}
		
		/// <summary> Launches a request to obtain all the members of the specified variable, and
		/// store them in the variable which would be returned by
		/// {@link DManager#getVariable(long)}.
		/// 
		/// </summary>
		/// <param name="valueId">id of variable whose members we want; underlying Variable must
		/// already be known by the PlayerSessionManager.
		/// 
		/// </param>
		/// <throws>  NoResponseException </throws>
		/// <throws>  NotConnectedException </throws>
		/// <throws>  NotSuspendedException </throws>
		internal virtual void  obtainMembers(int valueId)
		{
			if (!Suspended)
				throw new NotSuspendedException();
			
			// Get it from cache.  Normally, this should never fail; however, in
			// the case of Flex Builder, which is multithreaded, it is possible
			// that a thread has called this even after a different thread has
			// single-stepped, so that the original variable is no longer valid.
			// So, we'll check for a null return value.
			DValue v = m_manager.getValue(valueId);
			
			if (v != null && !v.membersObtained())
			{
				requestVariable(valueId, null, false, true);
			}
		}
		
		/// <summary> Get the value of the variable named 'name' using varId
		/// as the context id for the Variable.
		/// 
		/// This call is used to fire getters, where the id must
		/// be that of the original object and not the object id
		/// of where the getter actually lives.  For example
		/// a getter a() may live under o.__proto__.__proto__
		/// but you must use the id of o and the name of 'a'
		/// in order for the getter to fire correctly.  [Note: This
		/// paragraph was written for AS2; __proto__ doesn't exist
		/// in AS3.  TODO: revise this paragraph]
		/// </summary>
		public virtual Value getValue(int varId, String name)
		{
			Value v = null;
			if (Suspended)
			{
				int fireGetter = getPreference(SessionManager.PREF_INVOKE_GETTERS);
				
				// disable children attaching to parent variables and clear our
				// most recently seen variable
				m_manager.clearLastVariable();
				m_manager.enableChildAttach(false);
				
				try
				{
					requestVariable(varId, name, (fireGetter != 0), false);
					v = m_manager.lastVariable().getValue();
				}
				catch (NoResponseException e)
				{
					if (fireGetter != 0)
					{
						// We fired a getter -- most likely, what happened is that that getter
						// (which is actual code in the user's movie) just took too long to
						// calculate its value.  So rather than throwing an exception, we store
						// some error text for the value of the variable itself.
						//
						// TODO [mmorearty 4/20/06] Even though I wrote the below code, I now
						// am wondering if it is incorrect that I am calling addVariableMember(),
						// because in every other case, this function does not add members to
						// existing objects.  Need to revisit this.
						v = new DValue(VariableType.STRING, "String", "String", ValueAttribute.IS_EXCEPTION, e.getLocalizedMessage());
						DVariable var = new DVariable(name, (DValue) v);
						m_manager.enableChildAttach(true);
						m_manager.addVariableMember(varId, var);
					}
					else
					{
						throw e; // re-throw
					}
				}
				finally
				{
					// reset our attach flag, so that children attach to parent variables.
					m_manager.enableChildAttach(true);
				}
			}
			else
				throw new NotSuspendedException();
			
			return v;
		}
		
		private void  requestVariable(int id, String name)
		{
			requestVariable(id, name, false, false);
		}
		
		private void  requestVariable(int id, String name, bool fireGetter, bool alsoGetChildren)
		{
			if (!Suspended)
				throw new NotSuspendedException();
			
			name = getRawMemberName(id, name);
			
			DMessage dm = buildOutGetMessage(id, name, fireGetter, alsoGetChildren);
			
			// make sure any exception during the setter gets held onto
			m_manager.beginGetterSetter();
			
			int timeout = getPreference(SessionManager.PREF_GETVAR_RESPONSE_TIMEOUT);
			timeout += 500; // give the player enough time to raise its timeout exception
			
			bool result = simpleRequestResponseMessage(dm, DMessage.InGetVariable, timeout);
			
			// tell manager we're done; ignore returned FaultEvent
			m_manager.endGetterSetter();
			
			if (!result)
				throw new NoResponseException(timeout);
		}
		
		private DMessage buildOutGetMessage(int id, String name, bool fireGetter, bool alsoGetChildren)
		{
			const int FLAGS_SIZE = 4;
			name = (name == null)?"":name; //$NON-NLS-1$
			
			DMessage dm = DMessageCache.alloc(4 + DMessage.getStringLength(name) + 1 + FLAGS_SIZE);
			dm.Type = (!fireGetter)?DMessage.OutGetVariable:DMessage.OutGetVariableWhichInvokesGetter;
			dm.putDWord(id);
			try
			{
				dm.putString(name);
			}
			catch (IOException)
			{
				// couldn't write out the string, so just terminate it and complete anyway
				dm.putByte((byte) '\x0000');
			}
			
			// as an optimization, newer player builds allow us to tell them not to
			// send all the children of an object along with the object, because
			// frequently we don't care about the children
			int flags = GetVariableFlag.DONT_GET_FUNCTIONS; // we never want functions
			if (fireGetter)
				flags |= GetVariableFlag.INVOKE_GETTER;
			if (alsoGetChildren)
				flags |= GetVariableFlag.ALSO_GET_CHILDREN | GetVariableFlag.GET_CLASS_HIERARCHY;
			dm.putDWord(flags);
			
			return dm;
		}
		
		/*
		* @see Flash.Tools.Debugger.Session#setScalarMember(int, java.lang.String, int, java.lang.String)
		*/
		public virtual FaultEvent setScalarMember(int varId, String memberName, int type, String value)
		{
			if (!Suspended)
				throw new NotSuspendedException();
			
			// If the varId is that of a stack frame, then we need to check whether that
			// stack frame has an "activation object".  If it does, then all of the
			// arguments and locals are actually kept as members of that activation
			// object, and so we need to change varId to be the ID of that activation
			// object -- that way, the player will modify the member of the activation
			// object rather than modifying the "regular" argument or local.  See bug
			// 155031.
			if (varId <= Value.BASE_ID && varId > Value.LEVEL_ID)
			{
				int depth = Value.BASE_ID - varId;
				DStackContext context = m_manager.getFrame(depth);
				DVariable activationObject = context.ActivationObject;
				if (activationObject != null)
					varId = activationObject.getValue().Id;
			}
			
			memberName = getRawMemberName(varId, memberName);
			
			// see if it is our any of our special variables
			FaultEvent faultEvent = requestSetVariable(isPseudoVarId(varId)?0:varId, memberName, type, value);
			
			// now that we sent it out, we need to clear our variable cache
			// if it is our special context then mark the frame as stale.
			if (isPseudoVarId(varId) && m_manager.FrameCount > 0)
			{
				m_manager.getFrame(0).markStale();
			}
			else
			{
				DValue parent = m_manager.getValue(varId);
				if (parent != null)
					parent.removeAllMembers();
			}
			
			return faultEvent;
		}
		
		/// <summary> Returns whether a variable ID is "real" or not.  For example,
		/// Value.THIS_ID is a "pseudo" varId, as are all the other special
		/// hard-coded varIds in the Value class.
		/// </summary>
		private bool isPseudoVarId(int varId)
		{
			/*
			* Unfortunately, this is actually just taking a guess.  The old code
			* used "varId &lt; 0"; however, the Linux player sometimes has real
			* variable IDs which are less than zero.
			*/
			return (varId < 0 && varId > - 65535);
		}
		
		/// <summary> <code>memberName</code> might be just <code>"varname"</code>, or it
		/// might be <code>"namespace::varname"</code>, or it might be
		/// <code>"namespace@hexaddr::varname"</code>.  In the third case, it is
		/// fully resolved, and there is nothing we need to do.  But in the first
		/// and second cases, we may need to fully resolve it so that the Player
		/// will recognize it.
		/// </summary>
		private String getRawMemberName(int parentValueId, String memberName)
		{
			if (memberName != null)
			{
				DValue parent = m_manager.getValue(parentValueId);
				if (parent != null)
				{
					int doubleColon = memberName.IndexOf("::"); //$NON-NLS-1$
					String shortName = (doubleColon == - 1)?memberName:memberName.Substring(doubleColon + 2);
					DVariable member = parent.findMember(shortName);
					if (member != null)
						memberName = member.RawName;
				}
			}
			return memberName;
		}
		
		/// <returns> null for success, or fault event if a setter in the player threw an exception
		/// </returns>
		private FaultEvent requestSetVariable(int id, String name, int t, String value)
		{
			// convert type to typeName
			String type = DVariable.typeNameFor(t);
			DMessage dm = buildOutSetMessage(id, name, type, value);
			FaultEvent faultEvent = null;
			//		System.out.println("setmsg id="+id+",name="+name+",t="+type+",value="+value);
			
			// make sure any exception during the setter gets held onto
			m_manager.beginGetterSetter();
			
			// turn off squelch so we can hear the response
			sendSquelch(false);
			
			int timeout = getPreference(SessionManager.PREF_GETVAR_RESPONSE_TIMEOUT);
			
			if (!simpleRequestResponseMessage(dm, (t == VariableType.STRING)?DMessage.InSetVariable:DMessage.InSetVariable2, timeout))
				throw new NoResponseException(getPreference(SessionManager.PREF_RESPONSE_TIMEOUT));
			
			// turn it back on
			sendSquelch(true);
			
			// tell manager we're done, and get exception if any
			faultEvent = m_manager.endGetterSetter();
			
			// hammer the variable cache and context array
			m_manager.freeValueCache();
			return faultEvent;
		}
		
		private DMessage buildOutSetMessage(int id, String name, String type, String v)
		{
			DMessage dm = DMessageCache.alloc(4 + DMessage.getStringLength(name) + DMessage.getStringLength(type) + DMessage.getStringLength(v) + 3);
			dm.Type = DMessage.OutSetVariable;
			dm.putDWord(id);
			try
			{
				dm.putString(name);
			}
			catch (IOException)
			{
				dm.putByte((byte) '\x0000');
			}
			try
			{
				dm.putString(type);
			}
			catch (IOException)
			{
				dm.putByte((byte) '\x0000');
			}
			try
			{
				dm.putString(v);
			}
			catch (IOException)
			{
				dm.putByte((byte) '\x0000');
			}
			return dm;
		}
		
		/*
		* @see Flash.Tools.Debugger.Session#waitForEvent()
		*/
		public virtual void  waitForEvent()
		{
			Object eventNotifier = m_manager.EventNotifier;
			while (EventCount == 0 && Connected)
			{
				lock (eventNotifier)
				{
					System.Threading.Monitor.Wait(eventNotifier);
				}
			}
			
			// We should NOT call isConnected() to test for a broken connection!  That
			// is because we may have received one or more events AND lost the connection,
			// almost simultaneously.  If there are any messages available for the
			// caller to process, we should not throw an exception.
			if (nextEvent() == null && !Connected)
				throw new NotConnectedException();
		}
		
		/*
		* @see Flash.Tools.Debugger.Session#nextEvent()
		*/
		public virtual DebugEvent nextEvent()
		{
			return m_manager.nextEvent();
		}
		
		/// <summary> Adds a watchpoint on the given expression</summary>
		/// <throws>  NotConnectedException  </throws>
		/// <throws>  NoResponseException  </throws>
		/// <throws>  NotSuspendedException  </throws>
		public virtual bool addWatch(int varId, String varName, int type, int tag)
		{
			// TODO check for NoResponse, NotConnected
			varName = getRawMemberName(varId, varName);
			DMessage dm = DMessageCache.alloc(10 + DMessage.getStringLength(varName) + 1);
			dm.Type = DMessage.OutAddWatch2;
			dm.putDWord(varId);
			try
			{
				dm.putString(varName);
			}
			catch (IOException)
			{
				dm.putByte((byte) '\x0000');
			}
			dm.putWord(type);
			dm.putWord(tag);
			
			int timeout = getPreference(SessionManager.PREF_GETVAR_RESPONSE_TIMEOUT);
			bool result = simpleRequestResponseMessage(dm, DMessage.InWatch2, timeout);
			return result;
		}
		
		/// <summary> Removes a watchpoint on the given expression</summary>
		/// <throws>  NotConnectedException  </throws>
		/// <throws>  NoResponseException  </throws>
		/// <throws>  NotSuspendedException  </throws>
		public virtual bool removeWatch(int varId, String memberName)
		{
			memberName = getRawMemberName(varId, memberName);
			DMessage dm = DMessageCache.alloc(6 + DMessage.getStringLength(memberName) + 1);
			dm.Type = DMessage.OutRemoveWatch2;
			dm.putDWord(varId);
			try
			{
				dm.putString(memberName);
			}
			catch (IOException)
			{
				dm.putByte((byte) '\x0000');
			}
			
			int timeout = getPreference(SessionManager.PREF_GETVAR_RESPONSE_TIMEOUT);
			bool result = simpleRequestResponseMessage(dm, DMessage.InWatch2, timeout);
			return result;
		}
		
		/// <summary> Send a message that contains no data</summary>
		internal virtual void  sendMessage(int message)
		{
			DMessage dm = DMessageCache.alloc(0);
			dm.Type = message;
			sendMessage(dm);
		}
		
		/// <summary> Send a fully formed message and release it when done</summary>
		internal virtual void  sendMessage(DMessage dm)
		{
			lock (this)
			{
				try
				{
					m_protocol.txMessage(dm);
					
					if (m_debugMsgOn || m_debugMsgFileOn)
						trace(dm, false);
				}
				catch (IOException io)
				{
					if (Trace.error)
					{
						Trace.trace("Attempt to send message " + dm.outToString() + " failed"); //$NON-NLS-1$ //$NON-NLS-2$
                        Console.Error.Write(io.StackTrace);
                        Console.Error.Flush();
                    }
				}
				DMessageCache.free(dm);
			}
		}
		
		
		/// <summary> Tell the player to shut-up</summary>
		internal virtual bool sendSquelch(bool on)
		{
			bool responded;
			DMessage dm = DMessageCache.alloc(4);
			dm.Type = DMessage.OutSetSquelch;
			dm.putDWord(on?1:0);
			responded = simpleRequestResponseMessage(dm, DMessage.InSquelch);
			return responded;
		}
		
		internal virtual void  sendStopWarning()
		{
			// Currently, "disable_script_stuck_dialog" only works for AS2, not for AS3.
			String option = "disable_script_stuck_dialog"; //$NON-NLS-1$
			String value = "on"; //$NON-NLS-1$
			
			sendOptionMessage(option, value);
			
			// HACK: Completely disable the script-stuck notifications, so that we can
			// get AS3 debugging working.
			option = "disable_script_stuck"; //$NON-NLS-1$
			value = "on"; //$NON-NLS-1$
			
			sendOptionMessage(option, value);
		}
		
		internal virtual void  sendStopOnFault()
		{
			String option = "break_on_fault"; //$NON-NLS-1$
			String value = "on"; //$NON-NLS-1$
			
			sendOptionMessage(option, value);
		}
		
		internal virtual void  sendEnumerateOverride()
		{
			String option = "enumerate_override"; //$NON-NLS-1$
			String value = "on"; //$NON-NLS-1$
			
			sendOptionMessage(option, value);
		}
		
		internal virtual void  sendFailureNotify()
		{
			String option = "notify_on_failure"; //$NON-NLS-1$
			String value = "on"; //$NON-NLS-1$
			
			sendOptionMessage(option, value);
		}
		
		internal virtual void  sendInvokeSetters()
		{
			String option = "invoke_setters"; //$NON-NLS-1$
			String value = "on"; //$NON-NLS-1$
			
			sendOptionMessage(option, value);
		}
		
		internal virtual void  sendSwfloadNotify()
		{
			String option = "swf_load_messages"; //$NON-NLS-1$
			String value = "on"; //$NON-NLS-1$
			
			sendOptionMessage(option, value);
		}
		
		internal virtual void  sendConsoleErrorsAsTrace(bool on)
		{
			String option = "console_errors"; //$NON-NLS-1$
			String value = (on)?"on":"off"; //$NON-NLS-1$ //$NON-NLS-2$
			
			sendOptionMessage(option, value);
		}
		
		internal virtual void  sendGetterTimeout()
		{
			String option = "getter_timeout"; //$NON-NLS-1$
			String value = "" + getPreference(SessionManager.PREF_GETVAR_RESPONSE_TIMEOUT); //$NON-NLS-1$
			
			sendOptionMessage(option, value);
		}
		
		internal virtual void  sendSetterTimeout()
		{
			String option = "setter_timeout"; //$NON-NLS-1$
			String value = "" + getPreference(SessionManager.PREF_SETVAR_RESPONSE_TIMEOUT); //$NON-NLS-1$
			
			sendOptionMessage(option, value);
		}
		
		internal virtual void  sendOptionMessage(String option, String value)
		{
			int msgSize = DMessage.getStringLength(option) + DMessage.getStringLength(value) + 2; // add 2 for trailing nulls of each string
			
			DMessage dm = DMessageCache.alloc(msgSize);
			dm.Type = DMessage.OutSetOption;
			try
			{
				dm.putString(option);
			}
			catch (IOException)
			{
				dm.putByte((byte) '\x0000');
			}
			try
			{
				dm.putString(value);
			}
			catch (IOException)
			{
				dm.putByte((byte) '\x0000');
			}
			simpleRequestResponseMessage(dm, DMessage.InOption);
		}
		
		public virtual bool playerCanTerminate()
		{
			return getOption("can_terminate", false); //$NON-NLS-1$
		}
		
		/// <summary> Returns the value of a Flash Player boolean option that was requested by
		/// OutGetOption and returned by InOption.
		/// 
		/// </summary>
		/// <param name="optionName">the name of the option
		/// </param>
		/// <returns> its value, or null
		/// </returns>
		public virtual bool getOption(String optionName, bool defaultValue)
		{
			bool retval = defaultValue;
			String optionValue = getOption(optionName, null);
			
			if (optionValue != null)
			{
				retval = Boolean.Parse(optionValue);
			}
			
			return retval;
		}
		
		/// <summary> Returns the value of a Flash Player string option that was requested by
		/// OutGetOption and returned by InOption.
		/// 
		/// </summary>
		/// <param name="optionName">the name of the option
		/// </param>
		/// <returns> its value, or null
		/// </returns>
		public virtual String getOption(String optionName, String defaultValue)
		{
			String optionValue = defaultValue;
			
			int msgSize = DMessage.getStringLength(optionName) + 1; // add 1 for trailing null of string
			
			DMessage dm = DMessageCache.alloc(msgSize);
			dm.Type = DMessage.OutGetOption;
			try
			{
				dm.putString(optionName);
			}
			catch (IOException)
			{
				dm.putByte((byte) '\x0000');
			}
			if (simpleRequestResponseMessage(dm, DMessage.InOption))
				optionValue = m_manager.getOption(optionName);
			return optionValue;
		}
		
		/// <summary> Send our message and assume that the next response that is received is
		/// ours.  Primitive but there is no use in setting up a full request / response
		/// pattern since the player doesn't follow it.
		/// 
		/// </summary>
		/// <returns> false is no response.
		/// </returns>
		internal virtual bool simpleRequestResponseMessage(DMessage msg, int msgType, int timeout)
		{
			bool response = false;
			
			// use default or user supplied timeout
			timeout = (timeout > 0)?timeout:getPreference(SessionManager.PREF_RESPONSE_TIMEOUT);
			
			// note the number of messages of this type before our send
			DMessageCounter msgCounter = MessageCounter;
			long num = msgCounter.getInCount(msgType);
			long expect = num + 1;
			
			// send the message
			sendMessage(msg);
			
			long startTime = (DateTime.Now.Ticks - 621355968000000000) / 10000;
			//		System.out.println("sending- "+DMessage.outTypeName(msg.getType())+",timeout="+timeout+",start="+start);
			
			// now wait till we see a message come in
			m_incoming = false;
			lock (msgCounter.InLock)
			{
				while ((expect > msgCounter.getInCount(msgType)) && (DateTime.Now.Ticks - 621355968000000000) / 10000 < startTime + timeout && Connected)
				{
					// block until the message counter tells us that some message has been received
					try
					{
						System.Threading.Monitor.Wait(msgCounter.InLock, TimeSpan.FromMilliseconds(timeout));
					}
					catch (System.Threading.ThreadInterruptedException e)
					{
						// this should never happen
						Console.Error.Write(e.StackTrace);
                        Console.Error.Flush();
                    }
					
					// if we see incoming messages, then we should reset our timeout
					lock (this)
					{
						if (m_incoming)
						{
							startTime = (DateTime.Now.Ticks - 621355968000000000) / 10000;
							m_incoming = false;
						}
					}
				}
			}
			
			if (msgCounter.getInCount(msgType) >= expect)
				response = true;
			else if (timeout <= 0 && Trace.error)
				Trace.trace("Timed-out waiting for " + DMessage.inTypeName(msgType) + " response to message " + msg.outToString()); //$NON-NLS-1$ //$NON-NLS-2$
			
			//		long endTime = System.currentTimeMillis();
			//		System.out.println("    response- "+response+",timeout="+timeout+",elapsed="+(endTime-startTime));
			m_lastResponse = response;
			return response;
		}
		
		// use default timeout
		internal virtual bool simpleRequestResponseMessage(DMessage msg, int msgType)
		{
			return simpleRequestResponseMessage(msg, msgType, - 1);
		}
		internal virtual bool simpleRequestResponseMessage(int msg, int msgType)
		{
			return simpleRequestResponseMessage(msg, msgType, - 1);
		}
		
		// Convenience function
		internal virtual bool simpleRequestResponseMessage(int msg, int msgType, int timeout)
		{
			DMessage dm = DMessageCache.alloc(0);
			dm.Type = msg;
			return simpleRequestResponseMessage(dm, msgType, timeout);
		}
		
		/// <summary> We register ourself as a listener to DMessages from the pipe for the
		/// sole purpose of monitoring the state of the debugger.  All other
		/// object management occurs with DManager
		/// </summary>
		/// <summary> Issued when the socket connection to the player is cut</summary>
		public virtual void  disconnected()
		{
			m_isHalted = false;
			m_isConnected = false;
		}
		
		/// <summary> This is the core routine for decoding incoming messages and deciding what should be
		/// done with them.  We have registered ourself with DProtocol to be notified when any
		/// incoming messages have been received.
		/// 
		/// It is important to note that we should not rely on the contents of the message
		/// since it may be reused after we exit this method.
		/// </summary>
		public virtual void  messageArrived(DMessage msg, DProtocol which)
		{
			if (m_debugMsgOn || m_debugMsgFileOn)
				trace(msg, true);
			
			/* at this point we just open up a big switch statement and walk through all possible cases */
			int type = msg.Type;
			switch (type)
			{
				
				case DMessage.InExit: 
				{
					m_isConnected = false;
					break;
				}
				
				
				case DMessage.InProcessTag: 
				{
					// need to send a response to this message to keep the player going
					sendMessage(DMessage.OutProcessedTag);
					break;
				}
				
				
				case DMessage.InAskBreakpoints: 
				//			case DMessage.InBreakAt:
				case DMessage.InBreakAtExt: 
				{
					m_isHalted = true;
					break;
				}
				
				
				case DMessage.InContinue: 
				{
					m_isHalted = false;
					break;
				}
				
				
				case DMessage.InOption: 
				{
                    String s = msg.getString();
                    String v = msg.getString();
					
					// add it to our properties, for DEBUG purposes only
					m_prefs[s] = v;
					break;
				}
				
				
				default: 
				{
					/*
					* Simple indicator that we have received a message.  We
					* put this indicator in default so that InProcessTag msgs
					* wouldn't generate false triggers.  Mainly, we want to
					* reset our timeout counter when we receive trace messages.
					*/
					m_incoming = true;
					break;
				}
				
			}
			
			// something came in so assume that we can now talk
			// to the player
			m_lastResponse = true;
		}
		
		/// <summary> A background thread which wakes up periodically and fetches the SWF and SWD
		/// from the Player for new movies that have loaded.  It then uses these to create
		/// an instance of MovieMetaData (a class shared with the Profiler) from which
		/// fdb can cull function names.
		/// This work is done on a background thread because it can take several
		/// seconds, and we want the fdb user to be able to execute other commands
		/// while it is happening.
		/// </summary>
		public virtual void  Run()
		{
			long last = 0;
			while (Connected)
			{
				// try every 250ms
				try
				{
					System.Threading.Thread.Sleep(250);
				}
				catch (System.Threading.ThreadInterruptedException)
				{
				}
				
				try
				{
					// let's make sure that the traffic level is low before
					// we do our requests.
					long current = m_protocol.messagesReceived();
					long delta = last - current;
					last = current;
					
					// if the last message that went out was not responded to
					// or we are not suspended and have high traffic
					// then wait for later.
					if (!m_lastResponse || (!Suspended && delta > 5))
						throw new NotSuspendedException();
					
					// we are either suspended or low enough traffic
					
					// get the list of swfs we have
					int count = m_manager.SwfInfoCount;
					for (int i = 0; i < count; i++)
					{
						DSwfInfo info = m_manager.getSwfInfo(i);
						
						// no need to process if it's been removed
						if (info == null || info.isUnloaded() || info.isPopulated() || (info.VmVersion > 0))
							continue;
						
						// see if the swd has been loaded, throws exception if unable to load it.
						// Also triggers a callback into the info object to freshen its contents
						// if successful
						int size = info.getSwdSize(this);
						
						// check since our vm version info could get updated in between.
						if (info.VmVersion > 0)
						{
							// mark it populated if we haven't already done so
							info.setPopulated();
							continue;
						}
						
						// so by this point we know that we've got good swd data,
						// or we've made too many attempts and gave up.
						if (!info.SwdLoading && !info.isUnloaded())
						{
							// now load the swf, if we haven't already got it
							if (info.Swf == null && !info.isUnloaded())
								info.Swf = requestSwf(i);
							
							// only get the swd if we haven't got it
							if (info.Swd == null && !info.isUnloaded())
								info.Swd = requestSwd(i);
							
							try
							{
								// now go populate the functions tables...
								if (!info.isUnloaded())
									info.parseSwfSwd(m_manager);
							}
							catch (Exception e)
							{
								// oh this is not good and means that we should probably
								// give up.
								if (Trace.error)
								{
									Trace.trace("Error while parsing swf/swd '" + info.Url + "'. Giving up and marking it processed"); //$NON-NLS-1$ //$NON-NLS-2$
									Console.Error.Write(e.StackTrace);
                                    Console.Error.Flush();
                                }
								
								info.setPopulated();
							}
						}
					}
				}
				catch (InProgressException)
				{
					// swd is still loading so give us a bit of
					// time and then come back and try again
				}
				catch (NoResponseException)
				{
					// timed out on one of our requests so don't bother
					// continuing right now,  try again later
				}
				catch (NotSuspendedException)
				{
					// probably want to wait until we are halted before
					// doing this heavy action
				}
				catch (Exception e)
				{
					// maybe not good
					if (Trace.error)
					{
						Trace.trace("Exception in background swf/swd processing thread"); //$NON-NLS-1$
                        Trace.trace(e.Message);
                        if (e.InnerException != null)
                        {
                            Trace.trace(e.InnerException.Message);
                        }
                        else
                        {
                            Trace.trace("No inner exception");
                        }
						Console.Error.Write(e.StackTrace);
                        Console.Error.Flush();
                    }
				}
			}
		}
		
		internal virtual byte[] requestSwf(int index)
		{
			/* send the message */
			int to = getPreference(SessionManager.PREF_SWFSWD_LOAD_TIMEOUT);
			byte[] swf = null;
			
			// the query
			DMessage dm = DMessageCache.alloc(2);
			dm.Type = DMessage.OutGetSwf;
			dm.putWord(index);
			
			if (simpleRequestResponseMessage(dm, DMessage.InGetSwf, to))
				swf = m_manager.SWF;
			else
				throw new NoResponseException(to);
			
			return swf;
		}
		
		internal virtual byte[] requestSwd(int index)
		{
			/* send the message */
			int to = getPreference(SessionManager.PREF_SWFSWD_LOAD_TIMEOUT);
			byte[] swd = null;
			
			// the query
			DMessage dm = DMessageCache.alloc(2);
			dm.Type = DMessage.OutGetSwd;
			dm.putWord(index);
			
			if (simpleRequestResponseMessage(dm, DMessage.InGetSwd, to))
				swd = m_manager.SWD;
			else
				throw new NoResponseException(to);
			
			return swd;
		}
		
		//
		// Debug purposes only.  Dump contents of our messages to the screen
		// and/or file.
		//
		internal virtual void trace(DMessage dm, bool in_Renamed)
		{
			lock (this)
			{
				try
				{
					if (m_debugMsgOn)
						Console.Out.WriteLine((in_Renamed)?dm.inToString(m_debugMsgSize):dm.outToString(m_debugMsgSize));
					
					if (m_debugMsgFileOn)
					{
						traceFile().Write((in_Renamed)?dm.inToString(m_debugMsgFileSize):dm.outToString(m_debugMsgFileSize));
						m_trace.Write(s_newline);
						m_trace.Flush();
					}
				}
				catch (Exception)
				{
				}
			}
		}
		
		// i/o for tracing
		internal StreamWriter m_trace;
		
		internal virtual StreamWriter traceFile()
		{
			if (m_trace == null)
			{
				m_trace = new StreamWriter("mm_debug_api_trace.txt", false, System.Text.Encoding.Default); //$NON-NLS-1$
				try
				{
					m_trace.Write(DateTime.Now.ToString("r"));
				}
				catch (Exception)
				{
					m_trace.Write("Date unknown");
				} //$NON-NLS-1$
				try
				{
					m_trace.Write(s_newline);
					
					// java properties dump
					System.Collections.Specialized.NameValueCollection props = SystemProperties.getProperties();

                    foreach (String value in props)
                    {
                        m_trace.WriteLine(value);
                    }
					
					m_trace.Write(s_newline);
					
					// property dump
					System.Collections.IEnumerator keys = new SupportClass.HashSetSupport(m_prefs.Keys).GetEnumerator();
					while (keys.MoveNext())
					{
						Object key = keys.Current;
						Object value = m_prefs[key];
						m_trace.Write(key.ToString());
						m_trace.Write(" = "); //$NON-NLS-1$
						m_trace.Write(value.ToString());
						m_trace.Write(s_newline);
					}
				}
				catch (Exception e)
				{
                    if (Trace.error)
                    {
                        Console.Error.Write(e.StackTrace);
                        Console.Error.Flush();
                    }
				}
				m_trace.Write(s_newline);
			}
			return m_trace;
		}
	}
}