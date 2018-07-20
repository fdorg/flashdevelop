// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASCompletion.Model;
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml;

namespace MXMLCompletionBuilder
{
    class Program
    {
        static Regex reMatchEvents = new Regex("^\\s*\\[Event\\((name=)?\"([^\"]+)\"", RegexOptions.Multiline);
        static Regex reMatchStyles = new Regex("^\\s*\\[Style\\((name=)?\"([^\"]+)\"", RegexOptions.Multiline);
        static Regex reMatchEffects = new Regex("^\\s*\\[Effect\\((name=)?\"([^\"]+)\"", RegexOptions.Multiline);
        static Regex reMatchExcludes = new Regex("^\\s*\\[Exclude\\((name=)?\"([^\"]+)\"", RegexOptions.Multiline);
        static Regex reMatchDefaultProperty = new Regex("^\\s*\\[DefaultProperty\\(\"([^\"]+)\"", RegexOptions.Multiline);
        static Regex reMatchIncludes = new Regex("^\\s*include \"([^\"]+)\"", RegexOptions.Multiline);

        static private string inputFile;
        static private string outputFile;
        static private GeneratorConfig config;
        static private int index = 0;
        static private PathExplorer explorer;
        static private AS3Context.Context context;
        static private PathModel pathModel;
        static private HashSet<string> mxTags;
        static private Dictionary<string, TypeInfos> groups;
        static private StreamWriter output;
        static private StreamWriter log;
        static private List<string> names;

        static void Main(string[] args)
        {
            inputFile = "config.xml";
            outputFile = "mxml.xml";

            if (args.Length == 1 && args[0] == "help")
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  MXMLCompletionBuilder [<configuration file> [<output file>]]");
                return;
            }

            if (args.Length > 0)
            {
                inputFile = args[0];
                outputFile = Path.Combine(Path.GetDirectoryName(inputFile), outputFile);
            }
            if (args.Length > 1) outputFile = args[1];

            // read config file
            if (File.Exists(inputFile))
            {
                try
                {
                    config = GeneratorConfig.Deserialize(inputFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to load configuration:\n" + ex.Message);
                }
            }

            // write default config file
            if (config == null)
            {
                config = new GeneratorConfig();
                config.FlexFrameworkSWCs = new string[] {
                    @"C:\flex_sdk_3\frameworks\libs",
                    @"C:\flex_sdk_3\frameworks\libs\air"
                };
                config.Classpath = new string[] {
                    @"C:\Program Files\FlashDevelop\Library\AS3\intrinsic",
                    @"C:\flex_sdk_3\frameworks\projects\rpc\src",
                    @"C:\flex_sdk_3\frameworks\projects\framework\src",
                    @"C:\flex_sdk_3\frameworks\projects\airframework\src"
                };
                config.IncludePackage = new SerializableDictionary<string, string>();
                config.IncludePackage.Add("flash.display", "display");
                config.IncludePackage.Add("flash.errors", "errors");
                config.IncludePackage.Add("flash.events", "events");
                config.IncludePackage.Add("flash.external", "external");
                config.IncludePackage.Add("flash.filters", "filters");
                config.IncludePackage.Add("flash.geom", "geom");
                config.IncludePackage.Add("flash.media", "media");
                config.IncludePackage.Add("flash.net", "net");
                config.IncludePackage.Add("flash.printing", "printing");
                //config.IncludePackage.Add("flash.system", "system");
                config.IncludePackage.Add("flash.text", "text");
                config.IncludePackage.Add("flash.ui", "ui");
                config.IncludePackage.Add("flash.utils", "utils");
                config.IncludePackage.Add("flash.xml", "xml");

                config.BuiltInTags = new SerializableDictionary<string, string>();
                config.BuiltInTags.Add("arguments", "");
                config.BuiltInTags.Add("Array", "");
                config.BuiltInTags.Add("Binding", "source,destination");
                config.BuiltInTags.Add("Boolean", "");
                config.BuiltInTags.Add("Component", "className:s");
                config.BuiltInTags.Add("Metadata", "");
                config.BuiltInTags.Add("method", "name,concurrency,result:e,resultFormat,fault:e");
                config.BuiltInTags.Add("Model", "source");
                config.BuiltInTags.Add("Number", "");
                config.BuiltInTags.Add("Object", "");
                config.BuiltInTags.Add("operation", "name,concurrency,makeObjectsBindable,result:e,resultFormat,fault:e");
                config.BuiltInTags.Add("request", "");
                config.BuiltInTags.Add("Script", "source");
                config.BuiltInTags.Add("String", "");
                config.BuiltInTags.Add("Style", "source");
                config.BuiltInTags.Add("XML", "format,source");
                config.BuiltInTags.Add("XMLList", "");

                config.ForceMxNamespace = new string[] {
                    "Sprite", "Matrix", "Point",
                    "BevelFilter", "BlurFilter", "ColorMatrixFilter", "DisplacementFilter", "DropShadowFilter",
                    "GlowFilter", "GradientBevelFilter", "GradientGlowFilter"
                };

                config.LeafTags = new string[] {
                    "UIComponent"
                };
                config.ContainerTags = new string[] {
                    "Container", "IContainer"
                };
                GeneratorConfig.Serialize(inputFile, config);
            }
            
            log = File.CreateText(Path.Combine(Path.GetDirectoryName(inputFile), "log.txt"));
            output = File.CreateText(outputFile);

            // mx tags declared in SWCs
            mxTags = new HashSet<string>();
            foreach (string swcPath in config.FlexFrameworkSWCs)
            {
                string[] libs = Directory.GetFiles(swcPath, "*.swc");
                foreach (string lib in libs) ReadCatalog(lib);
            }

            // explore classpath
            context = new AS3Context.Context(new AS3Context.AS3Settings());
            groups = new Dictionary<string, TypeInfos>();
            ExploreNext();
            Console.ReadLine();
        }

