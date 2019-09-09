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
	public class Dictionary
	{
		public class AnonymousClassDefineTag:DefineTag
		{
			internal AnonymousClassDefineTag(int Param1):base(Param1)
			{
			}
			public override void  visit(TagHandler t)
			{
			}
		}
		virtual public int NextId
		{
			// method added to support Flash Authoring - jkamerer 2007.07.30
			
			get
			{
				return nextId;
			}
			
			// method added to support Flash Authoring - jkamerer 2007.07.30
			
			set
			{
				this.nextId = value;
			}
			
		}
		private static DefineTag INVALID_TAG = new AnonymousClassDefineTag(Flash.Swf.TagValues.stagEnd);
		internal System.Collections.IDictionary ids = new System.Collections.Hashtable();
		internal System.Collections.IDictionary tags = new System.Collections.Hashtable();
		internal System.Collections.IDictionary names = new System.Collections.Hashtable();
		internal System.Collections.IDictionary fonts = new System.Collections.Hashtable();
		private int nextId = 1;
		
		public virtual bool contains(int id)
		{
			return ids.Contains((System.Int32) id);
		}
		
		public virtual bool contains(DefineTag tag)
		{
			return tags.Contains(tag);
		}
		
		public virtual int getId(DefineTag tag)
		{
			if (tag == null || tag == INVALID_TAG)
			{
				return - 1; //throw new NullPointerException("no ids for null tags");
			}
			else
			{
				// when we're encoding, we should definitely find the tag here.
				System.Object idobj = tags[tag];
				
				if (idobj == null)
				{
					// When we're decoding, we don't fill in the tags map, and so we'll have
					// to search for the tag to see what it had when we read it in.
					
                    foreach (System.Collections.DictionaryEntry entry in ids)
					{
						// [ets 1/14/04] we use an exact comparison here instead of equals() because this point
						// should only be reached during *decoding*, by tools that want to report the id
						// that is only stored in the ids map.  Since each DefineTag from a single swf will
						// be a unique object, this should be safe.  During encoding, we will find ID's stored
						// in the tags map, and the ids map should be empty.  if we use equals(), it is possible
						// for tools to report the wrong ID, because the defineTags may not be fully decoded yet,
						// for example the ExportAssets may not have been reached, so the tag might not have its
						// name yet, and therefore compare equal to another unique but yet-unnamed tag.
						
						if (entry.Value == tag)
						{
							idobj = entry.Key;
							break;
						}
					}
				}
				
				if (idobj == null)
				{
					System.Diagnostics.Debug.Assert(false, "encoding error, " + tag.name + " not in dictionary");
				}
				
				return ((System.Int32) idobj);
			}
		}
		
		/// <summary> This is the method used during encoding.</summary>
		public virtual int add(DefineTag tag)
		{
			System.Diagnostics.Debug.Assert(tag != null);
            if (tags.Contains(tag))
            {
                //throw new IllegalArgumentException("symbol " +tag+ " redefined");
                return (int) tags[tag];
            }

			int key = nextId++;
			tags[tag] = key;
			ids[key] = tag;
			return key;
		}
		
		/// <summary> This is method used during decoding.
		/// 
		/// </summary>
		/// <param name="id">
		/// </param>
		/// <param name="s">
		/// </param>
		/// <throws>  IllegalArgumentException if the dictionary already has that id </throws>
		public virtual void  add(int id, DefineTag s)
		{
			System.Int32 key = (System.Int32) id;
			Tag t = (Tag) ids[key];
			if (t == null)
			{
				ids[key] = s;
				// This DefineTag is potentially very generic, for example
				// it's name is most likely null, so don't bother adding
				// it to the tags Map.
			}
			else
			{
				if (t.Equals(s))
					throw new System.ArgumentException("symbol " + id + " redefined by identical tag");
				else
					throw new System.ArgumentException("symbol " + id + " redefined by different tag");
			}
		}
		
		public virtual void  addName(DefineTag s, String name)
		{
			names[name] = s;
		}
		
		private static String makeFontKey(String name, bool bold, bool italic)
		{
			return name + (bold?"_bold_":"_normal_") + (italic?"_italic":"_regular");
		}
		public virtual void  addFontFace(DefineFont defineFont)
		{
			fonts[makeFontKey(defineFont.FontName, defineFont.Bold, defineFont.Italic)] = defineFont;
		}
		public virtual DefineFont getFontFace(String name, bool bold, bool italic)
		{
			return (DefineFont) fonts[makeFontKey(name, bold, italic)];
		}
		
		public virtual bool contains(String name)
		{
			return names.Contains(name);
		}
		
		public virtual DefineTag getTag(String name)
		{
			return (DefineTag) names[name];
		}
		
		/// <throws>  IllegalArgumentException if the id is not defined </throws>
		/// <param name="idref">
		/// </param>
		/// <returns>
		/// </returns>
		public virtual DefineTag getTag(int idref)
		{
			System.Int32 key = (System.Int32) idref;
			DefineTag t = (DefineTag) ids[key];
			if (t == null)
			{
				// [tpr 7/6/04] work around authoring tool bug of bogus 65535 ids
				if (idref != 65535)
					throw new System.ArgumentException("symbol " + idref + " not defined");
				else
					return INVALID_TAG;
			}
			return t;
		}
	}
}
