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
using SwfOp.Data;
using SwfOp.Data.Tags;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace SwfOp.IO {

    /// <summary>
    /// SwfWriter writes an swf to a stream using the Write method. 
    /// </summary>
    public class SwfWriter {
        
        /// <summary>
        /// BaseStream where swf gets written to using Write(..)
        /// </summary>
        private Stream baseStream;
        
        /// <summary>
        /// Writes the (compressed or uncompressed) swf data to a stream.
        /// The stream gets flushed and closed afterwards.
        /// </summary>
        public void Write(Swf swf) {
            
            swf.UpdateData();   // update tag lengths to adapt to bytecode length   
            SwfHeader header = swf.Header;

            BinaryWriter writer = new BinaryWriter(baseStream,System.Text.Encoding.GetEncoding("ascii"));   // ASCII seems to be ok for Flash 5 and 6+ as well  
            BinaryWriter dataWriter = writer;

            bool isCompressed = (header.Signature[0]=='C');
    
            if (isCompressed) {
                // SharpZipLib makes it easy for us, simply chain a Deflater into the stream
                DeflaterOutputStream def = new DeflaterOutputStream(baseStream);
                dataWriter = new BinaryWriter(def); 
            }           
            
            // writer header data, always uncompressed
            writer.Write(header.Signature);         
            writer.Write(swf.Version);  
            writer.Write(swf.ByteCount);
            writer.Flush();
            
            // write header data pt.2, using either original stream or deflater stream
            dataWriter.Write(header.Rect);
            dataWriter.Write(header.Fps);
            dataWriter.Write(header.Frames);            
            
            // write tag data
            foreach (BaseTag tag in swf) {
                dataWriter.Write(tag.Data);
            }
            
            // flush + close
            dataWriter.Flush();
            dataWriter.Close(); 
        }
        
        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name ="stream">Stream, the swf shall be written to.</param>
        public SwfWriter(Stream stream) {   
            baseStream = stream;
        }
    }
}
