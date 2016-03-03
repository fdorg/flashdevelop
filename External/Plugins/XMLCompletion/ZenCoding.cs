using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using LitJson;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;
using ScintillaNet;

namespace XMLCompletion
{
    #region zen settings
    public class ZenElementTypes
    {
        public Hashtable empty = new Hashtable();
        public Hashtable block_level = new Hashtable();
        public Hashtable inline_level = new Hashtable();

        public ZenElementTypes(Hashtable def)
        {
            if (def == null) return;
            if (def.ContainsKey("empty")) ParseSet(empty, (string)def["empty"]);
            if (def.ContainsKey("block_level")) ParseSet(block_level, (string)def["block_level"]);
            if (def.ContainsKey("inline_level")) ParseSet(inline_level, (string)def["inline_level"]);
        }

        private void ParseSet(Hashtable set, string def)
        {
            string[] tokens = def.Split(',');
            foreach (string token in tokens) set[token] = true;
        }
    }

    public class ZenLang 
    {
        public Hashtable snippets = new Hashtable();
        public Hashtable abbreviations = new Hashtable();
        public string extends; // set using reflexion
        public string filters;
        public ZenElementTypes element_types;
    }

    public class ZenSettings
    {
        public Hashtable variables;
        public Dictionary<string, ZenLang> langs = new Dictionary<string, ZenLang>();

        public static ZenSettings Read(string filePath)
        {
            string src = File.ReadAllText(filePath);
            src = SanitizeJSon(src);
            JsonReader reader = new JsonReader(src);
            return ReadZenSettings(reader);
        }

        private static ZenSettings ReadZenSettings(JsonReader reader)
        {
            ZenSettings settings = new ZenSettings();

            reader.Read();
            if (reader.Token != JsonToken.ObjectStart)
                return null;

            string currentProp = null;
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ObjectEnd) break;
                if (reader.Token == JsonToken.PropertyName)
                    currentProp = reader.Value.ToString();
                else if (reader.Token == JsonToken.ObjectStart)
                {
                    if (currentProp == "variables") settings.variables = ReadHashtable(reader);
                    else settings.langs.Add(currentProp, ReadZenLang(reader));
                }
            }

            foreach (ZenLang lang in settings.langs.Values)
            {
                if (lang.extends != null && settings.langs.ContainsKey(lang.extends))
                    ExtendLang(lang, settings.langs[lang.extends]);
            }

            if (!settings.langs.ContainsKey("xml"))
            {
                ZenLang xlang = new ZenLang();
                xlang.abbreviations = new Hashtable();
                xlang.element_types = new ZenElementTypes(null);
                xlang.filters = "xml, xsl";
                xlang.snippets = new Hashtable();
                settings.langs.Add("xml", xlang);
            }

