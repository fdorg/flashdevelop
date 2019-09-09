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
using SwfOp.ByteCode.Actions;
using SwfOp.CodeFlow;
using SwfOp.Utils;

namespace SwfOp.ByteCode
{
    /// <summary>
    ///  Decompiler class. Compiles swf byte code to list of <see cref="SwfOp.ByteCode.Actions.BaseAction">action objects</see>.
    /// </summary>
    public class Decompiler {
        
        /// <summary>
        /// Read <see cref="SwfOp.ByteCode.Actions.ActionIf">if</see> action from swf.
        /// </summary>
        private ActionIf ReadActionIf(BinaryReader br) {

            int len = Convert.ToInt32(br.ReadInt16());
            Int16 o = br.ReadInt16();
            ActionIf a = new ActionIf( 0 );
            a.Offset = Convert.ToInt32(o);
            //a.ByteSize = len+3;
            return a;
        }
        
        /// <summary>
        /// Read <see cref="SwfOp.ByteCode.Actions.ActionJump">jump</see> action from swf.
        /// </summary>
        private ActionJump ReadActionJump(BinaryReader br) {

            int len = Convert.ToInt32(br.ReadInt16());
            Int16 o = br.ReadInt16();
            ActionJump a = new ActionJump( 0 );
            a.Offset = Convert.ToInt32(o);
            //a.ByteSize = len+3;           
            return a;
        }
        
        /// <summary>
        /// Read <see cref="SwfOp.ByteCode.Actions.ActionConstantPool">constant pool</see> action from swf. the constant pool is not parsed.
        /// </summary>
        private ActionConstantPool ReadActionConstantPool(BinaryReader br) {
            
            // read block length
            int len = Convert.ToInt32(br.ReadUInt16());
            
            int constCount = Convert.ToInt32(br.ReadUInt16());
            string[] constantPool = new string[constCount];
            
            for (int i=0; i<constCount; i++) {
                constantPool[i] = BinaryStringRW.ReadString(br);
            }
            ActionConstantPool a = new ActionConstantPool(constantPool);
            //a.ByteSize = len+3;
            return a;
        }
        
        /// <summary>
        /// Read <see cref="SwfOp.ByteCode.Actions.ActionStoreRegister">store register</see> action from swf.
        /// </summary>
        private ActionStoreRegister ReadActionStoreRegister(BinaryReader br) {
            int len = Convert.ToInt32(br.ReadUInt16());
            ActionStoreRegister a = new ActionStoreRegister( br.ReadByte());
            //a.ByteSize = len+3;
            return a;
        }
        
        /// <summary>
        /// Read multiply push action action as <see cref="SwfOp.ByteCode.Actions.ActionPushList">ActionPushList</see> from swf.
        /// </summary>
        private ActionPushList ReadActionPush(BinaryReader br) {
            
            // read block length
            int len = Convert.ToInt32(br.ReadUInt16());
            int i = 0;
            ArrayList pushList = new ArrayList();
            
            while (i<len) {
                
                int pushType = Convert.ToInt32(br.ReadByte());
                i++;
                
                object val = new object();
                
                switch (pushType) {
                    
                    case 0: string str = BinaryStringRW.ReadString(br);
                            i+=str.Length+1;
                            val = str;
                            break;
                    
                    case 1: val = (object)br.ReadSingle();
                            i+=4;
                            break;
                    
                    case 2: val = null;
                            break;
                    
                    case 3: val = null;
                            break;
                    
                    case 4: val = (object) Convert.ToInt32(br.ReadByte());
                            i++;
                            break;
                    
                    case 5: val = (object )br.ReadBoolean();
                            i++;
                            break;
                    
                    case 6: byte[] b0 = br.ReadBytes(4);
                            byte[] b1 = br.ReadBytes(4);
                            byte[] b = new byte[8];
                            b0.CopyTo(b,4);
                            b1.CopyTo(b,0);                         
                            val = (object) BitConverter.ToDouble(b,0);
                            i+=8;
                            break;
                    
                    case 7: val =(object) br.ReadInt32();
                            i+=4;
                            break;
                    
                    case 8: val = (object) Convert.ToInt32(br.ReadByte());
                            i++;
                            break;
                    
                    case 9: val = (object) Convert.ToInt32(br.ReadUInt16());
                            i+=2;
                            break;
                }
                
                ActionPush p = new ActionPush(pushType,val);
                pushList.Add(p);
            }
            
            ActionPush[] pList = new ActionPush[pushList.Count];
            pushList.CopyTo(pList,0);
            ActionPushList a = new ActionPushList(pList);
            
            //a.ByteSize = len+3;
            return a;
            
        }
        
