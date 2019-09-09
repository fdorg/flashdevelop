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
	public class DefineButtonSound:Tag
	{
		override public System.Collections.IEnumerator References
		{
			get
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList(5);
				list.Add(button);
				if (sound0 != null)
					list.Add(sound0);
				if (sound1 != null)
					list.Add(sound1);
				if (sound2 != null)
					list.Add(sound2);
				if (sound3 != null)
					list.Add(sound3);
				return list.GetEnumerator();
			}
			
		}
		public DefineButtonSound():base(Flash.Swf.TagValues.stagDefineButtonSound)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			h.defineButtonSound(this);
		}
		
		public DefineTag sound0;
		public SoundInfo info0;
		public DefineTag sound1;
		public SoundInfo info1;
		public DefineTag sound2;
		public SoundInfo info2;
		public DefineTag sound3;
		public SoundInfo info3;
		
		public DefineButton button;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is DefineButtonSound))
			{
				DefineButtonSound defineButtonSound = (DefineButtonSound) object_Renamed;
				
				// [ed] don't compare button because that would be infinite recursion
				if (equals(defineButtonSound.sound0, this.sound0) && equals(defineButtonSound.info0, this.info0) && equals(defineButtonSound.sound1, this.sound1) && equals(defineButtonSound.info1, this.info1) && equals(defineButtonSound.sound2, this.sound2) && equals(defineButtonSound.info2, this.info2) && equals(defineButtonSound.sound3, this.sound3) && equals(defineButtonSound.info3, this.info3))
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
