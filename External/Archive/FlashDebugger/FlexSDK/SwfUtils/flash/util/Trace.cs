// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
using SystemProperties = JavaCompatibleClasses.SystemProperties;

namespace Flash.Util
{
	
	/// <summary> Primitive run-time tracing class
	/// 
	/// Code as follows:
	/// if (Trace.foo)
	/// Trace.trace("trace msg"...);
	/// 
	/// Enable as follows:
	/// java -Dtrace.foo -Dtrace.foo2 -Dtrace.foo3 or -Dtrace.all
	/// 
	/// Special flags:
	/// -Dtrace.flex                -- enables all tracing
	/// -Dtrace.foo                   -- enables tracing on foo subsystem
	/// -Dtrace.timeStamp             -- timeStamp all output lines
	/// -Dtrace.caller                -- print the Class:method caller
	/// -Dtrace.stackLines=10         -- print 10 stack lines
	/// -Dtrace.stackPrefix=java.lang -- print the stack up to java.lang
	/// 
	/// Add new xxx members as desired.
	/// </summary>
	public class Trace
	{
		public static readonly bool all = (SystemProperties.getProperty("trace.flex") != null);
		//public static final boolean asc = all || (System.getProperty("trace.asc") != null);
		public static readonly bool accessible = all || (SystemProperties.getProperty("trace.accessible") != null);
		public static readonly bool asdoc = all || (SystemProperties.getProperty("trace.asdoc") != null);
		//public static final boolean benchmark = all || (System.getProperty("trace.benchmark") != null);
		public static readonly bool cache = all || (SystemProperties.getProperty("trace.cache") != null);
		//public static final boolean compileTime = all || (System.getProperty("trace.compileTime") != null);
		public static readonly bool css = all || (SystemProperties.getProperty("trace.css") != null);
		public static readonly bool dependency = all || (SystemProperties.getProperty("trace.dependency") != null);
		public static readonly bool config = all || (SystemProperties.getProperty("trace.config") != null);
		public static readonly bool embed = all || (SystemProperties.getProperty("trace.embed") != null);
		public static readonly bool error = all || (SystemProperties.getProperty("trace.error") != null);
		public static readonly bool font = all || (SystemProperties.getProperty("trace.font") != null);
		public static readonly bool font_cubic = all || (SystemProperties.getProperty("trace.font.cubic") != null);
		//public static final boolean image = all || (System.getProperty("trace.image") != null);
		//public static final boolean lib = all || (System.getProperty("trace.lib") != null);
		public static readonly bool license = all || (SystemProperties.getProperty("trace.license") != null);
		//public static final boolean linker = all || (System.getProperty("trace.linker") != null);
		public static readonly bool mxml = all || (SystemProperties.getProperty("trace.mxml") != null);
		//public static final boolean parser = all || (System.getProperty("trace.parser") != null);
		public static readonly bool profiler = all || (SystemProperties.getProperty("trace.profiler") != null);
		//public static final boolean schema = all || (System.getProperty("trace.schema") != null);
		public static readonly bool swc = all || (SystemProperties.getProperty("trace.swc") != null);
		//public static final boolean swf = all || (System.getProperty("trace.swf") != null);
		public static readonly bool pathResolver = all || (SystemProperties.getProperty("trace.pathResolver") != null);
		public static readonly bool binding = all || (SystemProperties.getProperty("trace.binding") != null);
		
		// print just the stack caller
		public static readonly bool caller = (SystemProperties.getProperty("trace.caller") != null);
		// print stack up to the prefix
		public static readonly String stackPrefix = SystemProperties.getProperty("trace.stackPrefix");
		
		// print this number of stack lines
		public static int stackLines = 0;
		// print a timestamp with each line
		public static readonly bool timeStamp = (SystemProperties.getProperty("trace.timeStamp") != null);
		
		// print debug information related to the swc-checksum option
		public static readonly bool swcChecksum = all || (SystemProperties.getProperty("trace.swcChecksum") != null);
		
		/// <summary> Write the string as a line to the trace stream. If the
		/// "stack" property is enabled, then the caller's stack call
		/// is also shown in the date.
		/// </summary>
		public static void  trace(String str)
		{
			if (timeStamp)
			{
				Console.Error.Write(DateTime.Now.ToString("r"));
			}
			
			if (caller)
			{
                Console.Error.Write(new System.Diagnostics.StackTrace(1, true).ToString());
			}
			
			Console.Error.WriteLine(str);

#if false
			if (stackLines > 0)
			{
				Console.Error.WriteLine(ExceptionUtil.getStackTraceLines(new Exception(), stackLines));
			}
			else if (stackPrefix != null)
			{
				//UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to 'System.Exception' which has different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
				Console.Error.WriteLine(ExceptionUtil.getStackTraceUpTo(new Exception(), stackPrefix));
			}
#endif
		}
		static Trace()
		{
			{
                try
                {
                    stackLines = Int32.Parse(SystemProperties.getProperty("trace.stackLines"));
                }
                catch (FormatException)
                {
                }
                catch (ArgumentNullException)
                {
                }
			}
		}
	}
}
