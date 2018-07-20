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
using SwfEncoder = Flash.Swf.SwfEncoder;

namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class StyleChangeRecord:ShapeRecord
	{
		virtual public int FillStyle1
		{
			set
			{
				stateFillStyle1 = true;
				fillstyle1 = value;
			}
			
		}
		virtual public int FillStyle0
		{
			set
			{
				stateFillStyle0 = true;
				fillstyle0 = value;
			}
			
		}
		virtual public int Linestyle
		{
			set
			{
				stateLineStyle = true;
				linestyle = value;
			}
			
		}
		public bool stateNewStyles;
		public bool stateLineStyle;
		public bool stateFillStyle1;
		public bool stateFillStyle0;
		public bool stateMoveTo;
		
		public int moveDeltaX;
		public int moveDeltaY;
		
		public int fillstyle0;
		/// <summary> This is an index into the fillstyles array starting with 1.</summary>
		public int fillstyle1;
		public int linestyle;
		
		public System.Collections.ArrayList fillstyles;
		public System.Collections.ArrayList linestyles;
		
		public StyleChangeRecord()
		{
		}
		
		public StyleChangeRecord(bool stateFillStyle1, int fillstyle1, bool stateMoveTo, int moveDeltaX, int moveDeltaY)
		{
			this.stateFillStyle1 = stateFillStyle1;
			this.fillstyle1 = fillstyle1;
			this.stateMoveTo = stateMoveTo;
			this.moveDeltaX = moveDeltaX;
			this.moveDeltaY = moveDeltaY;
		}
		
		public override String ToString()
		{
			String retVal = "Style: newStyle=" + stateNewStyles + " lineStyle=" + stateLineStyle + " fillStyle=" + stateFillStyle1 + " fileStyle0=" + stateFillStyle0 + " moveTo=" + stateMoveTo;
			
			if (stateMoveTo)
			{
				retVal += (" x=" + moveDeltaX + " y=" + moveDeltaY);
			}
			
			return retVal;
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is StyleChangeRecord))
			{
				StyleChangeRecord styleChangeRecord = (StyleChangeRecord) object_Renamed;
				
				if ((styleChangeRecord.stateNewStyles == this.stateNewStyles) && (styleChangeRecord.stateLineStyle == this.stateLineStyle) && (styleChangeRecord.stateFillStyle1 == this.stateFillStyle1) && (styleChangeRecord.stateFillStyle0 == this.stateFillStyle0) && (styleChangeRecord.stateMoveTo == this.stateMoveTo) && (styleChangeRecord.moveDeltaX == this.moveDeltaX) && (styleChangeRecord.moveDeltaY == this.moveDeltaY) && (styleChangeRecord.fillstyle0 == this.fillstyle0) && (styleChangeRecord.fillstyle1 == this.fillstyle1) && (styleChangeRecord.linestyle == this.linestyle) && (((styleChangeRecord.fillstyles == null) && (this.fillstyles == null)) || ((styleChangeRecord.fillstyles != null) && (this.fillstyles != null) && ArrayLists.equals(styleChangeRecord.fillstyles, this.fillstyles))) && (((styleChangeRecord.linestyles == null) && (this.linestyles == null)) || ((styleChangeRecord.linestyles != null) && (this.linestyles != null) && ArrayLists.equals(styleChangeRecord.linestyles, this.linestyles))))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public virtual void  setMove(int x, int y)
		{
			stateMoveTo = true;
			moveDeltaX = x;
			moveDeltaY = y;
		}
		
		public override void  getReferenceList(System.Collections.IList list)
		{
			if (fillstyles != null)
			{
                foreach (FillStyle style in fillstyles)
				{
					if (style.hasBitmapId() && style.bitmap != null)
						list.Add(style.bitmap);
				}
			}
		}
		
		public virtual int nMoveBits()
		{
			return SwfEncoder.minBits(SwfEncoder.maxNum(moveDeltaX, moveDeltaY, 0, 0), 1);
		}
		
		public override void  addToDelta(int x, int y)
		{
			moveDeltaX += x;
			moveDeltaY += y;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
