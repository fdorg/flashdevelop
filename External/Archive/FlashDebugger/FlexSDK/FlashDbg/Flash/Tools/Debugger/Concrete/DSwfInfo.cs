// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2004-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using Flash.Tools.Debugger.Events;
using DebugModule = Flash.Swf.Debug.DebugModule;
using IntMap = Flash.Util.IntMap;
using LineRecord = Flash.Swf.Debug.LineRecord;

namespace Flash.Tools.Debugger.Concrete
{
	public class DSwfInfo : SwfInfo
	{
		/// <summary>SwfInfo interface </summary>
		virtual public String Path
		{
			get
			{
				return m_path;
			}
			
		}
		virtual public String Url
		{
			get
			{
				return m_url;
			}
			
		}
		virtual public int SwfSize
		{
			get
			{
				return m_swfSize;
			}
			
		}
		virtual public bool ProcessingComplete
		{
			get
			{
				return isPopulated();
			}
			
		}
		virtual public long Id
		{
			/* getters */
			
			get
			{
				return m_id;
			}
			
		}
		virtual public String Host
		{
			get
			{
				return m_host;
			}
			
		}
		virtual public int Port
		{
			get
			{
				return m_port;
			}
			
		}
		virtual public int RefreshCount
		{
			get
			{
				return m_numRefreshes;
			}
			
		}
		virtual public bool SwdLoading
		{
			get
			{
				return m_swdLoading;
			}
			
		}
		virtual public byte[] Swf
		{
			get
			{
				return m_swf;
			}
			
			set
			{
				m_swf = value;
			}
			
		}
		virtual public byte[] Swd
		{
			get
			{
				return m_swd;
			}
			
			set
			{
				m_swd = value;
			}
			
		}
		virtual public int SourceExpectedCount
		{
			get
			{
				return m_scriptsExpected;
			}
			
			set
			{
				m_scriptsExpected = value;
			}
			
		}
		virtual public int VmVersion
		{
			get
			{
				return m_vmVersion;
			}
			
			set
			{
				m_vmVersion = value;
			}
			
		}
		virtual public int FirstSourceId
		{
			get
			{
				return m_minId;
			}
			
		}
		virtual public int LastSourceId
		{
			get
			{
				return m_maxId;
			}
			
		}
		private int m_index;
		private long m_id;
		private IntMap m_source;
		private String m_path;
		private String m_url;
		private String m_host;
		private int m_port;
		private bool m_swdLoading;
		private int m_swfSize;
		private int m_swdSize;
		private int m_bpCount;
		private int m_offsetCount;
		private int m_scriptsExpected;
		private int m_minId; // first script id in the swf
		private int m_maxId; // last script id in this swf
		private byte[] m_swf; // actual swf contents
		private byte[] m_swd; // actual swd contents
		private bool m_unloaded; // set if the player has unloaded this swf
		private IntMap m_local2Global; // local script id to global script id mapping table
		private int m_numRefreshes; // number of refreshes we have taken
		private int m_vmVersion; // version of the vm
		
		private bool m_populated; // set if we have already tried to load swf/swd for this info
		private LineFunctionContainer m_container; // used for pulling out detailed info about the swf
		
		private static String UNKNOWN = PlayerSessionManager.LocalizationManager.getLocalizedTextString("unknown"); //$NON-NLS-1$
		
		public DSwfInfo(int index)
		{
			// defaults values of zero
			m_id = 0;
			m_index = index;
			m_source = new IntMap();
			m_path = UNKNOWN;
			m_url = UNKNOWN;
			m_host = UNKNOWN;
			m_port = 0;
			m_swdLoading = true;
			m_scriptsExpected = - 1; // means not yet set by anyone!
			// rest default to null, 0 or false
		}
		public virtual int getSwdSize(Session s)
		{
			swdLoaded(s); return m_swdSize;
		}
		public virtual bool isUnloaded()
		{
			return m_unloaded;
		}
		public virtual bool containsSource(SourceFile f)
		{
			return m_source.contains(f.Id);
		}
		public virtual int getSwdSize()
		{
			return m_swdSize;
		}
		public virtual bool isPopulated()
		{
			return m_populated;
		}
		
		//	public int			getBreakpointCount() throws InProgressException	{ swdLoading(); return m_bpCount; }
		//	public int			getOffsetCount() 		{ swdLoading(); return m_offsetCount; }
		public virtual int getSourceCount()
		{
			return m_source.size();
		}
		public virtual void  setUnloaded()
		{
			m_unloaded = true;
		}
		public virtual void  setPopulated()
		{
			m_swdLoading = false; m_populated = true;
		} // no more waiting for swd, we're done
		
		public virtual void  addSource(int i, DModule m)
		{
			m_source.put(i, m);
		}
		
		/// <summary> Return the number of sources that we have</summary>
		public virtual int getSourceCount(Session s)
		{
			// only if we don't have it all yet
			// then try to force a load
			if (!hasAllSource())
				swdLoaded(s);
			
			return getSourceCount();
		}
		
		/// <summary> Return a list of our sources</summary>
		public virtual SourceFile[] getSourceList(Session s)
		{
			// only if we don't have it all yet
			// then try to force a load
			if (!hasAllSource())
				swdLoaded(s);
			
			return (SourceFile[]) m_source.valuesToArray(new SourceFile[m_source.size()]);
		}
		
