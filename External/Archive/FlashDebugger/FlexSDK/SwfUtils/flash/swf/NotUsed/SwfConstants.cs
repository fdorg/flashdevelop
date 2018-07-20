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
	public struct SwfConstants_Fields{
		/// <summary> Assumes a resolution of 72 dpi, at which the Macromedia Flash Player
		/// renders 20 twips to a pixel.
		/// </summary>
		public readonly static int TWIPS_PER_PIXEL = 20;
		public readonly static int WIDE_OFFSET_THRESHOLD = 65535;
		public readonly static float FIXED_POINT_MULTIPLE = 65536.0F;
		public readonly static float FIXED_POINT_MULTIPLE_8 = 256.0F;
		public readonly static float MORPH_MAX_RATIO = 65535.0F;
		public readonly static int GRADIENT_SQUARE = 32768;
		public readonly static int LANGCODE_DEFAULT = 0;
		public readonly static int LANGCODE_LATIN = 1;
		public readonly static int LANGCODE_JAPANESE = 2;
		public readonly static int LANGCODE_KOREAN = 3;
		public readonly static int LANGCODE_SIMPLIFIED_CHINESE = 4;
		public readonly static int LANGCODE_TRADIIONAL_CHINESE = 5;
		public readonly static int TEXT_ALIGN_LEFT = 0;
		public readonly static int TEXT_ALIGN_RIGHT = 1;
		public readonly static int TEXT_ALIGN_CENTER = 2;
		public readonly static int TEXT_ALIGN_JUSTIFY = 3;
	}
	public interface SwfConstants
	{
		//UPGRADE_NOTE: Members of interface 'SwfConstants' were extracted into structure 'SwfConstants_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
		
	}
}