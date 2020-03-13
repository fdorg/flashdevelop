using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using PluginCore.Helpers;

namespace ScintillaNet.Configuration
{
    [Serializable()]
    public class Language : ConfigItem
    {
        public Lexer lexer;
        
        [XmlAttribute()]
        public string name;
        
        [XmlElement("line-comment")]
        public string linecomment;
        
        [XmlElement("comment-start")]
        public string commentstart;
        
        [XmlElement("comment-end")]
        public string commentend;
        
        [XmlElement("file-extensions")]
        public string fileextensions;
        
        [XmlElement("character-class")]
        public CharacterClass characterclass;

        [XmlElement("editor-style")]
        public EditorStyle editorstyle;

        [XmlArray("use-keywords")]
        [XmlArrayItem("keyword")]
        public UseKeyword[] usekeywords;

        [XmlArray("use-styles")]
        [XmlArrayItem("style")]
        public UseStyle[] usestyles;

        public UseStyle GetUseStyle(int style)
        {
            foreach (UseStyle us in this.usestyles)
            {
                if (us.key == style)
                {
                    return us;
                }
            }
            return null;
        }
        
        public UseKeyword GetUseKeyword(int key)
        {
            foreach (UseKeyword uk in this.usekeywords)
            {
                if (uk.key == key)
                {
                    return uk;
                }
            }
            return null;
        }
        
        public override void init(ConfigurationUtility utility, ConfigFile theParent)
        {
            base.init(utility, theParent);
            if (usekeywords is null) usekeywords = Array.Empty<UseKeyword>();
            if (usestyles is null) usestyles = Array.Empty<UseStyle>();
            for (int j = 0; j<usekeywords.Length; j++)
            {
                usekeywords[j].init(utility, _parent);
            }
            for (int i = 0; i<usestyles.Length; i++)
            {
                usestyles[i].language = this;
                usestyles[i].init(utility, _parent);
            }

            lexer?.init(utility, _parent);
            if (characterclass is null) characterclass = new CharacterClass();
            characterclass.init(utility, _parent);
            editorstyle?.init(utility, _parent);
        }

        public void AddExtension(string extension)
        {
            if (!HasExtension(extension))
            {
                if (string.IsNullOrEmpty(fileextensions))
                    fileextensions = extension;
                else
                    fileextensions += "," + extension;
            }
        }

        public bool RemoveExtension(string extension)
        {
            var extensions = new List<string>(fileextensions.Split(','));
            var anyRemoved = extensions.RemoveAll(s => s == extension) > 0;
            fileextensions = string.Join(",", extensions);
            return anyRemoved;
        }

        public bool HasExtension(string extension) => fileextensions.Split(',').Contains(extension);

        public override string ToString() => name;

        public void SaveExtensions()
        {
            string langPath = Path.Combine(PathHelper.SettingDir, "Languages");
            string filePath = Path.Combine(langPath, name + ".xml");
            var doc = new XmlDocument();
            doc.Load(filePath);
            XmlNode node = doc.SelectSingleNode("/Scintilla/languages/language/file-extensions");
            if (node != null)
                node.InnerText = fileextensions;
            doc.Save(filePath);
        }

    }
    
}
