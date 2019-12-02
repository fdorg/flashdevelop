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
using Flash.Swf;
using ArrayUtil = Flash.Util.ArrayUtil;

namespace Flash.Swf.Tags
{
	
	/// <summary> 
	/// DefineBits
	/// This tag defines a bitmap character with JPEG compression. It contains only the JPEG
	/// compressed image data (from the Frame Header onward). A separate JPEGTables tag contains
	/// the JPEG encoding data used to encode this image (the Tables/Misc segment). Note that only
	/// one JPEGTables tag is allowed in a SWF file, and thus all bitmaps defined with DefineBits must
	/// share common encoding tables.
	/// <p />
	/// The data in this tag begins with the JPEG SOI marker 0xFF, 0xD8 and ends with the EOI
	/// marker 0xFF, 0xD9.
	/// <p />
	/// DefineBits2 - includes all jpeg data
	/// DefineBits3 - includes all data plus a transparency map
	/// 
	/// </summary>
	/// <since> SWF1
	/// 
	/// </since>
	/// <author>  Clement Wong
	/// </author>
	public class DefineBits:DefineTag
	{
		public DefineBits(int code):base(code)
		{
		}
		
		public override void  visit(Flash.Swf.TagHandler h)
		{
			if (code == Flash.Swf.TagValues.stagDefineBitsJPEG2)
				h.defineBitsJPEG2(this);
			else
				h.defineBits(this);
		}
		
		public override Tag SimpleReference
		{
            get
            {
                return jpegTables;
            }
		}
		
		
		/// <summary>there is only one JPEG table in the entire movie </summary>
		public GenericTag jpegTables;
		public byte[] data;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineBits))
			{
				DefineBits defineBits = (DefineBits) object_Renamed;
				
				if (ArrayUtil.equals(defineBits.data, this.data) && equals(defineBits.jpegTables, this.jpegTables))
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
