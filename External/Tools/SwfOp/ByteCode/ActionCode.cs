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

namespace SwfOp.ByteCode
{       
    /// <summary>
    /// Action (byte-)codes for actions derived from <see cref="SwfOp.ByteCode.Actions.BaseAction"/>.   
    /// </summary>
    /// <remarks>
    /// Pseudo actions are given negative values since they are not compiled into swf.
    /// </remarks>
    public enum ActionCode {
        
        /// <summary>NextFrame instruction</summary>
        NextFrame = 0x04, 
        /// <summary>PreviousFrame instruction</summary>
        PreviousFrame = 0x05,
        /// <summary>Play instruction</summary>
        Play = 0x06,
        /// <summary>Stop instruction</summary>
        Stop = 0x07,
        /// <summary>ToggleQuality instruction</summary>
        ToggleQuality = 0x08,
        /// <summary>StopSounds instruction</summary>
        StopSounds = 0x09,
        /// <summary>Pop instruction</summary>
        Pop = 0x17,
        /// <summary>Add instruction</summary>
        Add = 0x0A,
        /// <summary>Subtract instruction</summary>
        Subtract = 0x0B,
        /// <summary>Multiply instruction</summary>
        Multiply = 0x0C,
        /// <summary>Divide  instruction</summary>
        Divide = 0x0D,
        /// <summary>Equals instruction</summary>
        Equals = 0x0E,
        /// <summary>Less instruction</summary>
        Less = 0x0F,
        /// <summary>And instruction</summary>
        And = 0x10,
        /// <summary>Or instruction</summary>
        Or = 0x11,
        /// <summary>Not instruction</summary>
        Not = 0x12,
        /// <summary>StringAdd instruction</summary>
        StringAdd = 0x21,
        /// <summary>StringEquals  instruction</summary>
        StringEquals = 0x13,
        /// <summary>StringExtract instruction</summary>
        StringExtract = 0x15,
        /// <summary>StringLength instruction</summary>
        StringLength = 0x14,
        /// <summary>StringLess  instruction</summary>
        StringLess = 0x29,
        /// <summary>MBStringExtract  instruction</summary>
        MBStringExtract = 0x35,
        /// <summary>MBStringLength instruction</summary>
        MBStringLength = 0x31,
        /// <summary>AsciiToChar instruction</summary>
        AsciiToChar = 0x33,
        /// <summary>CharToAscii  instruction</summary>
        CharToAscii = 0x32,
        /// <summary>ToInteger instruction</summary>
        ToInteger = 0x18,
        /// <summary>MBAsciiToChar instruction</summary>
        MBAsciiToChar = 0x37,
        /// <summary>MBCharToAscii instruction</summary>
        MBCharToAscii = 0x36,
        /// <summary>Call instruction</summary>
        Call = 0x9E,
        /// <summary>GetVariable  instruction</summary>
        GetVariable = 0x1C,
        /// <summary>SetVariable instruction</summary>
        SetVariable = 0x1D,
        /// <summary>GetPropertye instruction</summary>
        GetProperty = 0x22,
        /// <summary>RemoveSprite instruction</summary>
        RemoveSprite = 0x25,
        /// <summary>SetProperty instruction</summary>
        SetProperty = 0x23,
        /// <summary>SetTarget2 instruction</summary>
        SetTarget2 = 0x20,
        /// <summary>StartDrag instruction</summary>
        StartDrag = 0x27,
        /// <summary>CloneSprite instruction</summary>
        CloneSprite = 0x24,
        /// <summary>EndDrag instruction</summary>
        EndDrag = 0x28,
        /// <summary>GetTime instruction</summary>
        GetTime = 0x34,
        /// <summary>RandomNumber instruction</summary>
        RandomNumber = 0x30,
        /// <summary>Trace instruction</summary>
        Trace = 0x26,
        /// <summary>CallFunction instruction</summary>
        CallFunction = 0x3D,
        /// <summary>CallMethod instruction</summary>
        CallMethod = 0x52,
        /// <summary>DefineLocal instruction</summary>
        DefineLocal = 0x3C,
        /// <summary>DefineLocal2 instruction</summary>
        DefineLocal2 = 0x41,
        /// <summary>Delete instruction</summary>
        Delete = 0x3A,
        /// <summary>Delete2 instruction</summary>
        Delete2 = 0x3B,
        /// <summary>Enumerate instruction</summary>
        Enumerate = 0x46,
        /// <summary>Equals2  instruction</summary>
        Equals2 = 0x49,
        /// <summary>GetMembe instruction</summary>
        GetMember = 0x4E,
        /// <summary>InitArray instruction</summary>
        InitArray = 0x42,
        /// <summary>InitObject instruction</summary>
        InitObject = 0x43,
        /// <summary>NewMethod instruction</summary>
        NewMethod = 0x53,
        /// <summary>NewObject instruction</summary>
        NewObject = 0x40,
        /// <summary>SetMember  instruction</summary>
        SetMember = 0x4F,
        /// <summary>TargetPath instruction</summary>
        TargetPath = 0x45,
        /// <summary>ToNumber instruction</summary>
        ToNumber = 0x4A,
        /// <summary>ToString instruction</summary>
        ToString = 0x4B,
        /// <summary>TypeOf instruction</summary>
        TypeOf = 0x44,
        /// <summary>Add2 instruction</summary>
        Add2 = 0x47,
        /// <summary>Less2 instruction</summary>
        Less2 = 0x48,
        /// <summary>Modulo instruction</summary>
        Modulo = 0x3F,
        /// <summary>BitAnd instruction</summary>
        BitAnd = 0x60,
        /// <summary>BitLShift  instruction</summary>
        BitLShift = 0x63,
        /// <summary>BitOr instruction</summary>
        BitOr = 0x61,
        /// <summary>BitRShift  instruction</summary>
        BitRShift = 0x64,
        /// <summary>BitURShift instruction</summary>
        BitURShift = 0x65,
        /// <summary>BitXor instruction</summary>
        BitXor = 0x62,
        /// <summary>Decremente instruction</summary>
        Decrement = 0x51,
        /// <summary>Increment instruction</summary>
        Increment = 0x50,
        /// <summary>PushDuplicate instruction</summary>
        PushDuplicate = 0x4C,
        /// <summary>Return instruction</summary>
        Return = 0x3E,
        /// <summary>StackSwap instruction</summary>
        StackSwap = 0x4D,
        /// <summary>InstanceOf  instruction</summary>
        InstanceOf = 0x54,
        /// <summary>Enumerate2  instruction</summary>
        Enumerate2 = 0x55,
        /// <summary>StrictEquals instruction</summary>
        StrictEquals = 0x66,
        /// <summary>Greater instruction</summary>
        Greater = 0x67,
        /// <summary>StringGreater instruction</summary>
        StringGreater = 0x68,       
        /// <summary>Extends instruction</summary>
        Extends = 0x69,
        /// <summary>CastOp instruction</summary>
        CastOp = 0x2B,
        /// <summary>Implements instruction</summary>
        Implements = 0x2C,
        /// <summary>Throw instruction</summary>
        Throw = 0x2A,
        /// <summary>Action Record End</summary>
        End = 0x00,
        
