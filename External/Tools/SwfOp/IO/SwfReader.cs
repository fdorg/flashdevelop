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
using SwfOp.Data;
using SwfOp.Data.Tags;
using SwfOp.Utils;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System.Text;

namespace SwfOp.IO {
    
    /// <summary>
    /// The SwfReader class reads and parses an swf from a stream. All swf data structures that might contain actions 
    /// or are in the way of accessing bytecode-actions are de-coded here. It doesnt decompile bytecode, though.
    /// This is handled by <see cref="SwfOp.ByteCode.Decompiler">SwfOp.ByteCode.Decompiler</see>
    /// </summary>
    public class SwfReader {
        
        protected byte version = 0;
        protected BinaryReader br;
        protected long streamEnd;
        
        /// <summary>
        /// Read only property; returns Flash version of the read swf.
        /// </summary>
        public byte Version {
            get {
                return version;
            }
        }

        /// <summary>
        /// Reads RectData from swf into a byte-array.      
        /// </summary>
        /// <remarks>
        /// Since were only interested in bytecode actions, theres no need to parse this data.
        /// </remarks>
        protected byte[] ReadRectData() {

            byte[] b = br.ReadBytes(17); // max. byte size of rect record
            BitArray ba = BitParser.GetBitValues(b);
            
            int Nbits = (int)BitParser.ReadUInt32(ba,5);
            int bitcount=5+Nbits*4;
            int bytecount = Convert.ToInt32(Math.Ceiling((double)bitcount/8.0));
            
            byte[] rect = new byte[bytecount];
            for (int i=0; i<bytecount; i++) {
                rect[i] = b[i];
            }
            br.BaseStream.Position-=b.Length-bytecount;
            
            return rect;
        }
        
        /// <summary>
        /// Reads RectData from swf into a byte-array
        /// </summary>
        /// <remarks>
        /// Since were only interested in bytecode actions, theres no need to parse this data.
        /// </remarks>
        protected byte[] ReadCXFormWithAlphaData() {
            
            byte[] b = br.ReadBytes(17); // max. byte size of transform record
            BitArray ba = BitParser.GetBitValues(b);

            bool hasAdd = ba[0];
            bool hasMult = ba[1];           
            
            int Nbits = (int)BitParser.ReadUInt32(ba,2,4);          
            int bitcount = 6 + ( hasAdd ? Nbits*4 : 0) + ( hasMult ? Nbits*4 : 0);
            
            int bytecount = Convert.ToInt32(Math.Ceiling((double)bitcount/8.0));
            byte[] cfa = new byte[bytecount];
            for (int i=0; i<bytecount; i++) {
                cfa[i] = b[i];
            }
            br.BaseStream.Position-=b.Length-bytecount;
            return cfa;
        }
        
        /// <summary>
        /// Reads MatrixData from swf into a byte-array
        /// </summary>
        /// <remarks>
        /// Since were only interested in bytecode actions, theres no need to parse this data.
        /// </remarks>
        protected byte[] ReadMatrixData() {
            
            byte[] b = br.ReadBytes(27); // max. byte size of matrix record
            BitArray ba = BitParser.GetBitValues(b);
            
            int bitcount = 1;
            int Nbits;
            
            if (ba[bitcount-1]) {               
                Nbits = (int)BitParser.ReadUInt32(ba,bitcount,5);
                bitcount+=5+Nbits*2;                
            }
            bitcount+=1;
            if (ba[bitcount-1]) {
                Nbits = (int)BitParser.ReadUInt32(ba,bitcount,5);
                bitcount+=5+Nbits*2;
            }
            Nbits = (int)BitParser.ReadUInt32(ba,bitcount,5);
            bitcount+=5+Nbits*2;
            int bytecount = Convert.ToInt32(Math.Ceiling((double)bitcount/8.0));
            byte[] matrix = new byte[bytecount];
            for (int i=0; i<bytecount; i++) {
                matrix[i] = b[i];
            }
            br.BaseStream.Position-=b.Length-bytecount;
            return matrix;
        }       
        