        /// <summary>
        /// Read <see cref="SwfOp.ByteCode.Actions.ActionPushList">ActionDefineFunction</see> from swf. 
        /// including inner actions
        /// </summary>
        private ActionDefineFunction ReadActionDefineFunction(BinaryReader br) {
            
            int start = Convert.ToInt32(br.BaseStream.Position);
            
            // read block length
            int len = Convert.ToInt32(br.ReadUInt16());
            
            string name = BinaryStringRW.ReadString(br);
            int numParams = Convert.ToInt32(br.ReadUInt16());
            string[] parameterList = new string[numParams];
            for (int i=0; i<numParams; i++) {
                parameterList[i] = BinaryStringRW.ReadString(br);
            }
            
            int blockSize = Convert.ToInt32(br.ReadUInt16());
            
            // read function body
            ArrayList InnerActions = ReadActions(br.ReadBytes(blockSize));
            ActionDefineFunction a = new ActionDefineFunction(name,parameterList,InnerActions);
            
            int end =Convert.ToInt32( br.BaseStream.Position );
            //a.ByteSize = end-start +1;
            
            return a;
        }
        
        /// <summary>
        /// Read <see cref="SwfOp.ByteCode.Actions.ActionPushList">ActionDefineFunction2</see> from swf.
        /// including inner actions
        /// </summary>
        private ActionDefineFunction2 ReadActionDefineFunction2(BinaryReader br) {
            
            int start = Convert.ToInt32(br.BaseStream.Position);
            // read block length
            int len = Convert.ToInt32(br.ReadUInt16());
            
            string name = BinaryStringRW.ReadString(br);
            int numParams = Convert.ToInt32(br.ReadUInt16());
            int numRegs = Convert.ToInt32(br.ReadByte());
            byte flags1 = br.ReadByte();
            byte flags2 = br.ReadByte();
            
            ActionDefineFunction2.VariableFlagSet f 
                = new ActionDefineFunction2.VariableFlagSet(
                      (flags1 & 0x80) == 0x80,
                      (flags1 & 0x40) == 0x40,
                      (flags1 & 0x20) == 0x20,
                      (flags1 & 0x10) == 0x10,
                      (flags1 & 0x08) == 0x08,
                      (flags1 & 0x04) == 0x04,
                      (flags1 & 0x02) == 0x02,
                      (flags1 & 0x01) == 0x01,
                      (flags2 & 0x01) == 0x01
            );
            
            // read parameters
            ActionDefineFunction2.RegParamPair[] paramList = new ActionDefineFunction2.RegParamPair[numParams];
            
            for (int i=0; i<numParams; i++) {
                int r = Convert.ToInt32(br.ReadByte());
                string p =  BinaryStringRW.ReadString(br);
                paramList[i] = new ActionDefineFunction2.RegParamPair(r,p);
            }
            int blockSize = Convert.ToInt32(br.ReadUInt16());
            
            // read function body
            ArrayList InnerActions = ReadActions(br.ReadBytes(blockSize));
            ActionDefineFunction2 a = new ActionDefineFunction2(name,paramList,numRegs,f,InnerActions);
            
            int end =Convert.ToInt32( br.BaseStream.Position );
            ////a.ByteSize = len+3+blockSize;
            //a.ByteSize = end-start+1;     
    
            return a;
        }
        
