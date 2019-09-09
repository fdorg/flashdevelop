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
using System.Diagnostics;
using Flash.Tools.Debugger.Events;
using IntMap = Flash.Util.IntMap;
using Trace = Flash.Util.Trace;

namespace Flash.Tools.Debugger.Concrete
{
	/// <summary> Implements the receiving and updating of debug state from the socket connection
	/// of the Flash Player.
	/// </summary>
	public class DManager : DProtocolNotifierIF, SourceLocator
	{
		virtual public String URI
		{
			get
			{
				return m_uri;
			}
			
		}
		virtual public byte[] SWF
		{
			get
			{
				return m_swf;
			}
			
		}
		virtual public byte[] SWD
		{
			get
			{
				return m_swd;
			}
			
		}
		virtual public byte[] Actions
		{
			get
			{
				return m_actions;
			}
			
		}
		/// <summary>Returns the Flash Player version number; e.g. 9 for Flash Player 9.0 </summary>
		virtual public int Version
		{
			get
			{
				return m_playerVersion;
			}
			
		}
		virtual public SourceLocator SourceLocator
		{
			get
			{
				return m_sourceLocator;
			}
			
			set
			{
				m_sourceLocator = value;
			}
			
		}
		virtual public DSwfInfo[] SwfInfos
		{
			// Simple DSwfInfo getters
			
			get
			{
                lock (m_swfInfo.SyncRoot)
                {
                    return (DSwfInfo[])m_swfInfo.ToArray(typeof(DSwfInfo));
                }
			}
			
		}
		virtual public int SwfInfoCount
		{
			get
			{
				lock (m_swfInfo.SyncRoot)
				{
					return m_swfInfo.Count;
				}
			}
			
		}
		/// <summary> Get the most recently active swfInfo object.
		/// We define active as the most recently seen swfInfo
		/// </summary>
		virtual internal DSwfInfo ActiveSwfInfo
		{
			get
			{
				int count = SwfInfoCount;
				
				// pick up the last one seen
				DSwfInfo swf = m_lastSwfInfo;
				
				// still don't have one then get or create the most recent one
				// works if count = 0
				if (swf == null)
					swf = getOrCreateSwfInfo(count - 1);
				
				if (swf.hasAllSource())
				{
					// already full so create a new one on the end
					swf = getOrCreateSwfInfo(count);
				}
				return swf;
			}
			
		}
		virtual public DModule[] Sources
		{
			// @deprecated
			
			get
			{
				lock (m_source)
				{
					m_sourceListModified = false;
					
					/* find out the size of the array */
					DModule[] ar = new DModule[m_source.size()];
					
					int count = 0;

                    foreach (IntMap.IntMapEntry entry in m_source)
                    {
						ar[count++] = (DModule) entry.Value;
					}
					return ar;
				}
			}
			
		}
		virtual public String[] ConstantPool
		{
			//  @deprecated last pool that was read
			
			get
			{
				return m_lastConstantPool;
			}
			
		}
		virtual public DLocation[] Breakpoints
		{
			get
			{
				lock (m_breakpoints.SyncRoot)
				{
                    return (DLocation[])m_breakpoints.ToArray(typeof(DLocation));
				}
			}
			
		}
		virtual public int WatchpointCount
		{
			get
			{
				lock (m_watchpoints.SyncRoot)
				{
					return m_watchpoints.Count;
				}
			}
			
		}
		virtual public DWatch[] Watchpoints
		{
			get
			{
				lock (m_watchpoints.SyncRoot)
				{
                    return (DWatch[])m_watchpoints.ToArray(typeof(DWatch));
				}
			}
			
		}
		virtual public int FrameCount
		{
			get
			{
				return m_frames.Count;
			}
			
		}
		virtual public DStackContext[] Frames
		{
			get
			{
                return (DStackContext[])m_frames.ToArray(typeof(DStackContext));
			}
		}

        /// <summary> Get function is only supported in players that
		/// recognize the squelch message.
		/// </summary>
		virtual public bool GetSupported
		{
			get
			{
				return m_squelchEnabled;
			}
			
		}
		/// <summary> Returns a suspend information on
		/// why the Player has suspended execution.
		/// </summary>
		/// <returns> see SuspendReason
		/// </returns>
		virtual public DSuspendInfo SuspendInfo
		{
			get
			{
				return m_suspendInfo;
			}
			
		}
		/// <summary> Event management related stuff</summary>
		virtual public int EventCount
		{
			get
			{
				lock (m_event.SyncRoot)
				{
					return m_event.Count;
				}
			}
			
		}
		/// <summary> Get an object on which callers can call wait(), in order to wait until
		/// something happens.
		/// 
		/// Note: The object will be signalle when EITHER of the following happens:
		/// (1) An event is added to the event queue;
		/// (2) The network connection is broken (and thus there will be no more events).
		/// 
		/// </summary>
		/// <returns> an object on which the caller can call wait()
		/// </returns>
		virtual public Object EventNotifier
		{
			get
			{
				return m_event;
			}
			
		}
		virtual public int ChangeCount
		{
			/* (non-Javadoc)
			* @see Flash.Tools.Debugger.SourceLocator#getChangeCount()
			*/
			
			get
			{
				if (m_sourceLocator != null)
					return m_sourceLocator.ChangeCount;
				
				return 0;
			}
			
		}
		private System.Collections.Hashtable m_parms;
		private IntMap m_source; /* WARNING: accessed from multiple threads */
		private System.Collections.ArrayList m_breakpoints; /* WARNING: accessed from multiple threads */
		private System.Collections.ArrayList m_swfInfo; /* WARNING: accessed from multiple threads */
		private System.Collections.ArrayList m_watchpoints; /* WARNING: accessed from multiple threads */
		
		/// <summary> The currently active stack frames.</summary>
		private System.Collections.ArrayList m_frames;
		
		/// <summary> The stack frames that were active the last time the player was suspended.</summary>
		private System.Collections.ArrayList m_previousFrames;
		
		/// <summary> A list of all known variables in the player.  Stored as a mapping
		/// from an object's id to its DValue.
		/// </summary>
		private IntMap m_values;
		
		/// <summary> A list of all known variables in the player from the previous time
		/// the player was suspended.  Stored as a mapping from an object's id
		/// to its DValue.
		/// </summary>
		private IntMap m_previousValues;
		
		private System.Collections.ArrayList m_event; /* our event queue; WARNING: accessed from multiple threads */
		private DSuspendInfo m_suspendInfo; /* info for when we are stopped */
		private SourceLocator m_sourceLocator;
		
		private DSwfInfo m_lastSwfInfo; /* hack for syncing swfinfo records with incoming InScript messages */
		private DVariable m_lastInGetVariable; /* hack for getVariable call to work with getters */
		private bool m_attachChildren; /* hack for getVariable call to work with getters */
		private bool m_squelchEnabled; /* true if we are talking to a squelch enabled debug player */
		private int m_playerVersion; /* player version number obtained from InVersion message; e.g. 9 for Flash Player 9.0 */
		
		private bool m_sourceListModified; /* deprecated; indicates m_source has changed since last
		* call to getSource().
		* WARNING: lock with synchronized (m_source) { ... }
		*/
		private byte[] m_actions; /* deprecated */
		private String[] m_lastConstantPool; /* deprecated */
		
