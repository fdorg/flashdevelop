// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
using System.IO;

using Flash.Swf;
using Flash.Swf.Actions;
using Flash.Swf.Tags;
using Flash.Swf.Types;
using Trace = Flash.Util.Trace;

namespace Flash.Tools
{
	
	/// <summary> This class implements the TagHandler interface
	/// and provides a mechanism for containing the
	/// actions associated with a SWF.
	/// </summary>
	public class SwfActionContainer:TagHandler
	{
		virtual public ActionList MasterList
		{
			// getters 
			
			get
			{
				return m_master;
			}
			
		}
		virtual public Header Header
		{
			get
			{
				return m_header;
			}
			
		}
		virtual public Dictionary Dictionary
		{
			get
			{
				return m_dictionary;
			}
			set
			{
				m_dictionary = value;
			}
			
		}
		internal bool errorProcessing = true;
		internal ActionList m_master;
		
		// temporaries used while decoding
		internal Dictionary m_dictionary;
		internal Header m_header;
		
		public SwfActionContainer(byte[] swf, byte[] swd):this(new MemoryStream(swf), new MemoryStream(swd))
		{
		}
		public SwfActionContainer(Stream swfIn):this(swfIn, null)
		{
		}
		
		public SwfActionContainer(Stream swfIn, Stream swdIn)
		{
			TagDecoder p = new TagDecoder(swfIn, swdIn);
			try
			{
				process(p);
				errorProcessing = false;
			}
			catch (IOException io)
			{
                if (Trace.error)
                {
                    Console.Error.Write(io.StackTrace);
                    Console.Error.Flush();
                }
			}
		}
		
		// Did we hit an error in processing the swf? 
		public virtual bool hasErrors()
		{
			return errorProcessing;
		}
		
		/// <summary> Ask a TagDecoder to do its magic, calling us 
		/// upon each encounter of a new tag.
		/// </summary>
		internal virtual void  process(TagDecoder d)
		{
			m_master = new ActionList(true);
			d.KeepOffsets = true;
			d.parse(this);
		}
		
		/// <summary> Return a path to an ActionList that contains the given offset
		/// if an exact match is not found then return the largest
		/// that does not exceed offset.
		/// </summary>
		public virtual ActionLocation locationLessOrEqualTo(int offset)
		{
			ActionLocation l = new ActionLocation();
			locationLessOrEqualTo(l, m_master, offset);
			return l;
		}
		
		public static ActionLocation locationLessOrEqualTo(ActionLocation location, ActionList list, int offset)
		{
			int at = findLessOrEqualTo(list, offset);
			if (at > - 1)
			{
				// we hit so mark it and extract a constant pool if any
				location.at = at;
				location.actions = list;
				
				Action a = list.getAction(0);
				if (a.code == ActionConstants.sactionConstantPool)
					location.pool = (ConstantPool) a;
				
				// then see if we need to traverse
				a = list.getAction(at);
				if ((a.code == ActionConstants.sactionDefineFunction) || (a.code == ActionConstants.sactionDefineFunction2))
				{
					location.function = (DefineFunction) a;
					locationLessOrEqualTo(location, ((DefineFunction) a).actionList, offset);
				}
				else if (a is DummyAction)
				{
					// our dummy container, then we drop in
					locationLessOrEqualTo(location, ((DummyAction) a).ActionList, offset);
				}
			}
			return location;
		}
		
		// find the index of the largest offset in the list that does not
		// exceed the offset value provided. 
		public static int findLessOrEqualTo(ActionList list, int offset)
		{
			int i = find(list, offset);
			if (i < 0)
			{
				// means we didn't locate it, so get the next closest one
				// which is 1 below the insertion point
				i = (- i - 1) - 1;
			}
			return i;
		}
		
		// perform a binary search to locate the offset within the sorted
		// list of offsets within the action list.
		// if no match then (-i - 1) provides the index of where an insertion
		// would occur for this offset in the list.
		public static int find(ActionList list, int offset)
		{
			int lo = 0;
			int hi = list.size() - 1;
			
			while (lo <= hi)
			{
				int i = (lo + hi) / 2;
				int m = list.getOffset(i);
				if (offset > m)
					lo = i + 1;
				else if (offset < m)
					hi = i - 1;
				else
					return i; // offset found
			}
			return - (lo + 1); // offset not found, low is the insertion point
		}
		
		/// <summary> Dummy Action container for housing all of  our
		/// topmost level actionlists in a convenient form
		/// </summary>
		public class DummyAction:Action
		{
			virtual public ActionList ActionList
			{
				// getters/setters
				
				get
				{
					return m_actionList;
				}
				
			}
			virtual public String ClassName
			{
				get
				{
					return m_className;
				}
				
				set
				{
					m_className = value;
				}
				
			}
			public DummyAction(ActionList list) : base(ActionConstants.sactionNone)
			{
				m_actionList = list;
			}
			
			private ActionList m_actionList;
			private String m_className;
		}
		
		/// <summary> Store away the ActionLists for later retrieval</summary>
		internal virtual DummyAction recordActions(ActionList list)
		{
			DummyAction da = null;
			if (list != null && list.size() > 0)
			{
				// use the first offset as our reference point
				int offset = list.getOffset(0);
				
				// now create a pseudo action for this action list in our master
				da = new DummyAction(list);
				m_master.setActionOffset(offset, da);
			}
			return da;
		}
		
		/// <summary> -----------------------------------------------
		/// The following APIs override TagHandler.
		/// -----------------------------------------------
		/// </summary>
		public override void  doInitAction(DoInitAction tag)
		{
			DummyAction a = recordActions(tag.actionList);
			
			// now fill in the class name if we can
			if (m_header.version > 6 && tag.sprite != null)
			{
				String __Packages = MovieMetaData.idRef(tag.sprite, m_dictionary);
				String className = (__Packages != null && __Packages.StartsWith("__Packages"))?__Packages.Substring(11):null; //$NON-NLS-1$
				a.ClassName = className;
			}
		}

        public override void doAction(DoAction tag)
		{
			recordActions(tag.actionList);
		}


        public override void defineSprite(DefineSprite tag)
		{
			// @todo need to support actions in sprites!!! 
		}
		
		public override void  placeObject2(PlaceObject tag)
		{
			if (tag.hasClipAction())
			{
				System.Collections.IEnumerator it = tag.clipActions.clipActionRecords.GetEnumerator();
				while (it.MoveNext())
				{
					ClipActionRecord record = (ClipActionRecord) it.Current;
					recordActions(record.actionList);
				}
			}
		}

        public override void defineButton(DefineButton tag)
		{
			recordActions(tag.condActions[0].actionList);
		}

        public override void defineButton2(DefineButton tag)
		{
			if (tag.condActions.Length > 0)
			{
				for (int i = 0; i < tag.condActions.Length; i++)
				{
					ButtonCondAction cond = tag.condActions[i];
					recordActions(cond.actionList);
				}
			}
		}

        public override void header(Header h)
		{
			m_header = h;
		}
	}
}
