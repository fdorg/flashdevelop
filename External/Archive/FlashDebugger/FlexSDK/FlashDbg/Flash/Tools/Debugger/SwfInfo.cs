// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2004-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;

namespace Flash.Tools.Debugger
{
	/// <summary> The SwfInfo object contains information relating to
	/// a particular swf file that was loaded by the Player.
	/// Each SWF file contains a list of actionscript source
	/// files from which execution is performed.
	/// 
	/// It is important to note 2 or more SWF files may contain
	/// multiple copies of the same source code.  From the 
	/// Player's perspective and the API perspective these
	/// copies are unique and it is up to the user of the 
	/// API to detect these 'duplicate' files and either
	/// filter them from the user and/or present an
	/// appropriate disambiguous representation of 
	/// the file names.  Also internally they are treated
	/// as two distinct files and thus breakpoints 
	/// will most likely need to be set on both files
	/// independently.
	/// </summary>
	public interface SwfInfo
	{
		/// <summary> The full path of the SWF.</summary>
		String Path
		{
			get;
		}
		/// <summary> The URL for the SWF.  Includes any options
		/// at the end of the URL. For example ?debug=true
		/// </summary>
		String Url
		{
			get;
		}
		/// <summary> The size of this SWF in bytes</summary>
		int SwfSize
		{
			get;
		}
		/// <summary> Indicates whether the contents of the SWF file
		/// have been completely processed.
		/// Completely processed means that calls to getSwdSize
		/// and other calls that may throw an InProgressException will
		/// not throw this exception.  Additionally the function
		/// and offset related calls within SourceFile will return
		/// non-null values once this call returns true.
		/// </summary>
		/// <since> Version 2
		/// </since>
		bool ProcessingComplete
		{
			get;
		}
		
		/// <summary> The size of the debug SWD file, if any
		/// This may also be zero if the SWD load is in progress
		/// </summary>
		/// <throws>  InProgressException if the SWD has not yet been loaded </throws>
		int getSwdSize(Session s);
		
		/// <summary> Indication that this SWF, which was previously loaded into
		/// the Player, is now currently unloaded.  All breakpoints
		/// set on any of the files contained within this SWF will
		/// be inactive.  These breakpoints will still exist in the 
		/// list returned by Session.getBreakpointList()
		/// </summary>
		bool isUnloaded();
		
		/// <summary> Number of source files in this SWF.
		/// May be zero if no debug 
		/// </summary>
		/// <throws>  InProgressException if the SWD has not yet been loaded </throws>
		int getSourceCount(Session s);
		
		/// <summary> List of source files that are contained within 
		/// this SWF.
		/// </summary>
		/// <throws>  InProgressException if the SWD has not yet been loaded </throws>
		/// <since> Version 2
		/// </since>
		SourceFile[] getSourceList(Session s);
		
		/// <summary> Returns true if the given source file is contained 
		/// within this SWF. 
		/// </summary>
		/// <since> Version 2
		/// </since>
		bool containsSource(SourceFile f);
	}
}