		// SWF/SWD fetching and parsing
		private String m_uri;
		private byte[] m_swf; // latest swf obtained from get swf
		private byte[] m_swd; // latest swd obtained from get swd
		
		private bool m_inGetterSetter;
		private FaultEvent m_faultEventDuringGetterSetter;
		
		private System.Collections.IDictionary m_options = new System.Collections.Hashtable(); // Player options that have been queried by OutGetOption, and come back via InOption
		
		public const String ARGUMENTS_MARKER = "$arguments"; //$NON-NLS-1$
		public const String SCOPE_CHAIN_MARKER = "$scopechain"; //$NON-NLS-1$
		
		public DManager()
		{
			m_parms = new System.Collections.Hashtable();
			m_source = new IntMap();
			m_breakpoints = new System.Collections.ArrayList();
			m_values = new IntMap();
			m_previousValues = new IntMap();
			m_frames = new System.Collections.ArrayList();
			m_previousFrames = new System.Collections.ArrayList();
			m_swfInfo = new System.Collections.ArrayList();
			m_watchpoints = new System.Collections.ArrayList();
			m_event = new System.Collections.ArrayList();
			m_suspendInfo = null;
			m_sourceLocator = null;
			
			m_lastInGetVariable = null;
			m_attachChildren = true;
			m_squelchEnabled = false;
			m_lastConstantPool = null;
			m_playerVersion = - 1; // -1 => unknown
		}
		
		/// <summary> If this feature is enabled then we do not attempt to attach
		/// child variables to parents.
		/// </summary>
		public virtual void  enableChildAttach(bool enable)
		{
			m_attachChildren = enable;
		}
		
		// return/clear the last variable seen from an InGetVariable message
		public virtual DVariable lastVariable()
		{
			return m_lastInGetVariable;
		}
		public virtual void  clearLastVariable()
		{
			m_lastInGetVariable = null;
		}
		
		/*
		* Frees up any information we have kept about
		*/
		internal virtual void  freeCaches()
		{
			clearFrames();
			freeValueCache();
		}
		
		internal virtual void  freeValueCache()
		{
			m_previousValues = m_values;
			m_values = new IntMap();
			
			// mark all frames as stale
			int size = FrameCount;
			for (int i = 0; i < size; i++)
				getFrame(i).markStale();
		}
		
		// continuing our execution
		internal virtual void  continuing()
		{
			freeCaches();
			m_suspendInfo = null;
		}
		
		/// <summary> Variables</summary>
		internal virtual DValue getOrCreateValue(long id)
		{
			DValue v = getValue(id);
			if (v == null)
			{
				v = new DValue(id);
				putValue(id, v);
			}
			return v;
		}
		
		public virtual DSwfInfo getSwfInfo(int at)
		{
			lock (m_swfInfo.SyncRoot)
			{
				return (DSwfInfo) m_swfInfo[at];
			}
		}
		
		/// <summary> Obtains a SwfInfo object at the given index or if one
		/// doesn't yet exist at that location, creates a new empty
		/// one there and returns it.
		/// </summary>
		internal virtual DSwfInfo getOrCreateSwfInfo(int at)
		{
			lock (m_swfInfo.SyncRoot)
			{
				DSwfInfo i = (at > - 1 && at < SwfInfoCount)?getSwfInfo(at):null;
				if (i == null)
				{
					// are we above water
					at = (at < 0)?0:at;
					
					// fill all the gaps with null; really shouldn't be any...
					while (at > m_swfInfo.Count)
						m_swfInfo.Add(null);
					
					i = new DSwfInfo(at);
					m_swfInfo.Insert(at, i);
				}
				return i;
			}
		}
		
		/// <summary> Walk the list of scripts and add them to our swfInfo object
		/// This method may be called when min/max are zero and the swd
		/// has not yet fully loaded in the player or it could be called
		/// before we have all the scripts.
		/// </summary>
		internal virtual void  tieScriptsToSwf(DSwfInfo info)
		{
			if (!info.hasAllSource())
			{
				int min = info.FirstSourceId;
				int max = info.LastSourceId;
				//			System.out.println("attaching scripts "+min+"-"+max+" to "+info.getUrl());
				for (int i = min; i <= max; i++)
				{
					DModule m = getSource(i);
					if (m == null)
					{
						// this is ok, it means the scripts are coming...
					}
					else
					{
						info.addSource(i, m);
					}
				}
			}
		}
		
		/// <summary> Record a new source file.</summary>
		/// <param name="name">filename in "basepath;package;filename" format
		/// </param>
		/// <param name="swfIndex">the index of the SWF with which this source is associated,
		/// or -1 is we don't know
		/// </param>
		/// <returns> true if our list of source files was modified, or false if we
		/// already knew about that particular source file.
		/// </returns>
		private bool putSource(int swfIndex, int moduleId, int bitmap, String name, String text)
		{
			lock (m_source)
			{
				// if we haven't already recorded this script then do so.
				if (!m_source.contains(moduleId))
				{
					DModule s = new DModule(this, moduleId, bitmap, name, text);
					
					// put source in our large pool
					m_source.put(moduleId, s);
					
					// put the source in the currently active swf
					DSwfInfo swf;
					if (swfIndex == - 1)
					// caller didn't tell us what swf thi is for
						swf = ActiveSwfInfo;
					// ... so guess
					else
						swf = getOrCreateSwfInfo(swfIndex);
					
					swf.addSource(moduleId, s);
					
					return true;
				}
				
				return false;
			}
		}
		
		/// <summary> Remove our record of a particular source file.</summary>
		/// <param name="id">the id of the file to forget about.
		/// </param>
		/// <returns> true if source file was removed; false if we didn't know about
		/// it to begin with.
		/// </returns>
		private bool removeSource(int id)
		{
			lock (m_source)
			{
				try
				{
					m_source.remove(id);
				}
				catch (Exception)
				{
					return false;
				}
				return true;
			}
		}
		
		public virtual DModule getSource(int id)
		{
			lock (m_source)
			{
				return (DModule) m_source.get_Renamed(id);
			}
		}
		
		// @deprecated
		internal virtual bool sourceListModified()
		{
			lock (m_source)
			{
				return m_sourceListModified;
			}
		}
		
		public virtual DValue getValue(long id)
		{
			DValue v = (DValue) m_values.get_Renamed((int) id);
			return v;
		}
		
		/// <summary> Returns the previous value object for the given id -- that is, the value that that
		/// object had the last time the player was suspended.  Never requests it from the
		/// player (because it can't, of course).  Returns <code>null</code> if we don't have
		/// a value for that id.
		/// </summary>
		public virtual DValue getPreviousValue(long id)
		{
			return (DValue) m_previousValues.get_Renamed((int) id);
		}
		
		internal virtual void  putValue(long id, DValue v)
		{
			if (id != Value.UNKNOWN_ID)
			{
				m_values.put((int) id, v);
			}
		}
		
		internal virtual DValue removeValue(long id)
		{
			return (DValue) m_values.remove((int) id);
		}
		
		internal virtual void  addVariableMember(long parentId, DVariable child)
		{
			DValue parent = getValue(parentId);
			addVariableMember(parent, child);
		}
		
