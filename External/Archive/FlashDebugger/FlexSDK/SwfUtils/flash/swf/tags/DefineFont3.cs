// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
// ADOBE SYSTEMS INCORPORATED
// Copyright 2003-2006 Adobe Systems Incorporated
// All Rights Reserved.
//
// NOTICE: Adobe permits you to use, modify, and distribute this file
// in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using Flash.Swf;

namespace Flash.Swf.Tags
{
	
	/// <summary> The DefineFont3 tag extends the functionality of the DefineFont2 tag by
	/// expressing the Shape coordinates in the glyph shape table at 20 times the
	/// resolution. The EM square units are converted to twips to allow fractional
	/// resolution to 1/20th of a unit. The DefineFont3 tag was introduced in SWF 8.
	/// 
	/// </summary>
	/// <author>  Clement Wong
	/// </author>
	/// <author>  Peter Farland
	/// </author>
	public class DefineFont3:DefineFont2
	{
		/// <summary> Constructor.</summary>
		public DefineFont3():base(Flash.Swf.TagValues.stagDefineFont3)
		{
		}
		
		//--------------------------------------------------------------------------
		//
		// Fields and Bean Properties
		//
		//--------------------------------------------------------------------------
		
		public DefineFontAlignZones zones;
		
		//--------------------------------------------------------------------------
		//
		// Visitor Methods
		//
		//--------------------------------------------------------------------------
		
		/// <summary> Invokes the defineFont visitor on the given TagHandler.
		/// 
		/// </summary>
		/// <param name="handler">The SWF TagHandler.
		/// </param>
		public override void  visit(TagHandler handler)
		{
			if (code == Flash.Swf.TagValues.stagDefineFont3)
				handler.defineFont3(this);
		}
		
		//--------------------------------------------------------------------------
		//
		// Utility Methods
		//
		//--------------------------------------------------------------------------
		
		/// <summary> Tests whether this DefineFont3 tag is equivalent to another DefineFont3
		/// tag instance.
		/// 
		/// </summary>
		/// <param name="obj">Another DefineFont3 instance to test for equality.
		/// </param>
		/// <returns> true if the given instance is considered equal to this instance
		/// </returns>
		public  override bool Equals(System.Object obj)
		{
			bool isEqual = false;
			
			if (obj is DefineFont3 && base.Equals(obj))
			{
				DefineFont3 defineFont = (DefineFont3) obj;
				
				// DefineFontAlignZones already checks if its font is equal, so we
				// don't check here to avoid circular equality checking...
				//if (equals(defineFont.zones, this.zones))
				
				isEqual = true;
			}
			
			return isEqual;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