		/// <summary> Make sure that the player has loaded our swd.  If not
		/// we continue InProgressException to query the player for when its complete.
		/// At some point we give up and finally admit that
		/// we don't have a swd associated with this swf.
		/// </summary>
		internal virtual void  swdLoaded(Session s)
		{
			if (SwdLoading && !isUnloaded())
			{
				// make the request 
				try
				{
					((PlayerSession) s).requestSwfInfo(m_index);
				}
				catch (NoResponseException)
				{
				}
				
				// I should now be complete
				if (!m_swdLoading)
				{
				}
				// done!
				else if (SourceExpectedCount > - 1 && m_numRefreshes > 10)
					setPopulated();
				// tried too many times, so bail big time, no swd available (only if we already have our expected count)
				else
					throw new InProgressException(); // still loading!!!
			}
		}
		
		/// <summary> This method returns true once we have all the scripts
		/// that we expect to ever have.  We can get the information about
		/// how many scripts we should get from two sources, 1) we may
		/// get an InSwfInfo message from the player which contains
		/// this value and 2) we may get a InNumScript message which
		/// contains a script count.  A small caveat of course, is that
		/// in case 1. we may also not get the a value if the swd has
		/// not been fully processed by the player yet. 
		/// </summary>
		public virtual bool hasAllSource()
		{
			bool yes = false;
			int expect = SourceExpectedCount;
			int have = getSourceCount();
			
			// if they are equal we are done, unless
			// our expectation has not been set and have not yet loaded our swd
			if (expect == - 1 && SwdLoading)
				yes = false;
			else if (expect == have)
				yes = true;
			else
				yes = false;
			
			return yes;
		}
		
		public virtual void  freshen(long id, String path, String url, String host, long port, bool swdLoading, long swfSize, long swdSize, long bpCount, long offsetCount, long scriptCount, IntMap map, int minId, int maxId)
		{
			m_id = (int) id;
			m_path = path;
			m_url = url;
			m_host = host;
			m_port = (int) port;
			m_swfSize = (int) swfSize;
			m_swdSize = (int) swdSize;
			m_bpCount = (int) bpCount;
			m_offsetCount = (int) offsetCount;
			m_local2Global = map;
			m_minId = (swdSize > 0)?minId:0;
			m_maxId = (swdSize > 0)?maxId:0;
			m_swdLoading = swdLoading;
			m_numRefreshes++;
			
			// only touch expected count if swd already loaded
			if (!swdLoading)
				m_scriptsExpected = (int) scriptCount;
		}
		
		/// <summary> Locate the given offset within the swf</summary>
		public virtual ActionLocation locate(int offset)
		{
			return m_container.locationLessOrEqualTo(offset);
		}
		
		/// <summary> Ask the container to locate the next line
		/// record following the location specified in the 
		/// location, without spilling over into the next
		/// action list
		/// </summary>
		public virtual ActionLocation locateSourceLineEnd(ActionLocation l)
		{
			return locateSourceLineEnd(l, - 1);
		}
		
		public virtual ActionLocation locateSourceLineEnd(ActionLocation l, int stopAt)
		{
			ActionLocation end = m_container.endOfSourceLine(l);
			if (stopAt > - 1 && end.at > stopAt)
				end.at = stopAt;
			return end;
		}
		
		/// <summary> Use the local2global script id map that was provided by the
		/// Player, so that we can take the local id contained in the swd
		/// and convert it to a global one that the player has annoited
		/// to this script.
		/// 
		/// </summary>
		internal virtual int local2Global(int id)
		{
            if (m_local2Global.contains(id))
            {
                return (int)m_local2Global.get_Renamed(id);
            }
            else
            {
                return id;
            }
		}
		
		/// <summary> Freshen the contents of this object with the given swf info
		/// The items that we touch are all swd related, as everything else
		/// has arrriave
		/// </summary>
		
		// temporary while we parse
		internal DManager m_manager;
		
		/// <summary> Extracts information out of the SWF/SWD in order to populate
		/// function line number tables in SourceFile variabels.
		/// </summary>
		public virtual void  parseSwfSwd(DManager manager)
		{
			m_manager = manager;
			
			// suck in the swf/swd into action lists and then walk the lists
			// looking for LineRecords
			m_container = new LineFunctionContainer(m_swf, m_swd);
			m_container.combForLineRecords(this);
			
			// we are done, sucess or no
			setPopulated();
			
			// log event that we have complete done
			manager.addEvent(new FunctionMetaDataAvailableEvent());
			m_manager = null;
		}
		
		/// <summary> This is a callback function from LineFunctionContainer.combForLineRecords()
		/// We extract what we want and then update the associated module
		/// </summary>
		public virtual void  processLineRecord(ActionLocation where, LineRecord r)
		{
			int line = r.lineno;
			String func = (where.function == null)?null:where.function.name;
			DebugModule dm = r.module;
			
			// locate the source file
			int id = - 1;
			DModule module;
			
			if (dm == null || where.at == - 1)
			{
			}
			else if ((id = local2Global(dm.id)) < 0)
			{
			}
			else if ((module = m_manager.getSource(id)) == null)
			{
			}
			else
				module.addLineFunctionInfo(where.actions.getOffset(where.at), line, func);
		}
		
		/* for debugging */
		public override String ToString()
		{
			return m_path;
		}
	}
}