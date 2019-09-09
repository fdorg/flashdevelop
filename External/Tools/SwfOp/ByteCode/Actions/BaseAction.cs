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

using System;
using System.IO;
using SwfOp.ByteCode;

namespace SwfOp.ByteCode.Actions
{
    /// <summary>
    /// BaseAction is an abstract class that serves as a base for  all Action 
    /// classes resembling swf bytecode instructions
    /// </summary>
    
    public abstract class BaseAction {
        
        /// <summary>
        /// Action code of the class when compiled to swf
        /// Action codes are enumerated in <see cref="SwfOp.ByteCode.ActionCode"/>
        /// </summary>
        public readonly int Code;
        
        // no longer needed
        //public int ByteSize;
            
        /// <summary>
        /// public constructor
        /// </summary>
        /// <param name="c">action code as occuring in swf. Codes are listed in this <see cref="SwfOp.ByteCode.ActionCode">enumeration</see></param>
        public BaseAction(ActionCode c) {
            Code = (int) c;
            //ByteSize = 1;
        }
        
        /// <summary>
        /// byte count, calculated during compilation for <seealso cref="SwfOp.ByteCode.Actions.MultiByteAction">multibyte actions</seealso>
        /// </summary>
        public virtual int ByteCount {
            get {
                return 1;
            }
        }
        
        /// <summary>
        /// values pushed on stack by the operation associated with this action
        /// during swf runtime
        /// </summary>
        public virtual int PushCount {
            get {
                return 0;
            }
            set {
                
            }
        }
        
        /// <summary>
        /// values popped from stack by the operation associated with this action
        /// during swf runtime
        /// </summary>
        public virtual int PopCount {
            get {
                return 0;
            }
            set {
                
            }
        }
        
        /// <summary>
        /// compile action to byte code
        /// </summary>
        /// <param name="writer">
        /// Binary writer for writing byte code to stream
        /// </param>
        public virtual void Compile(BinaryWriter writer) {      
            writer.Write(Convert.ToByte(Code));
        }

    }
}

