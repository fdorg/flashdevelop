using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using SwfOp;
using SwfOp.Data;

namespace AS3Context
{
    #region AbcConverter class: ABC model builder

    public class AbcConverter
    {
        public static List<string> ExcludedASDocs = new() {"helpid", "keyword"};

        public static Regex reSafeChars = new Regex("[*\\:" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]", RegexOptions.Compiled);
        static readonly Regex reDocFile = new Regex("[/\\\\]([-_.$a-z0-9]+)\\.xml", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static Dictionary<string, Dictionary<string, ASDocItem>> Docs = new Dictionary<string, Dictionary<string, ASDocItem>>();

        static Dictionary<string, FileModel> genericTypes;
        static Dictionary<string, string> imports;
        static Dictionary<string, string> conflicts;
        static bool inSWF;
        static Dictionary<string, ASDocItem> thisDocs;
        static string docPath;

        /// <summary>
        /// Extract documentation from XML included in ASDocs-enriched SWCs
        /// </summary>
        static void ParseDocumentation(ContentParser parser)
        {
            if (parser.Catalog != null)
            {
                MxmlFilter.AddCatalogs(parser.Filename, parser.Catalog);
            }

            if (parser.Docs.Count > 0)
                foreach (string docFile in parser.Docs.Keys)
                {
                    if (docFile.EndsWithOrdinal(".dita.xml"))
                        continue;
                    try
                    {
                        var m = reDocFile.Match(docFile);
                        if (!m.Success) continue;
                        var package = m.Groups[1].Value;
                        if (!Docs.TryGetValue(package, out var packageDocs))
                            packageDocs = new Dictionary<string, ASDocItem>();
                        var rawDoc = parser.Docs[docFile];
                        var dr = new ASDocsReader(rawDoc) {ExcludedASDocs = ExcludedASDocs};
                        dr.Parse(packageDocs);
                        Docs[package] = packageDocs;
                    }
                    catch
                    {
                    }
                }
        }

        /// <summary>
        /// Create virtual FileModel objects from Abc bytecode
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="path"></param>
        /// <param name="context"></param>
        public static void Convert(ContentParser parser, PathModel path, IASContext context)
        {
            inSWF = Path.GetExtension(path.Path).ToLower() == ".swf";

            // extract documentation
            ParseDocumentation(parser);

            // extract models
            var models = new Dictionary<string, FileModel>();
            var privateClasses = new FileModel(Path.Combine(path.Path, "__Private.as"))
            {
                Version = 3,
                Package = "private"
            };
            genericTypes = new Dictionary<string, FileModel>();
            imports = new Dictionary<string, string>();
            conflicts = new Dictionary<string, string>();

            foreach (Abc abc in parser.Abcs)
            {
                // types
                foreach (Traits trait in abc.classes)
                {
                    Traits instance = trait.itraits;
                    if (instance is null) continue;
                    imports.Clear();
                    conflicts.Clear();

                    FileModel model = new FileModel("");
                    model.Context = context;
                    model.Package = reSafeChars.Replace(instance.name.uri, "_");
                    model.HasPackage = true;
                    string filename = reSafeChars.Replace(trait.name.ToString(), "_").TrimEnd('$');
                    model.FileName = Path.Combine(path.Path, model.Package.Replace('.', Path.DirectorySeparatorChar), filename);
                    model.Version = 3;

                    ClassModel type = new ClassModel();
                    model.Classes = new List<ClassModel> {type};

                    type.InFile = model;
                    type.Type = instance.name.ToTypeString();
                    type.Name = instance.name.localName;
                    type.Flags = FlagType.Class;
                    conflicts.Add(type.Name, type.QualifiedName);

                    if ((instance.flags & TraitFlag.Interface) > 0)
                        type.Flags |= FlagType.Interface;
                    else
                    {
                        if ((instance.flags & TraitFlag.Final) > 0)
                            type.Flags |= FlagType.Final;

                        if ((instance.flags & TraitFlag.Sealed) == 0)
                            type.Flags |= FlagType.Dynamic;

                    }

                    thisDocs = GetDocs(model.Package);
                    if (thisDocs != null)
                    {
                        docPath = (model.Package.Length > 0 ? model.Package + ":" : "globalClassifier:") + type.Name;
                        if (thisDocs.TryGetValue(docPath, out var doc))
                        {
                            ApplyASDoc(doc, type);
                            if (doc.Meta != null) type.MetaDatas = doc.Meta;
                        }
                        if (model.Package.Length == 0) docPath = type.Name;
                    }

                    type.ExtendsType = instance.baseName.uri == model.Package
                        ? ImportType(instance.baseName.localName)
                        : ImportType(instance.baseName);

                    if (!instance.interfaces.IsNullOrEmpty())
                    {
                        type.Implements = new List<string>();
                        foreach (QName name in instance.interfaces)
                            type.Implements.Add(ImportType(name));
                    }

                    if (model.Package == "private")
                    {
                        model.Package = "";
                        type.Access = Visibility.Private;
                        type.Namespace = "private";
                    }
                    else if (model.Package == "__AS3__.vec")
                    {
                        model.Package = "";
                        type.Access = Visibility.Private;
                        type.Namespace = "private";
                        string genType = type.Name;
                        if (type.Name.IndexOf('$') > 0)
                        {
                            string[] itype = type.Name.Split('$');
                            genType = itype[0];
                            type.Name = itype[0] + "$" + itype[1];
                            type.IndexType = itype[1];
                        }
                        if (genericTypes.TryGetValue(genType, out var inFile))
                        {
                            model.Classes.Clear();
                            type.InFile = inFile;
                            genericTypes[genType].Classes.Add(type);
                        }
                        else genericTypes[genType] = model;
                    }
                    else if (type.Name.StartsWith('_') && string.IsNullOrEmpty(model.Package))
                    {
                        type.Access = Visibility.Private;
                        type.Namespace = "private";
                    }
                    else
                    {
                        type.Access = Visibility.Public;
                        type.Namespace = "public";
                    }

                    type.Members = GetMembers(trait.members, FlagType.Static, instance.name);
                    type.Members.Add(GetMembers(instance.members, FlagType.Dynamic, instance.name));

                    if ((type.Flags & FlagType.Interface) > 0)
                    {
                        // TODO properly support interface multiple inheritance
                        type.ExtendsType = null;
                        if (!type.Implements.IsNullOrEmpty())
                        {
                            type.ExtendsType = type.Implements[0];
                            type.Implements.RemoveAt(0);
                            if (type.Implements.Count == 0) type.Implements = null;
                        }

                        foreach (MemberModel member in type.Members)
                        {
                            member.Access = Visibility.Public;
                            member.Namespace = "";
                        }
                    }

                    // constructor
                    if (instance.init != null && (type.Flags & FlagType.Interface) == 0)
                    {
                        List<MemberInfo> temp = new List<MemberInfo>(new MemberInfo[] { instance.init });
                        MemberList result = GetMembers(temp, 0, instance.name);
                        if (result.Count > 0)
                        {
                            MemberModel ctor = result[0];
                            ctor.Flags |= FlagType.Constructor;
                            ctor.Access = Visibility.Public;
                            ctor.Type = type.Type;
                            ctor.Namespace = "public";
                            type.Members.Merge(result);
                            type.Constructor = ctor.Name;
                        }
                    }
                    else type.Constructor = type.Name;

                    if (type.Access == Visibility.Private)
                    {
                        model = privateClasses;
                        type.InFile = model;
                    }

                    if (model.Classes.Count > 0 || model.Members.Count > 0)
                    {
                        AddImports(model, imports);
                        models[model.FileName] = model;
                    }
                }

                // packages
                if (abc.scripts is null) continue;
                foreach (var trait in abc.scripts)
                {
                    FileModel model = null;
                    foreach (var info in trait.members)
                    {
                        if (info.kind == TraitMember.Class) continue;
                        var member = GetMember(info, 0);
                        if (member is null) continue;
                        if (model is null || model.Package != info.name.uri)
                        {
                            AddImports(model, imports);
                            var package = info.name.uri ?? "";
                            var filename = package.Length > 0 ? "package.as" : "toplevel.as";
                            filename = Path.Combine(path.Path, package.Replace('.', Path.DirectorySeparatorChar), filename);
                            if (!models.TryGetValue(filename, out model))
                            {
                                model = new FileModel
                                {
                                    Context = context,
                                    Package = package,
                                    HasPackage = true,
                                    FileName = filename,
                                    Version = 3
                                };
                                models[filename] = model;
                            }
                        }

                        thisDocs = GetDocs(model.Package);
                        if (thisDocs != null)
                        {
                            docPath = "globalOperation:" + (model.Package.Length > 0 ? model.Package + ":" : "")
                                + member.Name;
                            if (member.Access == Visibility.Public && !string.IsNullOrEmpty(member.Namespace)
                                && member.Namespace != "public")
                                docPath += member.Namespace + ":";
                            if ((member.Flags & FlagType.Setter) > 0) docPath += ":set";
                            else if ((member.Flags & FlagType.Getter) > 0) docPath += ":get";
                            if (thisDocs.TryGetValue(docPath, out var doc)) ApplyASDoc(doc, member);
                        }
                        member.InFile = model;
                        member.IsPackageLevel = true;
                        model.Members.Add(member);
                    }
                    AddImports(model, imports);
                }
            }

            if (privateClasses.Classes.Count > 0) models[privateClasses.FileName] = privateClasses;

            // some SWCs need manual fixes
            CustomFixes(path.Path, models);

            // fake SWC (like 'playerglobal_rb.swc', only provides documentation)
            if (models.Keys.Count == 1)
            {
                foreach (FileModel model in models.Values)
                    if (model.GetPublicClass().QualifiedName == "Empty")
                    {
                        models.Clear();
                        break;
                    }
            }

            path.SetFiles(models);
        }

        /// <summary>
        /// old name: setDoc()
        /// </summary>
        static void ApplyASDoc(ASDocItem doc, MemberModel model)
        {
            model.Comments = doc.LongDesc;
            if (doc.IsFinal) model.Flags |= FlagType.Final;
            if (doc.IsDynamic && model is ClassModel) model.Flags |= FlagType.Dynamic;
            if (doc.Value != null) model.Value = doc.Value;
            // TODO  Extract features in comments
            ApplyTypeComment(doc, model);
            ApplyTypeCommentToParams(doc, model);
        }

        static void ApplyTypeComment(ASDocItem doc, MemberModel model)
        {
            if (doc is null || model  is null) return;
            ASFileParserUtils.ParseTypeDefinitionInto(doc.ApiType, model, true, true);
        }

        static void ApplyTypeCommentToParams(ASDocItem doc, MemberModel model)
        {
            if (doc is null || model?.Parameters is null) return;
            foreach (var param in model.Parameters)
                if (doc.ParamTypes != null && doc.ParamTypes.TryGetValue(param.Name, out var typeDefinition))
                    ASFileParserUtils.ParseTypeDefinitionInto(typeDefinition, param, true, true);
        }

        static void CustomFixes(string path, IDictionary<string, FileModel> models)
        {
            var file = Path.GetFileName(path);
            if (file != "playerglobal.swc" && file != "airglobal.swc") return;
            var mathPath = Path.Combine(path, "Math");
            if (models.TryGetValue(mathPath, out var model))
            {
                var @class = model.GetPublicClass();
                foreach (var member in @class.Members)
                {
                    if (!member.Parameters.IsNullOrEmpty() && member.Parameters[0].Name == "x")
                    {
                        string n = member.Name;
                        if (member.Parameters.Count > 1)
                        {
                            if (n == "atan2") member.Parameters.Reverse();
                            else if (n == "min" || n == "max") { member.Parameters[0].Name = "val1"; member.Parameters[1].Name = "val2"; }
                            else if (n == "pow") { member.Parameters[0].Name = "base"; member.Parameters[1].Name = "pow"; }
                        }
                        else if (n == "sin" || n == "cos" || n == "tan") member.Parameters[0].Name = "angleRadians";
                        else member.Parameters[0].Name = "val";
                    }
                }
            }
            var objPath = Path.Combine(path, "Object");
            if (models.TryGetValue(objPath, out model))
            {
                var @class = model.GetPublicClass();
                if (!@class.Members.Contains("prototype"))
                {
                    @class.Members.Add(new MemberModel("prototype", "Object", FlagType.Dynamic | FlagType.Variable, Visibility.Public));
                }
            }
        }

        static void AddImports(FileModel model, Dictionary<string, string> imports)
        {
            if (model is null) return;
            foreach (var pair in imports)
                model.Imports.Add(new MemberModel(pair.Value, pair.Key, FlagType.Import, 0));
            imports.Clear();
        }

        static Dictionary<string, ASDocItem> GetDocs(string package)
        {
            var docPackage = package == "" ? "__Global__" : package;
            Docs.TryGetValue(docPackage, out var result);
            return result;
        }

        static MemberList GetMembers(IEnumerable<MemberInfo> abcMembers, FlagType baseFlags, QName instName)
        {
            MemberList list = new MemberList();
            string package = instName.uri;
            string protect = instName.ToString();

            foreach (MemberInfo info in abcMembers)
            {
                MemberModel member = GetMember(info, baseFlags);
                if (member is null) continue;

                string uri = info.name.uri ?? "";
                if (uri.Length > 0)
                {
                    if (uri == "private" || package == "private") continue;
                    if (uri == protect)
                    {
                        member.Access = Visibility.Protected;
                        member.Namespace = "protected";
                    }
                    else if (uri == package)
                    {
                        member.Access = Visibility.Internal;
                        member.Namespace = "internal";
                    }
                    else if (uri == "http://adobe.com/AS3/2006/builtin")
                    {
                        member.Access = Visibility.Public;
                        member.Namespace = "AS3";
                    }
                    else if (uri == "http://www.adobe.com/2006/flex/mx/internal")
                    {
                        continue;
                    }
                    else if (uri == "http://www.adobe.com/2006/actionscript/flash/proxy")
                    {
                        member.Access = Visibility.Public;
                        member.Namespace = "flash_proxy";
                    }
                    else if (uri == "http://www.adobe.com/2006/actionscript/flash/objectproxy")
                    {
                        member.Access = Visibility.Public;
                        member.Namespace = "object_proxy";
                    }
                    else // unknown namespace
                    {
                        member.Access = Visibility.Public;
                        member.Namespace = "internal";
                    }
                }

                if (thisDocs != null) GetMemberDoc(member);

                list.Add(member);
            }
            return list;
        }

        static MemberModel GetMember(MemberInfo info, FlagType baseFlags)
        {
            var member = new MemberModel
            {
                Name = info.name.localName,
                Flags = baseFlags,
                Access = Visibility.Public,
                Namespace = "public"
            };
            if (!info.metadata.IsNullOrEmpty())
            {
                var metadatas = member.MetaDatas;
                foreach (var metaInfo in info.metadata)
                {
                    if (metaInfo.name == "__go_to_definition_help") continue;
                    var meta = new ASMetaData(metaInfo.name);
                    var rawParams = new StringBuilder();
                    meta.Params = new Dictionary<string, string>(metaInfo.Count);
                    foreach (var entry in metaInfo)
                    {
                        if (entry.Length != 2) continue;
                        meta.Params[entry[0]] = entry[1];
                        if (rawParams.Length > 0) rawParams.Append(",");
                        rawParams.Append(entry[0] + "=\"" + entry[1] + "\"");
                    }
                    meta.RawParams = rawParams.ToString();

                    metadatas ??= new List<ASMetaData>(info.metadata.Count);
                    metadatas.Add(meta);
                }
                member.MetaDatas = metadatas;
            }

            if (info is SlotInfo slot)
            {
                member.Flags |= FlagType.Variable;
                if (slot.kind == TraitMember.Const) member.Flags |= FlagType.Constant;
                if (slot.value is Namespace ns)
                {
                    member.Flags |= FlagType.Namespace;
                    member.Value = '"' + ns.uri + '"';
                }
                member.Type = ImportType(slot.type);
            }
            else if (info is MethodInfo method)
            {
                member.Flags |= method.kind switch
                {
                    TraitMember.Setter => FlagType.Setter,
                    TraitMember.Getter => FlagType.Getter,
                    _ => FlagType.Function,
                };
                QName type = method.returnType;
                member.Type = ImportType(type);

                member.Parameters = new List<MemberModel>();
                int n = method.paramTypes.Length;
                int defaultValues = n - method.optionalValues?.Length ?? n;
                for (int i = 0; i < n; i++)
                {
                    MemberModel param = new MemberModel();
                    param.Flags = FlagType.ParameterVar | FlagType.Variable;
                    param.Name = (!inSWF && method.paramNames != null) ? method.paramNames[i] : "param" + i;
                    type = method.paramTypes[i];
                    param.Type = ImportType(type);

                    if (param.Name[0] == '.' && param.Type == "Array") // ...rest
                    {
                        param.Type = "";
                    }
                    else if (i >= defaultValues)
                    {
                        SetDefaultValue(param, method.optionalValues[i - defaultValues]);
                    }
                    member.Parameters.Add(param);
                }
            }
            else member = null;
            return member;
        }

        static void GetMemberDoc(MemberModel member)
        {
            string dPath = docPath + ":";
            if (member.Access == Visibility.Public && !string.IsNullOrEmpty(member.Namespace)
                && member.Namespace != "public")
                dPath += member.Namespace + ":";
            dPath += member.Name;
            if ((member.Flags & FlagType.Getter) > 0) dPath += ":get";
            else if ((member.Flags & FlagType.Setter) > 0) dPath += ":set";
            if (thisDocs.TryGetValue(dPath, out var doc)) ApplyASDoc(doc, member);
        }

        static string ImportType(QName type) => type is null ? "*" : ImportType(type.ToTypeString());

        static string ImportType(string qname)
        {
            if (qname is null) return "*";
            int p = qname.LastIndexOf('.');
            int q = qname.LastIndexOf('<');
            if (q > 0)
            {
                p = qname.IndexOf('>', q);
                if (p <= q) return qname;
                return qname.Substring(0, q + 1) + ImportType(qname.Substring(q + 1, p - q - 1)) + qname.Substring(p);
            }
            if (p < 0) return qname;
            if (imports.TryGetValue(qname, out var import)) return import;
            string cname = qname.Substring(p + 1);
            if (!conflicts.ContainsKey(cname)) conflicts.Add(cname, qname);
            else if (conflicts[cname] != qname) 
                cname = qname; // ambiguity
            imports[qname] = cname;
            return cname;
        }

        static void SetDefaultValue(MemberModel member, object value)
        {
            member.Value = value switch
            {
                null => "null",
                string _ when value.ToString() != "undefined" => '"' + value.ToString() + '"',
                bool _ => value.ToString().ToLower(),
                double d => d.ToString(CultureInfo.InvariantCulture.NumberFormat),
                _ => value.ToString(),
            };
        }
    }

