// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2005-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;

using Rect = Flash.Swf.Types.Rect;

namespace Flash.Swf.Tags
{
	
	/// <author>  Roger Gonzalez
	/// </author>
	public class DefineScalingGrid:Tag
	{
		public DefineScalingGrid():base(Flash.Swf.TagValues.stagDefineScalingGrid)
		{
		}
		public DefineScalingGrid(DefineTag tag):this()
		{
			System.Diagnostics.Debug.Assert(tag is DefineSprite || tag is DefineButton);
			
			if (tag is DefineSprite)
			{
				((DefineSprite) tag).scalingGrid = this;
			}
		}
		public override void  visit(TagHandler h)
		{
			h.defineScalingGrid(this);
		}
		
		public override Tag SimpleReference
		{
            get
            {
                return scalingTarget;
            }
		}
		
		public  override bool Equals(System.Object other)
		{
			return ((other is DefineScalingGrid) && ((DefineScalingGrid) other).scalingTarget == scalingTarget) && ((DefineScalingGrid) other).rect.Equals(rect);
		}
		
		public DefineTag scalingTarget;
		public Rect rect = new Rect();
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
