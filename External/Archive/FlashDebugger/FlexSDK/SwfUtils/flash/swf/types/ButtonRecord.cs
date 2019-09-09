// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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
using DefineTag = Flash.Swf.Tags.DefineTag;
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class ButtonRecord
	{
		virtual public String Flags
		{
			get
			{
				System.Text.StringBuilder b = new System.Text.StringBuilder();
				if (blendMode != - 1)
					b.Append("hasBlendMode,");
				if (filters != null)
					b.Append("hasFilterList,");
				if (hitTest)
					b.Append("hitTest,");
				if (down)
					b.Append("down,");
				if (over)
					b.Append("over,");
				if (up)
					b.Append("up,");
				if (b.Length > 0)
					b.Length -= 1;
				return b.ToString();
			}
			
		}
		public bool hitTest;
		public bool down;
		public bool over;
		public bool up;
		
		public DefineTag characterRef;
		public int placeDepth;
		public Matrix placeMatrix;
		
		/// <summary>only valid if this record is in a DefineButton2 </summary>
		public CXFormWithAlpha colorTransform;
		public System.Collections.IList filters;
		public int blendMode = - 1; // -1 ==> not set
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is ButtonRecord)
			{
				ButtonRecord buttonRecord = (ButtonRecord) object_Renamed;
				
				if ((buttonRecord.hitTest == this.hitTest) && (buttonRecord.down == this.down) && (buttonRecord.over == this.over) && (buttonRecord.up == this.up) && (buttonRecord.blendMode == this.blendMode) && compareFilters(buttonRecord.filters, this.filters) && (((buttonRecord.characterRef == null) && (this.characterRef == null)) || ((buttonRecord.characterRef != null) && (this.characterRef != null) && buttonRecord.characterRef.Equals(this.characterRef))) && (buttonRecord.placeDepth == this.placeDepth) && (((buttonRecord.placeMatrix == null) && (this.placeMatrix == null)) || ((buttonRecord.placeMatrix != null) && (this.placeMatrix != null) && buttonRecord.placeMatrix.Equals(this.placeMatrix))) && (((buttonRecord.colorTransform == null) && (this.colorTransform == null)) || ((buttonRecord.colorTransform != null) && (this.colorTransform != null) && buttonRecord.colorTransform.Equals(this.colorTransform))))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		private bool compareFilters(System.Collections.IList filterList1, System.Collections.IList filterList2)
		{
			if (filterList1 == filterList2)
				return true;
			if (filterList1 == null || filterList2 == null)
				return false;
			if (filterList1.Count != filterList2.Count)
				return false;
			for (int i = 0, size = filterList1.Count; i < size; i++)
			{
				// TODO: should really be comparing content...
				if (filterList1[i] != filterList2[i])
				{
					return false;
				}
			}
			return true;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
