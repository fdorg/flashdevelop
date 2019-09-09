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
using System.Text;
using System.Text.RegularExpressions;
using FileUtils = Flash.Util.FileUtils;
using SystemProperties = JavaCompatibleClasses.SystemProperties;

namespace Flash.Tools.Debugger.Concrete
{
	
	/// <summary> A module which is uniquly identified by an id, contains
	/// a short and long name and also a script
	/// </summary>
	public class DModule : SourceFile
	{
        private class FunctionPositionComparator : System.Collections.IComparer
		{
			public FunctionPositionComparator(DModule module)
			{
                m_Module = module;
			}
			private DModule m_Module;
			
			public virtual int Compare(Object o1, Object o2)
			{
				int line1 = ((Int32) m_Module.m_func2FirstLine[o1]);
				int line2 = ((Int32) m_Module.m_func2FirstLine[o2]);
				return line1 - line2;
			}
		}
		virtual public ScriptText Script
		{
			get
			{
				lock (this)
				{
					// If we have been using "dummy" source, and the user has changed the list of
					// directories that are searched for source, then we want to search again
					if (!m_gotRealScript && m_sourceLocator != null && m_sourceLocator.ChangeCount != m_sourceLocatorChangeCount)
					{
						m_script = null;
					}
					
					// lazy-initialize m_script, so that we don't read a disk file until
					// someone actually needs to look at the file
					if (m_script == null)
					{
						String script = scriptFromDisk(RawName);
						if (script == null)
						{
							script = ""; // use dummy source for now //$NON-NLS-1$
						}
						else
						{
							m_gotRealScript = true; // we got the real source
						}
						m_script = new ScriptText(script);
					}
					return m_script;
				}
			}
			
		}
		virtual public String BasePath
		{
			/* getters */
			
			get
			{
				return m_basePath;
			}
			
		}
		virtual public String Name
		{
			get
			{
				return m_shortName;
			}
			
		}
		virtual public String FullPath
		{
			get
			{
				return m_path;
			}
			
		}
		virtual public String RawName
		{
			get
			{
				return m_rawName;
			}
			
		}
		virtual public int Id
		{
			get
			{
				return m_id;
			}
			
		}
		virtual public int Bitmap
		{
			get
			{
				return m_bitmap;
			}
			
		}
		virtual public int LineCount
		{
			get
			{
				return Script.LineCount;
			}
			
		}
		private ScriptText m_script; // lazy-initialized by getScript()
		private bool m_gotRealScript;
		private String m_rawName;
		private String m_shortName;
		private String m_path;
		private String m_basePath;
		private int m_id;
		private int m_bitmap;
		private System.Collections.ArrayList m_line2Offset; // array of Integer
		private System.Collections.ArrayList m_line2Func; // each array is either null, String, or String[]
		private System.Collections.Hashtable m_func2FirstLine; // maps function name (String) to first line of function (Integer)
		private System.Collections.Hashtable m_func2LastLine; // maps function name (String) to last line of function (Integer)
		private String m_packageName;
		private bool m_gotAllFncNames;
		private int m_anonymousFunctionCounter = 0;
		private SourceLocator m_sourceLocator;
		private int m_sourceLocatorChangeCount;
		private static readonly String m_newline = Environment.NewLine; //$NON-NLS-1$
		
		/// <param name="name">filename in "basepath;package;filename" format
		/// </param>
		public DModule(SourceLocator sourceLocator, int id, int bitmap, String name, String script)
		{
			// If the caller gave us the script text, then we will create m_script
			// now.  But if the caller gave us an empty string, then we won't bother
			// looking for a disk file until someone actually asks for it.
			if (script != null && script.Length > 0)
			{
				m_script = new ScriptText(script);
				m_gotRealScript = true;
			}
			
			NameParser nameParser = new NameParser(name);
			
			m_sourceLocator = sourceLocator;
			m_rawName = name;
			m_basePath = nameParser.BasePath; // may be null
			m_bitmap = bitmap;
			m_id = id;
			m_shortName = generateShortName(nameParser);
			m_path = generatePath(nameParser);
			m_line2Offset = new System.Collections.ArrayList();
			m_line2Func = new System.Collections.ArrayList();
			m_func2FirstLine = new System.Collections.Hashtable();
			m_func2LastLine = new System.Collections.Hashtable();
			m_packageName = nameParser.Package;
			m_gotAllFncNames = false;
		}
		public virtual String getPackageName()
		{
			return (m_packageName == null)?"":m_packageName;
		} //$NON-NLS-1$
		public virtual String getLine(int i)
		{
			return (i > LineCount)?"// code goes here":Script.getLine(i);
		}
		
