using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Helpers;
using ProjectManager.Projects.AS3;

namespace AS3Context
{
    class MxmlFilterContext
    {
        public List<InlineRange> as3ranges = new List<InlineRange>();
        public MemberList mxmlMembers = new MemberList();
        public string baseTag = "";
        public Dictionary<string, string> namespaces = new Dictionary<string,string>();
        public List<MxmlCatalog> catalogs = new List<MxmlCatalog>();
        public FileModel model;
    }

    #region MXML Filter
    class MxmlFilter
    {
        static private readonly Regex tagName = new Regex("<(?<name>[a-z][a-z0-9_:]*)[\\s>]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static public string OLD_MX = "http://www.adobe.com/2006/mxml";
        static public string BETA_MX = "library://ns.adobe.com/flex/halo";
        static public string NEW_MX = "library://ns.adobe.com/flex/mx";

        static private List<MxmlCatalog> catalogs = new List<MxmlCatalog>();
        static private Dictionary<string, MxmlCatalogs> archive = new Dictionary<string, MxmlCatalogs>();

        /// <summary>
        /// Reset catalogs for new classpath definition
        /// </summary>
        static public void ClearCatalogs()
        {
            catalogs.Clear();
        }

        /// <summary>
        /// Look in current project configuration for user-defined namespaces
        /// </summary>
        static public void AddProjectManifests()
        {
            AS3Project project = PluginBase.CurrentProject as AS3Project;
            if (project != null)
            {
                //-compiler.namespaces.namespace http://e4xu.googlecode.com run\manifest.xml
                if (project.CompilerOptions.Additional != null)
                    foreach (string line in project.CompilerOptions.Additional)
                    {
                        string temp = line.Trim();
                        if (temp.StartsWithOrdinal("-compiler.namespaces.namespace") || temp.StartsWithOrdinal("-namespace"))
                        {
                            int p = temp.IndexOf(' ');
                            if (p < 0) p = temp.IndexOf('=');
                            if (p < 0) continue;
                            temp = temp.Substring(p + 1).Trim();
                            p = temp.IndexOf(' ');
                            if (p < 0) p = temp.IndexOf(',');
                            if (p < 0) continue;
                            string uri = temp.Substring(0, p);
                            string path = temp.Substring(p + 1).Trim();
                            if (path.StartsWith('\"')) path = path.Substring(1, path.Length - 2);
                            AddManifest(uri, PathHelper.ResolvePath(path, project.Directory));
                        }
                    }
            }
        }

        /// <summary>
        /// Check if a catalog was already extracted from indicated SWC 
        /// </summary>
        static public bool HasCatalog(string file)
        {
            return archive.ContainsKey(file);
        }

        /// <summary>
        /// Read a SWC catalog file
        /// </summary>
        static public void AddCatalogs(string file, byte[] rawData)
        {
            try
            {
                FileInfo info = new FileInfo(file);
                MxmlCatalogs cat;
                if (HasCatalog(file))
                {
                    cat = archive[file];
                    if (cat.TimeStamp == info.LastWriteTime)
                    {
                        if (cat.Count > 0)
                            catalogs.AddRange(cat.Values);
                        return;
                    }
                }

                cat = new MxmlCatalogs();
                cat.Read(file, rawData);
                cat.TimeStamp = info.LastWriteTime;
                archive[file] = cat;
                if (cat.Count > 0)
                    catalogs.AddRange(cat.Values);
            }
            catch (XmlException ex) { Console.WriteLine(ex.Message); }
            catch (Exception) { }
        }

        /// <summary>
        /// Add an archived SWC catalog
        /// </summary>
        static public void AddCatalog(string file)
        {
            MxmlCatalogs cat = archive[file];
            if (cat.Count > 0)
                catalogs.AddRange(cat.Values);
        }

        /// <summary>
        /// Read a manifest file
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="file"></param>
        static public void AddManifest(string uri, string file)
        {
            try
            {
                FileInfo info = new FileInfo(file);
                MxmlCatalogs cat;
                if (archive.ContainsKey(file))
                {
                    cat = archive[file];
                    if (cat.TimeStamp == info.LastWriteTime)
                    {
                        if (cat.Count > 0)
                            catalogs.AddRange(cat.Values);
                        return;
                    }
                }

                cat = new MxmlCatalogs();
                cat.TimeStamp = info.LastWriteTime;
                cat.Read(file, null, uri);
                archive[file] = cat;
                if (cat.Count > 0)
                    catalogs.AddRange(cat.Values);
            }
            catch (XmlException ex) { Console.WriteLine(ex.Message); }
            catch (Exception) { }
        }

        /// <summary>
        /// Called if a FileModel needs filtering
        /// - define inline AS3 ranges
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        static public string FilterSource(string name, string src, MxmlFilterContext ctx)
        {
            List<InlineRange> as3ranges = ctx.as3ranges;
            MemberList mxmlMembers = ctx.mxmlMembers;

            StringBuilder sb = new StringBuilder();
            sb.Append("package{");
            int len = src.Length - 8;
            int rangeStart = -1;
            int nodeStart = -1;
            int nodeEnd = -1;
            int line = 0;
            bool firstNode = true;
            bool skip = true;
            bool hadNL = false;
            bool inComment = false;
            for (int i = 0; i < len; i++)
            {
                char c = src[i];
                // keep newlines
                if (c == 10 || c == 13)
                {
                    if (c == 13) line++;
                    else if (i > 0 && src[i - 1] != 13) line++;
                    sb.Append(c);
                    hadNL = true;
                }
                // XML comment
                else if (inComment)
                {
                    if (i < len - 3 && c == '-' && src[i + 1] == '-' && src[i + 2] == '>')
                    {
                        inComment = false;
                        i += 3;
                    }
                    continue;
                }
                // in XML
                else if (skip)
                {
                    if (c == '<' && i < len-3)
                    {
                        if (src[i + 1] == '!')
                        {
                            if (src[i + 2] == '-' && src[i + 3] == '-')
                            {
                                inComment = true;
                                i += 3;
                                continue;
                            }
                            else if (src[i + 2] == '[' && src.Substring(i + 2, 7) == "[CDATA[")
                            {
                                i += 8;
                                skip = false;
                                hadNL = false;
                                rangeStart = i;
                                continue;
                            }
                        }
                        else
                        {
                            nodeStart = i + 1;
                            nodeEnd = -1;

                            if (firstNode && src[i + 1] != '?')
                            {
                                int space = src.IndexOfAny(new char[] { ' ', '\n' }, i);
                                string tag = GetXMLContextTag(src, space);
                                if (tag != null && space > 0)
                                {
                                    firstNode = false;
                                    ctx.baseTag = tag;
                                    ReadNamespaces(ctx, src, space);
                                    string type = MxmlComplete.ResolveType(ctx, tag);
                                    sb.Append("public class ").Append(name)
                                        .Append(" extends ").Append(type).Append('{');
                                }
                            }
                        }
                    }
                    else if (c == '=' && i > 4) // <node id="..."
                    {
                        int off = i - 1;
                        while (src[off] == ' ' && off > 4) off--;
                        if (src[off - 2] <= 32 && src[off - 1] == 'i' && src[off] == 'd')
                        {
                            string tag = GetXMLContextTag(src, i);
                            if (tag != null)
                            {
                                string id = GetAttributeValue(src, ref i);
                                if (id != null)
                                {
                                    MemberModel member = new MemberModel(id, tag, FlagType.Variable | FlagType.Dynamic, Visibility.Public);
                                    member.LineTo = member.LineFrom = line;
                                    string type = MxmlComplete.ResolveType(ctx, tag);
                                    mxmlMembers.Add(member);
                                    sb.Append("public var ").Append(id)
                                        .Append(':').Append(type).Append(';');
                                }
                            }
                        }
                    }
                    else if (nodeEnd < 0)
                    {
                        if (nodeStart >= 0 && (c == ' ' || c == '>'))
                        {
                            nodeEnd = i;
                        }
                    }
                }
                // in script
                else
                {
                    if (c == ']' && src[i] == ']' && src[i + 1] == '>')
                    {
                        skip = true;
                        if (hadNL) as3ranges.Add(new InlineRange("as3", rangeStart, i));
                        rangeStart = -1;
                    }
                    else sb.Append(c);
                }
            }
            if (rangeStart >= 0 && hadNL)
                as3ranges.Add(new InlineRange("as3", rangeStart, src.Length));
            sb.Append("}}");
            return sb.ToString();
        }

        private static void ReadNamespaces(MxmlFilterContext ctx, string src, int i)
        {
            // declared ns
            int len = src.Length;
            while (i < len)
            {
                string name = GetAttributeName(src, ref i);
                if (name == null) break;
                string value = GetAttributeValue(src, ref i);
                if (value == null) break;
                if (name.StartsWithOrdinal("xmlns"))
                {
                    string[] qname = name.Split(':');
                    if (qname.Length == 1) ctx.namespaces["*"] = value;
                    else ctx.namespaces[qname[1]] = value;
                }
            }
            // find catalogs
            foreach (string ns in ctx.namespaces.Keys)
            {
                string uri = ctx.namespaces[ns];
                if (uri == OLD_MX || uri == BETA_MX) uri = NEW_MX;
                foreach (MxmlCatalog cat in catalogs)
                    if (cat.URI == uri)
                    {
                        cat.NS = ns;
                        ctx.catalogs.Add(cat);
                    }
            }
        }

        /// <summary>
        /// Get the attribute name
        /// </summary>
        private static string GetAttributeName(string src, ref int i)
        {
            string name = "";
            char c;
            int oldPos = 0;
            int len = src.Length;
            bool skip = true;
            while (i < len)
            {
                c = src[i++];
                if (c == '>') return null;
                if (skip && c > 32) skip = false;
                if (c == '=')
                {
                    if (!skip) return name;
                    else break;
                }
                else if (!skip && c > 32) name += c;
            }
            i = oldPos;
            return null;
        }

        /// <summary>
        /// Get the attribute value
        /// </summary>
        private static string GetAttributeValue(string src, ref int i)
        {
            string value = "";
            char c;
            int oldPos = i;
            int len = src.Length;
            bool skip = true;
            while (i < len)
            {
                c = src[i++];
                if (c == 10 || c == 13) break;
                if (c == '"')
                {
                    if (!skip) return value;
                    else skip = false;
                }
                else if (!skip) value += c;
            }
            i = oldPos;
            return null;
        }

        /// <summary>
        /// Gets the xml context tag
        /// </summary> 
        private static string GetXMLContextTag(string src, int position)
        {
            if (position < 0) return null;
            StringBuilder sb = new StringBuilder();
            char c = src[position - 1];
            position -= 2;
            sb.Append(c);
            while (position >= 0)
            {
                c = src[position];
                sb.Insert(0, c);
                if (c == '>') return null;
                if (c == '<') break;
                position--;
            }
            string tag = sb.ToString();
            Match mTag = tagName.Match(tag + " ");
            if (mTag.Success) return mTag.Groups["name"].Value;
            else return null;
        }

        /// <summary>
        /// Called if a FileModel needs filtering
        /// - modify parsed model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        static public void FilterSource(FileModel model, MxmlFilterContext ctx)
        {
            ctx.model = model;
            model.InlinedIn = "xml";
            model.InlinedRanges = ctx.as3ranges;

            if (model.MetaDatas == null) model.MetaDatas = new List<ASMetaData>();
            foreach (string key in ctx.namespaces.Keys)
            {
                ASMetaData meta = new ASMetaData("Namespace");
                meta.Params = new Dictionary<string, string>();
                meta.Params.Add(key, ctx.namespaces[key]);
                model.MetaDatas.Add(meta);
            }

            ClassModel aClass = model.GetPublicClass();
            if (aClass == ClassModel.VoidClass) 
                return;
            aClass.Comments = "<" + ctx.baseTag + "/>";

            Dictionary<string, string> resolved = new Dictionary<string,string>();
            foreach (MemberModel mxmember in ctx.mxmlMembers)
            {
                string tag = mxmember.Type;
                string type = null;
                if (resolved.ContainsKey(tag)) type = resolved[tag];
                else
                {
                    type = MxmlComplete.ResolveType(ctx, tag);
                    resolved[tag] = type;
                }
                MemberModel member = aClass.Members.Search(mxmember.Name, FlagType.Variable, Visibility.Public);
                if (member != null)
                {
                    member.Comments = "<" + tag + "/>";
                    member.Type = type;
                }
            }
        }
    }
    #endregion

