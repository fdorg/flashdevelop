/*
 * 
 * User: Philippe Elsass
 * Date: 07/03/2006
 * Time: 22:19
 */

using System;
using System.IO;
using System.Collections;
using SwfOp.Data;

namespace SwfOp.Data.Tags
{
    /// <summary>
    /// Description of ExportTag.
    /// </summary>
    public class ExportTag : BaseTag
    {
        private ArrayList _names;
        private ArrayList _ids;
        
        public ExportTag(ArrayList ids, ArrayList names)
        {
            _tagCode = (int)TagCodeEnum.ExportAssets;
            _names = names;
            _ids = ids;
        }

        public ArrayList Ids
        {
            get { return _ids; }
        }
        
        public ArrayList Names 
        {
            get { return _names; }
        }
        
        /// <summary>
        /// see <see cref="SwfOp.Data.Tags.BaseTag">base class</see>
        /// </summary>
        public override void UpdateData(byte version) {
        
            MemoryStream m = new MemoryStream();
            BinaryWriter w = new BinaryWriter(m);
            
            int size = _ids.Count*2;
            foreach(string name in _names)
                size += name.Length+1; 
            
            RecordHeader rh = new RecordHeader(TagCode, 2 + size,true);
            
            rh.WriteTo(w);
            w.Write((ushort)_names.Count);
            
            for(int i=0; i<_names.Count; i++)
            {
                w.Write((ushort)_ids[i]);
                w.Write((string)_names[i]);
                w.Write((byte)0);
            }           
            
            // write to data array
            _data = m.ToArray();
        }
    }
}