    #endregion

    #region ASDocItem class: documented values container

    public class ASDocItem
    {
        public bool IsFinal;
        public bool IsDynamic;
        public bool IsStatic;

        public string ShortDesc;
        public string LongDesc;
        public string Returns;
        public string Value;
        public string ApiType;
        public string DeclType;

        public List<ASMetaData> Meta;
        public Dictionary<string, string> Params = new Dictionary<string, string>();
        public Dictionary<string, string> ParamTypes = new Dictionary<string, string>();
        public List<KeyValuePair<string, string>> ExtraAsDocs = new List<KeyValuePair<string, string>>();
    }

    #endregion

    #region ASDocsReader class: documentation parser

    class ASDocsReader : XmlTextReader
    {
        public List<string> ExcludedASDocs;
        Dictionary<string, ASDocItem> docs;


        public ASDocsReader(byte[] raw)
            : base(new MemoryStream(raw))
        {
            WhitespaceHandling = WhitespaceHandling.None;
        }

        public void Parse(Dictionary<string, ASDocItem> packageDocs)
        {
            docs = packageDocs;

            ASDocItem doc = new ASDocItem();
            MoveToContent();
            while (Read())
                ProcessDeclarationNodes(doc);

            docs = null;
        }


        //---------------------------
        //  PRIMARY
        //---------------------------

