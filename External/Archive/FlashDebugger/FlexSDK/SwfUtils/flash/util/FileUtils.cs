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
using System;
using System.IO;
using SystemProperties = JavaCompatibleClasses.SystemProperties;

namespace Flash.Util
{
    public abstract class FilenameFilter
    {
        public abstract Boolean accept(FileInfo dir, String file);
    }

	public sealed class FileUtils
	{
		public static String canonicalPath(String rootPath)
		{
			if (rootPath == null)
				return null;
			return canonicalPath(new FileInfo(rootPath));
		}
		
		public static String canonicalPath(FileInfo file)
		{
			return canonicalFile(file).FullName;
		}
		
		public static FileInfo canonicalFile(FileInfo file)
		{
			try
			{
				return new FileInfo(file.FullName);
			}
			catch (IOException)
			{
				return new FileInfo(file.FullName);
			}
		}
		
		private static System.Collections.Hashtable filemap = null;
		private static bool checkCase = false;
		
		/// <summary> Canonicalize on Win32 doesn't fix the case of the file to match what is on disk.
		/// Its annoying that this exists.  It will be very slow until the server stabilizes.
		/// If this is called with a pattern where many files from the same directory will be
		/// needed, then the cache should be changed to hold the entire directory contents
		/// and check the modtime of the dir.  It didn't seem like this was worth it for now.
		/// </summary>
		/// <param name="f">
		/// </param>
		/// <returns>
		/// </returns>
		public static String getTheRealPathBecauseCanonicalizeDoesNotFixCase(FileInfo f)
		{
			lock (typeof(Flash.Util.FileUtils))
			{
				if (filemap == null)
				{
					filemap = new System.Collections.Hashtable();
                    checkCase = true;
                    if (Environment.OSVersion.Platform == PlatformID.MacOSX ||
                        Environment.OSVersion.Platform == PlatformID.Unix)
                    {
                        checkCase = false;
                    }
				}
				
				String path = FileUtils.canonicalPath(f);
				
				bool tmpBool;
				if (File.Exists(f.FullName))
					tmpBool = true;
				else
					tmpBool = Directory.Exists(f.FullName);
				if (!checkCase || !tmpBool)
				// if the file doesn't exist, then we can't do anything about it.
					return path;
				
				// We're going to ignore the issue where someone changes the capitalization of a file on the fly.
				// If this becomes an important issue we'll have to make this cache smarter.
				
				if (filemap.ContainsKey(path))
				{
					return (String) filemap[path];
				}
				
				String file = f.Name;
				
				FileInfo canonfile = new FileInfo(path);
				
				FileInfo dir = new FileInfo(canonfile.DirectoryName);
				
				// removing dir.listFiles() because it is not supproted in .NET
				String[] ss = Directory.GetFileSystemEntries(dir.FullName);
				if (ss != null)
				{
					int n = ss.Length;
					FileInfo[] files = new FileInfo[n];
					for (int i = 0; i < n; i++)
					{
						files[i] = new FileInfo(canonfile.FullName + "\\" + ss[i]);
					}
					
					for (int i = 0; i < files.Length; ++i)
					{
						if (files[i].Name.ToUpper().Equals(file.ToUpper()))
						{
							filemap[path] = files[i].FullName;
							return files[i].FullName;
						}
					}
				}
				// If we got here, it must be because we can't read the directory?
				return path;
			}
		}
		
		public static String readFile(FileInfo file, String default_encoding)
		{
			FileStream fileInputStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
			
			try
			{
				System.Text.StringBuilder returnVal = new System.Text.StringBuilder((int) file.Length);
				BufferedStream input = new BufferedStream(fileInputStream);
				
				StreamReader reader = new StreamReader(input, System.Text.Encoding.GetEncoding(consumeBOM(input, default_encoding)));
				
				char[] line = new char[2000];
				int count = 0;
				
				while ((count = reader.Read(line, 0, line.Length)) >= 0)
				{
					returnVal.Append(line, 0, count);
				}
				
				return returnVal.ToString();
			}
			finally
			{
				fileInputStream.Close();
			}
		}
		
		public static String readFile(String filename, String default_encoding)
		{
			return readFile(new FileInfo(filename), default_encoding);
		}
		
		/// <param name="input">input stream
		/// </param>
		/// <param name="default_encoding">default encoding. null or "" => system default
		/// </param>
		/// <returns> file encoding..
		/// </returns>
		/// <throws>  IOException </throws>
		public static String consumeBOM(Stream input, String default_encoding)
		{
			return consumeBOM(input, default_encoding, false);
		}
		
