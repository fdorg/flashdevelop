using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using PluginCore;
using PluginCore.Helpers;

namespace ScintillaNet.Configuration
{
    public class ConfigurationUtility
    {
        protected Assembly _assembly;

        private const string coloringStart = "<!-- COLORING_START -->";
        private const string coloringEnd = "<!-- COLORING_END -->";

        protected virtual byte[] LoadFile(string filename, ConfigFile parent)
        {
            var res = OpenFile(filename, parent);
            if (res != null)
            {
                var buf = new byte[res.Length];
                res.Read(buf ,0 ,buf.Length);
                return buf;
            }
            return null;
        }

        protected virtual Stream OpenFile(string filename, ConfigFile parent)
        {
            Stream res;
            filename = filename.Replace("$(AppDir)", PathHelper.AppDir);
            filename = filename.Replace("$(UserAppDir)", PathHelper.UserAppDir);
            filename = filename.Replace("$(BaseDir)", PathHelper.BaseDir);
            if (File.Exists(filename)) res = new FileStream(filename, FileMode.Open, FileAccess.Read);
            else res = _assembly.GetManifestResourceStream($"{_assembly.GetName().Name}.{filename.Replace("\\", ".")}");
            if (res is null && parent?.filename != null)
            {
                int p = parent.filename.LastIndexOf('\\');
                if (p > 0) return OpenFile($"{parent.filename.Substring(0, p)}\\{filename}", null);
            }
            return res;
        }

        protected object Deserialize(TextReader reader, Type aType)
        {
            XmlSerializer xmlSerializer = XmlSerializer.FromTypes(new[]{aType})[0];
            object local = xmlSerializer.Deserialize(reader);
            reader.Close();
            return local;
        }

        public ConfigurationUtility(Assembly assembly)
        {
            _assembly = assembly;
        }

        public virtual object LoadConfiguration(ConfigFile parent)
        {
            return LoadConfiguration(typeof(Scintilla), "ScintillaNET.xml", parent);
        }

        public virtual object LoadConfiguration(string filename, ConfigFile parent)
        {
            return LoadConfiguration(typeof(Scintilla), filename, parent);
        }

        public virtual object LoadConfiguration(Type configType, ConfigFile parent)
        {
            return LoadConfiguration(configType, "ScintillaNET.xml", parent);
        }

        public virtual object LoadConfiguration(Type configType, string filename, ConfigFile parent)
        {
            ConfigFile configFile = null;
            filename = filename.Replace("$(AppDir)", PathHelper.AppDir);
            filename = filename.Replace("$(UserAppDir)", PathHelper.UserAppDir);
            filename = filename.Replace("$(BaseDir)", PathHelper.BaseDir);
            if (typeof(ConfigFile).IsAssignableFrom(configType))
            {
                if (File.Exists(filename + ".override"))
                {
                    try
                    {
                        string original = File.ReadAllText(filename);
                        string overriding = File.ReadAllText(filename + ".override");
                        string tabContent = overriding.Replace("\n", "\n\t\t\t");
                        int indexStart = original.IndexOfOrdinal(coloringStart);
                        int indexEnd = original.IndexOfOrdinal(coloringEnd);
                        if (indexStart > -1)
                        {
                            string replaceTarget = original.Substring(indexStart, indexEnd - indexStart + coloringEnd.Length);
                            string finalContent = original.Replace(replaceTarget, tabContent);
                            File.WriteAllText(filename, finalContent);
                            File.Delete(filename + ".override");
                        }
                    }
                    catch { /* NO ERRORS... */ }
                }
                TextReader textReader = new StreamReader(filename);
                configFile = Deserialize(textReader, configType) as ConfigFile;
                configFile.filename = filename;
                configFile.init(this, parent);
            }
            return configFile;
        }

        public virtual object LoadConfiguration()
        {
            return LoadConfiguration(typeof(Scintilla), "ScintillaNET.xml", null);
        }

        public virtual object LoadConfiguration(string filename)
        {
            return LoadConfiguration(typeof(Scintilla), filename, null);
        }

        public virtual object LoadConfiguration(Type configType)
        {
            return LoadConfiguration(configType, "ScintillaNET.xml", null);
        }

        public virtual object LoadConfiguration(Type configType, string filename)
        {
            return LoadConfiguration(configType, filename, null);
        }

        public virtual object LoadConfiguration(string[] files)
        {
            Scintilla configFile = new Scintilla();
            List<include> includes = new List<include>();
            foreach (var file in files)
            {
                include inc = new include();
                inc.file = file;
                includes.Add(inc);
            }
            configFile.includes = includes.ToArray();
            configFile.init(this, null);
            return configFile;
        }
        
    }

}
