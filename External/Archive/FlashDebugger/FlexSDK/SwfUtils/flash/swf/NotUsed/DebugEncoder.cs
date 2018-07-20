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
using DebugModule = flash.swf.debug.DebugModule;
using LineRecord = flash.swf.debug.LineRecord;
using RegisterRecord = flash.swf.debug.RegisterRecord;
using FlashUUID = flash.swf.types.FlashUUID;
namespace flash.swf
{
	
	/// <summary> Encoder for writing out SWD files in a canonical order.
	/// 
	/// </summary>
	/// <author>  Edwin Smith
	/// </author>
	/// <author>  Gordon Smith
	/// 
	/// The MXML compiler uses DebugEncoder to output a SWD file
	/// containing debugging information for use by fdb.
	/// 
	/// The compiler incrementally feeds debugging data to this class
	/// by calling its offset() method. The debugging data is stored
	/// in DebugScript, DebugOffset, and DebugBreakpoint objects
	/// so that it can be massaged and sorted into a canonical order
	/// appropriate for fdb before being serialized as a SWD.
	/// 
	/// The organization of a canonical Royale SWD is as follows:
	/// 
	/// The kDebugID tag follows immediately after the SWD header.
	/// 
	/// The kDebugScript tags appear in the order in which fdb
	/// wants to display them:
	/// - first the main MXML file for the application,
	/// with its "bitmap" field set to 1;
	/// - then other authored files in alphabetical order, with bitmap=2;
	/// - then framework files in alphabetical order, with bitmap=3;
	/// - then pairs of synthetic files for the various components
	/// in alphabetical order, with bitmap=4;
	/// - finally the synthetic "frame 1 actions" of the application,
	/// with bitmap=5.
	/// 
	/// Alphabetical order refers to the short fdb display name
	/// ("Button.as"), not the complete module name
	/// ("mx.controls.Button: C:\Royale\...\mx\controls\Button.as").
	/// 
	/// The synthetic modules get renamed to <MyComponent.1>,
	/// <MyComponent.2>, and <main>.
	/// 
	/// After this sorting by name, the script id numbers are renumbered
	/// to be sequential, starting with 1.
	/// 
	/// Immediately after each kDebugScript tag come the kDebugOffset tags
	/// that reference that kDebugScript, ordered by line number and
	/// subordered by byte offset.
	/// 
	/// Royale emits no kDebugBreakpoint tags in its SWDs, but if it did
	/// they would come last, ordered by offset.
	/// 
	/// </author>
	public class DebugEncoder : DebugHandler
	{
		virtual public System.String MainDebugScript
		{
			set
			{
				mainDebugScriptName = value;
			}
			
		}
		/*
		* Storage for the information that DebugEncoder
		* will encode into the header of the SWD file.
		* This is set when a client of DebugEncoder calls header().
		*/
		private int version;
		
		/*
		* Storage for the information that DebugEncoder
		* will encode into a kDebugID tag in the SWD file.
		* This is set when a client of DebugEncoder calls uuid() or updateUUID().
		*/
		private sbyte[] debugID;
		
		/*
		* A collection of DebugScript objects to be encoded into kDebugScript
		* tags in the SWD file. This collection is built by repeated calls
		* to offset() by clients of DebugEncoder. (Clients can also call module()
		* directly, but the MXML compiler only calls offset().)
		*/
		private System.Collections.IList debugScripts;
		
		//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'DebugScript' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
		/*
		* DebugScript objects are used only by this DebugEncoder class.
		* Each DebugScript object contains the information that DebugEncoder
		* will encode into one kDebugScript tag in the SWD file.
		*/
		private class DebugScript : System.IComparable
		{
			private void  InitBlock(DebugEncoder enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private DebugEncoder enclosingInstance;
			public DebugEncoder Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			internal int bitmap;
			internal System.String name;
			internal System.String text;
			
			/*
			* A collection of DebugOffset objects to be encoded into kDebugOffset
			* tags in the SWD file. This collection is built by repeated calls
			* to offset() by clients of DebugEncoder. Since each DebugOffset
			* is associated with one DebugScript, they are attached to their
			* corresponding DebugScript rather than directly to the DebugEncoder.
			*/
			internal System.Collections.IList debugOffsets;
			
			internal System.String comparableName;
			
			internal DebugScript(DebugEncoder enclosingInstance, System.String name, System.String text)
			{
				InitBlock(enclosingInstance);
				this.bitmap = 0;
				this.name = name;
				this.text = text;
				
				this.debugOffsets = new System.Collections.ArrayList();
			}
			
			/// <summary> Implement Comparable interface for sorting DebugScripts</summary>
			public virtual int CompareTo(System.Object o)
			{
				DebugScript other = (DebugScript) o;
				return String.CompareOrdinal(comparableName, other.comparableName);
			}
		}
		
