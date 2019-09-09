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
using flash.swf.tags;
using ButtonCondAction = flash.swf.types.ButtonCondAction;
using ButtonRecord = flash.swf.types.ButtonRecord;
using CXForm = flash.swf.types.CXForm;
using CXFormWithAlpha = flash.swf.types.CXFormWithAlpha;
using CurvedEdgeRecord = flash.swf.types.CurvedEdgeRecord;
using EdgeRecord = flash.swf.types.EdgeRecord;
using FillStyle = flash.swf.types.FillStyle;
using GlyphEntry = flash.swf.types.GlyphEntry;
using GradRecord = flash.swf.types.GradRecord;
using ImportRecord = flash.swf.types.ImportRecord;
using KerningRecord = flash.swf.types.KerningRecord;
using LineStyle = flash.swf.types.LineStyle;
using MD5 = flash.swf.types.MD5;
using Matrix = flash.swf.types.Matrix;
using MorphFillStyle = flash.swf.types.MorphFillStyle;
using MorphGradRecord = flash.swf.types.MorphGradRecord;
using MorphLineStyle = flash.swf.types.MorphLineStyle;
using Rect = flash.swf.types.Rect;
using Shape = flash.swf.types.Shape;
using ShapeRecord = flash.swf.types.ShapeRecord;
using ShapeWithStyle = flash.swf.types.ShapeWithStyle;
using SoundInfo = flash.swf.types.SoundInfo;
using StraightEdgeRecord = flash.swf.types.StraightEdgeRecord;
using StyleChangeRecord = flash.swf.types.StyleChangeRecord;
using TextRecord = flash.swf.types.TextRecord;
using Filter = flash.swf.types.Filter;
using DropShadowFilter = flash.swf.types.DropShadowFilter;
using BlurFilter = flash.swf.types.BlurFilter;
using ColorMatrixFilter = flash.swf.types.ColorMatrixFilter;
using GlowFilter = flash.swf.types.GlowFilter;
using ConvolutionFilter = flash.swf.types.ConvolutionFilter;
using BevelFilter = flash.swf.types.BevelFilter;
using GradientGlowFilter = flash.swf.types.GradientGlowFilter;
using GradientBevelFilter = flash.swf.types.GradientBevelFilter;
using Gradient = flash.swf.types.Gradient;
using FocalGradient = flash.swf.types.FocalGradient;
namespace flash.swf
{
	
	public class TagEncoder:TagHandler, TagValues
	{
		virtual public int Pos
		{
			get
			{
				return writer.Pos;
			}
			
		}
		virtual protected internal int SwfVersion
		{
			get
			{
				return header_Renamed_Field.version;
			}
			
		}
		virtual protected internal int FrameRate
		{
			get
			{
				return header_Renamed_Field.rate;
			}
			
		}
		virtual public Dictionary EncoderDictionary
		{
			set
			{
				assert((this.dict == null) || (this.dict.ids.Count == 0));
				this.dict = value;
			}
			
		}
		virtual public Dictionary Dictionary
		{
			get
			{
				return dict;
			}
			
		}
		virtual public bool Debug
		{
			get
			{
				return debug != null;
			}
			
		}
		virtual public int Width
		{
			get
			{
				return width / 20;
			}
			
		}
		virtual public int Height
		{
			get
			{
				return height / 20;
			}
			
		}
		virtual public System.String MainDebugScript
		{
			set
			{
				debug.MainDebugScript = value;
			}
			
		}
		override public SetBackgroundColor BackgroundColor
		{
			set
			{
				encodeTagHeader(value.code, 3, false);
				encodeRGB(value.color, writer);
			}
			
		}
		override public SetTabIndex TabIndex
		{
			set
			{
				tagw.writeUI16(value.depth);
				tagw.writeUI16(value.index);
				encodeTag(value);
			}
			
		}
		// changed from private to protected to support Flash Authoring - jkamerer 2007.07.30
		protected internal SwfEncoder writer;
		private SwfEncoder tagw;
		private int width;
		private int height;
		private int frames;
		private int framecountPos;
		private DebugEncoder debug;
		private Header header_Renamed_Field;
		
		protected internal Dictionary dict;
		private int uuidOffset;
		
		public TagEncoder()
		{
			dict = new Dictionary();
		}
		
		public TagEncoder(Dictionary dict)
		{
			this.dict = dict;
		}
		
		public override void  productInfo(ProductInfo tag)
		{
			tagw.write32(tag.Product);
			tagw.write32(tag.Edition);
			tagw.write(new sbyte[]{tag.MajorVersion, tag.MinorVersion});
			tagw.write64(tag.Build);
			tagw.write64(tag.CompileDate);
			encodeTag(tag);
		}
		
		public override void  fileAttributes(FileAttributes tag)
		{
			tagw.writeUBits(0, 3);
			tagw.writeBit(tag.hasMetadata);
			tagw.writeBit(tag.actionScript3);
			tagw.writeBit(tag.suppressCrossDomainCaching);
			tagw.writeBit(tag.swfRelativeUrls);
			tagw.writeBit(tag.useNetwork);
			tagw.writeUBits(0, 24);
			encodeTag(tag);
		}
		
		public override void  metadata(Metadata tag)
		{
			tagw.writeString(tag.xml);
			encodeTag(tag);
		}
		
		protected internal virtual SwfEncoder createEncoder(int swfVersion)
		{
			return new SwfEncoder(swfVersion);
		}
		
		public override void  header(Header header)
		{
			// get some header properties we need to know
			int swfVersion = header.version;
			this.header_Renamed_Field = header;
			this.writer = createEncoder(swfVersion);
			this.tagw = createEncoder(swfVersion);
			width = header.size.Width;
			height = header.size.Height;
			frames = 0;
			
			// write the header
			writer.writeUI8(header.compressed?'C':'F');
			writer.writeUI8('W');
			writer.writeUI8('S');
			writer.writeUI8(header.version);
			writer.write32((int) header.length);
			if (header.compressed)
			{
				writer.markComp();
			}
			encodeRect(header.size, writer);
			writer.writeUI8(header.rate >> 8);
			writer.writeUI8(header.rate & 255);
			framecountPos = writer.Pos;
			writer.writeUI16(header.framecount);
		}
		
		public override void  finish()
		{
			// write end marker
			writer.writeUI16(0);
			
			// update the length
			writer.write32at(4, writer.Pos);
			
			// update the frame count
			writer.writeUI16at(framecountPos, frames);
			
			if (debug != null)
			{
				// compute a crc and use it for the debug id.  that way it
				// is wholly dependent on the bytes in the SWF and not some
				// outside value.  If any of the bytes are different,
				// then the UUID will be different.
				sbyte[] md5 = MD5.getDigest(writer.ByteArray, writer.Length);
				writer.writeAt(uuidOffset, md5);
				debug.updateUUID(md5);
			}
		}
		
		public virtual void  writeTo(System.IO.Stream out_Renamed)
		{
			writer.WriteTo(out_Renamed);
		}
		
		
		public virtual void  writeDebugTo(System.IO.Stream out_Renamed)
		{
			debug.writeTo(out_Renamed);
		}
		
		public virtual void  encodeRect(Rect r, SwfEncoder w)
		{
			int nBits = r.nbits();
			w.writeUBits(nBits, 5);
			w.writeSBits(r.xMin, nBits);
			w.writeSBits(r.xMax, nBits);
			w.writeSBits(r.yMin, nBits);
			w.writeSBits(r.yMax, nBits);
			w.flushBits();
		}
		
		public override void  debugID(DebugID tag)
		{
			encodeTagHeader(tag.code, tag.uuid.bytes.Length, false);
			uuidOffset = writer.Pos;
			writer.write(tag.uuid.bytes);
			
			debug = new DebugEncoder();
			debug.header(SwfVersion);
			debug.uuid(tag.uuid);
		}
		
		private void  encodeTag(Tag tag)
		{
			try
			{
				tagw.compress();
				encodeTagHeader(tag.code, tagw.Pos, isLongHeader(tag));
				tagw.WriteTo(writer);
				tagw.reset();
			}
			catch (System.IO.IOException e)
			{
				assert(false);
			}
		}
		
