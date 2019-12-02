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
using TagHandler = Flash.Swf.TagHandler;
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public abstract class ShapeRecord
	{
		public virtual void  visitDependents(TagHandler h)
		{
		}
		
		public virtual void  getReferenceList(System.Collections.IList list)
		{
		}
		
		public  override bool Equals(System.Object o)
		{
			return (o is ShapeRecord);
		}
		
		public abstract void  addToDelta(int xTwips, int yTwips);
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
