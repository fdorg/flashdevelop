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
using FontFace = flash.fonts.FontFace;
using FontBuilder = flash.swf.builder.tags.FontBuilder;
using ZoneRecord = flash.swf.tags.ZoneRecord;
using Trace = flash.util.Trace;
namespace flash.swf.builder.types
{
	
	/// <summary> A simple abstract class to decouple FlashType ZoneRecord construction from
	/// FontBuilder.
	/// </summary>
	public class ZoneRecordBuilder
	{
		virtual public System.String FontAlias
		{
			set
			{
				fontAlias = value;
			}
			
		}
		virtual public FontBuilder FontBuilder
		{
			set
			{
				fontBuilder = value;
			}
			
		}
		virtual public FontFace FontFace
		{
			set
			{
				fontFace = value;
			}
			
		}
		private const System.String DEFAULT_BUILDER = "flash.fonts.flashtype.FlashTypeZoneRecordBuilder";
		
		protected internal System.String fontAlias;
		protected internal FontBuilder fontBuilder;
		protected internal FontFace fontFace;
		
		protected internal ZoneRecordBuilder()
		{
		}
		
		/// <summary> This no-op method returns an empty ZoneRecord. Subclasses should
		/// override this method. 
		/// </summary>
		public virtual ZoneRecord build(char character)
		{
			// Return an empty Zone Record...
			ZoneRecord zoneRecord = new ZoneRecord();
			zoneRecord.numZoneData = 2;
			zoneRecord.zoneData = new long[]{0, 0};
			zoneRecord.zoneMask = 0;
			return zoneRecord;
		}
		
		public static ZoneRecordBuilder createInstance()
		{
			try
			{
				//UPGRADE_TODO: The differences in the format  of parameters for method 'java.lang.Class.forName'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
				System.Type c = System.Type.GetType(DEFAULT_BUILDER);
				ZoneRecordBuilder builder = (ZoneRecordBuilder) System.Activator.CreateInstance(c);
				return builder;
			}
			//UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to 'System.Exception' which has different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
			catch (System.Exception t)
			{
				if (Trace.error)
					Trace.trace("ZoneRecordBuilder implementation not found '" + DEFAULT_BUILDER + "'");
			}
			
			return null;
		}
	}
}