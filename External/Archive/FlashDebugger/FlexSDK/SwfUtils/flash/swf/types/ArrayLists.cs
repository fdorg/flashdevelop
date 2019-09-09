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
namespace Flash.Swf.Types
{
	
	/// <summary> If you want to use ArrayList.toArray() so that you can use Arrays.equals,
	/// please use this class.
	/// </summary>
	public class ArrayLists
	{
		public static bool equals(System.Collections.IList a1, System.Collections.IList a2)
		{
			if (a1 == a2)
			{
				return true;
			}
			
			if (a1 == null || a2 == null)
			{
				return false;
			}
			
			int length = a1.Count;
			if (a2.Count != length)
			{
				return false;
			}
			
			for (int i = 0; i < length; i++)
			{
				System.Object o1 = a1[i];
				System.Object o2 = a2[i];
				if (!(o1 == null?o2 == null:o1.Equals(o2)))
				{
					return false;
				}
			}
			
			return true;
		}
	}
}
