// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
	
	/// <summary> This cache directly manages the creation/destruction of DMessages
	/// by allowing DMessages to be re-used.
	/// 
	/// It has been observed that the Player send a tremendous number of
	/// small (&lt; 8Byte of data) messages and that by allocating a fixed
	/// number of these, and then re-using them, we can assist the garbage
	/// collector greatly.
	/// 
	/// The cache is arranged as an array whereby DMessages with 'index'
	/// number of bytes for data are housed.  It is asssumed that at
	/// any moment in time only one DMessage will be required and thus
	/// this technique works.  If DMessages are to be stored for 
	/// later processing (implying that many will exist at any moment)
	/// then we need to implement a more sophisticated cache (probably
	/// storing a Vector of DMessages at each index).
	/// 
	/// Very large DMessages are currently not cached.
	/// 
	/// This is class is a singleton.
	/// </summary>
	public class DMessageCache
	{
		public const int MAX_CACHED_DATA_SIZE = 128; /* should consume around 4n + n(n+1)/2 bytes */
		
		/* our cache */
		internal static DMessage[] m_cache = new DMessage[MAX_CACHED_DATA_SIZE];
		
		/// <summary> Obtain a DMessage from the cache if possible, otherwise make one for me.</summary>
		public static DMessage alloc(int size)
		{
			DMessage msg;
			
			int index = size2Index(size);
			
			/*
			* We see if this could possibly be found in our cache,
			* if so, then see if there is one for us to use,
			* otherwise create a new one 
			*/
			if (index < 0)
				msg = new DMessage(size);
			else if (m_cache[index] == null)
				msg = new DMessage(size);
			else
			{
				msg = m_cache[index];
				m_cache[index] = null;
			}
			
			//		System.out.println("msgsize="+size+uft());
			return msg;
		}

#if false
		private static String uft()
		{
			System.Diagnostics.Process rt = System.Diagnostics.Process.GetCurrentProcess();
			long free = rt.freeMemory(), total = rt.totalMemory(), used = total - free;
			//		long max = rt.maxMemory();
			SupportClass.TextNumberFormat nf = SupportClass.TextNumberFormat.getTextNumberInstance();
			//        System.out.println("used: "+nf.format(used)+" free: "+nf.format(free)+" total: "+nf.format(total)+" max: "+nf.format(max));
			return ", used " + nf.FormatLong(used) + ", free " + nf.FormatLong(free) + ", total " + nf.FormatLong(total); //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
		}
#endif

		/// <summary> Put a DMessage into the cache for reuse</summary>
		public static void  free(DMessage msg)
		{
			int index = size2Index(msg.Size);
			
			msg.clear(); /* clear stuff up for re-use */
			
			/* 
			* If it is too big we don't store cache, assuming
			* the GC can do a better job than us at reusing the memory,
			* Otherwise we put it in our cache
			*/
			if (index < 0)
			{
			}
			else if (m_cache[index] != null)
			{
			}
			/* bad => need to use a Vector in the array to house multiple DMessages */
			else
				m_cache[index] = msg;
		}
		
		public static int size2Index(int size)
		{
			return ((size < MAX_CACHED_DATA_SIZE)?size:- 1);
		}
		//	public static int size2Index(int size) { return -1; }
	}
}