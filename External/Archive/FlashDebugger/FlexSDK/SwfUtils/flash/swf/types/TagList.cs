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
using Tag = Flash.Swf.Tag;
using TagHandler = Flash.Swf.TagHandler;
using Flash.Swf.Tags;
namespace Flash.Swf.Types
{
	
	/// <summary> This is a simple container for a list of tags.  It's the physical
	/// representation of a timeline too, although strictly speaking, only
	/// the control tags are interesting on the timeline (placeobject,
	/// removeobject, startsound, showframe, etc).
	/// </summary>
	/// <author>  Clement Wong
	/// </author>
	public class TagList:TagHandler
	{
		override public SetTabIndex TabIndex
		{
			set
			{
				tags.Add(value);
			}
			
		}
		override public SetBackgroundColor BackgroundColor
		{
			set
			{
				tags.Add(value);
			}
			
		}
		public TagList()
		{
			this.tags = new System.Collections.ArrayList();
		}
		
		public System.Collections.IList tags;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is TagList)
			{
				TagList tagList = (TagList) object_Renamed;
				
				if (((tagList.tags == null) && (this.tags == null)) || ((tagList.tags != null) && (this.tags != null) && ArrayLists.equals(tagList.tags, this.tags)))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override String ToString()
		{
			System.Text.StringBuilder stringBuffer = new System.Text.StringBuilder();
			
			stringBuffer.Append("TagList:\n");
			
			for (int i = 0; i < tags.Count; i++)
			{
				stringBuffer.Append("\t" + i + " = " + tags[i] + "\n");
			}
			
			return stringBuffer.ToString();
		}
		
		public virtual void  visitTags(TagHandler handler)
		{
			int size = tags.Count;
			for (int i = 0; i < size; i++)
			{
				Tag t = (Tag) tags[i];
				t.visit(handler);
			}
		}
		
		public override void  debugID(DebugID tag)
		{
			tags.Add(tag);
		}
		
		public override void  scriptLimits(ScriptLimits tag)
		{
			tags.Add(tag);
		}
		
		public override void  showFrame(ShowFrame tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineShape(DefineShape tag)
		{
			tags.Add(tag);
		}
		
		public override void  placeObject(PlaceObject tag)
		{
			tags.Add(tag);
		}
		
		public override void  removeObject(RemoveObject tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineBits(DefineBits tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineButton(DefineButton tag)
		{
			tags.Add(tag);
		}
		
		public override void  jpegTables(GenericTag tag)
		{
			tags.Add(tag);
		}
		
		public virtual void  defineFont(DefineFont tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineText(DefineText tag)
		{
			tags.Add(tag);
		}
		
		public override void  doAction(DoAction tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineFontInfo(DefineFontInfo tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineSound(DefineSound tag)
		{
			tags.Add(tag);
		}
		
		public override void  startSound(StartSound tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineButtonSound(DefineButtonSound tag)
		{
			tags.Add(tag);
		}
		
		public override void  soundStreamHead(SoundStreamHead tag)
		{
			tags.Add(tag);
		}
		
		public override void  soundStreamBlock(GenericTag tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineBitsLossless(DefineBitsLossless tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineBitsJPEG2(DefineBits tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineShape2(DefineShape tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineButtonCxform(DefineButtonCxform tag)
		{
			tags.Add(tag);
		}
		
		public override void  protect(GenericTag tag)
		{
			tags.Add(tag);
		}
		
		public override void  placeObject2(PlaceObject tag)
		{
			tags.Add(tag);
		}
		
		public override void  placeObject3(PlaceObject tag)
		{
			tags.Add(tag);
		}
		
		public override void  removeObject2(RemoveObject tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineShape3(DefineShape tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineShape6(DefineShape tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineText2(DefineText tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineButton2(DefineButton tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineBitsJPEG3(DefineBitsJPEG3 tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineBitsLossless2(DefineBitsLossless tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineEditText(DefineEditText tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineSprite(DefineSprite tag)
		{
			tags.Add(tag);
		}
		
		public override void  frameLabel(FrameLabel tag)
		{
			tags.Add(tag);
		}
		
		public override void  soundStreamHead2(SoundStreamHead tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineMorphShape(DefineMorphShape tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineMorphShape2(DefineMorphShape tag)
		{
			tags.Add(tag);
		}
		
		public virtual void  defineFont2(DefineFont tag)
		{
			tags.Add(tag);
		}
		
		public virtual void  defineFont3(DefineFont tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineFontAlignZones(DefineFontAlignZones tag)
		{
			tags.Add(tag);
		}
		
		public override void  csmTextSettings(CSMTextSettings tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineFontName(DefineFontName tag)
		{
			tags.Add(tag);
		}
		
		public override void  exportAssets(ExportAssets tag)
		{
			tags.Add(tag);
		}
		
		public override void  importAssets(ImportAssets tag)
		{
			tags.Add(tag);
		}
		
		public override void  importAssets2(ImportAssets tag)
		{
			tags.Add(tag);
		}
		
		public override void  enableDebugger(EnableDebugger tag)
		{
			tags.Add(tag);
		}
		
		public override void  doInitAction(DoInitAction tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineScalingGrid(DefineScalingGrid tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineVideoStream(DefineVideoStream tag)
		{
			tags.Add(tag);
		}
		
		public override void  videoFrame(VideoFrame tag)
		{
			tags.Add(tag);
		}
		
		public override void  defineFontInfo2(DefineFontInfo tag)
		{
			tags.Add(tag);
		}
		
		public override void  enableDebugger2(EnableDebugger tag)
		{
			tags.Add(tag);
		}
		
		public override void  unknown(GenericTag tag)
		{
			tags.Add(tag);
		}
		
		public override void  productInfo(ProductInfo tag)
		{
			tags.Add(tag);
		}
		
		public override void  fileAttributes(FileAttributes tag)
		{
			tags.Add(tag);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
