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
	public class CXForm
	{
		public bool hasAdd;
		public bool hasMult;
		
		public int redMultTerm;
		public int greenMultTerm;
		public int blueMultTerm;
		public int redAddTerm;
		public int greenAddTerm;
		public int blueAddTerm;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is CXForm)
			{
				CXForm cxForm = (CXForm) object_Renamed;
				
				if ((cxForm.hasAdd == this.hasAdd) && (cxForm.hasMult == this.hasMult) && (cxForm.redMultTerm == this.redMultTerm) && (cxForm.greenMultTerm == this.greenMultTerm) && (cxForm.blueMultTerm == this.blueMultTerm) && (cxForm.redAddTerm == this.redAddTerm) && (cxForm.greenAddTerm == this.greenAddTerm) && (cxForm.blueAddTerm == this.blueAddTerm))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override String ToString()
		{
			return redMultTerm + "r" + (redAddTerm >= 0?"+":"") + redAddTerm + " " + greenMultTerm + "g" + (greenAddTerm >= 0?"+":"") + greenAddTerm + " " + blueMultTerm + "b" + (blueAddTerm >= 0?"+":"") + blueAddTerm;
		}
		
		public virtual int nbits()
		{
			// two step process to find maximum value of 6 numbers because "FSWFStream::MaxNum" takes only 4 arguments
			int max = SwfEncoder.maxNum(redMultTerm, greenMultTerm, blueMultTerm, redAddTerm);
			max = SwfEncoder.maxNum(greenAddTerm, blueAddTerm, max, 0);
			return SwfEncoder.minBits(max, 1);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
