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
using System.Collections;
using System.IO;
using SwfOp.ByteCode.Actions;

namespace SwfOp.ByteCode
{
    ///<summary>
    /// Compiler class, exposes one public method: <see cref="SwfOp.ByteCode.Compiler.Compile"/>.
    /// </summary>
    public class Compiler {
        
        /// <summary>
        /// Collaps sequence of single push actions into one multiple-push action
        /// </summary>
        private void CollapsPushActions(ArrayList actionRecord) {

            int i = 0;
            bool isPush;
            
            while (i<(actionRecord.Count-1)) {
                
                isPush = actionRecord[i] is ActionPush;
                if (isPush) {
                    
                    int j = i;
                    int count = 1;
                    
                    do {
                        i++;
                        if (i<actionRecord.Count) {
                            isPush=(actionRecord[i] is ActionPush);
                            if (isPush) count++;
                        }
                    } while ((isPush)&&(i<actionRecord.Count));
                    
                    if (count>1) {
                        ActionPush[] pushList = new ActionPush[count];
                        actionRecord.CopyTo(j,pushList,0,count);
                        
                        actionRecord.RemoveRange(j,count);                      
                        ActionPushList pl = new ActionPushList(pushList);                   
                        actionRecord.Insert(j,pl);
                        
                        i=j+1;                      
                    }
                    
                    
                } else {
                    
                    // recursively step through functions inner actions
                    ActionDefineFunction f = actionRecord[i] as ActionDefineFunction;
                    if (f!=null) CollapsPushActions(f.ActionRecord);
                    
                    // and function2 of course
                    ActionDefineFunction2 f2 = actionRecord[i] as ActionDefineFunction2;
                    if (f2!=null) CollapsPushActions(f2.ActionRecord);
                    i++;
                }
            }

        }
        
        ///<summary>
        /// Inner struct for storing branch data.
        /// </summary>
        struct JumpPos {

            public readonly int Position;
            public readonly IJump Jump;
            
            public JumpPos(int pos,IJump j) {
                Position = pos;
                Jump = j;
            }
        }
        
        ///<summary>
        /// Calculate branch offsets.
        ///</summary>
        private void CalcBranchOffsets(ArrayList actionRecord) {
            
            if (actionRecord.Count<1) return;
            
            ArrayList jumpList = new ArrayList();
            Hashtable labelPos = new Hashtable();
            
            int pos = 0;
            for (int i=0; i<actionRecord.Count; i++) {
                
                BaseAction action = (BaseAction) actionRecord[i];
                ActionLabel label = action as ActionLabel;
                IJump jump = action as IJump;               
                
                if (label!=null) {
                    labelPos[label.LabelId] = pos;
                }           
                
                if (jump!=null) {
                    jumpList.Add(new JumpPos(pos,jump));
                }
                
                // recursively step through function blocks
                ActionDefineFunction f = actionRecord[i] as ActionDefineFunction;
                if (f!=null) CalcBranchOffsets(f.ActionRecord);
                
                ActionDefineFunction2 f2 = actionRecord[i] as ActionDefineFunction2;
                if (f2!=null) CalcBranchOffsets(f2.ActionRecord);
                
                pos+=action.ByteCount;
            }
            
            for (int i=0; i<jumpList.Count; i++) {
                
                JumpPos j = (JumpPos) jumpList[i];
                int offset = (int)labelPos[j.Jump.LabelId]-j.Position-5;
                j.Jump.Offset = offset;
            }           
        }
        
        ///<summary>
        /// Calculate size or offset for action blocks.
        ///</summary>
        private void CalcBlockOffsets(ArrayList actionRecord) {
            
            if (actionRecord.Count<1) return;
            
            for (int i=0; i<actionRecord.Count; i++) {
                
                BaseAction a = (BaseAction) actionRecord[i];
                
                // action with
                ActionWith aWith = a as ActionWith;
                if (aWith!=null) {
                    int j=i;
                    int offset = 0;
                    do {                        
                        j++;
                        offset+=((BaseAction)actionRecord[j]).ByteCount;
                    } while ((actionRecord[j] as ActionEndWith)==null);
                    
                    int oldOffset = aWith.BlockLength;
                    
                    aWith.BlockLength = offset;
                }
                
                // action waitForFrame
                ActionWaitForFrame aWait = a as ActionWaitForFrame;
                if (aWait!=null) {
                    int j=i;
                    int count = 0;
                    BaseAction ca;
                    do {
                        j++;
                        ca = (BaseAction)actionRecord[j];
                        if ((ca.Code>=0)||(ca.Code==(int)ActionCode.PushList)) count++;
                    } while ( (ca as ActionEndWait) == null);
                    aWait.SkipCount = count;
                }
                
                // action waitForFrame2
                ActionWaitForFrame2 aWait2 = a as ActionWaitForFrame2;              
                if (aWait2!=null) {

                    int j=i;
                    int count = 0;
                    BaseAction ca;
                    do {
                        j++;
                        ca = (BaseAction)actionRecord[j];
                        if ((ca.Code>=0)||(ca.Code==(int)ActionCode.PushList)) count++;
                    } while ( (ca as ActionEndWait) == null);
                    aWait2.SkipCount = count;
                }
                
                // action function
                ActionDefineFunction f = actionRecord[i] as ActionDefineFunction;
                if (f!=null) CalcBlockOffsets(f.ActionRecord);
                
                // action function2
                ActionDefineFunction2 f2 = actionRecord[i] as ActionDefineFunction2;
                if (f2!=null) CalcBlockOffsets(f2.ActionRecord);
            }
            
        }       
        
        ///<summary>
        /// Compile list of Action objects to byte code.
        ///</summary>
        /// <param name="actionRecord">List of <see cref="SwfOp.ByteCode.Actions.BaseAction">action objects</see></param>
        public byte[] Compile(ArrayList actionRecord) {
            
            // code blocks
            CollapsPushActions(actionRecord);
            CalcBranchOffsets(actionRecord);    
            CalcBlockOffsets(actionRecord);
            
            // compile action-by-action
            foreach (object o in actionRecord) {
                BaseAction action = (BaseAction) o;
                action.Compile(binWriter);
            }

            return memStream.ToArray();
        }
        
        // writer
        private BinaryWriter binWriter;
        private MemoryStream memStream;
                
        ///<summary>
        /// Constructor.
        ///</summary>
        public Compiler() { 
            memStream = new MemoryStream();
            binWriter = new BinaryWriter(memStream,System.Text.Encoding.Default);
        }
    }   
}
