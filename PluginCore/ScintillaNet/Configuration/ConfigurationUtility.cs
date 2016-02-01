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

        private const String coloringStart = "<!-- COLORING_START -->";
        private const String coloringEnd = "<!-- COLORING_END -->";

        protected virtual byte[] LoadFile(string filename, ConfigFile parent)
        {
            Stream res;
            byte[] buf;
            res = OpenFile(filename, parent);
            if (res != null)
            {
                buf = new byte[res.Length];
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
            else res = _assembly.GetManifestResourceStream(String.Format( "{0}.{1}" , _assembly.GetName().Name, filename.Replace("\\" , "." )));
            if (res == null && parent != null && parent.filename != null)
            {
                int p = parent.filename.LastIndexOf('\\');
                if (p > 0) return OpenFile(String.Format( "{0}\\{1}", parent.filename.Substring(0, p), filename), null);
            }
            return res;
        }

        protected object Deserialize(TextReader reader, Type aType)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(aType);
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
            TextReader textReader = null;
            filename = filename.Replace("$(AppDir)", PathHelper.AppDir);
            filename = filename.Replace("$(UserAppDir)", PathHelper.UserAppDir);
            filename = filename.Replace("$(BaseDir)", PathHelper.BaseDir);
            if (typeof(ConfigFile).IsAssignableFrom(configType))
            {
                if (File.Exists(filename + ".override"))
                {
                    try
                    {
                        String original = File.ReadAllText(filename);
                        String overriding = File.ReadAllText(filename + ".override");
                        String tabContent = overriding.Replace("\n", "\n\t\t\t");
                        Int32 indexStart = original.IndexOfOrdinal(coloringStart);
                        Int32 indexEnd = original.IndexOfOrdinal(coloringEnd);
                        if (indexStart > -1)
                        {
                            String replaceTarget = original.Substring(indexStart, indexEnd - indexStart + coloringEnd.Length);
                            String finalContent = original.Replace(replaceTarget, tabContent);
                            File.WriteAllText(filename, finalContent);
                            File.Delete(filename + ".override");
                        }
                    }
                    catch { /* NO ERRORS... */ }
                }
                textReader = new StreamReader(filename);
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
            for (Int32 i = 0; i < files.Length; i++)
            {
                include inc = new include();
                inc.file = files[i];
                includes.Add(inc);
            }
            configFile.includes = includes.ToArray();
            configFile.init(this, null);
            return configFile;
        }
        
    }

}