        void ReadDeclaration(string declType)
        {
            if (IsEmptyElement) return;
            ExcludedASDocs ??= new List<string>();
            var doc = new ASDocItem {DeclType = declType};
            var id = GetAttribute("id");
            if (id != null)
            {
                // type doubled in doc: "flash.utils:IDataOutput:flash.utils:IDataOutput:writeDouble"
                int colon = id.IndexOf(':') + 1;
                if (colon > 0)
                {
                    int dup = id.IndexOfOrdinal(id.Substring(0, colon), colon);
                    if (dup > 0) id = id.Substring(dup);
                }
                doc.ApiType = id;
            }

            string eon = Name;
            ReadStartElement();
            while (Name != eon)
            {
                ProcessDeclarationNodes(doc);
                Read();
            }

            if (id != null)
            {
                if (doc.ApiType == "String" && doc.Value != null && !doc.Value.StartsWith('"'))
                    doc.Value = "\"" + doc.Value + "\"";

                doc.LongDesc ??= "";

                if (doc.ShortDesc is null) doc.ShortDesc = doc.LongDesc;
                else doc.LongDesc = doc.LongDesc.Trim();

                if (doc.LongDesc.Length == 0 && doc.ShortDesc.Length > 0)
                    doc.LongDesc = doc.ShortDesc;

                if (!ExcludedASDocs.Contains("param") && doc.Params != null)
                    foreach (string name in doc.Params.Keys)
                        doc.LongDesc += "\n@param\t" + name + "\t" + doc.Params[name].Trim();

                if (!ExcludedASDocs.Contains("return") && doc.Returns != null)
                    doc.LongDesc += "\n@return\t" + doc.Returns.Trim();

                if (doc.ExtraAsDocs != null)
                    foreach (var extraASDoc in doc.ExtraAsDocs)
                        if (!ExcludedASDocs.Contains(extraASDoc.Key))
                            doc.LongDesc += "\n@" + extraASDoc.Key + "\t" + extraASDoc.Value;

                // keep definitions including either documentation or static values
                if (doc.ShortDesc.Length > 0 || doc.LongDesc.Length > 0
                    || (doc.IsStatic && doc.Value != null && doc.DeclType == "apiValue"))
                    docs[id] = doc;
            }
        }

