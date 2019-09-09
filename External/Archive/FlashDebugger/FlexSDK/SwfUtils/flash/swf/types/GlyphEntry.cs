////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2003-2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using ZoneRecord = Flash.Swf.Tags.ZoneRecord;
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class GlyphEntry : System.ICloneable
	{
		virtual public int Index
		{
			get
			{
				int result;
				
				if (original != null)
				{
					result = original.Index;
				}
				else
				{
					result = index;
				}
				
				return result;
			}
			
			set
			{
				this.index = value;
			}
			
		}
		virtual public Shape Shape
		{
			get
			{
				return this.shape;
			}
			
			// Retained for coldfusion.document.CFFontManager implementation
			
			set
			{
				this.shape = value;
			}
			
		}
		private GlyphEntry original;
		private int index;
		public int advance;
		
		//Utilities for DefineFont
		public char character;
		public Rect bounds;
		public ZoneRecord zoneRecord;
		public Shape shape;
		
		public virtual System.Object Clone()
		{
			System.Object clone = null;
			
			try
			{
				clone = base.MemberwiseClone();
				((GlyphEntry) clone).original = this;
			}
			catch (Exception cloneNotSupportedException)
			{
				// preilly: We should never get here, but just in case print a stack trace.
                Console.Error.Write(cloneNotSupportedException.StackTrace);
                Console.Error.Flush();
            }
			
			return clone;
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is GlyphEntry)
			{
				GlyphEntry glyphEntry = (GlyphEntry) object_Renamed;
				
				if ((glyphEntry.index == this.index) && (glyphEntry.advance == this.advance))
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
