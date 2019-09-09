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

namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public abstract class EdgeRecord:ShapeRecord
	{
		/// <summary> The maximum value that an EdgeRecord delta can take in SWF.
		/// <p />
		/// This limit arises due to the SWF encoding format of a Shape's EdgeRecord.
		/// A delta is encoded using a specified bit length, however, the number
		/// of bits used to record this bitlength (minus 2) is restricted to 4 bits,
		/// which is 15 + 2 = 17 bits for unsigned, or 16 bits for signed.
		/// <p />
		/// This restricts the signed delta values to 65535 twips, i.e. (2 pow 16) - 1.
		/// 
		/// </summary>
		public const int MAX_DELTA_IN_TWIPS = 65535;
		
		public  override bool Equals(Object o)
		{
			return (base.Equals(o) && (o is EdgeRecord));
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
