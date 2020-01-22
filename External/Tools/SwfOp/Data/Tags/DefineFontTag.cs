using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SwfOp.Data.Tags
{
    class DefineFontTag:BaseTag
    {
        string name;
        bool isBold;
        bool isItalics;
        int glyphCount;

        public DefineFontTag(byte[] data, int offset, int tag): base(data, tag)
        {
            MemoryStream m = new MemoryStream(_data);
            BinaryReader br = new BinaryReader(m);
            br.BaseStream.Position += offset;
            br.ReadInt16(); // f_font2_id
            
            byte flags = br.ReadByte();
            isBold = (flags & 1) == 1;
            isItalics = (flags & 2) == 2;
            
            br.ReadChar(); // f_font2_language
            byte len = br.ReadByte(); // f_font2_name_length
            name = UTF8Encoding.UTF8.GetString(br.ReadBytes(len - 1));
            br.ReadByte(); // null
            
            glyphCount = br.ReadInt16();

            /*string style = "";
            if (isBold) style += " Bold";
            if (isItalics) style += " Italic";
            Console.WriteLine((TagCodeEnum)tag + " : " + name + style + " (" + glyphCount + ")");
            Console.WriteLine(Convert.ToString(flags, 2));*/
        }

        public string Name { get { return name; } }
        public int GlyphCount { get { return glyphCount; } }
        public bool IsBold { get { return isBold; } }
        public bool IsItalics { get { return isItalics; } }

    }
}
