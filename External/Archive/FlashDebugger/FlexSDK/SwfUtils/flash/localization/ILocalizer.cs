////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2006-2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;

namespace Flash.Localization
{
	
	/// <author>  Roger Gonzalez
	/// </author>
	public interface ILocalizer
	{
		ILocalizedText getLocalizedText(System.Globalization.CultureInfo locale, String id);
	}
}
