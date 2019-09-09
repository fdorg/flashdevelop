// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2005-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System.IO;

namespace Flash.Tools.Debugger.Concrete
{
	
	/// <summary> Reads a stream, and sends the contents somewhere.</summary>
	/// <author>  mmoreart
	/// </author>
	public class StreamListener
	{
		internal TextReader m_fIn;
		internal TextWriter m_fOut;
		internal System.Threading.Thread m_Thread;
		
		/// <summary> Creates a StreamListener which will copy everything from
		/// 'in' to 'out'.
		/// </summary>
		/// <param name="reader">the stream to read
		/// </param>
		/// <param name="writer">the stream to write to, or 'null' to discard input
		/// </param>
		public StreamListener(TextReader reader, TextWriter writer)
		{
			m_Thread = new System.Threading.Thread(Run);
			m_Thread.IsBackground = true;
			m_Thread.Name = "DJAPI StreamListener";
			m_fIn = reader;
			m_fOut = writer;
		}
		
		public void Run()
		{
			char[] buf = new char[4096];
			
			try
			{
				for ( ; ; )
				{
					int count = m_fIn.Read(buf, 0, buf.Length);
					if (count == 0)
						return; // thread is done

					if (m_fOut != null)
					{
						try
						{
							m_fOut.Write(buf, 0, count);
						}
						catch (IOException)
						{
							// the write failed (unlikely), but we still
							// want to keep reading
						}
					}
				}
			}
			catch (IOException)
			{
				// do nothing -- we're done
			}
		}

		public void Start()
		{
			m_Thread.Start();
		}
	}
}
