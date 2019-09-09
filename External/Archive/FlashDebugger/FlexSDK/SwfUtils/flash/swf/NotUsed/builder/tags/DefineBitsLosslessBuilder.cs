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
using DefineBitsLossless = flash.swf.tags.DefineBitsLossless;
using DefineSprite = flash.swf.tags.DefineSprite;
using DefineShape = flash.swf.tags.DefineShape;
using PlaceObject = flash.swf.tags.PlaceObject;
using DefineTag = flash.swf.tags.DefineTag;
using DefineBits = flash.swf.tags.DefineBits;
using TagValues = flash.swf.TagValues;
using TagList = flash.swf.types.TagList;
using Matrix = flash.swf.types.Matrix;
using LosslessImage = flash.graphics.images.LosslessImage;
namespace flash.swf.builder.tags
{
	
	/// <author>  Paul Reilly
	/// </author>
	/// <author>  Peter Farland
	/// </author>
	public class DefineBitsLosslessBuilder
	{
		private DefineBitsLosslessBuilder()
		{
		}
		
		public static DefineBitsLossless build(LosslessImage image)
		{
			DefineBitsLossless defineBitsLossless = new DefineBitsLossless(flash.swf.TagValues_Fields.stagDefineBitsLossless2);
			defineBitsLossless.format = 5;
			
			defineBitsLossless.width = image.Width;
			defineBitsLossless.height = image.Height;
			int[] pixels = (int[]) image.Pixels;
			
			defineBitsLossless.data = new sbyte[pixels.Length * 4];
			
			for (int i = 0; i < pixels.Length; i++)
			{
				int offset = i * 4;
				int alpha = (pixels[i] >> 24) & 0xFF;
				defineBitsLossless.data[offset] = (sbyte) alpha;
				
				// [preilly] Ignore the other components if alpha is transparent.  This seems
				// to be a bug in the player.  Additionally, premultiply the alpha and the
				// colors, because the player expects this.
				if (defineBitsLossless.data[offset] != 0)
				{
					int red = (pixels[i] >> 16) & 0xFF;
					defineBitsLossless.data[offset + 1] = (sbyte) ((red * alpha) / 255);
					int green = (pixels[i] >> 8) & 0xFF;
					defineBitsLossless.data[offset + 2] = (sbyte) ((green * alpha) / 255);
					int blue = pixels[i] & 0xFF;
					defineBitsLossless.data[offset + 3] = (sbyte) ((blue * alpha) / 255);
				}
			}
			
			return defineBitsLossless;
		}
		
		public static DefineBits build(System.String name, LosslessImage image)
		{
			DefineBitsLossless defineBits = build(image);
			defineBits.name = name;
			return defineBits;
		}
		
		public static DefineTag buildSprite(System.String name, LosslessImage image)
		{
			TagList taglist = new TagList();
			DefineBitsLossless defineBits = build(image);
			taglist.defineBitsLossless2(defineBits);
			
			DefineShape ds3 = ImageShapeBuilder.buildImage(defineBits, defineBits.width, defineBits.height);
			taglist.defineShape3(ds3);
			
			PlaceObject po2 = new PlaceObject(ds3, 1);
			po2.Matrix = new Matrix();
			//Ahipo2.setName(name);
			
			taglist.placeObject2(po2);
			
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