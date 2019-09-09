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

namespace SwfOp.Data.Tags {
    
    /// <summary>
    /// DefineSprite tag object
    /// </summary>
    public class DefineSpriteTag : BaseTag {
        
        /// <summary>
        /// raw header data
        /// </summary>
        private byte[] header;  
        
        /// <summary>
        /// inner tags
        /// </summary>
        private BaseTag[] tagList;
        
        /// <summary>
        /// action count including inner tags´ actions
        /// </summary>
        private int _actionCount;
        
        /// <summary>
        /// tag index for action block index
        /// </summary>
        private int[] tagForAction;
        
        /// <summary>
        /// contains action block counts for inner tags
        /// </summary>
        private int[] tagOffset;

        /// <summary>
        /// total size of the sprite (including children)
        /// </summary>
        private long size;

        /// <summary>
        /// symbol id
        /// </summary>
        private ushort id = ushort.MaxValue;
        
        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="header">header data</param>
        /// <param name="tags">list of inner tags</param>
        public DefineSpriteTag(byte[] header, BaseTag[] tags, long size) {
            
            this.header = header;
            this.tagList = tags;
            this.size = size;
            
            _tagCode = (int)TagCodeEnum.DefineSprite;
            
            _actionCount = 0;           
            foreach (BaseTag b in tagList) {
                _actionCount += b.ActionRecCount;
            }
            
            tagForAction = new int[_actionCount];
            tagOffset = new int[tagList.Length];
            
            int actionIdx = 0;
            for (int i=0; i<tagList.Length; i++) {              
                
                tagOffset[i] = actionIdx;
                int count = ((BaseTag)tagList[i]).ActionRecCount;
                if (count>0) {                  
                    for (int j=0; j<count; j++) {
                        tagForAction[actionIdx+j]=i;                        
                    }
                    actionIdx+=count;                   
                }
            }
        }

        /// <summary>
        /// Sprite ID
        /// </summary>
        public ushort Id
        {
            get
            {
                if (id != ushort.MaxValue) return id;

                using (BinaryReader br = new BinaryReader(new MemoryStream(header)))
                {
                    id = br.ReadUInt16();
                }
                return id;
            }
        }

        /// <summary>
        /// total size of the sprite (including children)
        /// </summary>
        public override long Size { get { return size; } }
        
        /// <summary>
        /// see <see cref="SwfOp.Data.Tags.BaseTag">base class</see>
        /// </summary>
        public override int ActionRecCount {
            get {
                return _actionCount;
            }
        }
        
        /// <summary>
        /// see <see cref="SwfOp.Data.Tags.BaseTag">base class</see>
        /// </summary>
        public override byte[] this[int index] {
            
            get {
                if ((index<0) || (index>=ActionRecCount)) return null;
                
                int offset = index-tagOffset[tagForAction[index]];
                return tagList[tagForAction[index]] [offset];
            }
            
            set {
                if ((index>-1) && (index<ActionRecCount)) {
                    int offset = index-tagOffset[tagForAction[index]];
                    tagList[tagForAction[index]][offset] = value;
                }
            }
        }
        
        /// <summary>
        /// see <see cref="SwfOp.Data.Tags.BaseTag">base class</see>
        /// </summary>
        public override void UpdateData(byte version) {         
            
            // update inner tags
            int len = 0;
            for (int i=0; i<tagList.Length; i++) {
                BaseTag tag = (BaseTag) tagList[i];
                tag.UpdateData(version);
                len += tag.Data.Length;             
            }               
            
            MemoryStream m = new MemoryStream();
            BinaryWriter w = new BinaryWriter(m);
                        
            RecordHeader rh = new RecordHeader(TagCode, len + header.Length ,true);
            
            rh.WriteTo(w);
            w.Write(header);
            for (int i=0; i<tagList.Length; i++) {
                BaseTag tag = (BaseTag) tagList[i];
                w.Write(tag.Data);
            }
            
            // update data
            _data = m.ToArray();
        }           
    }
}
