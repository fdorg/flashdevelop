// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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

using Flash.Swf;
using Flash.Swf.Types;

namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class DefineShape:DefineTag
	{
		override public System.Collections.IEnumerator References
		{
			get
			{
				System.Collections.ArrayList refs = new System.Collections.ArrayList();
				
				shapeWithStyle.getReferenceList(refs);
				
				return refs.GetEnumerator();
			}
			
		}
		public DefineShape(int code):base(code)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			switch (code)
			{
				
				case Flash.Swf.TagValues.stagDefineShape: 
					h.defineShape(this);
					break;
				
				case Flash.Swf.TagValues.stagDefineShape2: 
					h.defineShape2(this);
					break;
				
				case Flash.Swf.TagValues.stagDefineShape3: 
					h.defineShape3(this);
					break;
				
				case Flash.Swf.TagValues.stagDefineShape6: 
					h.defineShape6(this);
					break;
				
				default:
					System.Diagnostics.Debug.Assert(false);
					break;
				
			}
		}
		
		public Rect bounds;
		public ShapeWithStyle shapeWithStyle;
		public bool usesNonScalingStrokes;
		public bool usesScalingStrokes;
		public Rect edgeBounds;
		
		public DefineScalingGrid scalingGrid;
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineShape))
			{
				DefineShape defineShape = (DefineShape) object_Renamed;
				
				if (equals(defineShape.bounds, this.bounds) && equals(defineShape.shapeWithStyle, this.shapeWithStyle) && equals(defineShape.edgeBounds, this.edgeBounds) && (defineShape.usesNonScalingStrokes == this.usesNonScalingStrokes) && (defineShape.usesScalingStrokes == this.usesScalingStrokes))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode += DefineTag.PRIME * bounds.GetHashCode();
			if (shapeWithStyle.shapeRecords != null)
			{
				hashCode += DefineTag.PRIME * shapeWithStyle.shapeRecords.Count;
			}
			if (shapeWithStyle.linestyles != null)
			{
				hashCode += DefineTag.PRIME * shapeWithStyle.linestyles.Count;
			}
			return hashCode;
		}
	}
}