        /// <summary>
        /// Read <see cref="SwfOp.ByteCode.Actions.ActionSetTarget">ActionSetTarget</see> from swf.
        /// </summary>      
        private ActionSetTarget ReadActionSetTarget(BinaryReader br) {
            
            int len = Convert.ToInt32(br.ReadUInt16());         
            
            ActionSetTarget a = new ActionSetTarget(BinaryStringRW.ReadString(br));
            //a.ByteSize = len+3; 
            
            return a;
        }
        
        /// <summary>
        /// Read <see cref="SwfOp.ByteCode.Actions.ActionGotoFrame">ActionGotoFrame</see> from swf.
        /// </summary>      
        private ActionGotoFrame ReadActionGotoFrame(BinaryReader br) {  
            
            int len = Convert.ToInt32(br.ReadUInt16()); 
            short f = br.ReadInt16();
            
            ActionGotoFrame a = new ActionGotoFrame(f);
            //a.ByteSize = len+3;
            
            return a;
        }
        
        /// <summary>
        /// Read <see cref="SwfOp.ByteCode.Actions.ActionGotoFrame2">ActionGotoFrame2</see> from swf.
        /// </summary>
        private ActionGotoFrame2 ReadActionGotoFrame2(BinaryReader br) {
            
            int len = Convert.ToInt32(br.ReadUInt16());
            
            byte[] b = br.ReadBytes(len);
            
            ActionGotoFrame2 a = new ActionGotoFrame2(b);
            //a.ByteSize = len+3;
            
            return a;
        }
        
        /// <summary>
        /// Read <see cref="SwfOp.ByteCode.Actions.ActionGotoLabel">ActionGotoLabel</see> from swf.
        /// </summary>
        private ActionGotoLabel ReadActionGotoLabel(BinaryReader br) {
            
            int len = Convert.ToInt32(br.ReadUInt16());
            string label = BinaryStringRW.ReadString(br);
            
            ActionGotoLabel a = new ActionGotoLabel(label);
            //a.ByteSize = len+3;
            
            return a;
        }
        
        
        /// <summary>
        /// Read <see cref="SwfOp.ByteCode.Actions.ActionGetUrl">ActionGetUrl</see> from swf.
        /// </summary>
        private ActionGetUrl ReadActionGetUrl(BinaryReader br) {
            
            int len = Convert.ToInt32(br.ReadUInt16());
            
            string urlStr = BinaryStringRW.ReadString(br);
            string targetStr = BinaryStringRW.ReadString(br);
            
            ActionGetUrl a = new ActionGetUrl(urlStr,targetStr);
            //a.ByteSize = len+3;           
            
            return a;
        }
        
        /// <summary>
        /// Read <see cref="SwfOp.ByteCode.Actions.ActionGetUrl2">ActionGetUrl2</see> from swf.
        /// </summary>
        private ActionGetUrl2 ReadActionGetUrl2(BinaryReader br) {
            
            int len = Convert.ToInt32(br.ReadUInt16());
            ActionGetUrl2 a = new ActionGetUrl2(br.ReadByte());
            //a.ByteSize = len+3;           
            
            return a; 
        }
    
        
        /// <summary>
        /// Read <see cref="SwfOp.ByteCode.Actions.ActionWaitForFrame">ActionWaitForFrame</see> from swf.
        /// </summary>      
        private ActionWaitForFrame ReadActionWaitForFrame(BinaryReader br) {
            
            int len = Convert.ToInt32(br.ReadUInt16());
            short frame = br.ReadInt16();
            byte skip = br.ReadByte();
            
            ActionWaitForFrame a = new ActionWaitForFrame(frame,skip);
            //a.ByteSize = len+3;           
            
            return a;
        }
        
