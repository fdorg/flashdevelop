// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2005-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;

namespace Flash.Tools.Debugger.Concrete
{
	
	/// <author>  mmorearty
	/// </author>
	public class StandalonePlayer:AbstractPlayer
	{
		override public int Type
		{
			/*
			* @see Flash.Tools.Debugger.Player#getType()
			*/
			
			get
			{
				return Flash.Tools.Debugger.Player.STANDALONE;
			}
			
		}
		/// <param name="path">
		/// </param>
		public StandalonePlayer(FileInfo path):base(null, path)
		{
		}
	}
}
