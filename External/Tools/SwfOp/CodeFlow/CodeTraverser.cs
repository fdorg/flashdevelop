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
    /// The CodeTraverser provides a simple functionality for traversing Action lists
    /// by stepping through action lists and following branches. In opposite to <see cref="SwfOp.CodeFlow.CodeWalker"/>
    /// each index is only visited once. Action objects are handled by the IActionExaminer
    /// that is passed to the Traverse method.
    /// CodeTraverser provides a unified base functionality for code-flow analysis.
    /// </summary>
    public class CodeTraverser {        
        
        /// <summary>
        /// ArrayList of action objects derived from <see cref="SwfOp.ByteCode.Actions.BaseAction"/>.
        /// </summary>
        protected ArrayList actionRec;      
        private ArrayList visitedLabels;    
        private SortedList labelIndexTable; 
        
        private void traverse(int index,IActionExaminer examiner) {
            
            while (index<actionRec.Count) {
                
                BaseAction a = (BaseAction)actionRec[index];
                
                if (a is ActionLabel) {
                    ActionLabel l = a as ActionLabel;                       
                    if (visitedLabels.Contains(l.LabelId)) {
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
                        traverse( (int)labelIndexTable[ai.LabelId], examiner.Clone() ); 
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
        /// Start traversing.
        /// </summary>
        /// <param name="examiner">IActionExaminer object</param>
        public virtual void Traverse(IActionExaminer examiner) {
            
            visitedLabels = new ArrayList();    
            traverse(0,examiner);
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actions">ArrayList of action objects derived from BaseAction.</param>
        public CodeTraverser(ArrayList actions) {           
            
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
