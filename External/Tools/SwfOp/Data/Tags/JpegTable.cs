/*
    swfOP is an open source library for manipulation and examination of
    Macromedia Flash ActionScript bytecode.
    Copyright (C) 2004 Florian Krüsch
    see Licence.cs for LGPL full text 
    
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

namespace SwfOp.Data.Tags
{
    /// <summary>
    /// Description of JPEGTableTag.    
    /// </summary>
    public class JpegTableTag : BaseTag
    {       
        /// <summary>
        /// constructor.
        /// </summary>
        public JpegTableTag(byte[] jpeg)        
        {
            jpegData = jpeg;
        }
        
        private byte[] jpegData;
        
        /// <summary>
        /// JPEG Data
        /// </summary>
        public byte[] JpegData {
            get {
                return jpegData;
            }
            set {
                jpegData = value;
            }
        }
        
        /// <summary>
        /// see <see cref="SwfOp.Data.Tags.BaseTag">base class</see>
        /// </summary>
        public override void UpdateData(byte version) {
                
            MemoryStream m = new MemoryStream();
            BinaryWriter w = new BinaryWriter(m);
            
            RecordHeader rh = new RecordHeader(TagCode, jpegData.Length ,true);
            
            rh.WriteTo(w);
            w.Write(jpegData);
            
            // write to data array
            _data = m.ToArray();            
        }
    }
}