		internal virtual void  setPackageName(String name)
		{
			m_packageName = name;
		}
		
		/// <returns> the offset within the swf for a given line 
		/// of source.  0 if unknown.
		/// </returns>
		public virtual int getOffsetForLine(int line)
		{
			int offset = 0;
			if (line < m_line2Offset.Count)
			{
                if (m_line2Offset.Contains(line))
                {
                    offset = (int)m_line2Offset[line];
                }
			}
			return offset;
		}
		
		public virtual int getLineForFunctionName(Session s, String name)
		{
			int value = - 1;
			primeAllFncNames(s);

            if (m_func2FirstLine.Contains(name))
            {
                value = (int)m_func2FirstLine[name];
            }
			
			return value;
		}
		
		/*
		* @see Flash.Tools.Debugger.SourceFile#getFunctionNameForLine(Flash.Tools.Debugger.Session, int)
		*/
		public virtual String getFunctionNameForLine(Session s, int line)
		{
			primeFncName(s, line);
			
			String[] funcNames = getFunctionNamesForLine(s, line);
			
			if (funcNames != null && funcNames.Length == 1)
				return funcNames[0];
			else
				return null;
		}
		
		/// <summary> Return the function names for a given line number, or an empty array
		/// if there are none; never returns <code>null</code>.
		/// </summary>
		private String[] getFunctionNamesForLine(Session s, int line)
		{
			primeFncName(s, line);
			
			if (line < m_line2Func.Count)
			{
				Object obj = m_line2Func[line];
				
				if (obj is String)
					return new String[]{(String) obj};
				else if (obj is String[])
					return (String[]) obj;
			}
			
			return new String[0];
		}
		
		public virtual String[] getFunctionNames(Session s)
		{
			/* find out the size of the array */
			primeAllFncNames(s);
			int count = m_func2FirstLine.Count;
            String[] names = new String[count];
            m_func2FirstLine.Keys.CopyTo(names, 0);
			return names;
		}
		
		private static String generateShortName(NameParser nameParser)
		{
			String name = nameParser.OriginalName;
			String s = name;
			
			if (nameParser.PathPackageAndFilename)
			{
				s = nameParser.Filename;
			}
			else
			{
				/* do we have a file name? */
				int dotAt = name.LastIndexOf('.');
				if (dotAt > 1)
				{
					/* yes let's strip the directory off */
					int lastSlashAt = name.LastIndexOf('\\', dotAt);
					lastSlashAt = Math.Max(lastSlashAt, name.LastIndexOf('/', dotAt));
					
					s = name.Substring(lastSlashAt + 1);
				}
				else
				{
					/* not a file name ... */
					s = name;
				}
			}
			return s.Trim();
		}
		
		/// <summary> Produce a name that contains a file specification including full path.
		/// File names may come in as 'mx.bla : file:/bla.foo.as' or as
		/// 'file://bla.foo.as' or as 'C:\'(?) or as 'basepath;package;filename'
		/// </summary>
		private static String generatePath(NameParser nameParser)
		{
			String name = nameParser.OriginalName;
			String s = name;
			
			/* strip off first colon of stuff if package exists */
			int colonAt = name.IndexOf(':');
			if (colonAt > 1 && !name.StartsWith("Actions for"))
			//$NON-NLS-1$
			{
				if (name[colonAt + 1] == ' ')
					s = name.Substring(colonAt + 2);
			}
			else if (name.IndexOf('.') > - 1 && name[0] != '<')
			{
				/* some other type of file name */
				s = nameParser.recombine();
			}
			else
			{
				// no path
				s = ""; //$NON-NLS-1$
			}
			return s.Trim();
		}
		
