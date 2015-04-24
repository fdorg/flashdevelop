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
    /// bytecode instruction object
    /// </summary>

    public class ActionConstantPool : MultiByteAction {
        
        /// <summary>
        /// list of constant pool strings by index 
        /// </summary>
        public ArrayList ConstantList;
        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="c">constant list</param>
        public ActionConstantPool(string[] c):base(ActionCode.ConstantPool)
        {
            ConstantList = new ArrayList(c);
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.ByteCount"/>
        public override int ByteCount {
            get {
                int count = 5;
                for (int i=0; i<ConstantList.Count; i++) {
                    string s = (string) ConstantList[i];
                    count+=s.Length+1;
                }
                return count;
            }
        }
        
        /// <summary>
        /// overriden ToString() method
        /// </summary>/// <summary>overriden ToString method</summary>
        public override string ToString() {
            string[] clist = new string[ConstantList.Count];
            for (int i=0; i<clist.Length; i++) {
                clist[i] = String.Format("'{0}'",ConstantList[i]);
            }
            return String.Format("constantPool {0}",String.Join(",",clist));
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.Compile"/>/// <see cref="SwfOp.ByteCode.Actions.BaseAction.Compile"/>
        public override void Compile(BinaryWriter w) 
{
            
            base.Compile(w);            
            w.Write(Convert.ToUInt16(ConstantList.Count));
            
            foreach(object c in ConstantList) {
                string stringToWrite = (string) c;
                BinaryStringRW.WriteString(w,stringToWrite);
            }
        }
    }
}