		private bool isLongHeader(Tag t)
		{
			switch (t.code)
			{
				
				// [preilly] In the player code, ScriptThread::DefineBits() assumes all DefineBits
				// tags use a long header.  See "ch->data = AttachData(pos-8);".  If the player
				// also supported a short header, it would use "pos-4".
				case flash.swf.TagValues_Fields.stagDefineBits: 
				case flash.swf.TagValues_Fields.stagDefineBitsJPEG2: 
				case flash.swf.TagValues_Fields.stagDefineBitsJPEG3: 
				case flash.swf.TagValues_Fields.stagDefineBitsLossless: 
				case flash.swf.TagValues_Fields.stagDefineBitsLossless2: 
					return true;
					
					// [ed] the FlashPaper codebase also indicates that stagSoundStreamBlock must use
					// a long format header.  todo - verify by looking at the player code.
				
				case flash.swf.TagValues_Fields.stagSoundStreamBlock: 
					return true;
					
					// [edsmith] these tags have code in them.  When we're writing a SWD, we use long headers
					// so we can predict SWF offsets correctly when writing SWD line/offset records.
				
				case flash.swf.TagValues_Fields.stagDefineButton: 
				case flash.swf.TagValues_Fields.stagDefineButton2: 
				case flash.swf.TagValues_Fields.stagDefineSprite: 
				case flash.swf.TagValues_Fields.stagDoInitAction: 
				case flash.swf.TagValues_Fields.stagDoAction: 
					return Debug;
				
				
				case flash.swf.TagValues_Fields.stagPlaceObject2: 
					return Debug && ((PlaceObject) t).hasClipAction();
					
					// all other tags will use short/long headers depending on their length
				
				default: 
					return false;
				
			}
		}
		
		private void  encodeTagHeader(int code, int length, bool longHeader)
		{
			if (longHeader || length >= 63)
			{
				writer.writeUI16((code << 6) | 63);
				writer.write32(length);
			}
			else
			{
				writer.writeUI16((code << 6) | length);
			}
		}
		
		public override void  defineScalingGrid(DefineScalingGrid tag)
		{
			int idref = dict.getId(tag.scalingTarget);
			tagw.writeUI16(idref);
			encodeRect(tag.rect, tagw);
			encodeTag(tag);
		}
		
		public override void  defineBinaryData(DefineBinaryData tag)
		{
			encodeTagHeader(tag.code, 6 + tag.data.Length, false);
			int id = dict.add(tag);
			writer.writeUI16(id);
			writer.write32(tag.reserved);
			writer.write(tag.data);
		}
		
		public override void  defineBits(DefineBits tag)
		{
			encodeTagHeader(tag.code, 2 + tag.data.Length, true);
			int id = dict.add(tag);
			writer.writeUI16(id);
			writer.write(tag.data);
		}
		
		public override void  defineBitsJPEG2(DefineBits tag)
		{
			defineBits(tag);
		}
		
		public override void  defineBitsJPEG3(DefineBitsJPEG3 tag)
		{
			int id = dict.add(tag);
			tagw.writeUI16(id);
			tagw.write32(tag.data.Length);
			tagw.write(tag.data);
			tagw.markComp();
			tagw.write(tag.alphaData);
			encodeTag(tag);
		}
		
		public override void  defineBitsLossless(DefineBitsLossless tag)
		{
			int id = dict.add(tag);
			tagw.writeUI16(id);
			tagw.writeUI8(tag.format);
			tagw.writeUI16(tag.width);
			tagw.writeUI16(tag.height);
			switch (tag.format)
			{
				
				case 3: 
					tagw.writeUI8(tag.colorData.Length - 1);
					tagw.markComp();
					encodeColorMapData(tag.colorData, tag.data, tagw);
					break;
				
				case 4: 
				case 5: 
					tagw.markComp();
					encodeBitmapData(tag.data, tagw);
					break;
				}
			encodeTag(tag);
		}
		
		private void  encodeBitmapData(sbyte[] data, SwfEncoder w)
		{
			w.write(data);
		}
		
		private void  encodeColorMapData(int[] colorData, sbyte[] pixelData, SwfEncoder w)
		{
			for (int i = 0; i < colorData.Length; i++)
			{
				encodeRGB(colorData[i], w);
			}
			w.write(pixelData);
		}
		
		/// <param name="rgb">as 0x00RRGGBB
		/// </param>
		/// <param name="w">
		/// </param>
		private void  encodeRGB(int rgb, SwfEncoder w)
		{
			w.writeUI8(SupportClass.URShift(rgb, 16)); // red. we don't mask this because if rgb has an Alpha value, something's wrong
			w.writeUI8((SupportClass.URShift(rgb, 8)) & 255);
			w.writeUI8(rgb & 255); // blue
		}
		
		public override void  defineBitsLossless2(DefineBitsLossless tag)
		{
			int id = dict.add(tag);
			tagw.writeUI16(id);
			tagw.writeUI8(tag.format);
			tagw.writeUI16(tag.width);
			tagw.writeUI16(tag.height);
			switch (tag.format)
			{
				
				case 3: 
					tagw.writeUI8(tag.colorData.Length - 1);
					tagw.markComp();
					encodeAlphaColorMapData(tag.colorData, tag.data, tagw);
					break;
				
				case 4: 
				case 5: 
					tagw.markComp();
					encodeBitmapData(tag.data, tagw);
					break;
				}
			encodeTag(tag);
		}
		
		private void  encodeAlphaColorMapData(int[] colorData, sbyte[] pixelData, SwfEncoder w)
		{
			for (int i = 0; i < colorData.Length; i++)
			{
				encodeRGBA(colorData[i], w);
			}
			w.write(pixelData);
		}
		
		/// <param name="rgba">as 0xAARRGGBB
		/// </param>
		/// <param name="w">
		/// </param>
		private void  encodeRGBA(int rgba, SwfEncoder w)
		{
			w.writeUI8((SupportClass.URShift(rgba, 16)) & 255); // red
			w.writeUI8((SupportClass.URShift(rgba, 8)) & 255); // green
			w.writeUI8(rgba & 255); // blue
			w.writeUI8(SupportClass.URShift(rgba, 24)); // alpha
		}
		
		public override void  defineButton(DefineButton tag)
		{
			int id = dict.add(tag);
			tagw.writeUI16(id);
			
			if (Debug)
			{
				debug.adjust = writer.Pos + 6;
			}
			
			for (int i = 0; i < tag.buttonRecords.Length; i++)
			{
				encodeButtonRecord(tag.buttonRecords[i], tagw, tag.code);
			}
			tagw.writeUI8(0); // no more button records
			
			// assume there is only one condition we will handle
			new ActionEncoder(tagw, debug).encode(tag.condActions[0].actionList);
			tagw.writeUI8(0); // write action end flag, must be zero
			encodeTag(tag);
			
			if (Debug)
			{
				debug.adjust = 0;
			}
		}
		
		private void  encodeButtonRecord(ButtonRecord record, SwfEncoder w, int defineButton)
		{
			if (defineButton == flash.swf.TagValues_Fields.stagDefineButton2)
			{
				w.writeUBits(0, 2);
				w.writeBit(record.blendMode != - 1);
				w.writeBit(record.filters != null);
			}
			else
			{
				w.writeUBits(0, 4);
			}
			w.writeBit(record.hitTest);
			w.writeBit(record.down);
			w.writeBit(record.over);
			w.writeBit(record.up);
			
			w.writeUI16(dict.getId(record.characterRef));
			w.writeUI16(record.placeDepth);
			encodeMatrix(record.placeMatrix, w);
			
			if (defineButton == flash.swf.TagValues_Fields.stagDefineButton2)
			{
				encodeCxforma(record.colorTransform, w);
				if (record.filters != null)
				{
					this.encodeFilterList(record.filters, w);
				}
				if (record.blendMode != - 1)
				{
					w.writeUI8(record.blendMode);
				}
			}
		}
		
		private void  encodeCxforma(CXFormWithAlpha cxforma, SwfEncoder w)
		{
			w.writeBit(cxforma.hasAdd);
			w.writeBit(cxforma.hasMult);
			
			int nbits = cxforma.nbits();
			w.writeUBits(nbits, 4);
			
			if (cxforma.hasMult)
			{
				w.writeSBits(cxforma.redMultTerm, nbits);
				w.writeSBits(cxforma.greenMultTerm, nbits);
				w.writeSBits(cxforma.blueMultTerm, nbits);
				w.writeSBits(cxforma.alphaMultTerm, nbits);
			}
			
			if (cxforma.hasAdd)
			{
				w.writeSBits(cxforma.redAddTerm, nbits);
				w.writeSBits(cxforma.greenAddTerm, nbits);
				w.writeSBits(cxforma.blueAddTerm, nbits);
				w.writeSBits(cxforma.alphaAddTerm, nbits);
			}
			
			w.flushBits();
		}
		
		private void  encodeMatrix(Matrix matrix, SwfEncoder w)
		{
			w.writeBit(matrix.hasScale);
			if (matrix.hasScale)
			{
				int nScaleBits = matrix.nScaleBits();
				w.writeUBits(nScaleBits, 5);
				w.writeSBits(matrix.scaleX, nScaleBits);
				w.writeSBits(matrix.scaleY, nScaleBits);
			}
			
			w.writeBit(matrix.hasRotate);
			if (matrix.hasRotate)
			{
				int nRotateBits = matrix.nRotateBits();
				w.writeUBits(nRotateBits, 5);
				w.writeSBits(matrix.rotateSkew0, nRotateBits);
				w.writeSBits(matrix.rotateSkew1, nRotateBits);
			}
			
			int nTranslateBits = matrix.nTranslateBits();
			w.writeUBits(nTranslateBits, 5);
			w.writeSBits(matrix.translateX, nTranslateBits);
			w.writeSBits(matrix.translateY, nTranslateBits);
			
			w.flushBits();
		}
		
