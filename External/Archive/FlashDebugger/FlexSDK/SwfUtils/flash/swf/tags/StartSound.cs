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
using SoundInfo = Flash.Swf.Types.SoundInfo;
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class StartSound:Tag
	{
		public StartSound():base(Flash.Swf.TagValues.stagStartSound)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			h.startSound(this);
		}
		
		public override Tag SimpleReference
		{
            get
            {
                return sound;
            }
		}
		
		/// <summary> ID of sound character to play</summary>
		public DefineSound sound;
		
		/// <summary> sound style information</summary>
		public SoundInfo soundInfo;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is StartSound))
			{
				StartSound startSound = (StartSound) object_Renamed;
				
				if (equals(startSound.sound, this.sound) && equals(startSound.soundInfo, this.soundInfo))
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
