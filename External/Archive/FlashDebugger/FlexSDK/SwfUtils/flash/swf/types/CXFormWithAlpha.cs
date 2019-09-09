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
using SwfEncoder = Flash.Swf.SwfEncoder;
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class CXFormWithAlpha:CXForm
	{
		public int alphaMultTerm;
		public int alphaAddTerm;
		
		public override int nbits()
		{
			// FFileWrite's MaxNum method takes only 4 arguments, so finding maximum value of 8 arguments takes three steps:
			int maxMult = SwfEncoder.maxNum(redMultTerm, greenMultTerm, blueMultTerm, alphaMultTerm);
			int maxAdd = SwfEncoder.maxNum(redAddTerm, greenAddTerm, blueAddTerm, alphaAddTerm);
			int maxValue = SwfEncoder.maxNum(maxMult, maxAdd, 0, 0);
			return SwfEncoder.minBits(maxValue, 1);
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is CXFormWithAlpha))
			{
				CXFormWithAlpha cxFormWithAlpha = (CXFormWithAlpha) object_Renamed;
				
				if ((cxFormWithAlpha.alphaMultTerm == this.alphaMultTerm) && (cxFormWithAlpha.alphaAddTerm == this.alphaAddTerm))
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