    #region Catalogs
    class MxmlCatalogs : Dictionary<String, MxmlCatalog>
    {
        public String FileName;
        public DateTime TimeStamp;

        public void Read(string fileName, byte[] rawData)
        {
            Read(fileName, rawData, null);
        }

        public void Read(string fileName, byte[] rawData, string defaultURI)
        {
            FileName = fileName;
            XmlReader reader;
            if (rawData == null) reader = new XmlTextReader(fileName);
            else reader = new XmlTextReader(new MemoryStream(rawData));

            MxmlCatalog cat = null;
            reader.MoveToContent();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element
                    && reader.Name == "component")
                {
                    string className = reader.GetAttribute("className") ?? reader.GetAttribute("class");
                    string name = reader.GetAttribute("name") ?? reader.GetAttribute("id");
                    string uri = reader.GetAttribute("uri") ?? defaultURI;
                    if (uri == MxmlFilter.BETA_MX || uri == MxmlFilter.OLD_MX) uri = MxmlFilter.NEW_MX;

                    if (cat == null || cat.URI != uri)
                    {
                        if (ContainsKey(uri)) cat = this[uri];
                        else {
                            cat = new MxmlCatalog();
                            cat.URI = uri;
                            Add(uri, cat);
                        }
                    }
                    cat[name] = className.Replace(':', '.');
                }
            }
        }
    }

    class MxmlCatalog : Dictionary<string, string>
    {
        public string URI;
        public string NS;
    }
    #endregion
}
