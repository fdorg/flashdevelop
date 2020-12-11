// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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
using Tag = Flash.Swf.Tag;
using TagHandler = Flash.Swf.TagHandler;
using Header = Flash.Swf.Header;
using TagList = Flash.Swf.Types.TagList;
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class DefineSprite:DefineTag
	{
		override public System.Collections.IEnumerator References
		{
			get
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList();

                foreach (Tag tag in tagList.tags)
				{
					for (System.Collections.IEnumerator j = tag.References; j.MoveNext(); )
					{
						list.Add(j.Current);
					}
				}

                return list.GetEnumerator();
			}
			
		}
		public DefineSprite():base(Flash.Swf.TagValues.stagDefineSprite)
		{
			this.tagList = new TagList();
		}
		
		public DefineSprite(String name):this()
		{
			this.name = name;
		}
		
		public DefineSprite(DefineSprite source):this()
		{
			this.name = source.name;
			SupportClass.ICollectionSupport.AddAll(this.tagList.tags, source.tagList.tags);
			this.initAction = source.initAction;
			this.framecount = source.framecount;
			this.header = source.header;
			if (source.scalingGrid != null)
			{
				scalingGrid = new DefineScalingGrid();
				scalingGrid.scalingTarget = this;
				scalingGrid.rect.xMin = source.scalingGrid.rect.xMin;
				scalingGrid.rect.xMax = source.scalingGrid.rect.xMax;
				scalingGrid.rect.yMin = source.scalingGrid.rect.yMin;
				scalingGrid.rect.yMax = source.scalingGrid.rect.yMax;
			}
		}
		
		public override void  visit(TagHandler h)
		{
			h.defineSprite(this);
		}
		
		public int framecount;
		public TagList tagList;
		public DoInitAction initAction;
		public DefineScalingGrid scalingGrid;
		
		// the header of the SWF this sprite originally came from.  Tells us its framerate and SWF version.
		public Header header;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineSprite))
			{
				DefineSprite defineSprite = (DefineSprite) object_Renamed;
				
				if ((defineSprite.framecount == this.framecount) && equals(defineSprite.tagList, this.tagList) && equals(defineSprite.scalingGrid, this.scalingGrid) && equals(defineSprite.initAction, this.initAction))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			
			if (name != null)
			{
				hashCode += name.GetHashCode();
			}
			
			hashCode += DefineTag.PRIME * framecount;
			if (tagList.tags != null)
			{
				hashCode += DefineTag.PRIME * tagList.tags.Count;
			}
			if (initAction != null)
			{
				if (initAction.actionList != null)
				{
					hashCode += DefineTag.PRIME * initAction.actionList.size();
				}
			}
			return hashCode;
		}
		
		public override String ToString()
		{
			System.Text.StringBuilder stringBuffer = new System.Text.StringBuilder();
			
			stringBuffer.Append("DefineSprite: name = " + name + ", framecount = " + framecount + ", tagList = " + tagList + ", initAction = " + initAction);
			
			return stringBuffer.ToString();
		}
		
		/*
		private void fitRect(Rect inner, Rect outer)
		{
		if (outer.xMin == 0 && outer.xMax == 0 && outer.yMin==0 && outer.yMax==0)
		{
		outer.xMin = inner.xMin;
		outer.xMax = inner.xMax;
		outer.yMin = inner.yMin;
		outer.yMax = inner.yMax;
		}
		else
		{
		outer.xMin = inner.xMin < outer.xMin ? inner.xMin : outer.xMin;
		outer.yMin = inner.yMin < outer.yMin ? inner.yMin : outer.yMin;
		outer.xMax = inner.xMax > outer.xMax ? inner.xMax : outer.xMax;
		outer.yMax = inner.yMax > outer.yMax ? inner.yMax : outer.yMax;
		}
		}
		
		public Rect getBounds()
		{
		Iterator it = timeline.tags.iterator();
		Rect bounds = new Rect();
		loop: while (it.hasNext())
		{
		Tag t = (Tag) it.next();
		switch (t.code)
		{
		case stagShowFrame:
		// stop at end of first frame
		break loop;
		case stagPlaceObject:
		case stagPlaceObject2:
		PlaceObject po = (PlaceObject) t;
		switch (po.ref.code)
		{
		case stagDefineEditText:
		// how to calculate bounds?
		break;
		case stagDefineSprite:
		DefineSprite defineSprite = (DefineSprite) po.ref;
		Rect spriteBounds = defineSprite.getBounds();
		Rect newBounds = po.hasMatrix() ? po.matrix.xformRect(spriteBounds) : spriteBounds;
		fitRect(newBounds, bounds);
		break;
		case stagDefineShape:
		case stagDefineShape2:
		case stagDefineShape3:
		DefineShape defineShape = (DefineShape) po.ref;
		fitRect(defineShape.bounds, bounds);
		break;
		}
		break;
		}
		}
		return bounds;
		} */
	}
}
