using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using ASCompletion.Settings;
using flash.tools.debugger;
using flash.tools.debugger.events;
using PluginCore.Localization;
using PluginCore.Managers;

namespace FlashDebugger
{
	public delegate void TraceEventHandler(object sender, string trace);
	public delegate void SwfLoadedEventHandler(object sender, SwfLoadedEvent e);
	public delegate void SwfUnloadedEventHandler(object sender, SwfUnloadedEvent e);
	public delegate void DebuggerEventHandler(object sender);
    public delegate void DebuggerProgressEventHandler(object sender, int current, int total);

	public class FlashInterface : IProgress
	{
		// default metadata retry count 8 attempts per waitForMetadata() call * 5 calls
		public const int METADATA_RETRIES = 8 * 5;
		public event TraceEventHandler	TraceEvent;
		public event DebuggerEventHandler DisconnectedEvent;
		public event DebuggerEventHandler PauseFailedEvent;
		public event DebuggerEventHandler StartedEvent;
		public event DebuggerEventHandler BreakpointEvent;
		public event DebuggerEventHandler FaultEvent;
		public event DebuggerEventHandler PauseEvent;
		public event DebuggerEventHandler StepEvent;
		public event DebuggerEventHandler ScriptLoadedEvent;
		public event DebuggerEventHandler WatchpointEvent;
		public event DebuggerEventHandler UnknownHaltEvent;
        public event DebuggerProgressEventHandler ProgressEvent;
		public event DebuggerEventHandler ThreadsEvent;

		#region Public Properties

		public BreakPointManager m_BreakPointManager = null;

		public bool isDebuggerStarted
		{
			get
			{
				return m_Session != null && m_CurrentState != DebuggerState.Initializing && m_CurrentState != DebuggerState.Stopped;
			}
		}

		public bool isDebuggerSuspended
		{
			get
			{
				if (ActiveSession == 1)
				{
					return m_Session.isSuspended();
				}
				return runningIsolates[ActiveSession].i_Session.isSuspended();
			}
		}

		public int suspendReason
		{
			get
			{
				return m_Session.suspendReason();
			}
		}

		public Session Session
		{
			get
			{
				return m_Session;
			}
		}

		public Dictionary<int, IsolateInfo> IsolateSessions
		{
			get
			{
				return runningIsolates;
			}
		}

		public int ActiveSession
		{
			get
			{
				return m_activeSession;
			}
			set
			{
				// todo
				m_activeSession = value;
				if (ThreadsEvent != null)
				{
					ThreadsEvent(this);
				}
			}
		}

		#endregion

		#region Private Properties

		private Session m_Session = null;
		private DebuggerState m_CurrentState = DebuggerState.Initializing;
		private Boolean m_RequestPause;
		private Boolean m_RequestResume;
		private Boolean m_RequestStop;
		private Boolean m_RequestDetach;
		private Boolean m_StepResume;
		private EventWaitHandle m_SuspendWait = new EventWaitHandle(false, EventResetMode.ManualReset);
		private Boolean m_SuspendWaiting;
		private int m_activeSession; // 1 is m_session else lookup runningIsolates

        // Isolates 
        private Dictionary<int, IsolateInfo> runningIsolates;

        private IsolateInfo addRunningIsolate(int i_id)
        {
            removeRunningIsolate(i_id);
            runningIsolates[i_id] = new IsolateInfo();

            return runningIsolates[i_id];
        }

        private void removeRunningIsolate(int i_id)
        {
			if (runningIsolates.ContainsKey(i_id))
			{
				runningIsolates.Remove(i_id);
			}
			if (ActiveSession == i_id)
			{
				ActiveSession = 1;
			}
        }

        // Probably more info needed :)
        public class IsolateInfo
        {
            public IsolateSession i_Session = null;
			public Dictionary<BreakPointInfo, Location> breakpointLocations = new Dictionary<BreakPointInfo, Location>();
			public Boolean requestPause;
        }

		// breakpoint mappings
		private Dictionary<BreakPointInfo, Location> breakpointLocations = new Dictionary<BreakPointInfo, Location>();

		// Global Properties
		private int m_HaltTimeout;
		private int m_UpdateDelay;

		// Session Properties
		private int m_MetadataAttemptsPeriod;	// 1/4s per attempt
		private int m_MetadataNotAvailable;		// counter for failures
		private int m_MetadataAttempts;
		private int m_PlayerFullSupport;

		#endregion

		public FlashInterface()
		{
			m_HaltTimeout = 7000;
			m_UpdateDelay = 25;
			m_CurrentState = DebuggerState.Stopped;
		}