        void ProcessDeclarationNodes(ASDocItem doc)
        {
            if (NodeType != XmlNodeType.Element) return;
            switch (Name)
            {
                case "apiName": break; // TODO validate event name
                case "apiInheritDoc": break; // TODO link inherited doc?
                case "apiDetail":
                case "related-links": SkipContents(); break;

                case "apiClassifierDetail":
                    ReadApiClassifierDetail(doc);
                    break;

                case "apiClassifier":
                case "apiValue":
                case "apiOperation":
                case "apiConstructor":
                    ReadDeclaration(Name);
                    break;

                case "shortdesc": doc.ShortDesc = ReadValue(); break;
                case "prolog": ReadProlog(doc); break;
                case "apiDesc": doc.LongDesc = ReadValue(); break;
                case "apiData": doc.Value = ReadValue(); break;

                case "style": ReadStyleMeta(doc); break;
                case "Exclude": ReadExcludeMeta(doc); break;
                case "adobeApiEvent": ReadEventMeta(doc); break;

                case "apiFinal": doc.IsFinal = true; SkipContents(); break;
                case "apiStatic": 
                    doc.IsStatic = true; break;

                case "apiParam": ReadParamDesc(doc); break;
                case "apiReturn": ReadReturnsDesc(doc); break;
                case "apiException": ReadApiException(doc); break; // TODO link inherited doc?

                case "apiType": ReadApiType(doc); break;

                case "apiValueClassifier":
                case "apiOperationClassifier": ReadApiTypeAsClassifier(doc); break;
            }
        }


