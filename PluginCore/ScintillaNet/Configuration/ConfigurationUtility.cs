using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using PluginCore;
using PluginCore.Helpers;

namespace ScintillaNet.Configuration
{
    public class ConfigurationUtility
    {
        protected Assembly _assembly;

        const string coloringStart = "<!-- COLORING_START -->";
        const string coloringEnd = "<!-- COLORING_END -->";

        protected virtual byte[] LoadFile(string filename, ConfigFile parent)
        {
            var res = OpenFile(filename, parent);
            if (res is null) return null;
            var result = new byte[res.Length];
            res.Read(result, 0, result.Length);
            return result;
        }

        protected virtual Stream OpenFile(string filename, ConfigFile parent)
        {
            filename = filename.Replace("$(AppDir)", PathHelper.AppDir);
            filename = filename.Replace("$(UserAppDir)", PathHelper.UserAppDir);
            filename = filename.Replace("$(BaseDir)", PathHelper.BaseDir);
            var result = File.Exists(filename) ? new FileStream(filename, FileMode.Open, FileAccess.Read) : _assembly.GetManifestResourceStream($"{_assembly.GetName().Name}.{filename.Replace("\\", ".")}");
            if (result is null && parent?.filename != null)
            {
                int p = parent.filename.LastIndexOf('\\');
                if (p > 0) return OpenFile($"{parent.filename.Substring(0, p)}\\{filename}", null);
            }
            return result;
        }

        protected object Deserialize(TextReader reader, Type aType)
        {
            var xmlSerializer = XmlSerializer.FromTypes(new[]{aType})[0];
            var result = xmlSerializer.Deserialize(reader);
            reader.Close();
            return result;
        }

        public ConfigurationUtility(Assembly assembly) => _assembly = assembly;

        public virtual object LoadConfiguration(ConfigFile parent) => LoadConfiguration(typeof(Scintilla), "ScintillaNET.xml", parent);

        public virtual object LoadConfiguration(string filename, ConfigFile parent) => LoadConfiguration(typeof(Scintilla), filename, parent);

        public virtual object LoadConfiguration(Type configType, ConfigFile parent) => LoadConfiguration(configType, "ScintillaNET.xml", parent);

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

        public virtual object LoadConfiguration() => LoadConfiguration(typeof(Scintilla), "ScintillaNET.xml", null);

        public virtual object LoadConfiguration(string filename) => LoadConfiguration(typeof(Scintilla), filename, null);

        public virtual object LoadConfiguration(Type configType) => LoadConfiguration(configType, "ScintillaNET.xml", null);

        public virtual object LoadConfiguration(Type configType, string filename) => LoadConfiguration(configType, filename, null);

        public virtual object LoadConfiguration(string[] files)
        {
            var result = new Scintilla {includes = files.Select(static it => new include {file = it}).ToArray()};
            result.init(this, null);
            return result;
        }
    }
}