		/// <summary>
		/// Main loop
		/// </summary>
		public void Start()
		{
			m_CurrentState = DebuggerState.Starting;
			m_activeSession = 1;
            SessionManager mgr = Bootstrap.sessionManager();
			//mgr.setDebuggerCallbacks(new FlashDebuggerCallbacks()); // obsoleted
			mgr.setPreference(SessionManager_.PREF_GETVAR_RESPONSE_TIMEOUT, 5000);
			m_RequestDetach = false;
            mgr.startListening();
            try
            {
                m_Session = mgr.accept(this);
                if (mgr.isListening()) mgr.stopListening();
                TraceManager.AddAsync("[Starting debug session with FDB]", -1);
                if (m_Session == null)
                {
                    m_CurrentState = DebuggerState.Stopped;
                    throw new Exception(TextHelper.GetString("Info.UnableToStartDebugger"));
                }
                initSession();
                m_CurrentState = DebuggerState.Running;
                if (StartedEvent != null)
                {
                    StartedEvent(this);
                }
                try
                {
                    waitTilHalted();
                }
                catch (System.Exception){}
                try
                {
                    waitForMetaData();
                }
                catch (System.Exception){}
                m_CurrentState = DebuggerState.Running;
                m_Session.breakOnCaughtExceptions(PluginMain.settingObject.BreakOnThrow);
                // now poke to see if the player is good enough
                try
                {
                    if (m_Session.getPreference(SessionManager_.PLAYER_SUPPORTS_GET) == 0)
                    {
                        TraceManager.AddAsync(TextHelper.GetString("Info.WarningNotAllCommandsSupported"));
                    }
                }
                catch (System.Exception){}
				m_SuspendWaiting = false;
                bool stop = false;
                while (!stop)
                {
					if (m_Session != null && m_Session.getEventCount() > 0) m_SuspendWaiting = false;

                    processEvents();
                    // not there, not connected
                    if (m_RequestStop || m_RequestDetach || !haveConnection())
                    {
                        stop = true;
						if (m_RequestResume)
						{
							// resume before we disconnect, this is in case of ExceptionHalt
							m_Session.resume(); // just throw for now
							if (ThreadsEvent != null) ThreadsEvent(this);
						}
                        continue;
                    }
                    if (m_RequestResume)
                    {
                        // resume execution (request fulfilled)
                        try
                        {
                            if (m_StepResume)
                            {
                                m_Session.stepContinue();
                            }
                            else
                            {
                                m_Session.resume();
                            }
							if (ThreadsEvent != null) ThreadsEvent(this);
                        }
                        catch (NotSuspendedException)
                        {
                            TraceManager.AddAsync(TextHelper.GetString("Info.PlayerAlreadyRunning"));
                        }
                        catch (NoResponseException)
                        {
                            TraceManager.AddAsync(TextHelper.GetString("Info.ProcessNotResponding"));
                        }
                        m_RequestResume = false;
                        m_RequestPause = false;
                        m_StepResume = false;
                        m_CurrentState = DebuggerState.Running;
                        continue;
                    }

                    if (m_Session.isSuspended())
                    {
                        /*
                        * We have stopped for some reason.
                        * Now before we do this see, if we have a valid break reason, since
                        * we could be still receiving incoming messages, even though we have halted.
                        * This is definately the case with loading of multiple SWFs.  After the load
                        * we get info on the swf.
                        */

                        int tries = 3;
                        while (tries-- > 0 && m_Session.suspendReason() == SuspendReason_.Unknown)
                        {
                            try
                            {
                                System.Threading.Thread.Sleep(100);
                                processEvents();
                            }
                            catch (System.Threading.ThreadInterruptedException){}
                        }
                        m_SuspendWait.Reset();

                        switch (m_SuspendWaiting ? -1 : suspendReason)
                        {
                            case 1://SuspendReason_.Breakpoint:
                                m_CurrentState = DebuggerState.BreakHalt;
                                if (BreakpointEvent != null)
                                {
                                    m_RequestPause = true;
									// trigger only for main worker here (if we are main worker, or if we are not and are not suspended)
									if (ActiveSession == 1 || !isDebuggerSuspended)
									{
										Frame[] frames = m_Session.getFrames();
										Location loc = null;
										if (frames.Length > 0)
										{
											loc = frames[0].getLocation();
										}
										if (loc != null && m_BreakPointManager.ShouldBreak(loc.getFile(), loc.getLine(), frames[0]))
										{
											ActiveSession = 1;
											BreakpointEvent(this);
										}
										else
										{
											m_RequestResume = true;
											m_StepResume = true;
										}
									}
									else
									{
										if (ThreadsEvent != null)
										{
											ThreadsEvent(this);
										}
									}
                                }
                                else
                                {
                                    m_RequestResume = true;
                                }
                                break;

                            case 3://SuspendReason_.Fault:
                                m_CurrentState = DebuggerState.ExceptionHalt;
                                if (FaultEvent != null)
                                {
                                    FaultEvent(this);
                                }
                                else
                                {
                                    m_RequestResume = true;
                                }
                                break;

                            case 7://SuspendReason_.ScriptLoaded:
                                try
                                {
                                     waitForMetaData();
                                } 
                                catch (InProgressException) {}
                                m_CurrentState = DebuggerState.PauseHalt;

								// refresh breakpoints
								clearBreakpoints();
								UpdateBreakpoints(m_BreakPointManager.BreakPoints);

                                if (ScriptLoadedEvent != null)
                                {
                                    ScriptLoadedEvent(this);
                                }
                                else
                                {
                                    m_RequestResume = true;
                                }
                                break;

                            case 5://SuspendReason_.Step:
                                m_CurrentState = DebuggerState.BreakHalt;
                                if (StepEvent != null)
                                {
                                    StepEvent(this);
                                }
                                else
                                {
                                    m_RequestResume = true;
                                }
                                break;

                            case 4://SuspendReason_.StopRequest:
                                m_CurrentState = DebuggerState.PauseHalt;
                                if (PauseEvent != null)
                                {
                                    PauseEvent(this);
                                }
                                else
                                {
                                    m_RequestResume = true;
                                }
                                break;

                            case 2://SuspendReason_.Watch:
                                m_CurrentState = DebuggerState.BreakHalt;
                                if (WatchpointEvent != null)
                                {
                                    WatchpointEvent(this);
                                }
                                else
                                {
                                    m_RequestResume = true;
                                }
                                break;

							case -1:
								// waiting for susped to end, do nothing
								break;

                            default:
                                m_CurrentState = DebuggerState.BreakHalt;
                                if (UnknownHaltEvent != null)
                                {
                                    UnknownHaltEvent(this);
                                }
                                else
                                {
                                    m_RequestResume = true;
                                }
                                break;
                        }

                        if (!(m_RequestResume || m_RequestDetach))
                        {
							m_SuspendWaiting = !m_SuspendWait.WaitOne(500, false);
                        }
                    }
                    else
                    {
                        if (m_RequestPause)
                        {
                            try
                            {
                                m_CurrentState = DebuggerState.Pausing;
                                try
                                {
                                    m_Session.suspend();
                                }
                                catch { } // No crash here please...
                                if (!haveConnection()) // no connection => dump state and end
                                {
                                    stop = true;
                                    continue;
                                }
                                else if (!m_Session.isSuspended())
                                {
                                    m_RequestPause = false;
                                    m_CurrentState = DebuggerState.Running;
                                    TraceManager.AddAsync(TextHelper.GetString("Info.CouldNotHalt"));
                                    if (PauseFailedEvent != null)
                                    {
                                        PauseFailedEvent(this);
                                    }
                                }
                            }
                            catch (ArgumentException)
                            {
                                TraceManager.AddAsync(TextHelper.GetString("Info.EscapingFromDebuggerPendingLoop"));
                                stop = true;
                            }
                            catch (IOException io)
                            {
                                System.Collections.IDictionary args = new System.Collections.Hashtable();
                                args["error"] = io.Message; //$NON-NLS-1$
                                TraceManager.AddAsync(replaceInlineReferences(TextHelper.GetString("Info.ContinuingDueToError"), args));
                            }
                            catch (SuspendedException)
                            {
                                // lucky us, already paused
                            }
                        }
                    }
                    // sleep for a bit, then process our events.

					// process isolates
					foreach (var kv in runningIsolates)
					{
						if (kv.Value.requestPause)
						{
							try
							{
								kv.Value.i_Session.suspend();
							}
							catch (Exception e)
							{
								TraceManager.AddAsync("Isolate suspend exception: " + e.ToString());
							}
						}
						kv.Value.requestPause = false;
					}

                    try
                    {
                        System.Threading.Thread.Sleep(m_UpdateDelay);
                    }
                    catch {}
                }
            }
            catch (java.net.SocketException ex)
            {
                // No errors if requested
                if (!m_RequestStop) throw ex;
            }
			catch (java.net.SocketTimeoutException ex)
			{
				if (m_CurrentState != DebuggerState.Starting) throw ex;
				TraceManager.AddAsync("[No debug Flash player connection request]", -1);
			}
            finally
            {
                if (DisconnectedEvent != null)
                {
                    DisconnectedEvent(this);
                }
                exitSession();
            }
		}

