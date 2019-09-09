// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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
using Action = Flash.Swf.Action;
using ActionHandler = Flash.Swf.ActionHandler;
using ActionConstants = Flash.Swf.ActionConstants;
namespace Flash.Swf.Actions
{
	
	/// <author>  Clement Wong
	/// </author>
	public class GetURL2:Action
	{
		public GetURL2():base(Flash.Swf.ActionConstants.sactionGetURL2)
		{
		}
		
		public override void  visit(ActionHandler h)
		{
			h.getURL2(this);
		}
		
		/// <summary> 0 = None. Not a form request, don't encode variables.
		/// 1 = GET. variables encoded as a query string
		/// 2 = POST.  variables encoded as http request body
		/// </summary>
		public int method;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is GetURL2))
			{
				GetURL2 getURL2 = (GetURL2) object_Renamed;
				
				if (getURL2.method == this.method)
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
