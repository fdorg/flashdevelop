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
using System;
namespace Flash.Tools.Debugger
{
	
	/// <author>  mmorearty
	/// </author>
	public class ValueAttribute
	{
        /// <summary> Indicates that the value that has been returned for a variable
        /// is actually not its real value; instead, it is the message of
        /// an exception that was thrown while executing the getter for
        /// the variable.
        /// </summary>
        public const int IS_EXCEPTION = 0x00040000;
        /// <summary> Indicates that an object is actually a Class.  For example, if you have
        /// 
        /// <pre>    var someClass:Class = Button;</pre>
        /// 
        /// ... then someClass will have IS_CLASS set to true.
        /// </summary>
        public const int IS_CLASS = 0x04000000;
    }
}