        void settingObject_BreakOnThrowChanged(object sender, EventArgs e)
        {
            if (m_CurrentState != DebuggerState.Starting &&
                m_CurrentState != DebuggerState.Stopped)
            {
                m_Session.breakOnCaughtExceptions(PluginMain.settingObject.BreakOnThrow);
            }
        }

		internal virtual void initSession()
		{
			bool correctVersion = true;
			try
			{
				m_Session.bind();
			}
			catch (VersionException)
			{
				correctVersion = false;
			}
			// reset session properties
			m_MetadataAttemptsPeriod = 250;					// 1/4s per attempt
			m_MetadataNotAvailable = 0;						// counter for failures
			m_MetadataAttempts = METADATA_RETRIES;
			m_PlayerFullSupport = correctVersion ? 1 : 0;
			m_RequestStop = false;
			m_RequestPause = false;
			m_RequestResume = false;
			m_StepResume = false;

            runningIsolates = new Dictionary<int, IsolateInfo>();
            PluginMain.settingObject.BreakOnThrowChanged += settingObject_BreakOnThrowChanged;
		}

		/// <summary> If we still have a socket try to send an exit message
		/// Doesn't seem to work ?!?
		/// </summary>
		internal virtual void exitSession()
		{
            PluginMain.settingObject.BreakOnThrowChanged -= settingObject_BreakOnThrowChanged;
            // clear out our watchpoint list and displays
			// keep breakpoints around so that we can try to reapply them if we reconnect
			if (m_Session != null)
			{
				if (m_RequestDetach)
				{
					m_Session.unbind();
				}
				else
				{
					m_Session.terminate();                    
				}
				m_Session = null;
			}
            if (runningIsolates != null) runningIsolates.Clear();
            SessionManager mgr = Bootstrap.sessionManager();
            if (mgr != null && mgr.isListening()) mgr.stopListening();
            m_CurrentState = DebuggerState.Stopped;
			clearBreakpoints();
        }

