// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System.IO;

namespace Flash.Tools.Debugger
{
	
	/// <summary> Describes a web browser.
	/// 
	/// </summary>
	/// <author>  mmorearty
	/// </author>
	public abstract class Browser
	{
        /// <summary> Indicates an unknown browser type.
        /// 
        /// </summary>
        /// <seealso cref="Type">
        /// </seealso>
        public readonly static int UNKNOWN = 0;
        /// <summary> Indicates Internet Explorer.
        /// 
        /// </summary>
        /// <seealso cref="Type">
        /// </seealso>
        public readonly static int INTERNET_EXPLORER = 1;
        /// <summary> Indicates Netscape Navigator.
        /// 
        /// </summary>
        /// <seealso cref="Type">
        /// </seealso>
        public readonly static int NETSCAPE_NAVIGATOR = 2;
        /// <summary> Indicates Opera.
        /// 
        /// </summary>
        /// <seealso cref="Type">
        /// </seealso>
        public readonly static int OPERA = 3;
        /// <summary> Indicates the Mozilla browser, but <i>not</i> Firefox.
        /// 
        /// </summary>
        /// <seealso cref="Type">
        /// </seealso>
        public readonly static int MOZILLA = 4;
        /// <summary> Indicates Firefox.
        /// 
        /// </summary>
        /// <seealso cref="Type">
        /// </seealso>
        public readonly static int MOZILLA_FIREFOX = 5;

        /// <summary> Returns what type of Player this is, e.g. <code>INTERNET_EXPLORER</code>, etc.</summary>
		public abstract int Type
		{
			get;
			
		}
		/// <summary> Returns the path to the web browser executable -- e.g. the path to
		/// IExplore.exe, Firefox.exe, etc. (Filenames are obviously
		/// platform-specific.)
		/// </summary>
        public abstract FileInfo Path
		{
			get;
			
		}
	}
}