		internal virtual void  addVariableMember(DValue parent, DVariable child)
		{
			if (m_attachChildren)
			{
				// There are certain situations when the Flash player will send us more
				// than one variable or getter with the same name.  Basically, when a
				// subclass implements (or overrides) something that was also declared in a
				// superclass, then we'll see that variable or getter in both the
				// superclass and the subclass.
				//
				// Here are a few situations where that affects the debugger in different
				// ways:
				//
				// 1. When a class implements an interface, the class instance actually has
				//    *two* members for each implemented function: One which is public and
				//    represents the implementation function, and another which is internal
				//    to the interface, and represents the declaration of the function.
				//    Both of these come in to us.  In the UI, the one we want to show is
				//    the public one.  They come in in random order (they are stored in a
				//    hash table in the VM), so we don't know which one will come first.
				//
				// 2. When a superclass has a private member "m", and a subclass has its own
				//    private member with the same name "m", we will receive both of them.
				//    (They are scoped by different packages.)  In this case, the first one
				//    the player sent us is the one from the subclass, and that is the one
				//    we want to display in the debugger.
				//
				// The following logic correctly deals with all variations of those cases.
				DVariable existingChildWithSameName = parent.findMember(child.getName());
				if (existingChildWithSameName != null)
				{
					int existingScope = existingChildWithSameName.Scope;
					int newScope = child.Scope;
					
					if (existingScope == VariableAttribute.NAMESPACE_SCOPE && newScope == VariableAttribute.PUBLIC_SCOPE)
					{
						// This is the case described above where a class implements an interface,
						// so that class's definition includes both a namespace-scoped declaration
						// and a public declaration, in random order; in this case, the
						// namespace-scoped declaration came first.  We want to use the public
						// declaration.
						parent.addMember(child);
					}
					else if (existingScope == VariableAttribute.PUBLIC_SCOPE && newScope == VariableAttribute.NAMESPACE_SCOPE)
					{
						// One of two things happened here:
						//
						// 1. This is the case described above where a class implements an interface,
						//    so that class's definition includes both a namespace-scoped declaration
						//    and a public declaration, in random order; in this case, the
						//    public declaration came first.  It is tempting to use the public
						//    member in this case, but there is a catch...
						// 2. It might be more complicated than that: Perhaps there is interface I,
						//    and class C1 implements I, but class C2 extends C1, and overrides
						//    one of the members of I that was already implemented by C1.  In this
						//    case, the public declaration from C2 came first, but now we are seeing
						//    a namespace-scoped declaration in C1.  We need to record that the
						//    member is public, but we also need to record that it is a member
						//    of the base class, not just a member of the superclass.
						//
						// The easiest way to deal with both cases is to use the child that came from
						// the superclass, but to change its scope to public.
						child.makePublic();
						parent.addMember(child);
					}
					else if (existingScope != VariableAttribute.PRIVATE_SCOPE && existingScope == newScope)
					{
						// This is a public, protected, internal, or namespace-scoped member which
						// was defined in a base class and overridden in a subclass.  We want to
						// use the member from the base class, to that the debugger knows where the
						// variable was actually defined.
						parent.addMember(child);
					}
				}
				else
				{
					parent.addMember(child);
				}
				
				// put child in the registry if it has an id and not already there
				DValue childValue = (DValue) child.getValue();
				int childId = childValue.Id;
				if (childId != Value.UNKNOWN_ID)
				{
					DValue existingValue = getValue(childId);
					if (existingValue != null)
					{
						Debug.Assert(existingValue == childValue); // TODO is this right? what about getters?
					}
					else
					{
						putValue(childId, childValue);
					}
				}
			}
		}
		
		// TODO is this right?
		internal virtual void  addVariableMember(long parentId, DVariable child, long doubleAsId)
		{
			addVariableMember(parentId, child);
			
			// double book the child under another id
			if (m_attachChildren)
				putValue(doubleAsId, (DValue) child.getValue());
		}
		
		/// <summary> Breakpoints</summary>
		public virtual DLocation getBreakpoint(int id)
		{
			lock (m_breakpoints.SyncRoot)
			{
				DLocation loc = null;
				int which = findBreakpoint(id);
				if (which > - 1)
					loc = (DLocation) m_breakpoints[which];
				return loc;
			}
		}
		
		internal virtual int findBreakpoint(int id)
		{
			lock (m_breakpoints.SyncRoot)
			{
				int which = - 1;
				int size = m_breakpoints.Count;
				for (int i = 0; which < 0 && i < size; i++)
				{
					DLocation l = (DLocation) m_breakpoints[i];
					if (l.Id == id)
						which = i;
				}
				return which;
			}
		}
		
		internal virtual DLocation removeBreakpoint(int id)
		{
			lock (m_breakpoints.SyncRoot)
			{
				DLocation loc = null;
				int which = findBreakpoint(id);
				if (which > - 1)
				{
					loc = (DLocation) m_breakpoints[which];
					m_breakpoints.RemoveAt(which);
				}
				
				return loc;
			}
		}
		
		internal virtual void  addBreakpoint(int id, DLocation l)
		{
			lock (m_breakpoints.SyncRoot)
			{
				m_breakpoints.Add(l);
			}
		}
		
		/// <summary> Watchpoints</summary>
		public virtual DWatch getWatchpoint(int at)
		{
			lock (m_watchpoints.SyncRoot)
			{
				return (DWatch) m_watchpoints[at];
			}
		}
		
		internal virtual bool addWatchpoint(DWatch w)
		{
			lock (m_watchpoints.SyncRoot)
			{
				return m_watchpoints.Add(w) >= 0;
			}
		}
		
		internal virtual DWatch removeWatchpoint(int tag)
		{
			lock (m_watchpoints.SyncRoot)
			{
				DWatch w = null;
				int at = findWatchpoint(tag);
				if (at > - 1)
				{
					Object tempObject;
					tempObject = m_watchpoints[at];
					m_watchpoints.RemoveAt(at);
					w = (DWatch) tempObject;
				}
				return w;
			}
		}
		
		internal virtual int findWatchpoint(int tag)
		{
			lock (m_watchpoints.SyncRoot)
			{
				int at = - 1;
				int size = WatchpointCount;
				for (int i = 0; i < size && at < 0; i++)
				{
					DWatch w = getWatchpoint(i);
					if (w.Tag == tag)
						at = i;
				}
				return at;
			}
		}
		
		/// <summary> Frame stack management related stuff</summary>
		/// <returns> true if we added this frame; false if we ignored it
		/// </returns>
		internal virtual bool addFrame(DStackContext ds)
		{
			m_frames.Add(ds);
			return true;
		}
		
		internal virtual void  clearFrames()
		{
			if (m_frames.Count > 0)
				m_previousFrames = m_frames;
			m_frames = new System.Collections.ArrayList();
		}
		
		public virtual DStackContext getFrame(int at)
		{
			return (DStackContext) m_frames[at];
		}
		
		private bool stringsEqual(String s1, String s2)
		{
			if (s1 == null)
				return s2 == null;
			else
				return s1.Equals(s2);
		}
		
