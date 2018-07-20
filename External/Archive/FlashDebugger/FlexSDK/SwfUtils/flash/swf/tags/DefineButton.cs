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
using TagHandler = Flash.Swf.TagHandler;
using ButtonCondAction = Flash.Swf.Types.ButtonCondAction;
using ButtonRecord = Flash.Swf.Types.ButtonRecord;
using ArrayUtil = Flash.Util.ArrayUtil;
namespace Flash.Swf.Tags
{
	
	/// <author>  Clement Wong
	/// </author>
	public class DefineButton:DefineTag
	{
		private class DefineButtonIterator : System.Collections.IEnumerator
		{
            public DefineButtonIterator(DefineButton defineButton)
			{
                m_DefineButton = defineButton;
			}
            private DefineButton m_DefineButton;
			public virtual System.Object Current
			{
				get
				{
                    if (0 <= record && record <= m_DefineButton.buttonRecords.Length && m_DefineButton.buttonRecords[record].characterRef != null)
                        return m_DefineButton.buttonRecords[record].characterRef;
                    throw new System.InvalidOperationException();
				}
				
			}
			private int record = -1;
			
			public virtual bool MoveNext()
			{
                if (record >= m_DefineButton.buttonRecords.Length)
                {
                    throw new InvalidOperationException();
                }

				// skip null entries
                while (record < m_DefineButton.buttonRecords.Length && m_DefineButton.buttonRecords[record].characterRef == null)
                {
					record++;
                }

                return record < m_DefineButton.buttonRecords.Length;
			}
			public virtual void  remove()
			{
				throw new System.NotSupportedException();
			}
			virtual public void  Reset()
			{
			}
		}
		override public System.Collections.IEnumerator References
		{
			get
			{
                return new DefineButtonIterator(this);
			}
			
		}
		public DefineButton(int code):base(code)
		{
		}
		
		public override void  visit(TagHandler h)
		{
			if (code == Flash.Swf.TagValues.stagDefineButton)
				h.defineButton(this);
			else
				h.defineButton2(this);
		}
		
		public ButtonRecord[] buttonRecords;
		public DefineButtonSound sounds;
		public DefineButtonCxform cxform;
		public DefineScalingGrid scalingGrid;
		
		/// <summary> false = track as normal button
		/// true = track as menu button
		/// </summary>
		public bool trackAsMenu;
		
		/// <summary> actions to execute at particular button events.  For defineButton
		/// this will only have one entry.  For defineButton2 it could have more
		/// than one entry for different conditions.
		/// </summary>
		public ButtonCondAction[] condActions;
		
		public  override bool Equals(System.Object obj)
		{
			bool isEqual = false;

            if (base.Equals(obj) && (obj is DefineButton))
			{
                DefineButton defineButton = (DefineButton)obj;
				
				if (ArrayUtil.equals(defineButton.buttonRecords, this.buttonRecords) && equals(defineButton.sounds, this.sounds) && equals(defineButton.cxform, this.cxform) && (defineButton.trackAsMenu == this.trackAsMenu) && ArrayUtil.equals(defineButton.condActions, this.condActions))
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