		//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'DebugOffset' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
		/*
		* DebugOffset objects are used only by this DebugEncoder class.
		* Each DebugOffset object stores the information that DebugEncoder
		* will encode into one kDebugOffset tag in the SWD file.
		* They are created by repeated calls to offset() by a client
		* of DebugEncoder.
		*/
		private class DebugOffset : System.IComparable
		{
			private void  InitBlock(DebugEncoder enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private DebugEncoder enclosingInstance;
			public DebugEncoder Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			internal DebugOffset(DebugEncoder enclosingInstance, int lineNumber, int byteOffset)
			{
				InitBlock(enclosingInstance);
				this.lineNumber = lineNumber;
				this.byteOffset = byteOffset;
			}
			internal int lineNumber;
			internal int byteOffset;
			
			/// <summary> Implement Comparable interface for sorting DebugOffsets.</summary>
			public virtual int CompareTo(System.Object o)
			{
				DebugOffset other = (DebugOffset) o;
				long a = (((long) lineNumber) << 32) | byteOffset;
				long b = (((long) other.lineNumber) << 32) | other.byteOffset;
				if (a < b)
					return - 1;
				else if (a > b)
					return 1;
				else
					return 0;
			}
		}
		
		/*
		* A name -> DebugScript map used to quickly determine whether we
		* have already created a DebugScript with a particular name.
		* Note: We can't use the id in the DebugModule passed to offset()
		* for this purpose, because it isn't unique! All the MXML files
		* for the application and its components come in with id 0!
		*/
		//UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
		private System.Collections.Hashtable debugScriptsByName;
		
		/*
		* A collection of DebugBreakpoint objects to be encoded into
		* kDebugBreakpoint tags in the SWD file. This collection is built
		* by repeated calls to breakpoint() by clients of DebugEncoder.
		*/
		private System.Collections.IList debugBreakpoints;
		
		//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'DebugBreakpoint' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
		/*
		* DebugOffset objects are used only by this DebugEncoder class.
		* Each DebugBreakpoint object stores the information that DebugEncoder
		* will encode into one kDebugBreakpoint tag in the SWD file.
		*/
		private class DebugBreakpoint : System.IComparable
		{
			private void  InitBlock(DebugEncoder enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private DebugEncoder enclosingInstance;
			public DebugEncoder Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			internal DebugBreakpoint(DebugEncoder enclosingInstance, int offset)
			{
				InitBlock(enclosingInstance);
				this.offset = offset;
			}
			
			internal int offset;
			
			/// <summary> Implement Comparable interface for sorting DebugBreakpoints.</summary>
			public virtual int CompareTo(System.Object o)
			{
				DebugBreakpoint other = (DebugBreakpoint) o;
				//UPGRADE_NOTE: Exceptions thrown by the equivalent in .NET of method 'java.lang.Integer.compareTo' may be different. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1099'"
				return ((System.Int32) offset).CompareTo((System.Int32) other.offset);
			}
		}
		
		/*
		* A collection of DebugRegister objects to be encoded into
		* kDebugRegister tags in the SWD file. This collection is built
		* by repeated calls to registers() by clients of DebugEncoder.
		*/
		private System.Collections.IList debugRegisters;
		