        /// <summary>
        /// Read <see cref="SwfOp.ByteCode.Actions.ActionWaitForFrame2">ActionWaitForFrame2</see> from swf.
        /// </summary>      
        private ActionWaitForFrame2 ReadActionWaitForFrame2(BinaryReader br) {

            int len = Convert.ToInt32(br.ReadUInt16());
            byte skip = br.ReadByte();

            ActionWaitForFrame2 a = new ActionWaitForFrame2(skip);
            //a.ByteSize = len+3;           
            
            return a;
        }
        
        
        /// <summary>
        /// Read <see cref="SwfOp.ByteCode.Actions.ActionWith">ActionWith</see> from swf.
        /// </summary>
        private ActionWith ReadActionWith(BinaryReader br) {
            
            int len = Convert.ToInt32(br.ReadUInt16());
            ushort block = br.ReadUInt16();
            
            ActionWith a = new ActionWith(block);
            //a.ByteSize = len+3;           
            
            return a;
        }
    
        
        /// <summary>
        /// Read try/catch block from swf and create corresponding
        /// <see cref="SwfOp.ByteCode.Actions.ActionTry">ActionTry</see>,
        /// <see cref="SwfOp.ByteCode.Actions.ActionCatch">ActionCatch</see>,
        /// <see cref="SwfOp.ByteCode.Actions.ActionFinally">ActionFinally</see>,
        /// <see cref="SwfOp.ByteCode.Actions.ActionEndTryBlock">ActionEndTryBlock</see>
        /// actions.
        /// </summary>
        private ActionTry ReadActionTry(BinaryReader br) {          
            
            br.ReadUInt16();
            
            long startStream = br.BaseStream.Position;
            
            byte flags = br.ReadByte();
            
            bool catchInRegister = ((flags & 0x04) == 0x04);
            bool finallyFlag = ((flags & 0x02) == 0x02);
            bool catchFlag = ((flags & 0x01) == 0x01);
            
            ushort trySize = br.ReadUInt16();
            ushort catchSize = br.ReadUInt16();
            ushort finallySize = br.ReadUInt16();
            
            string catchName = "";
            byte catchRegister = 0;
            
            if (catchInRegister) {
                catchRegister = br.ReadByte();
            } else {
                catchName = BinaryStringRW.ReadString(br);
            }
            
            int len = Convert.ToInt32(br.BaseStream.Position-startStream);
            
            ActionTry a = new ActionTry(catchInRegister,finallyFlag,catchFlag,
                                        trySize,catchSize,finallySize,
                                        catchName,catchRegister);
            
            //a.ByteSize = len+3;
            
            return a;
        }
        

        /// <summary>
        /// Read unknown instruction as <see cref="SwfOp.ByteCode.Actions.UnknownAction">UnknownAction</see> object
        /// </summary>      
        private UnknownAction ReadUnknownAction(byte code,BinaryReader br) {
            
            byte[] bytecode;
            
            if (code<0x80) {
                bytecode = new byte[1] {code};
            } else {
                int len = Convert.ToInt32(br.ReadUInt16());
                br.BaseStream.Position-=3;
                bytecode = br.ReadBytes(len+3);             
            }
            
            UnknownAction u = new UnknownAction(code,bytecode);
            //u.ByteSize = bytecode.Length;
            
            return u;
        }
    