		/// <summary> Correlates the old list of stack frames, from the last time the player
		/// was suspended, with the new list of stack frames, attempting to guess
		/// which frames correspond to each other.  This is done so that
		/// Variable.hasValueChanged() can work correctly for local variables.
		/// </summary>
		private void  mapOldFramesToNew()
		{
			int oldSize = m_previousFrames.Count;
			int newSize = m_frames.Count;
			
			// discard all old frames (we will restore some of them below)
			DValue[] oldFrames = new DValue[oldSize];
			for (int depth = 0; depth < oldSize; depth++)
			{
				oldFrames[depth] = (DValue) m_previousValues.remove(Value.BASE_ID - depth);
			}
			
			// Start at the end of the stack (the stack frame farthest from the
			// current one), and try to match up stack frames
			int oldDepth = oldSize - 1;
			int newDepth = newSize - 1;
			while (oldDepth >= 0 && newDepth >= 0)
			{
				DStackContext oldFrame = (DStackContext) m_previousFrames[oldDepth];
				DStackContext newFrame = (DStackContext) m_frames[newDepth];
				if (oldFrame != null && newFrame != null)
				{
					if (stringsEqual(oldFrame.CallSignature, newFrame.CallSignature))
					{
						DValue frame = oldFrames[oldDepth];
						if (frame != null)
							m_previousValues.put(Value.BASE_ID - newDepth, frame);
					}
				}
				oldDepth--;
				newDepth--;
			}
		}
		
		public virtual DebugEvent nextEvent()
		{
			DebugEvent s = null;
			lock (m_event.SyncRoot)
			{
				if (m_event.Count > 0)
				{
					Object tempObject;
					tempObject = m_event[0];
					m_event.RemoveAt(0);
					s = (DebugEvent) tempObject;
				}
			}
			return s;
		}
		
		public virtual void  addEvent(DebugEvent e)
		{
			lock (this)
			{
				lock (m_event.SyncRoot)
				{
					m_event.Add(e);
					System.Threading.Monitor.PulseAll(m_event.SyncRoot); // wake up listeners (see getEventNotifier())
				}
			}
		}
		
		/// <summary> Attach this manager to listen to the socket.</summary>
		public virtual void  attach(DProtocol d)
		{
			d.addListener(this);
		}
		
		public virtual void  release(DProtocol d)
		{
			d.removeListener(this);
		}
		