		internal virtual void Detach()
		{
			m_RequestDetach = true;
			m_SuspendWait.Set();
		}

		private bool haveConnection()
		{
			return m_Session != null && m_Session.isConnected();
		}

		private void waitForMetaData()
		{
			// perform a query to see if our metadata has loaded
			int metadatatries = m_MetadataAttempts;
			int maxPerCall = 8; // cap on how many attempt we make per call
			int tries = System.Math.Min(maxPerCall, metadatatries);
			if (tries > 0)
			{
				int remain = metadatatries - tries; // assume all get used up
				// perform the call and then update our remaining number of attempts
				try
				{
					tries = waitForMetaData(tries);
					remain = metadatatries - tries; // update our used count
				}
				catch (InProgressException ipe)
				{
					m_MetadataAttempts = remain;
					throw ipe;
				}
			}
		}

		/// <summary> Wait for the API to load function names, which
		/// exist in the form of external meta-data.
		/// 
		/// Only do this tries times, then give up
		/// 
		/// We wait period * attempts
		/// </summary>
		public virtual int waitForMetaData(int attempts)
		{
			int start = attempts;
			int period = m_MetadataAttemptsPeriod;
			while (attempts > 0)
			{
				// are we done yet?
                if (MetaDataAvailable) break;
                else
                {
                    try
                    {
                        attempts--;
                        System.Threading.Thread.Sleep(period);
                    }
                    catch (System.Threading.ThreadInterruptedException){}
                }
			}
			// throw exception if still not ready
			if (!MetaDataAvailable) throw new InProgressException();
			return start - attempts; // remaining number of tries
		}

		/// <summary> Ask each swf if metadata processing is complete</summary>
		virtual public bool MetaDataAvailable
		{
			get
			{
				bool allLoaded = true;
				try
				{
					// we need to ask the session since our fileinfocache will hide the exception
					SwfInfo[] swfs = m_Session.getSwfs();
					for (int i = 0; i < swfs.Length; i++)
					{
						// check if our processing is finished.
						SwfInfo swf = swfs[i];
						if (swf != null && !swf.isProcessingComplete())
						{
							allLoaded = false;
							break;
						}
					}
				}
				catch (NoResponseException)
				{
					// ok we still need to wait for player to read the swd in
					allLoaded = false;
				}
				// count the number of times we checked and it wasn't there
				if (!allLoaded) m_MetadataNotAvailable++;
				else
				{
					// success so we reset our attempt counter
					m_MetadataAttempts = METADATA_RETRIES;
				}
				return allLoaded;
			}
		}

