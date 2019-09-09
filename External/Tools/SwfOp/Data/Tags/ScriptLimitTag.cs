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
using System.Collections;
using SwfOp.Data.Tags;

namespace SwfOp.Data.Tags {

    /// <summary>
    /// ScriptLimit tag
    /// </summary>
    public class ScriptLimitTag : BaseTag {             
        
        private ushort _recursion;
        private ushort _timeout;
        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="recursion">recursion depth</param>
        /// <param name="timeout">specified timeout</param>
        public ScriptLimitTag(ushort recursion,ushort timeout) {
            
            _recursion = recursion;
            _timeout = timeout;
            
            _tagCode = (int)TagCodeEnum.ScriptLimit;
        }
        
        /// <summary>
        /// recursion property
        /// </summary>
        public int Recursion {
            get {
                return Convert.ToInt32(_recursion);
            }
            set {
                _recursion = Convert.ToUInt16(value);
            }
        }
        
        /// <summary>
        /// timeout property
        /// </summary>
        public int TimeOut {
            get {
                return Convert.ToInt32(_timeout);
            }
            set {
                _timeout = Convert.ToUInt16(value);
            }
        }
        
        /// <summary>
        /// see <see cref="SwfOp.Data.Tags.BaseTag">base class</see>
        /// </summary>
        public override int ActionRecCount {
            get {
                return 0;
            }
        }
        
        /// <summary>
        /// see <see cref="SwfOp.Data.Tags.BaseTag">base class</see>
        /// </summary>
        public override void UpdateData(byte version) {
            
            MemoryStream m = new MemoryStream();
            BinaryWriter w = new BinaryWriter(m);
            
            RecordHeader rh = new RecordHeader(TagCode,4,false);
            
            rh.WriteTo(w);
            w.Write(_recursion);
            w.Write(_timeout);
            
            // write to data array
            _data = m.ToArray();
        }
        
    }
}
