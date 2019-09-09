// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
using ActionList = flash.swf.types.ActionList;
using ImportRecord = flash.swf.types.ImportRecord;
namespace flash.swf
{
	
	/*
	* Encode movies by traversing them and calling the taghandler with each
	* tag of interest.  this class encapsulates knowlege about how the flash
	* player executes.  In particular, the order of execution of initActions
	* and frame actions.
	*
	* @author Edwin Smith
	*/
	public class MovieEncoder
	{
		private TagHandler handler;
		//UPGRADE_TODO: Class 'java.util.HashSet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashSet'"
		private SupportClass.HashSetSupport done;
		
		public MovieEncoder(TagHandler handler)
		{
			this.handler = handler;
			//UPGRADE_TODO: Class 'java.util.HashSet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashSet'"
			done = new SupportClass.HashSetSupport();
		}
		
		public virtual void  export(Movie m)
		{
			// define the header
			Header h = new Header();
			h.version = m.version;
			h.compressed = Header.useCompression(m.version);
			h.size = m.size;
			h.rate = m.framerate;
			
			handler.header(h);
			
			// movie-wide tags
			if (m.fileAttributes != null)
			{
				if (m.metadata != null)
					m.fileAttributes.hasMetadata = true;
				
				m.fileAttributes.visit(handler); // FileAttributes MUST be first tag after header!
			}
			if (m.metadata != null)
			{
				m.metadata.visit(handler);
			}
			if (m.enableDebugger != null)
			{
				m.enableDebugger.visit(handler);
			}
			if (m.uuid != null)
			{
				new DebugID(m.uuid).visit(handler);
			}
			if (m.protect != null)
			{
				m.protect.visit(handler);
			}
			if (m.scriptLimits != null)
			{
				m.scriptLimits.visit(handler);
			}
			if (m.bgcolor != null)
			{
				m.bgcolor.visit(handler);
			}
			if (m.productInfo != null)
			{
				m.productInfo.visit(handler);
			}
			if (m.sceneAndFrameLabelData != null)
			{
				m.sceneAndFrameLabelData.visit(handler);
			}
			
			// finally, output the frames
			bool associateRootClass = (m.topLevelClass != null);
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			for (System.Collections.IEnumerator i = m.frames.GetEnumerator(); i.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				Frame frame = (Frame) i.Current;
				
				if (frame.label != null)
				{
					frame.label.visit(handler);
				}
				
				if (!(frame.imports.Count == 0))
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
					for (System.Collections.IEnumerator j = frame.imports.GetEnumerator(); j.MoveNext(); )
					{
						//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
						ImportAssets importAssets = (ImportAssets) j.Current;
						importAssets.visit(handler);
					}
				}
				
				// definitions needed in this frame
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
				for (System.Collections.IEnumerator j = frame.References; j.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
					DefineTag ref_Renamed = (DefineTag) j.Current;
					define(ref_Renamed);
				}
				
				// exports
				if (frame.hasExports())
				{
					ExportAssets exportAssets = new ExportAssets();
					//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
					for (System.Collections.IEnumerator j = frame.exportIterator(); j.MoveNext(); )
					{
						//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
						DefineTag tag = (DefineTag) j.Current;
						exportAssets.exports.Add(tag);
					}
					exportAssets.visit(handler);
				}
				
				// TODO: Review this... temporarily special casing fonts here as they should not be
				// included in ExportAssets as they are not required to be exported by name!
				
				// fonts
				if (frame.hasFonts())
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
					for (System.Collections.IEnumerator k = frame.fontsIterator(); k.MoveNext(); )
					{
						//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
						DefineFont tag = (DefineFont) k.Current;
						
						// We may have already visited this font because of symbolClasses.
						if (!done.Contains(tag))
						{
							tag.visit(handler);
							done.Add(tag);
						}
					}
				}
				
				// abc tags
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
				for (System.Collections.IEnumerator j = frame.doABCs.GetEnumerator(); j.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
					Tag tag = (Tag) j.Current;
					tag.visit(handler);
				}
				
				SymbolClass classes = new SymbolClass();
				
				if (frame.hasSymbolClasses())
				{
					SupportClass.MapSupport.PutAll(classes.class2tag, frame.symbolClass.class2tag);
				}
				if (associateRootClass)
				{
					// only works on frame 1
					classes.topLevelClass = m.topLevelClass; // Why do we do this on every frame's symclass?
				}
				if (associateRootClass || frame.hasSymbolClasses())
				{
					classes.visit(handler);
				}
				associateRootClass = false;
				
				// control tags
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
				for (System.Collections.IEnumerator j = frame.controlTags.GetEnumerator(); j.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
					Tag tag = (Tag) j.Current;
					tag.visit(handler);
				}
				
				// then frame actions
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
				for (System.Collections.IEnumerator j = frame.doActions.GetEnumerator(); j.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
					ActionList list = (ActionList) j.Current;
					new DoAction(list).visit(handler);
				}
				
				// oh yeah, then showFrame!
				new ShowFrame().visit(handler);
			}
			
			handler.finish();
		}
		
		// changed from private to public to support Flash Authoring - jkamerer 2007.07.30
		public virtual void  define(Tag tag)
		{
			if (!done.Contains(tag))
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
				for (System.Collections.IEnumerator i = tag.References; i.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
					Tag ref_Renamed = (Tag) i.Current;
					define(ref_Renamed);
				}
				// ImportRecords are pre-visited via their parent ImportAssets tag.
				if (!(tag is ImportRecord))
				{
					tag.visit(handler);
					
					// FIXME: we really need a general handler for references that should be handled after the
					// parent tag is visited.  Or maybe all references can be changed so that they are handled
					// after the main tag is visited?
					
					Tag visitAfter = null;
					if (tag is DefineSprite)
					{
						visitAfter = ((DefineSprite) tag).scalingGrid;
					}
					else if (tag is DefineButton)
					{
						visitAfter = ((DefineButton) tag).scalingGrid;
					}
					else if (tag is DefineShape)
					{
						visitAfter = ((DefineShape) tag).scalingGrid;
					}
					else if (tag is DefineFont3)
					{
						visitAfter = ((DefineFont3) tag).zones;
					}
					else if (tag is DefineEditText)
					{
						visitAfter = ((DefineEditText) tag).csmTextSettings;
					}
					else if (tag is DefineText)
					{
						visitAfter = ((DefineText) tag).csmTextSettings;
					}
					
					this.visitAfter(visitAfter);
					
					visitAfter = null;
					if (tag is DefineFont)
					{
						visitAfter = ((DefineFont) tag).license;
					}
					
					this.visitAfter(visitAfter);
				}
				done.Add(tag);
			}
		}
		
		private void  visitAfter(Tag visitAfter)
		{
			if (visitAfter != null)
			{
				//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
				assert !done.contains(visitAfter);
				visitAfter.visit(handler);
				done.Add(visitAfter);
			}
		}
	}
}