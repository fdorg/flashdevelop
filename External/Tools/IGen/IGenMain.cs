/*
 * Actionscript Intrinsic Classes Generator
 *
 * @author Philippe Elsass
 */
using System;
using System.IO;
using System.Text;
using System.Collections;
using ASCompletion;

namespace IGenMain
{
    class MainClass: IASContext
    {
        #region IASContext interface members

        /// <summary>
        /// Add the current class' base path to the classpath
        /// </summary>
        /// <param name="fileName">Relative to this file</param>
        /// <param name="basePath">Resolved this base path</param>
        public void SetTemporaryBasePath(string fileName, string basePath)
        {
            //
        }

        /// <summary>
        /// Retrieves a parsed class from its name
        /// </summary>
        /// <param name="cname">Class (short or full) name</param>
        /// <param name="inClass">Current class</param>
        /// <returns>A parsed class or an empty ASClass if the class is not found</returns>
        public ASClass GetClassByName(string cname, ASClass inClass)
        {
            ASClass aClass = new ASClass();
            aClass.ClassName = cname;
            return aClass;
        }

        /// <summary>
        /// Retrieves a parsed class from its filename
        /// </summary>
        /// <param name="fileName">Class' file name</param>
        /// <returns>A parsed class or an empty ASClass if the class is not found or invalid</returns>
        public ASClass GetClassByFile(string fileName)
        {
            ASClass aClass = new ASClass();
            aClass.FileName = fileName;
            return aClass;
        }

        /// <summary>
        /// Retrieves the current active class
        /// </summary>
        /// <returns>ASClass objet</returns>
        public ASClass GetCurrentClass()
        {
            return null;
        }

        /// <summary>
        /// Find folder and classes in classpath
        /// </summary>
        /// <param name="folder">Path to eval</param>
        /// <returns>Package folders and classes</returns>
        public ASMemberList GetSubClasses(string folder)
        {
            return null;
        }

        /// <summary>
        /// (Re)Parse and cache a class file
        /// </summary>
        /// <param name="aClass">Class object</param>
        /// <returns>The class object</returns>
        public ASClass GetCachedClass(ASClass aClass)
        {
            return aClass;
        }

        /// <summary>
        /// Resolve wildcards in imports
        /// </summary>
        /// <param name="package">Package to explore</param>
        /// <param name="inClass">Current class</param>
        /// <param name="known">Packages already added</param>
        public void ResolveWildcards(string package, ASClass inClass, ArrayList known)
        {
            if (!known.Contains(package))
            {
                known.Add(package);
                ASMember pMember = new ASMember();
                pMember.Name = package+"*";
                pMember.Type = package+"*";
                inClass.Imports.Add(pMember);
            }
        }

        /// <summary>
        /// Depending on the context UI, display some message
        /// </summary>
        public void DisplayError(string message)
        {
            Console.WriteLine("IGen Parser error: "+message);
        }
        #endregion

        // already explored pathes
        static ArrayList known;
        // remove orphan intrinsic classes and empty folders
        static bool clean;
        static int filesCleaned;
        static int foldersCleaned;
        // source path
        static string srcPath;
        // destination path
        static string destPath;
        // classes processed
        static string currentFile;
        static int total;

