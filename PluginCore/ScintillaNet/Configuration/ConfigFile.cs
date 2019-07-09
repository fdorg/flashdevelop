using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [Serializable()]
    public class ConfigFile : ConfigItem
    {
        [NonSerialized()]
        protected ConfigFile[] includedFiles;

        [XmlArray("includes")]
        [XmlArrayItem("include")]
        public include[] includes;

        [NonSerialized()]
        public string filename;

        protected virtual Scintilla ChildScintilla => null;

        public virtual Scintilla MasterScintilla => _parent == this ? ChildScintilla : _parent.MasterScintilla;

        public virtual void addIncludedFile(ConfigFile file)
        {
            var configFiles = new ConfigFile[includedFiles.Length + 1];
            includedFiles.CopyTo(configFiles, 0);
            configFiles[includedFiles.Length] = file;
            includedFiles = configFiles;
        }

        public override void init(ConfigurationUtility utility, ConfigFile theParent)
        {
            includedFiles = new ConfigFile[0];
            base.init(utility, theParent);
            if (includes is null) includes = new include[0];
            foreach (var include in includes) include.init(utility, _parent);
            foreach (var include in includes)
            {
                var configFile = (ConfigFile)utility.LoadConfiguration(base.GetType(), include.file, _parent);
                addIncludedFile(configFile);
            }
            CollectScintillaNodes(null);
        }

        public virtual void CollectScintillaNodes(List<ConfigItem> list)
        {
            if (_parent == this)
            {
                if (list != null) return;
                list = new List<ConfigItem>();
                ChildScintilla?.CollectScintillaNodes(list);
            }
            else if (list is null) return;

            if (includedFiles != null)
            {
                foreach (var cf in includedFiles)
                {
                    if (cf is null) continue;
                    if (cf.ChildScintilla != null) list.Add(cf.ChildScintilla);
                    cf.ChildScintilla?.CollectScintillaNodes(list);
                    if( cf.includedFiles != null && cf.includedFiles.Length > 0) cf.CollectScintillaNodes(list);
                }
            }
            if (_parent == this)
            {
                ChildScintilla.includedFiles = new ConfigFile[list.Count];
                list.CopyTo(ChildScintilla.includedFiles);
            }
        }
        
    }

}
