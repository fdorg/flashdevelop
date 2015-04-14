/*
 * 
 * User: Philippe Elsass
 * Date: 07/03/2006
 * Time: 22:40
 */

using System;
using System.IO;
using SwfOp.Data;
using SwfOp.Data.Tags;
using System.Text;
using System.Collections;

namespace SwfOp.IO
{
    /// <summary>
    /// Specialized SWF parser to extract some specific tags
    /// </summary>
    public class SwfExportTagReader: SwfReader
    {
        public SwfExportTagReader(Stream stream): base(stream) 
        { 
        }
        
        override protected BaseTag ReadTag() 
        {
            long posBefore = br.BaseStream.Position;
            RecordHeader rh = ReadRecordHeader();
            int offset = (int)(br.BaseStream.Position-posBefore);       
            br.BaseStream.Position = posBefore;

            TagCodeEnum tag = (TagCodeEnum)rh.TagCode;
            //if (tag != TagCodeEnum.End) Console.WriteLine("Tag: " + (TagCodeEnum)rh.TagCode);
            switch (tag)
            {
                //-- Parse sub-clips
                case TagCodeEnum.DefineSprite: 
                    return ReadDefineSpriteTag();
                                    
                case TagCodeEnum.ExportAssets:
                case TagCodeEnum.SymbolClass:
                    return ReadExportTag(offset);

                case TagCodeEnum.DoABCDefine:
                    return new AbcTag(br.ReadBytes(rh.TagLength + offset), offset, true);

                case TagCodeEnum.DoABC:
                    return new AbcTag(br.ReadBytes(rh.TagLength + offset), offset, false);
                
                case TagCodeEnum.FrameLabel:
                    return ReadFrameTag(offset);

                case TagCodeEnum.MetaData:
                    return ReadMetaDataTag(offset);

                case TagCodeEnum.DefineFont2:
                case TagCodeEnum.DefineFont3:
                    return new DefineFontTag(br.ReadBytes(rh.TagLength + offset), offset, rh.TagCode);

                /*case TagCodeEnum.DefineFontName:
                    br.BaseStream.Position += offset;
                    byte b;
                    Console.WriteLine(tag);
                    for (int i = 0; i < Math.Min(256, rh.TagLength); i++)
                    {
                        b = br.ReadByte();
                        Console.Write((b >= 32) ? ((char)b).ToString() : "[" + b.ToString() + "]");
                    }
                    Console.WriteLine();
                    br.BaseStream.Position = posBefore;
                    return new BaseTag(br.ReadBytes(rh.TagLength + offset), rh.TagCode);*/
                    
                default:
                    // Dump tag
                    /*br.BaseStream.Position += offset;
                    byte b;
                    Console.WriteLine(tag);
                    for (int i = 0; i < rh.TagLength; i++)
                    {
                        b = br.ReadByte();
                        Console.Write((b >= 32) ? ((char)b).ToString() :"[" + b.ToString() + "]");
                    }
                    Console.WriteLine();
                    br.BaseStream.Position = posBefore;*/
                    return new BaseTag(br.ReadBytes(rh.TagLength+offset), rh.TagCode);
            }
                
        }

        BaseTag ReadMetaDataTag(int offset)
        {
            br.BaseStream.Position += offset;
            string meta = ReadString();
            return new MetaDataTag(meta);
        }

        BaseTag ReadFrameTag(int offset)
        {
            br.BaseStream.Position += offset;
            string name = ReadString();
            return new FrameTag(name);
        }
    }
}
