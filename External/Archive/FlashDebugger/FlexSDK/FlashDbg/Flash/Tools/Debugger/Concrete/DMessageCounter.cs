// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2003-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
namespace Flash.Tools.Debugger.Concrete
{
	
	/// <summary> This class can be to count the number of messages
	/// received during a debug session.
	/// 
	/// </summary>
	public class DMessageCounter : DProtocolNotifierIF
	{
		/// <summary> Returns the object on which external code can call "wait" in order
		/// to block until a message is received.
		/// </summary>
		virtual public System.Object InLock
		{
			get
			{
				return m_inCounts;
			}
			
		}
		/// <summary> Returns the object on which external code can call "wait" in order
		/// to block until a message is sent.
		/// </summary>
		virtual public System.Object OutLock
		{
			get
			{
				return m_outCounts;
			}
			
		}
		internal long[] m_inCounts;
		internal long[] m_outCounts;
		
		public DMessageCounter()
		{
			m_inCounts = new long[DMessage.InSIZE + 1];
			m_outCounts = new long[DMessage.OutSIZE + 1];
			
			clearArray(m_inCounts);
			clearArray(m_outCounts);
		}
		
		public virtual void  disconnected()
		{
			// We're being notified (via the DProtocolNotifierIF interface) that
			// the socket connection has been broken.  If anyone is waiting for
			// a message to come in, they ain't gonna get one.  So, we'll notify()
			// them so that they can wake up and realize that the connection has
			// been broken.
			System.Object inLock = InLock;
			lock (inLock)
			{
				System.Threading.Monitor.PulseAll(inLock);
			}
			System.Object outLock = OutLock;
			lock (outLock)
			{
				System.Threading.Monitor.PulseAll(outLock);
			}
		}
		
		/// <summary> Collect stats on outgoing messages </summary>
		public virtual void  messageSent(DMessage msg)
		{
			int type = msg.Type;
			if (type < 0 || type >= DMessage.OutSIZE)
				type = DMessage.OutSIZE;
			
			System.Object outLock = OutLock;
			lock (outLock)
			{
				m_outCounts[type] += 1;
				System.Threading.Monitor.PulseAll(outLock); // tell anyone who is waiting that a message has been sent
			}
		}
		
		/// <summary> Collect stats on the messages </summary>
		public virtual void  messageArrived(DMessage msg, DProtocol which)
		{
			/* extract type */
			int type = msg.Type;
			
			//		System.out.println("msg counter ="+type);
			
			/* anything we don't know about goes in a special slot at the end of the array. */
			if (type < 0 || type >= DMessage.InSIZE)
				type = DMessage.InSIZE;
			
			System.Object inLock = InLock;
			lock (inLock)
			{
				m_inCounts[type] += 1;
				System.Threading.Monitor.PulseAll(inLock); // tell anyone who is waiting that a message has been received
			}
		}
		
		/* getters */
		public virtual long getInCount(int type)
		{
			lock (InLock)
			{
				return m_inCounts[type];
			}
		}
		public virtual long getOutCount(int type)
		{
			lock (OutLock)
			{
				return m_outCounts[type];
			}
		}
		
		/* setters */
		public virtual void  clearInCounts()
		{
			lock (InLock)
			{
				clearArray(m_inCounts);
			}
		}
		public virtual void  clearOutCounts()
		{
			lock (OutLock)
			{
				clearArray(m_outCounts);
			}
		}
		
		/// <summary> Clear out the array </summary>
		internal virtual void  clearArray(long[] ar)
		{
			for (int i = 0; i < ar.Length; i++)
				ar[i] = 0;
		}
	}
}