        /// <remarks>multibyte actions:</remarks>
        /// <summary>SetTarget instruction; multi-byte</summary>
        SetTarget = 0x8B,             
        /// <summary>GotoFrame instruction; multi-byte</summary>
        GotoFrame = 0x81, 
        /// <summary>GotoFrame2 instruction; multi-byte</summary>
        GotoFrame2 = 0x9F,  
        /// <summary>GoToLabel instruction; multi-byte</summary>
        GoToLabel = 0x8C, 
        /// <summary>GetURL  instruction; multi-byte</summary>
        GetURL = 0x83,  
        /// <summary>GetURL2 instruction; multi-byte</summary>
        GetURL2 = 0x9A,  
        /// <summary>StoreRegister instruction; multi-byte</summary>
        StoreRegister = 0x87,  
        /// <summary>ConstantPool instruction; multi-byte</summary>
        ConstantPool = 0x88,        
                
        /// <remarks>jump actions:</remarks>
        /// <summary>If instruction (conditional branch); multi-byte</summary>
        If = 0x9D,  
        /// <summary>Jump instruction (branch); multi-byte</summary>
        Jump = 0x99,  
            
        /// <remarks>block actions:</remarks>
        /// <summary>DefineFunction (block) instruction; multi-byte</summary>
        DefineFunction = 0x9B,  
        /// <summary>DefineFunction2 (block) instruction; multi-byte</summary>
        DefineFunction2 = 0x8E,
        
        /// <summary>Push instruction; multi-byte</summary>
        Push = 0x96,
        /// <summary>Try (block) instruction; multi-byte</summary>
        Try = 0x8F, 
        /// <summary>With instruction; multi-byte</summary>
        With = 0x94,
        /// <summary>WaitForFrame instruction; multi-byte</summary>
        WaitForFrame = 0x8A,  
        /// <summary>WaitForFrame2 instruction; multi-byte</summary>
        WaitForFrame2 = 0x8D, 
        
        /// <remarks>pseudo actions:</remarks>
        /// <summary>Dummy pseudo instruction</summary>
        Dummy = -1,     
        /// <summary>PushList pseudo instruction</summary>
        PushList = -2,
        /// <summary>Label pseudo instruction</summary>
        Label = -3,     
        /// <summary>Catch pseudo instruction</summary>
        Catch = -4,
        /// <summary>Finally pseudo instruction</summary>
        Finally = -5,
        /// <summary>EndTry pseudo instruction</summary>
        EndTry = -6,        
        /// <summary>EndWith pseudo instruction</summary>
        EndWith = -7,       
        /// <summary>EndWait pseudo instruction</summary>
        EndWait = -8,
        /// <summary>action container pseudo instruction</summary>
        Container = -9
    }
}
