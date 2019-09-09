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
using System.Collections;
using System.Reflection;
using System;

using SwfOp.ByteCode;
using SwfOp.ByteCode.Actions;
using SwfOp.Utils;

namespace SwfOp.ByteCode.Actions
{

    /// <summary>
    /// bytecode instruction object
    /// </summary>

    public class ActionPush : MultiByteAction {
        
        /// <summary>
        /// enumaration of push types
        /// </summary>
        public enum PushType {
            /// <summary>push type 0: string</summary>
            String = 0,
            /// <summary>push type 1: float</summary>
            Float = 1,
            /// <summary>push type 2: null</summary>
            Null = 2,
            /// <summary>push type 3: undef</summary>
            Undef = 3,
            /// <summary>push type 4: register</summary>
            Register = 4,
            /// <summary>push type 5: bool</summary>
            Boolean  = 5,
            /// <summary>push type 6: double</summary>
            Double = 6,
            /// <summary>push type 7: int</summary>
            Int = 7,
            /// <summary>push type 8: constant8</summary>
            Constant8 = 8,
            /// <summary>push type 9: constant9</summary>
            Constant16 = 9
        }
        
        private string[] PushTypeNames = new string[10] {
            "string ",
            "float ",
            "null ",
            "undef ",
            "register ",
            "bool ",
            "double ",
            "int ",
            "var ",
            "var "
        };
        
        /// <summary>
        /// push type
        /// </summary>
        public int Type;
        
        /// <summary>
        /// push value
        /// </summary>
        public object Value;
        
        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="type">push type</param>
        /// <param name="val">push value</param>
        public ActionPush(int type,object val):base(ActionCode.Push)
        {
            if ((type<(int)PushType.String)||(type>(int)PushType.Constant16)) {
                throw new InvalidPushTypeException();
            }
            Type = type;
            Value = val;
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.ByteCount "/>
        public override int ByteCount {
            
            get {
                
                switch ( (PushType) Type) {
                    
                        case PushType.String:   
                                string str = (string) Value;
                                return str.Length + 5;
                        
                        case PushType.Float:    
                                return 8;
                        
                        case PushType.Register: 
                                return 5;
                        
                        case PushType.Boolean:  
                                return 5;
                        
                        case PushType.Double:   
                                return 12;
                        
                        case PushType.Int:      
                                return 8;
                        
                        case PushType.Constant8: 
                                return 5;
                        
                        case PushType.Constant16: 
                                return 6;                       
                }
                // Null, Undef
                return 4;
            }
        }
        
        /// <summary>
        /// get push value as int
        /// </summary>
        /// <returns>pushed value as int</returns>
        public int GetIntValue() {
            
            switch ( (PushType) Type) {
                
                case PushType.Float: return Convert.ToInt32((Single)Value);
                    
                case PushType.Double: return Convert.ToInt32((double)Value);
                    
                case PushType.String: return Convert.ToInt32((string)Value);
                    
                case PushType.Int: return (int)Value;
                    
                default: //Console.WriteLine("WARNING"); 
                         return -1;
            }
        }
        
        /// <summary>
        /// get value as string
        /// </summary>
        /// <returns>pushed value as string</returns>
        public string GetStringValue() {
            return ((PushType)Type==PushType.String) ? (string)Value : null;
        }
        
        /// <summary>
        /// compile push type and value (but not action code), so method can
        /// be used by <see cref="SwfOp.ByteCode.Actions.ActionPushList">ActionPushList</see> as well
        /// </summary>
        public void CompileBody(BinaryWriter w) {           
            
            w.Write(Convert.ToByte(Type));
            
            switch ( (PushType) Type) {
                
                case PushType.String:   
                            string stringToWrite = (string) Value;
                            BinaryStringRW.WriteString(w,stringToWrite);
                            break;
                
                case PushType.Float: 
                            w.Write(Convert.ToSingle(Value));
                            break;
                
                case PushType.Register:
                            w.Write(Convert.ToByte(Value));
                            break;
                
                case PushType.Boolean: 
                            w.Write((bool)Value);
                            break;
                
                case PushType.Double:   
                            byte[] b = BitConverter.GetBytes((double)Value);
                            for (int i=0; i<4; i++) {
                                byte temp = b[i];
                                b[i] = b[4+i];
                                b[4+i] = temp;
                            }
                            w.Write(b);
                            break;
                
                case PushType.Int: 
                            w.Write((int)Value);
                            break;
                
                case PushType.Constant8: 
                            w.Write(Convert.ToByte(Value));
                            break;
                
                case PushType.Constant16: 
                            w.Write(Convert.ToUInt16(Value));
                            break;      
                
            }
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.PopCount"/>
        public override int PopCount {
            get {
                return 0;
            }
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.PushCount"/>
        public override int PushCount {
            get {
                return 1;
            }
        }
        
        /// <see cref="SwfOp.ByteCode.Actions.BaseAction.Compile"/>
        public override void Compile(BinaryWriter w) 
{
            base.Compile(w);
            CompileBody(w);
        }       
        
        /// <summary>overriden ToString method</summary>
        public override string ToString() {
            string b = (( (PushType)Type == PushType.String) ? "'" :"");
            return String.Format("push {1} as {0}",PushTypeNames[(int)Type],b+Value+b);
        }
    }
}