        //---------------------------
        //  COMMONS
        //---------------------------

        void SkipContents()
        {
            if (IsEmptyElement)
                return;

            string eon = Name;
            ReadStartElement();
            while (Name != eon)
                Read();
        }

        string ReadValue()
        {
            if (IsEmptyElement)
            {
                string see = GetAttribute("conref");
                if (see != null) return "@see " + see;
                return "";
            }

            string desc = "";

            string prefix = "";
            string postfix = "";
            string eon = Name;
            ReadStartElement();
            while (Name != eon)
            {
                var lcName = Name.ToLower(); // name in lower case
                if (lcName == "codeblock" || lcName == "listing")
                {
                    if (NodeType == XmlNodeType.Element)
                    {
                        prefix = "\n<" + lcName + ">\n";
                        postfix = "\n</" + lcName + ">\n";
                    }
                    else
                    {
                        prefix = "";
                        postfix = "";
                    }
                }

                switch (NodeType)
                {
                    case XmlNodeType.Element:
                        ReadStartElement();
                        break;

                    case XmlNodeType.EndElement:
                        ReadEndElement();
                        break;

                    case XmlNodeType.Text:
                        desc += prefix + ReadString() + postfix;
                        break;

                    default: Read(); break;
                }
            }
            return desc;
        }


