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
using System.IO;

using Flash.Swf.Debug;
using Flash.Swf.Types;
using Flash.Util;

namespace Flash.Swf
{
	
	/// <summary> The swd file format is as follows
	/// 
	/// swd(header) (tag)*
	/// 
	/// </summary>
	/// <author>  Edwin Smith
	/// </author>
	public class DebugDecoder
	{
		private class AnonymousClassDebugHandler : DebugHandler
		{
            public override void header(int version)
			{
				Console.Out.WriteLine("FWD" + version);
			}

            public override void uuid(FlashUUID id)
			{
				Console.Out.WriteLine("DebugID " + id);
			}

            public override void module(DebugModule dm)
			{
				Console.Out.WriteLine("DebugScript #" + dm.id + " " + dm.bitmap + " " + dm.name + " (nlines = " + (dm.offsets.Length - 1) + ")");
			}

            public override void offset(int offset, LineRecord lr)
			{
				Console.Out.WriteLine("DebugOffset #" + lr.module.id + ":" + lr.lineno + " " + offset);
			}

            public override void breakpoint(int offset)
			{
				Console.Out.WriteLine("DebugBreakpoint " + offset);
			}

            public override void registers(int offset, RegisterRecord r)
			{
				Console.Out.WriteLine("DebugRegisters " + r.ToString());
			}

            public override void error(String msg)
			{
				Console.Error.WriteLine("***ERROR: " + msg);
			}
		}
		virtual public byte[] TagData
		{
			set
			{
				in_Renamed = new SwfDecoder(value, 0);
			}
			
		}
		public const int kDebugScript = 0;
		public const int kDebugOffset = 1;
		public const int kDebugBreakpoint = 2;
		public const int kDebugID = 3;
		public const int kDebugRegisters = 5;
		
		/// <summary> table of line numbers, indexed by offset in the SWF file</summary>
		private SwfDecoder in_Renamed;
		private IntMap modules = new IntMap();
		
		public DebugDecoder(byte[] b):this(new MemoryStream(b))
		{
		}
		
		public DebugDecoder(Stream in_Renamed)
		{
			this.in_Renamed = new SwfDecoder(in_Renamed, 0);
		}
		
		public virtual void  readSwd(DebugHandler h)
		{
			readHeader(h);
			readTags(h);
		}
		
		internal virtual void  readHeader(DebugHandler handler)
		{
			byte[] sig = new byte[4];
			
			in_Renamed.readFully(sig);
			
			if (sig[0] != 'F' || sig[1] != 'W' || sig[2] != 'D' || sig[3] < 6)
			{
				throw new SwfFormatException("not a Flash 6 or later SWD file");
			}
			
			in_Renamed.swfVersion = sig[3];
			
			handler.header(in_Renamed.swfVersion);
		}
		
		public virtual void  readTags(DebugHandler handler)
		{
			System.Collections.ArrayList lineRecords = new System.Collections.ArrayList();
			
			do 
			{
				int tag = (int) in_Renamed.readUI32();
				switch (tag)
				{
					
					case kDebugScript: 
						DebugModule m = new DebugModule();
						int id = (int) in_Renamed.readUI32();
						m.id = id;
						m.bitmap = (int) in_Renamed.readUI32();
						m.name = in_Renamed.readString();
						m.Text = in_Renamed.readString();
						
						adjustModuleName(m);
						
						if (modules.contains(id))
						{
							DebugModule m2 = (DebugModule) modules.get_Renamed(id);
							if (!m.Equals(m2))
							{
								handler.error("Module '" + m2.name + "' has the same ID as Module '" + m.name + "'");
								handler.error("Let's check for kDebugOffset that came before Module '" + m2.name + "'");
								handler.error("Before: Number of accumulated line records: " + lineRecords.Count);
								lineRecords = purgeLineRecords(lineRecords, id, handler);
								handler.error("After: Number of accumulated line records: " + lineRecords.Count);
							}
						}
						modules.put(id, m);
						handler.module(m);
						break;
					
					case kDebugOffset: 
						id = (int) in_Renamed.readUI32();
						int lineno = (int) in_Renamed.readUI32();
						DebugModule module = (DebugModule) modules.get_Renamed(id);
						LineRecord lr = new LineRecord(lineno, module);
						int offset = (int) in_Renamed.readUI32();
						
						if (module != null)
						{
							// not corrupted before we add the offset and offset add fails
							bool wasCorrupt = module.corrupt;
							if (!module.addOffset(lr, offset) && !wasCorrupt)
								handler.error(module.name + ":" + lineno + " does not exist for offset " + offset + ", module marked for exclusion from debugging");
							handler.offset(offset, lr);
						}
						else
						{
							lineRecords.Add((System.Int32) id);
							lineRecords.Add(lr);
							lineRecords.Add((System.Int32) offset);
						}
						break;
					
					case kDebugBreakpoint: 
						handler.breakpoint((int) in_Renamed.readUI32());
						break;
					
					case kDebugRegisters: 
					{
						offset = (int) in_Renamed.readUI32();
						int size = in_Renamed.readUI8();
						RegisterRecord r = new RegisterRecord(offset, size);
						for (int i = 0; i < size; i++)
						{
							int nbr = in_Renamed.readUI8();
							String name = in_Renamed.readString();
							r.addRegister(nbr, name);
						}
						handler.registers(offset, r);
						break;
					}
					
					
					case kDebugID: 
						FlashUUID uuid = new FlashUUID();
						in_Renamed.readFully(uuid.bytes);
						handler.uuid(uuid);
						break;
					
					case - 1: 
						break;
					
					default: 
						throw new SwfFormatException("Unexpected tag id " + tag);
					
				}
				
				if (tag == - 1)
				{
					break;
				}
			}
			while (true);
			
			int i2 = 0, size2 = lineRecords.Count;
			while (i2 < size2)
			{
				int id = ((System.Int32) lineRecords[i2]);
				LineRecord lr = (LineRecord) lineRecords[i2 + 1];
				int offset = ((System.Int32) lineRecords[i2 + 2]);
				lr.module = (DebugModule) modules.get_Renamed(id);
				
				if (lr.module != null)
				{
					//System.out.println("updated module "+id+" out of order");
					// not corrupted before we add the offset and offset add fails
					bool wasCorrupt = lr.module.corrupt;
					if (!lr.module.addOffset(lr, offset) && !wasCorrupt)
						handler.error(lr.module.name + ":" + lr.lineno + " does not exist for offset " + offset + ", module marked for exclusion from debugging");
					
					handler.offset(offset, lr);
				}
				else
				{
					handler.error("Could not find debug module (id = " + id + ") for offset = " + offset);
				}
				
				i2 += 3;
			}
		}
		