        /// <summary>
        /// Reads and parses swf header information into an instance of <see cref="SwfOp.Data.SwfHeader">SwfHeader</see>
        /// </summary>
        protected SwfHeader ReadHeader() {
            
            SwfHeader h = new SwfHeader(                                 
                br.ReadChars(3),
                br.ReadByte(),
                br.ReadUInt32(),
                this.ReadRectData(),
                br.ReadUInt16(),
                br.ReadUInt16()
            );
            
            this.version = h.Version;
            
            return h;
        }
        
        /// <summary>
        /// Reads and parses tag header as <see cref="SwfOp.Data.RecordHeader">RecordHeader</see>
        /// </summary>
        protected RecordHeader ReadRecordHeader() {

            if (br.BaseStream.Position >= streamEnd-2)
                return new RecordHeader(0, 2, false);

            UInt16 tagCL = br.ReadUInt16();
            UInt16 tagCode = Convert.ToUInt16(tagCL>>6);
            int tagLength = Convert.ToInt32(tagCL - (tagCode<<6));
            
            bool longTag;
            
            if (tagLength == 0x3F) {
                uint len = br.ReadUInt32();
                tagLength = Convert.ToInt32(len);
                longTag = (tagLength <= 127);
            } else {
                longTag = false;
            }
            
            if (tagLength>br.BaseStream.Length){
                throw new InvalidTagLengthException();
            }
            
            return new RecordHeader(tagCode,tagLength,longTag);
        }
        
        /// <summary>
        /// Reads and parses a <see cref="SwfOp.Data.ClipActionRec">ClipActionRec</see>
        /// </summary>
        protected ClipActionRec ReadClipActionRec() {
            
            // different behaviour for Flash 5 and Flash 6+
            if (version>=6) {
                if (br.ReadInt32()==0) {
                    return null;
                }
                br.BaseStream.Position -= 4;
            } else {                
                if (br.ReadInt16()==0) {
                    return null;
                }
                br.BaseStream.Position -= 2;
            }
                        
            byte[] flags;
            int size;           
            int keyCode;
            byte[] actions;
            
            // different behaviour for Flash 5 and Flash 6+
            if (version>=6) {
                
                flags = br.ReadBytes(4);                
                
                // swf events
                byte f6Events = flags[2];
                bool hasKeyPress = ( (f6Events & 0x02) != 0);
                bool hasConstruct = ( (f6Events & 0x04) != 0); // Flash 7 +

                size = br.ReadInt32();
                
                if (hasKeyPress) {                  
                    keyCode = Convert.ToInt32(br.ReadByte());
                    size--;                 
                } else { keyCode = -1; }

                actions = br.ReadBytes(size);
                
            } else {
                
                flags = br.ReadBytes(2);
                size = br.ReadInt32();              
                keyCode = -1;               
                actions = br.ReadBytes(size);
            }
        
            return new ClipActionRec(flags,keyCode,actions);
        }
        
        /**************************************************
                        Read SWF Tags
        **************************************************/

