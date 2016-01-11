using System;
using System.Collections;
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

        protected virtual Scintilla ChildScintilla
        {
            get { return null; }
        }

        public virtual Scintilla MasterScintilla
        {
            get { return (_parent == this) ? ChildScintilla : _parent.MasterScintilla; }
        }

        public virtual void addIncludedFile(ConfigFile file)
        {
            ConfigFile[] configFiles = null;
            configFiles = new ConfigFile[includedFiles.Length + 1];
            includedFiles.CopyTo(configFiles, 0);
            configFiles[includedFiles.Length] = file;
            includedFiles = configFiles;
        }

        public override void init(ConfigurationUtility utility, ConfigFile theParent)
        {
            ConfigFile configFile = null;
            includedFiles = new ConfigFile[0];
            base.init(utility, theParent);
            if (includes == null) includes = new include[0];
            for (int j = 0; j<includes.Length; j++) includes[j].init(utility, _parent);
            for (int i = 0; i<includes.Length; i++)
            {
                configFile = (ConfigFile)utility.LoadConfiguration(base.GetType(), includes[i].file, _parent);
                addIncludedFile(configFile);
            }
            CollectScintillaNodes(null);
        }

        public virtual void CollectScintillaNodes(ArrayList list)
        {
            if (_parent == this)
            {
                if (list != null) return;
                list = new ArrayList();
                if (ChildScintilla != null) ChildScintilla.CollectScintillaNodes(list);
            }
            else if (list == null) return;
            ConfigFile cf;
            if (includedFiles != null)
            {
                for (int i = 0 ; i<includedFiles.Length; i++)
                {
                    cf = includedFiles[i];
                    if (cf == null) continue;
                    if (cf.ChildScintilla != null) list.Add(cf.ChildScintilla);
                    if( cf.ChildScintilla != null ) cf.ChildScintilla.CollectScintillaNodes(list);
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
