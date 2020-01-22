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
namespace Flash.Util
{
	
	/// <summary> This class servers as a proxy to java.util.Arrays so that the
	/// J# implementation does not need to directly reference this class.
	/// 
	/// See macromedia/util/ArrayUtil.jsl for the .NET implementation.
	/// 
	/// </summary>
	public class ArrayUtil
	{
		public ArrayUtil()
		{
		}
		
		public static void  sort(System.Object[] a)
		{
			System.Array.Sort(a);
		}
		
		public static void  sort(System.Object[] a, System.Collections.IComparer c)
		{
			System.Array.Sort(a, c);
		}
		
		public static bool equals(System.Object[] a1, System.Object[] a2)
		{
			return SupportClass.ArraySupport.Equals(a1, a2);
		}
		
		public static bool equals(byte[] a1, byte[] a2)
		{
			return SupportClass.ArraySupport.Equals(a1, a2);
		}
		
		public static bool equals(long[] a1, long[] a2)
		{
			return SupportClass.ArraySupport.Equals(a1, a2);
		}
		
		public static bool equals(int[] a1, int[] a2)
		{
			return SupportClass.ArraySupport.Equals(a1, a2);
		}
		
		public static bool equals(double[] a1, double[] a2)
		{
			return SupportClass.ArraySupport.Equals(a1, a2);
		}
		
		public static bool equals(char[] a1, char[] a2)
		{
			return SupportClass.ArraySupport.Equals(a1, a2);
		}
		
		public static bool equals(short[] a1, short[] a2)
		{
			return SupportClass.ArraySupport.Equals(a1, a2);
		}
	}
}
