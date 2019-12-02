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
namespace Flash.Swf.Debug
{
	
	/// <summary> this object holds the script for a debug module, and its name,
	/// lines, and the offset value of each of the debug offsets that points
	/// within this script.
	/// 
	/// </summary>
	/// <author>  Edwin Smith
	/// </author>
	public class DebugModule
	{
		virtual public String Text
		{
			set
			{
				this.text = value;
				
				int count = 1;
				
				int length = value.Length;
				int last;
				for (int i = eolIndexOf(value); i != - 1; i = eolIndexOf(value, last))
				{
					last = i + 1;
					count++;
				}
				// allways make room for the last line whether it is empty or not.
				count++;
				
				index = new int[count];
				index[0] = 0;
				count = 1;
				for (int i = eolIndexOf(value); i != - 1; i = eolIndexOf(value, last))
				{
					index[count++] = last = i + 1;
				}
				index[count++] = length;
				
				offsets = new int[count];
			}
			
		}
		public int id;
		public int bitmap;
		public String name;
		public String text;
		
		/// <summary>offsets[n] = offset of line n </summary>
		public int[] offsets;
		
		/// <summary>index[n] = index of end of line n in text </summary>
		public int[] index;
		
		/* is this module potentially corrupt; see 81918 */
		public bool corrupt = false;
		
		public virtual bool addOffset(LineRecord lr, int offset)
		{
			bool worked = true;
			if (lr.lineno < offsets.Length)
			{
				offsets[lr.lineno] = offset;
			}
			else
			{
				// We have a condition 81918/78188 whereby Matador can produce a swd
				// where module ids were not unique, resulting in collision of offset records.
				// The best we can do is to mark the entire module as bad
				corrupt = true;
				worked = false;
			}
			return worked;
		}
		
		public  override bool Equals(System.Object obj)
		{
			if (obj == this)
				return true;
			if (!(obj is DebugModule))
				return false;
			DebugModule other = (DebugModule) obj;
			return this.bitmap == other.bitmap && this.name.Equals(other.name) && this.text.Equals(other.text);
		}
		
		public override int GetHashCode()
		{
			return name.GetHashCode() ^ text.GetHashCode() ^ bitmap;
		}
		
		public virtual int getLineNumber(int offset)
		{
			int closestMatch = 0;
			for (int i = 0; i < offsets.Length; i++)
			{
				int delta = offset - offsets[i];
				if (delta >= 0 && delta < (offset - offsets[closestMatch]))
					closestMatch = i;
			}
			return closestMatch;
		}
		
		public static int eolIndexOf(String text)
		{
			return eolIndexOf(text, 0);
		}
		
		public static int eolIndexOf(String text, int i)
		{
			int at = - 1;
			
			// scan starting at location i
			int size = text.Length;
			while (i < size && at < 0)
			{
				char c = text[i];
				
				// newline?
				if (c == '\n')
					at = i;
				// carriage return?
				else if (c == '\r')
				{
					at = i;
					
					// might be cr/newline...chew pacman chew...
					if (i + 1 < size && text[i + 1] == '\n')
						at++;
				}
				// some crack may use form feeds?
				else if (c == '\f')
					at = i;
				
				i++;
			}
			return at;
		}
		
		[STAThread]
		public static void  Main(String[] args)
		{
			new DebugModule().Text = "hello";
		}
	}
}