        /// <summary>
        /// Read and parse PlaceObject2Tag, leave non-bytecode data as raw byte-array 
        /// </summary>
        protected PlaceObject2Tag ReadPlaceObject2Tag() {
                        
            ReadRecordHeader();
            
            // get flags
            byte byte0 = br.ReadByte();
            BitArray flags = new BitArray(new byte[1]{byte0});
            BitParser.BytewiseReverse(flags);
            
            byte[] depth = br.ReadBytes(2);
            byte[] chId;
            byte[] matrix;
            byte[] ctf;
            byte[] ratio;
            byte[] name;
            byte[] clpd;
            
            bool hasActions; // even if length is 0
            byte[] actionHead;

            // contains byte code data
            ClipActionRec[] clipActions;            
            
            // read data, depending on flag settings
            if (flags[6]) { 
                chId = br.ReadBytes(2);
            } else {
                chId = new byte[0];
            }
            
            if (flags[5]) { 
                matrix = ReadMatrixData(); 
            } else {
                matrix = new byte[0];
            }
            
            if (flags[4]) { 
                ctf = ReadCXFormWithAlphaData(); 
            } else {
                ctf = new byte[0];
            }
            
            if (flags[3]) { 
                ratio = br.ReadBytes(2); 
            } else {
                ratio = new byte[0];
            }
            
            if (flags[2]) { 
                
                // save stream position
                long startStream = br.BaseStream.Position;
                
                // read characters
                byte ch;
                do { 
                    ch=br.ReadByte(); 
                } while (ch!=0);
                
                // block length
                int len = Convert.ToInt32(br.BaseStream.Position - startStream);
                
                // reset stream position
                br.BaseStream.Position = startStream;               
                // read name
                name = br.ReadBytes(len);
                
            } else {
                name = new byte[0];
            }
            
            // clip id
            if ( flags[1] ) { 
                clpd = br.ReadBytes(2); 
            } else {
                clpd = new byte[0];
            }
            
            hasActions = flags[0];
            
            // get bytecode actions
            if (hasActions) {
                
                // different behaviour for Flash 6+
                actionHead = (version>=6) ? br.ReadBytes(6) : br.ReadBytes(4);
                
                // read clip action records to list
                ArrayList clpAc = new ArrayList();
                
                ClipActionRec a;
                do {
                    a = ReadClipActionRec();                    
                    if (a!=null) clpAc.Add(a);
                    
                } while (a!=null);
                
                // copy list to array
                clipActions = new ClipActionRec[clpAc.Count];
                clpAc.CopyTo(clipActions,0);                
                
            } else {
                // no actions -> null
                return null; 
            }
            
            // tag-header (non-bytecode data before clipActionRec in tag) size varies with flags
            int size =  3 // flags 
                        +chId.Length                        
                        +matrix.Length
                        +ctf.Length
                        +ratio.Length
                        +name.Length
                        +clpd.Length
                        +actionHead.Length;
            
            byte[] header = new byte[size];         
            int pos = 1;        
            
            // copy all data to our tag-header array
            header[0] = byte0;          
            depth.CopyTo(header,1);     pos += depth.Length;
            chId.CopyTo(header,pos);    pos += chId.Length;
            matrix.CopyTo(header,pos);  pos += matrix.Length;
            ctf.CopyTo(header,pos);     pos += ctf.Length;
            ratio.CopyTo(header,pos);   pos += ratio.Length;
            name.CopyTo(header,pos);    pos += name.Length;
            clpd.CopyTo(header,pos);    pos += clpd.Length;
            actionHead.CopyTo(header,pos);   
            
            // return tag
            return new PlaceObject2Tag(header,clipActions);
        }
        
        /// <summary>
        /// Read and parse DefineSpriteTag, into inner tags and raw byte-array header data
        /// </summary>
        protected DefineSpriteTag ReadDefineSpriteTag() {
            
            RecordHeader rh = ReadRecordHeader();   

            // inner tags
            ArrayList tagList = new ArrayList();
            long endPos = br.BaseStream.Position + rh.TagLength;

            // stuff before inner tags, just read it and dont look any further
            byte[] header = br.ReadBytes(4);
            
            while (br.BaseStream.Position<endPos) {
                BaseTag b = this.ReadTag();
                tagList.Add(b);     
            }
            
            BaseTag[] tags = new BaseTag[tagList.Count];
            tagList.CopyTo(tags,0);

            return new DefineSpriteTag(header, tags, rh.TagLength);
        }
        
        /// <summary>
        /// Read and parse InitActionTag, containing sprite ID and bytecode
        /// </summary>
        protected InitActionTag ReadInitActionTag() {
                        
            RecordHeader rh = ReadRecordHeader();
            
            ushort spriteId = br.ReadUInt16();
            byte[] actionList = br.ReadBytes(rh.TagLength-2);
            
            return new InitActionTag(spriteId,actionList);
        }   
        
        /// <summary>
        /// Read and parse DoActionTag, contains only bytecode
        /// </summary>
        protected DoActionTag ReadDoActionTag() {
            
            RecordHeader rh = ReadRecordHeader();
            
            byte[] actionList = br.ReadBytes(rh.TagLength);
            
            return new DoActionTag(actionList);
        }
        
        /// <summary>
        /// Read and parse ScriptLimitTag
        /// </summary>
        protected ScriptLimitTag ReadScriptLimitTag() {
            
            RecordHeader rh = ReadRecordHeader();
            
            ushort maxRecursionDepth = br.ReadUInt16();
            ushort scriptTimeOut = br.ReadUInt16();
                        
            return new ScriptLimitTag(maxRecursionDepth,scriptTimeOut);
        }
        