        //---------------------------
        //  apiClassifierDetail
        //---------------------------

        void ReadApiClassifierDetail(ASDocItem doc)
        {
            doc.LongDesc = "";

            if (IsEmptyElement)
                return;

            string eon = Name;
            Read();
            while (Name != eon)
            {
                switch (Name)
                {
                    case "apiClassifierDef":
                        ReadApiClassifierDef(doc);
                        Read();
                        break;

                    case "apiDesc":
                        doc.LongDesc += ReadInnerXml() +"\n";
                    //    Read();
                        break;

                    case "example":
                        doc.LongDesc += "\nEXAMPLE: \n\n" + ReadInnerXml() +"\n";
                    //    Read();
                        break;

                    default:
                        ReadInnerXml();
                        break;
                }
            }
        }

        void ReadApiClassifierDef(ASDocItem doc)
        {
            if (IsEmptyElement)
                return;

            string eon = Name;
            ReadStartElement();
            while (Name != eon)
            {
                if (Name == "apiFinal")
                    doc.IsFinal = true;
                else if (Name == "apiDynamic")
                    doc.IsDynamic = true;

                Read();
            }
        }


        //---------------------------
        //  prolog
        //---------------------------

        /// <summary>
        /// ---
        /// Example:
        /// <prolog>
        ///     <asMetadata>
        ///         <apiVersion>
        ///             <apiLanguage version="3.0" />
        ///             <apiPlatform description="" name="Flash" version="10" />
        ///             <apiPlatform description="" name="AIR" version="1.5" />
        ///             <apiTool description="" name="Flex" version="3" />
        ///         </apiVersion>
        ///     </asMetadata>
        ///     <asCustoms>
        ///         <customAsDoc>
        ///             <type c="String" />
        ///         </customAsDoc>
        ///     </asCustoms>
        /// </prolog>
        /// ---
        /// </summary>
        /// <param name="doc"></param>
        void ReadProlog(ASDocItem doc)
        {
            if (IsEmptyElement)
                return;

            string eon = Name;
            ReadStartElement();
            while (Name != eon)
            {
                if (Name == "asMetadata")
                    ReadPrologMetadata(doc);
                else if (Name == "asCustoms")
                    ReadPrologCustoms(doc, Name);

                Read();
            }
        }

