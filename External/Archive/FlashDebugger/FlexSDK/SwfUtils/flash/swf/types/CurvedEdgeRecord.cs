// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
	public class CurvedEdgeRecord:EdgeRecord
	{
		public int controlDeltaX;
		public int controlDeltaY;
		public int anchorDeltaX;
		public int anchorDeltaY;
		
		public override void  addToDelta(int xTwips, int yTwips)
		{
			anchorDeltaX += xTwips;
			anchorDeltaY += yTwips;
		}
		
		public override String ToString()
		{
			return "Curve: cx=" + controlDeltaX + " cy=" + controlDeltaY + " dx=" + anchorDeltaX + " dy=" + anchorDeltaY;
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is CurvedEdgeRecord))
			{
				CurvedEdgeRecord curvedEdgeRecord = (CurvedEdgeRecord) object_Renamed;
				
				if ((curvedEdgeRecord.controlDeltaX == this.controlDeltaX) && (curvedEdgeRecord.controlDeltaY == this.controlDeltaY) && (curvedEdgeRecord.anchorDeltaX == this.anchorDeltaX) && (curvedEdgeRecord.anchorDeltaY == this.anchorDeltaY))
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