        /// <summary>
        /// Read actions according to action code in swf
        /// </summary>      
        private BaseAction ReadAction(BinaryReader br) {
            
            byte bytecode = br.ReadByte();
            
            switch ( (ActionCode) Convert.ToInt32(bytecode) ) {
                
                    // singlebyte actions
                    case ActionCode.End:    return  new ActionEnd();
                    case ActionCode.NextFrame:  return  new ActionNextFrame();
                    case ActionCode.PreviousFrame: return  new ActionPreviousFrame();
                    case ActionCode.Play: return  new ActionPlay();
                    case ActionCode.Stop: return  new ActionStop();
                    case ActionCode.ToggleQuality: return  new ActionToggleQuality();
                    case ActionCode.StopSounds: return  new ActionStopSounds();
                    case ActionCode.Pop: return  new ActionPop();
                    case ActionCode.Add: return  new ActionAdd();
                    case ActionCode.Subtract: return  new ActionSubtract();
                    case ActionCode.Multiply: return  new ActionMultiply();
                    case ActionCode.Divide: return  new ActionDivide();
                    case ActionCode.Equals: return  new ActionEquals();
                    case ActionCode.Less: return  new ActionLess();
                    case ActionCode.And: return  new ActionAnd();
                    case ActionCode.Or: return  new ActionOr();
                    case ActionCode.Not: return  new ActionNot();
                    case ActionCode.StringAdd: return  new ActionStringAdd();
                    case ActionCode.StringEquals: return  new ActionStringEquals();
                    case ActionCode.StringExtract: return  new ActionStringExtract();
                    case ActionCode.StringLength: return  new ActionStringLength();
                    case ActionCode.StringLess: return  new ActionStringLess();
                    case ActionCode.MBStringExtract: return  new ActionMBStringExtract();
                    case ActionCode.MBStringLength: return  new ActionMBStringLength();
                    case ActionCode.AsciiToChar: return  new ActionAsciiToChar();
                    case ActionCode.CharToAscii: return  new ActionCharToAscii();
                    case ActionCode.ToInteger: return  new ActionToInteger();
                    case ActionCode.MBAsciiToChar: return  new ActionMBAsciiToChar();
                    case ActionCode.MBCharToAscii: return  new ActionMBCharToAscii();
                    case ActionCode.Call: return  new ActionCall();
                    case ActionCode.GetVariable: return  new ActionGetVariable();
                    case ActionCode.SetVariable: return  new ActionSetVariable();
                    case ActionCode.GetProperty: return  new ActionGetProperty();
                    case ActionCode.RemoveSprite: return  new ActionRemoveSprite();
                    case ActionCode.SetProperty: return  new ActionSetProperty();
                    case ActionCode.SetTarget2: return  new ActionSetTarget2();
                    case ActionCode.StartDrag: return  new ActionStartDrag();
                    case ActionCode.CloneSprite: return  new ActionCloneSprite();
                    case ActionCode.EndDrag: return  new ActionEndDrag();
                    case ActionCode.GetTime: return  new ActionGetTime();
                    case ActionCode.RandomNumber: return  new ActionRandomNumber();
                    case ActionCode.Trace: return  new ActionTrace();
                    case ActionCode.CallFunction: return  new ActionCallFunction();
                    case ActionCode.CallMethod: return  new ActionCallMethod();
                    case ActionCode.DefineLocal: return  new ActionDefineLocal();
                    case ActionCode.DefineLocal2: return  new ActionDefineLocal2();
                    case ActionCode.Delete: return  new ActionDelete();
                    case ActionCode.Delete2: return  new ActionDelete2();
                    case ActionCode.Enumerate: return  new ActionEnumerate();
                    case ActionCode.Equals2: return  new ActionEquals2();
                    case ActionCode.GetMember: return  new ActionGetMember();
                    case ActionCode.InitArray: return  new ActionInitArray();
                    case ActionCode.InitObject: return  new ActionInitObject();
                    case ActionCode.NewMethod: return  new ActionNewMethod();
                    case ActionCode.NewObject: return  new ActionNewObject();
                    case ActionCode.SetMember: return  new ActionSetMember();
                    case ActionCode.TargetPath: return  new ActionTargetPath();
                    case ActionCode.ToNumber: return  new ActionToNumber();
                    case ActionCode.ToString: return  new ActionToString();
                    case ActionCode.TypeOf: return  new ActionTypeOf();
                    case ActionCode.Add2: return  new ActionAdd2();
                    case ActionCode.Less2: return  new ActionLess2();
                    case ActionCode.Modulo: return  new ActionModulo();
                    case ActionCode.BitAnd: return  new ActionBitAnd();
                    case ActionCode.BitLShift: return  new ActionBitLShift();
                    case ActionCode.BitOr: return  new ActionBitOr();
                    case ActionCode.BitRShift: return  new ActionBitRShift();
                    case ActionCode.BitURShift: return  new ActionBitURShift();
                    case ActionCode.BitXor: return  new ActionBitXor();
                    case ActionCode.Decrement: return  new ActionDecrement();
                    case ActionCode.Increment: return  new ActionIncrement();
                    case ActionCode.PushDuplicate: return  new ActionPushDuplicate();
                    case ActionCode.Return: return  new ActionReturn();
                    case ActionCode.StackSwap: return  new ActionStackSwap();
                    case ActionCode.InstanceOf: return  new ActionInstanceOf();
                    case ActionCode.Enumerate2: return  new ActionEnumerate2();
                    case ActionCode.StrictEquals: return  new ActionStrictEquals();
                    case ActionCode.Greater: return  new ActionGreater();
                    case ActionCode.StringGreater: return  new ActionStringGreater();
                    case ActionCode.Extends: return  new ActionExtends();
                    case ActionCode.CastOp: return  new ActionCastOp();
                    case ActionCode.Implements: return  new ActionImplements();
                    case ActionCode.Throw: return  new ActionThrow();
                    
                    // multibyte actions
                    case ActionCode.ConstantPool: return ReadActionConstantPool(br);                    
                    case ActionCode.GetURL: return ReadActionGetUrl(br);    
                    case ActionCode.GetURL2:    return ReadActionGetUrl2(br);                   
                    case ActionCode.WaitForFrame: return ReadActionWaitForFrame(br);
                    case ActionCode.WaitForFrame2: return ReadActionWaitForFrame2(br);                                      
                    case ActionCode.GotoFrame: return ReadActionGotoFrame(br);
                    case ActionCode.GotoFrame2: return ReadActionGotoFrame2(br);
                    case ActionCode.GoToLabel : return ReadActionGotoLabel(br);                 
                    case ActionCode.SetTarget : return ReadActionSetTarget(br);                 
                    case ActionCode.With: return ReadActionWith(br);
                    case ActionCode.Try: return ReadActionTry(br);
                    case ActionCode.Push: return ReadActionPush(br);
                    case ActionCode.StoreRegister: return   ReadActionStoreRegister(br);                    
                    case ActionCode.Jump: return ReadActionJump(br);
                    case ActionCode.If: return ReadActionIf(br);                        
                    case ActionCode.DefineFunction: return ReadActionDefineFunction(br);
                    case ActionCode.DefineFunction2: return ReadActionDefineFunction2(br);                  
                    
            }

            return ReadUnknownAction(bytecode,br);
        }
        