		/// <summary> Issued when the socket connection to the player is cut</summary>
		public virtual void  disconnected()
		{
			lock (m_event.SyncRoot)
			{
				System.Threading.Monitor.PulseAll(m_event.SyncRoot); // see getEventNotifier()
			}
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
			/* at this point we just open up a big switch statement and walk through all possible cases */
			int type = msg.Type;
			//		System.out.println("manager msg = "+DMessage.inTypeName(type));
			
			switch (type)
			{
				
				case DMessage.InVersion: 
				{
                    long ver = msg.getDWord();
					m_playerVersion = (int) ver;
					break;
				}
				
				
				case DMessage.InErrorExecLimit: 
				{
					handleFaultEvent(new RecursionLimitFault());
					break;
				}
				
				
				case DMessage.InErrorWith: 
				{
					handleFaultEvent(new InvalidWithFault());
					break;
				}
				
				
				case DMessage.InErrorProtoLimit: 
				{
					handleFaultEvent(new ProtoLimitFault());
					break;
				}
				
				
				case DMessage.InErrorURLOpen: 
				{
                    String url = msg.getString();
					handleFaultEvent(new InvalidURLFault(url));
					break;
				}
				
				
				case DMessage.InErrorTarget: 
				{
                    String name = msg.getString();
					handleFaultEvent(new InvalidTargetFault(name));
					break;
				}
				
				
				case DMessage.InErrorException: 
				{
                    long offset = msg.getDWord();
					// As of FP9, the player will also send the "toString()" message
					// of the exception.  But for backward compatibility with older
					// players, we won't assume that that is there.
					String exceptionMessage;
					if (msg.Remaining > 0)
                        exceptionMessage = msg.getString();
					else
						exceptionMessage = ""; //$NON-NLS-1$
					handleFaultEvent(new ExceptionFault(exceptionMessage));
					break;
				}
				
				
				case DMessage.InErrorStackUnderflow: 
				{
                    long offset = msg.getDWord();
					handleFaultEvent(new StackUnderFlowFault());
					break;
				}
				
				
				case DMessage.InErrorZeroDivide: 
				{
                    long offset = msg.getDWord();
					handleFaultEvent(new DivideByZeroFault());
					break;
				}
				
				
				case DMessage.InErrorScriptStuck: 
				{
					handleFaultEvent(new ScriptTimeoutFault());
					break;
				}
				
				
				case DMessage.InErrorConsole: 
				{
                    String s = msg.getString();
					handleFaultEvent(new ConsoleErrorFault(s));
					break;
				}
				
				
				case DMessage.InTrace: 
				{
                    String text = msg.getString();
					addEvent(new TraceEvent(text));
					break;
				}
				
				
				case DMessage.InSquelch: 
				{
                    long state = msg.getDWord();
					m_squelchEnabled = (state != 0)?true:false;
					break;
				}
				
				
				case DMessage.InParam: 
				{
                    String name = msg.getString();
                    String value = msg.getString();
					
					// here's where we get movie = URL and password which I'm not sure what to do with?
					//				System.out.println(name+"="+value);
					m_parms[name] = value;
					
					// if string is a "movie", then this is a URL
					if (name.StartsWith("movie"))
					//$NON-NLS-1$
						m_uri = convertToURI(value);
					break;
				}
				
				
				case DMessage.InPlaceObject: 
				{
                    long objId = msg.getDWord();
					String path = msg.getString();
					//				m_bag.placeObject((int)objId, path);
					break;
				}
				
				
				case DMessage.InSetProperty: 
				{
                    long objId = msg.getDWord();
					int item = msg.getWord();
                    String value = msg.getString();
					break;
				}
				
				
				case DMessage.InNewObject: 
				{
                    long objId = msg.getDWord();
					break;
				}
				
				
				case DMessage.InRemoveObject: 
				{
                    long objId = msg.getDWord();
					//				m_bag.removeObject((int)objId);
					break;
				}
				
				
				case DMessage.InSetVariable: 
				{
                    long objId = msg.getDWord();
                    String name = msg.getString();
					int dType = msg.getWord();
                    int flags = (int)msg.getDWord();
                    String value = msg.getString();
					
					//				m_bag.createVariable((int)objId, name, dType, flags, value);
					break;
				}
				
				
				case DMessage.InDeleteVariable: 
				{
                    long objId = msg.getDWord();
                    String name = msg.getString();
					//				m_bag.deleteVariable((int)objId, name);
					break;
				}
				
				
				case DMessage.InScript: 
				{
                    int module = (int)msg.getDWord();
                    int bitmap = (int)msg.getDWord();
                    String name = msg.getString(); // in "basepath;package;filename" format
                    String text = msg.getString();
					int swfIndex = - 1;
					
					/* new in flash player 9: player tells us what swf this is for */
					if (msg.Remaining >= 4)
                        swfIndex = (int)msg.getDWord();
					
					lock (m_source)
					{
						// create new source file
						if (putSource(swfIndex, module, bitmap, name, text))
						{
							// have we changed the list since last query
							if (!m_sourceListModified)
								addEvent(new FileListModifiedEvent());
							
							m_sourceListModified = true; /* current source list is stale */
						}
					}
					break;
				}
				
				
				case DMessage.InRemoveScript: 
				{
                    long module = msg.getDWord();
					lock (m_source)
					{
						if (removeSource((int) module))
						{
							// have we changed the list since last query
							if (!m_sourceListModified)
								addEvent(new FileListModifiedEvent());
							
							m_sourceListModified = true; /* current source list is stale */
						}
					}
					break;
				}
				
				
				case DMessage.InAskBreakpoints: 
				{
					// the player has just loaded a swf and we know the player
					// has halted, waiting for us to continue.  The only caveat
					// is that it looks like it still does a number of things in
					// the background which take a few seconds to complete.
					if (m_suspendInfo == null)
						m_suspendInfo = new DSuspendInfo(SuspendReason.ScriptLoaded, 0, 0, 0, 0);
					break;
				}
				
				
				case DMessage.InBreakAt: 
				{
                    long bp = msg.getDWord();
                    long id = msg.getDWord();
                    String stack = msg.getString();
					//				System.out.println(msg.getInTypeName()+",bp="+(bp&0xffff)+":"+(bp>>16)+",id="+id+",stack=\n"+stack);
					
					//System.out.println("InBreakAt");
					
					int module = DLocation.decodeFile(bp);
					int line = DLocation.decodeLine(bp);
					addEvent(new BreakEvent(module, line));
					break;
				}
				
				
				case DMessage.InContinue: 
				{
					/* we are running again so trash all our variable contents */
					continuing();
					break;
				}
				
				
				case DMessage.InSetLocalVariables: 
				{
                    long objId = msg.getDWord();
					//				m_bag.markObjectLocal((int)objId, true);
					break;
				}
				
				
				case DMessage.InSetBreakpoint: 
				{
                    long count = msg.getDWord();
					while (count-- > 0)
					{
                        long bp = msg.getDWord();
						
						int fileId = DLocation.decodeFile(bp);
						int line = DLocation.decodeLine(bp);
						
						DModule file = getSource(fileId);
						DLocation l = new DLocation(file, line);
						
						if (file != null)
							addBreakpoint((int) bp, l);
					}
					break;
				}
				
				
				case DMessage.InNumScript: 
				{
					/* lets us know how many scripts there are */
                    int num = (int)msg.getDWord();
					DSwfInfo swf;
					
					/*
					* New as of flash player 9: another dword indicating which swf this is for.
					* That means we don't have to guess whether this is for an old SWF
					* which has just had some more modules loaded, or for a new SWF!
					*/
					if (msg.Remaining >= 4)
					{
                        int swfIndex = (int)msg.getDWord();
						swf = getOrCreateSwfInfo(swfIndex);
						m_lastSwfInfo = swf;
					}
					else
					{
						/* This is not flash player 9 (or it is an early build of fp9).
						*
						* We use this message as a trigger that a new swf has been loaded, so make sure
						* we are ready to accept the scripts.
						*/
						swf = ActiveSwfInfo;
					}
					
					// It is NOT an error for the player to have sent us a new,
					// different sourceExpectedCount from whatever we had before!
					// In fact, this happens all the time, whenever a SWF has more
					// than one ABC.
					swf.SourceExpectedCount = num;
					break;
				}
				
				
				case DMessage.InRemoveBreakpoint: 
				{
                    long count = msg.getDWord();
					while (count-- > 0)
					{
                        long bp = msg.getDWord();
						removeBreakpoint((int) bp);
					}
					break;
				}
				
				
				case DMessage.InBreakAtExt: 
				{
                    long bp = msg.getDWord();
                    long num = msg.getDWord();
					
					//				System.out.println(msg.getInTypeName()+",bp="+(bp&0xffff)+":"+(bp>>16));
					/* we have stack info to store away */
					clearFrames(); // just in case
					int depth = 0;
					while (num-- > 0)
					{
                        long bpi = msg.getDWord();
                        long id = msg.getDWord();
                        String stack = msg.getString();
						int module = DLocation.decodeFile(bpi);
						int line = DLocation.decodeLine(bpi);
						DModule m = getSource(module);
						DStackContext c = new DStackContext(module, line, m, (int) id, stack, depth);
						// If addFrame() returns false, that means it chose to ignore this
						// frame, so we do NOT want to increment our depth for the next
						// time through the loop.  If it returns true, then we do want to.
						if (addFrame(c))
							++depth;
						//					System.out.println("   this="+id+",@"+(bpi&0xffff)+":"+(bpi>>16)+",stack="+stack);
					}
					mapOldFramesToNew();
					break;
				}
				
				
				case DMessage.InFrame: 
				{
					// For InFrame the first element is really our frame id
					DValue frame = null;
					DVariable child = null;
					System.Collections.ArrayList v = new System.Collections.ArrayList();
					System.Collections.ArrayList registers = new System.Collections.ArrayList();

                    int depth = (int)msg.getDWord(); // depth of frame
					
					// make sure we have a valid depth
					if (depth > - 1)
					{
						// first thing is number of registers
                        int num = (int)msg.getDWord();
						for (int i = 0; i < num; i++)
							registers.Add(extractRegister(msg, i + 1));
					}
					
					int currentArg = - 1;
					bool gettingScopeChain = false;
					
					// then our frame itself
					while (msg.Remaining > 0)
					{
                        long frameId = msg.getDWord();
						
						if (frame == null)
						{
							frame = getOrCreateValue(frameId);
							extractVariable(msg); // put the rest of the info in the trash
						}
						else
						{
							child = extractVariable(msg);
							if (currentArg == - 1 && child.getName().Equals(ARGUMENTS_MARKER))
							{
								currentArg = 0;
								gettingScopeChain = false;
							}
							else if (child.getName().Equals(SCOPE_CHAIN_MARKER))
							{
								currentArg = - 1;
								gettingScopeChain = true;
							}
							else if (currentArg >= 0)
							{
								// work around a compiler bug: If the variable's name is "undefined",
								// then change its name to "_argN", where "N" is the argument index,
								// e.g. _arg1, _arg2, etc.
								++currentArg;
								if (child.getName().Equals("undefined"))
								//$NON-NLS-1$
									child.setName("_arg" + currentArg); //$NON-NLS-1$
							}
							
							// All args and locals get added as "children" of
							// the frame; but scope chain entries do not.
							if (!gettingScopeChain)
								addVariableMember(frameId, child);
							
							// Everything gets added to the ordered list of
							// variables that came in.
							v.Add(child);
						}
					}
					
					// let's transfer our newly gained knowledge into the stack context
					if (depth == 0)
						populateRootNode(frame, v);
					else
						populateFrame(depth, v);
					
					break;
				}
				
				
				case DMessage.InOption: 
				{
                    String s = msg.getString();
                    String v = msg.getString();
					m_options[s] = v;
					break;
				}
				
				
				case DMessage.InGetVariable: 
				{
					// For InGetVariable the first element is the original entity we requested
					DValue parent = null;
					DVariable child = null;
					String definingClass = null;
					int level = 0;
					int highestLevelWithMembers = - 1;
					System.Collections.IList classes = new System.Collections.ArrayList();
					
					while (msg.Remaining > 0)
					{
                        long parentId = msg.getDWord();
						
						// build or get parent node
						if (parent == null)
						{
                            String name = msg.getString();
							
							// pull the contents of the node which normally are disposed of except if we did a 0,name call
							m_lastInGetVariable = extractVariable(msg, name);
							
							parent = getOrCreateValue(parentId);
						}
						else
						{
							// extract the child and add it to the parent.
							child = extractVariable(msg);
							if (showMember(child))
							{
								if (child.isAttributeSet(VariableAttribute.IS_DYNAMIC))
								{
									// Dynamic attributes always come in marked as a member of
									// class "Object"; but to the user, it makes more sense to
									// consider them as members of the topmost class.
									if (classes.Count > 0)
									{
										child.setDefiningClass(0, (String) classes[0]);
										highestLevelWithMembers = Math.Max(highestLevelWithMembers, 0);
									}
								}
								else
								{
									child.setDefiningClass(level, definingClass);
									if (definingClass != null)
									{
										highestLevelWithMembers = Math.Max(highestLevelWithMembers, level);
									}
								}
								addVariableMember(parent.Id, child);
							}
							else
							{
								if (isTraits(child))
								{
									definingClass = child.QualifiedName;
									level = classes.Count;
									
									// If the traits name end with "$", then it represents a class object --
									// in other words, the variables inside it are static variables of that
									// class.  In that case, we need to juggle the information.  For example,
									// if we are told that a variable is a member of "MyClass$", we actually
									// store it into the information for "MyClass".
									if (definingClass.EndsWith("$"))
									{
										//$NON-NLS-1$
										String classWithoutDollar = definingClass.Substring(0, (definingClass.Length - 1) - (0));
										int indexOfClass = classes.IndexOf(classWithoutDollar);
										if (indexOfClass != - 1)
										{
											level = indexOfClass;
											definingClass = classWithoutDollar;
										}
									}
									
									// It wasn't static -- so, add this class to the end of the list of classes
									if (level == classes.Count)
									{
										classes.Add(definingClass);
									}
								}
							}
						}
					}
					
					if (parent != null && parent.getClassHierarchy(true) == null)
					{
                        String[] classesArray = new String[classes.Count];
                        int index = 0;

                        foreach (String className in classes)
                        {
                            classesArray[index++] = className;
                        }

						parent.setClassHierarchy(classesArray, highestLevelWithMembers + 1);
					}
					
					break;
				}
				
				
				case DMessage.InWatch: 
				// for AS2; sends 16-bit ID field
				case DMessage.InWatch2:  // for AS3; sends 32-bit ID field
					{
						// This message is sent whenever a watchpoint is added
						// modified or removed.
						//
						// For an addition, flags will be non-zero and
						// success will be true.
						//
						// For a modification flags  will be non-zero.
						// and oldFlags will be non-zero and success
						// will be true.  Additionally oldFlags will not
						// be equal to flags.
						//
						// For a removal flags will be zero.  oldFlags
						// will be non-zero.
						//
						// flags identifies the type of watchpoint added,
						// see WatchKind.
						//
						// success indicates whether the operation was successful
						//
						// request.   It will be associated with the watchpoint.
                        int success = msg.getWord();
                        int oldFlags = msg.getWord();
                        int oldTag = msg.getWord();
                        int flags = msg.getWord();
                        int tag = msg.getWord();
						// for AS2, the ID came in above as a Word.  For AS3, the above value is
						// bogus, and it has been sent again as a DWord.
                        int id = (int)((type == DMessage.InWatch2) ? msg.getDWord() : msg.getWord());
                        String name = msg.getString();
						
						if (success != 0)
						{
							if (flags == 0)
							{
								removeWatchpoint(oldTag);
							}
							else
							{
								// modification or addition is the same to us
								// a new watch is created and added into the table
								// while any old entry if it exists is removed.
								removeWatchpoint(oldTag);
								DWatch w = new DWatch(id, name, flags, tag);
								addWatchpoint(w);
							}
						}
						break;
					}
				
				
				case DMessage.InGetSwf: 
				{
					// we only house the swf temporarily, PlayerSession then
					// pieces it back into swfinfo record.  Also, we don't
					// send any extra data in the message so that we need not
					// copy the bytes.
					m_swf = msg.Data;
					break;
				}
				
				
				case DMessage.InGetSwd: 
				{
					// we only house the swd temporarily, PlayerSession then
					// pieces it back into swfinfo record.
					m_swd = msg.Data;
					break;
				}
				
				
				case DMessage.InBreakReason: 
					{
						// the id map 1-1 with out SuspendReason interface constants
                        int suspendReason = msg.getWord();
                        int suspendPlayer = msg.getWord(); // item index of player
                        int breakOffset = (int)msg.getDWord(); // current script offset
                        int prevBreakOffset = (int)msg.getDWord(); // prev script offset
                        int nextBreakOffset = (int)msg.getDWord(); // next script offset
						m_suspendInfo = new DSuspendInfo(suspendReason, suspendPlayer, breakOffset, prevBreakOffset, nextBreakOffset);
						
						// augment the current frame with this information.  It
						// should work ok since we only get this message after a
						// InBreakAtExt message
						try
						{
							DStackContext c = getFrame(0);
							c.Offset = breakOffset;
							c.SwfIndex = suspendPlayer;
						}
						catch (Exception e)
						{
							if (Trace.error)
							{
								Trace.trace("Oh my god, gag me with a spoon...getFrame(0) call failed"); //$NON-NLS-1$
                                Console.Error.Write(e.StackTrace);
                                Console.Error.Flush();
                            }
						}
						break;
					}
					
					// obtain raw action script byte codes
				
				case DMessage.InGetActions: 
					{
                        int item = msg.getWord();
                        int rsvd = msg.getWord();
                        int at = (int)msg.getDWord();
                        int len = (int)msg.getDWord();
						int i = 0;
						
						m_actions = (len <= 0)?null:new byte[len];
						while (len-- > 0)
                            m_actions[i++] = (byte)msg.getByte();
						
						break;
					}
					
					// obtain data about a SWF
				
				case DMessage.InSwfInfo: 
					{
                        int count = msg.getWord();
						for (int i = 0; i < count; i++)
						{
                            long index = msg.getDWord();
                            long id = msg.getDWord();
							
							// get it
							DSwfInfo info = getOrCreateSwfInfo((int) index);
							
							// remember which was last seen
							m_lastSwfInfo = info;
							
							if (id != 0)
							{
                                bool debugComing = (msg.getByte() == 0) ? false : true;
                                byte vmVersion = (byte)msg.getByte(); // AS vm version number (1 = avm+, 0 == avm-)
                                int rsvd1 = msg.getWord();

                                long swfSize = msg.getDWord();
                                long swdSize = msg.getDWord();
                                long scriptCount = msg.getDWord();
                                long offsetCount = msg.getDWord();
                                long breakpointCount = msg.getDWord();

                                long port = msg.getDWord();
                                String path = msg.getString();
                                String url = msg.getString();
                                String host = msg.getString();
								
								// now we read in the swd debugging map (which provides
								// local to global mappings of the script ids
                                long num = msg.getDWord();
								IntMap local2global = new IntMap();
								int minId = Int32.MaxValue;
								int maxId = Int32.MinValue;
								for (int j = 0; j < num; j++)
								{
                                    long local = msg.getDWord();
                                    long global = msg.getDWord();
									local2global.put((int) local, (Int32) global);
									minId = ((int) global < minId)?(int) global:minId;
									maxId = ((int) global > maxId)?(int) global:maxId;
								}
								
								// If its a new record then the swf size would have been unknown at creation time
								bool justCreated = (info.SwfSize == 0);
								
								// if we are a avm+ engine then we don't wait for the swd to load
								if (vmVersion > 0)
								{
									debugComing = false;
									info.VmVersion = vmVersion;
									info.setPopulated(); // added by mmorearty on 9/5/05 for RSL debugging
								}
								
								// update this swfinfo with the lastest data
								info.freshen(id, path, url, host, port, debugComing, swfSize, swdSize, breakpointCount, offsetCount, scriptCount, local2global, minId, maxId);
								// now tie any scripts that have been loaded into this swfinfo object
								tieScriptsToSwf(info);
								
								// notify if its newly created
								if (justCreated)
									addEvent(new SwfLoadedEvent(id, (int) index, path, url, host, port, swfSize));
							}
							else
							{
								// note our state before marking it
								bool alreadyUnloaded = info.isUnloaded();
								
								// clear it out
								info.setUnloaded();
								
								// notify if this information is new.
								if (!alreadyUnloaded)
									addEvent(new SwfUnloadedEvent(info.Id, info.Path, (int) index));
							}
							//					System.out.println("[SWFLOAD] Loaded "+path+", size="+swfSize+", scripts="+scriptCount);
						}
						break;
					}
					
					// obtain the constant pool of some player
				
				case DMessage.InConstantPool: 
					{
                        int item = msg.getWord();
                        int count = (int)msg.getDWord();
						
						String[] pool = new String[count];
						for (int i = 0; i < count; i++)
						{
                            long id = msg.getDWord();
							DVariable var = extractVariable(msg);
							
							// we only need the contents of the variable
							pool[i] = var.getValue().ValueAsString;
						}
						m_lastConstantPool = pool;
						break;
					}
					
					// obtain one or more function name line number mappings.
				
				case DMessage.InGetFncNames: 
				{
                    long id = msg.getDWord(); // module id
                    long count = msg.getDWord(); // number of entries
					
					// get the DModule
					DModule m = getSource((int) id);
					if (m != null)
					{
						for (int i = 0; i < count; i++)
						{
                            int offset = (int)msg.getDWord();
                            int firstLine = (int)msg.getDWord();
                            int lastLine = (int)msg.getDWord();
                            String name = msg.getString();
							
							// now add the entries
							m.addLineFunctionInfo(offset, firstLine, lastLine, name);
						}
					}
					break;
				}
				
				
				default: 
				{
					break;
				}
				
			}
		}
		
