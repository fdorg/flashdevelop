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
namespace flash.swf
{
	
	/// <author>  Peter Farland
	/// </author>
	public class SwfUtils : SwfConstants
	{
		/// <summary> Forces a value within the range of 0 to 255.
		/// 
		/// </summary>
		/// <param name="raw">the initial value
		/// </param>
		/// <returns> a value forced between 0 and 255
		/// </returns>
		public static int applyRange255(int raw)
		{
			if (raw < 0)
				raw = 0;
			else if (raw > 255)
				raw = 255;
			
			return raw;
		}
		
		/// <param name="color">a color object
		/// </param>
		/// <returns> integer representation, as 0xAARRGGBB
		/// </returns>
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		public static int colorToInt(ref System.Drawing.Color color)
		{
			//UPGRADE_TODO: The equivalent in .NET for method 'java.awt.Color.getBlue' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
			//UPGRADE_TODO: The equivalent in .NET for method 'java.awt.Color.getGreen' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
			//UPGRADE_TODO: The equivalent in .NET for method 'java.awt.Color.getRed' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
			//UPGRADE_TODO: The equivalent in .NET for method 'java.awt.Color.getAlpha' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
			return (((int) color.B & 0xFF) | (((int) color.G & 0xFF) << 8) | (((int) color.R & 0xFF) << 16) | (((int) color.A & 0xFF) << 24));
		}
		
		/// <param name="r">
		/// </param>
		/// <param name="g">
		/// </param>
		/// <param name="b">
		/// </param>
		/// <returns> 0x00RRGGBB
		/// </returns>
		public static int colorToInt(int r, int g, int b)
		{
			return ((b & 0xFF) | ((g & 0xFF) << 8) | ((r & 0xFF) << 16));
		}
		
		/// <param name="r">
		/// </param>
		/// <param name="g">
		/// </param>
		/// <param name="b">
		/// </param>
		/// <param name="a">
		/// </param>
		/// <returns> 0xAARRGGBB
		/// </returns>
		public static int colorToInt(int r, int g, int b, int a)
		{
			return ((b & 0xFF) | ((g & 0xFF) << 8) | ((r & 0xFF) << 16) | ((a & 0xFF) << 24));
		}
		
		/// <param name="i">as 0xAARRGGBB
		/// </param>
		/// <returns>
		/// </returns>
		public static System.Drawing.Color intToColor(int i)
		{
			return System.Drawing.Color.FromArgb(i >> 24 & 0xFF, (i >> 16) & 0xFF, i >> 8 & 0xFF, i & 0xFF);
		}
		
		public static int toTwips(double d)
		{
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			return (int) System.Math.Round((double) (d * flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL));
		}
		
		public static double fromTwips(int d)
		{
			return d / flash.swf.SwfConstants_Fields.TWIPS_PER_PIXEL;
		}
		
		public static System.String toUnicodePoint(char c)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(5);
			for (int i = 3; i >= 0; i--)
			{
				//UPGRADE_ISSUE: Method 'java.lang.Character.forDigit' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangCharacterforDigit_int_int'"
				sb.Append(Character.forDigit((c >> (4 * i)) & 15, 16));
			}
			return sb.ToString();
		}
	}
}