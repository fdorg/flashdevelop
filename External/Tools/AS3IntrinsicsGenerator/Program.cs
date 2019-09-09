// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using AS3Context;
using ASCompletion.Model;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;

namespace AS3IntrinsicsGenerator
{
    class Program
    {
        static private Context context;
        static private Dictionary<string, BlockModel> types;
        static private Dictionary<string, string> generated;
        static private PathModel currentClassPath;
        
        static void Main(string[] args)
        {
            ExtractManifests();
            //return;

            // AS3 doc parsing
            string AS3XML = "ActionsPanel_3.xml";
            if (!File.Exists(AS3XML))
            {
                Console.WriteLine("Error: missing {0}, copy this file from Flash CS4 installation", AS3XML);
                return;
            }
            ExtractXML(AS3XML);

            // SWC parsing
            Console.WriteLine("Parsing SWCs...");
            AS3Settings settings = new AS3Settings();
            context = new Context(settings);
            context.Classpath = new List<PathModel>();
            context.Classpath.Add(ParseSWC("libs\\player\\9\\playerglobal.swc"));
            context.Classpath.Add(ParseSWC("libs\\player\\10\\playerglobal.swc"));
            context.Classpath.Add(ParseSWC("libs\\player\\10.1\\playerglobal.swc"));
            context.Classpath.Add(ParseSWC("libs\\air\\servicemonitor.swc"));
            context.Classpath.Add(ParseSWC("libs\\air\\airglobal.swc"));
            context.Classpath.Add(ParseSWC("libs\\air\\applicationupdater.swc"));
            context.Classpath.Add(ParseSWC("libs\\air\\applicationupdater_ui.swc"));
            context.Classpath.Add(ParseSWC("libs\\air\\airframework.swc"));

            // Intrinsics generation
            Console.WriteLine("Generating intrinsics...");
            generated = new Dictionary<string, string>();
            GenerateIntrinsics("FP9", context.Classpath[0], false, false);
            GenerateIntrinsics("FP10", context.Classpath[1], false, true);
            GenerateIntrinsics("FP10.1", context.Classpath[2], false, true);
            GenerateIntrinsics("AIR", context.Classpath[3], true, true);
            GenerateIntrinsics("AIR", context.Classpath[4], true, true);
            GenerateIntrinsics("AIR", context.Classpath[5], true, true);
            GenerateIntrinsics("AIR", context.Classpath[6], true, true);
            GenerateIntrinsics("AIRFlex3", context.Classpath[7], true, true);

            Console.WriteLine("Done.");
        }

        private static void ExtractManifests()
        {
            Directory.CreateDirectory("catalogs");

            string[] files = Directory.GetFiles("libs");
            CreateCatalogs(files);
            files = Directory.GetFiles("libs\\air");
            CreateCatalogs(files);
        }

        private static void CreateCatalogs(string[] files)
        {
            foreach (string file in files)
            {
                Stream filestream = File.OpenRead(file);
                ZipFile zfile = new ZipFile(filestream);
                foreach (ZipEntry entry in zfile)
                {
                    if (entry.Name == "catalog.xml")
                    {
                        Stream stream = zfile.GetInputStream(entry);
                        byte[] data = new byte[entry.Size];
                        int length = stream.Read(data, 0, (int)entry.Size);
                        string src = UTF8Encoding.UTF8.GetString(data);
                        if (src.IndexOf("<components>") > 0)
                        {
                            src = Regex.Replace(src, "\\s*<libraries>.*</libraries>", "", RegexOptions.Singleline);
                            src = Regex.Replace(src, "\\s*<features>.*</features>", "", RegexOptions.Singleline);
                            src = Regex.Replace(src, "\\s*<files>.*</files>", "", RegexOptions.Singleline);
                            src = Regex.Replace(src, "icon=\"[^\"]+\"\\s*", "");
                            File.WriteAllText("catalogs\\" + Path.GetFileNameWithoutExtension(file) + "_catalog.xml", src);
                        }
                    }
                }
            }
        }

