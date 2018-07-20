// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
using Tag = Flash.Swf.Tag;
using TagHandler = Flash.Swf.TagHandler;
namespace Flash.Swf.Tags
{
	
	/// <summary> The FrameLabel tag gives the specified Name to the current frame. This name is used by
	/// ActionGoToLabel to identify the frame.
	/// 
	/// Any frame can have a FrameLabel tag but only the main timeline
	/// may have bookmark labels.  bookmark labels on sprite timelines
	/// are ignored by the player.
	/// 
	/// </summary>
	/// <author>  Clement Wong
	/// </author>
	/// <since> SWF3
	/// </since>
	public class FrameLabel:Tag
	{
		public FrameLabel():base(Flash.Swf.TagValues.stagFrameLabel)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			h.frameLabel(this);
		}
		
		/// <summary> label for the current frame</summary>
		public String label;
		
		/// <summary> named anchor flag.  labels this frame for seeking using HTML anchor syntax.</summary>
		/// <since> SWF6
		/// </since>
		public bool anchor;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is FrameLabel))
			{
				FrameLabel frameLabel = (FrameLabel) object_Renamed;
				
				if (equals(frameLabel.label, this.label) && (frameLabel.anchor == this.anchor))
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
