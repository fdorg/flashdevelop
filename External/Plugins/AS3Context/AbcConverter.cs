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
        static public List<string> ExcludedASDocs = getDefaultExcludedASDocs();

        static public Regex reSafeChars = new Regex("[*\\:" + Regex.Escape(new String(Path.GetInvalidPathChars())) + "]", RegexOptions.Compiled);
        static private Regex reDocFile = new Regex("[/\\\\]([-_.$a-z0-9]+)\\.xml", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        static public Dictionary<string, Dictionary<string, ASDocItem>> Docs = new Dictionary<string, Dictionary<string, ASDocItem>>();

        private static Dictionary<string, FileModel> genericTypes;
        private static Dictionary<string, string> imports;
        private static Dictionary<string, string> conflicts;
        private static bool inSWF;
        private static Dictionary<string, ASDocItem> thisDocs;
        private static string docPath;

        ///
        private static List<string> getDefaultExcludedASDocs()
        {
            List<string> list = new List<string>();
            list.Add("helpid");
            list.Add("keyword");
            return list;
        }

        /// <summary>
        /// Extract documentation from XML included in ASDocs-enriched SWCs
        /// </summary>
        /// <param name="rawDocs"></param>
        private static void ParseDocumentation(ContentParser parser)
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
                        Match m = reDocFile.Match(docFile);
                        if (!m.Success) continue;
                        string package = m.Groups[1].Value;
                        Dictionary<string, ASDocItem> packageDocs = Docs.ContainsKey(package)
                            ? Docs[package]
                            : new Dictionary<string, ASDocItem>();

                        byte[] rawDoc = parser.Docs[docFile];
                        ASDocsReader dr = new ASDocsReader(rawDoc);
                        dr.ExcludedASDocs = ExcludedASDocs;
                        dr.Parse(packageDocs);

                        Docs[package] = packageDocs;
                    }
                    catch (Exception)
                    {
                    }
                }
        }

        /// <summary>
        /// Create virtual FileModel objects from Abc bytecode
        /// </summary>
        /// <param name="abcs"></param>
        /// <param name="path"></param>
        /// <param name="context"></param>
        public static void Convert(ContentParser parser, PathModel path, IASContext context)
        {
            inSWF = Path.GetExtension(path.Path).ToLower() == ".swf";

            // extract documentation
            ParseDocumentation(parser);

            // extract models
            Dictionary<string, FileModel> models = new Dictionary<string, FileModel>();
            FileModel privateClasses = new FileModel(Path.Combine(path.Path, "__Private.as"));
            privateClasses.Version = 3;
            privateClasses.Package = "private";
            genericTypes = new Dictionary<string, FileModel>();
            imports = new Dictionary<string, string>();
            conflicts = new Dictionary<string, string>();

            foreach (Abc abc in parser.Abcs)
            {
                // types
                foreach (Traits trait in abc.classes)
                {
                    Traits instance = trait.itraits;
                    if (instance == null)
                        continue;
                    imports.Clear();
                    conflicts.Clear();

                    FileModel model = new FileModel("");
                    model.Context = context;
                    model.Package = reSafeChars.Replace(instance.name.uri, "_");
                    model.HasPackage = true;
                    string filename = reSafeChars.Replace(trait.name.ToString(), "_").TrimEnd('$');
                    filename = Path.Combine(model.Package.Replace('.', Path.DirectorySeparatorChar), filename);
                    model.FileName = Path.Combine(path.Path, filename);
                    model.Version = 3;

                    ClassModel type = new ClassModel();
                    model.Classes = new List<ClassModel>();
                    model.Classes.Add(type);

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
                        if (thisDocs.ContainsKey(docPath))
                        {
                            ASDocItem doc = thisDocs[docPath];
                            applyASDoc(doc, type);
                            if (doc.Meta != null) type.MetaDatas = doc.Meta;
                        }
                        if (model.Package.Length == 0) docPath = type.Name;
                    }

                    if (instance.baseName.uri == model.Package)
                        type.ExtendsType = ImportType(instance.baseName.localName);
                    else type.ExtendsType = ImportType(instance.baseName);

                    if (instance.interfaces != null && instance.interfaces.Length > 0)
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
                        if (genericTypes.ContainsKey(genType))
                        {
                            model.Classes.Clear();
                            type.InFile = genericTypes[genType];
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
                        if (type.Implements != null && type.Implements.Count > 0)
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
                        result = null;
                        temp = null;
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
                if (abc.scripts == null)
                    continue;
                foreach (Traits trait in abc.scripts)
                {
                    FileModel model = null;
                    foreach (MemberInfo info in trait.members)
                    {
                        if (info.kind == TraitMember.Class)
                            continue;

                        MemberModel member = GetMember(info, 0);
                        if (member == null) continue;

                        if (model == null || model.Package != info.name.uri)
                        {
                            AddImports(model, imports);

                            string package = info.name.uri ?? "";
                            string filename = package.Length > 0 ? "package.as" : "toplevel.as";
                            filename = Path.Combine(package.Replace('.', Path.DirectorySeparatorChar), filename);
                            filename = Path.Combine(path.Path, filename);
                            if (models.ContainsKey(filename))
                                model = models[filename];
                            else
                            {
                                model = new FileModel("");
                                model.Context = context;
                                model.Package = package;
                                model.HasPackage = true;
                                model.FileName = filename;
                                model.Version = 3;
                                models[filename] = model;
                            }
                        }

                        thisDocs = GetDocs(model.Package);
                        if (thisDocs != null)
                        {
                            docPath = "globalOperation:" + (model.Package.Length > 0 ? model.Package + ":" : "")
                                + member.Name;
                            if (member.Access == Visibility.Public && !String.IsNullOrEmpty(member.Namespace)
                                && member.Namespace != "public")
                                docPath += member.Namespace + ":";
                            if ((member.Flags & FlagType.Setter) > 0) docPath += ":set";
                            else if ((member.Flags & FlagType.Getter) > 0) docPath += ":get";

                            if (thisDocs.ContainsKey(docPath)) applyASDoc(thisDocs[docPath], member);
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
        private static void applyASDoc(ASDocItem doc, MemberModel model)
        {
            model.Comments = doc.LongDesc;

            if (doc.IsFinal)
                model.Flags |= FlagType.Final;

            if (doc.IsDynamic && (model is ClassModel))
                model.Flags |= FlagType.Dynamic;

            if (doc.Value != null)
                model.Value = doc.Value;

            // TODO  Extract features in comments
            applyTypeComment(doc, model);
            applyTypeCommentToParams(doc, model);
        }

        private static void applyTypeComment(ASDocItem doc, MemberModel model)
        {
            if (doc == null || model ==null)
                return;

            ASFileParserUtils.ParseTypeDefinitionInto(doc.ApiType, model, true, true);
        }

        private static void applyTypeCommentToParams(ASDocItem doc, MemberModel model)
        {
            if (doc == null || model == null || model.Parameters == null)
                return;

            foreach (MemberModel param in model.Parameters)
                if (doc.ParamTypes != null && doc.ParamTypes.ContainsKey(param.Name))
                    ASFileParserUtils.ParseTypeDefinitionInto(doc.ParamTypes[param.Name], param, true, true);
        }

        private static void CustomFixes(string path, Dictionary<string, FileModel> models)
        {
            string file = Path.GetFileName(path);
            if (file == "playerglobal.swc" || file == "airglobal.swc")
            {
                string mathPath = Path.Combine(path, "Math");
                if (models.ContainsKey(mathPath))
                {
                    ClassModel mathModel = models[mathPath].GetPublicClass();
                    foreach (MemberModel member in mathModel.Members)
                    {
                        if (member.Parameters != null && member.Parameters.Count > 0 && member.Parameters[0].Name == "x")
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
                string objPath = Path.Combine(path, "Object");
                if (models.ContainsKey(objPath))
                {
                    ClassModel objModel = models[objPath].GetPublicClass();
                    if (objModel.Members.Search("prototype", 0, 0) == null)
                    {
                        MemberModel proto = new MemberModel("prototype", "Object", FlagType.Dynamic | FlagType.Variable, Visibility.Public);
                        objModel.Members.Add(proto);
                    }
                }
            }
        }

        private static void AddImports(FileModel model, Dictionary<string, string> imports)
        {
            if (model != null)
            {
                foreach (string import in imports.Keys)
                    model.Imports.Add(new MemberModel(imports[import], import, FlagType.Import, 0));
                
                imports.Clear();
            }
        }

        private static Dictionary<string, ASDocItem> GetDocs(string package)
        {
            string docPackage = package == "" ? "__Global__" : package;
            if (Docs.ContainsKey(docPackage)) return Docs[docPackage];
            else return null;
        }

        private static MemberList GetMembers(List<MemberInfo> abcMembers, FlagType baseFlags, QName instName)
        {
            MemberList list = new MemberList();
            string package = instName.uri;
            string protect = instName.ToString();

            foreach (MemberInfo info in abcMembers)
            {
                MemberModel member = GetMember(info, baseFlags);
                if (member == null) continue;

                string uri = info.name.uri ?? "";
                if (uri.Length > 0)
                {
                    if (uri == "private" || package == "private")
                    {
                        continue;
                    }
                    else if (uri == protect)
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

        private static MemberModel GetMember(MemberInfo info, FlagType baseFlags)
        {
            MemberModel member = new MemberModel();
            member.Name = info.name.localName;
            member.Flags = baseFlags;
            member.Access = Visibility.Public;
            member.Namespace = "public";

            if (info.metadata != null && info.metadata.Count > 0)
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

                    if (metadatas == null) metadatas = new List<ASMetaData>(info.metadata.Count);
                    metadatas.Add(meta);
                }
                member.MetaDatas = metadatas;
            }

            if (info is SlotInfo)
            {
                SlotInfo slot = info as SlotInfo;
                member.Flags |= FlagType.Variable;
                if (slot.kind == TraitMember.Const) member.Flags |= FlagType.Constant;
                if (slot.value is Namespace)
                {
                    member.Flags |= FlagType.Namespace;
                    member.Value = '"' + (slot.value as Namespace).uri + '"';
                }
                member.Type = ImportType(slot.type);
            }
            else if (info is MethodInfo)
            {
                switch (info.kind)
                {
                    case TraitMember.Setter: member.Flags |= FlagType.Setter; break;
                    case TraitMember.Getter: member.Flags |= FlagType.Getter; break;
                    default: member.Flags |= FlagType.Function; break;
                }
                MethodInfo method = info as MethodInfo;
                QName type = method.returnType;
                member.Type = ImportType(type);

                member.Parameters = new List<MemberModel>();
                int n = method.paramTypes.Length;
                int defaultValues = (method.optionalValues != null) ? n - method.optionalValues.Length : n;
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
            else
            {
                member = null;
            }

            return member;
        }

        private static void GetMemberDoc(MemberModel member)
        {
            string dPath = docPath + ":";
            if (member.Access == Visibility.Public && !String.IsNullOrEmpty(member.Namespace)
                && member.Namespace != "public")
                dPath += member.Namespace + ":";
            dPath += member.Name;
            if ((member.Flags & FlagType.Getter) > 0) dPath += ":get";
            else if ((member.Flags & FlagType.Setter) > 0) dPath += ":set";

            if (thisDocs.ContainsKey(dPath)) applyASDoc(thisDocs[dPath], member);
        }

        private static string ImportType(QName type)
        {
            if (type == null) return "*";
            else return ImportType(type.ToTypeString());
        }

        private static string ImportType(string qname)
        {
            if (qname == null) return "*";
            int p = qname.LastIndexOf('.');
            int q = qname.LastIndexOf('<');
            if (q > 0)
            {
                p = qname.IndexOf('>', q);
                if (p <= q) return qname;
                else
                    return qname.Substring(0, q + 1) + ImportType(qname.Substring(q + 1, p - q - 1)) + qname.Substring(p);
            }
            if (p < 0) return qname;
            if (imports.ContainsKey(qname)) return imports[qname];
            string cname = qname.Substring(p + 1);
            if (!conflicts.ContainsKey(cname)) conflicts.Add(cname, qname);
            else if (conflicts[cname] != qname) 
                cname = qname; // ambiguity
            imports[qname] = cname;
            return cname;
        }

        private static void SetDefaultValue(MemberModel member, object value)
        {
            if (value == null) member.Value = "null";
            else if (value is string && value.ToString() != "undefined") member.Value = '"' + value.ToString() + '"';
            else if (value is bool) member.Value = value.ToString().ToLower();
            else if (value is double) member.Value = ((double)value).ToString(CultureInfo.InvariantCulture.NumberFormat);
            else member.Value = value.ToString();
        }
    }

    #endregion

    #region ASDocItem class: documented values container

    public class ASDocItem
    {
        public bool IsFinal = false;
        public bool IsDynamic = false;
        public bool IsStatic = false;

        public string ShortDesc = null;
        public string LongDesc = null;
        public string Returns = null;
        public string Value = null;
        public string ApiType = null;
        public string DeclType = null;

        public List<ASMetaData> Meta;
        public Dictionary<string, string> Params = new Dictionary<string, string>();
        public Dictionary<string, string> ParamTypes = new Dictionary<string, string>();
        public List<KeyValuePair<string, string>> ExtraAsDocs = new List<KeyValuePair<string, string>>();
    }

    #endregion

    #region ASDocsReader class: documentation parser

    class ASDocsReader : XmlTextReader
    {
        public List<string> ExcludedASDocs = null;
        private Dictionary<string, ASDocItem> docs;


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

        private void ReadDeclaration(string declType)
        {
            if (IsEmptyElement)
                return;

            if (this.ExcludedASDocs == null)
                this.ExcludedASDocs = new List<string>();

            ASDocItem doc = new ASDocItem();
            doc.DeclType = declType;

            string id = GetAttribute("id");

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
                if (doc.ApiType == "String" && doc.Value != null && !doc.Value.StartsWith('\"'))
                    doc.Value = "\"" + doc.Value + "\"";

                if (doc.LongDesc == null)
                    doc.LongDesc = "";

                if (doc.ShortDesc == null)
                    doc.ShortDesc = doc.LongDesc;
                else
                    doc.LongDesc = doc.LongDesc.Trim();

                if (doc.LongDesc.Length == 0 && doc.ShortDesc.Length > 0)
                    doc.LongDesc = doc.ShortDesc;

                if (!this.ExcludedASDocs.Contains("param") && doc.Params != null)
                    foreach (string name in doc.Params.Keys)
                        doc.LongDesc += "\n@param\t" + name + "\t" + doc.Params[name].Trim();

                if (!this.ExcludedASDocs.Contains("return") && doc.Returns != null)
                    doc.LongDesc += "\n@return\t" + doc.Returns.Trim();

                if (doc.ExtraAsDocs != null)
                    foreach (KeyValuePair<string, string> extraASDoc in doc.ExtraAsDocs)
                        if (!this.ExcludedASDocs.Contains(extraASDoc.Key))
                            doc.LongDesc += "\n@" + extraASDoc.Key + "\t" + extraASDoc.Value;

                // keep definitions including either documentation or static values
                if (doc.ShortDesc.Length > 0 || doc.LongDesc.Length > 0
                    || (doc.IsStatic && doc.Value != null && doc.DeclType == "apiValue"))
                    docs[id] = doc;
            }
        }

        private void ProcessDeclarationNodes(ASDocItem doc)
        {
            if (NodeType != XmlNodeType.Element)
                return;

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

        private void SkipContents()
        {
            if (IsEmptyElement)
                return;

            string eon = Name;
            ReadStartElement();
            while (Name != eon)
                Read();
        }

        private string ReadValue()
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
            string lcName; // name in lower case
            ReadStartElement();
            while (Name != eon)
            {
                lcName = Name.ToLower();
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

        private void ReadApiClassifierDetail(ASDocItem doc)
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
                        doc.LongDesc += this.ReadInnerXml() +"\n";
                    //    Read();
                        break;

                    case "example":
                        doc.LongDesc += "\nEXAMPLE: \n\n" + this.ReadInnerXml() +"\n";
                    //    Read();
                        break;

                    default:
                        this.ReadInnerXml();
                        break;
                }
            }
        }

        private void ReadApiClassifierDef(ASDocItem doc)
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
        private void ReadProlog(ASDocItem doc)
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

        private void ReadPrologMetadata(ASDocItem doc)
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

        private void ReadPrologMetadataApiVersion(ASDocItem doc)
        {
            if (IsEmptyElement)
                return;

            string asdocKey;
            string asdocVal;

            string eon = Name;
            ReadStartElement();
            while (Name != eon)
            {
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

        private void ReadPrologMetadataStyles(ASDocItem doc)
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

        private void ReadPrologMetadataDefaultProperty(ASDocItem doc)
        {
            ASMetaData meta = new ASMetaData("DefaultProperty");
            meta.Kind = ASMetaKind.DefaultProperty;
            meta.Comments = "";

            meta.Params = new Dictionary<string, string>();

            string defValue = GetAttribute("name");
            meta.Params["default"] = defValue;

            meta.RawParams = string.Format("\"{0}\"", defValue);

            if (doc.Meta == null) doc.Meta = new List<ASMetaData>();
            doc.Meta.Add(meta);
        }

        private void ReadPrologCustoms(ASDocItem doc, string terminationNode)
        {
            if (IsEmptyElement)
                return;

            string asdocKey;
            string asdocVal;

            string eon = terminationNode;
            ReadStartElement();
            while (!(Name == eon && NodeType == XmlNodeType.EndElement))
            {
                asdocKey = this.Name;

                /*
                if (asdocKey == "maelexample")
                {
                    asdocVal = this.ReadValue();
                    Read();
                }
                else
                {
                    */
                    asdocVal = this.ReadInnerXml();
              //  }

                doc.ExtraAsDocs.Add(new KeyValuePair<string, string>(asdocKey, asdocVal));
            }
        }


        //---------------------------
        //  apiType
        //---------------------------

        private void ReadApiType(ASDocItem doc)
        {
            SetApiType(doc, GetAttribute("value"));
        }

        private void ReadApiTypeAsClassifier(ASDocItem doc)
        {
            SetApiType(doc, ReadValue());
        }

        private void SetApiType(ASDocItem doc, string apiType)
        {
            doc.ApiType = apiType == "any" ? "*" : apiType;
        }


        //---------------------------
        //  apiOperationDetail
        //---------------------------

        private void ReadParamDesc(ASDocItem doc)
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

        private void ReadReturnsDesc(ASDocItem doc)
        {
            if (IsEmptyElement)
                return;

            string eon = Name;
            ReadStartElement();
            while (Name != eon)
            {
                switch (this.Name)
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

        private void ReadApiException(ASDocItem doc)
        {
            if (IsEmptyElement)
                return;

            string apiDesc = "";
            string apiItemName = "";
            string apiOperationClassifier = "";

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
                        apiOperationClassifier = ReadValue();
                        break;
                }
                Read();
            }

            doc.ExtraAsDocs.Add(new KeyValuePair<string, string>("throws", apiItemName + " " + apiDesc));
        }


        //---------------------------
        //  Meta tags
        //---------------------------

        private void ReadExcludeMeta(ASDocItem doc)
        {
            if (!HasAttributes) return;

            ASMetaData meta = new ASMetaData("Style");
            meta.Kind = ASMetaKind.Exclude;
            string sKind = GetAttribute("kind");
            string sName = GetAttribute("name");

            if (doc.Meta == null) doc.Meta = new List<ASMetaData>();
            meta.Params = new Dictionary<string, string>();
            meta.Params["kind"] = sKind;
            meta.Params["name"] = sName;
            meta.RawParams = String.Format("kind=\"{0}\", name=\"{1}\"", sKind, sName);
            doc.Meta.Add(meta);
        }

        private void ReadStyleMeta(ASDocItem doc)
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

            if (doc.Meta == null) doc.Meta = new List<ASMetaData>();
            if (sDefault != null) meta.Comments = meta.Comments.Trim() + "\n@default\t" + sDefault;
            meta.Params = new Dictionary<string, string>();
            meta.Params["name"] = sName;
            meta.Params["type"] = sType;
            meta.RawParams = String.Format("name=\"{0}\", type=\"{1}\"", sName, sType);
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

        private void ReadEventMeta(ASDocItem doc)
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

            if (doc.Meta == null) doc.Meta = new List<ASMetaData>();
            meta.Params = new Dictionary<string, string>();
            meta.Params["name"] = eName;
            meta.Params["type"] = eType;
            if (eFullType != null)
                meta.Comments = meta.Comments.Trim() + "\n@eventType\t" + eFullType.Replace(':', '.');
            meta.RawParams = String.Format("name=\"{0}\", type=\"{1}\"", eName, eType);
            doc.Meta.Add(meta);
        }
    }

    #endregion
}