		/// <summary> Process the incoming debug event queue</summary>
		internal virtual void processEvents()
		{
			while (m_Session != null && m_Session.getEventCount() > 0)
			{
				DebugEvent e = m_Session.nextEvent();
				if (e is TraceEvent)
				{
					dumpTraceLine(e.information);
					if (TraceEvent != null)
					{
						TraceEvent(this, e.information);
					}
				}
				else if (e is SwfLoadedEvent)
				{
                    if (PluginMain.settingObject.VerboseOutput)
					    dumpSwfLoadedEvent((SwfLoadedEvent)e);
				}
				else if (e is SwfUnloadedEvent)
				{
                    if (PluginMain.settingObject.VerboseOutput)
					    dumpSwfUnloadedEvent((SwfUnloadedEvent)e);
				}
                else if (e is IsolateCreateEvent)
                {
                    TraceManager.AddAsync("Created Worker " + ((IsolateCreateEvent)e).isolate.getId());
                    dumpIsolateCreateEvent((IsolateCreateEvent)e);
                }
                else if (e is IsolateExitEvent)
                {
                    TraceManager.AddAsync("Worker " + ((IsolateExitEvent)e).isolate.getId() + " Exited");
                    dumpIsolateExitEvent((IsolateExitEvent)e);
                }
                else if (e is BreakEvent)
                {
                    // handle only worker events
					BreakEvent be = (BreakEvent)e;
					if (PluginMain.settingObject.VerboseOutput)
						TraceManager.AddAsync("Worker " + be.isolateId + " BreakEvent");
					if (be.isolateId > 1 && runningIsolates.ContainsKey(be.isolateId))
					{
						IsolateInfo ii = runningIsolates[be.isolateId];
						if (!ii.i_Session.isSuspended())
						{
							continue;
						}
						Frame[] frames = ii.i_Session.getFrames();
						Location loc = null;
						if (frames.Length > 0)
						{
							loc = frames[0].getLocation();
						}

						if (ii.i_Session.suspendReason() == 1) // SuspendReason_.Breakpoint
						{
							if (loc != null && m_BreakPointManager.ShouldBreak(loc.getFile(), loc.getLine(), frames[0]))
							{
								// switch and break only if we are not dealing with some other thread
								if (ActiveSession == be.isolateId || !isDebuggerSuspended)
								{
									ActiveSession = be.isolateId;
									if (BreakpointEvent != null) BreakpointEvent(this);
								}
								else
								{
									if (ThreadsEvent != null) ThreadsEvent(this);
								}
							}
							else
							{
								ii.i_Session.stepContinue();
							}
						}
						else if (ii.i_Session.suspendReason() == 3) //SuspendReason_.Fault
						{
							if (ActiveSession == be.isolateId || !isDebuggerSuspended)
							{
								ActiveSession = be.isolateId;
								if (FaultEvent != null) FaultEvent(this); else ii.i_Session.resume();
							}
							else
							{
								if (ThreadsEvent != null) ThreadsEvent(this);
							}
						}
						else if (ii.i_Session.suspendReason() == 5) //SuspendReason_.Step
						{
							if (ActiveSession == be.isolateId || !isDebuggerSuspended)
							{
								ActiveSession = be.isolateId;
								if (StepEvent != null) StepEvent(this); else ii.i_Session.resume();
							}
							else
							{
								if (ThreadsEvent != null) ThreadsEvent(this);
							}
						}
						else if (ii.i_Session.suspendReason() == 4) //SuspendReason_.StopRequest
						{
							if (ActiveSession == be.isolateId || !isDebuggerSuspended)
							{
								ActiveSession = be.isolateId;
								if (PauseEvent != null) PauseEvent(this); else ii.i_Session.resume();
							}
							else
							{
								if (ThreadsEvent != null) ThreadsEvent(this);
							}
						}
						else if (ii.i_Session.suspendReason() == 2) //SuspendReason_.Watch
						{
							if (ActiveSession == be.isolateId || !isDebuggerSuspended)
							{
								ActiveSession = be.isolateId;
								if (WatchpointEvent != null) WatchpointEvent(this); else ii.i_Session.resume();
							}
							else
							{
								if (ThreadsEvent != null) ThreadsEvent(this);
							}
						}
					}
                }
                else if (e is FileListModifiedEvent)
                {
                    // we ignore this
                }
                else if (e is FunctionMetaDataAvailableEvent)
                {
                    // we ignore this
                }
                else if (e is FaultEvent)
                {
                    dumpFaultLine((FaultEvent)e);
                }
                else
                {
                    if (PluginMain.settingObject.VerboseOutput)
                    {
                        System.Collections.IDictionary args = new System.Collections.Hashtable();
                        args["type"] = e; //$NON-NLS-1$
                        args["info"] = e.information; //$NON-NLS-1$
                        TraceManager.AddAsync(replaceInlineReferences(TextHelper.GetString("Info.UnknownEvent"), args));
                    }
                }
			}
		}