        void ReadPrologMetadata(ASDocItem doc)
        {
            if (IsEmptyElement)
                return;

            string eon = Name;
            ReadStartElement();
            while (Name != eon)
            {
                if (Name == "apiVersion")
                    ReadPrologMetadataApiVersion(doc);
                else if (Name == "styles")
                    ReadPrologMetadataStyles(doc);
                else if (Name == "DefaultProperty")
                    ReadPrologMetadataDefaultProperty(doc);
                Read();
            }
        }

        void ReadPrologMetadataApiVersion(ASDocItem doc)
        {
            if (IsEmptyElement)
                return;

            string eon = Name;
            ReadStartElement();
            while (Name != eon)
            {
                string asdocVal;
                string asdocKey;
                if (Name == "apiLanguage")
                {
                    string sVers = GetAttribute("version");

                    asdocKey = "langversion";
                    asdocVal = sVers;

                    doc.ExtraAsDocs.Add(new KeyValuePair<string, string>(asdocKey, asdocVal));
                }
                else if (Name == "apiPlatform")
                {
                    string sDesc = GetAttribute("description");
                    string sName = GetAttribute("name");
                    string sVers = GetAttribute("version");

                    asdocKey = "playerversion";
                    asdocVal = sName + " " + sVers + "  " + sDesc;

                    doc.ExtraAsDocs.Add(new KeyValuePair<string, string>(asdocKey, asdocVal));
                }
                else if (Name == "apiTool")
                {
                    string sDesc = GetAttribute("description");
                    string sName = GetAttribute("name");
                    string sVers = GetAttribute("version");

                    asdocKey = "productversion";
                    asdocVal = sName + " " + sVers + "  " + sDesc;

                    doc.ExtraAsDocs.Add(new KeyValuePair<string, string>(asdocKey, asdocVal));
                }

                Read();
            }
        }

        void ReadPrologMetadataStyles(ASDocItem doc)
        {
            if (IsEmptyElement)
                return;

            string eon = Name;
            ReadStartElement();
            while (Name != eon)
            {
                if (Name == "style")
                    ReadStyleMeta(doc);
                Read();
            }
        }

        void ReadPrologMetadataDefaultProperty(ASDocItem doc)
        {
            ASMetaData meta = new ASMetaData("DefaultProperty");
            meta.Kind = ASMetaKind.DefaultProperty;
            meta.Comments = "";

            meta.Params = new Dictionary<string, string>();

            string defValue = GetAttribute("name");
            meta.Params["default"] = defValue;

            meta.RawParams = $"\"{defValue}\"";

            doc.Meta ??= new List<ASMetaData>();
            doc.Meta.Add(meta);
        }

        void ReadPrologCustoms(ASDocItem doc, string terminationNode)
        {
            if (IsEmptyElement)
                return;

            string eon = terminationNode;
            ReadStartElement();
            while (!(Name == eon && NodeType == XmlNodeType.EndElement))
            {
                var asdocKey = Name;

                /*
                if (asdocKey == "maelexample")
                {
                    asdocVal = this.ReadValue();
                    Read();
                }
                else
                {
                    */
                    var asdocVal = ReadInnerXml();
              //  }

                doc.ExtraAsDocs.Add(new KeyValuePair<string, string>(asdocKey, asdocVal));
            }
        }


        //---------------------------
        //  apiType
        //---------------------------

        void ReadApiType(ASDocItem doc) => SetApiType(doc, GetAttribute("value"));

        void ReadApiTypeAsClassifier(ASDocItem doc) => SetApiType(doc, ReadValue());

        static void SetApiType(ASDocItem doc, string apiType) => doc.ApiType = apiType == "any" ? "*" : apiType;
        
        //---------------------------
        //  apiOperationDetail
        //---------------------------

        void ReadParamDesc(ASDocItem doc)
        {
            if (IsEmptyElement)
                return;

            string name = null;
            string desc = null;
            string type = null;

            string eon = Name;
            ReadStartElement();
            while (Name != eon)
            {
                if (NodeType == XmlNodeType.Element)
                {
                    switch (Name)
                    {
                        case "apiItemName":
                            name = ReadValue();
                            break;

                        case "apiDesc":
                            desc = ReadValue();
                            break;

                        case "apiType":
                            type = GetAttribute("value");
                            break;
                    }
                }
                Read();
            }

            if (name != null)
            {
                if (desc != null)
                    doc.Params[name] = desc;

                if (type != null)
                    doc.ParamTypes[name] = type;
            }
        }

