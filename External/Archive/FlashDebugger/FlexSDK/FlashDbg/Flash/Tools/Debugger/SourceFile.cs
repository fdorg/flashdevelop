// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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

namespace Flash.Tools.Debugger
{
	
	/// <summary> A SourceFile contains information about a specific segment 
	/// of ActionScript source code.  The source code could be
	/// derived from a number of locations; an ActionScript file, a 
	/// snip-it of code from a frame, compiler generated code, etc.
	/// </summary>
	public interface SourceFile
	{
		/// <summary> Base path for this filename, without the package-name portion.  For
		/// example, if class mx.controls.Button.as was in
		/// C:\flex\sdk\frameworks\mx\controls\Button.as, then getBasePath()
		/// would return "C:\flex\sdk\frameworks" (note that the "mx\controls"
		/// part would NOT be returned).
		/// </summary>
		/// <returns> base path, or null
		/// </returns>
		String BasePath
		{
			get;
			
		}
		/// <summary> File name of this SourceFile.  In the case of a disk-based SourceFile,
		/// this is the same as the filename with no path, e.g. 'myfile.as'
		/// </summary>
		/// <returns> filename, or "" (never null)
		/// </returns>
		String Name
		{
			get;
			
		}
		/// <summary> Full path and file name, if its exists, for this SourceFile.  For
		/// disk-based SourceFiles, this is equivalent to
		/// <code>getBasePath + slash + getPackageName() + slash + getName()</code>
		/// where "slash" is a platform-specific slash character.
		/// </summary>
		/// <returns> path, never null
		/// </returns>
		String FullPath
		{
			get;
			
		}
		/// <summary> Raw, unprocessed file name for this SourceFile.</summary>
		/// <since> As of Version 2
		/// </since>
		String RawName
		{
			get;
			
		}
		/// <summary> Returns the number of source lines in the given file</summary>
		/// <returns> -1 indicates an error.  Call getError() to 
		/// obtain specific reason code.
		/// </returns>
		int LineCount
		{
			get;
			
		}
		/// <summary> Return a unique identifier for this SourceFile. </summary>
		int Id
		{
			get;
			
		}
		
		/// <summary> Get the package name portion of the path for this file. For example, if
		/// class mx.controls.Button.as was in
		/// C:\flex\sdk\frameworks\mx\controls\Button.as, then getPackageName() would
		/// return "mx\controls".
		/// 
		/// </summary>
		/// <returns> package name, or "" (never null)
		/// </returns>
		String getPackageName();
		
		/// <summary> Obtains the textual content of the given line
		/// from within a source file.  
		/// Line numbers start at 1 and go to getLineCount().
		/// 
		/// </summary>
		/// <returns> the line of source of the file.  Any carriage
		/// return and/or line feed are stripped from the
		/// end of the string.
		/// </returns>
		String getLine(int lineNum);
		
		/// <summary>---------------------------------------------------
		/// WARNING:  The functions below will return null
		/// and/or 0 values while 
		/// Session.fileMetaDataLoaded() is false.
		/// ---------------------------------------------------
		/// 
		///
		/// Return the function name for a given line number, or <code>null</code>
		/// if not known or if the line matches more than one function.
		/// </summary>
		/// <since> Version 3.
		/// </since>
		String getFunctionNameForLine(Session s, int lineNum);
		
		/// <summary> Return the line number for the given function name
		/// if it doesn't exists -1 is returned
		/// </summary>
		int getLineForFunctionName(Session s, String name);
		
		/// <summary> Get a list of all function names for this SourceFile</summary>
		String[] getFunctionNames(Session s);
		
		/// <summary> Return the offset within the SWF for a given line 
		/// number.
		/// </summary>
		int getOffsetForLine(int lineNum);
	}
}