		//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'DebugRegisters' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
		/*
		* DebugRegisters objects are used only by this DebugEncoder class.
		* Each DebugRegisters object stores the information that DebugEncoder
		* will encode into one kDebugRegisters tag in the SWD file.
		*/
		private class DebugRegisters : System.IComparable
		{
			private void  InitBlock(DebugEncoder enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private DebugEncoder enclosingInstance;
			public DebugEncoder Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			internal DebugRegisters(DebugEncoder enclosingInstance, int offset, RegisterRecord r)
			{
				InitBlock(enclosingInstance);
				this.offset = offset;
				this.registerNumbers = r.registerNumbers;
				this.variableNames = r.variableNames;
			}
			
			internal int offset;
			internal int[] registerNumbers;
			internal System.String[] variableNames;
			
			/// <summary> Implement Comparable interface for sorting DebugRegisters.</summary>
			public virtual int CompareTo(System.Object o)
			{
				DebugRegisters other = (DebugRegisters) o;
				//UPGRADE_NOTE: Exceptions thrown by the equivalent in .NET of method 'java.lang.Integer.compareTo' may be different. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1099'"
				return ((System.Int32) offset).CompareTo((System.Int32) other.offset);
			}
		}
		
		/*
		* The MXML compiler sets this by calling setMainDebugScript()
		* in order to specify which DebugScript should be first in the SWD,
		* with id 1. fdb can then make this script the initial current script
		* for a debugging session.
		*/
		private System.String mainDebugScriptName;
		
		/*
		* A public property of DebugEncoder used by clients to adjust
		* the bytecode offsets for subsequent calls to offset().
		* (From relative-to-beginning-of-byte-code-for-one-module
		* to relative-to-beginning-of-SWD-file?)
		*/
		public int adjust;
		
		public DebugEncoder()
		{
			debugScripts = new System.Collections.ArrayList();
			//UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
			debugScriptsByName = new System.Collections.Hashtable();
			debugBreakpoints = new System.Collections.ArrayList();
			debugRegisters = new System.Collections.ArrayList();
		}
		
		public virtual void  header(int version)
		{
			this.version = version;
		}
		
		public virtual void  uuid(FlashUUID uuid)
		{
			debugID = uuid.bytes;
		}
		
		internal virtual void  updateUUID(sbyte[] uuid)
		{
			debugID = uuid;
		}
		
		public virtual void  offset(int offset, LineRecord lr)
		{
			//System.out.print(lr.module.id + " " + lr.module.name + " " + lr.lineno + " " + offset + "\n");
			
			// NOTE: Each DebugModule coming in to this method via the
			// LineRecord doesn't have a unique id!
			// In particular, lr.module.id is 0 for all MXML files.
			// And the others are generated by a random number generator
			// that could conceivably repeat.
			// Therefore there is no point at even looking at the id.
			
			// Module name strings arrive here in various formats:
			// An MXML file (application or component) is a full path like
			// "C:\Royale\flash\experience\royale\apps\dev.war\checkinTest\checkinTest.mxml".
			// An ActionScript component or a framework package combines
			// a package name with a full path, as in
			// "custom.as.myBox: C:\Royale\flash\experience\royale\apps\dev.war\checkinTest\custom\as\myBox.as"
			// or
			// "mx.core.UIComponent: C:\Royale\flash\experience\royale\apps\dev.war\WEB-INF\flex\frameworks\mx\core\UIComponent.as".
			// A framework ActionScript file is a package an
			// Various autogenerated modules look like this:
			// "synthetic: checkinTest";
			// "synthetic: Object.registerClass() for checkinTest";
			// "synthetic: main frame 1 actions".
			// #include files may have non-canonical full paths like
			// "C:\Royale\flash\experience\royale\apps\dev.war\WEB-INF\flex\frameworks\mx\core\..\core\ComponentVersion.as"
			// and must be canonicalized to
			// "C:\Royale\flash\experience\royale\apps\dev.war\WEB-INF\flex\frameworks\mx\core\ComponentVersion.as"
			// so that they don't show up multiple times in an fdb file listing.
			
			/* C: The debug module name conversion is now centralized in DebugDecoder.adjustModuleName().
			if (lr.module.name.indexOf(": ") < 0)
			{
			lr.module.name = FileUtils.canonicalPath(lr.module.name);
			//System.out.print("*** " + lr.module.name + "\n");
			}
			*/
			
			// Don't bother to record corrupted modules
			if (lr.module.corrupt)
				return ;
			
			// If we haven't already created a DebugScript for the
			// module referenced by the specified LineRecord, do so.
			System.String name = lr.module.name;
			if (!debugScriptsByName.ContainsKey(name))
			{
				module(lr.module);
			}
			
			// Get the DebugScript for this script.
			//UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
			DebugScript script = (DebugScript) debugScriptsByName[name];
			
			// Create a DebugOffset for the specified lineNumber/byteOffset pair.
			DebugOffset debugOffset = new DebugOffset(this, lr.lineno, offset + adjust);
			
			// Attach the DebugOffset to the DebugScript it is associated with.
			script.debugOffsets.Add(debugOffset);
		}
		
