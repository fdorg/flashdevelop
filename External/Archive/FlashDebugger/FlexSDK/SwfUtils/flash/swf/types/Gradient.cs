////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using ArrayUtil = Flash.Util.ArrayUtil;
namespace Flash.Swf.Types
{
	
	/// <author>  Roger Gonzalez
	/// </author>
	public class Gradient
	{
		public const int SPREAD_MODE_PAD = 0;
		public const int SPREAD_MODE_REFLECT = 1;
		public const int SPREAD_MODE_REPEAT = 2;
		public const int INTERPOLATION_MODE_NORMAL = 0;
		public const int INTERPOLATION_MODE_LINEAR = 1;
		
		public int spreadMode;
		public int interpolationMode;
		public GradRecord[] records;
		
		public  override bool Equals(System.Object o)
		{
			if (!(o is Gradient))
				return false;
			
			Gradient otherGradient = (Gradient) o;
			return ((otherGradient.spreadMode == spreadMode) && (otherGradient.interpolationMode == interpolationMode) && ArrayUtil.equals(otherGradient.records, records));
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
