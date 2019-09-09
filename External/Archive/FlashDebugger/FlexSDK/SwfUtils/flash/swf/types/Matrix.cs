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
	public class Matrix
	{
		public bool hasScale;
		public int scaleX;
		public int scaleY;
		public bool hasRotate;
		public int rotateSkew0;
		public int rotateSkew1;
		public int translateX;
		public int translateY;
		
		public Matrix()
		{
		}
		
		public Matrix(int translateX, int translateY)
		{
			this.translateX = translateX;
			this.translateY = translateY;
		}
		
		public Matrix(int translateX, int translateY, double scaleX, double scaleY)
		{
			this.translateX = translateX;
			this.translateY = translateY;
			setScale(scaleX, scaleY);
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is Matrix)
			{
				Matrix matrix = (Matrix) object_Renamed;
				
				if ((matrix.hasScale == this.hasScale) && (matrix.scaleX == this.scaleX) && (matrix.scaleY == this.scaleY) && (matrix.hasRotate == this.hasRotate) && (matrix.rotateSkew0 == this.rotateSkew0) && (matrix.rotateSkew1 == this.rotateSkew1) && (matrix.translateX == this.translateX) && (matrix.translateY == this.translateY))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override String ToString()
		{
			System.Text.StringBuilder b = new System.Text.StringBuilder();
			
			if (hasScale)
			{
				b.Append("s");
				b.Append((float) (scaleX / 65536.0)).Append(",").Append((float) (scaleY / 65536.0));
				b.Append(" ");
			}
			
			if (hasRotate)
			{
				b.Append("r");
				b.Append((float) (rotateSkew0 / 65536.0)).Append(",").Append((float) (rotateSkew1 / 65536.0));
				b.Append(" ");
			}
			
			b.Append("t");
			b.Append(translateX).Append(",").Append(translateY);
			
			return b.ToString();
		}
		
		public virtual int nTranslateBits()
		{
			return SwfEncoder.minBits(SwfEncoder.maxNum(translateX, translateY, 0, 0), 1);
		}
		
		public virtual int nRotateBits()
		{
			return SwfEncoder.minBits(SwfEncoder.maxNum(rotateSkew0, rotateSkew1, 0, 0), 1);
		}
		
		public virtual int nScaleBits()
		{
			return SwfEncoder.minBits(SwfEncoder.maxNum(scaleX, scaleY, 0, 0), 1);
		}
		
		public virtual void  setRotate(double r0, double r1)
		{
			hasRotate = true;
			rotateSkew0 = (int) (65536 * r0);
			rotateSkew1 = (int) (65536 * r1);
		}
		
		public virtual void  setScale(double x, double y)
		{
			hasScale = true;
			scaleX = (int) (65536 * x);
			scaleY = (int) (65536 * y);
		}
		/*
		public int xformx(int x, int y)
		{
		return (int)(x*(scaleX/65536.0) + y*(rotateSkew1/65536.0) + translateX);
		}
		public int xformy(int x, int y)
		{
		return (int)(x*(rotateSkew0/65536.0) + y*(scaleY/65536.0) + translateY);
		}
		
		public Rect xformRect(Rect in)
		{
		int xmin = xformx(in.xMin, in.yMin);
		int ymin = xformy(in.xMin, in.yMin);
		int xmax = xformx(in.xMin, in.yMin);
		int ymax = xformy(in.xMin, in.yMin);
		
		Rect out = new Rect();
		out.xMin = xmin < xmax ? xmin : xmax;
		out.yMin = ymin < ymax ? ymin : ymax;
		out.xMax = xmin > xmax ? xmin : xmax;
		out.yMax = ymin > ymax ? ymin : ymax;
		
		return out;
		} */
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
