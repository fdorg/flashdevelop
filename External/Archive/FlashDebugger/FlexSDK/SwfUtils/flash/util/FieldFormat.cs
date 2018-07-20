// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
namespace Flash.Util
{
	
	/// <summary> Format a sequence of characters into a fixed length field
	/// justifying if applied.
	/// </summary>
	public class FieldFormat
	{
		public const int ALIGN_UNKNOWN = 0;
		public const int ALIGN_LEFT = 1;
		public const int ALIGN_RIGHT = 2;
		public const int ALIGN_CENTER = 3;
		
		// Right justifies a long value into a hex field with leading zeros
		public static System.Text.StringBuilder formatLongToHex(System.Text.StringBuilder sb, long v, int length)
		{
			return formatLongToHex(sb, v, length, true);
		}
		
		// Right justifies a long value into a field optionally zero filling the opening. 
		public static System.Text.StringBuilder formatLongToHex(System.Text.StringBuilder sb, long v, int length, bool leadingZeros)
		{
			return format(sb, System.Convert.ToString(v, 16), length, ALIGN_RIGHT, ((leadingZeros)?'0':' '), ' ');
		}
		
		// Right justifies a long value into a fixed length field
		public static System.Text.StringBuilder formatLong(System.Text.StringBuilder sb, long v, int length)
		{
			return formatLong(sb, v, length, false);
		}
		public static System.Text.StringBuilder formatLong(System.Text.StringBuilder sb, long v, int length, bool leadingZeros)
		{
			return format(sb, System.Convert.ToString(v), length, ((leadingZeros)?ALIGN_RIGHT:ALIGN_LEFT), ((leadingZeros)?'0':' '), ' ');
		}
		
		// basis for all formats 
		public static System.Text.StringBuilder format(System.Text.StringBuilder sb, String chars, int length, int alignment, char preFieldCharacter, char postFieldCharacter)
		{
			// find the length of our string 
			int charsCount = chars.Length;
			
			// position within the field
			int startAt = 0;
			if (alignment == ALIGN_RIGHT)
				startAt = length - charsCount;
			else if (alignment == ALIGN_CENTER)
				startAt = (length - charsCount) / 2;
			
			// force to right it off edge
			if (startAt < 0)
				startAt = 0;
			
			// truncate it
			if ((startAt + charsCount) > length)
				charsCount = length - startAt;
			
			// now add the pre-field if any
			for (int i = 0; i < startAt; i++)
				sb.Append(preFieldCharacter);
			
			// the content
			sb.Append(chars.Substring(0, (charsCount) - (0)));
			
			// post field if any
			for (int i = startAt + charsCount; i < length; i++)
				sb.Append(postFieldCharacter);
			
			return sb;
		}
	}
}