		/// <param name="input">input stream
		/// </param>
		/// <param name="default_encoding">default encoding. null or "" => system default
		/// </param>
		/// <param name="alwaysConsumeBOM">If true, then consume the UTF-16 BOM. 
		/// If false use the previous behavior that consumes
		/// a UTF-8 BOM but not a UTF-16 BOM.
		/// This flag is useful when reading a file into
		/// a string that is then passed to a parser. The parser may 
		/// not know to strip out the BOM. 
		/// </param>
		/// <returns> file encoding..
		/// </returns>
		/// <throws>  IOException </throws>
		public static String consumeBOM(Stream input, String default_encoding, bool alwaysConsumeBOM)
		{
            long markPosition = input.Position;

			// Determine file encoding...
			// ASCII - no header (use the supplied encoding)
			// UTF8  - EF BB BF
			// UTF16 - FF FE or FE FF (decoder chooses endian-ness)
			if (input.ReadByte() == 0xef && input.ReadByte() == 0xbb && input.ReadByte() == 0xbf)
			{
				// UTF-8 reader does not consume BOM, so do not reset
				if (SystemProperties.getProperty("flex.platform.CLR") != null)
				{
					return "UTF8";
				}
				else
				{
					return "UTF-8";
				}
			}
			else
			{
                input.Position = markPosition;

                int b0 = input.ReadByte();
                int b1 = input.ReadByte();
				if (b0 == 0xff && b1 == 0xfe || b0 == 0xfe && b1 == 0xff)
				{
					// If we don't consume the BOM is its assumed a
					// UTF-16 reader will consume BOM
					if (!alwaysConsumeBOM)
					{
                        input.Position = markPosition;
                    }
					
					if (SystemProperties.getProperty("flex.platform.CLR") != null)
					{
						return "UTF16";
					}
					else
					{
						return "UTF-16";
					}
				}
				else
				{
					// no BOM found
                    input.Position = markPosition;
                    
                    if (default_encoding != null && default_encoding.Length != 0)
					{
						return default_encoding;
					}
					else
					{
						return SystemProperties.getProperty("file.encoding");
					}
				}
			}
		}
		
		/* post-1.2 File methods */
		
		public static FileInfo getAbsoluteFile(FileInfo f)
		{
			FileInfo absolute = null;
			
			try
			{
				absolute = f == null?null:new FileInfo(f.FullName);
			}
			catch (System.Security.SecurityException se)
			{
				if (Trace.pathResolver)
				{
					Trace.trace(se.Message);
				}
			}
			
			return absolute;
		}
		
		public static FileInfo getCanonicalFile(FileInfo f)
		{
			return new FileInfo(f.FullName);
		}
		
		public static FileInfo getParentFile(FileInfo f)
		{
			String p = f.DirectoryName;
			if (p == null)
			{
				return null;
			}
			return new FileInfo(p);
		}
		
		public static FileInfo[] listFiles(FileInfo dir)
		{
			String[] fileNames = Directory.GetFileSystemEntries(dir.FullName);
			
			if (fileNames == null)
			{
				return null;
			}
			
			FileInfo[] fileList = new FileInfo[fileNames.Length];
			for (int i = 0; i < fileNames.Length; i++)
			{
				fileList[i] = new FileInfo(dir.FullName + "\\" + fileNames[i]);
			}
			return fileList;
		}
		
		public static FileInfo[] listFiles(FileInfo dir, FilenameFilter filter)
		{
			String[] fileNames = Directory.GetFileSystemEntries(dir.FullName);
			
			if (fileNames == null)
			{
				return null;
			}
			
			System.Collections.ArrayList filteredFiles = new System.Collections.ArrayList();
			for (int i = 0; i < fileNames.Length; i++)
			{
				if ((filter == null) || filter.accept(dir, fileNames[i]))
				{
					filteredFiles.Add(new FileInfo(dir.FullName + "\\" + fileNames[i]));
				}
			}
			
			return (FileInfo[]) (SupportClass.ICollectionSupport.ToArray(filteredFiles, new FileInfo[0]));
		}
		
		public static Uri toURL(FileInfo f)
		{
			String s = f.FullName;
			if (Path.DirectorySeparatorChar != '/')
			{
				s = s.Replace(Path.DirectorySeparatorChar, '/');
			}
			if (!s.StartsWith("/"))
			{
				s = "/" + s;
			}
			if (!s.EndsWith("/") && Directory.Exists(f.FullName))
			{
				s = s + "/";
			}
			UriBuilder temp_uriBuilder;
			temp_uriBuilder = new UriBuilder("file", "");
			temp_uriBuilder.Path = s;
			return temp_uriBuilder.Uri;
		}
		
		public static String addPathComponents(String p1, String p2, char sepchar)
		{
			if (p1 == null)
				p1 = "";
			if (p2 == null)
				p2 = "";
			
			int r1 = p1.Length - 1;
			
			while ((r1 >= 0) && ((p1[r1] == sepchar)))
				--r1;
			
			int r2 = 0;
			while ((r2 < p2.Length) && (p2[r2] == sepchar))
				++r2;
			
			String left = p1.Substring(0, (r1 + 1) - (0));
			String right = p2.Substring(r2);
			
			String sep = "";
			if ((left.Length > 0) && (right.Length > 0))
				sep += sepchar;
			
			return left + sep + right;
		}
		
