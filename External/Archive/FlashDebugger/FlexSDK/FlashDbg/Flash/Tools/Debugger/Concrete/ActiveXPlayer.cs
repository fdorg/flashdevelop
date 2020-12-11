// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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
using System.IO;

namespace Flash.Tools.Debugger.Concrete
{
	
	/// <author>  mmorearty
	/// </author>
	public class ActiveXPlayer:AbstractPlayer
	{
		override public int Type
		{
			/*
			* @see Flash.Tools.Debugger.Player#getType()
			*/
			
			get
			{
				return Player.ACTIVEX;
			}
			
		}
		public ActiveXPlayer(FileInfo iexploreExe, FileInfo path):base(iexploreExe, path)
		{
		}
	}
}