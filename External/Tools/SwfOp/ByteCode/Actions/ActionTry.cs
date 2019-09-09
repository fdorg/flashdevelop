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
using System.Collections;
using System;
using SwfOp.Utils;

namespace SwfOp.ByteCode.Actions
{       
    /// <summary>
    /// bytecode instruction object try/catch/finally block
    /// </summary>

    public class ActionTry : MultiByteAction {
                
        private bool catchesInReg;
        private bool hasFinally,hasCatch;
        private ushort trySize,catchSize,finallySize;
        private string catchVar;
        private byte catchReg;
        
        /// <summary>
        /// byte size of try block
        /// </summary>
        public int SizeTry {
            get {
                return Convert.ToInt32(trySize);
            }
            set {
                trySize = Convert.ToUInt16(value);
            }
        }
        
        /// <summary>
        /// byte size of catch block
        /// </summary>
        public int SizeCatch {
            get {
                return Convert.ToInt32(catchSize);
            }
            set {
                catchSize = Convert.ToUInt16(value);
                hasCatch = (catchSize>(ushort)0);
            }
        }
        
        /// <summary>
        /// byte size of finally block
        /// </summary>
        public int SizeFinally {
            get {
                return Convert.ToInt32(finallySize);
            }
            set {
                finallySize = Convert.ToUInt16(value);
                hasFinally = (finallySize>(ushort)0);
            }
        }
        
        /// <summary>
        /// constructor
        /// </summary>
        public ActionTry(   bool catchRegFlag,
                            bool finallyFlag,
                            bool catchFlag,
                            ushort trySize,
                            ushort catchSize,
                            ushort finallySize,
                            string catchName,
                            byte catchRegister                          
        ) : base(ActionCode.Try)
        {
            hasFinally = finallyFlag;
            hasCatch = catchFlag;
            catchesInReg = catchRegFlag;
            
            SizeTry = trySize;
            SizeCatch = hasCatch ? catchSize : (ushort)0;
            SizeFinally = hasFinally ? finallySize : (ushort)0;
            
            catchVar = catchName;
            catchReg = catchRegister;
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.ByteCount"/>
        public override int ByteCount {
            get {
                return 10+(catchesInReg ? 1 : catchVar.Length+1);
            }
        }
        
        /// <summary>overriden ToString method</summary>
        public override string ToString() {
            string catchStr = hasCatch ? 
                                String.Format("catch({0}) [{1}]",(catchesInReg ? ("r:"+catchReg.ToString()):"'"+catchVar+"'"),catchSize) 
                                : "";
            string finallyStr = hasFinally ? String.Format("finally [{0}]",finallySize) : "";
            return String.Format("TryBlock: try[{0}] {1} {2}",trySize,catchStr,finallyStr);
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.Compile"/>
        public override void Compile(BinaryWriter w) 
{
            
            base.Compile(w);
            
            byte flags = (byte)0;
            if (catchesInReg) flags+=(byte) 0x04;
            if (hasFinally) flags+=(byte) 0x02;
            if (hasCatch) flags+=(byte) 0x01;
            
            w.Write(flags);
            w.Write(trySize);
            w.Write(catchSize);
            w.Write(finallySize);
            
            if (catchesInReg) { 
                w.Write(catchReg);
            } else {
                BinaryStringRW.WriteString(w,catchVar);
            }
        }
    }       
}
