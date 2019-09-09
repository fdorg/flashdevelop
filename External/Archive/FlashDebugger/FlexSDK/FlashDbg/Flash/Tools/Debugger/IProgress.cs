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
	/// <summary> A simple interface to report progress on some operation.
	/// 
	/// </summary>
	/// <author>  mmorearty
	/// </author>
	public interface IProgress
	{
		/// <summary> Reports how much work has been done.
		/// 
		/// </summary>
		/// <param name="current">how much progress has been made toward the total
		/// </param>
		/// <param name="total">the total amount of work
		/// </param>
		void  setProgress(int current, int total);
	}
}
