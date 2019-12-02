// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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

namespace Flash.Tools.Debugger.Concrete
{
	public class ProcessListener
	{
		internal PlayerSessionManager m_Manager;
		internal System.Diagnostics.Process m_Process;
		internal volatile bool m_bDone;
		internal System.Threading.Thread m_Thread;
		
		public ProcessListener(PlayerSessionManager mgr, System.Diagnostics.Process process)
		{
			m_bDone = false;
			m_Manager = mgr;
			m_Process = process;
			m_Thread = new System.Threading.Thread(Run);
			m_Thread.Name = "DJAPI ProcessListener";
			m_Thread.IsBackground = true;
		}
		
		public void Run()
		{
			while (!m_bDone)
			{
				try
				{
					m_Process.WaitForExit();
					System.Int32 generatedAux = m_Process.ExitCode;
					m_bDone = true;
					m_Manager.ProcessDead = m_Process.ExitCode;
				}
				catch (System.Threading.ThreadInterruptedException)
				{
					// this will happen if finish() calls Thread.interrupt()
				}
			}
		}

		public virtual void Start()
		{
			m_Thread.Start();
		}

		public virtual void Finish()
		{
			m_bDone = true;
			m_Thread.Interrupt(); // wake up the listening thread
		}
	}
}