		public virtual void  lineMapping(System.Text.StringBuilder sb)
		{
			System.Collections.IDictionary args = new System.Collections.Hashtable();
			args["fileName"] = Name; //$NON-NLS-1$
			args["fileNumber"] = Convert.ToString(Id); //$NON-NLS-1$
			sb.Append(PlayerSessionManager.LocalizationManager.getLocalizedTextString("functionsInFile", args)); //$NON-NLS-1$
			sb.Append(m_newline);
			String[] funcNames = new String[m_func2FirstLine.Count];
            m_func2FirstLine.Keys.CopyTo(funcNames, 0);
            Array.Sort(funcNames, new FunctionPositionComparator(this));
			
			for (int i = 0; i < funcNames.Length; ++i)
			{
				String name = (String) funcNames[i];
				int firstLine = ((Int32) m_func2FirstLine[name]);
				int lastLine = ((Int32) m_func2LastLine[name]);
				
				sb.Append(" "); //$NON-NLS-1$
				sb.Append(name);
				sb.Append(" "); //$NON-NLS-1$
				sb.Append(firstLine);
				sb.Append(" "); //$NON-NLS-1$
				sb.Append(lastLine);
				sb.Append(m_newline);
			}
		}
		
		internal virtual int compareTo(DModule other)
		{
			return String.CompareOrdinal(Name, other.Name);
		}
		
		/// <summary> Called in order to make sure that we have a function name available
		/// at the given location.  For AVM+ swfs we don't need a swd and therefore
		/// don't have access to function names in the same fashion.
		/// We need to ask the player for a particular function name.
		/// </summary>
		internal virtual void  primeFncName(Session s, int line)
		{
			// for now we do all, optimize later
			primeAllFncNames(s);
		}
		
		internal virtual void  primeAllFncNames(Session s)
		{
			// send out the request for all functions that the player knows
			// about for this module
			
			// we block on this call waiting for an answer and after we get it
			// the DManager thread should have populated our mapping tables
			// under the covers.  If its fails then no biggie we just won't
			// see anything in the tables.
			PlayerSession ps = (PlayerSession) s;
			if (!m_gotAllFncNames && ps.playerVersion() >= 9)
			{
				try
				{
					ps.requestFunctionNames(m_id, - 1);
				}
				catch (VersionException)
				{
					;
				}
				catch (NoResponseException)
				{
					;
				}
			}
			m_gotAllFncNames = true;
		}
		
		internal virtual void  addLineFunctionInfo(int offset, int line, String funcName)
		{
			addLineFunctionInfo(offset, line, line, funcName);
		}
		
		/// <summary> Called by DSwfInfo in order to add function name / line / offset mapping
		/// information to the module.  
		/// </summary>
		internal virtual void  addLineFunctionInfo(int offset, int firstLine, int lastLine, String funcName)
		{
			int line;
			
			// strip down the function name
			if (funcName == null || funcName.Length == 0)
			{
				funcName = "<anonymous$" + (++m_anonymousFunctionCounter) + ">"; //$NON-NLS-1$ //$NON-NLS-2$
			}
			else
			{
				// colons or slashes then this is an AS3 name, strip off the core::
				int colon = funcName.LastIndexOf(':');
				int slash = funcName.LastIndexOf('/');
				if (colon > - 1 || slash > - 1)
				{
					int greater = Math.Max(colon, slash);
					funcName = funcName.Substring(greater + 1);
				}
				else
				{
					int dot = funcName.LastIndexOf('.');
					if (dot > - 1)
					{
						// extract function and package
						String pkg = funcName.Substring(0, (dot) - (0));
						funcName = funcName.Substring(dot + 1);
						
						// attempt to set the package name while we're in here
						setPackageName(pkg);
						//					System.out.println(m_id+"-func="+funcName+",pkg="+pkg);
					}
				}
			}
			
			//		System.out.println(m_id+"@"+offset+"="+getPath()+".adding func="+funcName);
			
			// make sure m_line2Offset is big enough for the lines we're about to set
			//SupportClass.IListSupport.EnsureCapacity(m_line2Offset, firstLine + 1);
			while (firstLine >= m_line2Offset.Count)
				m_line2Offset.Add(null);
			
			// add the offset mapping
			m_line2Offset[firstLine] = (Int32) offset;
			
			// make sure m_line2Func is big enough for the lines we're about to se
			//SupportClass.IListSupport.EnsureCapacity(m_line2Func, lastLine + 1);
			while (lastLine >= m_line2Func.Count)
				m_line2Func.Add(null);
			
			// offset and byteCode ignored currently, only add the name for the first hit
			for (line = firstLine; line <= lastLine; ++line)
			{
				Object funcs = m_line2Func[line];
				// A line can correspond to more than one function.  The most common case
				// of that is an MXML tag with two event handlers on the same line, e.g.
				//		<mx:Button mouseOver="overHandler()" mouseOut="outHandler()" />;
				// another case is the line that declares an inner anonymous function:
				//		var f:Function = function() { trace('hi') }
				// In any such case, we store a list of function names separated by commas,
				// e.g. "func1, func2"
				if (funcs == null)
				{
					m_line2Func[line] = funcName;
				}
				else if (funcs is String)
				{
					String oldFunc = (String) funcs;
					m_line2Func[line] = new String[]{oldFunc, funcName};
				}
				else if (funcs is String[])
				{
					String[] oldFuncs = (String[]) funcs;
					String[] newFuncs = new String[oldFuncs.Length + 1];
					Array.Copy(oldFuncs, 0, newFuncs, 0, oldFuncs.Length);
					newFuncs[newFuncs.Length - 1] = funcName;
					m_line2Func[line] = newFuncs;
				}
			}
			
			// add to our function name list
			if (m_func2FirstLine[funcName] == null)
			{
				m_func2FirstLine[funcName] = (Int32) firstLine;
				m_func2LastLine[funcName] = (Int32) lastLine;
			}
		}
		