		public virtual void  module(DebugModule m)
		{
			if (m.corrupt)
				return ;
			
			DebugScript script = new DebugScript(this, m.name, m.text);
			debugScripts.Add(script);
			debugScriptsByName[script.name] = script;
		}
		
		public virtual void  breakpoint(int offset)
		{
			debugBreakpoints.Add((System.Int32) offset);
		}
		
		public virtual void  registers(int offset, RegisterRecord r)
		{
			// Create a DebugRegister for the specified registers/byteOffset pair.
			DebugRegisters debug = new DebugRegisters(this, offset + adjust, r);
			debugRegisters.Add(debug);
		}
		
		private static System.String generateShortName(System.String name)
		{
			System.String s = name;
			
			/* do we have a file name? */
			int dotAt = name.LastIndexOf('.');
			if (dotAt != - 1)
			{
				/* yes let's strip the directory off */
				//UPGRADE_WARNING: Method 'java.lang.String.lastIndexOf' was converted to 'System.String.LastIndexOf' which may throw an exception. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
				int lastSlashAt = name.LastIndexOf((System.Char) System.IO.Path.DirectorySeparatorChar, dotAt);
				if (lastSlashAt == - 1 && System.IO.Path.DirectorySeparatorChar == '\\')
				{
					//UPGRADE_WARNING: Method 'java.lang.String.lastIndexOf' was converted to 'System.String.LastIndexOf' which may throw an exception. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
					lastSlashAt = name.LastIndexOf('/', dotAt);
				}
				s = name.Substring(lastSlashAt + 1);
			}
			else
			{
				/* not a file name ... */
				s = name;
			}
			return s.Trim();
		}
		
		private void  fixNamesAndBitmaps()
		{
			System.String synthetic = "synthetic: ";
			System.String actions = "Actions for ";
			
			System.Collections.IEnumerator debugScriptIter = debugScripts.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (debugScriptIter.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				DebugScript debugScript = (DebugScript) debugScriptIter.Current;
				if (isFrameworkClass(debugScript.name))
				{
					// bitmap = 3 => Framework file
					debugScript.bitmap = 3;
				}
				else if (debugScript.name.StartsWith(synthetic))
				{
					// bitmap = 4 => Other per-component synthetic files
					// produced by MXML compiler
					debugScript.bitmap = 4;
					
					System.String lookFor = "synthetic: Object.registerClass() for ";
					if (debugScript.name.StartsWith(lookFor))
					{
						System.String componentName = debugScript.name.Substring(lookFor.Length);
						debugScript.name = "<" + componentName + ".2>";
					}
					else
					{
						// R: should really check for a collision here...
						System.String componentName = debugScript.name.Substring(synthetic.Length);
						debugScript.name = "<" + componentName + ".1>";
					}
				}
				else if (debugScript.name.StartsWith(actions))
				{
					// bitmap = 5 => Actions ...
					debugScript.bitmap = 5;
				}
				else if (debugScript.name.Equals(mainDebugScriptName))
				{
					// bitmap = 1 => Main MXML file for application
					debugScript.bitmap = 1;
				}
				else
				{
					// bitmap = 2 => Other file, presumably an MXML or AS file
					// written by the application author
					debugScript.name = DebugDecoder.adjustModuleName(debugScript.name);
					debugScript.bitmap = 2;
				}
				
				// Set the comparableName field of each DebugScript
				// to the concatenation of the bitmap and the "short name" that fdb uses.
				// This will ensure that DebugScripts are sorted alphabetically
				// within each "bitmap" category.
				debugScript.comparableName = ((System.Int32) debugScript.bitmap).ToString() + generateShortName(debugScript.name);
				//            System.out.print(debugScript.comparableName + " " + debugScript.name);
			}
		}
		
