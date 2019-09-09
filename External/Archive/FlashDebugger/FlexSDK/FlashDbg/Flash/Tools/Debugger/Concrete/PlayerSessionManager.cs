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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

using LocalizationManager = Flash.Localization.LocalizationManager;
using URLHelper = Flash.Util.URLHelper;

namespace Flash.Tools.Debugger.Concrete
{
	
	public class PlayerSessionManager : SessionManager
	{
		public override bool Listening
		{
			/*
			* @see Flash.Tools.Debugger.SessionManager#isListening()
			*/
			
			get
			{
				return (m_serverSocket == null)?false:true;
			}
			
		}
		/// <summary> Look in the Windows registry for the Mozilla version of the Flash player.</summary>
		private FileInfo WindowsMozillaPlayerPathFromRegistry
		{
			get
			{
				String KEY = "\\SOFTWARE\\MozillaPlugins\\@adobe.com/FlashPlayer"; //$NON-NLS-1$
				String PATH = "Path"; //$NON-NLS-1$
				
				// According to
				//
				//    http://developer.mozilla.org/en/docs/Plugins:_The_first_install_problem
				//
				// the MozillaPlugins key can be written to either HKEY_CURRENT_USER or
				// HKEY_LOCAL_MACHINE.  Unfortunately, as of this writing, Firefox
				// (version 2.0.0.2) doesn't actually work that way -- it only checks
				// HKEY_LOCAL_MACHINE, but not HKEY_CURRENT_USER.
				//
				// But in hopeful anticipation of a fix for that, we are going to check both
				// locations.  On current builds, that won't do any harm, because the
				// current Flash Player installer only writes to HKEY_LOCAL_MACHINE.  In the
				// future, if Mozilla gets fixed and then the Flash player installer gets
				// updated, then our code will already work correctly.
				//
				// Another quirk: In my opinion, it would be better for Mozilla to look first
				// in HKEY_CURRENT_USER, and then in HKEY_LOCAL_MACHINE.  However, according to
				//
				//    http://developer.mozilla.org/en/docs/Installing_plugins_to_Gecko_embedding_browsers_on_Windows
				//
				// they don't agree with that -- they want HKEY_LOCAL_MACHINE first.
				
				String[] roots = new String[]{"HKEY_LOCAL_MACHINE", "HKEY_CURRENT_USER"}; //$NON-NLS-1$ //$NON-NLS-2$
				for (int i = 0; i < roots.Length; ++i)
				{
					try
					{
						String path = m_debuggerCallbacks.queryWindowsRegistry(roots[i] + KEY, PATH);
						if (path != null)
							return new FileInfo(path);
					}
					catch (IOException)
					{
						// ignore
					}
				}
				
				return null;
			}
			
		}
		/// <summary> Callback for ProcessListener </summary>
		virtual public int ProcessDead
		{
			set
			{
				m_processExitValue = value;
				m_processDead = true; // called if process we launch dies
			}
			
		}
		/// <summary> Returns the localization manager.  Use this for all localized strings.</summary>
		public static LocalizationManager LocalizationManager
		{
			get
			{
				return m_localizationManager;
			}
			
		}
		internal System.Net.Sockets.TcpListener m_serverSocket;
		internal System.Collections.Hashtable m_prefs;
		internal bool m_processDead;
		private IDebuggerCallbacks m_debuggerCallbacks;
		private static LocalizationManager m_localizationManager;
		private StringWriter m_processMessages;
		private int m_processExitValue;
		private String[] m_launchCommand;
		private static readonly String s_newline = Environment.NewLine; //$NON-NLS-1$
		
		public PlayerSessionManager()
		{
			m_debuggerCallbacks = new DefaultDebuggerCallbacks();
			
			m_serverSocket = null;
			m_prefs = new System.Collections.Hashtable();
			
			// manager
			setPreference(SessionManager.PREF_ACCEPT_TIMEOUT, 120000); // 2 minutes
			setPreference(SessionManager.PREF_URI_MODIFICATION, 1);
			
			// session
			
			// response to requests
			setPreference(SessionManager.PREF_RESPONSE_TIMEOUT, 750); // 0.75s
			setPreference(SessionManager.PREF_CONTEXT_RESPONSE_TIMEOUT, 1000); // 1s
			setPreference(SessionManager.PREF_GETVAR_RESPONSE_TIMEOUT, 1500); // 1.5s
			setPreference(SessionManager.PREF_SETVAR_RESPONSE_TIMEOUT, 5000); // 5s
			setPreference(SessionManager.PREF_SWFSWD_LOAD_TIMEOUT, 5000); // 5s
			
			// wait for a suspend to occur after a halt
			setPreference(SessionManager.PREF_SUSPEND_WAIT, 7000);
			
			// invoke getters by default
			setPreference(SessionManager.PREF_INVOKE_GETTERS, 1);
			
			// hierarchical variables view
			setPreference(SessionManager.PREF_HIERARCHICAL_VARIABLES, 0);
		}
		