		public override void  defineButton2(DefineButton tag)
		{
			if (Debug)
			{
				debug.adjust = writer.Pos + 6;
			}
			
			int id = dict.add(tag);
			tagw.writeUI16(id);
			tagw.writeUBits(0, 7); // reserved
			tagw.writeBit(tag.trackAsMenu);
			int offsetPos = tagw.Pos;
			tagw.writeUI16(0); // actionOffset
			
			for (int i = 0; i < tag.buttonRecords.Length; i++)
			{
				encodeButtonRecord(tag.buttonRecords[i], tagw, tag.code);
			}
			
			tagw.writeUI8(0); // charEndFlag
			
			if (tag.condActions.Length > 0)
			{
				tagw.writeUI16at(offsetPos, tagw.Pos - offsetPos);
				
				for (int i = 0; i < tag.condActions.Length; i++)
				{
					bool isLast = i + 1 == tag.condActions.Length;
					encodeButtonCondAction(tag.condActions[i], tagw, isLast);
				}
			}
			encodeTag(tag);
			
			if (Debug)
			{
				debug.adjust = 0;
			}
		}
		
		private void  encodeButtonCondAction(ButtonCondAction condAction, SwfEncoder w, bool last)
		{
			int pos = w.Pos;
			w.writeUI16(0);
			
			w.writeUBits(condAction.keyPress, 7);
			w.writeBit(condAction.overDownToIdle);
			
			w.writeBit(condAction.idleToOverDown);
			w.writeBit(condAction.outDownToIdle);
			w.writeBit(condAction.outDownToOverDown);
			w.writeBit(condAction.overDownToOutDown);
			w.writeBit(condAction.overDownToOverUp);
			w.writeBit(condAction.overUpToOverDown);
			w.writeBit(condAction.overUpToIdle);
			w.writeBit(condAction.idleToOverUp);
			
			new ActionEncoder(w, debug).encode(condAction.actionList);
			w.writeUI8(0); // end action byte
			
			if (!last)
			{
				w.writeUI16at(pos, w.Pos - pos);
			}
		}
		
		public override void  defineButtonCxform(DefineButtonCxform tag)
		{
			int idref = dict.getId(tag.button);
			tagw.writeUI16(idref);
			encodeCxform(tag.colorTransform, tagw);
			encodeTag(tag);
		}
		
		private void  encodeCxform(CXForm cxform, SwfEncoder w)
		{
			
			w.writeBit(cxform.hasAdd);
			w.writeBit(cxform.hasMult);
			
			int nbits = cxform.nbits();
			w.writeUBits(nbits, 4);
			
			if (cxform.hasMult)
			{
				w.writeSBits(cxform.redMultTerm, nbits);
				w.writeSBits(cxform.greenMultTerm, nbits);
				w.writeSBits(cxform.blueMultTerm, nbits);
			}
			
			if (cxform.hasAdd)
			{
				w.writeSBits(cxform.redAddTerm, nbits);
				w.writeSBits(cxform.greenAddTerm, nbits);
				w.writeSBits(cxform.blueAddTerm, nbits);
			}
			
			w.flushBits();
		}
		
		public override void  defineButtonSound(DefineButtonSound tag)
		{
			int idref = dict.getId(tag.button);
			tagw.writeUI16(idref);
			if (tag.sound0 != null)
			{
				tagw.writeUI16(dict.getId(tag.sound0));
				encodeSoundInfo(tag.info0, tagw);
			}
			else
			{
				tagw.writeUI16(0);
			}
			if (tag.sound1 != null)
			{
				tagw.writeUI16(dict.getId(tag.sound1));
				encodeSoundInfo(tag.info1, tagw);
			}
			else
			{
				tagw.writeUI16(0);
			}
			if (tag.sound2 != null)
			{
				tagw.writeUI16(dict.getId(tag.sound2));
				encodeSoundInfo(tag.info2, tagw);
			}
			else
			{
				tagw.writeUI16(0);
			}
			if (tag.sound3 != null)
			{
				tagw.writeUI16(dict.getId(tag.sound3));
				encodeSoundInfo(tag.info3, tagw);
			}
			else
			{
				tagw.writeUI16(0);
			}
			encodeTag(tag);
		}
		
		private void  encodeSoundInfo(SoundInfo info, SwfEncoder w)
		{
			w.writeUBits(0, 2); // reserved
			w.writeBit(info.syncStop);
			w.writeBit(info.syncNoMultiple);
			w.writeBit(info.records != null);
			w.writeBit(info.loopCount != SoundInfo.UNINITIALIZED);
			w.writeBit(info.outPoint != SoundInfo.UNINITIALIZED);
			w.writeBit(info.inPoint != SoundInfo.UNINITIALIZED);
			
			if (info.inPoint != SoundInfo.UNINITIALIZED)
			{
				w.write32((int) info.inPoint);
			}
			if (info.outPoint != SoundInfo.UNINITIALIZED)
			{
				w.write32((int) info.outPoint);
			}
			if (info.loopCount != SoundInfo.UNINITIALIZED)
			{
				w.writeUI16(info.loopCount);
			}
			if (info.records != null)
			{
				w.writeUI8(info.records.Length);
				for (int k = 0; k < info.records.Length; k++)
				{
					w.write64(info.records[k]);
				}
			}
		}
		
		public override void  defineEditText(DefineEditText tag)
		{
			int id = dict.add(tag);
			tagw.writeUI16(id);
			encodeRect(tag.bounds, tagw);
			
			tagw.writeBit(tag.hasText);
			tagw.writeBit(tag.wordWrap);
			tagw.writeBit(tag.multiline);
			tagw.writeBit(tag.password);
			tagw.writeBit(tag.readOnly);
			tagw.writeBit(tag.hasTextColor);
			tagw.writeBit(tag.hasMaxLength);
			tagw.writeBit(tag.hasFont);
			
			tagw.writeBit(false); // reserved
			tagw.writeBit(tag.autoSize);
			tagw.writeBit(tag.hasLayout);
			tagw.writeBit(tag.noSelect);
			tagw.writeBit(tag.border);
			tagw.writeBit(tag.wasStatic);
			tagw.writeBit(tag.html);
			tagw.writeBit(tag.useOutlines);
			
			tagw.flushBits();
			
			if (tag.hasFont)
			{
				int idref = dict.getId(tag.font);
				tagw.writeUI16(idref);
				tagw.writeUI16(tag.height);
			}
			
			if (tag.hasTextColor)
			{
				encodeRGBA(tag.color, tagw);
			}
			
			if (tag.hasMaxLength)
			{
				tagw.writeUI16(tag.maxLength);
			}
			
			if (tag.hasLayout)
			{
				tagw.writeUI8(tag.align);
				tagw.writeUI16(tag.leftMargin);
				tagw.writeUI16(tag.rightMargin);
				tagw.writeUI16(tag.ident);
				tagw.writeSI16(tag.leading); // see errata, leading is signed
			}
			
			tagw.writeString(tag.varName);
			if (tag.hasText)
			{
				tagw.writeString(tag.initialText);
			}
			encodeTag(tag);
		}
		
		public override void  defineFont(DefineFont1 tag)
		{
			int id = dict.add(tag);
			tagw.writeUI16(id);
			
			int count = tag.glyphShapeTable.Length;
			
			int offsetPos = tagw.Pos;
			
			// write offset placeholders
			for (int i = 0; i < count; i++)
			{
				tagw.writeUI16(0);
			}
			
			// now write glyphs and update the encoded offset table
			for (int i = 0; i < count; i++)
			{
				tagw.writeUI16at(offsetPos + 2 * i, tagw.Pos - offsetPos);
				encodeShape(tag.glyphShapeTable[i], tagw, flash.swf.TagValues_Fields.stagDefineShape3, 1, 0);
			}
			
			encodeTag(tag);
		}
		
