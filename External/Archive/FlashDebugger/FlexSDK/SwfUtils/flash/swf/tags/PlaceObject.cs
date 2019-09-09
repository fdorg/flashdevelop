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
using TagValues = Flash.Swf.TagValues;
using CXForm = Flash.Swf.Types.CXForm;
using ClipActions = Flash.Swf.Types.ClipActions;
using Matrix = Flash.Swf.Types.Matrix;
namespace Flash.Swf.Tags
{
	
	/// <summary> This is the place command.  The encoded form can be PlaceObject or PlaceObject2.</summary>
	/// <author>  Clement Wong
	/// </author>
	public class PlaceObject:Tag
	{
		virtual public DefineTag Ref
		{
			set
			{
				if (value == null)
					throw new System.NullReferenceException();
				this.ref_Renamed = value;
				flags = value != null?flags | HAS_CHARACTER:flags & ~ HAS_CHARACTER;
			}
			
		}
		virtual public Matrix Matrix
		{
			set
			{
				this.matrix = value;
				flags = value != null?flags | HAS_MATRIX:flags & ~ HAS_MATRIX;
			}
			
		}
		virtual public int ClipDepth
		{
			set
			{
				this.clipDepth = value;
				flags |= HAS_CLIP_DEPTH;
			}
			
		}
		virtual public int Ratio
		{
			set
			{
				this.ratio = value;
				flags |= HAS_RATIO;
			}
			
		}
		virtual public CXForm Cxform
		{
			set
			{
				this.colorTransform = value;
				flags = value != null?flags | HAS_CXFORM:flags & ~ HAS_CXFORM;
			}
			
		}
		virtual public String Name
		{
			set
			{
				this.name = value;
				flags = value != null?flags | HAS_NAME:flags & ~ HAS_NAME;
			}
			
		}
		virtual public ClipActions ClipActions
		{
			set
			{
				clipActions = value;
				flags = value != null?flags | HAS_CLIP_ACTION:flags & ~ HAS_CLIP_ACTION;
			}
			
		}
		virtual public String ClassName
		{
			set
			{
				this.className = value;
				flags2 = value != null?flags2 | HAS_CLASS_NAME:flags2 & ~ HAS_CLASS_NAME;
			}
			
		}
		virtual public bool HasImage
		{
			set
			{
				flags2 = value?flags2 | HAS_IMAGE:flags2 & ~ HAS_IMAGE;
			}
			
		}
		public int flags;
		private const int HAS_CLIP_ACTION = 1 << 7;
		private const int HAS_CLIP_DEPTH = 1 << 6;
		private const int HAS_NAME = 1 << 5;
		private const int HAS_RATIO = 1 << 4;
		private const int HAS_CXFORM = 1 << 3;
		private const int HAS_MATRIX = 1 << 2;
		private const int HAS_CHARACTER = 1 << 1;
		private const int HAS_MOVE = 1 << 0;
		
		public int flags2;
		private const int HAS_IMAGE = 1 << 4;
		private const int HAS_CLASS_NAME = 1 << 3;
		private const int HAS_CACHE_AS_BITMAP = 1 << 2;
		private const int HAS_BLEND_MODE = 1 << 1;
		private const int HAS_FILTER_LIST = 1 << 0;
		
		public int ratio;
		public String name;
		public int clipDepth;
		public ClipActions clipActions;
		public int depth;
		public Matrix matrix;
		public CXForm colorTransform;
		public DefineTag ref_Renamed;
		public System.Collections.IList filters;
		public int blendMode;
		public String className;
		
		public PlaceObject(int code):base(code)
		{
		}
		
		public PlaceObject(Matrix m, DefineTag ref_Renamed, int depth, String name):base(Flash.Swf.TagValues.stagPlaceObject2)
		{
			this.depth = depth;
			Matrix = m;
			Ref = ref_Renamed;
			Name = name;
		}
		
		public PlaceObject(DefineTag ref_Renamed, int depth):base(Flash.Swf.TagValues.stagPlaceObject2)
		{
			this.depth = depth;
			Ref = ref_Renamed;
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is PlaceObject))
			{
				PlaceObject placeObject = (PlaceObject) object_Renamed;
				
				// not comparing filters list
				if ((placeObject.flags == this.flags) && (placeObject.flags2 == this.flags2) && (placeObject.ratio == this.ratio) && equals(placeObject.name, this.name) && (placeObject.clipDepth == this.clipDepth) && equals(placeObject.clipActions, this.clipActions) && (placeObject.depth == this.depth) && equals(placeObject.matrix, this.matrix) && equals(placeObject.colorTransform, this.colorTransform) && equals(placeObject.ref_Renamed, this.ref_Renamed) && (placeObject.blendMode == this.blendMode) && equals(placeObject.className, this.className))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode += DefineTag.PRIME * flags;
			hashCode += DefineTag.PRIME * ratio;
			if (name != null)
			{
				hashCode += name.GetHashCode();
			}
			hashCode += DefineTag.PRIME * depth;
			return hashCode;
		}
		
		
		public override void  visit(TagHandler h)
		{
			if (code == Flash.Swf.TagValues.stagPlaceObject)
				h.placeObject(this);
			else if (code == Flash.Swf.TagValues.stagPlaceObject2)
				h.placeObject2(this);
			// if (code == stagPlaceObject3)
			else
				h.placeObject3(this);
		}
		
		public override Tag SimpleReference
		{
            get
            {
                return hasCharID() ? ref_Renamed : null;
            }
		}
		
		public virtual bool hasClipAction()
		{
			return (flags & HAS_CLIP_ACTION) != 0;
		}
		
		public virtual bool hasClipDepth()
		{
			return (flags & HAS_CLIP_DEPTH) != 0;
		}
		
		public virtual bool hasName()
		{
			return (flags & HAS_NAME) != 0;
		}
		
		public virtual bool hasRatio()
		{
			return (flags & HAS_RATIO) != 0;
		}
		
		public virtual bool hasCharID()
		{
			return (flags & HAS_CHARACTER) != 0;
		}
		
		public virtual bool hasMove()
		{
			return (flags & HAS_MOVE) != 0;
		}
		
		public virtual bool hasMatrix()
		{
			return (flags & HAS_MATRIX) != 0;
		}
		
		public virtual bool hasCxform()
		{
			return (flags & HAS_CXFORM) != 0;
		}
		
		public virtual bool hasFilterList()
		{
			return (flags2 & HAS_FILTER_LIST) != 0;
		}
		
		public virtual bool hasBlendMode()
		{
			return (flags2 & HAS_BLEND_MODE) != 0;
		}
		
		public virtual bool hasCacheAsBitmap()
		{
			return (flags2 & HAS_CACHE_AS_BITMAP) != 0;
		}
		
		public virtual bool hasClassName()
		{
			return (flags2 & HAS_CLASS_NAME) != 0;
		}
		
		public virtual bool hasImage()
		{
			return (flags2 & HAS_IMAGE) != 0;
		}
	}
}
