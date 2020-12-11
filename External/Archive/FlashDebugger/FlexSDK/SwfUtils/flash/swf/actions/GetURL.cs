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
using System;
using ActionHandler = Flash.Swf.ActionHandler;
using ActionConstants = Flash.Swf.ActionConstants;
namespace Flash.Swf.Actions
{
	
	/// <author>  Clement Wong
	/// </author>
	public class GetURL:Flash.Swf.Action
	{
		public GetURL():base(Flash.Swf.ActionConstants.sactionGetURL)
		{
		}
		
		public override void  visit(ActionHandler h)
		{
			h.getURL(this);
		}
		
		/// <summary> the URL can be of any type, including an HTML file, an image, or another SWF movie.</summary>
		public String url;
		
		/// <summary> If this movie is playing in a browser, the url will be displayed in the
		/// browser frame given by this target string.  The special target names
		/// "_level0" and "_level1" are used to load another SWF movie into levels 0
		/// and 1 respectively.
		/// </summary>
		public String target;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is GetURL))
			{
				GetURL getURL = (GetURL) object_Renamed;
				
				if (equals(getURL.url, this.url) && equals(getURL.target, this.target))
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
