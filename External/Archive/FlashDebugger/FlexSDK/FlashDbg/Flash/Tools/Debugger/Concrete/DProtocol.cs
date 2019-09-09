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
using System.IO;
using Trace = Flash.Util.Trace;

namespace Flash.Tools.Debugger.Concrete
{
	
	/// <summary> Implements the lower portion of Flash Player debug protocol.  This class is able to
	/// communicate with the Flash Player sending and receiving any and all messages neccessary
	/// in order to continue a debug session with the Player.
	/// 
	/// It does not understand the context of messages that it receives and merely provides
	/// a channel for formatting and unformatting the messages.
	/// 
	/// The messages are defined on the flash side in core/debugtags.h and handled in the 
	/// code under core/playerdebugger.cpp
	/// 
	/// Messages that are received via this class are packaged in a DMessage and then
	/// provided to any listeners if requested.   Filtering of incoming messages 
	/// at this level is not supported.
	/// </summary>
	public class DProtocol
	{
		virtual public DMessageCounter MessageCounter
		{
			get
			{
				// The last listeners is always the message counter
				return (DMessageCounter) m_listeners[m_listeners.Count - 1];
			}
			
		}
		public const int DEBUG_PORT = 7935;
		
		private BufferedStream m_in;
		private BufferedStream m_out;
		private System.Collections.ArrayList m_listeners; // WARNING: accessed from multiple threads (but Vector is synchronized)
		private long m_msgRx; // WARNING: accessed from multiple threads; use synchronized (this)
		private long m_msgTx; // WARNING: accessed from multiple threads; use synchronized (this)
		private volatile bool m_stopRx; // WARNING: accessed from multiple threads
		private volatile System.Threading.Thread m_rxThread; // WARNING: accessed from multiple threads
        //private RingBuffer m_ReceivedLog;

        public DProtocol(BufferedStream inStream, BufferedStream outStream)
		{
            m_in = inStream;
            m_out = outStream;
			m_listeners = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
			m_msgRx = 0;
			m_msgTx = 0;
			m_stopRx = false;
			m_rxThread = null;
            //m_ReceivedLog = new RingBuffer(8192, true);
			
			// Create a message counter, which will listen to us for messages
			addListener(new DMessageCounter());
		}
		
		/// <summary> Build a DProtocol object from a the given socket connection.</summary>
		internal static DProtocol createFromSocket(System.Net.Sockets.TcpClient s)
		{
			// For performance reasons, it is very important that we setTcpNoDelay(true),
			// thus disabling Nagle's algorithm.  Google for TCP_NODELAY or Nagle
			// for more information.
			//
			// In addition, we now use a BufferedOutputStream instead of an OutputStream.
			//
			// These changes result in a huge speedup on the Mac.
			
			s.NoDelay = true;

            // We need to make sure that the socket is in blocking mode or BufferedStream
            // gets confused and screws up.
            s.ReceiveTimeout = 0;

			BufferedStream inStream = new BufferedStream(s.GetStream());
            BufferedStream outStream = new BufferedStream(s.GetStream());

            DProtocol dp = new DProtocol(inStream, outStream);
			return dp;
		}
		
		/// <summary> Allow outside entities to listen for incoming DMessages.  We
		/// make no presumptions about the ordering and do not filter
		/// anything at this level
		/// </summary>
		public virtual bool addListener(DProtocolNotifierIF n)
		{
			if (m_listeners.Count == 0)
			{
				m_listeners.Add(n);
			}
			else
			{
				// The DMessageCounter must always be the LAST listener, so that the message has
				// been fully processed before we wake up any threads that were waiting until a
				// message comes in.  So, insert this listener at the second-to-last position in
				// the list of listeners.
				m_listeners.Insert(m_listeners.Count - 1, n);
			}
			return true;
		}
		
		public virtual bool removeListener(DProtocolNotifierIF n)
		{
			m_listeners.Remove(n);
			return true;
		}
		
		public virtual long messagesReceived()
		{
			lock (this)
			{
				return m_msgRx;
			}
		}
		public virtual long messagesSent()
		{
			lock (this)
			{
				return m_msgTx;
			}
		}
		
		/// <summary> Entry point for our receive thread </summary>
		public virtual void  Run()
		{
			try
			{
				m_stopRx = false;
				listenForMessages();
			}
			catch (Exception ex)
			{
				if (Trace.error && !(ex is System.Net.Sockets.SocketException && ex.Message.ToUpper().Equals("socket closed".ToUpper())))
				// closed-socket is not an error //$NON-NLS-1$
				{
                    Console.Error.Write(ex.StackTrace);
                    Console.Error.Flush();
                }
			}
			
			/* notify our listeners that we are no longer listening;  game over */
			Object[] listeners = m_listeners.ToArray(); // copy the list to avoid multithreading problems
			for (int i = 0; i < listeners.Length; ++i)
			{
				DProtocolNotifierIF elem = (DProtocolNotifierIF) listeners[i];
				try
				{
					elem.disconnected();
				}
				catch (Exception exc)
				/* catch unchecked exceptions */
				{
                    if (Trace.error)
                    {
                        Console.Error.Write(exc.StackTrace);
                        Console.Error.Flush();
                    }
				}
			}
			
			// final notice that this thread is dead! 
			m_rxThread = null;
		}
		