		/// <summary> Java's File.renameTo doesn't try very hard.</summary>
		/// <param name="from"> absolute origin
		/// </param>
		/// <param name="to">   absolute target
		/// </param>
		/// <returns> true if rename succeeded
		/// </returns>
		public static bool renameFile(FileInfo from, FileInfo to)
		{
			if (!from.Exists && !Directory.Exists(from.FullName))
			{
				return false;
			}

			try
			{
				if (to.Exists || Directory.Exists(to.FullName))
				{
					if (to.Exists)
					{
						to.Delete();
					}
					else if (Directory.Exists(to.FullName))
					{
						Directory.Delete(to.FullName, true);
					}
					else
                    {
						FileInfo old = new FileInfo(to.FullName + ".old");
						
						if (old.Exists)
						{
							old.Delete();
						}
						else if (Directory.Exists(old.FullName))
						{
							Directory.Delete(old.FullName);
						}

                        try
                        {
                            to.MoveTo(old.FullName);
                        }
                        catch (Exception)
                        {
                        }
					}
				}
			}
			catch (Exception)
			{
				// eat exception, keep on going...
			}
			
			try
			{
                from.MoveTo(to.FullName);
                return true;
            }
            catch (Exception)
            {
				// everything seems to have failed.  copy the bits.
				
				BufferedStream input = new BufferedStream(new FileStream(from.FullName, FileMode.Open, FileAccess.Read));
				BufferedStream output = new BufferedStream(new FileStream(to.FullName, FileMode.Create));
				byte[] buf = new byte[8 * 1024];
				
				long remain = from.Length;
				
				try
				{
					while (remain > 0)
					{
                        try
                        {
						    int r = input.Read(buf, 0, buf.Length);
						    remain -= r;
						    output.Write(buf, 0, r);
						    output.Flush();
                        }
                        catch (Exception)
                        {
                            return false;
                        }
					}
				}
				finally
				{
					input.Close();
					output.Close();
				}

                if (from.Length == to.Length)
				{
					if (from.Exists)
					{
						File.Delete(from.FullName);
					}
					else if (Directory.Exists(from.FullName))
					{
						Directory.Delete(from.FullName);
					}
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		
		public static byte[] toByteArray(Stream input)
		{
			MemoryStream baos = new MemoryStream(8192);
			byte[] buffer = new byte[8192];
			int num = 0;
			Stream inputStream = new BufferedStream(input);
			try
			{
				while ((num = inputStream.Read(buffer, 0, buffer.Length)) != - 1)
				{
					baos.Write(buffer, 0, num);
				}
			}
			catch (IOException ex)
			{
                if (Trace.error)
                {
                    Console.Error.Write(ex.StackTrace);
                    Console.Error.Flush();
                }				
				// FIXME: do we really want to catch this without an exception?
			}
			finally
			{
				try
				{
					input.Close();
				}
				catch (IOException)
				{
				}
			}
			
			return baos.ToArray();
		}

#if false
		public static sbyte[] toByteArray(Stream input, int length)
		{
			BufferedStream inputStream = new BufferedStream(input);
			sbyte[] buffer = new sbyte[length];
			
			try
			{
				int bytesRead = 0;
				int index = 0;
				
				while ((bytesRead >= 0) && (index < buffer.Length))
				{
					bytesRead = inputStream is Flash.Swf.SwfDecoder?((Flash.Swf.SwfDecoder) inputStream).read(buffer, index, buffer.Length - index):SupportClass.ReadInput(inputStream, buffer, index, buffer.Length - index);
					index += bytesRead;
				}
			}
			finally
			{
				inputStream.Close();
			}
			return buffer;
		}
#endif
		
		public static void  writeClassToFile(String baseDir, String packageName, String className, String str)
		{
			String reldir = packageName.Replace('.', Path.DirectorySeparatorChar);
			String dir = FileUtils.addPathComponents(baseDir, reldir, Path.DirectorySeparatorChar);
			Directory.CreateDirectory(new FileInfo(dir).FullName);
			String generatedName = FileUtils.addPathComponents(dir, className, Path.DirectorySeparatorChar);
			StreamWriter fileWriter = null;
			
			try
			{

                fileWriter = new StreamWriter(new FileStream(generatedName, FileMode.Create), System.Text.Encoding.GetEncoding("UTF-8"));
				fileWriter.Write(str);
				fileWriter.Flush();
			}
			finally
			{
				if (fileWriter != null)
				{
					try
					{
						fileWriter.Close();
					}
					catch (IOException)
					{
					}
				}
			}
		}
		
		/// <summary> returns whether the file is absolute
		/// if a security exception is thrown, always returns false
		/// </summary>
		public static bool isAbsolute(FileInfo f)
		{
            return true;
		}
		
		/// <summary> returns whether the file exists
		/// if a security exception is thrown, always returns false
		/// </summary>
		public static bool exists(FileInfo f)
		{
            return f.Exists;
		}
		
		/// <summary> returns whether it's a file
		/// if a security exception is thrown, always returns false
		/// </summary>
		public static bool isFile(FileInfo f)
		{
            return (f.Attributes & (FileAttributes.Directory | FileAttributes.Device)) == 0;
		}
		
		/// <summary> returns whether it's a directory
		/// if a security exception is thrown, always returns false
		/// </summary>
		public static bool isDirectory(FileInfo f)
		{
            return (f.Attributes & FileAttributes.Directory) != 0;
        }
	}
}