        private static void ExtractXML(string AS3XML)
        {
            types = new Dictionary<string, BlockModel>();
            Console.WriteLine("Extracting XML...");
            XmlDocument doc = new XmlDocument();
            doc.Load(AS3XML);
            XmlNodeList nodes = doc.LastChild.FirstChild.ChildNodes;
            foreach (XmlNode node in nodes)
            {
                string id = GetAttribute(node, "id");
                if (id.Length == 0) continue;
                if (id.IndexOf(' ') > 0)
                {
                    if (id == "Top Level") ParsePackage(node, "");
                    else if (id == "Language Elements") ParseTopLevel(node);
                    else Console.WriteLine("Unsupported: " + id);
                }
                else ParsePackage(node, id);
            }
        }

        private static PathModel ParseSWC(string swcFile)
        {
            PathModel path = new PathModel(System.IO.Path.GetFullPath(swcFile), context);
            if (!File.Exists(swcFile))
            {
                Console.WriteLine("Error: missing {0}, copy Flex SDK's lib directory", swcFile);
                return path;
            }
            SwfOp.ContentParser parser = new SwfOp.ContentParser(path.Path);
            parser.Run();
            AbcConverter.Convert(parser.Abcs, path, context);
            return path;
        }

        #region Intrinsics generation

        private static void GenerateIntrinsics(string dir, PathModel pathModel, bool targetAIR, bool targetFP10)
        {
            currentClassPath = pathModel;
            foreach (FileModel aFile in pathModel.Files.Values)
            {
                if (aFile.Package == "private") continue;

                ClassModel aClass = aFile.GetPublicClass();
                string type;
                string fileName;
                string src;

                // package-level declarations
                if (aClass.IsVoid()) 
                {
                    // MANUAL FIX toplevel
                    if (aFile.Package.Length == 0)
                    {
                        // clear inconsistent (although valid) default values
                        aFile.Members.Items
                            .FindAll(member => member.Parameters != null && member.Parameters.Count == 1)
                            .ForEach(member => member.Parameters[0].Value = null);

                        if (aFile.Members.Search("trace", 0, 0) == null)
                        {
                            MemberModel member = new MemberModel("trace", "String", FlagType.Function, Visibility.Public);
                            member.Parameters = new List<MemberModel>();
                            member.Parameters.Add(new MemberModel("...rest", null, 0, 0));
                            aFile.Members.Add(member);
                        }
                    }

                    type = aFile.Package + ".*";
                    aFile.Members.Sort();
                    if (types.Keys.Contains<string>(type))
                    {
                        BlockModel docModel = types[type];
                        AddDocumentation(aFile.Members, docModel);
                    }
                    fileName = (aFile.Package.Length == 0) ? "toplevel" : aFile.Package + ".package";
                    fileName = Path.Combine(Path.Combine("out", dir),
                        fileName.Replace('.', Path.DirectorySeparatorChar)) + ".as";

                    src = CompactSrc(aFile.GenerateIntrinsic(false));
                    src = src.Replace(";\r\n", ";\r\n\r\n");
                    if (generated.ContainsKey(type) && generated[type] == src) continue;
                    else generated[type] = src;
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                    File.WriteAllText(fileName, src/*, Encoding.UTF8*/);
                    continue;
                }

                // class declaration
                aFile.Members.Sort();
                aClass.Members.Sort();

                type = aClass.QualifiedName;
                fileName = Path.Combine(Path.Combine("out", dir), 
                    type.Replace('.', Path.DirectorySeparatorChar)) + ".as";

                // removing non-public members
                if ((aClass.Flags & FlagType.Interface) == 0)
                    aClass.Members.Items
                        .RemoveAll(member => member.Access != Visibility.Public);
                // removing AIR members
                if (dir != "AIR")
                    aClass.Members.Items
                        .RemoveAll(member => member.Comments != null && member.Comments.StartsWith("[AIR]"));

                // MANUAL FIX event consts' values
                if (aFile.Package == "flash.events" || aFile.Package == "mx.events")
                    aClass.Members.Items
                        .FindAll(member => (member.Flags & FlagType.Constant) > 0 && member.Type == "String" 
                            && Char.IsUpper(member.Name[0]) && member.Name != "VERSION")
                        .ForEach(member => member.Value = '"' + BaseModel.Camelize(member.Name) + '"');

                // MANUAL FIX MovieClip.addFrameScript
                if (aClass.QualifiedName == "flash.display.MovieClip")
                {
                    MemberModel member = aClass.Members.Search("addFrameScript", 0, 0);
                    if (member != null)
                    {
                        member.Comments = "[Undocumented] Takes a collection of frame (zero-based) - method pairs that associates a method with a frame on the timeline.";
                        member.Parameters = new List<MemberModel>();
                        member.Parameters.Add(new MemberModel("frame", "int", 0, 0));
                        member.Parameters.Add(new MemberModel("method", "Function", 0, 0));
                    }
                }

                // MANUAL FIX Sprite.toString (needed for override)
                if (aClass.QualifiedName == "flash.display.Sprite")
                {
                    if (aClass.Members.Search("toString", 0, 0) == null)
                        aClass.Members.Add(new MemberModel("toString", "String", FlagType.Function, Visibility.Public));
                }

                // MANUAL FIX Math
                if (aClass.QualifiedName == "Math")
                {
                    MemberModel member = aClass.Members.Search("atan2", 0, 0);
                    if (member != null)
                    {
                        member.Parameters[0].Name = "y";
                        member.Parameters[1].Name = "x";
                    }
                }

                // MANUAL FIX Object
                if (aClass.QualifiedName == "Object")
                {
                    if (aClass.Members.Search("toString", 0, 0) == null)
                        aClass.Members.Add(new MemberModel("toString", "String", FlagType.Function, Visibility.Public));
                    if (aClass.Members.Search("valueOf", 0, 0) == null)
                        aClass.Members.Add(new MemberModel("valueOf", "Object", FlagType.Function, Visibility.Public));
                    if (aClass.Members.Search("setPropertyIsEnumerable", 0, 0) == null)
                    {
                        MemberModel member = new MemberModel("setPropertyIsEnumerable", "void", FlagType.Function, Visibility.Public);
                        member.Parameters = new List<MemberModel>();
                        member.Parameters.Add(new MemberModel("name", "String", 0, 0));
                        member.Parameters.Add(new MemberModel("isEnum", "Boolean", 0, 0));
                        member.Parameters[1].Value = "true";
                        aClass.Members.Add(member);
                    }
                }

                // MANUAL FIX Proxy
                // TODO  Need to check ABC parser for specific namespaces
                if (aClass.QualifiedName == "flash.utils.Proxy")
                {
                    aClass.Members.Items.ForEach(member => member.Namespace = "flash_proxy");
                }

                // MANUAL FIX Array
                if (aClass.QualifiedName == "Array")
                {
                    MemberModel member = aClass.Members.Search("slice", 0, 0);
                    if (member != null)
                    {
                        member.Parameters[0].Name = "startIndex";
                        member.Parameters[1].Name = "endIndex";
                    }
                    member = aClass.Members.Search("splice", 0, 0);
                    if (member != null)
                    {
                        member.Parameters = new List<MemberModel>();
                        member.Parameters.Add(new MemberModel("startIndex", "int", 0, 0));
                        member.Parameters.Add(new MemberModel("deleteCount", "uint", 0, 0));
                        member.Parameters.Add(new MemberModel("...values", "", 0, 0));
                        member.Type = "Array";
                    }
                    member = aClass.Members.Search("sort", 0, 0);
                    if (member != null) member.Type = "Array";
                    member = aClass.Members.Search("sortOn", 0, 0);
                    if (member != null) member.Type = "Array";
                    member = aClass.Members.Search("length", FlagType.Constant, 0); // static const length WTF
                    if (member != null) aClass.Members.Remove(member);
                }

                // adding comments extracted from XML
                if (types.Keys.Contains<string>(type))
                {
                    BlockModel docModel = types[type];
                    aClass.Comments = docModel.Blocks[0].Comment;
                    AddDocumentation(aFile.Members, docModel);
                    AddDocumentation(aClass.Members, docModel.Blocks[0]);
                    AddEvents(aFile, docModel.Blocks[0], targetAIR, targetFP10);
                }

                src = CompactSrc(aFile.GenerateIntrinsic(false));
                if (generated.ContainsKey(type) && generated[type] == src) continue;
                else generated[type] = src;
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                File.WriteAllText(fileName, src/*, Encoding.UTF8*/);
            }
        }

