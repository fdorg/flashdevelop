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

namespace SwfOp.Data {  
    
    /// <summary>
    /// RecordHeader class representing swf tag headers.
    /// </summary>
    public class RecordHeader { 
        
        private int tagCode;
        private int tagLength;
        private bool longTag;       
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public RecordHeader(int tag,int length,bool longT) {
            
            tagCode = tag;
            tagLength = length;
            longTag = longT;
        }
        
        /// <summary>
        /// Tag code property.
        /// </summary>
        public int TagCode {
            get {
                return tagCode;
            }
        }
        
        /// <summary>
        /// Tag length property.
        /// </summary>
        public int TagLength {
            get {
                return tagLength;
            }
        }
        
        /// <summary>
        /// Writes binary data to given BinaryWriter.
        /// </summary>
        public void WriteTo(BinaryWriter w) {           
            
            if ( longTag || (tagLength>0x3e) ) {
                
                byte[] b = BitConverter.GetBytes(
                    Convert.ToUInt16( (Convert.ToUInt16(tagCode) << 6) + 0x3F )
                );
                w.Write( b );
                
                UInt32 len = Convert.ToUInt32(tagLength);
                b = BitConverter.GetBytes(len);             
                w.Write( b );
                
            } else {
                byte[] b = BitConverter.GetBytes(
                    Convert.ToUInt16( (Convert.ToUInt16(tagCode) << 6) + Convert.ToUInt16(tagLength) )
                );
                w.Write( b );
            }                   
        }
    }
}
