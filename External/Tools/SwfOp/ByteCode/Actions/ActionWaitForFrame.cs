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

namespace SwfOp.ByteCode.Actions
{   
    
    /// <summary>
    /// bytecode instruction object
    /// </summary>

    public class ActionWaitForFrame : MultiByteAction {
        
        private short waitFrame;
        private byte skipCount;
        
        /// <summary>
        /// count of byte to skip
        /// </summary>
        public int SkipCount {
            get {
                return Convert.ToInt32(skipCount);
            }
            set {
                skipCount = Convert.ToByte(value);
            }
        }
        
        /// <summary>
        /// constructor
        /// </summary>
        public ActionWaitForFrame(short wait,byte skip):base(ActionCode.WaitForFrame)
        {
            waitFrame = wait;
            skipCount = skip;
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.ByteCount "/>
        public override int ByteCount {
            get {
                return 6;
            }
        }
        
        /// <summary>overriden ToString method</summary>
        public override string ToString() {
            return String.Format("WaitForFrame {0}",waitFrame.ToString());
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.Compile"/>
        public override void Compile(BinaryWriter w) 
{
            base.Compile(w);
            w.Write(waitFrame);
            w.Write(skipCount);
        }
    }
    
}