        private static string CompactSrc(string src)
        {
            return src; //.Replace(" (", "(").Replace(" : ", ":");
        }

        private static void AddEvents(FileModel aFile, BlockModel docModel, bool targetAIR, bool targetFP10)
        {
            aFile.MetaDatas = new List<ASMetaData>();
            foreach (EventModel ev in docModel.Events)
            {
                if (!targetAIR && ev.IsAIR) continue;
                if (!targetFP10 && ev.IsFP10) continue;

                int p = ev.EventType.LastIndexOf('.');
                string type = ev.EventType.Substring(0, p);
                string pname = ev.EventType.Substring(p + 1);
                ClassModel eClass = getClass(type);
                if (eClass.IsVoid()) continue;
                string value = '"' + ev.Name + '"';
                if (eClass.Members.Items.Any<MemberModel>(item => item.Name == pname))
                {
                    ASMetaData meta = new ASMetaData("Event");
                    if (ev.Comment != null)
                        meta.Comments = "\r\n\t * " + ev.Comment + "\r\n\t * @eventType " + ev.EventType;
                    meta.ParseParams(String.Format("name=\"{0}\", type=\"{1}\"", ev.Name, type));
                    aFile.MetaDatas.Add(meta);
                }
            }
        }

        private static ClassModel getClass(string name)
        {
            string fileName = Path.Combine(currentClassPath.Path, name.Replace('.', '\\') + "$.as").ToUpper();
            if (currentClassPath.Files.ContainsKey(fileName))
                return currentClassPath.Files[fileName].GetPublicClass();
            return ClassModel.VoidClass;
        }