		/// <summary> Create and start up a thread for our receiving messages.  </summary>
		public virtual bool bind()
		{
			/* create a new thread object for us which just listens to incoming messages */
			bool worked = true;
			if (m_rxThread == null)
			{
				MessageCounter.clearInCounts();
				MessageCounter.clearOutCounts();
				
				m_rxThread = new System.Threading.Thread(Run);
				m_rxThread.Name = "DJAPI message listener"; //$NON-NLS-1$
				m_rxThread.IsBackground = true;
				m_rxThread.Start();
			}
			else
				worked = false;
			
			return worked;
		}
		
		/// <summary> Shutdown our receive thread </summary>
		public virtual bool unbind()
		{
			bool worked = true;
			if (m_rxThread == null)
				worked = false;
			else
				m_stopRx = true;
			
			return worked;
		}
		
		/// <summary> Main rx loop which waits for commands and then issues them to anyone listening.</summary>
		internal virtual void  listenForMessages()
		{
			DProtocolNotifierIF[] listeners = new DProtocolNotifierIF[0];
			
			while (!m_stopRx)
			{
				/* read the data */
                try
                {
                    DMessage msg = rxMessage();

                    /* Now traverse our list of interested parties and let them deal with the message */
                    listeners = (DProtocolNotifierIF[])m_listeners.ToArray(typeof(DProtocolNotifierIF)); // copy the array to avoid multithreading problems
                    for (int i = 0; i < listeners.Length; ++i)
                    {
                        DProtocolNotifierIF elem = listeners[i];
                        try
                        {
                            elem.messageArrived(msg, this);
                        }
                        catch (Exception exc)
                        /* catch unchecked exceptions */
                        {
                            if (Trace.error)
                            {
                                Console.Error.WriteLine("Error in listener parsing incoming message :"); //$NON-NLS-1$
                                Console.Error.WriteLine(msg.inToString(16));
                                Console.Error.Write(exc.StackTrace);
                                Console.Error.Flush();
                            }
                        }
                        msg.reset(); /* allow others to reparse the message */
                    }

                    /* now dispose with the message */
                    DMessageCache.free(msg);
                }
                //catch (IOException e)
                //{
                //    // this is a healthy exception that we simply ignore, since it means we haven't seen
                //    // data for a while; is all.
                //}
                finally
                {
                }
			}
		}
		
		/// <summary> Transmit the message down the socket.
		/// 
		/// This function is not synchronized; it is only called from one place, which is
		/// PlayerSession.sendMessage().  That function is synchronized.
		/// </summary>
		internal virtual void  txMessage(DMessage message)
		{
			int size = message.Size;
			int command = message.Type;
			
			//System.out.println("txMessage: " + DMessage.outTypeName(command) + " size=" + size);
			
			writeDWord(size);
			writeDWord(command);
			writeData(message.Data, size);
			
			m_out.Flush();
			lock (this)
			{
				m_msgTx++;
			}
			MessageCounter.messageSent(message);
		}
		
		/// <summary> Get the next message on the input stream, using the context contained within 
		/// the message itself to demark its end
		/// </summary>
		private DMessage rxMessage()
		{
			long size = readDWord();
			long command = readDWord();
			
			//System.out.println("rxMessage: " + DMessage.inTypeName(command) + " size=" + size);
			
			if (size < 0 || command < 0)
				throw new IOException("socket closed"); //$NON-NLS-1$

			/* 
			* Ask our message cache for a message
			*/
			DMessage message = DMessageCache.alloc((int)size);
			byte[] messageContent = message.Data;
			int offset = 0;
			
			/* block until we get the entire message, which may come in pieces */
            while (offset < size)
            {
                int count = m_in.Read(messageContent, offset, (int)size - offset);
                //m_ReceivedLog.Add(messageContent, offset, count);
                offset += count;
            }
			
			/* now we have the data of the message, set its type and we are done */
			message.Type = (int)command;
			lock (this)
			{
				m_msgRx++;
			}
			return message;
		}
		
		internal virtual void  writeDWord(long dw)
		{
			byte b0 = (byte) (dw & 0xff);
			byte b1 = (byte) ((dw >> 8) & 0xff);
			byte b2 = (byte) ((dw >> 16) & 0xff);
			byte b3 = (byte) ((dw >> 24) & 0xff);
			
			m_out.WriteByte((byte) b0);
			m_out.WriteByte((byte) b1);
			m_out.WriteByte((byte) b2);
			m_out.WriteByte((byte) b3);
		}
		
		internal virtual void  writeData(byte[] data, long size)
		{
			if (size > 0)
				m_out.Write(data, 0, (int) size);
		}
		
		
		/// <summary> Extract the next 4 bytes, which form a 32b integer, from the stream</summary>
		internal virtual long readDWord()
		{
            int b0 = m_in.ReadByte(); // m_ReceivedLog.Add((Byte)b0);
            int b1 = m_in.ReadByte(); // m_ReceivedLog.Add((Byte)b1);
            int b2 = m_in.ReadByte(); // m_ReceivedLog.Add((Byte)b2);
            int b3 = m_in.ReadByte(); // m_ReceivedLog.Add((Byte)b3);

            if (b0 < 0 || b1 < 0 || b2 < 0 || b3 < 0)
            {
                return -1;
            }

            long value = (long)((uint)(byte)b3 << 24 | (uint)(byte)b2 << 16 | (uint)(byte)b1 << 8 | (uint)(byte)b0);

            return value;
		}
	}
}