		public virtual void  encodeShape(Shape s, SwfEncoder w, int shape, int nFillStyles, int nLineStyles)
		{
			int[] numFillBits = new int[]{SwfEncoder.minBits(nFillStyles, 0)};
			int[] numLineBits = new int[]{SwfEncoder.minBits(nLineStyles, 0)};
			
			w.writeUBits(numFillBits[0], 4);
			w.writeUBits(numLineBits[0], 4);
			
			System.Collections.IEnumerator it = s.shapeRecords.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				ShapeRecord record = (ShapeRecord) it.Current;
				if (record is StyleChangeRecord)
				{
					// style change
					w.writeBit(false);
					StyleChangeRecord change = (StyleChangeRecord) record;
					encodeStyleChangeRecord(w, change, numFillBits, numLineBits, shape);
				}
				else
				{
					// edge
					w.writeBit(true);
					EdgeRecord e = (EdgeRecord) record;
					bool straight = e is StraightEdgeRecord;
					w.writeBit(straight);
					int nbits = straight?calcBits((StraightEdgeRecord) e):calcBits((CurvedEdgeRecord) e);
					if (nbits < 2)
						nbits = 2;
					w.writeUBits(nbits - 2, 4);
					if (straight)
					{
						// line
						StraightEdgeRecord line = (StraightEdgeRecord) e;
						encodeStraightEdgeRecord(line, w, nbits);
					}
					else
					{
						// curve
						CurvedEdgeRecord curve = (CurvedEdgeRecord) e;
						w.writeSBits(curve.controlDeltaX, nbits);
						w.writeSBits(curve.controlDeltaY, nbits);
						w.writeSBits(curve.anchorDeltaX, nbits);
						w.writeSBits(curve.anchorDeltaY, nbits);
					}
				}
			}
			
			// endshaperecord
			w.writeUBits(0, 6);
			
			w.flushBits();
		}
		
		private int calcBits(StraightEdgeRecord edge)
		{
			return SwfEncoder.minBits(SwfEncoder.maxNum(edge.deltaX, edge.deltaY, 0, 0), 1);
		}
		
		private int calcBits(CurvedEdgeRecord edge)
		{
			return SwfEncoder.minBits(SwfEncoder.maxNum(edge.controlDeltaX, edge.controlDeltaY, edge.anchorDeltaX, edge.anchorDeltaY), 1);
		}
		
		private void  encodeStraightEdgeRecord(StraightEdgeRecord line, SwfEncoder w, int nbits)
		{
			if (line.deltaX == 0)
			{
				w.writeUBits(1, 2); // vertical line
				w.writeSBits(line.deltaY, nbits);
			}
			else if (line.deltaY == 0)
			{
				w.writeUBits(0, 2); // horizontal line
				w.writeSBits(line.deltaX, nbits);
			}
			else
			{
				w.writeBit(true); // general line
				w.writeSBits(line.deltaX, nbits);
				w.writeSBits(line.deltaY, nbits);
			}
		}
		
		private void  encodeStyleChangeRecord(SwfEncoder w, StyleChangeRecord s, int[] numFillBits, int[] numLineBits, int shape)
		{
			w.writeBit(s.stateNewStyles);
			w.writeBit(s.stateLineStyle);
			w.writeBit(s.stateFillStyle1);
			w.writeBit(s.stateFillStyle0);
			w.writeBit(s.stateMoveTo);
			
			if (s.stateMoveTo)
			{
				int moveBits = s.nMoveBits();
				w.writeUBits(moveBits, 5);
				w.writeSBits(s.moveDeltaX, moveBits);
				w.writeSBits(s.moveDeltaY, moveBits);
			}
			
			if (s.stateFillStyle0)
			{
				w.writeUBits(s.fillstyle0, numFillBits[0]);
			}
			
			if (s.stateFillStyle1)
			{
				w.writeUBits(s.fillstyle1, numFillBits[0]);
			}
			
			if (s.stateLineStyle)
			{
				w.writeUBits(s.linestyle, numLineBits[0]);
			}
			
			if (s.stateNewStyles)
			{
				w.flushBits();
				
				encodeFillstyles(s.fillstyles, w, shape);
				encodeLinestyles(s.linestyles, w, shape);
				
				numFillBits[0] = SwfEncoder.minBits(s.fillstyles.Count, 0);
				numLineBits[0] = SwfEncoder.minBits(s.linestyles.Count, 0);
				w.writeUBits(numFillBits[0], 4);
				w.writeUBits(numLineBits[0], 4);
			}
		}
		
		private void  encodeLinestyles(System.Collections.ArrayList linestyles, SwfEncoder w, int shape)
		{
			int count = linestyles.Count;
			if (count > 0xFF)
			{
				w.writeUI8(0xFF);
				w.writeUI16(count);
			}
			else
			{
				w.writeUI8(count);
			}
			
			for (int i = 0; i < count; i++)
			{
				encodeLineStyle((LineStyle) linestyles[i], w, shape);
			}
		}
		
		private void  encodeLineStyle(LineStyle lineStyle, SwfEncoder w, int shape)
		{
			w.writeUI16(lineStyle.width);
			
			if (shape == flash.swf.TagValues_Fields.stagDefineShape6)
			{
				w.writeUI16(lineStyle.flags);
				if (lineStyle.hasMiterJoint())
					w.writeUI16(lineStyle.miterLimit);
			}
			
			if (shape == flash.swf.TagValues_Fields.stagDefineShape6 && lineStyle.hasFillStyle())
			{
				encodeFillStyle(lineStyle.fillStyle, w, shape);
			}
			else if ((shape == flash.swf.TagValues_Fields.stagDefineShape3) || (shape == flash.swf.TagValues_Fields.stagDefineShape6))
			{
				encodeRGBA(lineStyle.color, w);
			}
			else
			{
				encodeRGB(lineStyle.color, w);
			}
		}
		
