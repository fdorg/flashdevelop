////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2004-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
namespace flash.swf.builder.types
{
	
	/// <author>  Peter Farland
	/// </author>
	public class PathIteratorWrapper : ShapeIterator
	{
		virtual public bool Done
		{
			get
			{
				//UPGRADE_ISSUE: Method 'java.awt.geom.PathIterator.isDone' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtgeomPathIteratorisDone'"
				return pi.isDone();
			}
			
		}
		//UPGRADE_TODO: Interface 'java.awt.geom.PathIterator' was converted to 'System.Drawing.Drawing2D.GraphicsPathIterator' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
		private System.Drawing.Drawing2D.GraphicsPathIterator pi;
		
		//UPGRADE_TODO: Interface 'java.awt.geom.PathIterator' was converted to 'System.Drawing.Drawing2D.GraphicsPathIterator' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
		public PathIteratorWrapper(System.Drawing.Drawing2D.GraphicsPathIterator pi)
		{
			this.pi = pi;
		}
		
		public virtual short currentSegment(double[] coords)
		{
			//UPGRADE_ISSUE: Method 'java.awt.geom.PathIterator.currentSegment' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtgeomPathIteratorcurrentSegment_double[]'"
			int code = pi.currentSegment(coords);
			return (short) code;
		}
		
		public virtual void  next()
		{
			//UPGRADE_ISSUE: Method 'java.awt.geom.PathIterator.next' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtgeomPathIteratornext'"
			pi.next();
		}
	}
}