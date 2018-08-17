using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ASCompletion;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using ScintillaNet;
using XMLCompletion;

namespace AS3Context
{
    class MxmlComplete
    {
        static public bool IsDirty;
        static public Context context;
        static public MxmlFilterContext mxmlContext;

        #region shortcuts
        public static bool GotoDeclaration()
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci == null) return false;
            if (sci.ConfigurationLanguage != "xml") return false;

            int pos = sci.CurrentPos;
            int len = sci.TextLength;
            while (pos < len)
            {
                char c = (char)sci.CharAt(pos);
                if (c <= 32 || c == '/' || c == '>') break;
                pos ++;
            }
            XMLContextTag ctag = XMLComplete.GetXMLContextTag(sci, pos);
            if (ctag.Name == null) return true;
            string word = sci.GetWordFromPosition(sci.CurrentPos);

            string type = ResolveType(mxmlContext, ctag.Name);
            ClassModel model = context.ResolveType(type, mxmlContext.model);

            if (model.IsVoid()) // try resolving tag as member of parent tag
            {
                parentTag = XMLComplete.GetParentTag(sci, ctag);
                if (parentTag.Name != null)
                {
                    ctag = parentTag;
                    type = ResolveType(mxmlContext, ctag.Name);
                    model = context.ResolveType(type, mxmlContext.model);
                    if (model.IsVoid()) return true;
                }
                else return true;
            }