		/// <summary> Returns whether a given child member should be shown, or should be filtered out.</summary>
		private bool showMember(DVariable child)
		{
			if (isTraits(child))
				return false;
			return true;
		}
		
		/// <summary> Returns whether this is not a variable at all, but is instead a representation
		/// of a "traits" object.  A "traits" object is the Flash player's way of describing
		/// one class.
		/// </summary>
		private bool isTraits(DVariable variable)
		{
			Value value = variable.getValue();
			if (value.getType() == VariableType.UNKNOWN && Value.TRAITS_TYPE_NAME.Equals(value.getTypeName()))
			{
				return true;
			}
			return false;
		}
		
		/// <summary> Here's where some ugly stuff happens. Since our context contains
		/// more info than what's contained within the stackcontext, we
		/// augment it  with the variables.  Also, we build up a
		/// list of variables that appears under root, that can be
		/// accessed without further qualification; this includes args,
		/// locals and _global.
		/// </summary>
		internal virtual void  populateRootNode(DValue frame, System.Collections.ArrayList orderedChildList)
		{
			// first populate the stack node with children
			populateFrame(0, orderedChildList);
			
			/*
			* We mark it as members obtained so that we don't go to the player
			* and request it, which would be bad, since its our artifical creation.
			*/
			DValue base_Renamed = getOrCreateValue(Value.BASE_ID);
			base_Renamed.MembersObtained = true;
			
			/*
			* Technically, we don't need to create the following nodes, but
			* we like to give them nice type names
			*/
			
			// now let's create a _global node and attach it to base
		}
		
