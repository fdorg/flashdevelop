/*
    swfOP is an open source library for manipulation and examination of
    Macromedia Flash (SWF) ActionScript bytecode.
    Copyright (C) 2004 Florian Krüsch.
    see Licence.cs for LGPL full text!
    
    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.
    
    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.
    
    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System.IO;
using System;

using SwfOp.ByteCode;
using SwfOp.ByteCode.Actions;
using SwfOp.Utils;

namespace SwfOp.ByteCode.Actions
{   
    /// <summary>
    /// bytecode instruction object
    /// </summary>

    public class ActionPushList : MultiByteAction {
        
        private ActionPush[] pushList;
        
        /// <summary>
        /// length of push list
        /// </summary>
        public int Length {
            get {
                return pushList.Length;
            }
        }
        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="p">list of single push instructions</param>
        public ActionPushList(ActionPush[] p):base(ActionCode.PushList)
        {
            pushList = p;
            
        }
        
        /// <summary>
        /// indexer to access single push actions
        /// </summary>
        public ActionPush this[int i] {
            get {
                return pushList[i];
            }
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.ByteCount"/>
        public override int ByteCount {
            get {
                int count = 3;
                foreach (ActionPush p in pushList) {
                    count+=p.ByteCount-3;
                }
                return count;
            }
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.PopCount"/>
        public override int PopCount {
            get {
                return 0;
            }
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.PushCount"/>
        public override int PushCount {
            get {
                return pushList.Length;
            }
        }
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.Compile"/>
        public override void Compile(BinaryWriter w) 
{
            
            // base.Compile(w); 
            w.Write((byte)0x96);
            w.Write(Convert.ToUInt16(ByteCount-3));
            
            foreach (ActionPush push in pushList) {
                push.CompileBody(w);
            }
        }
        
        /// <summary>overriden ToString method</summary>
        public override string ToString() {
            string[] s = new string[pushList.Length];
            for (int i=0; i<pushList.Length; i++) {
                s[i] = pushList[i].ToString().Substring(5);
            }
            return String.Format("push {0}",String.Join(", ",s));
        }
    }
}