        /// <summary>
        /// Read and parse JpegTableTag
        /// </summary>
        protected JpegTableTag ReadJpegTableTag() {
            
            RecordHeader rh = ReadRecordHeader();
            int tl = rh.TagLength;          
            byte[] data = br.ReadBytes(tl);
            
            return new JpegTableTag(data);          
        }
        
        /// <summary>
        /// Read and parse DefineBitsTag
        /// </summary>
        protected DefineBitsTag ReadDefineBitsTag() {
            
            RecordHeader rh = ReadRecordHeader();
            int tl = rh.TagLength;
            ushort id = br.ReadUInt16();            
            byte[] data = br.ReadBytes(tl-2);           
            return new DefineBitsTag(id,data);          
        }
        
        /// <summary>
        /// Read and parse DefineBitsJpeg2Tag
        /// </summary>
        protected DefineBitsJpeg2Tag ReadDefineBitsJpeg2Tag() {
            
            RecordHeader rh = ReadRecordHeader();
            int tl = rh.TagLength;
            ushort id = br.ReadUInt16();            
            byte[] data = br.ReadBytes(tl-2);           
            return new DefineBitsJpeg2Tag(id,data);         
        }
        
        /// <summary>
        /// Read and parse DefineBitsJpeg3Tag
        /// </summary>
        protected DefineBitsJpeg3Tag ReadDefineBitsJpeg3Tag() {
            
            RecordHeader rh = ReadRecordHeader();
            int tl = rh.TagLength;
            ushort id = br.ReadUInt16();    
            int imgLen = Convert.ToInt32(br.ReadUInt32());
            byte[] img = br.ReadBytes(imgLen);          
            byte[] alpha = br.ReadBytes(tl-6-imgLen);
            
            return new DefineBitsJpeg3Tag(id,img,alpha);
        }

        /// <summary>
        /// Read and parse DefineSoundTag
        /// </summary>
        protected DefineSoundTag ReadDefineSoundTag()
        {
            RecordHeader rh = ReadRecordHeader();
            int tl = rh.TagLength;
            ushort id = br.ReadUInt16();
            byte[] data = br.ReadBytes(tl - 2);
            return new DefineSoundTag(id, data);
        }
        

        protected BaseTag ReadExportTag(int offset)
        {
            br.BaseStream.Position += offset;
            ushort count = br.ReadUInt16();
            ushort id;
            ArrayList ids = new ArrayList();
            ArrayList names = new ArrayList();
            for (int i = 0; i < count; i++)
            {
                id = br.ReadUInt16();
                ids.Add(id);
                names.Add(ReadString());
            }
            return new ExportTag(ids, names);
        }