        private static void AddDocumentation(MemberList members, BlockModel docModel)
        {
            AddDocComments(members, docModel.Properties.ToArray());
            AddDocComments(members, docModel.Methods.ToArray());
        }

        private static void AddDocComments(MemberList members, BaseModel[] docMembers)
        {
            foreach (BaseModel docMember in docMembers)
            {
                MemberModel member = members.Search(docMember.Name, 0, 0);
                if (member == null) continue;
                member.Comments = docMember.Comment;
            }
        }

        private static void RegisterBlock(BlockModel block)
        {
            if (block.Blocks.Count > 0)
            {
                BlockModel classBlock = block.Blocks[0];
                types.Add((block.Name.Length > 0 ? block.Name + "." : "") + classBlock.Name, block);

                // MANUAL FIX Vector.join
                if (classBlock.Name == "Vector")
                {
                    classBlock.Methods.ForEach(method => 
                    {
                        if (method.Name == "join")
                            method.Params = method.Params.Replace("= ,", "= \",\"");
                    });
                }
            }
            else if (block.Methods.Count != 0 || block.Properties.Count != 0)
            {
                // MANUAL FIX flah.net package
                if (block.Name == "flash.net")
                {
                    block.Imports = new List<string>();
                    block.Imports.Add("flash.net.URLRequest");
                    block.Methods.ForEach(method =>
                    {
                        method.Params = method.Params.Replace("flash.net.URLRequest", "URLRequest");
                    });
                }
                types[block.Name + ".*"] = block;
            }
        }

        #endregion

        #region Extracting information from XML

        private static string GetAttribute(XmlNode node, string name)
        {
            try { return node.Attributes[name].Value; }
            catch { return ""; }
        }

        private static void ParseTopLevel(XmlNode node)
        {
            BlockModel block = new BlockModel();
            block.Name = "";
            //block.Decl = "package";
            foreach (XmlNode part in node.ChildNodes)
                ParseTopLevelPart(part, block);

            RegisterBlock(block);
        }

        private static void ParseTopLevelPart(XmlNode part, BlockModel block)
        {
            string id = GetAttribute(part, "id");
            if (id == "Global Functions")
            {
                ParseMethods(part, block);
                List<MethodModel> methods = new List<MethodModel>();
                foreach (MethodModel method in block.Methods)
                {
                    // ignore convertion methods
                    if (method.Name != method.ReturnType && method.Name != "Vector")
                    {
                        methods.Add(method);
                    }
                }
                block.Methods = methods;
            }
            else if (id == "Global Constants")
            {
                ParseProperties(part, block);
                foreach (PropertyModel prop in block.Properties)
                {
                    prop.Kind = "var";
                }
            }
        }