		// wait a little bit of time until the player halts, if not throw an exception!
		internal virtual void waitTilHalted()
		{
			if (!haveConnection()) throw new System.InvalidOperationException();
			// spin for a while waiting for a halt; updating trace messages as we get them
			waitForSuspend(m_HaltTimeout, m_UpdateDelay);
			if (!m_Session.isSuspended())
			{
				throw new System.Threading.SynchronizationLockException();
			}
		}

		/// <summary> We spin in this spot until the player reaches the
		/// requested suspend state, either true or false.
		/// 
		/// During this time we wake up every period milliseconds
		/// and update the display and our state with information
		/// received from the debug event queue.
		/// </summary>
		internal virtual void waitForSuspend(int timeout, int period)
		{
			while (timeout > 0)
			{
				// dump our events to the console while we are waiting.
				processEvents();
				if (m_Session.isSuspended())
				{
					break;
				}
				try
				{
					System.Threading.Thread.Sleep(period);
				}
				catch (System.Threading.ThreadInterruptedException){}
				timeout -= period;
			}
		}

		// pretty print a trace statement to the console
		internal virtual void dumpTraceLine(String s)
		{
			TraceManager.AddAsync(s, 1);
		}

		// pretty print a fault statement to the console
		internal virtual void dumpFaultLine(FaultEvent e)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			// use a slightly different format for ConsoleErrorFaults
			if (e is ConsoleErrorFault)
			{
				sb.Append(TextHelper.GetString("Info.LinePrefixWhenDisplayingConsoleError")); //$NON-NLS-1$
				sb.Append(' ');
				sb.Append(e.information);
			}
			else
			{
				String name = e.name();
                sb.Append(TextHelper.GetString("Info.LinePrefixWhenDisplayingFault")); //$NON-NLS-1$
				sb.Append(' ');
				sb.Append(name);
				if ((string)e.information != null && e.information.length() > 0)
				{
                    sb.Append(TextHelper.GetString("Info.InformationAboutFault")); //$NON-NLS-1$
					sb.Append(e.information);
				}
                if (PluginMain.settingObject.VerboseOutput)
                {
                    sb.AppendLine();
                    sb.Append(e.stackTrace());
                }
			}

            if (e.isolateId == 1)
                TraceManager.AddAsync(sb.ToString(), 3);
            else
                TraceManager.AddAsync("Worker " + e.isolateId + ": " + sb.ToString(), 3);
		}

		/// <summary> Called when a swf has been loaded by the player</summary>
		/// <param name="e">event documenting the load
		/// </param>
		internal virtual void dumpSwfLoadedEvent(SwfLoadedEvent e)
		{
			// now rip off any trailing ? options
			int at = e.path.lastIndexOf('?');
			String name = (at > -1) ? e.path.substring(0, (at) - (0)) : e.path;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(TextHelper.GetString("Info.LinePrefixWhenSwfLoaded"));
			sb.Append(' ');
			sb.Append(name);
			sb.Append(" - "); //$NON-NLS-1$
			System.Collections.IDictionary args = new System.Collections.Hashtable();
			args["size"] = e.swfSize.ToString("N0"); //$NON-NLS-1$
			sb.Append(replaceInlineReferences(TextHelper.GetString("Info.SizeAfterDecompression"), args));
            TraceManager.AddAsync(sb.ToString());
		}

		/// <summary> Perform the tasks need for when a swf is unloaded
		/// the player
		/// </summary>
		internal virtual void dumpSwfUnloadedEvent(SwfUnloadedEvent e)
		{
			// now rip off any trailing ? options
			int at = e.path.lastIndexOf('?');
			String name = (at > -1) ? e.path.substring(0, (at) - (0)) : e.path;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(TextHelper.GetString("Info.LinePrefixWhenSwfUnloaded")); //$NON-NLS-1$
			sb.Append(' ');
			sb.Append(name);
            TraceManager.AddAsync(sb.ToString());
		}

        /// <summary> Perform the tasks need for when an isolate is created
		/// </summary>
        internal virtual void dumpIsolateCreateEvent(IsolateCreateEvent e)
        {
            IsolateSession i_Session = m_Session.getWorkerSession(e.isolate.getId());

            i_Session.breakOnCaughtExceptions(true);

            IsolateInfo ii = addRunningIsolate(e.isolate.getId());
			ii.i_Session = i_Session;

			UpdateIsolateBreakpoints(m_BreakPointManager.BreakPoints, ii);

			i_Session.resume();

			if (ThreadsEvent != null)
			{
				ThreadsEvent(this);
			}
        }

