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
using System.Collections.Specialized;

namespace SwfOp.Utils {
    
    /// <summary>
    /// A helper class with static methods for bit-parsing swf data
    /// </summary>
    public class BitParser {
        
        /// <remarks>
        /// List of int values for each bit; for fast access to <see cref="System.Collections.Specialized.BitVector32">BitVector32</see> sections
        /// </remarks>      
        private static int[] bitValueList = new int[32];
        
        /// <remarks>
        /// Static constructor: initialize bitValueList
        /// </remarks>
        static BitParser() {
            
            bitValueList[0] = BitVector32.CreateMask();
            
            for (int i=1; i<32; i++) {
                bitValueList[i] = BitVector32.CreateMask(bitValueList[i-1]);
            }
        }   
        
        /// <summary>
        /// Reverse bit order in each byte (8 bits) of a BitArray
        /// (change endian bit order)
        /// </summary>      
        public static void BytewiseReverse(BitArray bitArr) {       
            int byteCount = bitArr.Length/8;        
            for (int i=0; i<byteCount; i++) {
                for (int j=0; j<4; j++) {
                    bool temp = bitArr[i*8+7-j];
                    bitArr[i*8+7-j] = bitArr[i*8+j];
                    bitArr[i*8+j] = temp;
                }
            }
        }
        
        /// <summary>
        /// Prepare read bytes for bit parsing 
        /// </summary> 
        /// <param name="byteSequence">
        /// Byte sequence read from swf by a<see cref="System.IO.BinaryReader.ReadBytes">BinaryReader</see>
        /// </param>
        public static BitArray GetBitValues(byte[] byteSequence) {
            BitArray ba = new BitArray(byteSequence);
            BytewiseReverse(ba);
            return ba;
        }
        
        /// <summary>
        /// Overloaded static method<para/>
        /// Converts part of a <see cref="System.Collections.BitArray">BitArray</see> to a signed int32
        /// </summary>
        /// <param name="bitArr">source BitArray</param>
        /// <param name="index">start index</param>
        /// <param name="length">bit count</param>
        public static int ReadInt32(BitArray bitArr,int index,int length) {
            
            BitVector32 bitVec = new BitVector32(0);
            for (int i=0; i<length; i++) {
                bitVec[bitValueList[i]]  = bitArr[index+length-i-1];
            }
            bool sign = bitArr[index];
            for (int i=length; i<32; i++) {
                bitVec[bitValueList[i]] = sign;
            }
            return bitVec.Data;
        }
        
        /// <summary>
        /// starts at index 0
        /// </summary>
        /// <param name="bitArr">source BitArray</param>
        /// <param name="length">bit count</param>
        public static int ReadInt32(BitArray bitArr,int length) {
            return ReadInt32(bitArr,0,length);
        }

        /// <summary>
        /// convert total BitArray
        /// </summary>
        /// <param name="bitArr">source BitArray</param>
        public static int ReadInt32(BitArray bitArr) {      
            return ReadInt32(bitArr,0,bitArr.Length);
        }
        
        /// <summary>
        /// convert part of a <see cref="System.Collections.BitArray">BitArray</see> to a unsigned integer (uint32)
        /// </summary>
        /// <param name="bitArr">source BitArray</param>
        /// <param name="index">start index</param>
        /// <param name="length">bit count</param>
        public static int ReadUInt32(BitArray bitArr,int index,int length) {
            
            if (length>31) throw new ArgumentOutOfRangeException();
            
            BitVector32 bitVec = new BitVector32(0);            
            for (int i=0; i<length; i++) {
                bitVec[bitValueList[i]]  = bitArr[index+length-i-1];
            }
            return bitVec.Data;
        }
        /// <summary>
        /// start at index 0
        /// </summary>
        /// <param name="bitArr">source BitArray</param>
        /// <param name="length">bit count</param>
        public static int ReadUInt32(BitArray bitArr,int length) {
            return ReadUInt32(bitArr,0,length);
        }

        /// <summary>
        /// convert total BitArray
        /// </summary>
        /// <param name="bitArr">source BitArray</param>
        public static int ReadUInt32(BitArray bitArr) {     
            return ReadUInt32(bitArr,0,bitArr.Length);
        }       
        
        /// <remarks>
        /// no public constructor, class is only used in static context
        /// </remarks>
        private BitParser() {}
    }
    
}
