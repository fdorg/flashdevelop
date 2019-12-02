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
using SwfEncoder = Flash.Swf.SwfEncoder;
using DefineTag = Flash.Swf.Tags.DefineTag;
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class Rect
	{
		virtual public int Width
		{
			get
			{
				return xMax - xMin;
			}
			
		}
		virtual public int Height
		{
			get
			{
				return yMax - yMin;
			}
			
		}
		public Rect(int width, int height)
		{
			xMax = width;
			yMax = height;
			nbits();
		}
		
		public Rect()
		{
		}
		
		public Rect(int xMin, int xMax, int yMin, int yMax)
		{
			this.xMin = xMin;
			this.xMax = xMax;
			this.yMin = yMin;
			this.yMax = yMax;
		}
		
		public int xMin;
		public int xMax;
		public int yMin;
		public int yMax;
		
		public override String ToString()
		{
			if ((xMin != 0) || (yMin != 0))
			{
				return "(" + xMin + "," + yMin + "),(" + xMax + "," + yMax + ")";
			}
			else
			{
				return new System.Text.StringBuilder().Append(xMax).Append('x').Append(yMax).ToString();
			}
		}
		
		public virtual int nbits()
		{
			int maxCoord = SwfEncoder.maxNum(xMin, xMax, yMin, yMax);
			return SwfEncoder.minBits(maxCoord, 1);
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is Rect)
			{
				Rect rect = (Rect) object_Renamed;
				
				if ((rect.xMin == this.xMin) && (rect.xMax == this.xMax) && (rect.yMin == this.yMin) && (rect.yMax == this.yMax))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode += DefineTag.PRIME * xMin;
			hashCode += DefineTag.PRIME * xMax;
			hashCode += DefineTag.PRIME * yMin;
			hashCode += DefineTag.PRIME * yMax;
			
			return hashCode;
		}
	}
}
