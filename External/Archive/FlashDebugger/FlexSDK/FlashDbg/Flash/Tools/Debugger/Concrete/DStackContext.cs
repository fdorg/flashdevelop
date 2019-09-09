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
using JavaCompatibleClasses;

using Frame = Flash.Tools.Debugger.Frame;
using Location = Flash.Tools.Debugger.Location;
using NoResponseException = Flash.Tools.Debugger.NoResponseException;
using NotConnectedException = Flash.Tools.Debugger.NotConnectedException;
using NotSuspendedException = Flash.Tools.Debugger.NotSuspendedException;
using Session = Flash.Tools.Debugger.Session;
using Variable = Flash.Tools.Debugger.Variable;
namespace Flash.Tools.Debugger.Concrete
{
	
	public class DStackContext : Frame
	{
		virtual public Location Location
		{
			/*
			* @see Flash.Tools.Debugger.Frame#getLocation()
			*/
			
			get
			{
				return m_location;
			}
			
		}
		virtual public String CallSignature
		{
			/* getters */
			
			get
			{
				return m_functionSignature;
			}
			
		}
		virtual public int Module
		{
			get
			{
				return m_module;
			}
			
		}
		virtual public int Line
		{
			get
			{
				return m_line;
			}
			
		}
		virtual internal int Depth
		{
			set
			{
				m_depth = value;
			}
			
		}
		virtual internal int SwfIndex
		{
			set
			{
				m_swfIndex = value;
			}
			
		}
		virtual internal int Offset
		{
			set
			{
				m_offset = value;
			}
			
		}
		/// <summary> Gets the activation object for this frame, or <code>null</code>
		/// if none.  See bug FB-2674.
		/// </summary>
		virtual internal DVariable ActivationObject
		{
			get
			{
				return m_activationObject;
			}
			
		}
		private DModule m_source;
		private String m_functionSignature;
		private int m_depth;
		private int m_module;
		private int m_line;
		private DVariable m_this;
		private System.Collections.IDictionary m_args;
        private System.Collections.IDictionary m_locals;
		private System.Collections.IList m_scopeChain;
		private DLocation m_location;
		private int m_swfIndex; /* index of swf that we halted within (really part of location) */
		private int m_offset; /* offset within swf where we halted. (really part of location) */
		private bool m_populated;
		private DVariable m_activationObject;
		
		public DStackContext(int module, int line, DModule f, int thisId, String functionSignature, int depth)
		{
			m_source = f;
			m_module = module;
			m_line = line;
			// the passed-in 'thisId' seems to always be equal to one, which does more harm than good
			m_this = null;
			m_functionSignature = functionSignature;
			m_depth = depth;
            m_args = new LinkedHashMap(); // preserves order
            m_locals = new LinkedHashMap(); // preserves order
			m_scopeChain = new System.Collections.ArrayList();
			m_populated = false;
			m_location = new DLocation(m_source, line);
		}
		
		/*
		* @see Flash.Tools.Debugger.Frame#getArguments(Flash.Tools.Debugger.Session)
		*/
		public virtual Variable[] getArguments(Session s)
		{
			populate(s);
            Variable[] args = new Variable[m_args.Count];
            m_args.Values.CopyTo(args, 0);
            return args;
		}
		
		/*
		* @see Flash.Tools.Debugger.Frame#getLocals(Flash.Tools.Debugger.Session)
		*/
		public virtual Variable[] getLocals(Session s)
		{
			populate(s);
            Variable[] locals = new Variable[m_locals.Count];
            m_locals.Values.CopyTo(locals, 0);
            return locals;
		}
		
		/*
		* @see Flash.Tools.Debugger.Frame#getThis(Flash.Tools.Debugger.Session)
		*/
		public virtual Variable getThis(Session s)
		{
			populate(s);
			return getThis();
		}
		
		/*
		* @see Flash.Tools.Debugger.Frame#getScopeChain()
		*/
		public virtual Variable[] getScopeChain(Session s)
		{
			populate(s);
			return (Variable[]) SupportClass.ICollectionSupport.ToArray(m_scopeChain, new Variable[m_scopeChain.Count]);
		}
		public virtual DVariable getThis()
		{
			return m_this;
		}
		
		/* setters */
		internal virtual void  addArgument(DVariable v)
		{
            m_args[v.getName()] = v;
		}
		internal virtual void  addLocal(DVariable v)
		{
			m_locals[v.getName()] = v;
		}
		internal virtual void  addScopeChainEntry(DVariable v)
		{
			m_scopeChain.Add(v);
		}
		internal virtual void  removeAllArguments()
		{
			m_args.Clear();
		}
		internal virtual void  removeAllLocals()
		{
			m_locals.Clear(); m_activationObject = null;
		}
		internal virtual void  removeAllScopeChainEntries()
		{
			m_scopeChain.Clear();
		}
		internal virtual void  removeAllVariables()
		{
			removeAllLocals(); removeAllArguments(); removeAllScopeChainEntries();
		}
		internal virtual void  setThis(DVariable v)
		{
			m_this = v;
		}
		internal virtual void  markStale()
		{
			m_populated = false;
		} // triggers a reload of variables.
		
		/// <summary> Removes the specified variable from the list of locals, and
		/// remembers that the specified variable is the "activation object"
		/// for this frame.  See bug 155031.
		/// </summary>
		internal virtual void  convertLocalToActivationObject(DVariable v)
		{
			m_activationObject = v;
			m_locals.Remove(v.getName());
		}
		
		/// <summary> Populate ensures that we have some locals and args. That is
		/// that we have triggered a InFrame call to the player
		/// </summary>
		/// <throws>  NoResponseException </throws>
		/// <throws>  NotSuspendedException </throws>
		/// <throws>  NotConnectedException </throws>
		internal virtual void  populate(Session s)
		{
			if (!m_populated)
			{
				((PlayerSession) s).requestFrame(m_depth);
				m_populated = true;
				foreach (DVariable v in m_args.Values)
				{
					v.Session = s;
				}
				foreach (DVariable v in m_locals.Values)
				{
					v.Session = s;
				}
				if (m_this != null) m_this.Session = s;
			}
		}
	}
}