        private static void ParsePackage(XmlNode node, string package)
        {
            if (package.StartsWith("fl.")) 
                return;
            foreach (XmlNode part in node.ChildNodes)
            {
                BlockModel block = new BlockModel();
                block.Name = package;
                //block.Decl = "package " + package;

                ParsePart(part, block);
                
                // MANUAL FIX FOR SPECIAL CASES
                if (package == "flash.utils" && block.Blocks.Count == 0)
                {
                    PropertyModel ns = new PropertyModel();
                    ns.Kind = "namespace";
                    ns.Name = "flash_proxy";
                    ns.Comment = "Proxy methods namespace";
                    block.Properties.Insert(0, ns);
                }

                RegisterBlock(block);
            }
        }

        private static void ParsePart(XmlNode part, BlockModel block)
        {
            string id = GetAttribute(part, "id");
            if (id == "Methods") ParseMethods(part, block);
            else if (id == "Properties") ParseProperties(part, block);
            else if (id == "Events") ParseEvents(part, block);
            else ParseClass(part, block);
        }

        private static void ParseMethods(XmlNode part, BlockModel block)
        {
            foreach (XmlNode node in part.ChildNodes)
            {
                MethodModel model = new MethodModel();
                model.Name = GetAttribute(node, "name");
                string text = GetAttribute(node, "text");
                int p = text.IndexOf("):");
                if (p > 0) model.ReturnType = text.Substring(p + 2);
                else if (model.Name != block.Name) model.ReturnType = "void";
                text = text.Substring(text.IndexOf('%') + 1);
                text = text.Substring(0, text.LastIndexOf('%'));
                model.Params = text;
                model.Comment = FixComment(GetAttribute(node, "tiptext"));
                model.FixParams();
                block.Methods.Add(model);
            }
        }

        private static void ParseProperties(XmlNode part, BlockModel block)
        {
            foreach (XmlNode node in part.ChildNodes)
            {
                PropertyModel model = new PropertyModel();
                model.Name = GetAttribute(node, "name");
                string text = GetAttribute(node, "text");
                if (GetAttribute(node, "constant") == "true") model.Kind = "const";

                model.Comment = FixComment(GetAttribute(node, "tiptext"));
                block.Properties.Add(model);
            }
        }

        private static void ParseEvents(XmlNode part, BlockModel block)
        {
            foreach (XmlNode node in part.ChildNodes)
            {
                EventModel model = new EventModel();
                model.Name = GetAttribute(node, "name");
                model.IsAIR = GetAttribute(node, "playername").Trim() == "AIR" || GetAttribute(node, "version").Trim() == "1.0";
                model.IsFP10 = GetAttribute(node, "version").Trim() == "1.5";

                string temp = GetAttribute(node, "text");
                int p = temp.IndexOf("%type:String=");
                if (p > 0)
                {
                    temp = temp.Substring(p + 13);
                    model.EventType = "flash.events." + temp.Substring(0, temp.IndexOf('{'));
                }
                else model.EventType = "???";
                model.Comment = FixComment(GetAttribute(node, "tiptext"));
                block.Events.Add(model);
            }
        }

        private static void ParseClass(XmlNode node, BlockModel parentBlock)
        {
            BlockModel block = new BlockModel();
            block.Name = GetAttribute(node, "name");
            string ext = GetAttribute(node, "asAncestors");
            if (ext.Length > 0 && ext != "Object") ext = " extends " + ext.Split(',')[0].Replace(':', '.');
            else ext = "";
            block.Comment = FixComment(GetAttribute(node, "tiptext"));

            foreach (XmlNode part in node.ChildNodes)
                ParsePart(part, block);

            parentBlock.Blocks.Add(block);
        }

        private static string FixComment(string text)
        {
            // replace xml entities
            text = Regex.Replace(text, "\\&#([xA-Z0-9]+);", match =>
            {
                string num = match.Groups[1].Value;
                int v = num[0] == 'x' ? Convert.ToInt32(num.Substring(1), 16) : int.Parse(num);
                return ((char)v).ToString();
            });
            // replace html entities
            return text.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "'");
        }

        #endregion
    }
}