		/// <summary> We are done, so let's look for a number of special variables, since our
		/// frame comes in 3 pieces.  First off is a "this" pointer, followed
		/// by a "$arguments" dummy node, followed by a "super" which marks
		/// the end of the arguments.
		/// 
		/// All of this stuff gets pulled apart after we build the frame node.
		/// </summary>
		internal virtual void  populateFrame(int depth, System.Collections.ArrayList frameVars)
		{
			// get our stack context
			DStackContext context = null;
			bool inArgs = false;
			int nArgs = - 1;
			bool inScopeChain = false;
			
			// create a root node for each stack frame; first is at BASE_ID
			DValue root = getOrCreateValue(Value.BASE_ID - depth);
			
			if (depth < FrameCount)
				context = getFrame(depth);
			
			// trim all current args from this context
			if (context != null)
				context.removeAllVariables();
			
			// use the ordered child list
            foreach (DVariable v in frameVars)
            {
				String name = v.getName();
				
				// let's clear a couple of attributes that may get in our way
				v.clearAttribute(VariableAttribute.IS_LOCAL);
				v.clearAttribute(VariableAttribute.IS_ARGUMENT);
				if (name.Equals("this"))
				//$NON-NLS-1$
				{
					if (context != null)
						context.setThis(v);
					
					// from our current frame, put a pseudo this entry into the cache and hang it off base, mark it as an implied arg
					v.Attribute = VariableAttribute.IS_ARGUMENT;
					addVariableMember(root, v);
					
					// also add this variable under THIS_ID
					if (depth == 0)
						putValue(Value.THIS_ID, (DValue) v.getValue());
				}
				else if (name.Equals("super"))
				//$NON-NLS-1$
				{
					// we are at the end of the arg list and let's make super part of global
					inArgs = false;
				}
				else if (name.Equals(ARGUMENTS_MARKER))
				{
					inArgs = true;
					
					// see if we can extract an arg count from this variable
					try
					{
						nArgs = (int) Convert.ToDouble(((ValueType) (v.getValue().ValueAsObject)));
					}
					catch (FormatException)
					{
					}
				}
				else if (name.Equals(SCOPE_CHAIN_MARKER))
				{
					inArgs = false;
					inScopeChain = true;
				}
				else
				{
					// add it to our root, marking it as an arg if we know, otherwise local
					if (inArgs)
					{
						v.Attribute = VariableAttribute.IS_ARGUMENT;
						
						if (context != null)
							context.addArgument(v);
						
						// decrement arg count if we have it
						if (nArgs > - 1)
						{
							if (--nArgs <= 0)
								inArgs = false;
						}
					}
					else if (inScopeChain)
					{
						if (context != null)
							context.addScopeChainEntry(v);
					}
					else
					{
						v.Attribute = VariableAttribute.IS_LOCAL;
						if (context != null)
							context.addLocal(v);
					}
					
					// add locals and arguments to root
					if (!inScopeChain)
						addVariableMember(root, v);
				}
			}
		}
		
		/// <summary> Map DMessage / Player attributes to VariableAttributes</summary>
		internal virtual int toAttributes(int pAttr)
		{
			int attr = pAttr; /* 1-1 mapping */
			return attr;
		}
		
		internal virtual DVariable extractVariable(DMessage msg)
		{
            DVariable v = extractVariable(msg, msg.getString());
			return v;
		}
		
		/// <summary> Build a variable based on the information we can extract from the messsage</summary>
		internal virtual DVariable extractVariable(DMessage msg, String name)
		{
            int oType = msg.getWord();
            int flags = (int)msg.getDWord();
			return extractAtom(msg, name, oType, flags);
		}
		
		/// <summary> Extracts an builds a register variable</summary>
		internal virtual DVariable extractRegister(DMessage msg, int number)
		{
            int oType = msg.getWord();
			return extractAtom(msg, "$" + number, oType, 0); //$NON-NLS-1$
		}
		
