// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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
using Tag = Flash.Swf.Tag;
namespace Flash.Swf.Tags
{
	
	public abstract class DefineTag:Tag
	{
		virtual public int ID
		{
			get
			{
				return id;
			}
			
			set
			{
				this.id = value;
			}
			
		}
		public DefineTag(int code):base(code)
		{
		}
		
		/// <summary>the export name of this symbol, or null if the symbol is not exported </summary>
		public String name;
		private int id;
		public const int PRIME = 1000003;
		
		public override String ToString()
		{
			return name != null?name:base.ToString();
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineTag))
			{
				DefineTag defineTag = (DefineTag) object_Renamed;
				
				if (equals(defineTag.name, this.name))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			
			if (name != null)
			{
				hashCode ^= name.GetHashCode() << 1;
			}
			
			return hashCode;
		}
	}
}