		/// <summary> Scan the disk looking for the location of where the source resides.  May
		/// also peel open a swd file looking for the source file.
		/// </summary>
		/// <param name="name">original full path name of the source file
		/// </param>
		/// <returns> string containing the contents of the file, or null if not found
		/// </returns>
		private String scriptFromDisk(String name)
		{
			// we expect the form of the filename to be in the form
			// "c:/src/project;debug;myFile.as"
			// where the semicolons demark the include directory searched by the
			// compiler followed by package directories then file name.
			// any slashes are to be forward slash only!
			
			// translate to neutral form
			name = name.Replace('\\', '/'); //@todo remove this when compiler is complete
			
			// pull the name apart
			const char SEP = ';';
			String pkgPart = ""; //$NON-NLS-1$
			String pathPart = ""; //$NON-NLS-1$
			String namePart = ""; //$NON-NLS-1$
			int at = name.IndexOf((Char) SEP);
			if (at > - 1)
			{
				// have at least 2 parts to name
				int nextAt = name.IndexOf((Char) SEP, at + 1);
				if (nextAt > - 1)
				{
					// have 3 parts
					pathPart = name.Substring(0, (at) - (0));
					pkgPart = name.Substring(at + 1, (nextAt) - (at + 1));
					namePart = name.Substring(nextAt + 1);
				}
				else
				{
					// 2 parts means no package.
					pathPart = name.Substring(0, (at) - (0));
					namePart = name.Substring(at + 1);
				}
			}
			else
			{
				// should not be here....
				// trim by last slash
				at = name.LastIndexOf('/');
				if (at > - 1)
				{
					// cheat by looking for dirname "mx" in path
					int mx = name.LastIndexOf("/mx/"); //$NON-NLS-1$
					if (mx > - 1)
					{
						pathPart = name.Substring(0, (mx) - (0));
						pkgPart = name.Substring(mx + 1, (at) - (mx + 1));
					}
					else
					{
						pathPart = name.Substring(0, (at) - (0));
					}
					
					namePart = name.Substring(at + 1);
				}
				else
				{
					pathPart = "."; //$NON-NLS-1$
					namePart = name;
				}
			}
			
			String script = null;
			try
			{
				// now try to locate the thing on disk or in a swd.
				Encoding realEncoding = null;
                Encoding bomEncoding = null;
				Stream in_Renamed = locateScriptFile(pathPart, pkgPart, namePart);
				if (in_Renamed != null)
				{
					try
					{
						// Read the file using the appropriate encoding, based on
						// the BOM (if there is a BOM) or the default charset for
						// the system (if there isn't a BOM)
						BufferedStream bis = new BufferedStream(in_Renamed);
						bomEncoding = getEncodingFromBOM(bis);
						script = pullInSource(bis, bomEncoding);
						
						// If the file is an XML file with an <?xml> directive,
						// it may specify a different directive 
						realEncoding = getEncodingFromXMLDirective(script);
					}
					finally
					{
						try
						{
							in_Renamed.Close();
						}
						catch (IOException)
						{
						}
					}
				}
				
				// If we found an <?xml> directive with a specified encoding, and
				// it doesn't match the encoding we used to read the file initially,
				// start over.
				if (realEncoding != null && realEncoding != bomEncoding)
				{
					in_Renamed = locateScriptFile(pathPart, pkgPart, namePart);
					if (in_Renamed != null)
					{
						try
						{
							// Read the file using the real encoding, based on the
							// <?xml...> directive
							BufferedStream bis = new BufferedStream(in_Renamed);
							getEncodingFromBOM(bis);
							script = pullInSource(bis, realEncoding);
						}
						finally
						{
							try
							{
								in_Renamed.Close();
							}
							catch (IOException)
							{
							}
						}
					}
				}
			}
			catch (FileNotFoundException fnf)
			{
                Console.Error.Write(fnf.StackTrace);
                Console.Error.Flush();
            }
			return script;
		}
		
