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

namespace SwfOp.Data {  

    /// <summary>
    /// Swf header structur.
    /// </summary>
    public struct SwfHeader {
        
        /// <summary>Signature property ('FWS' or 'CWS').</summary>
        public char[] Signature;
        
        /// <summary>Version.</summary>
        public byte Version;
        
        /// <summary>FileSize.</summary>
        public uint FileSize;
        
        /// <summary>Bytecode of rect data containing swf dimensions.</summary>
        public byte[] Rect;         
        
        /// <summary>Frames per second.</summary>
        public ushort Fps;
        
        /// <summary>Total number of frames.</summary>
        public ushort Frames;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public SwfHeader (char[] sig,byte v,uint fs,byte[] ms,ushort fps,ushort fr) {
            
            Signature = sig;
            Version = v;
            FileSize = fs;
            Rect = ms;  
            Fps = fps;
            Frames = fr;            
        }
    }
}