		private void  encodeFillstyles(System.Collections.ArrayList fillstyles, SwfEncoder w, int shape)
		{
			int count = fillstyles.Count;
			if (count >= 0xFF)
			{
				w.writeUI8(0xFF);
				w.writeUI16(count);
			}
			else
			{
				w.writeUI8(count);
			}
			
			System.Collections.IEnumerator it = fillstyles.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				FillStyle style = (FillStyle) it.Current;
				encodeFillStyle(style, w, shape);
			}
		}
		
		private void  encodeFillStyle(FillStyle style, SwfEncoder w, int shape)
		{
			w.writeUI8(style.type);
			switch (style.type)
			{
				
				case FillStyle.FILL_SOLID:  // 0x00
					if ((shape == flash.swf.TagValues_Fields.stagDefineShape3) || (shape == flash.swf.TagValues_Fields.stagDefineShape6))
						encodeRGBA(style.color, w);
					else
						encodeRGB(style.color, w);
					break;
				
				case FillStyle.FILL_GRADIENT: 
				// 0x10 linear gradient fill
				case FillStyle.FILL_RADIAL_GRADIENT: 
				// 0x12 radial gradient fill
				case FillStyle.FILL_FOCAL_RADIAL_GRADIENT:  // 0x13 focal radial gradient fill
					encodeMatrix(style.matrix, w);
					encodeGradient(style.gradient, w, shape);
					break;
				
				case FillStyle.FILL_BITS: 
				// 0x40 tiled bitmap fill
				case (FillStyle.FILL_BITS | FillStyle.FILL_BITS_CLIP): 
				// 0x41 clipped bitmap fill
				case (FillStyle.FILL_BITS | FillStyle.FILL_BITS_NOSMOOTH): 
				// 0x42 tiled non-smoothed fill
				case (FillStyle.FILL_BITS | FillStyle.FILL_BITS_CLIP | FillStyle.FILL_BITS_NOSMOOTH):  // 0x43 clipped non-smoothed fill
					w.writeUI16(dict.getId(style.bitmap));
					encodeMatrix(style.matrix, w);
					break;
				}
		}
		
		private void  encodeGradient(Gradient gradient, SwfEncoder w, int shape)
		{
			w.writeUBits(gradient.spreadMode, 2);
			w.writeUBits(gradient.interpolationMode, 2);
			w.writeUBits(gradient.records.Length, 4);
			for (int i = 0; i < gradient.records.Length; i++)
			{
				encodeGradRecord(gradient.records[i], w, shape);
			}
			if (gradient is FocalGradient)
			{
				w.writeFixed8(((FocalGradient) gradient).focalPoint);
			}
		}
		
		private void  encodeGradRecord(GradRecord record, SwfEncoder w, int shape)
		{
			w.writeUI8(record.ratio);
			if ((shape == flash.swf.TagValues_Fields.stagDefineShape3) || (shape == flash.swf.TagValues_Fields.stagDefineShape6))
				encodeRGBA(record.color, w);
			else
				encodeRGB(record.color, w);
		}
		
		public override void  defineFont2(DefineFont2 tag)
		{
			int id = dict.add(tag);
			tagw.writeUI16(id);
			int startPos = tagw.Pos;
			bool again;
			
			if (tag.code == flash.swf.TagValues_Fields.stagDefineFont3)
			{
				tag.wideCodes = true;
			}
			
			if (!tag.wideCodes)
			{
				for (int i = 0; i < tag.codeTable.Length; i++)
				{
					if (tag.codeTable[i] > 255)
					{
						tag.wideCodes = true;
						break;
					}
				}
			}
			
			//UPGRADE_NOTE: Label 'loop' was moved. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1014'"
			do 
			{
				again = false;
				tagw.writeBit(tag.hasLayout);
				tagw.writeBit(tag.shiftJIS);
				tagw.writeBit(tag.smallText);
				tagw.writeBit(tag.ansi);
				tagw.writeBit(tag.wideOffsets);
				tagw.writeBit(tag.wideCodes);
				tagw.writeBit(tag.italic);
				tagw.writeBit(tag.bold);
				tagw.flushBits();
				
				tagw.writeUI8(tag.langCode);
				
				tagw.writeLengthString(tag.fontName);
				int count = tag.glyphShapeTable.Length;
				
				tagw.writeUI16(count);
				int offsetPos = tagw.Pos;
				
				// save space for the offset table
				if (tag.wideOffsets)
				{
					for (int i = 0; i < count; i++)
					{
						tagw.write32(0);
					}
				}
				else
				{
					for (int i = 0; i < count; i++)
					{
						tagw.writeUI16(0);
					}
				}
				
				//PJF: write placeholder for codeTableOffset, this will be changed after shapes encoded
				if (count > 0)
				{
					if (tag.wideOffsets)
					{
						tagw.write32(0);
					}
					else
					{
						tagw.writeUI16(0);
					}
				}
				
				for (int i = 0; i < count; i++)
				{
					// save offset to this glyph
					int offset = tagw.Pos - offsetPos;
					if (!tag.wideOffsets && offset > 65535)
					{
						again = true;
						tag.wideOffsets = true;
						tagw.Pos = startPos;
						//UPGRADE_NOTE: Labeled continue statement was changed to a goto statement. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1015'"
						goto loop;
					}
					if (tag.wideOffsets)
						tagw.write32at(offsetPos + 4 * i, offset);
					else
						tagw.writeUI16at(offsetPos + 2 * i, offset);
					
					encodeShape(tag.glyphShapeTable[i], tagw, flash.swf.TagValues_Fields.stagDefineShape3, 1, 0);
				}
				
				// update codeTableOffset
				int offset2 = tagw.Pos - offsetPos;
				if (!tag.wideOffsets && offset2 > 65535)
				{
					again = true;
					tag.wideOffsets = true;
					tagw.Pos = startPos;
					//UPGRADE_NOTE: Labeled continue statement was changed to a goto statement. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1015'"
					goto loop;
				}
				if (tag.wideOffsets)
				{
					tagw.write32at(offsetPos + 4 * count, offset2);
				}
				else
				{
					tagw.writeUI16at(offsetPos + 2 * count, offset2);
				}
				
				// now write the codetable
				
				if (tag.wideCodes)
				{
					for (int i = 0; i < tag.codeTable.Length; i++)
					{
						tagw.writeUI16(tag.codeTable[i]);
					}
				}
				else
				{
					for (int i = 0; i < tag.codeTable.Length; i++)
					{
						tagw.writeUI8(tag.codeTable[i]);
					}
				}
				
				if (tag.hasLayout)
				{
					tagw.writeSI16(tag.ascent);
					tagw.writeSI16(tag.descent);
					tagw.writeSI16(tag.leading);
					
					for (int i = 0; i < tag.advanceTable.Length; i++)
					{
						tagw.writeSI16(tag.advanceTable[i]);
					}
					
					for (int i = 0; i < tag.boundsTable.Length; i++)
					{
						encodeRect(tag.boundsTable[i], tagw);
					}
					
					tagw.writeUI16(tag.kerningTable.Length);
					
					for (int i = 0; i < tag.kerningTable.Length; i++)
					{
						if (!tag.wideCodes && ((tag.kerningTable[i].code1 > 255) || (tag.kerningTable[i].code2 > 255)))
						{
							again = true;
							tag.wideCodes = true;
							tagw.Pos = startPos;
							//UPGRADE_NOTE: Labeled continue statement was changed to a goto statement. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1015'"
							goto loop;
						}
						
						encodeKerningRecord(tag.kerningTable[i], tagw, tag.wideCodes);
					}
				}
				//UPGRADE_NOTE: Label 'loop' was moved. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1014'"
loop: ;
			}
			while (again);
			
			encodeTag(tag);
		}
		
		public override void  defineFont3(DefineFont3 tag)
		{
			defineFont2(tag);
		}
		
		public override void  defineFont4(DefineFont4 tag)
		{
			int id = dict.add(tag);
			tagw.writeUI16(id);
			
			tagw.writeUBits(0, 5); // reserved
			tagw.writeBit(tag.hasFontData);
			//tagw.writeBit(tag.smallText);
			tagw.writeBit(tag.italic);
			tagw.writeBit(tag.bold);
			tagw.flushBits();
			
			//tagw.writeUI8(tag.langCode);
			tagw.writeString(tag.fontName);
			if (tag.hasFontData)
			{
				tagw.write(tag.data);
			}
			
			encodeTag(tag);
		}
		
		public override void  defineFontAlignZones(DefineFontAlignZones tag)
		{
			int fontID = dict.getId(tag.font);
			tagw.writeUI16(fontID);
			tagw.writeUBits(tag.csmTableHint, 2);
			tagw.writeUBits(0, 6); // reserved
			for (int i = 0; i < tag.zoneTable.Length; i++)
			{
				ZoneRecord record = tag.zoneTable[i];
				tagw.writeUI8(record.numZoneData);
				for (int j = 0; j < record.numZoneData; j++)
				{
					tagw.write32((int) record.zoneData[j]);
				}
				tagw.writeUI8(record.zoneMask);
			}
			encodeTag(tag);
		}
		
		public override void  csmTextSettings(CSMTextSettings tag)
		{
			int textID = 0;
			if (tag.textReference != null)
			{
				textID = dict.getId(tag.textReference);
			}
			tagw.writeUI16(textID);
			tagw.writeUBits(tag.styleFlagsUseSaffron, 2);
			tagw.writeUBits(tag.gridFitType, 3);
			tagw.writeUBits(0, 3); // reserved
			// FIXME: thickness/sharpness should be written out as 32 bit IEEE Single Precision format in little Endian
			tagw.writeUBits((int) tag.thickness, 32);
			tagw.writeUBits((int) tag.sharpness, 32);
			tagw.writeUBits(0, 8); //reserved
			
			encodeTag(tag);
		}
		
		public override void  defineFontName(DefineFontName tag)
		{
			int fontID = dict.getId(tag.font);
			tagw.writeUI16(fontID);
			if (tag.fontName != null)
			{
				tagw.writeString(tag.fontName);
			}
			else
			{
				tagw.writeString("");
			}
			if (tag.copyright != null)
			{
				tagw.writeString(tag.copyright);
			}
			else
			{
				tagw.writeString("");
			}
			
			encodeTag(tag);
		}
		
		private void  encodeKerningRecord(KerningRecord kerningRecord, SwfEncoder w, bool wideCodes)
		{
			if (wideCodes)
			{
				w.writeUI16(kerningRecord.code1);
				w.writeUI16(kerningRecord.code2);
			}
			else
			{
				w.writeUI8(kerningRecord.code1);
				w.writeUI8(kerningRecord.code2);
			}
			w.writeUI16(kerningRecord.adjustment);
		}
		
		public override void  defineFontInfo(DefineFontInfo tag)
		{
			int idref = dict.getId(tag.font);
			tagw.writeUI16(idref);
			
			tagw.writeLengthString(tag.name);
			
			tagw.writeUBits(0, 3); // reserved
			tagw.writeBit(tag.shiftJIS);
			tagw.writeBit(tag.ansi);
			tagw.writeBit(tag.italic);
			tagw.writeBit(tag.bold);
			
			if (tag.code == flash.swf.TagValues_Fields.stagDefineFontInfo2)
			{
				tagw.writeBit(tag.wideCodes = true);
				tagw.writeUI8(tag.langCode);
			}
			else
			{
				if (!tag.wideCodes)
				{
					for (int i = 0; i < tag.codeTable.Length; i++)
					{
						if (tag.codeTable[i] > 255)
						{
							tag.wideCodes = true;
							break;
						}
					}
				}
				tagw.writeBit(tag.wideCodes);
			}
			
			if (tag.wideCodes)
			{
				for (int i = 0; i < tag.codeTable.Length; i++)
					tagw.writeUI16(tag.codeTable[i]);
			}
			else
			{
				for (int i = 0; i < tag.codeTable.Length; i++)
					tagw.writeUI8(tag.codeTable[i]);
			}
			encodeTag(tag);
		}
		
		public override void  defineFontInfo2(DefineFontInfo tag)
		{
			defineFontInfo(tag);
		}
		
		public override void  defineMorphShape(DefineMorphShape tag)
		{
			defineMorphShape2(tag);
		}
		
		public override void  defineMorphShape2(DefineMorphShape tag)
		{
			int id = dict.add(tag);
			tagw.writeUI16(id);
			encodeRect(tag.startBounds, tagw);
			encodeRect(tag.endBounds, tagw);
			if (tag.code == flash.swf.TagValues_Fields.stagDefineMorphShape2)
			{
				encodeRect(tag.startEdgeBounds, tagw);
				encodeRect(tag.endEdgeBounds, tagw);
				tagw.writeUBits(tag.reserved, 6);
				tagw.writeUBits(tag.usesNonScalingStrokes?1:0, 1);
				tagw.writeUBits(tag.usesScalingStrokes?1:0, 1);
			}
			tagw.write32(0);
			int pos = tagw.Pos;
			encodeMorphFillstyles(tag.fillStyles, tagw, tag.code);
			encodeMorphLinestyles(tag.lineStyles, tagw, tag.code);
			encodeShape(tag.startEdges, tagw, flash.swf.TagValues_Fields.stagDefineShape3, tag.fillStyles.Length, tag.lineStyles.Length);
			tagw.write32at(pos - 4, tagw.Pos - pos);
			// end shape contains only edges, no style information
			encodeShape(tag.endEdges, tagw, flash.swf.TagValues_Fields.stagDefineShape3, 0, 0);
			encodeTag(tag);
		}
		
		private void  encodeMorphFillstyles(MorphFillStyle[] fillStyles, SwfEncoder w, int code)
		{
			int count = fillStyles.Length;
			if (count >= 0xFF)
			{
				w.writeUI8(0xFF);
				w.writeUI16(count);
			}
			else
			{
				w.writeUI8(count);
			}
			
			for (int i = 0; i < count; i++)
			{
				encodeMorphFillstyle(fillStyles[i], w, code);
			}
		}
		
		private void  encodeMorphFillstyle(MorphFillStyle style, SwfEncoder w, int code)
		{
			w.writeUI8(style.type);
			switch (style.type)
			{
				
				case FillStyle.FILL_SOLID:  // 0x00
					encodeRGBA(style.startColor, w);
					encodeRGBA(style.endColor, w);
					break;
				
				case FillStyle.FILL_GRADIENT: 
				// 0x10 linear gradient fill
				case FillStyle.FILL_RADIAL_GRADIENT: 
				// 0x12 radial gradient fill
				case FillStyle.FILL_FOCAL_RADIAL_GRADIENT:  // 0x13 focal radial gradient fill
					encodeMatrix(style.startGradientMatrix, w);
					encodeMatrix(style.endGradientMatrix, w);
					encodeMorphGradient(style.gradRecords, w);
					if (style.type == FillStyle.FILL_FOCAL_RADIAL_GRADIENT && code == flash.swf.TagValues_Fields.stagDefineMorphShape2)
					{
						w.writeSI16(style.ratio1);
						w.writeSI16(style.ratio2);
					}
					break;
				
				case FillStyle.FILL_BITS: 
				// 0x40 tiled bitmap fill
				case (FillStyle.FILL_BITS | FillStyle.FILL_BITS_CLIP): 
				// 0x41 clipped bitmap fill
				case (FillStyle.FILL_BITS | FillStyle.FILL_BITS_NOSMOOTH): 
				// 0x42 tiled non-smoothed fill
				case (FillStyle.FILL_BITS | FillStyle.FILL_BITS_CLIP | FillStyle.FILL_BITS_NOSMOOTH):  // 0x43 clipped non-smoothed fill
					w.writeUI16(dict.getId(style.bitmap));
					encodeMatrix(style.startBitmapMatrix, w);
					encodeMatrix(style.endBitmapMatrix, w);
					break;
				
				default: 
					assert(false);
					//throw new IOException("unrecognized fill style type: " + style.type);
					break;
				
			}
		}
		
		private void  encodeMorphGradient(MorphGradRecord[] gradRecords, SwfEncoder w)
		{
			w.writeUI8(gradRecords.Length);
			for (int i = 0; i < gradRecords.Length; i++)
			{
				MorphGradRecord record = gradRecords[i];
				w.writeUI8(record.startRatio);
				encodeRGBA(record.startColor, w);
				w.writeUI8(record.endRatio);
				encodeRGBA(record.endColor, w);
			}
		}
		
		private void  encodeMorphLinestyles(MorphLineStyle[] lineStyles, SwfEncoder w, int code)
		{
			if (lineStyles.Length >= 0xFF)
			{
				w.writeUI8(0xFF);
				w.writeUI16(lineStyles.Length);
			}
			else
			{
				w.writeUI8(lineStyles.Length);
			}
			
			for (int i = 0; i < lineStyles.Length; i++)
			{
				MorphLineStyle style = lineStyles[i];
				w.writeUI16(style.startWidth);
				w.writeUI16(style.endWidth);
				if (code == flash.swf.TagValues_Fields.stagDefineMorphShape2)
				{
					w.writeUBits(style.startCapsStyle, 2);
					w.writeUBits(style.jointStyle, 2);
					w.writeBit(style.hasFill);
					w.writeBit(style.noHScale);
					w.writeBit(style.noVScale);
					w.writeBit(style.pixelHinting);
					w.writeUBits(0, 5); // reserved
					w.writeBit(style.noClose);
					w.writeUBits(style.endCapsStyle, 2);
					if (style.jointStyle == 2)
					{
						w.writeUI16(style.miterLimit);
					}
				}
				if (!style.hasFill)
				{
					encodeRGBA(style.startColor, w);
					encodeRGBA(style.endColor, w);
				}
				if (style.hasFill)
				{
					encodeMorphFillstyle(style.fillType, w, code);
				}
			}
		}
		
		public override void  defineShape(DefineShape tag)
		{
			int id = dict.add(tag);
			tagw.writeUI16(id);
			encodeRect(tag.bounds, tagw);
			if (tag.code == flash.swf.TagValues_Fields.stagDefineShape6)
			{
				encodeRect(tag.edgeBounds, tagw);
				tagw.writeUBits(0, 6);
				tagw.writeBit(tag.usesNonScalingStrokes);
				tagw.writeBit(tag.usesScalingStrokes);
			}
			encodeShapeWithStyle(tag.shapeWithStyle, tagw, tag.code);
			encodeTag(tag);
		}
		
		private void  encodeShapeWithStyle(ShapeWithStyle shapeWithStyle, SwfEncoder w, int shape)
		{
			encodeFillstyles(shapeWithStyle.fillstyles, w, shape);
			encodeLinestyles(shapeWithStyle.linestyles, w, shape);
			
			encodeShape(shapeWithStyle, w, shape, shapeWithStyle.fillstyles.Count, shapeWithStyle.linestyles.Count);
		}
		
		public override void  defineShape2(DefineShape tag)
		{
			defineShape(tag);
		}
		
		public override void  defineShape3(DefineShape tag)
		{
			defineShape(tag);
		}
		
		public override void  defineShape6(DefineShape tag)
		{
			defineShape(tag);
		}
		
		public override void  defineSound(DefineSound tag)
		{
			int id = dict.add(tag);
			tagw.writeUI16(id);
			tagw.writeUBits(tag.format, 4);
			tagw.writeUBits(tag.rate, 2);
			tagw.writeUBits(tag.size, 1);
			tagw.writeUBits(tag.type, 1);
			tagw.write32((int) tag.sampleCount);
			tagw.write(tag.data);
			encodeTag(tag);
		}
		
		public override void  defineSprite(DefineSprite tag)
		{
			int id = dict.add(tag);
			tagw.writeUI16(id);
			tagw.writeUI16(tag.framecount);
			
			if (Debug)
			{
				debug.adjust = writer.Pos + 6;
			}
			
			// save frame count
			int oldFrames = frames;
			frames = 0;
			
			// save the movie writer, and push a new writer
			SwfEncoder oldWriter = writer;
			writer = tagw;
			tagw = createEncoder(SwfVersion);
			
			// write sprite tags
			System.Collections.IList tags = tag.tagList.tags;
			int size = tags.Count;
			for (int i = 0; i < size; i++)
			{
				Tag t = (Tag) tags[i];
				if (!(t is DefineTag))
					t.visit(this);
			}
			
			// terminate with end marker
			writer.writeUI16(0);
			
			// update frame count
			writer.writeUI16at(2, frames);
			
			// restore writers
			tagw = writer;
			writer = oldWriter;
			frames = oldFrames;
			
			if (Debug)
			{
				debug.adjust = 0;
			}
			
			encodeTag(tag);
		}
		
		public override void  defineText(DefineText tag)
		{
			encodeDefineText(tag, tagw, tag.code);
			encodeTag(tag);
		}
		
		private void  encodeDefineText(DefineText tag, SwfEncoder w, int type)
		{
			int id = dict.add(tag);
			w.writeUI16(id);
			encodeRect(tag.bounds, w);
			encodeMatrix(tag.matrix, w);
			int length = tag.records.Count;
			
			// compute necessary bit width
			int glyphBits = 0;
			int advanceBits = 0;
			for (int i = 0; i < length; i++)
			{
				TextRecord tr = (TextRecord) tag.records[i];
				
				for (int j = 0; j < tr.entries.Length; j++)
				{
					GlyphEntry entry = tr.entries[j];
					
					while (entry.Index > (1 << glyphBits))
						glyphBits++;
					while (System.Math.Abs(entry.advance) > (1 << advanceBits))
						advanceBits++;
				}
			}
			
			// increment to get from bit index to bit count.
			++glyphBits;
			++advanceBits;
			
			w.writeUI8(glyphBits);
			w.writeUI8(++advanceBits); // add one extra bit because advances are signed
			
			for (int i = 0; i < length; i++)
			{
				TextRecord record = (TextRecord) tag.records[i];
				encodeTextRecord(record, w, type, glyphBits, advanceBits);
			}
			
			w.writeUI8(0);
		}
		
		private void  encodeFilterList(System.Collections.IList filters, SwfEncoder w)
		{
			int count = filters.Count;
			w.writeUI8(count);
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			for (System.Collections.IEnumerator it = filters.GetEnumerator(); it.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				Filter f = (Filter) it.Current;
				w.writeUI8(f.getID());
				// I've never quite understood why the serialization code isn't in the tags themselves..
				switch (f.getID())
				{
					
					case DropShadowFilter.ID:  encodeDropShadowFilter(w, (DropShadowFilter) f); break;
					
					case BlurFilter.ID:  encodeBlurFilter(w, (BlurFilter) f); break;
					
					case ConvolutionFilter.ID:  encodeConvolutionFilter(w, (ConvolutionFilter) f); break;
					
					case GlowFilter.ID:  encodeGlowFilter(w, (GlowFilter) f); break;
					
					case BevelFilter.ID:  encodeBevelFilter(w, (BevelFilter) f); break;
					
					case ColorMatrixFilter.ID:  encodeColorMatrixFilter(w, (ColorMatrixFilter) f); break;
					
					case GradientGlowFilter.ID:  encodeGradientGlowFilter(w, (GradientGlowFilter) f); break;
					
					case GradientBevelFilter.ID:  encodeGradientBevelFilter(w, (GradientBevelFilter) f); break;
					}
			}
		}
		
		private void  encodeDropShadowFilter(SwfEncoder w, DropShadowFilter f)
		{
			encodeRGBA(f.color, w);
			w.write32(f.blurX);
			w.write32(f.blurY);
			w.write32(f.angle);
			w.write32(f.distance);
			w.writeUI16(f.strength);
			w.writeUI8(f.flags);
		}
		
		private void  encodeBlurFilter(SwfEncoder w, BlurFilter f)
		{
			w.write32(f.blurX);
			w.write32(f.blurY);
			w.writeUI8(f.passes);
		}
		private void  encodeColorMatrixFilter(SwfEncoder w, ColorMatrixFilter f)
		{
			for (int i = 0; i < 20; ++i)
			{
				w.writeFloat(f.values[i]);
			}
		}
		private void  encodeConvolutionFilter(SwfEncoder w, ConvolutionFilter f)
		{
			w.writeUI8(f.matrixX);
			w.writeUI8(f.matrixY);
			w.writeFloat(f.divisor);
			w.writeFloat(f.bias);
			for (int i = 0; i < f.matrix.Length; ++i)
				w.writeFloat(f.matrix[i]);
			w.writeUI8(f.flags);
		}
		private void  encodeGlowFilter(SwfEncoder w, GlowFilter f)
		{
			encodeRGBA(f.color, w);
			w.write32(f.blurX);
			w.write32(f.blurY);
			w.writeUI16(f.strength);
			w.writeUI8(f.flags);
		}
		private void  encodeBevelFilter(SwfEncoder w, BevelFilter f)
		{
			encodeRGBA(f.shadowColor, w);
			encodeRGBA(f.highlightColor, w);
			w.write32(f.blurX);
			w.write32(f.blurY);
			w.write32(f.angle);
			w.write32(f.distance);
			w.writeUI16(f.strength);
			w.writeUI8(f.flags);
		}
		
		private void  encodeGradientGlowFilter(SwfEncoder w, GradientGlowFilter f)
		{
			w.writeUI8(f.numcolors);
			for (int i = 0; i < f.numcolors; ++i)
				encodeRGBA(f.gradientColors[i], w);
			for (int i = 0; i < f.numcolors; ++i)
				w.writeUI8(f.gradientRatio[i]);
			//w.write32( f.color );
			w.write32(f.blurX);
			w.write32(f.blurY);
			w.write32(f.angle);
			w.write32(f.distance);
			w.writeUI16(f.strength);
			w.writeUI8(f.flags);
		}
		private void  encodeGradientBevelFilter(SwfEncoder w, GradientBevelFilter f)
		{
			w.writeUI8(f.numcolors);
			for (int i = 0; i < f.numcolors; ++i)
				encodeRGBA(f.gradientColors[i], w);
			for (int i = 0; i < f.numcolors; ++i)
				w.writeUI8(f.gradientRatio[i]);
			
			//        w.write32( f.shadowColor );
			//        w.write32( f.highlightColor );
			w.write32(f.blurX);
			w.write32(f.blurY);
			w.write32(f.angle);
			w.write32(f.distance);
			w.writeUI16(f.strength);
			w.writeUI8(f.flags);
		}
		
		private void  encodeTextRecord(TextRecord record, SwfEncoder w, int type, int glyphBits, int advanceBits)
		{
			w.writeUI8(record.flags);
			
			if (record.hasFont())
			{
				w.writeUI16(dict.getId(record.font));
			}
			
			if (record.hasColor())
			{
				if (type == flash.swf.TagValues_Fields.stagDefineText2)
					encodeRGBA(record.color, w);
				else
					encodeRGB(record.color, w);
			}
			
			if (record.hasX())
			{
				w.writeSI16(record.xOffset);
			}
			
			if (record.hasY())
			{
				w.writeSI16(record.yOffset);
			}
			
			if (record.hasHeight())
			{
				w.writeUI16(record.height);
			}
			
			w.writeUI8(record.entries.Length);
			
			for (int i = 0; i < record.entries.Length; i++)
			{
				w.writeUBits(record.entries[i].Index, glyphBits);
				w.writeSBits(record.entries[i].advance, advanceBits);
			}
			w.flushBits();
		}
		
		public override void  defineText2(DefineText tag)
		{
			defineText(tag);
		}
		
		public override void  defineVideoStream(DefineVideoStream tag)
		{
			int id = dict.add(tag);
			tagw.writeUI16(id);
			tagw.writeUI16(tag.numFrames);
			tagw.writeUI16(tag.width);
			tagw.writeUI16(tag.height);
			
			tagw.writeUBits(0, 4); // reserved
			tagw.writeUBits(tag.deblocking, 3);
			tagw.writeBit(tag.smoothing);
			
			tagw.writeUI8(tag.codecID);
			encodeTag(tag);
		}
		
		public override void  doAction(DoAction tag)
		{
			int adjust = 0;
			if (Debug)
			{
				adjust = writer.Pos + 6;
				debug.adjust += adjust;
			}
			
			new ActionEncoder(tagw, debug).encode(tag.actionList);
			tagw.writeUI8(0);
			encodeTag(tag);
			
			if (Debug)
			{
				debug.adjust -= adjust;
			}
		}
		
		public override void  doInitAction(DoInitAction tag)
		{
			int adjust = 0;
			if (Debug)
			{
				adjust = writer.Pos + 6;
				debug.adjust += adjust;
			}
			
			int idref = dict.getId(tag.sprite);
			tagw.writeUI16(idref);
			new ActionEncoder(tagw, debug).encode(tag.actionList);
			tagw.writeUI8(0);
			encodeTag(tag);
			
			if (Debug)
			{
				debug.adjust -= adjust;
			}
		}
		
		public override void  enableDebugger(EnableDebugger tag)
		{
			tagw.writeString(tag.password);
			encodeTag(tag);
		}
		
		public override void  enableDebugger2(EnableDebugger tag)
		{
			// This corresponds to the constant used in the player,
			// core/splay.cpp, in ScriptThread::EnableDebugger().
			tagw.writeUI16(0x1975);
			tagw.writeString(tag.password);
			encodeTag(tag);
		}
		
		public override void  exportAssets(ExportAssets tag)
		{
			tagw.writeUI16(tag.exports.Count);
			System.Collections.IEnumerator it = tag.exports.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				DefineTag ref_Renamed = (DefineTag) it.Current;
				int idref = dict.getId(ref_Renamed);
				tagw.writeUI16(idref);
				assert(ref_Renamed.name != null); // exported symbols must have names
				tagw.writeString(ref_Renamed.name);
				dict.addName(ref_Renamed, ref_Renamed.name);
			}
			encodeTag(tag);
		}
		
		public override void  symbolClass(SymbolClass tag)
		{
			tagw.writeUI16(tag.class2tag.Count + (tag.topLevelClass != null?1:0));
			//UPGRADE_TODO: Method 'java.util.Map.entrySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilMapentrySet'"
			System.Collections.IEnumerator it = new SupportClass.HashSetSupport(tag.class2tag).GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				System.Collections.DictionaryEntry e = (System.Collections.DictionaryEntry) it.Current;
				System.String name = (System.String) e.Key;
				DefineTag ref_Renamed = (DefineTag) e.Value;
				
				int idref = dict.getId(ref_Renamed);
				tagw.writeUI16(idref);
				tagw.writeString(name);
			}
			if (tag.topLevelClass != null)
			{
				tagw.writeUI16(0);
				tagw.writeString(tag.topLevelClass);
			}
			encodeTag(tag);
		}
		
		public override void  frameLabel(FrameLabel tag)
		{
			tagw.writeString(tag.label);
			if (tag.anchor && SwfVersion >= 6)
			{
				tagw.writeUI8(1);
			}
			encodeTag(tag);
		}
		
		public override void  importAssets(ImportAssets tag)
		{
			tagw.writeString(tag.url);
			if (tag.code == flash.swf.TagValues_Fields.stagImportAssets2)
			{
				tagw.writeUI8(tag.downloadNow?1:0);
				tagw.writeUI8(tag.SHA1 != null?1:0);
				if (tag.SHA1 != null)
				{
					tagw.write(tag.SHA1);
				}
			}
			tagw.writeUI16(tag.importRecords.Count);
			System.Collections.IEnumerator it = tag.importRecords.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
			while (it.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				ImportRecord record = (ImportRecord) it.Current;
				int id = dict.add(record);
				tagw.writeUI16(id);
				tagw.writeString(record.name);
			}
			encodeTag(tag);
		}
		
		public override void  importAssets2(ImportAssets tag)
		{
			importAssets(tag);
		}
		
		public override void  jpegTables(GenericTag tag)
		{
			encodeTagHeader(tag.code, tag.data.Length, false);
			writer.write(tag.data);
		}
		
		public override void  placeObject(PlaceObject tag)
		{
			int idref = dict.getId(tag.ref_Renamed);
			tagw.writeUI16(idref);
			tagw.writeUI16(tag.depth);
			encodeMatrix(tag.matrix, tagw);
			if (tag.colorTransform != null)
			{
				encodeCxform(tag.colorTransform, tagw);
			}
			encodeTag(tag);
		}
		
		public override void  placeObject2(PlaceObject tag)
		{
			placeObject23(tag);
		}
		
		public override void  placeObject3(PlaceObject tag)
		{
			placeObject23(tag);
		}
		
		public virtual void  placeObject23(PlaceObject tag)
		{
			tagw.writeUI8(tag.flags);
			if (tag.code == flash.swf.TagValues_Fields.stagPlaceObject3)
			{
				tagw.writeUI8(tag.flags2);
			}
			tagw.writeUI16(tag.depth);
			if (tag.hasClassName())
			{
				tagw.writeString(tag.className);
			}
			if (tag.hasCharID())
			{
				int idref = dict.getId(tag.ref_Renamed);
				tagw.writeUI16(idref);
			}
			if (tag.hasMatrix())
			{
				encodeMatrix(tag.matrix, tagw);
			}
			if (tag.hasCxform())
			{
				// ed 5/22/03 the SWF 6 file format spec says this should be a CXFORM, but
				// the spec is wrong.  the player expects a CXFORMA.
				encodeCxforma(((CXFormWithAlpha) tag.colorTransform), tagw);
			}
			if (tag.hasRatio())
			{
				tagw.writeUI16(tag.ratio);
			}
			if (tag.hasName())
			{
				tagw.writeString(tag.name);
			}
			if (tag.hasClipDepth())
			{
				tagw.writeUI16(tag.clipDepth);
			}
			if (tag.code == flash.swf.TagValues_Fields.stagPlaceObject3)
			{
				if (tag.hasFilterList())
				{
					encodeFilterList(tag.filters, tagw);
				}
				if (tag.hasBlendMode())
				{
					tagw.writeUI8(tag.blendMode);
				}
			}
			if (tag.hasClipAction())
			{
				int adjust = 0;
				if (Debug)
				{
					adjust = writer.Pos + 6;
					debug.adjust += adjust;
				}
				new ActionEncoder(tagw, debug).encodeClipActions(tag.clipActions);
				if (Debug)
				{
					debug.adjust -= adjust;
				}
			}
			encodeTag(tag);
		}
		
		public override void  protect(GenericTag tag)
		{
			if (tag.data != null)
			{
				encodeTagHeader(tag.code, tag.data.Length, false);
				writer.write(tag.data);
			}
			else
			{
				encodeTagHeader(tag.code, 0, false);
			}
		}
		
		public override void  removeObject(RemoveObject tag)
		{
			encodeTagHeader(tag.code, 4, false);
			int idref = dict.getId(tag.ref_Renamed);
			writer.writeUI16(idref);
			writer.writeUI16(tag.depth);
		}
		
		public override void  removeObject2(RemoveObject tag)
		{
			encodeTagHeader(tag.code, 2, false);
			writer.writeUI16(tag.depth);
		}
		
		public override void  showFrame(ShowFrame tag)
		{
			encodeTagHeader(tag.code, 0, false);
			frames++;
		}
		
		public override void  soundStreamBlock(GenericTag tag)
		{
			encodeTagHeader(tag.code, tag.data.Length, false);
			writer.write(tag.data);
		}
		
		public override void  soundStreamHead(SoundStreamHead tag)
		{
			int length = 4;
			
			// we need to add two bytes for an extra SI16 (latencySeek)
			if (tag.compression == SoundStreamHead.sndCompressMP3)
			{
				length += 2;
			}
			
			encodeTagHeader(tag.code, length, false);
			
			// 1 byte
			writer.writeUBits(0, 4); // reserved
			writer.writeUBits(tag.playbackRate, 2);
			writer.writeUBits(tag.playbackSize, 1);
			writer.writeUBits(tag.playbackType, 1);
			
			// 1 byte
			writer.writeUBits(tag.compression, 4);
			writer.writeUBits(tag.streamRate, 2);
			writer.writeUBits(tag.streamSize, 1);
			writer.writeUBits(tag.streamType, 1);
			
			// 2 bytes
			writer.writeUI16(tag.streamSampleCount);
			
			if (tag.compression == SoundStreamHead.sndCompressMP3)
			{
				// 2 bytes
				writer.writeSI16(tag.latencySeek);
			}
		}
		
		public override void  soundStreamHead2(SoundStreamHead tag)
		{
			soundStreamHead(tag);
		}
		
		public override void  startSound(StartSound tag)
		{
			int idref = dict.getId(tag.sound);
			tagw.writeUI16(idref);
			encodeSoundInfo(tag.soundInfo, tagw);
			encodeTag(tag);
		}
		
		public override void  videoFrame(VideoFrame tag)
		{
			encodeTagHeader(tag.code, 4 + tag.videoData.Length, false);
			int idref = dict.getId(tag.stream);
			writer.writeUI16(idref);
			writer.writeUI16(tag.frameNum);
			writer.write(tag.videoData);
		}
		
		public override void  defineSceneAndFrameLabelData(DefineSceneAndFrameLabelData tag)
		{
			encodeTagHeader(tag.code, tag.data.Length, false);
			writer.write(tag.data);
		}
		
		public override void  doABC(DoABC tag)
		{
			if (tag.code == flash.swf.TagValues_Fields.stagDoABC2)
			{
				encodeTagHeader(tag.code, 4 + tag.name.Length + 1 + tag.abc.Length, false);
				writer.write32(tag.flag);
				writer.writeString(tag.name);
			}
			else
			{
				encodeTagHeader(tag.code, tag.abc.Length, false);
			}
			
			writer.write(tag.abc);
		}
		
		public override void  unknown(GenericTag tag)
		{
			encodeTagHeader(tag.code, tag.data.Length, false);
			writer.write(tag.data);
		}
		
		public virtual sbyte[] toByteArray()
		{
			//TODO this could be improved, tricky bit is that writeTo is not trivial
			//     and has the side effect of compressing (meaning the writer.size()
			//     may be larger than necessary)
			System.IO.MemoryStream out_Renamed = new System.IO.MemoryStream(writer.Length);
			writeTo(out_Renamed);
			return SupportClass.ToSByteArray(out_Renamed.ToArray());
		}
		
		public override void  scriptLimits(ScriptLimits tag)
		{
			tagw.writeUI16(tag.scriptRecursionLimit);
			tagw.writeUI16(tag.scriptTimeLimit);
			encodeTag(tag);
		}
	}
}