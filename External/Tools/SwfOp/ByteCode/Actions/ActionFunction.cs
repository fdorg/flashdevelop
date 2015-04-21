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

    public class ActionDefineFunction : MultiByteAction {
        
        /// <summary>
        /// function name
        /// </summary>
        public string Name;
        
        /// <summary>
        /// list of function parameters
        /// </summary>
        public string[] ParameterList;
        
        /// <summary>
        /// inner actions (function body)
        /// </summary>
        public ArrayList ActionRecord; // inner actions, function body
        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="n">function name</param>
        /// <param name="parmList">funtion parameters</param>
        /// <param name="actionRec">inner action block (function body)</param>
        public ActionDefineFunction(string n,string[] parmList,ArrayList actionRec) : 
                base(ActionCode.DefineFunction)
        {
            Name = n;
            ParameterList = parmList;
            ActionRecord = actionRec;
        }
        
        private int innerByteCount {
            get {               
                int count = 0;
                for (int i=0; i<ActionRecord.Count; i++) {
                    BaseAction a = (BaseAction) ActionRecord[i];
                    count+=a.ByteCount;
                }
                return count;
            }
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.ByteCount"/>
        public override int ByteCount {
            get {               
                int count = 3+Name.Length+1+2+2;
                for (int i=0; i<ParameterList.Length; i++) {
                    count+=ParameterList[i].Length+1;                   
                }
                count+=innerByteCount;
                return count;
            }
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.Compile"/>
        public override void Compile(BinaryWriter w) 
{
            
            w.Write( Convert.ToByte(Code));
            w.Write(Convert.ToUInt16(ByteCount-innerByteCount-3));
            BinaryStringRW.WriteString(w,Name);
            
            w.Write(Convert.ToUInt16(ParameterList.Length));            
            foreach(string str in ParameterList) {
                BinaryStringRW.WriteString(w,str);
            }
            
            w.Write(Convert.ToUInt16(innerByteCount));
            foreach (object a in ActionRecord) {
                BaseAction action = (BaseAction) a;
                action.Compile(w);
            }
        }       
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.PopCount"/>
        public override int PopCount {  get { return 0; } }     
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.PushCount"/>
        public override int PushCount { get { return 1; } }
        
        /// <summary>overriden ToString method</summary>
        public override string ToString() {
            
            string[] s = new string[ActionRecord.Count];
            for (int i=0; i<ActionRecord.Count; i++) {
                BaseAction a = (BaseAction)ActionRecord[i];
                s[i] = a.ToString();            
            }
            
            return String.Format("function '{0}' ({1})\n{2}\nend function {3}"
                                 ,Name
                                 ,String.Join(",",ParameterList)
                                 ,String.Join("\n",s)
                                 ,Name
                                 );
        }
    }
}
