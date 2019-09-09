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
using TagHandler = Flash.Swf.TagHandler;
using Rect = Flash.Swf.Types.Rect;
using Shape = Flash.Swf.Types.Shape;
using MorphFillStyle = Flash.Swf.Types.MorphFillStyle;
using MorphLineStyle = Flash.Swf.Types.MorphLineStyle;
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class DefineMorphShape:DefineTag
	{
		override public System.Collections.IEnumerator References
		{
			get
			{
				// This is yucky.
				System.Collections.IList refs = new System.Collections.ArrayList();
				
				startEdges.getReferenceList(refs);
				endEdges.getReferenceList(refs);
				
				return refs.GetEnumerator();
			}
			
		}
		public DefineMorphShape(int code):base(code)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			if (code == Flash.Swf.TagValues.stagDefineMorphShape)
				h.defineMorphShape(this);
			// if (code == stagDefineMorphShape2)
			else
				h.defineMorphShape2(this);
		}
		
		public Rect startBounds;
		public Rect endBounds;
		public Rect startEdgeBounds;
		public Rect endEdgeBounds;
		public int reserved;
		public bool usesNonScalingStrokes;
		public bool usesScalingStrokes;
		public MorphFillStyle[] fillStyles;
		public MorphLineStyle[] lineStyles;
		public Shape startEdges;
		public Shape endEdges;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineMorphShape))
			{
				DefineMorphShape defineMorphShape = (DefineMorphShape) object_Renamed;
				
				if (defineMorphShape.code == this.code && equals(defineMorphShape.startBounds, this.startBounds) && equals(defineMorphShape.endBounds, this.endBounds) && equals(defineMorphShape.fillStyles, this.fillStyles) && equals(defineMorphShape.lineStyles, this.lineStyles) && equals(defineMorphShape.startEdges, this.startEdges) && equals(defineMorphShape.endEdges, this.endEdges))
				{
					isEqual = true;
					if (this.code == Flash.Swf.TagValues.stagDefineMorphShape2)
					{
						isEqual = equals(defineMorphShape.startEdgeBounds, this.startEdgeBounds) && equals(defineMorphShape.endEdgeBounds, this.endEdgeBounds) && defineMorphShape.usesNonScalingStrokes == this.usesNonScalingStrokes && defineMorphShape.usesScalingStrokes == this.usesScalingStrokes;
					}
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
