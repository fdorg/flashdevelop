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
using Flash.Swf.Tags;
namespace Flash.Swf
{
	
	/// <author>  Clement Wong
	/// </author>
	public class TagHandler
	{
		virtual public SetBackgroundColor BackgroundColor
		{
			set
			{
			}
			
		}
		virtual public Dictionary DecoderDictionary
		{
			set
			{
			}
			
		}
		virtual public SetTabIndex TabIndex
		{
			set
			{
			}
			
		}
		public virtual void  setOffsetAndSize(int offset, int size)
		{
		}
		
		public virtual void  productInfo(ProductInfo tag)
		{
		}
		
		public virtual void  header(Header h)
		{
		}
		
		public virtual void  fileAttributes(FileAttributes tag)
		{
		}
		
		public virtual void  metadata(Metadata tag)
		{
		}
		
		public virtual void  showFrame(ShowFrame tag)
		{
		}
		
		public virtual void  defineShape(DefineShape tag)
		{
		}
		
		public virtual void  placeObject(PlaceObject tag)
		{
		}
		
		public virtual void  removeObject(RemoveObject tag)
		{
		}
		
		public virtual void  defineBinaryData(DefineBinaryData tag)
		{
		}
		
		public virtual void  defineFontName(DefineFontName tag)
		{
		}
		
		public virtual void  defineBits(DefineBits tag)
		{
		}
		
		public virtual void  defineButton(DefineButton tag)
		{
		}
		
		public virtual void  jpegTables(GenericTag tag)
		{
		}
		
		public virtual void  defineFont(DefineFont1 tag)
		{
		}
		
		public virtual void  defineFontAlignZones(DefineFontAlignZones tag)
		{
		}
		
		public virtual void  csmTextSettings(CSMTextSettings tag)
		{
		}
		
		public virtual void  defineText(DefineText tag)
		{
		}
		
		public virtual void  defineSceneAndFrameLabelData(DefineSceneAndFrameLabelData tag)
		{
		}
		
		public virtual void  doAction(DoAction tag)
		{
		}
		
		public virtual void  defineFontInfo(DefineFontInfo tag)
		{
		}
		
		public virtual void  defineSound(DefineSound tag)
		{
		}
		
		public virtual void  startSound(StartSound tag)
		{
		}
		
		public virtual void  defineButtonSound(DefineButtonSound tag)
		{
		}
		
		public virtual void  soundStreamHead(SoundStreamHead tag)
		{
		}
		
		public virtual void  soundStreamBlock(GenericTag tag)
		{
		}
		
		public virtual void  defineBitsLossless(DefineBitsLossless tag)
		{
		}
		
		public virtual void  defineBitsJPEG2(DefineBits tag)
		{
		}
		
		public virtual void  defineShape2(DefineShape tag)
		{
		}
		
		public virtual void  defineButtonCxform(DefineButtonCxform tag)
		{
		}
		
		public virtual void  protect(GenericTag tag)
		{
		}
		
		public virtual void  placeObject2(PlaceObject tag)
		{
		}
		
		public virtual void  placeObject3(PlaceObject tag)
		{
		}
		
		public virtual void  removeObject2(RemoveObject tag)
		{
		}
		
		public virtual void  defineShape3(DefineShape tag)
		{
		}
		
		public virtual void  defineShape6(DefineShape tag)
		{
		}
		public virtual void  defineText2(DefineText tag)
		{
		}
		
		public virtual void  defineButton2(DefineButton tag)
		{
		}
		
		public virtual void  defineBitsJPEG3(DefineBitsJPEG3 tag)
		{
		}
		
		public virtual void  defineBitsLossless2(DefineBitsLossless tag)
		{
		}
		
		public virtual void  defineEditText(DefineEditText tag)
		{
		}
		
		public virtual void  defineSprite(DefineSprite tag)
		{
		}
		
		public virtual void  defineScalingGrid(DefineScalingGrid tag)
		{
		}
		
		public virtual void  frameLabel(FrameLabel tag)
		{
		}
		
		public virtual void  soundStreamHead2(SoundStreamHead tag)
		{
		}
		
		public virtual void  defineMorphShape(DefineMorphShape tag)
		{
		}
		
		public virtual void  defineMorphShape2(DefineMorphShape tag)
		{
		}
		
		public virtual void  defineFont2(DefineFont2 tag)
		{
		}
		
		public virtual void  defineFont3(DefineFont3 tag)
		{
		}
		
		public virtual void  defineFont4(DefineFont4 tag)
		{
		}
		
		public virtual void  exportAssets(ExportAssets tag)
		{
		}
		
		public virtual void  symbolClass(SymbolClass tag)
		{
		}
		
		public virtual void  importAssets(ImportAssets tag)
		{
		}
		
		public virtual void  importAssets2(ImportAssets tag)
		{
		}
		
		public virtual void  enableDebugger(EnableDebugger tag)
		{
		}
		
		public virtual void  doInitAction(DoInitAction tag)
		{
		}
		
		public virtual void  defineVideoStream(DefineVideoStream tag)
		{
		}
		
		public virtual void  videoFrame(VideoFrame tag)
		{
		}
		
		public virtual void  defineFontInfo2(DefineFontInfo tag)
		{
		}
		
		public virtual void  enableDebugger2(EnableDebugger tag)
		{
		}
		
		public virtual void  debugID(DebugID tag)
		{
		}
		
		public virtual void  unknown(GenericTag tag)
		{
		}
		
		public virtual void  any(Tag tag)
		{
		}
		/// <summary> called when we are done, no more tags coming</summary>
		public virtual void  finish()
		{
		}
		
		public virtual void  error(String s)
		{
		}
		
		public virtual void  scriptLimits(ScriptLimits tag)
		{
		}
		
		public virtual void  doABC(DoABC tag)
		{
		}
	}
}
