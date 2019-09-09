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
using System.Text;
using SwfOp.Utils;

namespace SwfOp.ByteCode.Actions
{
    /// <summary>
    /// bytecode instruction object
    /// </summary>

    public class ActionDefineFunction2 : MultiByteAction {
        
        /// <summary>
        /// register/name pair of function parameter
        /// </summary>
        public struct RegParamPair {
        
            internal int Register;
            internal string Parameter;
            
            /// <summary>
            /// constructor
            /// </summary>
            public RegParamPair(int r,string p) {
                Register = r;
                Parameter = p;
            }
            
            /// <summary>
            /// overriden ToString method, called by outer class
            /// </summary>
            public override string ToString() {
                return String.Format("r:{0}='{1}'",Register,Parameter);
            }
        } 
        
        /// <summary>
        /// set of flags for automatically storing variables in registers 
        /// </summary>
        public struct VariableFlagSet {
            
            /// <summary>preload '_parent' variable</summary>
            public bool PreloadParent;
            /// <summary>preload '_root' variable</summary>
            public bool PreloadRoot;
            /// <summary>suppress '_root' variable</summary>
            public bool SuppressSuper;
            /// <summary>preload 'super' variable</summary>
            public bool PreloadSuper;
            /// <summary>suppress 'super' variable</summary>
            public bool SuppressArguments;
            /// <summary>preload 'arguments' variable</summary>
            public bool PreloadArguments;
            /// <summary>suppress 'arguments' variable</summary>
            public bool SuppressThis;
            /// <summary>preload 'this' variable</summary>
            public bool PreloadThis;
            /// <summary>preload '_global' variable</summary>
            public bool PreloadGlobal;
            
            /// <summary>constructor</summary>
            public VariableFlagSet(bool loadParent,
                                   bool loadRoot,
                                   bool dontSuper,
                                   bool loadSuper,
                                   bool dontArgs,
                                   bool loadArgs,
                                   bool dontThis,
                                   bool loadThis,
                                   bool loadGlobal)
                               
            {               
                PreloadParent = loadParent;
                PreloadRoot = loadRoot;
                SuppressSuper =dontSuper;
                PreloadSuper = loadSuper;
                SuppressArguments = dontArgs;
                PreloadArguments = loadArgs;
                SuppressThis = dontThis;
                PreloadThis = loadThis;
                PreloadGlobal = loadGlobal;             
            }
            
            /// <summary>
            /// count of positive flags
            /// </summary>
            internal int Count {
                get {
                    int i=0;
                    
                    if (PreloadParent) i++;
                    if (PreloadRoot) i++;
                    if (SuppressSuper) i++;
                    if (PreloadSuper) i++;
                    if (SuppressArguments) i++;
                    if (PreloadArguments) i++;
                    if (SuppressThis) i++;
                    if (PreloadThis) i++;
                    if (PreloadGlobal) i++;
                    
                    return i;
                }
            }
            
            /// <summary>
            /// check wether flags exclude each other
            /// </summary>
            internal void CheckExclusion() {
                
                if (PreloadSuper) {
                    SuppressSuper = false;
                }
                if (PreloadArguments) {
                    SuppressArguments = false;
                }
                if (PreloadThis) {
                    SuppressThis = false;
                }
            }
            
            /// <summary>
            /// bytecode for flag set
            /// </summary>
            internal byte[] Bytecode {
                
                get {               
                    byte b0 = 0;
                    byte b1 = 0;
                    
                    CheckExclusion();
                    
                    if (PreloadParent) b0+=0x80;
                    if (PreloadRoot) b0+=0x40;
                    if (SuppressSuper) b0+=0x20;
                    if (PreloadSuper) b0+=0x10;
                    if (SuppressArguments) b0+=0x08;
                    if (PreloadArguments) b0+=0x04;
                    if (SuppressThis) b0+=0x02;
                    if (PreloadThis) b0+=0x01;
                    if (PreloadGlobal) b1+=0x01;
                        
                    return new byte[2] { b0,b1 };
                }
            }           
            