		private void  encodeSwdData(SwfEncoder buffer)
		{
			// Encode the header.
			buffer.write32(('F') | ('W' << 8) | ('D' << 16) | (version << 24));
			
			// Encode one kDebugID tag.
			buffer.write32(flash.swf.DebugTags_Fields.kDebugID);
			buffer.write(debugID);
			
			// Encode the kDebugScript and kDebugOffset tags.
			// The kDebugScript tags are in module number order (1,2,3,...).
			// After each one of these are the associated kDebugOffset tags
			// for that module number, in ascending order
			// by line number and byte offset.
			
			SupportClass.CollectionsSupport.Sort(debugScripts, null);
			int id = 0;
			System.Collections.IEnumerator debugScriptIter = debugScripts.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (debugScriptIter.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				DebugScript debugScript = (DebugScript) debugScriptIter.Current;
				id++;
				
				buffer.write32(flash.swf.DebugTags_Fields.kDebugScript);
				buffer.write32(id);
				buffer.write32(debugScript.bitmap);
				buffer.writeString(debugScript.name);
				buffer.writeString(debugScript.text);
				
				SupportClass.CollectionsSupport.Sort(debugScript.debugOffsets, null);
				System.Collections.IEnumerator debugOffsetIter = debugScript.debugOffsets.GetEnumerator();
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
				while (debugOffsetIter.MoveNext())
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
					DebugOffset debugOffset = (DebugOffset) debugOffsetIter.Current;
					
					buffer.write32(flash.swf.DebugTags_Fields.kDebugOffset);
					buffer.write32(id);
					buffer.write32(debugOffset.lineNumber);
					buffer.write32(debugOffset.byteOffset);
				}
			}
			
			// Encode the kDebugRegister tags
			SupportClass.CollectionsSupport.Sort(debugRegisters, null);
			System.Collections.IEnumerator itr = debugRegisters.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (itr.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				DebugRegisters debug = (DebugRegisters) itr.Current;
				int size = debug.registerNumbers.Length;
				
				buffer.write32(flash.swf.DebugTags_Fields.kDebugRegisters);
				buffer.write32(debug.offset);
				buffer.writeUI8(size);
				for (int i = 0; i < debug.registerNumbers.Length; i++)
				{
					buffer.writeUI8(debug.registerNumbers[i]);
					buffer.writeString(debug.variableNames[i]);
				}
			}
			
			// Encode the kDebugBreakpoint tags
			SupportClass.CollectionsSupport.Sort(debugBreakpoints, null);
			System.Collections.IEnumerator debugBreakpointIterator = debugBreakpoints.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (debugBreakpointIterator.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				DebugBreakpoint debugBreakpoint = (DebugBreakpoint) debugBreakpointIterator.Current;
				
				buffer.write32(flash.swf.DebugTags_Fields.kDebugBreakpoint);
				buffer.write32(debugBreakpoint.offset);
			}
		}
		
		public virtual void  writeTo(System.IO.Stream out_Renamed)
		{
			SwfEncoder buffer = new SwfEncoder(version);
			fixNamesAndBitmaps();
			encodeSwdData(buffer);
			buffer.WriteTo(out_Renamed);
		}
		
		public virtual void  error(System.String msg)
		{
		}
		
		/// <summary> C: The SWD generation for fdb is... flaky, especially the way to figure out
		/// the bitmap category based on debug module names. DebugModule and DebugScript
		/// must be set with a flag indicating whether they are classes, frame actions,
		/// etc, etc, etc.
		/// 
		/// R: I don't particularly like it either and would prefer it if this stuff
		/// lived on the fdb side, not in here.
		/// </summary>
		private bool isFrameworkClass(System.String name)
		{
			bool isIt = (name.StartsWith("mx.") && name.IndexOf(":") != - 1 && name.EndsWith(".as")) || (name.IndexOf("/mx/") > - 1);
			
			return isIt;
		}
	}
}