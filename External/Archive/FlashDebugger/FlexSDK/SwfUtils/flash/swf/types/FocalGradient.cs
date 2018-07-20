// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
namespace Flash.Swf.Types
{
	
	/// <author>  Roger Gonzalez
	/// </author>
	public class FocalGradient:Gradient
	{
		public float focalPoint;
		
		public  override bool Equals(System.Object o)
		{
			if (!(o is FocalGradient) || !base.Equals(o))
				return false;
			
			FocalGradient otherGradient = (FocalGradient) o;
			return (otherGradient.focalPoint == focalPoint);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