		/// <summary> Logic to poke around on disk in order to find the given
		/// filename.  We look under the mattress and all other possible
		/// places for the silly thing.  We always try locating
		/// the file directly first, if that fails then we hunt out
		/// the swd.
		/// </summary>
		internal virtual Stream locateScriptFile(String path, String pkg, String name)
		{
			if (m_sourceLocator != null)
			{
				m_sourceLocatorChangeCount = m_sourceLocator.ChangeCount;
				Stream is_Renamed = m_sourceLocator.locateSource(path, pkg, name);
				if (is_Renamed != null)
					return is_Renamed;
			}
			
			// convert slashes first
			path = path.Replace('/', Path.DirectorySeparatorChar);
			pkg = pkg.Replace('/', Path.DirectorySeparatorChar);
			FileInfo f;
			
			// use a package base directory if it exists
			if (path.Length > 0)
			{
				try
				{
					String pkgAndName = ""; //$NON-NLS-1$
					if (pkg.Length > 0)
					// have to do this so we don't end up with just "/filename"
						pkgAndName += (pkg + Path.DirectorySeparatorChar);
					pkgAndName += name;
					f = new FileInfo(path + "\\" + pkgAndName);
					bool tmpBool;
					if (File.Exists(f.FullName))
						tmpBool = true;
					else
						tmpBool = Directory.Exists(f.FullName);
					if (tmpBool)
					{
						return new FileStream(f.FullName, FileMode.Open, FileAccess.Read);
					}
				}
				catch (NullReferenceException)
				{
					// skip it.
				}
			}
			
			// try the current directory plus package
			if (pkg.Length > 0)
			// new File("", foo) looks in root directory!
			{
				f = new FileInfo(pkg + "\\" + name);
				bool tmpBool2;
				if (File.Exists(f.FullName))
					tmpBool2 = true;
				else
					tmpBool2 = Directory.Exists(f.FullName);
				if (tmpBool2)
				{
					return new FileStream(f.FullName, FileMode.Open, System.IO.FileAccess.Read);
				}
			}
			
			// look in the current directory without the package
			f = new System.IO.FileInfo(name);
			bool tmpBool3;
			if (System.IO.File.Exists(f.FullName))
				tmpBool3 = true;
			else
				tmpBool3 = System.IO.Directory.Exists(f.FullName);
			if (tmpBool3)
			{
				return new System.IO.FileStream(f.FullName, System.IO.FileMode.Open, FileAccess.Read);
			}
			
			// @todo try to pry open a swd file...
			
			return null;
		}
		
		/// <summary> See if this document starts with a BOM and try to figure
		/// out an encoding from it.
		/// </summary>
		/// <param name="bis">for document (so that we can reset the stream
		/// if we establish that the first characters aren't a BOM)
		/// </param>
		/// <returns>			CharSet from BOM (or system default / null)
		/// </returns>
		private Encoding getEncodingFromBOM(BufferedStream bis)
		{
			Encoding bomEncoding = null;
			//SupportClass.BufferedStreamManager.manager.MarkPosition(3, bis);
			String bomEncodingString;
			try
			{
				bomEncodingString = FileUtils.consumeBOM(bis, null);
			}
			catch (IOException)
			{
				bomEncodingString = SystemProperties.getProperty("file.encoding"); //$NON-NLS-1$
			}
			
			bomEncoding = Encoding.GetEncoding(bomEncodingString);
			
			return bomEncoding;
		}
		
		/// <summary> Syntax for an &lt;?xml ...> directive with an encoding (used by getEncodingFromXMLDirective)</summary>
		private static readonly Regex sXMLDeclarationPattern = new Regex(@"^<\?xml[^>]*encoding\s*=\s*(""([^""]*)""|'([^']*)')"); //$NON-NLS-1$
		