        private static void ReadCatalog(string lib)
        {
            Log("> " + lib);
            ZipFile zfile = new ZipFile(lib);
            foreach (ZipEntry entry in zfile)
            {
                if (entry.Name == "catalog.xml")
                {
                    ParseCatalog(zfile.GetInputStream(entry));
                    break;
                }
            }
        }

        private static void ParseCatalog(Stream stream)
        {
            XmlTextReader reader = new XmlTextReader(stream);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "component")
                {
                    string name = reader.GetAttribute("className").Replace(':', '.');
                    mxTags.Add(name);
                    Log("include " + name);
                }
            }
        }

        private static void ExploreNext()
        {
            if (index >= config.Classpath.Length)
            {
                ForceMXNamespace();
                AddBuiltInTags();
                ExpandInheritance();
                BuildGroups();
                //AddBuiltInEvents(); // FD intrinsics now contain required information
                GenerateDeclarations();
                output.Close();
                log.Close();
                Console.WriteLine("Finished. Press Enter to continue.");
            }
            else
            {
                string path = config.Classpath[index++];
                Log("> " + path);
                pathModel = new PathModel(path, context);
                explorer = new PathExplorer(context, pathModel);
                explorer.Run();
                explorer.OnExplorationDone += new PathExplorer.ExplorationDoneHandler(explorer_OnExplorationDone);
            }
        }

        private static void ForceMXNamespace()
        {
            foreach (string cname in config.ForceMxNamespace)
            {
                if (groups.ContainsKey(cname))
                {
                    TypeInfos info = groups[cname];
                    info.ns = "mx";
                }
                else Console.WriteLine(String.Format("Force mx namespace on '{0}' failed - type not found", cname));
            }
        }

        private static void AddBuiltInTags()
        {
            foreach (string key in config.BuiltInTags.Keys)
                AddCustomTag(key, config.BuiltInTags[key]);
        }

        private static void AddCustomTag(string name, string attributes)
        {
            TypeInfos infos = new TypeInfos(name);
            infos.ns = "mx";
            infos.isLeaf = false;
            infos.isBuiltIn = true;
            infos.declAt = attributes;
            infos.isEmpty = attributes.Length == 0;
            infos.declInc = "id,@" + name;
            groups[name] = infos;
        }

        /*private static void AddBuiltInEvents()
        {
            // TODO add missing built-in events
            // http://livedocs.adobe.com/flash/9.0/ActionScriptLangRefV3/flash/events/EventDispatcher.html
            AddParams("EventDispatcher", "activate:e,deactivate:e");
            AddParams("DisplayObject", "added:e,addedToStage:e,enterFrame:e,removed:e,removedFromStage:e,render:e");
            AddParams("InteractiveObject",
                "click:e,contextMenu:e,doubleClick:e,focusIn:e,focusOut:e,"
                + "keyDown:e,keyFocusChange:e,keyUp:e,middleClick:e,middleMouseDown:e,middleMouseUp:e,"
                + "mouseDown:e,mouseFocusChange:e,mouseMove:e,mouseOut:e,mouseOver:e,mouseUp:e,"
                + "mouseWheel:e,nativeDragComplete:e,nativeDragDrop:e,nativeDragEnter:e,nativeDragExit:e,"
                + "nativeDragOver:e,nativeDragStart:e,nativeDragUpdate:e,"
                + "render:e,rightClick:e,rightMouseDown:e,rightMouseUp:e,rollOut:e,rollOver:e,"
                + "tabChildrenChange:e,tabEnabledChange:e,tabIndexChange");
            AddParams("URLLoader", "complete:e,httpResponseStatus:e,httpStatus:e,ioError:e,open:e,progress:e,securityError:e");
            AddParams("LoaderInfo", "complete:e,httpStatus:e,init:e,ioError:e,open:e,progress:e,unload:e");
            AddParams("Timer", "timer:e,timerComplete:e");
        }*/

        private static void AddParams(string tagName, string add)
        {
            if (groups.Keys.Contains(tagName))
            {
                TypeInfos info = groups[tagName];
                info.isEmpty = false;
                if (info.declAt == null) info.declAt = "";
                else if (info.declAt.Length > 0) info.declAt += ",";
                info.declAt += add;
            }
            else Console.WriteLine("AddParams failed: '" + tagName + "' not found");
        }

        private static void Log(string msg)
        {
            Console.WriteLine(msg);
            log.WriteLine(msg);
        }

        private static void GenerateDeclarations()
        {
            ListNames();
            output.WriteLine("<declarations>");
            GenerateGroupsDeclarations();
            GenerateTagsDeclarations();
            output.WriteLine("</declarations>");
        }

        private static void ListNames()
        {
            names = new List<string>(groups.Keys.Count);
            foreach (TypeInfos infos in groups.Values) names.Add(infos.name);
            names.Sort();
        }

        private static void GenerateGroupsDeclarations()
        {
            output.WriteLine("<groups>");
            foreach (string name in names)
            {
                TypeInfos infos = groups[name];
                if (infos.isEmpty) continue;
                output.WriteLine(String.Format("\t<group id=\"@{0}\" at=\"{1}\"/>", infos.name, infos.declAt));
            }
            output.WriteLine("</groups>");
        }

        private static void GenerateTagsDeclarations()
        {
            output.WriteLine("<tags defaultNS=\"mx\">");
            foreach (string name in names)
            {
                TypeInfos infos = groups[name];
                if (infos.ignore || infos.isAbstract) continue;
                string isLeaf = (GetIsLeaf(infos)) ? " isLeaf=\"yes\"" : "";
                output.WriteLine(String.Format("\t<{0} ns=\"{1}\"{2} at=\"{3}\"/>", 
                    infos.name, infos.ns, isLeaf, infos.declInc));
            }
            output.WriteLine("</tags>");
        }

        private static bool GetIsLeaf(TypeInfos infos)
        {
            if (!infos.isLeaf) 
                return false; // we already know it isn't a leaf
            TypeInfos extInfos = infos;
            while (extInfos != null)
            {
                if (!extInfos.isLeaf) return false;
                else if (config.LeafTags.Contains<string>(extInfos.name)) return true;
                extInfos = GetExtends(extInfos);
            }
            return false;
        }

        private static void ExpandInheritance()
        {
            foreach (TypeInfos infos in groups.Values)
            {
                TypeInfos extInfos = GetExtends(infos);
                while (extInfos != null)
                {
                    if (!extInfos.isEmpty) infos.includes.Add(extInfos);
                    extInfos = GetExtends(extInfos);
                }
            }
        }

        private static TypeInfos GetExtends(TypeInfos infos)
        {
            string ext = infos.extends;
            if (ext == null || ext.Length == 0 || ext == "Object" || ext == infos.name) return null;
            if (ext.IndexOf('.') > 0) ext = ext.Substring(ext.LastIndexOf('.') + 1);
            if (!groups.ContainsKey(ext)) return null;
            else return groups[ext];
        }

        private static void BuildGroups()
        {
            foreach (TypeInfos infos in groups.Values)
            {
                if (infos.isBuiltIn) 
                    continue;

                List<string> inc = new List<string>();
                inc.Add("id");
                foreach (TypeInfos subInfos in infos.includes)
                {
                    if (!subInfos.isEmpty) inc.Add("@" + subInfos.name);
                }
                List<string> ats = new List<string>();
                foreach (string at in infos.members) if (infos.excludes.IndexOf(at) < 0) ats.Add(at);
                foreach (string at in infos.events) ats.Add(at + ":e");
                foreach (string at in infos.styles) ats.Add(at + ":s");
                foreach (string at in infos.effects) ats.Add(at + ":x");
                if (ats.Count == 0)
                {
                    infos.isEmpty = true;
                    if (infos.isAbstract)
                    {
                        infos.ignore = true;
                        Log("ignore " + infos.name);
                        continue;
                    }
                }
                infos.declAt = String.Join(",", ats.ToArray());
                if (!infos.isEmpty) inc.Insert(1, "@" + infos.name);
                infos.declInc = String.Join(",", inc.ToArray());
            }
        }

        static void explorer_OnExplorationDone(string path)
        {
            foreach (FileModel model in pathModel.Files.Values)
            {
                ClassModel type = model.GetPublicClass();
                if (type.IsVoid() || (type.Flags & FlagType.Interface) > 0)
                {
                    // silently ignore
                    continue;
                }
                string package = model.Package;
                string typeName = type.Name;

                TypeInfos infos;
                if (groups.ContainsKey(typeName))
                {
                    Log("-- merging classes with same name: " + type.Name);
                    infos = groups[typeName]; // merge blocs with same name
                }
                else infos = new TypeInfos(type.Name);

                if (mxTags.Contains(type.QualifiedName))
                {
                    infos.ns = "mx";
                    infos.ignore = false;
                }
                else if (package.StartsWith("mx."))
                {
                    infos.ns = "mx";
                    infos.ignore = true;
                }
                else if ((type.Flags & FlagType.Interface) > 0 || package.Length == 0
                    || !config.IncludePackage.ContainsKey(package))
                {
                    Log("ignore " + type.QualifiedName);
                    continue;
                }
                else infos.ns = config.IncludePackage[package];

                if (type.ExtendsType != null && !type.ExtendsType.EndsWith(type.Name))
                    infos.extends = type.ExtendsType;
                infos.isAbstract = (type.Members.Search(type.Constructor, 0, 0) == null);

                if (CheckIsContainer(infos, type))
                    infos.isLeaf = false;

                if (!ReadMetaData(model.FileName, infos))
                {
                    Log("excluded " + type.QualifiedName);
                    continue;
                }
                AddMembers(infos, type);
                groups[type.Name] = infos;
            }
            ExploreNext();
        }

        private static bool CheckIsContainer(TypeInfos infos, ClassModel model)
        {
            if (config.ContainerTags.Contains<string>(infos.name))
                return true;
            if (model.Implements != null)
            {
                foreach (string cname in model.Implements)
                    if (config.ContainerTags.Contains<string>(cname))
                        return true;
            }
            return false;
        }

        private static void AddMembers(TypeInfos infos, ClassModel type)
        {
            FlagType mask = FlagType.Variable | FlagType.Setter;
            foreach (MemberModel member in type.Members)
            {
                if (member.Access == Visibility.Public
                    && (member.Flags & FlagType.Dynamic) > 0 && (member.Flags & mask) > 0
                    && infos.excludes.IndexOf(member.Name) < 0)
                {
                    string paramType = ((member.Flags & FlagType.Setter) > 0) ? member.Parameters[0].Type : member.Type;
                    if (paramType == "Array") AddSpecialTag(member);
                    else infos.members.Add(member.Name);
                }
            }
        }

        private static void AddSpecialTag(MemberModel member)
        {
            if (groups.ContainsKey(member.Name)) return;
            TypeInfos infos = new TypeInfos(member.Name);
            infos.isLeaf = false;
            infos.ns = "mx";
            groups[member.Name] = infos;
        }

        private static bool ReadMetaData(string fileName, TypeInfos infos)
        {
            string src = File.ReadAllText(fileName);
            if (src.IndexOf("[ExcludeClass]") > 0) 
                return false;
            MatchCollection eventMatches = reMatchEvents.Matches(src);
            MatchCollection stylesMatches = reMatchStyles.Matches(src);
            MatchCollection effectsMatches = reMatchEffects.Matches(src);
            MatchCollection exMatches = reMatchExcludes.Matches(src);
            Match defaultProp = reMatchDefaultProperty.Match(src);
            if (defaultProp.Success)
            {
                infos.excludes.Add(defaultProp.Groups[1].Value);
                infos.isLeaf = false;
            }
            foreach (Match m in eventMatches) infos.events.Add(m.Groups[2].Value);
            foreach (Match m in stylesMatches) infos.styles.Add(m.Groups[2].Value);
            foreach (Match m in effectsMatches) infos.effects.Add(m.Groups[2].Value);
            foreach (Match m in exMatches) infos.excludes.Add(m.Groups[2].Value);

            string path = Path.GetDirectoryName(fileName);
            MatchCollection incMatches = reMatchIncludes.Matches(src);
            foreach (Match m in incMatches)
            {
                TypeInfos subInfos = GetSubGroup(m.Groups[1].Value, fileName);
                if (subInfos == null) continue;
                else
                {
                    foreach (string p in subInfos.events) infos.events.Add(p);
                    foreach (string p in subInfos.styles) infos.styles.Add(p);
                    foreach (string p in subInfos.effects) infos.effects.Add(p);
                    foreach (string p in subInfos.members) infos.members.Add(p);
                }
            }
            return true;
        }

        private static TypeInfos GetSubGroup(string relPath, string fromFile)
        {
            string name = Path.GetFileNameWithoutExtension(relPath);
            if (name == "Version") return null;
            if (groups.ContainsKey(name)) return groups[name];

            string fileName = Path.Combine(Path.GetDirectoryName(fromFile), relPath);
            TypeInfos infos = new TypeInfos(name);
            infos.isAbstract = true;
            infos.ignore = true;
            ReadMetaData(fileName, infos);
            groups[name] = infos;
            return infos;
        }
    }

    class TypeInfos
    {
        public string name;
        public string ns;
        public string extends;
        public bool isLeaf = true;
        public bool isAbstract;
        public bool isEmpty;
        public bool isBuiltIn;
        public List<string> members = new List<string>();
        public List<string> events = new List<string>();
        public List<string> styles = new List<string>();
        public List<string> effects = new List<string>();
        public List<string> excludes = new List<string>();
        public List<TypeInfos> includes = new List<TypeInfos>();
        public bool ignore;
        public string declAt;
        public string declInc;

        public TypeInfos(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            if (extends != null) return name + " extends " + extends;
            else return name;
        }
    }
}
