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
    /// PlaceObject2 tag object
    /// </summary>
    public class PlaceObject2Tag : BaseTag {    
        
            private byte[] header;
            private ClipActionRec[] clipActions;            
        
            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="h">header data</param>
            /// <param name="clp">swf action blocks (clip actions)</param>
            public PlaceObject2Tag(byte[] h,ClipActionRec[] clp) {
                
                    header = h;
                    clipActions = clp;
                
                    _tagCode = (int)TagCodeEnum.PlaceObject2;
            }
            
            /// <summary>
            /// see <see cref="SwfOp.Data.Tags.BaseTag">base class</see>
            /// </summary>
            public override int ActionRecCount {
                get {
                    return clipActions.Length;
                }
            }
            
            /// <summary>
            /// see <see cref="SwfOp.Data.Tags.BaseTag">base class</see>
            /// </summary>
            public override byte[] this[int index] {
                get {
                    return clipActions[index].ActionRecord;
                }
                set {
                    clipActions[index].ActionRecord = value;
                }
            }
            
            /// <summary>
            /// see <see cref="SwfOp.Data.Tags.BaseTag">base class</see>
            /// </summary>
            public override void UpdateData(byte version) {
            
                MemoryStream m = new MemoryStream();
                BinaryWriter w = new BinaryWriter(m);
                
                w.Write(header);
                
                // ClipActionRecords
                foreach (ClipActionRec clpA in clipActions) {
                    w.Write(clpA.GetData(version));
                }
                // ClipActionRecords end
                if (version>=6) {
                    w.Write((int)0);
                } else {
                    w.Write((short)0);              
                }
                
                // data to byte array
                byte[] d = m.ToArray();
                
                // writer
                m = new MemoryStream();
                w = new BinaryWriter(m);                
            
                // add header
                RecordHeader rh = new RecordHeader(TagCode,d.Length,true);
                rh.WriteTo( w );
                w.Write (d );
                
                // write to data array
                _data = m.ToArray();
            }
    }
}