        /// <summary>
        /// Read bytecode actions from swf 
        /// </summary>
        
        private ArrayList ReadActions(byte[] codeblock) {
            
            ArrayList actionsRead = new ArrayList();
            
            // create binary reader
            MemoryStream stream = new MemoryStream(codeblock,false);
            BinaryReader reader = new BinaryReader(stream,System.Text.Encoding.UTF8);
            
            // read bytecode sequenz
            while (reader.PeekChar()!=-1) {
                
                // read
                BaseAction a = ReadAction(reader);
                                
                // define constant pool
                actionsRead.Add(a);             
            }           

            CreateBranchLabels(actionsRead);
            CreatePseudoActions(actionsRead);
            
            return actionsRead;
        }
        
        /// <summary>
        /// convert <see cref="SwfOp.ByteCode.Actions.ActionPushList">push list</see> to sequence of single <see cref="SwfOp.ByteCode.Actions.ActionPush" >push</see> actions
        /// </summary>      
        private void ExplodePushLists(ArrayList actionRecord) {
            
            for (int i=0; i<actionRecord.Count; i++) {
                
                BaseAction a = (BaseAction) actionRecord[i];
                
                // check if action is multiple push
                ActionPushList pl = actionRecord[i] as ActionPushList;              
                if (pl != null) {                       
                    
                    // resolve pushs to single push actions
                    for (int j=0; j<pl.Length; j++) {
                        
                        ActionPush p = pl[j];                                           
                        actionRecord.Insert(i+1+j,p);
                    }
                    
                    actionRecord.RemoveAt(i);                   
                }
                
                // process inner actionblocks               
                if (a as ActionDefineFunction != null) {    
                    ActionDefineFunction f = (ActionDefineFunction) a;
                    ExplodePushLists(f.ActionRecord);
                }
                if (a as ActionDefineFunction2 != null) {   
                    ActionDefineFunction2 f = (ActionDefineFunction2) a;
                    ExplodePushLists(f.ActionRecord);
                }
            }
        }
        