		/// <summary> process any dangling line records that belong to the given module</summary>
		/// <param name="lineRecords">
		/// </param>
		/// <param name="moduleId">
		/// </param>
		/// <param name="handler">
		/// </param>
		/// <returns>
		/// </returns>
		private System.Collections.ArrayList purgeLineRecords(System.Collections.ArrayList lineRecords, int moduleId, DebugHandler handler)
		{
			System.Collections.ArrayList newLineRecords = new System.Collections.ArrayList();
			DebugModule module = (DebugModule) modules.get_Renamed(moduleId);
			int i = 0, size = lineRecords.Count;
			while (i < size)
			{
				System.Int32 id = (System.Int32) lineRecords[i];
				LineRecord lr = (LineRecord) lineRecords[i + 1];
				System.Int32 offset = (System.Int32) lineRecords[i + 2];
				
				if (id == moduleId)
				{
					lr.module = module;
					
					if (lr.module != null)
					{
						lr.module.addOffset(lr, offset);
						handler.offset(offset, lr);
					}
					else
					{
						handler.error("Could not find kDebugScript with module ID = " + id);
					}
				}
				else
				{
					newLineRecords.Add(id);
					newLineRecords.Add(lr);
					newLineRecords.Add(offset);
				}
				
				i += 3;
			}
			
			return newLineRecords;
		}
		
		[STAThread]
		public static void  Main(String[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				Stream inStream = new FileStream(args[i], FileMode.Open, FileAccess.Read);
				try
				{
                    new DebugDecoder(inStream).readSwd(new AnonymousClassDebugHandler());
					System.Console.Out.WriteLine();
				}
				finally
				{
                    inStream.Close();
				}
			}
		}
		
		/// <summary> Royale Enhancement Request: 53160...
		/// 
		/// If a debug module represents an AS2 class, the module name should be in the form of classname: fileURL
		/// Matador uses classname: absolutePath (note: absolute, not cannonical)
		/// 
		/// </summary>
		/// <param name="d">
		/// </param>
		protected internal static void  adjustModuleName(DebugModule d)
		{
			d.name = adjustModuleName(d.name);
		}
		
		public static String adjustModuleName(String name)
		{
			if (name.StartsWith("<") && name.EndsWith(">"))
			{
				return name;
			}
			
			String token1, token2;
			
			try
			{
				System.Uri u = new System.Uri(name);
				// good URL, return...
				return name;
			}
			catch (UriFormatException)
			{
				// not an URL, continue...
			}
			
			FileInfo f;
			
			try
			{
				f = new FileInfo(name);
			}
			catch (ApplicationException)
			{
				// the CLR will throw this when a java.io.File object is init'd in a location
				// that causes a .NET System.SecurityException - can't create File objects on
				// .NET as a way of testing whether they are valid files without catching the error
				f = null;
			}
			
			if (f == null || !File.Exists(f.FullName))
			{
				int colon = name.IndexOf(':');
				if (colon != - 1)
				{
					token1 = name.Substring(0, (colon) - (0)).Trim();
					token2 = name.Substring(colon + 1).Trim();
				}
				else
				{
					token1 = "";
					token2 = name;
				}
			}
			else
			{
				token1 = "";
				token2 = name;
			}
			
			try
			{
				f = new FileInfo(token2);
			}
			catch (ApplicationException)
			{
				// the CLR will throw this when a java.io.File object is init'd in a location
				// that causes a .NET System.SecurityException - can't create File objects on
				// .NET as a way of testing whether they are valid files without catching the error
				f = null;
			}
			
			if (f != null && File.Exists(f.FullName))
			{
				try
				{
					if (token2.IndexOf("..") != - 1 || token2.IndexOf(".") != - 1)
					{
						f = FileUtils.getCanonicalFile(f);
					}
					token2 = FileUtils.toURL(f).ToString();
				}
				catch (IOException)
				{
				}
			}
			
			if (token1.Length == 0)
			{
				name = token2;
			}
			else
			{
				name = token1.Trim() + ": " + token2.Trim();
			}
			
			return name;
		}
	}
}
