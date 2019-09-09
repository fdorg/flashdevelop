////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2004-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using DefineTag = flash.swf.tags.DefineTag;
using FrameLabel = flash.swf.tags.FrameLabel;
using SymbolClass = flash.swf.tags.SymbolClass;
using DefineFont = flash.swf.tags.DefineFont;
namespace flash.swf
{
	
	/// <summary> one SWF frame.  each frame runs its initActions, doActions, and control
	/// tags in a specific order, so we group them this way while forming the movie.
	/// 
	/// </summary>
	/// <author>  Edwin Smith
	/// </author>
	public class Frame
	{
		virtual public System.Collections.IEnumerator References
		{
			get
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList();
				
				// exported symbols
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
				for (System.Collections.IEnumerator j = exportDefs.GetEnumerator(); j.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
					DefineTag def = (DefineTag) j.Current;
					list.Add(def);
				}
				
				list.AddRange(symbolClass.class2tag.Values);
				
				// definitions for control tags
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
				for (System.Collections.IEnumerator j = controlTags.GetEnumerator(); j.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
					Tag tag = (Tag) j.Current;
					//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
					for (System.Collections.IEnumerator k = tag.References; k.MoveNext(); )
					{
						//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
						DefineTag def = (DefineTag) k.Current;
						list.Add(def);
					}
				}
				
				return list.GetEnumerator();
			}
			
		}
		virtual public System.Collections.IDictionary Exports
		{
			set
			{
				//UPGRADE_TODO: Method 'java.util.Map.entrySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilMapentrySet'"
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
				for (System.Collections.IEnumerator i = new SupportClass.HashSetSupport(value).GetEnumerator(); i.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
					System.Collections.DictionaryEntry entry = (System.Collections.DictionaryEntry) i.Current;
					DefineTag def = (DefineTag) entry.Value;
					addExport(def);
				}
			}
			
		}
		public System.Collections.IList doActions; // DoAction[]
		public System.Collections.IList controlTags; // Tag[]
		public FrameLabel label;
		public System.Collections.IList imports; // ImportAssets[]
		public int pos = 1;
		
		private System.Collections.IDictionary exports; // String -> DefineTag
		private System.Collections.IList exportDefs;
		
		public System.Collections.IList doABCs; // DoABC
		
		public SymbolClass symbolClass;
		
		public System.Collections.IList fonts;
		
		public Frame()
		{
			//UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
			exports = new System.Collections.Hashtable();
			exportDefs = new System.Collections.ArrayList();
			doActions = new System.Collections.ArrayList();
			controlTags = new System.Collections.ArrayList();
			imports = new System.Collections.ArrayList();
			fonts = new System.Collections.ArrayList();
			
			doABCs = new System.Collections.ArrayList();
			symbolClass = new SymbolClass();
		}
		
		public virtual void  mergeSymbolClass(SymbolClass symbolClass)
		{
			SupportClass.MapSupport.PutAll(this.symbolClass.class2tag, symbolClass.class2tag);
		}
		public virtual void  addSymbolClass(System.String className, DefineTag symbol)
		{
			DefineTag tag = (DefineTag) symbolClass.class2tag[className];
			// FIXME: error below should be possible... need to figure out why it is happening when running 'ant frameworks'
			//if (tag != null && ! tag.equals(symbol))
			//{
			//    throw new IllegalStateException("Attempted to define SymbolClass for " + className + " as both " +
			//            symbol + " and " + tag);
			//}
			this.symbolClass.class2tag[className] = symbol;
		}
		
		public virtual bool hasSymbolClasses()
		{
			return !(symbolClass.class2tag.Count == 0);
		}
		
		public virtual void  addExport(DefineTag def)
		{
			System.Object tempObject;
			tempObject = exports[def.name];
			exports[def.name] = def;
			System.Object old = tempObject;
			if (old != null)
			{
				exportDefs.Remove(old);
			}
			exportDefs.Add(def);
		}
		
		public virtual bool hasExports()
		{
			return !(exports.Count == 0);
		}
		
		public virtual System.Collections.IEnumerator exportIterator()
		{
			return exportDefs.GetEnumerator();
		}
		
		public virtual void  removeExport(System.String name)
		{
			System.Object tempObject;
			tempObject = exports[name];
			exports.Remove(name);
			System.Object d = tempObject;
			if (d != null)
			{
				exportDefs.Remove(d);
			}
		}
		
		public virtual bool hasFonts()
		{
			return !(fonts.Count == 0);
		}
		
		public virtual void  addFont(DefineFont tag)
		{
			fonts.Add(tag);
		}
		
		public virtual System.Collections.IEnumerator fontsIterator()
		{
			return fonts.GetEnumerator();
		}
	}
}