            settings.variables["child"] = "";
            return settings;
        }

        private static void ExtendLang(ZenLang lang, ZenLang lang2)
        {
            MergeHashtable(ref lang.abbreviations, ref lang2.abbreviations);
            MergeHashtable(ref lang.snippets, ref lang2.snippets);
            if (lang.element_types == null) lang.element_types = lang2.element_types;
            else if (lang2.element_types != null)
            {
                MergeHashtable(ref lang.element_types.empty, ref lang2.element_types.empty);
                MergeHashtable(ref lang.element_types.block_level, ref lang2.element_types.block_level);
                MergeHashtable(ref lang.element_types.inline_level, ref lang2.element_types.inline_level);
            }
        }

        private static void MergeHashtable(ref Hashtable t1, ref Hashtable t2)
        {
            if (t1 == null) t1 = t2.Clone() as Hashtable;
            else if (t2 != null)
                foreach (string key in t2.Keys)
                    if (!t1.ContainsKey(key)) t1[key] = t2[key];
        }

        private static Hashtable ReadHashtable(JsonReader reader)
        {
            Hashtable table = new Hashtable();
            string currentKey = null;
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ObjectEnd) break;
                if (reader.Token == JsonToken.PropertyName) currentKey = reader.Value.ToString();
                else if (reader.Token == JsonToken.String) table[currentKey] = reader.Value;
            }
            return table;
        }

        private static ZenLang ReadZenLang(JsonReader reader)
        {
            ZenLang lang = new ZenLang();
            Type objType = lang.GetType();

            string currentProp = null;
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ObjectEnd) break;
                if (reader.Token == JsonToken.PropertyName) currentProp = reader.Value.ToString();
                else if (reader.Token == JsonToken.String)
                {
                    string value = reader.Value.ToString();
                    FieldInfo info = objType.GetField(currentProp);
                    if (info != null) info.SetValue(lang, value);
                }
                else if (reader.Token == JsonToken.ObjectStart)
                {
                    Hashtable table = ReadHashtable(reader);
                    if (currentProp == "element_types")
                    {
                        lang.element_types = new ZenElementTypes(table);
                    }
                    else
                    {
                        FieldInfo info = objType.GetField(currentProp);
                        if (info != null) info.SetValue(lang, table);
                    }
                }
            }
            return lang;
        }

        private static string SanitizeJSon(string src)
        {
            src = src.Substring(src.IndexOf('{'));
            src = src.Substring(0, src.LastIndexOf('}') + 1);
            src = src.Replace("'\\", "'\\\\"); // escaped unicode values "\00AB"
            src = Regex.Replace(src, "['\"]\\s*\\+\\s*['\"]", ""); // "..." + "..."
            return src;
        }
    }
    #endregion

    public class ZenCoding
    {
        static private ZenLang lang;
        static private bool inited;
        static private ZenSettings settings;
        static private Timer delayOpenConfig;
        static private FileSystemWatcher watcherConfig;
        static private Regex reVariable = new Regex("\\${([-_a-z0-9]+)}", RegexOptions.IgnoreCase);

        #region initialization
        static private void init()
        {
            if (!inited)
            {
                inited = true;

                LoadResource("zen_settings.js");

                if (delayOpenConfig == null) // timer for opening config files
                {
                    delayOpenConfig = new Timer();
                    delayOpenConfig.Interval = 100;
                    delayOpenConfig.Tick += new EventHandler(delayOpenConfig_Tick);
                }
                if (watcherConfig == null) // watching config files changes
                {
                    watcherConfig = new FileSystemWatcher(Path.Combine(PathHelper.DataDir, "XMLCompletion"), "zen*");
                    watcherConfig.Changed += new FileSystemEventHandler(watcherConfig_Changed);
                    watcherConfig.Created += new FileSystemEventHandler(watcherConfig_Changed);
                    watcherConfig.EnableRaisingEvents = true;
                }
            }
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            string docType = sci != null ? sci.ConfigurationLanguage.ToLower() : null;
            lang = null;
            if (docType != null)
            {
                if (settings.langs.ContainsKey(docType))
                    lang = settings.langs[docType];
            }
        }

        static void watcherConfig_Changed(object sender, FileSystemEventArgs e)
        {
            inited = false;
        }

        static void delayOpenConfig_Tick(object sender, EventArgs e)
        {
            delayOpenConfig.Stop();
            string path = Path.Combine(PathHelper.DataDir, "XMLCompletion");
            PluginBase.MainForm.OpenEditableDocument(Path.Combine(path, "zen_settings.js"));
        }

        private static void LoadResource(string file)
        {
            string path = Path.Combine(PathHelper.DataDir, "XMLCompletion");
            string filePath = Path.Combine(path, file);
            try
            {
                if (!File.Exists(filePath) && !WriteResource(file, filePath))
                    return;
                settings = ZenSettings.Read(filePath);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        private static bool WriteResource(string file, string filePath)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream src = assembly.GetManifestResourceStream("XMLCompletion.Resources." + file);
                if (src == null)
                    return false;

                String content;
                using (StreamReader sr = new StreamReader(src))
                {
                    content = sr.ReadToEnd();
                    sr.Close();
                }
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.Write(content);
                    sw.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region expansion
        static public bool expandSnippet(Hashtable data)
        {
            if (data["snippet"] == null)
            {
                ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                if (sci == null) return false;
                // extract zen expression
                int pos = sci.CurrentPos - 1;
                int lastValid = sci.CurrentPos;
                char c = ' ';
                while (pos >= 0)
                {
                    c = (char)sci.CharAt(pos);
                    if (c <= 32)
                    {
                        lastValid = pos + 1;
                        break;
                    }
                    if (c == '>')
                    {
                        if (lastValid - 1 <= pos) break;
                        lastValid = pos + 1;
                    }
                    else if (!Char.IsLetterOrDigit(c) && "+*$.#:-".IndexOf(c) < 0) break;
                    pos--;
                    if (pos < 0) lastValid = 0;
                }
                // expand
                if (lastValid < sci.CurrentPos)
                {
                    sci.SetSel(lastValid, sci.CurrentPos);
                    try
                    {
                        string expr = expandExpression(sci.SelText);
                        if (expr == null) return false;
                        if (expr.IndexOfOrdinal("$(EntryPoint)") < 0) expr += "$(EntryPoint)";
                        data["snippet"] = expr;
                    }
                    catch (ZenExpandException zex)
                    {
                        // error in expression, no snippet display
                        TraceManager.AddAsync(zex.Message);
                        return true;
                    }
                    // insert modified snippet or show snippet list
                    return false; 
                }
            }
            return false;
        }

        static public string expandExpression(string expr)
        {
            init(); // load config
            if (lang == null) return null;

            if (expr == "zen") // show config
            {
                delayOpenConfig.Start();
                return "";
            }
            else if (expr.EndsWith('+'))
            {
                if (lang.abbreviations.ContainsKey(expr))
                    expr = (string)lang.abbreviations[expr]; // expandos
                else return expr;
            }
            if (expr.Length == 0) return "";

            // process
            string src = expr[0] == '<' ? expr : expandZen(expr);
            int p = src.IndexOf('|');
            src = src.Replace("|", "");
            if (p < 0) return src;
            else return src.Substring(0, p) + "$(EntryPoint)" + src.Substring(p);
        }

        private static string expandZen(string expr)
        {
            if (expr.Length == 0) 
                throw new ZenExpandException("Empty expression found");

            string src = "";
            string[] parts = expr.Split('>');
            Array.Reverse(parts);
            bool inline = true;
            int index = 1;
            foreach (string part in parts)
            {
                if (part.Length == 0)
                    throw new ZenExpandException("Empty sub-expression found (sub1>sub2)");

                string subSrc = src;
                src = "";
                string[] sparts = part.Split('+');
                foreach (string spart in sparts)
                {
                    if (spart.Length == 0)
                        throw new ZenExpandException("Empty sub-expression part found (part1+part2)");

                    if (!inline && src.Length > 0) src += "\n";

                    int multiply = 1;
                    string tag = spart;
                    // read multiplier
                    string mult = extractEnd('*', ref tag);
                    if (mult != null)
                    {
                        multiply = -1;
                        int.TryParse(mult, out multiply);
                        if (multiply < 0)
                            throw new ZenExpandException("Invalid multiplier value (" + mult + ")");
                    }
                    // read css classes
                    string css = "";
                    string cssClass;
                    do
                    {
                        cssClass = extractEnd('.', ref tag);
                        if (cssClass != null) css = cssClass + " " + css;
                    }
                    while (cssClass != null);
                    // read ID
                    string id = extractEnd('#', ref tag) ?? "";
                    
                    // build attributes
                    string atId = id.Length > 0 ? " id=\"" + id + "\"" : "";
                    string atClass = css.Length > 0 ? " class=\"" + css.Trim() + "\"" : "";

                    // build tag
                    string tagStart = "";
                    string tagEnd = "";
                    bool closedTag = false;
                    
                    // custom HTML
                    bool customExpand = false;
                    bool customChildIndent = false;
                    if (lang.snippets.ContainsKey(tag))
                    {
                        tag = (string)lang.snippets[tag];
                        customExpand = true;
                    }
                    else if (lang.abbreviations.ContainsKey(tag))
                    {
                        tag = (string)lang.abbreviations[tag];
                        customExpand = true;

                        if (tag.Length > 2 && tag[0] == '<') // insert attributes
                        {
                            if (tag[1] != '!')
                            {
                                int sp = tag.IndexOf(' ');
                                if (sp >= 0)
                                {
                                    tagStart = tag.Substring(0, sp);
                                    tagEnd = tag.Substring(sp);
                                    tag = tagStart;
                                    if (atId.Length > 0)
                                    {
                                        if (tagEnd.IndexOfOrdinal(" id=") < 0) tag += atId;
                                        else tagEnd = tagEnd.Replace(" id=\"\"", atId);
                                    }
                                    if (atClass.Length > 0)
                                    {
                                        if (tagEnd.IndexOfOrdinal(" class=") < 0) tag += atClass;
                                        else tagEnd = tagEnd.Replace(" class=\"\"", atClass);
                                    }
                                    tag += tagEnd;
                                }
                            }
                        }
                    }
                    else extractEnd(':', ref tag);

                    if (customExpand)
                    {
                        if (tag.IndexOfOrdinal("${") >= 0) tag = ProcessVars(tag);

                        tag = tag.Replace("\\n", "\n").Replace("\\t", "\t");
                        if (tag.IndexOf('|') < 0) tag = tag.Replace("\"\"", "\"|\"");

                        int child = tag.IndexOfOrdinal("${child}");
                        if (child >= 0)
                        {
                            tag = tag.Replace("${child}", "");
                            customChildIndent = true;
                        }
                        else
                        {
                            child = tag.IndexOfOrdinal("><");
                            if (child > 0) child++;
                        }
                        
                        if (child > 0)
                        {
                            tagStart = tag.Substring(0, child);
                            tagEnd = "|" + tag.Substring(child);
                        }
                        else closedTag = true;
                    }
                    else
                    {
                        closedTag = lang.element_types.empty.ContainsKey(tag);
                        if (closedTag) tag = "<" + tag + "/>";
                    }

                    if (tag.Length > 0 && tag[0] != '<')
                    {
                        tagStart = "<" + tag + atId + atClass + ">";
                        tagEnd = "</" + tag + ">";
                    }

                    string master;
                    string temp = spart == sparts[sparts.Length - 1] ? subSrc : "";
                    if (closedTag)
                    {
                        inline = sparts.Length == 1 && isInline(tag);
                        master = tag;
                    }
                    else
                    {
                        string wrapIn = "";
                        string wrapOut = "";
                        if (temp.Length > 0 && (!inline || isBlock(tag)))
                        {
                            wrapIn = customChildIndent ? "" : "\n\t";
                            wrapOut = customChildIndent ? "" : "\n";
                            temp = addIndent(temp);
                        }
                        if (temp.Length == 0) temp = "|";

                        inline = sparts.Length == 1 && isInline(tag);
                        master = tagStart + wrapIn + temp + wrapOut + tagEnd;
                    }

                    for (int i = 1; i <= multiply; i++)
                    {
                        if (multiply > 1)
                        {
                            index = i;
                            src += master.Replace("$", index.ToString());
                        }
                        else src += master;
                        if (!inline && i < multiply) src += "\n";
                    }
                }
            }
            return src;
        }

        private static string ProcessVars(string tag)
        {
            return reVariable.Replace(tag, VarReplacer);
        }

        private static string VarReplacer(Match m)
        {
            string name = m.Groups[1].Value;
            if (name != "child" && settings.variables.ContainsKey(name)) 
                return (string)settings.variables[name];
            else return m.Value;
        }

        private static string addIndent(string res)
        {
            string[] lines = res.Split('\n');
            res = "";
            foreach (string line in lines) res += "\t" + line + "\n";
            return res.Trim();
        }

        private static bool isBlock(string tag)
        {
            return lang.element_types.block_level.ContainsKey(tag);
        }

        private static bool isInline(string tag)
        {
            if (tag.Length > 3 && tag[0] == '<') 
            {
                // extract tag name
                tag = tag.Substring(1).Split(new char[] { ' ', '"', '\'', '/', '|', '>' }, 2)[0];
            }
            return lang.element_types.inline_level.ContainsKey(tag);
        }

        private static string extractEnd(char sep, ref string part)
        {
            int p = part.LastIndexOf(sep);
            if (p == 0)
                throw new ZenExpandException("Empty '" + sep + "' argument found (" + part + ")");
            if (p > 0)
            {
                string ret = part.Substring(p + 1);
                part = part.Substring(0, p);
                return ret;
            }
            return null;
        }
        #endregion
    }

    public class ZenExpandException : Exception
    {
        public ZenExpandException(string message) : base(message) { }
    }
}
