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
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using SwfOp.ByteCode;
using SwfOp.ByteCode.Actions;

namespace SwfOp.CodeFlow
{       
    /// <summary>
    /// The InvocationExaminer class analyses method/function calls and object
    /// initializations to find out how many values get pushed on or popped from stack.
    /// It is passed to an instance of CodeTraverser by the decompiler.
    /// </summary>
    public class InvocationExaminer : IActionExaminer {
        
        private Stack stack;                
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public InvocationExaminer() {   
            
            stack = new Stack();
            
            // we need to put some dummy values on stack
            // in case values get popped from empty stack
            for (int i=0; i<5; i++) stack.Push(null);
        }   
        
        /// <summary>
        /// Clone method, necessary for handling branches.
        /// </summary>
        public IActionExaminer Clone() {
            return new InvocationExaminer((Stack)stack.Clone());
        }

        /// <summary>
        /// Private constructor, used by Clone method.
        /// </summary>
        private InvocationExaminer(Stack s) {
            stack = s;
        }
        
        /// <summary>
        /// Examine byte code action at index in action record.
        /// </summary>
        public void Examine(int index,BaseAction a) {
            
            ActionPush p;
            int args;
            CodeTraverser trav;
            
            switch ( a.Code ) {
                
                case (int)ActionCode.StackSwap: 
                    object o1 = stack.Pop();
                    object o2 = stack.Pop();
                    stack.Push(o1);
                    stack.Push(o2);
                    break;
                
                case (int)ActionCode.PushDuplicate:
                    stack.Push(stack.Peek());
                    break;
                
                case (int)ActionCode.Push:  
                    stack.Push(a);
                    break;
                
                // --------------------------------------
                
                case (int)ActionCode.CallMethod:
                    
                    ActionCallMethod cm = a as ActionCallMethod;
                    
                    stack.Pop(); // name
                    stack.Pop(); // script object
                    
                    p = stack.Pop() as ActionPush;                  
                    args = p.GetIntValue();
                    for (int i=0; i<args; i++) {
                        stack.Pop();
                    }                           
                    if (args>-1) cm.NumArgs = args;
                    stack.Push(null);                                   
                    break;
                
                case (int)ActionCode.CallFunction:
                    
                    ActionCallFunction cf = a as ActionCallFunction;
                    stack.Pop(); // name                    
                    p = stack.Pop() as ActionPush;
                    args = p.GetIntValue();
                    for (int i=0; i<args; i++) {
                        stack.Pop();
                    }                           
                    if (args>-1) cf.NumArgs = args;
                    stack.Push(null);
                    break;
                
                // --------------------------------------
                
                case (int)ActionCode.InitArray:
                    
                    ActionInitArray ia = a as ActionInitArray;
                    p = stack.Pop() as ActionPush;                  
                    args = p.GetIntValue();
                    for (int i=0; i<args; i++) {
                        stack.Pop();
                    }           
                    
                    ia.NumValues = args;
                    stack.Push(null);
                    break;
                
                case (int)ActionCode.InitObject:
                    
                    ActionInitObject io = a as ActionInitObject;
                    p = stack.Pop() as ActionPush;                  
                    args = p.GetIntValue();
                    for (int i=0; i<args; i++) {
                        stack.Pop();
                        stack.Pop();
                    }                               
                    io.NumProps = args;
                    stack.Push(null);
                    break;
                
                case (int)ActionCode.NewObject:
                    
                    ActionNewObject n = a as ActionNewObject;
                    stack.Pop();
                    p = stack.Pop() as ActionPush;
                    args = p.GetIntValue();
                    for (int i=0; i<args; i++) {
                        stack.Pop();
                    }
                    n.NumArgs = args;
                    stack.Push(null);
                    break;
                
                case (int)ActionCode.NewMethod:
                    ActionNewMethod nm = a as ActionNewMethod;
                    stack.Pop();
                    stack.Pop();
                    p = stack.Pop() as ActionPush;
                    args = p.GetIntValue();
                    for (int i=0; i<args; i++) {
                        stack.Pop();
                    }   
                    nm.NumArgs = args;
                    stack.Push(null);
                    break;
                
                case (int)ActionCode.Implements:
                    
                    ActionImplements aimpl = a as ActionImplements;
                    stack.Pop(); // constructor function
                    
                    // interface count
                    p = stack.Pop() as ActionPush;                  
                    args = p.GetIntValue(); 

                    // pop interfaces
                    for (int i=0; i<args; i++) {
                        stack.Pop();
                    }   
                    aimpl.NumInterfaces = args;                 
                    break;
                
                // --------------------------------------
                
                case (int)ActionCode.DefineFunction:
                    ActionDefineFunction f = a as ActionDefineFunction;
                    trav = new CodeTraverser(f.ActionRecord);
                    trav.Traverse(new InvocationExaminer());
                    stack.Push(null);
                    break;
                
                case (int)ActionCode.DefineFunction2:
                    ActionDefineFunction2 f2 = a as ActionDefineFunction2;
                    trav = new CodeTraverser(f2.ActionRecord);
                    trav.Traverse(new InvocationExaminer());
                    stack.Push(null);
                    break;
                
                // --------------------------------------
                
                default:    
                    try {
                        for (int i=0; i<a.PopCount; i++) {
                            stack.Pop();
                        }
                        for (int i=0; i<a.PushCount; i++) {
                            stack.Push(null);                       
                        }                   
                    }
                    // stack empty
                    catch (InvalidOperationException e) {
                        if (e!=null) {}
                        stack.Clear();                      
                    }
                    break;
            }
        }       
    }   
}
