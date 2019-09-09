// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2005-2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
namespace Flash.Tools.Debugger.Concrete
{
	
	/// <summary> Flags to the OutGetVariable and OutGetVariableWhichInvokesGetter commands
	/// which are sent from the debugger to the player.
	/// 
	/// These values must be kept in sync with 'enum OutGetVariableFlags' in
	/// the player's playerdebugger.h file.
	/// 
	/// </summary>
	/// <author>  mmorearty
	/// </author>
	public class GetVariableFlag
	{
		/// <summary> Indicates that if the variable which is being retrieved is a
		/// getter, then the player should invoke the getter and return
		/// the result.  If this flag is *not* set, then the player will
		/// simply return the address of the getter itself.
		/// </summary>
		public const int INVOKE_GETTER = 0x00000001;
		/// <summary> Indicates that if the variable which is being retrieved is a
		/// compound object (e.g. an instance of a class, as opposed to
		/// a string or int or something like that), then the player
		/// should also return all of the child members of the object.
		/// </summary>
        public const int ALSO_GET_CHILDREN = 0x00000002;
		/// <summary> Indicates that when retrieving children, we only want fields
		/// and getters -- we are not interested in regular functions.
		/// This is an optimization to decrease the amount of network
		/// traffic.
		/// </summary>
        public const int DONT_GET_FUNCTIONS = 0x00000004;
		/// <summary> Indicates that when retrieving children, we also want to
		/// know exactly which class each child was defined in.  For
		/// example, if the variable is of class Foo which extends
		/// class Bar, we want to know which member fields came from
		/// Foo and which ones came from Bar.
		/// </summary>
        public const int GET_CLASS_HIERARCHY = 0x00000008;
	}
}