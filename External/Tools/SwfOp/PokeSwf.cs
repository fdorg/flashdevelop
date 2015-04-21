/*
 * 
 * User: Philippe Elsass
 * Date: 07/03/2006
 * Time: 20:47
 */

using System;
using System.IO;
using System.Collections;
using SwfOp.IO;
using SwfOp.Data;
using SwfOp.Data.Tags;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.Text;

namespace SwfOp
{
    /// <summary>
    /// C:\as3\projets\MonProjet\deploy\App.swf
    /// C:\as3\projets\MonProjet\App2.swf
    /// C:\flex_sdk_2\frameworks\libs\framework.swc
    /// C:\flex_sdk_2\frameworks\libs\playerglobal.swc
    /// C:\as3\library\ImageProcessing.swc
    /// C:\as3\corelib\bin\corelib.swc
    /// </summary>
    public class PokeSwf
    {
        static string operation;

        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("SwfOp <file.swf>: list all library symbols of the SWF");
                return;
            }
            string filename = args[args.Length-1];
            operation = (args.Length > 1) ? args[0] : "-list";
            
            // read SWF
            try
            {
                Stream filestream = File.OpenRead(filename);

                // SWC file: extract 'library.swf' file
                if (filename.EndsWith(".swc", StringComparison.OrdinalIgnoreCase))
                {
                    ZipFile zfile = new ZipFile(filestream);
                    foreach(ZipEntry entry in zfile)
                    {
                        if (entry.Name.EndsWith(".swf", StringComparison.OrdinalIgnoreCase))
                        {
                            ExploreSWF(new MemoryStream(UnzipFile(zfile, entry)));
                        }
                        else if (entry.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)
                            && entry.Name.StartsWith("docs/"))
                        {
                            string docSrc = Encoding.UTF8.GetString(UnzipFile(zfile, entry));
                        }
                    }
                    zfile.Close();
                    filestream.Close();
                }
                else if (filename.EndsWith(".abc", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] data = new byte[filestream.Length];
                    filestream.Read(data, 0, (int)filestream.Length);
                    BinaryReader br = new BinaryReader(new MemoryStream(data));
                    Abc abc = new Abc(br);
                }
                // regular SWF
                else ExploreSWF(new BufferedStream(filestream));
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("-- SwfOp Error: File not found");
            }
            catch(Exception ex)
            {
                Console.WriteLine("-- SwfOp Error: "+ex.Message);
            }
            Console.ReadLine();
        }

        private static byte[] UnzipFile(ZipFile zfile, ZipEntry entry)
        {
            Stream stream = zfile.GetInputStream(entry);
            byte[] data = new byte[entry.Size];
            int length = stream.Read(data, 0, (int)entry.Size);
            if (length != entry.Size)
                throw new Exception("Corrupted archive");
            return data;
        }
        
        static void ExploreSWF(Stream stream)
        {
            if (stream == null) return;
            SwfReader reader = new SwfReader(stream);
            //SwfExportTagReader reader = new SwfExportTagReader(stream);
            Swf swf = null;
            try
            {
                swf = reader.ReadSwf();
                foreach (BaseTag tag in swf)
                {
                    if (tag is ExportTag)
                    {
                        ExportTag etag = (ExportTag)tag;
                        for (int i = 0; i < etag.Ids.Count; i++)
                        {
                            BaseTag ftag = FindObject(swf, (ushort)etag.Ids[i]);
                            if (ftag is DefineSpriteTag)
                            {
                                DefineSpriteTag stag = (DefineSpriteTag)ftag;
                                Console.WriteLine("Symbol '" + etag.Names[i] + "' - " + stag.Size);
                            }
                            else if (ftag is DefineSoundTag)
                            {
                                DefineSoundTag stag = (DefineSoundTag)ftag;
                                Console.WriteLine("Sound '" + etag.Names[i] + "' - " + stag.MediaData.Length);
                            }
                            else if (ftag is DefineBitsTag)
                            {
                                DefineBitsTag btag = (DefineBitsTag)ftag;
                                Console.WriteLine("Image '" + etag.Names[i] + "' - " + btag.MediaData.Length);
                            }
                        }
                    }
                    else if (tag is DefineFontTag)
                    {
                        DefineFontTag ftag = (DefineFontTag)tag;
                        Console.WriteLine("Font '" + ftag.Name + "' - " + ftag.Data.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("-- Swf error: " + ex.Message);
            }
        }

        private static BaseTag FindObject(Swf swf, ushort id)
        {
            foreach (BaseTag tag in swf)
            {
                if (tag is DefineSpriteTag && (tag as DefineSpriteTag).Id == id)
                    return tag;
                else if (tag is DefineBitsTag && (tag as DefineBitsTag).Id == id)
                    return tag;
            }
            return null;
        }
    }
}