        void ReadReturnsDesc(ASDocItem doc)
        {
            if (IsEmptyElement)
                return;

            string eon = Name;
            ReadStartElement();
            while (Name != eon)
            {
                switch (Name)
                {
                    case "apiDesc":
                        doc.Returns = ReadValue();
                        break;

                    case "apiType":
                        ReadApiType(doc);
                        break;

                    case "apiValueClassifier":
                    case "apiOperationClassifier":
                        ReadApiTypeAsClassifier(doc);
                        break;
                }
                Read();
            }
        }


        //---------------------------
        //  apiException
        //---------------------------

        void ReadApiException(ASDocItem doc)
        {
            if (IsEmptyElement)
                return;

            string apiDesc = "";
            string apiItemName = "";

            string eon = Name;
            ReadStartElement();
            while (Name != eon)
            {
                switch (Name)
                {
                    case "apiDesc":
                        apiDesc = ReadValue();
                        break;

                    case "apiItemName":
                        apiItemName = ReadValue();
                        break;

                    case "apiOperationClassifier":
                        ReadValue();
                        break;
                }
                Read();
            }

            doc.ExtraAsDocs.Add(new KeyValuePair<string, string>("throws", apiItemName + " " + apiDesc));
        }


        //---------------------------
        //  Meta tags
        //---------------------------

        void ReadExcludeMeta(ASDocItem doc)
        {
            if (!HasAttributes) return;

            ASMetaData meta = new ASMetaData("Style") {Kind = ASMetaKind.Exclude};
            string sKind = GetAttribute("kind");
            string sName = GetAttribute("name");

            doc.Meta ??= new List<ASMetaData>();
            meta.Params = new Dictionary<string, string> {["kind"] = sKind, ["name"] = sName};
            meta.RawParams = $"kind=\"{sKind}\", name=\"{sName}\"";
            doc.Meta.Add(meta);
        }

        void ReadStyleMeta(ASDocItem doc)
        {
            if (IsEmptyElement || !HasAttributes) return;

            ASMetaData meta = new ASMetaData("Style");
            meta.Kind = ASMetaKind.Style;
            meta.Comments = "";

            string sName = GetAttribute("name");
            string sType = GetAttribute("type");
            string sInherit = GetAttribute("inherit");
            //string sFormat = GetAttribute("format");
            string sEnum = GetAttribute("enumeration");
            string sDefault = null;

            string eon = Name;
            ReadStartElement();
            while (Name != eon)
            {
                if (NodeType == XmlNodeType.Element)
                    switch (Name)
                    {
                        case "description": meta.Comments = ReadValue() ?? ""; break;
                        case "default": sDefault = ReadValue(); break;
                    }
                Read();
            }

            doc.Meta ??= new List<ASMetaData>();
            if (sDefault != null) meta.Comments = meta.Comments.Trim() + "\n@default\t" + sDefault;
            meta.Params = new Dictionary<string, string> {["name"] = sName, ["type"] = sType};
            meta.RawParams = $"name=\"{sName}\", type=\"{sType}\"";
            if (sInherit != null)
            {
                meta.Params["inherit"] = sInherit;
                meta.RawParams += ", inherit=\"" + sInherit + "\"";
            }
            if (sEnum != null)
            {
                meta.Params["enumeration"] = sEnum;
                meta.RawParams += ", enumeration=\"" + sEnum + "\"";
            }
            doc.Meta.Add(meta);
        }

        void ReadEventMeta(ASDocItem doc)
        {
            if (IsEmptyElement) return;

            ASMetaData meta = new ASMetaData("Event");
            meta.Kind = ASMetaKind.Event;
            meta.Comments = "";
            string eName = null;
            string eType = null;
            string eFullType = null;

            string eon = Name;
            ReadStartElement();
            while (Name != eon)
            {
                if (NodeType == XmlNodeType.Element)
                    switch (Name)
                    {
                        case "shortdesc": meta.Comments = ReadValue() ?? ""; break;
                        case "apiDesc": if (meta.Comments == "") meta.Comments = ReadValue() ?? ""; break;
                        case "apiName": eName = ReadValue(); break;
                        case "adobeApiEventClassifier": eType = ReadValue().Replace(':', '.'); break;
                        case "apiEventType": eFullType = ReadValue(); break;
                    }
                Read();
            }

            doc.Meta ??= new List<ASMetaData>();
            meta.Params = new Dictionary<string, string> {["name"] = eName, ["type"] = eType};
            if (eFullType != null)
                meta.Comments = meta.Comments.Trim() + "\n@eventType\t" + eFullType.Replace(':', '.');
            meta.RawParams = $"name=\"{eName}\", type=\"{eType}\"";
            doc.Meta.Add(meta);
        }
    }

    #endregion
}