        #region tools IO methods
        /// <summary>
        /// From FlashDevelop: FileSystem.cs
        /// Reads the file codepage from the file data.
        /// </summary>
        static int GetFileCodepage(string file)
        {
            byte[] bom = new byte[4];
            System.IO.FileStream fs = new System.IO.FileStream(file, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
            if (fs.CanSeek)
            {
                fs.Read(bom, 0, 4); fs.Close();
                if ((bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf))
                {
                    return Encoding.UTF8.CodePage;
                }
                else if ((bom[0] == 0xff && bom[1] == 0xfe))
                {
                    return Encoding.Unicode.CodePage;
                }
                else if ((bom[0] == 0xfe && bom[1] == 0xff))
                {
                    return Encoding.BigEndianUnicode.CodePage;
                }
                else if ((bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0xfe && bom[3] == 0x76))
                {
                    return Encoding.UTF7.CodePage;
                }
                else
                {
                    return Encoding.Default.CodePage;
                }
            }
            else
            {
                return Encoding.Default.CodePage;
            }
        }

        /// <summary>
        /// From FlashDevelop: FileSystem.cs
        /// </summary>
        static void Write(string file, string text, Encoding enc)
        {
            using (StreamWriter sw = new StreamWriter(file, false, enc))
            {
                sw.Write(text);
            }
        }
        #endregion

        /// <summary>
        /// Recursively convert classes
        /// </summary>
        /// <param name="path">folder to convert</param>
        static void ExploreFolder(string path)
        {
            currentFile = path;
            known.Add(path);

            // convert classes
            string[] files = Directory.GetFiles(path, "*.as");
            ASClass fClass;
            string destFile;
            DateTime timestamp;
            int codepage;
            foreach(string file in files)
            {
                currentFile = file;
                destFile = destPath+file.Substring(srcPath.Length);
                // not modified: ignore
                timestamp = File.GetLastWriteTime(file);
                if (File.Exists(destFile) && File.GetLastWriteTime(destFile) == timestamp)
                    continue;

                // parse class
                codepage = GetFileCodepage(file);
                fClass = new ASClass();
                fClass.FileName = file;
                ASClassParser.ParseClass(fClass);
                if (fClass.IsVoid())
                    continue;

                // create intrinsic
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                Write(destFile, fClass.GenerateIntrinsic(), Encoding.GetEncoding(codepage));
                File.SetCreationTime(destFile, timestamp);
                File.SetLastWriteTime(destFile, timestamp);
                total++;
            }

            // explore subfolders
            currentFile = path;
            string[] dirs = Directory.GetDirectories(path);
            foreach(string dir in dirs)
            {
                if (!known.Contains(dir)) ExploreFolder(dir);
            }
        }

        /// <summary>
        /// Remove empty folders and classes without match in the destination path
        /// </summary>
        /// <param name="path">Path to clean</param>
        /// <param name="keepFolder">Don't delete this folder if it's empty</param>
        /// <returns>The folder was empty & deleted</returns>
        static bool CleanFolder(string path, bool keepFolder)
        {
            known.Add(path);

            // check classes without match
            string[] files = Directory.GetFiles(path, "*.as");
            string checkFile;
            int filesCount = files.Length;
            foreach(string file in files)
            {
                checkFile = srcPath+file.Substring(destPath.Length);
                if (!File.Exists(checkFile))
                {
                    File.Delete(file);
                    filesCount--;
                    filesCleaned++;
                }
            }

            // explore subfolders
            string[] dirs = Directory.GetDirectories(path);
            int folderCount = dirs.Length;
            foreach(string dir in dirs)
            {
                if (!known.Contains(dir))
                    if (CleanFolder(dir, false))
                        folderCount--;
            }

            // folder is empty
            // -> will fail silently if there are still files in the folder
            if (!keepFolder && (folderCount == 0) && (filesCount == 0))
            {
                try
                {
                    Directory.Delete(path);
                    foldersCleaned++;
                    return true;
                }
                catch {}
            }
            return false;
        }

        /// <summary>
        /// IGen application
        /// </summary>
        public static int Main(string[] args)
        {
            // parameters
            if (args.Length < 2)
            {
                Console.WriteLine("IGen 1.1 - Intrinsic Classes Generator for ActionScript 2");
                Console.WriteLine("Copyright (c) 2007 Philippe Elsass");
                Console.WriteLine("");
                Console.WriteLine("Usage: igen [-clean] <source folder> <dest folder>");
                return 1;
            }
            clean = (args[0] == "-clean");
            srcPath = args[args.Length-2];
            destPath = args[args.Length-1];
            if (srcPath == destPath)
            {
                Console.WriteLine("IGen Error: <source path> and <dest path> must be different");
                return 2;
            }

            // parser context
            ASClassParser.Context = new MainClass();
            // exploration context
            known = new ArrayList();
            known.Add(destPath);

            // explore
            try
            {
                ExploreFolder(srcPath);
                if (clean)
                    CleanFolder(destPath, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("IGen Exception: "+currentFile+"\n"+ex.Message+"\n"+ex.StackTrace);
                return 3;
            }

            // done
            Console.WriteLine("IGen: "+total+" classes processed");
            if (clean)
            {
                Console.WriteLine("      "+filesCleaned+" classes cleaned");
                Console.WriteLine("      "+foldersCleaned+" empty folders removed");
            }
            return 0;
        }
    }
}