        /// <summary>
        /// create <see cref="SwfOp.ByteCode.Actions.ActionLabel">ActionLabel</see> pseudo actions for branch labels
        /// </summary>      
        private void CreateBranchLabels(ArrayList actionRecord) {
        
            SortedList labelList = new SortedList();            
    
            int idx = 0;
            while (idx<actionRecord.Count) {
                
                // read action
                BaseAction a = (BaseAction) actionRecord[idx];
                
                // check if action is branch
                if (a as IJump!=null) {

                    IJump jump = (IJump) a;
                    int offset = jump.Offset;
                    int sidx = idx;
                    
                    if (offset<0) {     
                        
                        // back branch
                        offset+=a.ByteCount;
    
                        while (offset<0) {

                            sidx--;             
                            if (sidx<0) break;

                            BaseAction ac = (BaseAction) actionRecord[sidx];
                            offset+=ac.ByteCount;                           
                        }
                        
                        if (!labelList.ContainsKey(sidx)) 
                        {
                            this.LabelId ++;
                            labelList[sidx] = this.LabelId;
                            jump.LabelId = this.LabelId;
                        } else {
                            jump.LabelId = (int)labelList[sidx];
                        }
                        
                        
                    } else {

                        if (offset==0) {        
                            sidx = idx+1;
                            if (!labelList.ContainsKey(sidx)) 
                            {
                                this.LabelId ++;
                                labelList[sidx] = this.LabelId;
                                jump.LabelId = this.LabelId;
                            } else {
                                jump.LabelId = (int)labelList[sidx];
                            }
                        } else {
                            // offset>0
                            do {
                                sidx++;     
                                if (sidx>=actionRecord.Count) break;
                            
                                BaseAction ac = (BaseAction) actionRecord[sidx];
                                offset-=ac.ByteCount;
                                
                            } while (offset>0);
                            sidx++;
                            if (!labelList.ContainsKey(sidx)) 
                            {
                                this.LabelId ++;
                                labelList[sidx] = this.LabelId;
                                jump.LabelId = this.LabelId;
                            } else {
                                jump.LabelId = (int)labelList[sidx];
                            }
                        
                        }
                    }
                }

                idx++;
            }
            
            ArrayList lines = new ArrayList(labelList.GetKeyList());
        
            foreach (int line in lines) {
                int label = (int)labelList[line];
                if (line<actionRecord.Count) {
                    BaseAction a = (BaseAction)actionRecord[line];
                    actionRecord[line] = new ActionContainer(
                        new BaseAction[2] {
                            new ActionLabel(label),
                            a
                        }
                    );
                } else {
                    actionRecord.Add(new ActionLabel(label));
                }
            }
            
            idx=0;
            while (idx<actionRecord.Count) {
                BaseAction a = (BaseAction) actionRecord[idx];          
                if (a is ActionContainer) {
                    BaseAction[] bl = ((ActionContainer)a).ActionList;
                    actionRecord.RemoveAt(idx);
                    int j=0;
                    while (j<bl.Length) {   
                        actionRecord.Insert(idx,bl[j]);                     
                        j++;
                        idx++;
                    }
                    continue;
                }                
                idx++;
            }       
        }   
        
