////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2004-2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using flash.swf.tags;
namespace flash.swf
{
	
	/// <summary> handle parsing events for a SWF movie.  Keep track of each frame and build
	/// up a framelist.  there are a number of singleton tags in swf movies, so invoke
	/// errors if those singleton events are defined more than once.
	/// 
	/// </summary>
	/// <author>  Edwin Smith
	/// </author>
	public class MovieDecoder:TagHandler
	{
		override public SetTabIndex TabIndex
		{
			set
			{
				frame.controlTags.Add(value);
			}
			
		}
		override public SetBackgroundColor BackgroundColor
		{
			set
			{
				if (value != null)
				{
					// assume player ignores duplicate bgcolors
					error("duplicate SetBackgroundColor " + value.color);
				}
				
				m.bgcolor = value;
			}
			
		}
		private Movie m;
		private Frame frame;
		
		public MovieDecoder(Movie m)
		{
			this.m = m;
		}
		
		public override void  header(Header h)
		{
			m.version = h.version;
			m.framerate = h.rate;
			m.size = h.size;
			m.frames = new System.Collections.ArrayList(h.framecount);
			frame = new Frame();
		}
		
		public override void  finish()
		{
			// C: If there is no ShowFrame at the end, don't throw away the frame.
			if (frame != null && !m.frames.Contains(frame))
			{
				m.frames.Add(frame);
			}
			// we are done loading the movie.  now set currentFrame == null
			frame = null;
		}
		
		public override void  debugID(DebugID tag)
		{
			if (m.uuid != null)
			{
				error("duplicate uuid" + tag.uuid);
			}
			
			m.uuid = tag.uuid;
		}
		
		public override void  doAction(DoAction tag)
		{
			frame.doActions.Add(tag.actionList);
		}
		
		public override void  doInitAction(DoInitAction tag)
		{
			frame.controlTags.Add(tag);
		}
		
		public override void  enableDebugger(EnableDebugger tag)
		{
			if (m.enableDebugger != null)
			{
				error("duplicate EnableDebugger " + tag.password);
			}
			
			m.enableDebugger = tag;
		}
		
		public override void  enableDebugger2(EnableDebugger tag)
		{
			enableDebugger(tag);
		}
		
		public override void  exportAssets(ExportAssets tag)
		{
			// we only care what tags were exported in this frame, because all the
			// code in this frame could depend on those definitions.
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			for (System.Collections.IEnumerator i = tag.exports.GetEnumerator(); i.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				DefineTag def = (DefineTag) i.Current;
				frame.addExport(def);
			}
		}
		
		public override void  defineSceneAndFrameLabelData(DefineSceneAndFrameLabelData tag)
		{
			m.sceneAndFrameLabelData = tag;
		}
		
		public override void  doABC(DoABC tag)
		{
			frame.doABCs.Add(tag);
		}
		
		public override void  symbolClass(SymbolClass tag)
		{
			frame.mergeSymbolClass(tag);
			
			// populate Movie.topLevelClass if this is the first frame and SymbolClass.topLevelClass is non-null.
			if (m.frames.Count == 0 && tag.topLevelClass != null)
			{
				m.topLevelClass = tag.topLevelClass;
			}
		}
		
		public override void  frameLabel(FrameLabel tag)
		{
			if (frame.label != null)
			{
				error("duplicate label " + tag.label);
			}
			
			frame.label = tag;
		}
		
		public override void  importAssets(ImportAssets tag)
		{
			frame.imports.Add(tag);
		}
		
		public override void  importAssets2(ImportAssets tag)
		{
			frame.imports.Add(tag);
		}
		
		public override void  placeObject(PlaceObject tag)
		{
			placeObject2(tag);
		}
		
		public override void  placeObject2(PlaceObject tag)
		{
			frame.controlTags.Add(tag);
		}
		public override void  placeObject3(PlaceObject tag)
		{
			frame.controlTags.Add(tag);
		}
		
		public override void  protect(GenericTag tag)
		{
			if (m.protect != null)
			{
				error("duplicate Protect ");
			}
			
			m.protect = tag;
		}
		
		public override void  removeObject(RemoveObject tag)
		{
			removeObject2(tag);
		}
		
		public override void  removeObject2(RemoveObject tag)
		{
			frame.controlTags.Add(tag);
		}
		
		public override void  scriptLimits(ScriptLimits tag)
		{
			if (m.scriptLimits != null)
			{
				// assume player ignores duplicate scriptlimits
				error("duplicate script limits");
			}
			
			m.scriptLimits = tag;
		}
		
		public override void  showFrame(ShowFrame tag)
		{
			m.frames.Add(frame);
			frame = new Frame();
		}
		
		public override void  soundStreamBlock(GenericTag tag)
		{
			frame.controlTags.Add(tag);
		}
		
		public override void  soundStreamHead(SoundStreamHead tag)
		{
			frame.controlTags.Add(tag);
		}
		
		public override void  soundStreamHead2(SoundStreamHead tag)
		{
			frame.controlTags.Add(tag);
		}
		
		public override void  startSound(StartSound tag)
		{
			frame.controlTags.Add(tag);
		}
		
		public override void  unknown(GenericTag tag)
		{
			frame.controlTags.Add(tag);
		}
		
		public override void  videoFrame(VideoFrame tag)
		{
			frame.controlTags.Add(tag);
		}
		
		public override void  productInfo(ProductInfo tag)
		{
			frame.controlTags.Add(tag);
		}
		
		public override void  metadata(Metadata tag)
		{
			m.metadata = tag;
		}
		
		public override void  fileAttributes(FileAttributes tag)
		{
			if (m.fileAttributes != null)
			{
				error("duplicate FileAttributes");
			}
			m.fileAttributes = tag;
		}
		
		public override void  defineButtonCxform(DefineButtonCxform tag)
		{
			frame.controlTags.Add(tag);
		}
		
		public override void  defineButtonSound(DefineButtonSound tag)
		{
			frame.controlTags.Add(tag);
		}
		
		public virtual void  defineFont(DefineFont tag)
		{
			defineFont2(tag);
		}
		
		public virtual void  defineFont2(DefineFont tag)
		{
			frame.fonts.Add(tag);
		}
		
		public virtual void  defineFont3(DefineFont tag)
		{
			defineFont2(tag);
		}
		
		public override void  defineFontAlignZones(DefineFontAlignZones tag)
		{
			frame.controlTags.Add(tag);
		}
		
		public override void  csmTextSettings(CSMTextSettings tag)
		{
			frame.controlTags.Add(tag);
		}
		
		public override void  defineFontName(DefineFontName tag)
		{
			frame.controlTags.Add(tag);
		}
	}
}