        /// <summary> Perform the tasks need for when an isolate has exited
        /// </summary>
        internal virtual void dumpIsolateExitEvent(IsolateExitEvent e)
        {
            removeRunningIsolate(e.isolate.getId());
			if (ThreadsEvent != null)
			{
				ThreadsEvent(this);
			}

		}

		internal virtual Location getCurrentLocation()
		{
			if (ActiveSession == 1)
			{
				return getCurrentMainLocation();
			}
			return getCurrentIsolateLocation(ActiveSession);
		}

		internal virtual Location getCurrentMainLocation()
		{
			Location where = null;
			try
			{
				Frame[] frames = m_Session.getFrames();

				where = frames.Length > 0 ? frames[0].getLocation() : null;
			}
			catch (PlayerDebugException)
			{
				// where == null
			}
			return where;
		}

        internal virtual Location getCurrentIsolateLocation(int i_id)
        {
            IsolateInfo ii = runningIsolates[i_id];

            Location where = null;
            try
            {
                Frame[] frames = ii.i_Session.getFrames();

                where = frames.Length > 0 ? frames[0].getLocation() : null;
            }
            catch (PlayerDebugException)
            {
                // where == null
            }
            return where;
        }

		public void Stop()
		{
            m_RequestStop = true;
            if (m_CurrentState == DebuggerState.Starting)
            {
                SessionManager mgr = Bootstrap.sessionManager();
                mgr.stopListening();
                return;
            }
			if (m_CurrentState == DebuggerState.ExceptionHalt)
			{
				m_RequestResume = true;
			}
			m_SuspendWait.Set();
		}

		public void Cleanup()
		{
			if (isDebuggerStarted)
			{
				Stop();
			}
		}

		public void Next()
		{
			if (ActiveSession > 1)
			{
				if (runningIsolates[ActiveSession].i_Session.isSuspended())
				{
					runningIsolates[ActiveSession].i_Session.stepOver();
				}
				return;
			}
			if (m_Session.isSuspended())
			{
				m_Session.stepOver();
				m_SuspendWait.Set();
			}
		}

		public void Step()
		{
			if (ActiveSession > 1)
			{
				if (runningIsolates[ActiveSession].i_Session.isSuspended())
				{
					runningIsolates[ActiveSession].i_Session.stepInto();
				}
				return;
			}
			if (m_Session.isSuspended())
			{
				m_Session.stepInto();
				m_SuspendWait.Set();
			}
		}

		public void StepResume()
		{
			// fix me, needs to move to thread
			if (ActiveSession > 1)
			{
				if (runningIsolates[ActiveSession].i_Session.isSuspended())
				{
					runningIsolates[ActiveSession].i_Session.resume();
				}
				return;
			}
			if (m_Session.isSuspended())
			{
				m_StepResume = true;
				m_RequestResume = true;
				m_SuspendWait.Set();
			}
		}

		public void Continue()
		{
			// fix me, needs to move to thread
			if (ActiveSession > 1)
			{
				if (runningIsolates[ActiveSession].i_Session.isSuspended())
				{
					runningIsolates[ActiveSession].i_Session.resume();
				}
				return;
			}
			if (m_Session.isSuspended())
			{
				m_RequestResume = true;
				m_SuspendWait.Set();
			}
		}

		public void Pause()
		{
			// fix me, needs to move to thread
			if (ActiveSession > 1)
			{
				if (!runningIsolates[ActiveSession].i_Session.isSuspended())
				{
					runningIsolates[ActiveSession].requestPause = true;
					m_SuspendWait.Set();
				}
				return;
			}
			if (!m_Session.isSuspended())
			{
				m_RequestPause = true;
				m_SuspendWait.Set();
			}
		}

		public void Finish()
		{
			if (ActiveSession > 1)
			{
				if (runningIsolates[ActiveSession].i_Session.isSuspended())
				{
					runningIsolates[ActiveSession].i_Session.stepOut();
				}
				return;
			}
			if (m_Session.isSuspended())
			{
				m_Session.stepOut();
				m_SuspendWait.Set();
			}
		}

		public Frame[] GetFrames()
		{
			if (ActiveSession == 1)
			{
				return m_Session.getFrames();
			}
			return runningIsolates[ActiveSession].i_Session.getFrames();
		}

		public Variable[] GetArgs(int frameNumber)
		{
			return GetFrames()[frameNumber].getArguments(m_Session);
		}

		public Variable GetThis(int frameNumber)
		{
			return GetFrames()[frameNumber].getThis(m_Session);
		}