            if (word != null && !ctag.Name.EndsWithOrdinal(word))
            {
                ASResult found = ResolveAttribute(model, word);
                ASComplete.OpenDocumentToDeclaration(sci, found);
            }
            else
            {
                ASResult found = new ASResult();
                found.InFile = model.InFile;
                found.Type = model;
                ASComplete.OpenDocumentToDeclaration(sci, found);
            }
            return true;
        }
        #endregion

        #region tag completion
        static private XMLContextTag tagContext;
        static private XMLContextTag parentTag;
        static private string tokenContext;
        static private string checksum;
        static private Dictionary<string, List<string>> allTags;
        //static private Regex reIncPath = new Regex("[\"']([^\"']+)", RegexOptions.Compiled);
        static private Regex reIncPath = new Regex("(\"|')([^\r\n]+)(\\1)", RegexOptions.Compiled);
        static private Dictionary<string, FileModel> includesCache = new Dictionary<string,FileModel>();

        /// <summary>
        /// Called 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public bool HandleElement(object data)
        {
            if (!GetContext(data)) return false;

            if (!string.IsNullOrEmpty(tagContext.Name) && tagContext.Name.IndexOf(':') > 0)
                return HandleNamespace(data);

            List<ICompletionListItem> mix = new List<ICompletionListItem>();
            List<string> excludes = new List<string>();

            bool isContainer = AddParentAttributes(mix, excludes); // current tag attributes

            if (isContainer) // container children tag
            foreach (string ns in mxmlContext.namespaces.Keys)
            {
                string uri = mxmlContext.namespaces[ns];
                if (ns != "*") mix.Add(new NamespaceItem(ns, uri));

                if (!allTags.ContainsKey(ns)) 
                    continue;
                foreach (string tag in allTags[ns])
                {
                    if (ns == "*") mix.Add(new HtmlTagItem(tag, tag));
                    else mix.Add(new HtmlTagItem(tag, ns + ":" + tag, uri));
                }
            }

            // cleanup and show list
            mix.Sort(new MXMLListItemComparer());
            List<ICompletionListItem> items = new List<ICompletionListItem>();
            string previous = null;
            foreach (ICompletionListItem item in mix)
            {
                if (previous == item.Label) continue;
                previous = item.Label;
                if (excludes.Contains(previous)) continue;
                items.Add(item);
            }

            if (mix.Count == 0) return true;
            if (!string.IsNullOrEmpty(tagContext.Name)) CompletionList.Show(items, false, tagContext.Name);
            else CompletionList.Show(items, true);
            CompletionList.MinWordLength = 0;
            return true;
        }

        private static bool AddParentAttributes(List<ICompletionListItem> mix, List<string> excludes)
        {
            bool isContainer = true;
            if (parentTag.Name != null) // add parent tag members
            {
                if (tagContext.Closing) // closing tag, only show parent tag
                {
                    isContainer = false;
                    mix.Add(new HtmlTagItem(parentTag.Name.Substring(parentTag.Name.IndexOf(':') + 1), parentTag.Name + '>'));
                }
                else
                {
                    var parentType = ResolveType(mxmlContext, parentTag.Name);
                    var parentClass = context.ResolveType(parentType, mxmlContext.model);
                    if (!parentClass.IsVoid())
                    {
                        parentClass.ResolveExtends();
                        isContainer = GetTagAttributes(parentClass, mix, excludes, parentTag.NameSpace);
                    }
                }
            }
            return isContainer;
        }

        static public bool HandleNamespace(object data)
        {
            if (!GetContext(data) || string.IsNullOrEmpty(tagContext.Name)) 
                return false;

            int p = tagContext.Name.IndexOf(':');
            if (p < 0) return false;
            string ns = tagContext.Name.Substring(0, p);
            if (!mxmlContext.namespaces.ContainsKey(ns)) 
                return true;

            string uri = mxmlContext.namespaces[ns];
            List<ICompletionListItem> mix = new List<ICompletionListItem>();
            List<string> excludes = new List<string>();

            bool isContainer = AddParentAttributes(mix, excludes); // current tag attributes

            if (isContainer && allTags.ContainsKey(ns)) // container children tags
                foreach (string tag in allTags[ns])
                    mix.Add(new HtmlTagItem(tag, ns + ":" + tag, uri));

            // cleanup and show list
            mix.Sort(new MXMLListItemComparer());
            List<ICompletionListItem> items = new List<ICompletionListItem>();
            string previous = null;
            foreach (ICompletionListItem item in mix)
            {
                if (previous == item.Label) continue;
                previous = item.Label;
                if (excludes.Contains(previous)) continue;
                items.Add(item);
            }

            if (mix.Count == 0) return true;
            CompletionList.Show(items, true, tagContext.Name ?? "");
            CompletionList.MinWordLength = 0;
            return true;
        }

        static public bool HandleElementClose(object data)
        {
            if (!GetContext(data)) return false;

            if (tagContext.Closing) return false;

            string type = ResolveType(mxmlContext, tagContext.Name);
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;

            if (type.StartsWithOrdinal("mx.builtin.") || type.StartsWithOrdinal("fx.builtin.")) // special tags
            {
                if (type.EndsWithOrdinal(".Script"))
                {
                    string snip = "$(Boundary)\n\t<![CDATA[\n\t$(EntryPoint)\n\t]]>\n</" + tagContext.Name + ">";
                    SnippetHelper.InsertSnippetText(sci, sci.CurrentPos, snip);
                    return true;
                }
                if (type.EndsWithOrdinal(".Style"))
                {
                    string snip = "$(Boundary)";
                    foreach (string ns in mxmlContext.namespaces.Keys)
                    {
                        string uri = mxmlContext.namespaces[ns];
                        if (ns != "fx")
                            snip += String.Format("\n\t@namespace {0} \"{1}\";", ns, uri);
                    }
                    snip += "\n\t$(EntryPoint)\n</" + tagContext.Name + ">";
                    SnippetHelper.InsertSnippetText(sci, sci.CurrentPos, snip);
                    return true;
                }
            }
            return false;
        }

        static public bool HandleAttribute(object data)
        {
            if (!GetContext(data)) return false;

            string type = ResolveType(mxmlContext, tagContext.Name);
            ClassModel tagClass = context.ResolveType(type, mxmlContext.model);
            if (tagClass.IsVoid()) return true;
            tagClass.ResolveExtends();

            List<ICompletionListItem> mix = new List<ICompletionListItem>();
            List<string> excludes = new List<string>();
            GetTagAttributes(tagClass, mix, excludes, null);

            // cleanup and show list
            mix.Sort(new MXMLListItemComparer());
            List<ICompletionListItem> items = new List<ICompletionListItem>();
            string previous = null;
            foreach (ICompletionListItem item in mix)
            {
                if (previous == item.Label) continue;
                previous = item.Label;
                if (excludes.Contains(previous)) continue;
                items.Add(item);
            }

            if (items.Count == 0) return true;
            if (!string.IsNullOrEmpty(tokenContext)) CompletionList.Show(items, false, tokenContext);
            else CompletionList.Show(items, true);
            CompletionList.MinWordLength = 0;
            return true;
        }

        static public bool HandleAttributeValue(object data)
        {
            if (!GetContext(data)) return false;

            string type = ResolveType(mxmlContext, tagContext.Name);
            ClassModel tagClass = context.ResolveType(type, mxmlContext.model);
            if (tagClass.IsVoid()) return true;
            tagClass.ResolveExtends();

            StringBuilder caBuilder = new StringBuilder();
            bool possibleStartFound = false, startFound = false;
            for (int i = tagContext.Tag.Length - 1; i >= 0; i--)
            {
                char currChar = tagContext.Tag[i];
                if (currChar == '=')
                {
                    possibleStartFound = true;
                }
                else if (startFound)
                {
                    if (Char.IsWhiteSpace(currChar))
                        break;

                    caBuilder.Insert(0, currChar);
                }
                else if (possibleStartFound && !Char.IsWhiteSpace(currChar))
                {
                    startFound = true;
                    caBuilder.Insert(0, currChar);
                }
            }

            var currentAttribute = caBuilder.ToString();

            List<ICompletionListItem> mix = GetTagAttributeValues(tagClass, null, currentAttribute);

            if (mix == null || mix.Count == 0) return true;

            // cleanup and show list
            mix.Sort(new MXMLListItemComparer());
            List<ICompletionListItem> items = new List<ICompletionListItem>();
            string previous = null;
            foreach (ICompletionListItem item in mix)
            {
                if (previous == item.Label) continue;
                previous = item.Label;
                items.Add(item);
            }

            if (items.Count == 0) return true;
            if (!string.IsNullOrEmpty(tokenContext)) CompletionList.Show(items, false, tokenContext);
            else CompletionList.Show(items, true);
            CompletionList.MinWordLength = 0;
            return true;
        }

        private static bool GetTagAttributes(ClassModel tagClass, List<ICompletionListItem> mix, List<string> excludes, string ns)
        {
            ClassModel curClass = mxmlContext.model.GetPublicClass();
            ClassModel tmpClass = tagClass;
            FlagType mask = FlagType.Variable | FlagType.Setter;
            Visibility acc = context.TypesAffinity(curClass, tmpClass);
            bool isContainer = false;

            if (tmpClass.InFile.Package != "mx.builtin" && tmpClass.InFile.Package != "fx.builtin")
                mix.Add(new HtmlAttributeItem("id", "String", null, ns));
            else isContainer = true;

            while (tmpClass != null && !tmpClass.IsVoid())
            {
                string className = tmpClass.Name;
                // look for containers
                if (!isContainer)
                {
                    if (tmpClass.Implements != null
                        && (tmpClass.Implements.Contains("IContainer") // Flex
                        || tmpClass.Implements.Contains("IVisualElementContainer")
                        || tmpClass.Implements.Contains("IFocusManagerContainer")
                        || tmpClass.Implements.Contains("IPopUpHost") // FlexJS
                        || tmpClass.Implements.Contains("IParent")))
                        isContainer = true;
                }

                foreach (MemberModel member in tmpClass.Members)
                    if ((member.Flags & FlagType.Dynamic) > 0 && (member.Flags & mask) > 0
                        && (member.Access & acc) > 0)
                    {
                        string mtype = member.Type;

                        if ((member.Flags & FlagType.Setter) > 0)
                        {
                            if (member.Parameters != null && member.Parameters.Count > 0)
                                mtype = member.Parameters[0].Type;
                            else mtype = null;
                        }
                        mix.Add(new HtmlAttributeItem(member.Name, mtype, className, ns));
                    }

                ExploreMetadatas(tmpClass, mix, excludes, ns, tagClass == tmpClass);

                tmpClass = tmpClass.Extends;
                if (tmpClass.InFile.Package == "" && tmpClass.Name == "Object")
                    break;
                // members visibility
                acc = context.TypesAffinity(curClass, tmpClass);
            }

            return isContainer;
        }

        private static List<ICompletionListItem> GetTagAttributeValues(ClassModel tagClass, string ns, string attribute)
        {
            ClassModel curClass = mxmlContext.model.GetPublicClass();
            ClassModel tmpClass = tagClass;
            FlagType mask = FlagType.Variable | FlagType.Setter | FlagType.Getter;
            Visibility acc = context.TypesAffinity(curClass, tmpClass);

            if (tmpClass.InFile.Package != "mx.builtin" && tmpClass.InFile.Package != "fx.builtin" && attribute == "id")
                return null;

            // Inspectable metadata should be appended to getter types, and according to latest guidelines, the attribute have both a getter and setter
            // However, if a component has just a setter I want to autocomplete it if possible, also, the getter or setter may be defined in a super class
            bool hasGetterSetter = false;
            List<ASMetaData> metas = null;
            string setterType = null;
            while (tmpClass != null && !tmpClass.IsVoid())
            {
                foreach (MemberModel member in tmpClass.Members)
                    if ((member.Flags & FlagType.Dynamic) > 0 && (member.Flags & mask) > 0
                        && (member.Access & acc) > 0)
                    {
                        if (member.Name == attribute)
                        {
                            string mtype = member.Type;
                            
                            if ((member.Flags & FlagType.Setter) > 0)
                            {
                                if (member.Parameters != null && member.Parameters.Count > 0)
                                    mtype = member.Parameters[0].Type;
                                else mtype = null;

                                if (!hasGetterSetter)
                                {
                                    hasGetterSetter = true;
                                    setterType = mtype;
                                    continue;
                                }
                                return GetAutoCompletionValuesFromInspectable(mtype, metas);
                            }
                            else if ((member.Flags & FlagType.Getter) > 0)
                            {
                                if (!hasGetterSetter)
                                {
                                    hasGetterSetter = true;
                                    metas = member.MetaDatas;
                                    continue;
                                }
                                return GetAutoCompletionValuesFromInspectable(setterType, metas);
                            }

                            return GetAutoCompletionValuesFromType(mtype);
                        }

                    }

                List<ICompletionListItem> retVal;
                if (GetAutoCompletionValuesFromMetaData(tmpClass, attribute, tagClass, tmpClass, out retVal))
                    return retVal;

                tmpClass = tmpClass.Extends;
                if (tmpClass.InFile.Package == "" && tmpClass.Name == "Object")
                    break;
                // members visibility
                acc = context.TypesAffinity(curClass, tmpClass);
            }

            if (setterType != null)
                return GetAutoCompletionValuesFromType(setterType);

            return null;
        }

        private static List<ICompletionListItem> GetAutoCompletionValuesFromInspectable(string type, List<ASMetaData> metas)
        {
            if (metas == null || metas.Count == 0)
                return GetAutoCompletionValuesFromType(type);

            foreach (var meta in metas)
            {
                if (meta.Name != "Inspectable") continue;

                string enumValues = null;
                if (meta.Params.TryGetValue("enumeration", out enumValues))
                {
                    var retVal = new List<ICompletionListItem>();
                    foreach (string value in enumValues.Split(','))
                    {
                        var tValue = value.Trim();
                        if (tValue != string.Empty) retVal.Add(new HtmlAttributeItem(tValue));
                    }

                    if (retVal.Count > 0) return retVal;
                }
            }

            return GetAutoCompletionValuesFromType(type);
        }

        private static bool GetAutoCompletionValuesFromMetaData(ClassModel model, string attribute, ClassModel tagClass, ClassModel tmpClass, out List<ICompletionListItem> result)
        {
            if (model != null && model.MetaDatas != null)
            {
                foreach (ASMetaData meta in model.MetaDatas)
                {
                    string name = null;
                    if (!meta.Params.TryGetValue("name", out name) || name != attribute) continue;

                    string type = null;
                    switch (meta.Kind)
                    {
                        case ASMetaKind.Event:
                            string eventType;
                            if (!meta.Params.TryGetValue("type", out eventType)) eventType = "flash.events.Event";
                            result = GetAutoCompletionValuesFromEventType(eventType);
                            return true;
                        case ASMetaKind.Style:
                            string inherit;
                            if (meta.Params.TryGetValue("inherit", out inherit) && inherit == "no" && tagClass != tmpClass)
                                continue;
                            meta.Params.TryGetValue("type", out type);
                            break;
                        case ASMetaKind.Effect:
                            type = meta.Params["event"];
                            break;
                        case ASMetaKind.Exclude:
                            break;
                        case ASMetaKind.Include:    // TODO: Check this case...
                            Debug.Assert(false, "Please, check this case");
                            FileModel incModel = ParseInclude(model.InFile, meta);
                            return GetAutoCompletionValuesFromMetaData(incModel.GetPublicClass(), attribute, tagClass, tmpClass, out result);
                    }
                    if (meta.Params.ContainsKey("enumeration"))
                    {
                        var retVal = new List<ICompletionListItem>();
                        foreach (string value in meta.Params["enumeration"].Split(','))
                        {
                            var tValue = value.Trim();
                            if (tValue != string.Empty) retVal.Add(new HtmlAttributeItem(tValue));
                        }
                        result = retVal;

                        return true;
                    }

                    result = GetAutoCompletionValuesFromType(type);

                    return true;
                }
            }

            result = null;
            return false;
        }

        private static List<ICompletionListItem> GetAutoCompletionValuesFromEventType(string type)
        {
            ClassModel tmpClass = mxmlContext.model.GetPublicClass();
            ClassModel eventClass = context.ResolveType(type, mxmlContext.model);
            Visibility acc = Visibility.Default | Visibility.Internal | Visibility.Private | Visibility.Protected | Visibility.Public;

            tmpClass.ResolveExtends();
            eventClass.ResolveExtends();

            List<ICompletionListItem> result = null;
            var validTypes = new Dictionary<string, bool>();
            while (!tmpClass.IsVoid())
            {
                foreach (MemberModel member in tmpClass.Members)
                    if ((member.Flags & FlagType.Function) > 0 && (member.Access & acc) > 0 && member.Parameters != null && member.Parameters.Count > 0)
                    {
                        bool validFunction = true;
                        var argType = member.Parameters[0].Type;
                        if (argType != type && argType != "Object" && argType != "*" && !validTypes.TryGetValue(argType, out validFunction))
                        {
                            ClassModel argClass = context.ResolveType(argType, tmpClass.InFile);
                            if (argClass.IsVoid())
                                validTypes[argType] = validFunction = false;
                            else
                            {
                                validTypes[argType] = validFunction = (context.TypesAffinity(eventClass, argClass) & Visibility.Protected) > 0;
                                if (argType != argClass.Type) validTypes[argClass.Type] = validFunction;
                            }
                        }

                        if (!validFunction) continue;

                        for (int i = 1, count = member.Parameters.Count; i < count; i++)
                        {
                            if (member.Parameters[i].Value != null)
                            {
                                validFunction = false;
                                break;
                            }
                        }

                        if (!validFunction) continue;

                        if (result == null) result = new List<ICompletionListItem>();
                        result.Add(new MxmlEventHandlerItem(member));
                    }

                tmpClass = tmpClass.Extends;
                if (tmpClass.InFile.Package == "" && tmpClass.Name == "Object")
                    break;
                // members visibility
                // TODO: Take into account namespaces!
                acc = Visibility.Protected | Visibility.Public;
            }

            return result;
        }

        private static List<ICompletionListItem> GetAutoCompletionValuesFromType(string type)
        {
            if (type == "Boolean")
            {
                return new List<ICompletionListItem>() 
                {
                    new HtmlAttributeItem("true"),
                    new HtmlAttributeItem("false")
                };
            }
            else if (type == "Class")
            {
                ASComplete.HandleAllClassesCompletion(PluginBase.MainForm.CurrentDocument.SciControl, tokenContext,
                                                      true, false);
            }
            else if (type == "Function")
            {
                var tmpClass = mxmlContext.model.GetPublicClass();
                var access = Visibility.Default | Visibility.Internal | Visibility.Private | Visibility.Protected | Visibility.Public;
                tmpClass.ResolveExtends();
                List<ICompletionListItem> result = null;
                while (!tmpClass.IsVoid())
                {
                    foreach (MemberModel member in tmpClass.Members)
                        if ((member.Flags & FlagType.Function) > 0 && (member.Access & access) > 0)
                        {
                            if (result == null) result = new List<ICompletionListItem>();
                            result.Add(new MemberItem(member));
                        }

                    tmpClass = tmpClass.Extends;
                    if (tmpClass.InFile.Package == "" && tmpClass.Name == "Object")
                        break;
                    // members visibility
                    // TODO: Take into account namespaces!
                    access = Visibility.Protected | Visibility.Public;
                }
                return result;
            }
            return null;
        }

        private static void ExploreMetadatas(ClassModel model, List<ICompletionListItem> mix, List<string> excludes, string ns, bool isCurrentModel)
        {
            if (model == null || model.MetaDatas == null) 
                return;
            string className = model.IsVoid() ? Path.GetFileNameWithoutExtension(model.InFile.FileName) : model.Name;
            foreach (ASMetaData meta in model.MetaDatas)
            {
                string add = null;
                string type = null;
                switch (meta.Kind)
                {
                    case ASMetaKind.Event: add = ":e"; break;
                    case ASMetaKind.Style:
                        string inherit;
                        if (meta.Params == null || !meta.Params.TryGetValue("inherit", out inherit) || inherit != "no" || isCurrentModel)
                        {
                            add = ":s";
                            if (meta.Params == null || !meta.Params.TryGetValue("type", out type)) type = "Object";
                        }
                        break;
                    case ASMetaKind.Effect: 
                        add = ":x";
                        if (meta.Params != null) type = meta.Params["event"]; 
                        break;
                    case ASMetaKind.Exclude:
                        if (meta.Params != null) excludes.Add(meta.Params["name"]);
                        break;
                    case ASMetaKind.Include:
                        FileModel incModel = ParseInclude(model.InFile, meta);
                        ExploreMetadatas(incModel.GetPublicClass(), mix, excludes, ns, isCurrentModel);
                        break;
                }
                if (add != null && meta.Params.ContainsKey("name"))
                    mix.Add(new HtmlAttributeItem(meta.Params["name"] + add, type, className, ns));
            }
        }

        private static FileModel ParseInclude(FileModel fileModel, ASMetaData meta)
        {
            Match m = reIncPath.Match(meta.RawParams);
            if (m.Success)
            {
                string path = m.Groups[2].Value;
                if (path.Length == 0) return null;

                // retrieve from cache
                if (includesCache.ContainsKey(path))
                    return includesCache[path];

                // relative path?
                string fileName = path;
                if (!Path.IsPathRooted(fileName))
                {
                    if (fileName[0] == '/' || fileName[0] == '\\')
                        fileName = Path.Combine(fileModel.BasePath, fileName);
                    else
                        fileName = Path.Combine(Path.GetDirectoryName(fileModel.FileName), fileName);
                }

                // parse & cache
                if (!File.Exists(fileName)) return null;
                string src = File.ReadAllText(fileName);
                if (!src.Contains("package")) src = "package {" + src + "}";
                ASFileParser parser = new ASFileParser();
                FileModel model = new FileModel(path);
                parser.ParseSrc(model, src);

                includesCache[path] = model;
                return model;
            }
            return null;
        }
        #endregion

        #region context detection
        private static bool GetContext(object data)
        {
            if (mxmlContext == null || mxmlContext.model == null) 
                return false;

            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci == null) return false;

            // XmlComplete context
            try
            {
                if (data is XMLContextTag)
                    tagContext = (XMLContextTag)data;
                else
                {
                    object[] o = (object[])data;
                    tagContext = (XMLContextTag)o[0];
                    tokenContext = (string)o[1];
                }
            }
            catch
            {
                return false;
            }

            // more context
            parentTag = XMLComplete.GetParentTag(sci, tagContext);

            // rebuild tags cache?
            string sum = "" + context.GetAllProjectClasses().Count;
            foreach (string uri in mxmlContext.namespaces.Values)
                sum += uri;
            if (IsDirty || sum != checksum)
            {
                checksum = sum;
                GetAllTags();
            }
            return true;
        }
        #endregion

        #region tag resolution
        private static void GetAllTags()
        {
            Dictionary<string, string> nss = mxmlContext.namespaces;
            MemberList allClasses = context.GetAllProjectClasses();
            Dictionary<string, string> packages = new Dictionary<string, string>();
            allTags = new Dictionary<string, List<string>>();

            foreach (string key in nss.Keys)
            {
                string uri = nss[key];
                if (uri.EndsWithOrdinal(".*"))
                    packages[uri.Substring(0, uri.LastIndexOf('.') + 1)] = key;
                else if (uri == "*")
                    packages["*"] = key;
            }

            foreach (MemberModel model in allClasses)
            {
                if ((model.Flags & FlagType.Class) == 0 || (model.Flags & FlagType.Interface) != 0)
                    continue;
                int p = model.Type.IndexOf('.');
                string bns = p > 0 ? model.Type.Substring(0, p) : "";
                if (bns == "mx" || bns == "fx" || bns == "spark")
                    continue;

                p = model.Type.LastIndexOf('.');
                string pkg = model.Type.Substring(0, p + 1);
                if (pkg == "") pkg = "*";
                if (packages.ContainsKey(pkg))
                {
                    string ns = packages[pkg];
                    if (!allTags.ContainsKey(ns)) allTags.Add(ns, new List<string>());
                    allTags[ns].Add(model.Name.Substring(p + 1));
                }
            }

            foreach (MxmlCatalog cat in mxmlContext.catalogs)
            {
                List<string> cls = allTags.ContainsKey(cat.NS) ? allTags[cat.NS] : new List<string>();
                cls.AddRange(cat.Keys);
                allTags[cat.NS] = cls;
            }
        }

        public static string ResolveType(MxmlFilterContext ctx, string tag)
        {
            if (tag == null || ctx == null) return "void";
            int p = tag.IndexOf(':');
            if (p < 0) return ResolveType(ctx, "*", tag);
            else return ResolveType(ctx, tag.Substring(0, p), tag.Substring(p + 1));
        }

        public static string ResolveType(MxmlFilterContext ctx, string ns, string name)
        {
            if (!ctx.namespaces.ContainsKey(ns))
                return name;

            string uri = ctx.namespaces[ns];
            if (uri == "*")
                return name;
            if (uri.EndsWithOrdinal(".*"))
                return uri.Substring(0, uri.Length - 1) + name;

            if (uri == MxmlFilter.BETA_MX || uri == MxmlFilter.OLD_MX) 
                uri = MxmlFilter.NEW_MX;

            foreach (MxmlCatalog cat in ctx.catalogs)
            {
                if (cat.URI == uri && cat.ContainsKey(name))
                    return cat[name];
            }
            return name;
        }

        private static ASResult ResolveAttribute(ClassModel model, string word)
        {
            var result = new ASResult();
            var curClass = mxmlContext.model.GetPublicClass();
            var tmpClass = model;
            var acc = context.TypesAffinity(curClass, tmpClass);
            tmpClass.ResolveExtends();
            while (!tmpClass.IsVoid())
            {
                foreach (MemberModel member in tmpClass.Members)
                    if ((member.Flags & FlagType.Dynamic) > 0 && (member.Access & acc) > 0
                        && member.Name == word)
                    {
                        result.InFile = tmpClass.InFile;
                        if (member.LineFrom == 0) // cached model, reparse
                        {
                            result.InFile.OutOfDate = true;
                            result.InFile.Check();
                            if (result.InFile.Classes.Count > 0)
                            {
                                result.InClass = result.InFile.Classes[0];
                                result.Member = result.InClass.Members.Search(member.Name, member.Flags, 0);
                            }
                        }
                        else result.Member = member;
                        return result;
                    }

                // TODO inspect metadata & includes

                tmpClass = tmpClass.Extends;
                if (tmpClass.InFile.Package == "" && tmpClass.Name == "Object")
                    break;
                // members visibility
                acc = context.TypesAffinity(curClass, tmpClass);
            }
            return result;
        }
        #endregion
    }

    class MXMLListItemComparer : IComparer<ICompletionListItem>
    {
        public int Compare(ICompletionListItem a, ICompletionListItem b)
        {
            string a1;
            string b1;
            if (a.Label.Equals(b.Label, StringComparison.OrdinalIgnoreCase))
            {
                if (a is HtmlAttributeItem && b is HtmlTagItem) return 1;
                if (b is HtmlAttributeItem && a is HtmlTagItem) return -1;
            }
            if (a is IHtmlCompletionListItem)
            {
                a1 = ((IHtmlCompletionListItem)a).Name;
                if (a.Value.StartsWithOrdinal("mx:")) a1 += "z"; // push down mx: tags
            }
            else a1 = a.Label;
            if (b is IHtmlCompletionListItem)
            {
                b1 = ((IHtmlCompletionListItem)b).Name;
                if (b.Value.StartsWithOrdinal("mx:")) b1 += "z"; // push down mx: tags
            }
            else b1 = b.Label;
            return string.Compare(a1, b1);
        }

    }

    #region completion list
    /// <summary>
    /// Event member completion list item
    /// </summary>
    public class MxmlEventHandlerItem : ICompletionListItem
    {
        private MemberModel member;
        private int icon;

        public MxmlEventHandlerItem(MemberModel oMember)
        {
            member = oMember;
            icon = PluginUI.GetIcon(member.Flags, member.Access);
        }

        public string Label
        {
            get { return member.FullName; }
        }

        public string Description
        {
            get
            {
                return ClassModel.MemberDeclaration(member) + ASDocumentation.GetTipDetails(member, null);
            }
        }

        public Bitmap Icon
        {
            get { return (Bitmap)ASContext.Panel.GetIcon(icon); }
        }

        public string Value
        {
            get
            {
                return member.Name + "(event)";
            }
        }
    }
    #endregion

}