            /// <summary>
            /// ToString method, called by outer class´ ToString
            /// </summary>
            public override string ToString() {
                
                int reg = 1;
                StringBuilder stb = new StringBuilder();
                
                if (PreloadThis) {
                    stb.AppendFormat("r:{0}='this'",reg);
                    reg++;
                }
                if (PreloadArguments) {
                    stb.AppendFormat("{0}r:{1}='arguments'",(reg>1) ? "," : "" ,reg);
                    reg++;
                }
                if (PreloadSuper) {
                    stb.AppendFormat("{0}r:{1}='super'",(reg>1) ? "," : "" ,reg);
                    reg++;
                }
                if (PreloadRoot) {
                    stb.AppendFormat("{0}r:{1}='_root'",(reg>1) ? "," : "" ,reg);
                    reg++;
                }
                if (PreloadParent) {
                    stb.AppendFormat("{0}r:{1}='_parent'",(reg>1) ? "," : "" ,reg);
                    reg++;
                }
                if (PreloadGlobal) {
                    stb.AppendFormat("{0}r:{1}='_global'",(reg>1) ? "," : "" ,reg);
                    reg++;
                }
                return stb.ToString();
            }
        }
        
        /// <summary>
        /// function name
        /// </summary>
        public string Name;
        
        /// <summary>
        /// <see cref="SwfOp.ByteCode.Actions.ActionDefineFunction2.RegParamPair">RegParamPair</see> of expected parameters 
        /// </summary>
        public RegParamPair[] ParameterList;
        
        /// <summary>
        /// registers allocated by function
        /// </summary>
        public int RegisterCount;
        
        /// <summary>
        /// inner actions (function body)
        /// </summary>
        public ArrayList ActionRecord;
        
        private VariableFlagSet flags;
        
        /// <summary>
        /// automatically allocated registers flags (this, _global etc.)
        /// </summary>
        public VariableFlagSet Flags {
            get {
                return flags;
            }
        }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="n">function name</param>
        /// <param name="p">function parameters</param>
        /// <param name="r">register count</param>
        /// <param name="f">flags for automatic register allocation</param>
        /// <param name="actionRec">inner action block (body)</param>
        public ActionDefineFunction2(string n,
                                     RegParamPair[] p,
                                     int r,
                                     VariableFlagSet f,
                                     ArrayList actionRec) 
        : base(ActionCode.DefineFunction2)
        {
            Name = n;
            ParameterList = p;
            RegisterCount = r;
            flags = f;
            ActionRecord = actionRec;
        }
        
        /// <summary>
        /// summarized byte count of inner action block
        /// </summary>
        protected int InnerByteCount {
            get {               
                int count = 0;
                for (int i=0; i<ActionRecord.Count; i++) {
                    BaseAction a = (BaseAction) ActionRecord[i];
                    count += a.ByteCount;
                }
                return count;
            }
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.ByteCount"/>
        public override int ByteCount {
            get {               
                int count = 3+Name.Length+1+2+1+2+2+InnerByteCount;         
                for (int i=0; i<ParameterList.Length; i++) {
                    RegParamPair r = ParameterList[i];
                    count+=r.Parameter.Length+2;                    
                }

                return count;
            }
        }
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.Compile"/>
        public override void Compile(BinaryWriter w) 
{
            
            w.Write( Convert.ToByte(Code));
            w.Write(Convert.ToUInt16(ByteCount-InnerByteCount-3));
            BinaryStringRW.WriteString(w,Name);
            
            w.Write(Convert.ToUInt16(ParameterList.Length));
            w.Write(Convert.ToByte(RegisterCount));
            w.Write(flags.Bytecode);
            
            foreach (RegParamPair rp in ParameterList) {
                w.Write(Convert.ToByte(rp.Register));
                BinaryStringRW.WriteString(w,rp.Parameter);             
            }
            
            w.Write(Convert.ToUInt16(InnerByteCount));
            
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
            
            string[] s = new string[ParameterList.Length];
            
            for (int i=0; i<this.ParameterList.Length; i++) {
                RegParamPair p = this.ParameterList[i];
                s[i] = p.ToString();
            }
            
            string[] s2 = new string[ActionRecord.Count];
            for (int i=0; i<ActionRecord.Count; i++) {
                //s2[i] = String.Format("{0}",ActionRecord[i].ToString());
                BaseAction a = (BaseAction)ActionRecord[i];
                s2[i] = a.ToString();
            }
            
            return String.Format("\n/*function2 '{0}' ({2})({1})\n{3}\nend function {4}*/\n",
                                 Name,
                                 String.Join(",",s),
                                 flags.ToString(),
                                 String.Join("\n",s2),
                                 Name
                                 );
        }
    }
}