        protected string ReadString()
        {
            StringBuilder sb = new StringBuilder();
            byte c = br.ReadByte();
            while (c != 0)
            {
                sb.Append((char)c);
                c = br.ReadByte();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Read tag from swf input stream, non-bytecode tags are reads into base tags and wont get parsed
        /// </summary>
        virtual protected BaseTag ReadTag() {

            long posBefore = br.BaseStream.Position;
            RecordHeader rh = ReadRecordHeader();
            int offset = (int)(br.BaseStream.Position-posBefore);       
            br.BaseStream.Position = posBefore;
            switch (rh.TagCode) {
                
                case (int)TagCodeEnum.DefineSprite: return this.ReadDefineSpriteTag();
                
                case (int)TagCodeEnum.DoAction: return this.ReadDoActionTag();
                    
                case (int)TagCodeEnum.InitAction: return this.ReadInitActionTag();
                    
                case (int)TagCodeEnum.ScriptLimit: return this.ReadScriptLimitTag();
        
                case (int)TagCodeEnum.PlaceObject2: 
                    
                    PlaceObject2Tag p = this.ReadPlaceObject2Tag();
                    if (p==null) {
                        br.BaseStream.Position = posBefore; 
                        return new BaseTag(br.ReadBytes(rh.TagLength+offset));
                    } else {
                        return p;
                    }

                case (int)TagCodeEnum.DefineBits:
                case (int)TagCodeEnum.DefineBitsLossless:
                case (int)TagCodeEnum.DefineBitsLossless2:
                    return ReadDefineBitsTag();

                case (int)TagCodeEnum.DefineBitsJpeg2:
                    return ReadDefineBitsJpeg2Tag();

                case (int)TagCodeEnum.DefineBitsJpeg3:
                    return ReadDefineBitsJpeg3Tag();

                case (int)TagCodeEnum.DefineSound:
                    return ReadDefineSoundTag();

                case (int)TagCodeEnum.ExportAssets:
                case (int)TagCodeEnum.SymbolClass:
                    return ReadExportTag(offset);

                case (int)TagCodeEnum.DefineFont2:
                case (int)TagCodeEnum.DefineFont3:
                    return new DefineFontTag(br.ReadBytes(rh.TagLength + offset), offset, rh.TagCode);

                /*case 63:
                    // Dump tag
                    br.BaseStream.Position += offset;
                    byte b;
                    for (int i = 0; i < rh.TagLength; i++)
                    {
                        b = br.ReadByte();
                        Console.Write((b >= 32) ? ((char)b).ToString() : b.ToString());
                    }
                    br.BaseStream.Position = posBefore;
                    return new BaseTag(br.ReadBytes(rh.TagLength + offset));*/
                    
                default:
                    return new BaseTag(br.ReadBytes(rh.TagLength + offset), rh.TagCode);
            }
                        
        }

        /// <summary>
        /// Inflate compressed swf
        /// </summary>
        protected void inflate()
        {

            // read size
            br.BaseStream.Position = 4; // skip signature
            int size = Convert.ToInt32(br.ReadUInt32());

            // read swf head
            byte[] uncompressed = new byte[size];
            br.BaseStream.Position = 0;
            br.Read(uncompressed, 0, 8); // header data is not compre                                           

            // un-zip
            byte[] compressed = br.ReadBytes(size);
            Inflater zipInflator = new Inflater();
            zipInflator.SetInput(compressed);
            zipInflator.Inflate(uncompressed, 8, size - 8);

            // new memory stream for uncompressed swf
            MemoryStream m = new MemoryStream(uncompressed);
            br = new BinaryReader(m);
            br.BaseStream.Position = 0;
        }

        /// <summary>
        /// Uncompress LZMA compressed swf
        /// </summary>
        protected void inflateLZMA()
        {

            // read size
            br.BaseStream.Position = 4; // skip signature
            int size = Convert.ToInt32(br.ReadUInt32()); // uncompressed size
            int compressedSize = Convert.ToInt32(br.ReadUInt32()); // compressed size

            // read swf head
            byte[] uncompressed = new byte[size];
            br.BaseStream.Position = 0;
            br.Read(uncompressed, 0, 8); // header data (signature and uncompressed size) is not compressed                                 
            br.BaseStream.Position = 12; // skip compressed size

            byte[] properties = br.ReadBytes(5);
            byte[] compressed = br.ReadBytes(compressedSize);

            SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
            decoder.SetDecoderProperties(properties);
            // new memory stream for uncompressed swf
            MemoryStream m = new MemoryStream(uncompressed);

            m.Position = 8;
            decoder.Code(new MemoryStream(compressed), m, compressedSize, size-8, null);

            br = new BinaryReader(m);
            br.BaseStream.Position = 0;
        }
        
        /// <summary>
        /// Read swf (header and tags), this is the only public method of <see cref="SwfOp.IO.SwfReader">SwfReader</see>
        /// </summary>
        public Swf ReadSwf() {
            
            // compressed swf?
            if (br.PeekChar()=='C') {
                inflate();
            }
            else if (br.PeekChar() == 'Z') {
                inflateLZMA();
            }
            
            SwfHeader header = ReadHeader();
            this.version = header.Version;          

            ArrayList tagList = new ArrayList();

            try
            {
                streamEnd = br.BaseStream.Length;
                while (br.BaseStream.Position < streamEnd)
                {
                    BaseTag b = this.ReadTag();
                    tagList.Add(b);
                };
            }
            catch (Exception eos)
            {
                Console.WriteLine("-- Error: Tag reader error: [" + eos.Message + "]");
            }
            
            BaseTag[] tags = new BaseTag[tagList.Count];
            tagList.CopyTo(tags,0);
            br.Close();
            
            return new Swf(header,tags);
        }
        
        /// <summary>
        /// Swf Reader class, takes an input stream as single argument 
        /// </summary>
        /// <param name="stream">Stream to read swf from, must allow random access</param>
        public SwfReader(Stream stream) {               
            this.br = new BinaryReader(stream);         
        }
    }
}
