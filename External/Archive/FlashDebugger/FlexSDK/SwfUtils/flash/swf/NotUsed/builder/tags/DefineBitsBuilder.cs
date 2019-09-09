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
using DefineBits = flash.swf.tags.DefineBits;
using DefineSprite = flash.swf.tags.DefineSprite;
using DefineShape = flash.swf.tags.DefineShape;
using PlaceObject = flash.swf.tags.PlaceObject;
using Tag = flash.swf.Tag;
using TagList = flash.swf.types.TagList;
using Matrix = flash.swf.types.Matrix;
using JPEGImage = flash.graphics.images.JPEGImage;
namespace flash.swf.builder.tags
{
	
	/// <author>  Paul Reilly
	/// </author>
	/// <author>  Peter Farland
	/// </author>
	public class DefineBitsBuilder
	{
		private DefineBitsBuilder()
		{
		}
		
		public static DefineBits build(JPEGImage image)
		{
			DefineBits defineBits = new DefineBits(flash.swf.TagValues_Fields.stagDefineBitsJPEG2);
			
			try
			{
				defineBits.data = image.Data;
			}
			finally
			{
				image.dispose();
			}
			
			return defineBits;
		}
		
		public static DefineBits build(System.String name, JPEGImage image)
		{
			DefineBits defineBits = null;
			try
			{
				defineBits = build(image);
				defineBits.name = name;
			}
			catch (System.IO.IOException ex)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
				throw new System.SystemException("Error reading JPEG image " + image.Location + ". " + ex.Message);
			}
			finally
			{
				image.dispose();
			}
			
			return defineBits;
		}
		
		public static DefineSprite buildSprite(System.String name, JPEGImage image)
		{
			TagList taglist = new TagList();
			
			try
			{
				DefineBits defineBits = build(image);
				taglist.defineBitsJPEG2(defineBits);
				
				DefineShape ds3 = ImageShapeBuilder.buildImage(defineBits, image.Width, image.Height);
				taglist.defineShape3(ds3);
				
				PlaceObject po2 = new PlaceObject(ds3, 1);
				po2.Matrix = new Matrix();
				// po2.setName(name);
				
				taglist.placeObject2(po2);
			}
			catch (System.IO.IOException ex)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
				throw new System.SystemException("Error reading JPEG image " + image.Location + ". " + ex.Message);
			}
			finally
			{
				image.dispose();
			}
			
			return defineSprite(name, taglist);
		}
		
		private static DefineSprite defineSprite(System.String name, TagList taglist)
		{
			DefineSprite defineSprite = new DefineSprite();
			defineSprite.framecount = 1;
			defineSprite.tagList = taglist;
			defineSprite.name = name;
			return defineSprite;
		}
	}
}