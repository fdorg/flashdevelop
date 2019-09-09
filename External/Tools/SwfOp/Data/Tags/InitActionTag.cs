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
    /// InitAction tag object
    /// </summary>
    public class InitActionTag : BaseTag {      
        
        /// <summary>sprite id</summary>
        private ushort spriteId;
        
        /// <summary>bytecode block</summary>
        private byte[] actionRecord;
        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="spriteId">see swf docs</param>
        /// <param name="actionRecord">byte code block</param>
        public InitActionTag(ushort spriteId,byte[] actionRecord) {
            
            this.spriteId = spriteId;
            this.actionRecord = actionRecord;   
            
            _tagCode = (int)TagCodeEnum.InitAction;
        }
        
        /// <summary>
        /// see <see cref="SwfOp.Data.Tags.BaseTag">base class</see>
        /// </summary>
        public override int ActionRecCount {
            get {
                return 1;
            }
        }
        
        /// <summary>
        /// see <see cref="SwfOp.Data.Tags.BaseTag">base class</see>
        /// </summary>
        public override byte[] this[int index] {
            get {
                return actionRecord;
            }
            set {
                actionRecord = value;
            }
        }
        
        /// <summary>
        /// see <see cref="SwfOp.Data.Tags.BaseTag">base class</see>
        /// </summary>
        public override void UpdateData(byte version) {
        
            MemoryStream m = new MemoryStream();
            BinaryWriter w = new BinaryWriter(m);
            
            RecordHeader rh = new RecordHeader(TagCode, 2 + actionRecord.Length ,true);
            
            rh.WriteTo(w);
            w.Write(spriteId);
            w.Write(actionRecord);
            
            // write to data array
            _data = m.ToArray();
        }
    }
}
