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
namespace Flash.Swf.Types
{
	
	/// <author>  Clement Wong
	/// </author>
	public class ButtonCondAction
	{
		/// <summary> SWF 4+: key code
		/// Otherwise: always 0
		/// Valid key codes:
		/// 1: left arrow
		/// 2: right arrow
		/// 3: home
		/// 4: end
		/// 5: insert
		/// 6: delete
		/// 8: backspace
		/// 13: enter
		/// 14: up arrow
		/// 15: down arrow
		/// 16: page up
		/// 17: page down
		/// 18: tab
		/// 19: escape
		/// 32-126: follows ASCII
		/// </summary>
		public int keyPress;
		
		public bool overDownToIdle;
		public bool idleToOverDown;
		public bool outDownToIdle;
		public bool outDownToOverDown;
		public bool overDownToOutDown;
		public bool overDownToOverUp;
		public bool overUpToOverDown;
		public bool overUpToIdle;
		public bool idleToOverUp;
		
		/// <summary> actions to perform when this event is detected.</summary>
		public ActionList actionList;
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (object_Renamed is ButtonCondAction)
			{
				ButtonCondAction buttonCondAction = (ButtonCondAction) object_Renamed;
				
				if ((buttonCondAction.keyPress == this.keyPress) && (buttonCondAction.overDownToIdle == this.overDownToIdle) && (buttonCondAction.idleToOverDown == this.idleToOverDown) && (buttonCondAction.outDownToIdle == this.outDownToIdle) && (buttonCondAction.outDownToOverDown == this.outDownToOverDown) && (buttonCondAction.overDownToOutDown == this.overDownToOutDown) && (buttonCondAction.overDownToOverUp == this.overDownToOverUp) && (buttonCondAction.overUpToOverDown == this.overUpToOverDown) && (buttonCondAction.overUpToIdle == this.overUpToIdle) && (buttonCondAction.idleToOverUp == this.idleToOverUp) && (((buttonCondAction.actionList == null) && (this.actionList == null)) || ((buttonCondAction.actionList != null) && (this.actionList != null) && buttonCondAction.actionList.Equals(this.actionList))))
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override String ToString()
		{
			// return the flags as a string
			System.Text.StringBuilder b = new System.Text.StringBuilder();
			
			if (keyPress != 0)
				b.Append("keyPress<" + keyPress + ">,");
			if (overDownToIdle)
				b.Append("overDownToIdle,");
			if (idleToOverDown)
				b.Append("idleToOverDown,");
			if (outDownToIdle)
				b.Append("outDownToIdle,");
			if (outDownToOverDown)
				b.Append("outDownToOverDown,");
			if (overDownToOutDown)
				b.Append("overDownToOutDown,");
			if (overDownToOverUp)
				b.Append("overDownToOverUp,");
			if (overUpToOverDown)
				b.Append("overUpToOverDown,");
			if (overUpToIdle)
				b.Append("overUpToIdle,");
			if (idleToOverUp)
				b.Append("idleToOverUp,");
			
			// trim trailing comma
			if (b.Length > 0)
				b.Length -= 1;
			
			return b.ToString();
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
