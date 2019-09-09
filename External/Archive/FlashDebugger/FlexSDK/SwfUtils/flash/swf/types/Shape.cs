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
using TagHandler = Flash.Swf.TagHandler;
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class Shape
	{
		/// <summary>list of ShapeRecords that define this Shape </summary>
		public System.Collections.IList shapeRecords;
		
		public virtual void  visitDependents(TagHandler h)
		{
            foreach (ShapeRecord rec in shapeRecords)
			{
				rec.visitDependents(h);
			}
		}
		
		public virtual void  getReferenceList(System.Collections.IList refs)
		{
			System.Collections.IEnumerator it = shapeRecords.GetEnumerator();

            foreach (ShapeRecord rec in shapeRecords)
			{
				rec.getReferenceList(refs);
			}
		}
		
		public  override bool Equals(System.Object obj)
		{
			bool isEqual = false;

            if (obj is Shape)
			{
                Shape shape = (Shape)obj;
				
				if (((shape.shapeRecords == null) && (this.shapeRecords == null)) || ((shape.shapeRecords != null) && (this.shapeRecords != null) && ArrayLists.equals(shape.shapeRecords, this.shapeRecords)))
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
