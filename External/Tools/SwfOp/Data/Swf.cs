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
using SwfOp.Data.Tags;

namespace SwfOp.Data {
    
    /// <summary>
    /// The Swf class is basically a data structure for swf data, containing header and a collection of swf tags.
    /// </summary>
    public class Swf : IEnumerable {
        
        /// <summary>
        /// An array that contains BaseTag objects.
        /// </summary>
        private BaseTag[] tagList;
        
        /// <summary>
        /// The total number of bytecode blocks within all tags.
        /// </summary>
        private int _actionCount;   
        
        /// <summary>
        /// Swf header.
        /// </summary>
        public SwfHeader Header;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public Swf(SwfHeader header,BaseTag[] tagList) {
            
            this.Header = header;
            this.tagList = tagList;
            
            // count actions
            _actionCount = 0;
            foreach (BaseTag b in tagList) {
                _actionCount+=b.ActionRecCount;
            }
        }
        
        /// <summary>
        /// Get enumerator for iterating over tag collection
        /// </summary>
        public IEnumerator GetEnumerator() {            
            return (IEnumerator) new TagEnumerator(this.tagList);
        }
        
        // taglist iterator
        internal class TagEnumerator : IEnumerator {
            
            private int index = -1;
            private BaseTag[] tagList;
            
            public TagEnumerator(BaseTag[] tagList) {
                this.tagList = tagList;
                this.index = -1;
            }   
            
            public void Reset() {
                index = -1;
            }
            
            public bool MoveNext() {
                index++;
                return (index<tagList.Length);
            }
            
            // typed(=faster) access
            public BaseTag Current {
                get {
                    return (tagList[index]);
                }
            }
            
            // satisfy interface
            object IEnumerator.Current {
                get {
                    return (tagList[index]);
                }
            }
        }
        
        /// <summary>
        /// Swf version property.
        /// </summary>
        public byte Version {
            get {
                return Header.Version;
            }
            set {
                Header.Version = value;
            }
        }
        
        /// <summary>
        /// Uncompressed swf byte count.
        /// </summary>
        public int ByteCount {
            
            get {
                int len = 0;
                foreach (BaseTag tag in tagList) {
                    len += tag.Data.Length;
                }

                return 12+Header.Rect.Length+len;
            }
        }
        
        /// <summary>
        /// Accessor for total count of swf bytecode blocks.
        /// </summary>
        public int ActionCount {
            get {
                return _actionCount;
            }
        }
        
        /// <summary>
        /// Re-calc swf binary data.
        /// </summary>
        public void UpdateData() {
            
            foreach (BaseTag tag in tagList) {              
                tag.UpdateData(Header.Version);
            }           
        }       
        
    }
}

