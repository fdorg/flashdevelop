// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
using Tag = Flash.Swf.Tag;
using TagHandler = Flash.Swf.TagHandler;
using TagValues = Flash.Swf.TagValues;
namespace Flash.Swf.Tags
{
	
	/// <summary> ScriptLimits.  Change the player's default scripting limits. This
	/// tag applies until the next ScriptLimits is encountered at runtime;
	/// it can occur anywhere and any number of times.
	/// 
	/// </summary>
	/// <since> SWF7
	/// 
	/// </since>
	/// <author>  Paul Reilly
	/// </author>
	
	public class ScriptLimits:Tag
	{
		public ScriptLimits(int scriptRecursionLimit, int scriptTimeLimit):base(Flash.Swf.TagValues.stagScriptLimits)
		{
			
			this.scriptRecursionLimit = scriptRecursionLimit;
			this.scriptTimeLimit = scriptTimeLimit;
		}
		
		public override void  visit(TagHandler tagHandler)
		{
			tagHandler.scriptLimits(this);
		}
		
		public int scriptRecursionLimit;
		public int scriptTimeLimit;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is ScriptLimits))
			{
				ScriptLimits scriptLimits = (ScriptLimits) object_Renamed;
				
				if ((scriptLimits.scriptRecursionLimit == this.scriptRecursionLimit) && (scriptLimits.scriptTimeLimit == this.scriptTimeLimit))
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
