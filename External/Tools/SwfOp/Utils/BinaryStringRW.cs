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
using System.Text;
using System;

namespace SwfOp.Utils
{
    /// <summary>
    /// A helper class for reading/writing zero-byte terminated strings
    /// </summary>
    public class BinaryStringRW {

        /// <summary>   
        /// Read zero-byte terminated string with a <see cref="System.IO.BinaryReader">BinaryReader</see>
        /// </summary>
        public static string ReadString(BinaryReader br) {
            
            StringBuilder sb = new StringBuilder("");
            byte b = br.ReadByte();
            while (b>0){                
                sb.Append(Convert.ToChar(b));
                b = br.ReadByte();
            };
            return sb.ToString();
        }   

        /// <summary>   
        /// Write zero-byte terminated string with a <see cref="System.IO.BinaryWriter">BinaryWriter</see>
        /// </summary>
        public static void WriteString(BinaryWriter w,string str) {
            
            char[] ch = new char[str.Length];
            str.CopyTo(0,ch,0,str.Length);
            w.Write(ch);
            w.Write((byte)0);
        }

        /// <remarks>
        /// Hidden constructor, class is only used in static context.
        /// </remarks>
        private BinaryStringRW() {}
    }
}