        /// <summary>
        /// create other pseudo actions
        /// </summary>  
        
        private void CreatePseudoActions(ArrayList actionRecord) {  
            
            for (int i=0; i<actionRecord.Count; i++) {
                
                BaseAction a = (BaseAction)actionRecord[i];
                
                // -----------------------
                // try/catch/finally block
                // -----------------------
                
                ActionTry aTry = a as ActionTry;                
                if (aTry!=null) {
                    int j=i+1;
                    int offset = aTry.SizeTry;
                    // skip try block
                    while (offset>0) {
                        BaseAction currentAction = (BaseAction)actionRecord[j];
                        offset-=currentAction.ByteCount;                            
                        j++;
                    }
                    // skip catch
                    if (aTry.SizeCatch>0) {
                        actionRecord.Insert(j,new ActionCatch() );
                        j++;
                        offset = aTry.SizeCatch;
                        while (offset>0) {
                            BaseAction currentAction = (BaseAction)actionRecord[j];
                            offset-=currentAction.ByteCount;                            
                            j++;
                        }
                    }
                    // skip finally
                    if (aTry.SizeFinally>0) {
                        actionRecord.Insert(j,new ActionFinally() );
                        j++;
                        offset = aTry.SizeFinally;
                        while (offset>0) {
                            BaseAction currentAction = (BaseAction)actionRecord[j];
                            offset-=currentAction.ByteCount;                            
                            j++;
                        }
                    }
                    // end
                    actionRecord.Insert(j,new ActionEndTryBlock() );
                }
                
                // -----------------------
                //          with
                // -----------------------
                
                ActionWith aWith = a as ActionWith;
                
                if (aWith!=null) {
                    int j=i+1;
                    int offset = aWith.BlockLength;
                    while (offset>0) {
                        BaseAction currentAction = (BaseAction)actionRecord[j];
                        offset-=currentAction.ByteCount;                            
                        j++;
                    }
                    actionRecord.Insert(j,new ActionEndWith());
                }
                
                // -----------------------
                //    wait for frame
                // -----------------------
                        
                ActionWaitForFrame aWait = a as ActionWaitForFrame;     
                if (aWait!=null) {  
                    int j=i+1;
                    int count = aWait.SkipCount;
                    while (count>0) {
                        if (((int)((BaseAction)actionRecord[j]).Code >= 0x00)
                        ||(((BaseAction)actionRecord[j]).Code==(int)ActionCode.PushList)) {
                                count--;
                        }
                        j++;                        
                    }
                    actionRecord.Insert(j,new ActionEndWait());                 
                }
                ActionWaitForFrame2 aWait2 = a as ActionWaitForFrame2;      
                if (aWait2!=null) { 
                    int j=i+1;
                    int count = aWait2.SkipCount;
                    while (count>0) {
                        if (((int)((BaseAction)actionRecord[j]).Code >= 0x00)
                        ||(((BaseAction)actionRecord[j]).Code==(int)ActionCode.PushList)) {
                                count--;
                        }
                        j++;                        
                    }
                    actionRecord.Insert(j,new ActionEndWait());                 
                }
            }           
        }

        private int LabelId;
    
        /// <summary>
        /// decompile byte code to action objects
        /// </summary>
        public ArrayList Decompile(byte[] codeblock) {
            
            LabelId = 1;            
            ArrayList actionRec = ReadActions(codeblock);           
            ExplodePushLists(actionRec);
        
            // find argument count on stack 
            // e.g. for actions likecallFunction or initArray       
            CodeTraverser trav = new CodeTraverser(actionRec);
            trav.Traverse(new InvocationExaminer());        
        
            return actionRec;
        }
        
        // swf version info
        private int version;
        
        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="version">swf Version</param>
        public Decompiler(int version) {            
            this.version = version;
        }       
    }
}


