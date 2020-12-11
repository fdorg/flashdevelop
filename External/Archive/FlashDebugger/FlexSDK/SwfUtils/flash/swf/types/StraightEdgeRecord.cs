// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class StraightEdgeRecord:EdgeRecord
	{
		public int deltaX = 0;
		public int deltaY = 0;
		
		public virtual StraightEdgeRecord setX(int x)
		{
			deltaX = x;
			return this;
		}
		
		public virtual StraightEdgeRecord setY(int y)
		{
			deltaY = y;
			return this;
		}
		
		public StraightEdgeRecord()
		{
		}
		
		public StraightEdgeRecord(int deltaX, int deltaY)
		{
			this.deltaX = deltaX;
			this.deltaY = deltaY;
		}
		
		public override void  addToDelta(int xTwips, int yTwips)
		{
			deltaX += xTwips;
			deltaY += yTwips;
		}
		public override String ToString()
		{
			return "Line: x=" + deltaX + " y=" + deltaY;
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is StraightEdgeRecord)
			{
				StraightEdgeRecord straightEdgeRecord = (StraightEdgeRecord) object_Renamed;
				
				if ((straightEdgeRecord.deltaX == this.deltaX) && (straightEdgeRecord.deltaY == this.deltaY))
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