		/// <summary> Does the job of pulling together a variable based on
		/// the type of object encountered.
		/// </summary>
		internal virtual DVariable extractAtom(DMessage msg, String name, int oType, int flags)
		{
			int vType = VariableType.UNKNOWN;
			Object value = null;
			String typeName = ""; //$NON-NLS-1$
			String className = ""; //$NON-NLS-1$
			
			/* now we vary depending upon type */
			switch (oType)
			{
				
				case DMessage.kNumberType: 
				{
                    String s = msg.getString();
					double dval = 0;
					try
					{
                        System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo();
                        dval = Double.Parse(s, nfi);
                    }
					catch (FormatException)
					{
					}
					
					value = (double) dval;
					vType = VariableType.NUMBER;
					typeName = "Number"; //$NON-NLS-1$
					break;
				}
				
				
				case DMessage.kBooleanType: 
				{
                    int bval = msg.getByte();
					value = (bval == 0)?false:true;
					vType = VariableType.BOOLEAN;
					typeName = "Boolean"; //$NON-NLS-1$
					break;
				}
				
				
				case DMessage.kStringType: 
				{
                    String s = msg.getString();
					
					value = s;
					vType = VariableType.STRING;
					typeName = "String"; //$NON-NLS-1$
					break;
				}
				
				
				case DMessage.kObjectType: 
				case DMessage.kNamespaceType: 
				{
                    long oid = msg.getDWord();
                    long cType = (oid == -1) ? 0 : msg.getDWord();
                    int isFnc = (oid == -1) ? 0 : msg.getWord();
                    int rsvd = (oid == -1) ? 0 : msg.getWord();
                    typeName = (oid == -1) ? "" : msg.getString(); //$NON-NLS-1$
					className = DVariable.classNameFor(cType, false);
					
					value = (long) oid;
					vType = (isFnc == 0)?VariableType.OBJECT:VariableType.FUNCTION;
					break;
				}
				
				
				case DMessage.kMovieClipType: 
				{
                    long oid = msg.getDWord();
                    long cType = (oid == -1) ? 0 : msg.getDWord();
                    long rsvd = (oid == -1) ? 0 : msg.getDWord();
                    typeName = (oid == -1) ? "" : msg.getString(); //$NON-NLS-1$
					className = DVariable.classNameFor(cType, true);
					
					value = (long) oid;
					vType = VariableType.MOVIECLIP;
					break;
				}
				
				
				case DMessage.kNullType: 
				{
					vType = VariableType.NULL;
					typeName = "null"; //$NON-NLS-1$
					break;
				}
				
				
				case DMessage.kUndefinedType: 
				{
					vType = VariableType.UNDEFINED;
					typeName = "undefined"; //$NON-NLS-1$
					break;
				}
				
				
				case DMessage.kTraitsType: 
				{
					// This one is special: When passed to the debugger, it indicates
					// that the "variable" is not a variable at all, but rather is a
					// class name.  For example, if class Y extends class X, then
					// we will send a kDTypeTraits for class Y; then we'll send all the
					// members of class Y; then we'll send a kDTypeTraits for class X;
					// and then we'll send all the members of class X.  This is only
					// used by the AVM+ debugger.
					vType = VariableType.UNKNOWN;
					typeName = Value.TRAITS_TYPE_NAME;
					break;
				}
				
				
				case DMessage.kReferenceType: 
				case DMessage.kArrayType: 
				case DMessage.kObjectEndType: 
				case DMessage.kStrictArrayType: 
				case DMessage.kDateType: 
				case DMessage.kLongStringType: 
				case DMessage.kUnsupportedType: 
				case DMessage.kRecordSetType: 
				case DMessage.kXMLType: 
				case DMessage.kTypedObjectType: 
				case DMessage.kAvmPlusObjectType: 
				default: 
				{
					//				System.out.println("<unknown>");
					break;
				}
				}
			
			// create the variable based on the content we received.
			DValue valueObject = null;
			if (value is Int64)
			{
				valueObject = getValue((long) ((Int64) value));
			}
			
			if (valueObject == null)
			{
				valueObject = new DValue(vType, typeName, className, toAttributes(flags), value);
				
				if (value is Int64 && (toAttributes(flags) & VariableAttribute.HAS_GETTER) == 0)
					putValue((long) ((Int64) value), valueObject);
			}
			else
			{
				valueObject.setType(vType);
				valueObject.setTypeName(typeName);
				valueObject.setClassName(className);
				valueObject.setAttributes(toAttributes(flags));
				valueObject.Value = value;
			}
			
			DVariable var = new DVariable(name, valueObject);
			return var;
		}
		
		/// <summary> The player sends us a URI using '|' instead of ':'</summary>
		public static String convertToURI(String playerURL)
		{
			int index = playerURL.IndexOf('|');
			System.Text.StringBuilder sb = new System.Text.StringBuilder(playerURL);
			while (index > 0)
			{
				sb[index] = ':';
				index = playerURL.IndexOf('|', index + 1);
			}
			return sb.ToString();
		}
		
		/// <summary> Tell us that a getter or setter is about to be executed.  If a
		/// FaultEvent comes in while a getter or setter is executing, it
		/// is not added to the event queue in the normal way -- instead,
		/// it is saved, and is returned when endGetterSetter() is called.
		/// </summary>
		public virtual void  beginGetterSetter()
		{
			m_inGetterSetter = true;
			m_faultEventDuringGetterSetter = null;
		}
		
		/// <summary> Informs us that a getter or setter is no longer executing, and
		/// returns the fault, if any, which occurred while the getter/
		/// setter was executing.
		/// </summary>
		public virtual FaultEvent endGetterSetter()
		{
			m_inGetterSetter = false;
			FaultEvent e = m_faultEventDuringGetterSetter;
			m_faultEventDuringGetterSetter = null;
			return e;
		}
		
		/// <summary> When we've just received any FaultEvent from the player, this
		/// function gets called.  If a getter/setter is currently executing,
		/// we'll save the fault for someone to get later by calling
		/// endGetterSetter().  Otherwise, normal code execution is taking
		/// place, so we'll add the event to the event queue.
		/// </summary>
		private void  handleFaultEvent(FaultEvent faultEvent)
		{
			if (m_inGetterSetter)
			{
				if (m_faultEventDuringGetterSetter == null)
				// only save the first fault
				{
					// save the event away so that when someone later calls
					// endGetterSetter(), we can return the fault that
					// occurred
					m_faultEventDuringGetterSetter = faultEvent;
				}
			}
			else
			{
				// regular code is running; so post the event to the
				// event queue which the client debugger will see
				addEvent(faultEvent);
			}
		}
		
		/* (non-Javadoc)
		* @see Flash.Tools.Debugger.SourceLocator#locateSource(java.lang.String, java.lang.String, java.lang.String)
		*/
		public virtual Stream locateSource(String path, String pkg, String name)
		{
			if (m_sourceLocator != null)
				return m_sourceLocator.locateSource(path, pkg, name);
			
			return null;
		}
		
		/// <summary> Returns the value of a Flash Player option that was requested by
		/// OutGetOption and returned by InOption.
		/// 
		/// </summary>
		/// <param name="optionName">the name of the option
		/// </param>
		/// <returns> its value, or null
		/// </returns>
		public virtual String getOption(String optionName)
		{
			return (String) m_options[optionName];
		}
	}
}