		public Variable[] GetLocals(int frameNumber)
		{
			return GetFrames()[frameNumber].getLocals(m_Session);
		}

		//public Value GetValue(int idValue)
		//{
		//	return m_Session.getValue(idValue);
		//}

		public void UpdateBreakpoints(List<BreakPointInfo> breakpoints)
		{
			commonUpdateBreakpoints(breakpoints, breakpointLocations, null);
			foreach (IsolateInfo ii in IsolateSessions.Values)
			{
				UpdateIsolateBreakpoints(breakpoints, ii);
			}
		}

		private void UpdateIsolateBreakpoints(List<BreakPointInfo> breakpoints, IsolateInfo ii)
		{
			commonUpdateBreakpoints(breakpoints, ii.breakpointLocations, ii.i_Session);
		}

		private void commonUpdateBreakpoints(List<BreakPointInfo> breakpoints, Dictionary<BreakPointInfo, Location> breakpointLocations, IsolateSession i_Session)
		{
			Dictionary<string, int> files = new Dictionary<string, int>();
			foreach (BreakPointInfo bp in breakpoints)
			{
				if (!breakpointLocations.ContainsKey(bp))
				{
					if (!bp.IsDeleted && bp.IsEnabled)
					{
						if (!files.ContainsKey(bp.FileFullPath))
						{
							files.Add(bp.FileFullPath, int.MaxValue);
						}
					}
				}
			}
			int nFiles = files.Count;
			if (nFiles > 0)
			{
                // reverse loop to take latest loded swf first, and ignore old swf.
                SwfInfo[] swfInfo = (i_Session != null) ? i_Session.getSwfs() : m_Session.getSwfs();
                for (int swfC = swfInfo.Length - 1; swfC >= 0; swfC--) 
				{
                    SwfInfo swf = swfInfo[swfC];
					if (swf == null) continue;
					try
					{
						foreach (SourceFile src in swf.getSourceList(m_Session))
						{
							String localPath = PluginMain.debugManager.GetLocalPath(src);
							if (localPath != null && files.ContainsKey(localPath) && files[localPath] > src.getId())
							{
								files[localPath] = src.getId();
							}
						}
					}
					catch (InProgressException) { }
				}
			}

			foreach (BreakPointInfo bp in breakpoints)
			{
				if (!breakpointLocations.ContainsKey(bp))
				{
					if (bp.IsEnabled && !bp.IsDeleted)
					{
						if (files.ContainsKey(bp.FileFullPath) && files[bp.FileFullPath] != int.MaxValue)
						{
							Location l = (i_Session != null) ? i_Session.setBreakpoint(files[bp.FileFullPath], bp.Line + 1) : m_Session.setBreakpoint(files[bp.FileFullPath], bp.Line + 1);
                            if (l != null)
                            {
                                breakpointLocations.Add(bp, l);
                            }
						}
					}
				}
				else
				{
					if (bp.IsDeleted || !bp.IsEnabled)
					{
						// todo, i_Session does not have a clearBreakpoint method, m_Session clears them all. optimize out extra loops
						m_Session.clearBreakpoint(breakpointLocations[bp]);
						breakpointLocations.Remove(bp);
					}
				}
			}
		}

		private void clearBreakpoints()
		{
			if (isDebuggerStarted)
			{
				foreach (Location l in m_Session.getBreakpointList())
				{
					m_Session.clearBreakpoint(l);
				}
			}
            if (breakpointLocations != null) breakpointLocations.Clear();
			if (IsolateSessions != null)
                foreach (IsolateInfo ii in IsolateSessions.Values)
			    {
				    ii.breakpointLocations.Clear();
			    }
		}

		private static String replaceInlineReferences(String text, System.Collections.IDictionary parameters)
		{
			if (parameters == null) return text;
			int depth = 100;
			while (depth-- > 0)
			{
				int o = text.IndexOf("${");
				if (o == -1) break;
				if ((o >= 1) && (text[o - 1] == '$'))
				{
					o = text.IndexOf("${", o + 2);
					if (o == -1) break;
				}
				int c = text.IndexOf("}", o);
				if (c == -1)
				{
					return null; // FIXME
				}
				String name = text.Substring(o + 2, (c) - (o + 2));
				String value = null;
				if (parameters.Contains(name) && (parameters[name] != null))
				{
					value = parameters[name].ToString();
				}
				if (value == null)
				{
					value = "";
				}
				text = text.Substring(0, (o) - (0)) + value + text.Substring(c + 1);
			}
			return text.Replace("$${", "${");
		}

        public void setProgress(int current, int total)
        {
            if (ProgressEvent != null)
            {
                ProgressEvent(this, current, total);
            }
        }

    }

}
