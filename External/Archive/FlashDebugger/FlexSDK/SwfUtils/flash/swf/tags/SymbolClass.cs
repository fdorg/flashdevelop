// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
using TagHandler = Flash.Swf.TagHandler;
using Tag = Flash.Swf.Tag;
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class SymbolClass:Tag
	{
		override public System.Collections.IEnumerator References
		{
			get
			{
				return class2tag.Values.GetEnumerator();
			}
			
		}
		public SymbolClass():base(Flash.Swf.TagValues.stagSymbolClass)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			h.symbolClass(this);
		}
		
		public System.Collections.IDictionary class2tag = new System.Collections.Hashtable();
		public String topLevelClass;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is SymbolClass))
			{
				SymbolClass symbolClasses = (SymbolClass) object_Renamed;
				
				if (equals(symbolClasses.class2tag, this.class2tag))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