		/// <summary> See if this document starts with an &lt;?xml ...> directive and
		/// try to figure out an encoding from it.
		/// </summary>
		/// <param name="entireSource">of document
		/// </param>
		/// <returns>					specified Charset (or null)
		/// </returns>
		private Encoding getEncodingFromXMLDirective(String entireSource)
		{
			String encoding = null;
			Match xmlDeclarationMatcher = sXMLDeclarationPattern.Match(entireSource);
			if (xmlDeclarationMatcher.Success)
			{
				encoding = xmlDeclarationMatcher.Groups[2].ToString();
				if (encoding == null)
					encoding = xmlDeclarationMatcher.Groups[3].ToString();
				
				try
				{
					return System.Text.Encoding.GetEncoding(encoding);
				}
				catch (ArgumentException)
				{
				}
			}
			return null;
		}
		
		/// <summary> Given an input stream containing source file contents, read in each line</summary>
		/// <param name="input">of source file contents (with BOM removed)
		/// </param>
		/// <param name="encoding">to use (based on BOM, system default, or &lt;?xml...> directive
		/// if this is null, the system default will be used)
		/// </param>
		/// <returns>				source file contents (as String)
		/// </returns>
		internal virtual String pullInSource(Stream input, Encoding encoding)
		{
			String script = ""; //$NON-NLS-1$

            try
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
                BufferedStream bufferedStream = new BufferedStream(input);

                StreamReader f = new StreamReader(bufferedStream, encoding != null ? encoding : System.Text.Encoding.Default);

				String line;
				while ((line = f.ReadLine()) != null)
				{
					sb.Append(line);
					sb.Append('\n');
				}
				script = sb.ToString();
			}
			catch (IOException e)
			{
				//To change body of catch statement use File | Settings | File Templates.
                Console.Error.Write(e.StackTrace);
                Console.Error.Flush();
            }
			return script;
		}
		
		/// <summary>for debugging </summary>
		public override String ToString()
		{
			return FullPath;
		}
		
		private class NameParser
		{
			virtual public bool PathPackageAndFilename
			{
				get
				{
					return (fBasePath != null);
				}
				
			}
			virtual public String OriginalName
			{
				get
				{
					return fOriginalName;
				}
				
			}
			virtual public String BasePath
			{
				get
				{
					return fBasePath;
				}
				
			}
			virtual public String Filename
			{
				get
				{
					return fFilename;
				}
				
			}
			virtual public String Package
			{
				get
				{
					return fPackage;
				}
				
			}
			private String fOriginalName;
			private String fBasePath;
			private String fPackage;
			private String fFilename;
			private String fRecombinedName;

			/// <summary> Given a filename of the form "basepath;package;filename", return an
			/// array of 3 strings, one for each segment.
			/// </summary>
			/// <param name="name">a string which *may* be of the form "basepath;package;filename"
			/// </param>
			/// <returns> an array of 3 strings for the three pieces; or, if 'name' is
			/// not of expected form, returns null
			/// </returns>
			public NameParser(String name)
			{
				fOriginalName = name;
				
				/* is it of "basepath;package;filename" format? */
				int semicolonCount = 0;
				int i = 0;
				int firstSemi = - 1;
				int lastSemi = - 1;
				while ((i = name.IndexOf(';', i)) >= 0)
				{
					++semicolonCount;
					if (firstSemi == - 1)
						firstSemi = i;
					lastSemi = i;
					++i;
				}
				
				if (semicolonCount == 2)
				{
					fBasePath = name.Substring(0, (firstSemi) - (0));
					fPackage = name.Substring(firstSemi + 1, (lastSemi) - (firstSemi + 1));
					fFilename = name.Substring(lastSemi + 1);
				}
			}
			
			/// <summary> Returns a "recombined" form of the original name.
			/// 
			/// For filenames which came in in the form "basepath;package;filename",
			/// the recombined name is the original name with the semicolons replaced
			/// by platform-appropriate slash characters.  For any other type of original
			/// name, the recombined name is the same as the incoming name.
			/// </summary>
			public virtual String recombine()
			{
				if (fRecombinedName == null)
				{
					if (PathPackageAndFilename)
					{
						char slashChar;
						if (fOriginalName.IndexOf('\\') != - 1)
							slashChar = '\\';
						else
							slashChar = '/';
						
						fRecombinedName = fOriginalName.Replace(";;", ";").Replace(';', slashChar); //$NON-NLS-1$ //$NON-NLS-2$
					}
					else
					{
						fRecombinedName = fOriginalName;
					}
				}
				return fRecombinedName;
			}
		}
	}
}
