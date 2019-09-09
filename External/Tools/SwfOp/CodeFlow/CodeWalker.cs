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
using SwfOp.ByteCode.Actions;

namespace SwfOp.CodeFlow
{   
    /// <summary>
    /// The CodeWalker provides a simple functionality for traversing Action lists
    /// by stepping through code and following branches. In opposite to <see cref="SwfOp.CodeFlow.CodeTraverser"/>
    /// it doesn´t stop until a label is reached that has been visited before or a return
    /// statement is found. Action objects are handled by the IActionExaminerer that is passed to 
    /// the Traverse method.
    /// CodeWalker provide a base functionality for code analysis and simulation.
    /// </summary>
    public class CodeWalker {       
        
        private ArrayList actionRec;        
        private SortedList labelIndexTable;     
    
        private void Walk(int index,IActionExaminer examiner,ArrayList visitedLabels) {
                        
            while (index<actionRec.Count) {
                
                BaseAction a = (BaseAction)actionRec[index];
                
                if (a is ActionLabel) {
                    ActionLabel l = a as ActionLabel;                       
                    if (visitedLabels.Contains(l.LabelId)) {
                        //examiner.End();
                        return; 
                        
                    } else {
                        visitedLabels.Add(l.LabelId);
                    }
                }
                
                examiner.Examine(index,a);
                
                if (a is ActionJump) {
                        
                    ActionJump j = a as ActionJump;
                    index = (int)labelIndexTable[j.LabelId]-1;
                }               
                
                if (a is ActionIf) {
                    ActionIf ai = a as ActionIf;
                    if (!visitedLabels.Contains(ai.LabelId)) {
                        Walk( (int)labelIndexTable[ai.LabelId], examiner.Clone(), (ArrayList)visitedLabels.Clone() );   
                    }
                }
                
                if (a is ActionReturn) {
                    //examiner.End();
                    return;
                }
    
                index++;                
            }
            
            //examiner.End();
        }
        
        /// <summary>
        /// Start traversing at given index.
        /// </summary>
        /// <param name="examiner">IActionExaminer object</param>
        /// <param name="index">start index for code traversation.</param>
        public void Walk(IActionExaminer examiner,int index) {
            
            ArrayList visitedLabels = new ArrayList();  
            Walk(index,examiner,visitedLabels);
        }
        
        /// <summary>
        /// Start traversing code flow at index 0.
        /// </summary>
        /// <param name="examiner">IActionExaminer object</param>
        public void Walk(IActionExaminer examiner) {
            
            ArrayList visitedLabels = new ArrayList();  
            Walk(0,examiner,visitedLabels);
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actions">ArrayList of BaseAction objects.</param>
        public CodeWalker (ArrayList actions) {         
            
            actionRec = actions;
            labelIndexTable= new SortedList();
            
            // fill labelIndexTable
            for (int i=0; i<actions.Count; i++) {
                ActionLabel lbl = actions[i] as ActionLabel;
                if (lbl!=null) {
                    labelIndexTable[lbl.LabelId] = i;
                }
            }           
        }   
    }
}   