		/// <summary> Set preference 
		/// If an invalid preference is passed, it will be silently ignored.
		/// </summary>
        public override void setPreference(String pref, int value)
		{
			m_prefs[pref] = (Int32) value;
		}
        public override void setPreference(String pref, String value)
		{
			m_prefs[pref] = value;
		}

        public virtual SupportClass.SetSupport keySet()
		{
			return new SupportClass.HashSetSupport(m_prefs.Keys);
		}

        public virtual Object getPreferenceAsObject(String pref)
		{
			return m_prefs[pref];
		}
		
		/*
		* @see Flash.Tools.Debugger.SessionManager#getPreference(java.lang.String)
		*/
        public override int getPreference(String pref)
		{
            if (!m_prefs.ContainsKey(pref))
            {
				throw new NullReferenceException();
            }

			return (int) m_prefs[pref];
		}
		
		/*
		* @see Flash.Tools.Debugger.SessionManager#startListening()
		*/
        public override void startListening()
        {
            startListening(false);
        }
        public override void startListening(Boolean useAny)
		{
			if (m_serverSocket == null)
			{
				System.Net.Sockets.TcpListener temp_tcpListener;
                if (useAny) temp_tcpListener = new TcpListener(IPAddress.Any, DProtocol.DEBUG_PORT);
                else temp_tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), DProtocol.DEBUG_PORT);
                temp_tcpListener.Start(); // Start now...
				m_serverSocket = temp_tcpListener;
			}
		}
		
		/*
		* @see Flash.Tools.Debugger.SessionManager#stopListening()
		*/
        public override void stopListening()
		{
			if (m_serverSocket != null)
			{
				m_serverSocket.Stop();
				m_serverSocket = null;
			}
		}
		
		private class LaunchInfo
		{
			virtual public bool HttpOrAbout
			{
				get
				{
					return m_uri.StartsWith("http:") || m_uri.StartsWith("https:") || m_uri.StartsWith("about:"); //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
				}
				
			}
			virtual public bool WebPage
			{
				get
				{
					return HttpOrAbout || m_uri.EndsWith(".htm") || m_uri.EndsWith(".html"); //$NON-NLS-1$ //$NON-NLS-2$
				}
				
			}
			virtual public bool WebBrowserNativeLaunch()
			{
                return WebPage && (m_SessionManager.m_debuggerCallbacks.getHttpExe() != null);
			}

			virtual public bool PlayerNativeLaunch()
			{
                return m_uri.Length > 0 && !WebPage && (m_SessionManager.m_debuggerCallbacks.getPlayerExe() != null);
			}

            private String m_uri;
            private PlayerSessionManager m_SessionManager;
			
			public LaunchInfo(PlayerSessionManager sessionManager, String uri)
			{
                m_SessionManager = sessionManager;
                m_uri = uri;
			}
		}
		
		/*
		* @see Flash.Tools.Debugger.SessionManager#launch(java.lang.String, Flash.Tools.Debugger.AIRLaunchInfo, boolean, Flash.Tools.Debugger.IProgress)
		*/
        public override Session launch(String uri, AIRLaunchInfo airLaunchInfo, bool forDebugging, IProgress waitReporter)
		{
			bool modify = (getPreference(SessionManager.PREF_URI_MODIFICATION) != 0);
			LaunchInfo launchInfo = new LaunchInfo(this, uri);
			bool nativeLaunch = launchInfo.WebBrowserNativeLaunch() || launchInfo.PlayerNativeLaunch();
			
			// one of these is assigned to launchAction
			const int NO_ACTION = 0; // no special action
            const int SHOULD_LISTEN = 1; // create a ProcessListener
            const int WAIT_FOR_LAUNCH = 2; // block until process completes
			
			int launchAction; // either NO_ACTION, SHOULD_LISTEN, or WAIT_FOR_LAUNCH
			
			uri = uri.Trim();

            // bool isMacOSX = false;
            bool isWindows = false;
			// if isMacOSX and isWindows are both false, then it's *NIX
            if (Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                // isMacOSX = true;
            }
            else if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                isWindows = true;
            }
			
			if (airLaunchInfo == null)
			{
				// first let's see if it's an HTTP URL or not
				if (launchInfo.HttpOrAbout)
				{
					if (modify && forDebugging && !uri.StartsWith("about:"))
					//$NON-NLS-1$
					{
						// escape spaces if we have any
						uri = URLHelper.escapeSpace(uri);
						
						// be sure that ?debug=true is included in query string
						URLHelper urlHelper = new URLHelper(uri);
						System.Collections.IDictionary parameters = urlHelper.getParameterMap();
						parameters["debug"] = "true"; //$NON-NLS-1$ //$NON-NLS-2$
                        urlHelper.setParameterMap(parameters);
						
						uri = urlHelper.URL;
					}
				}
				else
				{
					// ok, its not an http: type request therefore we should be able to see
					// it on the file system, right?  If not then it's probably not valid
					FileInfo f = null;
					if (uri.StartsWith("file:")) //$NON-NLS-1$
					{
						f = new FileInfo(new Uri(uri).LocalPath);
					}
					else
					{
						f = new FileInfo(uri);
					}
					
					if (f != null && f.Exists)
						uri = f.FullName;
					else
						throw new FileNotFoundException(uri);
				}
				
				if (nativeLaunch)
				{
					// We used to have
					//
					//		launchAction = SHOULD_LISTEN;
					//
					// However, it turns out that when you launch Firefox, if there
					// is another instance of Firefox already running, then the
					// new instance just passes a message to the old one and then
					// immediately exits.  So, it doesn't work to abort when our
					// child process dies.
					launchAction = NO_ACTION;
				}
				else
				{
					launchAction = NO_ACTION;
				}
				
				/*
				* Various ways to launch this stupid thing.  If we have the exe
				* values for the player, then we can launch it directly, monitor
				* it and kill it when we die; otherwise we launch it through
				* a command shell (cmd.exe, open, or bash) and our Process object
				* dies right away since it spawned another process to run the
				* Player within.
				*/
#if false
				if (isMacOSX)
				{
					if (launchInfo.WebBrowserNativeLaunch)
					{
						FileInfo httpExe = m_debuggerCallbacks.getHttpExe();
						m_launchCommand = new String[]{"/usr/bin/open", "-a", httpExe.ToString(), uri}; //$NON-NLS-1$ //$NON-NLS-2$
					}
					else if (launchInfo.PlayerNativeLaunch)
					{
						FileInfo playerExe = m_debuggerCallbacks.getPlayerExe();
						m_launchCommand = new String[]{"/usr/bin/open", "-a", playerExe.ToString(), uri}; //$NON-NLS-1$ //$NON-NLS-2$
					}
					else
					{
						m_launchCommand = new String[]{"/usr/bin/open", uri}; //$NON-NLS-1$
					}
				}
				else
#endif
				{
					
					if (launchInfo.WebBrowserNativeLaunch())
					{
						FileInfo httpExe = m_debuggerCallbacks.getHttpExe();
						m_launchCommand = new String[]{httpExe.ToString(), uri};
					}
					else if (launchInfo.PlayerNativeLaunch())
					{
						FileInfo playerExe = m_debuggerCallbacks.getPlayerExe();
						m_launchCommand = new String[]{playerExe.ToString(), uri};
					}
					else
					{
						if (isWindows)
						{
							// We must quote all ampersands in the URL; if we don't, then
							// cmd.exe will interpret the ampersand as a command separator.
							uri = uri.Replace("&", "\"&\""); //$NON-NLS-1$ //$NON-NLS-2$
							
							m_launchCommand = new String[]{"cmd", "/c", "start", uri}; //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
						}
						else
						{
							String exeName;
							if (launchInfo.WebPage)
								exeName = m_debuggerCallbacks.HttpExeName;
							else
								exeName = m_debuggerCallbacks.PlayerExeName;
							throw new FileNotFoundException(exeName);
						}
					}
				}
			}
			// else, AIR
			else
			{
				if (forDebugging)
					launchAction = SHOULD_LISTEN;
				// wait inside accept() until ADL exits
				else
					launchAction = NO_ACTION; // just launch it
				
                LinkedList<String> cmdList = new LinkedList<String>();
				
				cmdList.AddLast(airLaunchInfo.airDebugLauncher.FullName);
				
				if (airLaunchInfo.airRuntimeDir != null && airLaunchInfo.airRuntimeDir.Length > 0)
				{
                    cmdList.AddLast("-runtime"); //$NON-NLS-1$
                    cmdList.AddLast(airLaunchInfo.airRuntimeDir.FullName);
				}
				
				if (airLaunchInfo.airSecurityPolicy != null && airLaunchInfo.airSecurityPolicy.Length > 0)
				{
                    cmdList.AddLast("-security-policy"); //$NON-NLS-1$
                    cmdList.AddLast(airLaunchInfo.airSecurityPolicy.FullName);
				}
				
				if (airLaunchInfo.airPublisherID != null && airLaunchInfo.airPublisherID.Length > 0)
				{
                    cmdList.AddLast("-pubid"); //$NON-NLS-1$
                    cmdList.AddLast(airLaunchInfo.airPublisherID);
				}
				
				// If it's a "file:" URL, then pass the actual filename; otherwise, use the URL
				// ok, its not an http: type request therefore we should be able to see
				// it on the file system, right?  If not then it's probably not valid
				FileInfo f = null;
				if (uri.StartsWith("file:"))
				//$NON-NLS-1$
				{
                    f = new FileInfo(new Uri(uri).LocalPath);
                    cmdList.AddLast(f.FullName);
				}
				else
				{
                    cmdList.AddLast(uri);
				}
				
				if (airLaunchInfo.applicationContentRootDir != null)
				{
                    cmdList.AddLast(airLaunchInfo.applicationContentRootDir.FullName);
				}
				
				if (airLaunchInfo.applicationArguments != null && airLaunchInfo.applicationArguments.Length > 0)
				{
                    cmdList.AddLast("--"); //$NON-NLS-1$

                    foreach (String arg in splitArgs(airLaunchInfo.applicationArguments))
                    {
                        cmdList.AddLast(arg);
                    }
				}

                m_launchCommand = new String[cmdList.Count];
                int index = 0;

                foreach (String arg in cmdList)
                {
                    m_launchCommand[index++] = arg;
                }
			}
			
			ProcessListener pl = null;
			PlayerSession session = null;
			try
			{
				// create the process and attach a thread to watch it during our accept phase
				System.Diagnostics.Process proc = m_debuggerCallbacks.launchDebugTarget(m_launchCommand);
				
				m_processMessages = new StringWriter();
				new StreamListener(proc.StandardOutput, m_processMessages).Start();
                new StreamListener(proc.StandardError, m_processMessages).Start();
#if false
				try
				{
					Stream stm = proc.StandardOutput.BaseStream;
					if (stm != null)
						stm.Close();
				}
				catch (IOException)
				{
					/* not serious; ignore */
				}
#endif				
				switch (launchAction)
				{
					case NO_ACTION: 
						break;

					case SHOULD_LISTEN: 
					{
						// allows us to hear when the process dies
						pl = new ProcessListener(this, proc);
						pl.Start();
						break;
					}

					case WAIT_FOR_LAUNCH: 
					{
						// block until the process completes
						bool done = false;
						while (!done)
						{
							try
							{
								proc.WaitForExit();
								Int32 generatedAux = proc.ExitCode;
								done = true;
							}
							catch (System.Threading.ThreadInterruptedException)
							{
								/* do nothing */
							}
						}
						if (proc.ExitCode != 0)
						{
							throw new IOException(m_processMessages.ToString());
						}
						break;
					}
				}
				
				if (forDebugging)
				{
					/* now wait for a connection */
					session = (PlayerSession) accept(waitReporter, airLaunchInfo != null);
					session.LaunchProcess = proc;
					session.LaunchUrl = uri;
					session.AIRLaunchInfo = airLaunchInfo;
				}
			}
			finally
			{
				if (pl != null)
					pl.Finish();
			}
			return session;
		}
		
		/// <summary> This is annoying: We must duplicate the operating system's behavior
		/// with regard to splitting arguments.
		/// 
		/// </summary>
		/// <param name="arguments">A single string of arguments that are intended to
		/// be passed to an AIR application.  The tricky part is that some
		/// of the arguments may be quoted, and if they are, then the quoting
		/// will be in a way that is specific to the current platform.  For
		/// example, on Windows, strings are quoted with the double-quote character
		/// ("); on Mac and Unix, strings can be quoted with either double-quote
		/// or single-quote.
		/// </param>
		/// <returns> The equivalent
		/// </returns>
		private System.Collections.IList splitArgs(String arguments)
		{
			System.Collections.IList retval = new System.Collections.ArrayList();
			
			arguments = arguments.Trim();
			
			// Windows quotes only with double-quote; Mac and Unix also allow single-quote.
            bool isMacOrUnix = Environment.OSVersion.Platform == PlatformID.Unix;
            bool isWindows = !isMacOrUnix;
			
			int i = 0;
			while (i < arguments.Length)
			{
				char ch = arguments[i];
				if (ch == ' ' || ch == '\t')
				{
					// keep looping
					i++;
				}
				else if (ch == '"' || (isMacOrUnix && ch == '\''))
				{
					char quote = ch;
					int nextQuote = arguments.IndexOf((Char) quote, i + 1);
					if (nextQuote == - 1)
					{
						retval.Add(arguments.Substring(i + 1));
						return retval;
					}
					else
					{
						retval.Add(arguments.Substring(i + 1, (nextQuote) - (i + 1)));
						i = nextQuote + 1;
					}
				}
				else
				{
					int startPos = i;
					while (i < arguments.Length)
					{
						ch = arguments[i];
						if (ch == ' ' || ch == '\t')
						{
							break;
						}
						i++;
					}
					retval.Add(arguments.Substring(startPos, (i) - (startPos)));
				}
			}
			
			return retval;
		}
		
		/*
		* @see Flash.Tools.Debugger.SessionManager#playerForUri(java.lang.String)
		*/
        public override Player playerForUri(String url)
		{
			// Find the Netscape plugin
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				FileInfo flashPlugin = new FileInfo("/Library/Internet Plug-Ins/Flash Player.plugin"); //$NON-NLS-1$
				return new NetscapePluginPlayer(m_debuggerCallbacks.getHttpExe(), flashPlugin);
			}
			else
			{
				LaunchInfo launchInfo = new LaunchInfo(this, url);
				if (launchInfo.WebBrowserNativeLaunch())
				{
					FileInfo httpExe = m_debuggerCallbacks.getHttpExe();
					if (httpExe.Name.ToUpper().Equals("iexplore.exe".ToUpper()))
					//$NON-NLS-1$
					{
						// IE on Windows: Find the ActiveX control
						String activeXFile = null;
						try
						{
							activeXFile = m_debuggerCallbacks.queryWindowsRegistry("HKEY_CLASSES_ROOT\\CLSID\\{D27CDB6E-AE6D-11cf-96B8-444553540000}\\InprocServer32", null); //$NON-NLS-1$
						}
						catch (IOException)
						{
							// ignore
						}
						if (activeXFile == null)
							return null; // we couldn't find the player
						FileInfo file = new FileInfo(activeXFile);
						return new ActiveXPlayer(httpExe, file);
					}
					else
					{
						// Find the Netscape plugin
						FileInfo browserDir = new FileInfo(httpExe.DirectoryName);
						
						// Opera puts plugins under "program\plugins" rather than under "plugins"
						if (httpExe.Name.ToUpper().Equals("opera.exe".ToUpper()))
						//$NON-NLS-1$
							browserDir = new FileInfo(browserDir.FullName + "\\" + "program"); //$NON-NLS-1$
						
						FileInfo pluginsDir = new FileInfo(browserDir.FullName + "\\" + "plugins"); //$NON-NLS-1$
						FileInfo flashPlugin = new FileInfo(pluginsDir.FullName + "\\" + "NPSWF32.dll"); // WARNING, Windows-specific //$NON-NLS-1$
						
						// Bug 199175: The player is now installed via a registry key, not
						// in the "plugins" directory.
						//
						// Although Mozilla does not document this, the actual behavior of
						// the browser seems to be that it looks first in the "plugins" directory,
						// and then, if the file is not found there, it looks in the registry.
						// So, we mimic that behavior.
						bool tmpBool;
						if (File.Exists(flashPlugin.FullName))
							tmpBool = true;
						else
							tmpBool = Directory.Exists(flashPlugin.FullName);
						if (!tmpBool)
						{
							FileInfo pathFromRegistry = WindowsMozillaPlayerPathFromRegistry;
							
							if (pathFromRegistry != null)
								flashPlugin = pathFromRegistry;
						}
						
						return new NetscapePluginPlayer(httpExe, flashPlugin);
					}
				}
				else if (launchInfo.PlayerNativeLaunch())
				{
					FileInfo playerExe = m_debuggerCallbacks.getPlayerExe();
					return new StandalonePlayer(playerExe);
				}
			}
			
			return null;
		}
		
		/*
		* @see Flash.Tools.Debugger.SessionManager#supportsLaunch()
		*/
        public override bool supportsLaunch()
		{
			return true;
		}
		
		/*
		* @see Flash.Tools.Debugger.SessionManager#accept(Flash.Tools.Debugger.IProgress)
		*/
        public override Session accept(IProgress waitReporter)
		{
			bool isAIRapp = false; // we don't know whether we're waiting for an AIR app
			return accept(waitReporter, isAIRapp);
		}
		
		/// <summary> A private variation on <code>accept()</code> that also has an argument
		/// indicating that the process we are waiting for is an AIR application. If
		/// it is, then we can sometimes give slightly better error messages (see bug
		/// FB-7544).
		/// 
		/// </summary>
		/// <param name="isAIRapp">if <code>true</code>, then the process we are waiting for
		/// is an AIR application. This is only used to give better error
		/// messages in the event that we can't establish a connection to
		/// that process.
		/// </param>
		private Session accept(IProgress waitReporter, bool isAIRapp)
		{
			// get timeout 
			int timeout = getPreference(SessionManager.PREF_ACCEPT_TIMEOUT);
			int totalTimeout = timeout;
			int iterateOn = 100;
			
			PlayerSession session = null;
			try
			{
				m_processDead = false;
                m_serverSocket.Server.ReceiveTimeout = iterateOn;
				
				// Wait 100ms per iteration.  We have to do that so that we can report how long
				// we have been waiting.
				System.Net.Sockets.TcpClient s = null;
				while (s == null && !m_processDead)
				{
					try
					{
                        if (m_serverSocket.Pending())
                        {
                            s = m_serverSocket.AcceptTcpClient();
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(iterateOn);
                            timeout -= iterateOn;
                            if (timeout < 0) throw new IOException();
                        }
                    }
                    catch (IOException ste)
                    {
                        timeout -= iterateOn;
                        if (timeout < 0 || m_serverSocket == null || !m_serverSocket.Server.Connected)
                            throw ste; // we reached the timeout, or someome called stopListening()
                    }

					// Tell the progress monitor we've waited a little while longer,
					// so that the Eclipse progress bar can keep chugging along
					if (waitReporter != null)
						waitReporter.setProgress(totalTimeout - timeout, totalTimeout);
				}
				
				if (s == null && m_processDead)
				{
					IOException e = null;
					String detailMessage = LocalizationManager.getLocalizedTextString("processTerminatedWithoutDebuggerConnection"); //$NON-NLS-1$
					
					if (m_processMessages != null)
					{
						String commandLineMessage = m_processMessages.ToString();
						if (commandLineMessage.Length > 0)
							e = new CommandLineException(detailMessage, m_launchCommand, commandLineMessage, m_processExitValue);
					}
					
					if (e == null)
					{
						if (isAIRapp)
						{
							// For bug FB-7544: give the user a hint about what might have gone wrong.
							detailMessage += s_newline;
							detailMessage += LocalizationManager.getLocalizedTextString("maybeAlreadyRunning"); //$NON-NLS-1$
						}
						
						e = new IOException(detailMessage);
					}
					
					throw e;
				}

				/* create a new session around this socket */
				session = PlayerSession.createFromSocket(s);
				
				// transfer preferences 
				session.Preferences = m_prefs;
			}
			catch (NullReferenceException)
			{
				throw new SocketException(); //$NON-NLS-1$
			}
			finally
			{
				m_processMessages = null;
				m_launchCommand = null;
			}
			
			return session;
		}
		
		/*
		* @see Flash.Tools.Debugger.SessionManager#getDebuggerCallbacks()
		*/
		public virtual IDebuggerCallbacks getDebuggerCallbacks()
		{
			return m_debuggerCallbacks;
		}
		
		/*
		* @see Flash.Tools.Debugger.SessionManager#setDebuggerCallbacks(Flash.Tools.Debugger.IDebuggerCallbacks)
		*/
        public override void setDebuggerCallbacks(IDebuggerCallbacks debuggerCallbacks)
		{
			m_debuggerCallbacks = debuggerCallbacks;
		}
		static PlayerSessionManager()
		{
			{
				// set up for localizing messages
				m_localizationManager = new LocalizationManager();
				m_localizationManager.addLocalizer(new DebuggerLocalizer("Flash.Tools.Debugger.Concrete.djapi.")); //$NON-NLS-1$
			}
		}
	}
}