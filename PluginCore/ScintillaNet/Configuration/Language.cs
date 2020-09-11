using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using PluginCore.Helpers;

namespace ScintillaNet.Configuration
{
    [Serializable]
    public class Language : ConfigItem
    {
        public Lexer lexer;
        
        [XmlAttribute]
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

        public UseStyle GetUseStyle(int style) => usestyles.FirstOrDefault(us => us.key == style);

        public UseKeyword GetUseKeyword(int key) => usekeywords.FirstOrDefault(uk => uk.key == key);

        public override void init(ConfigurationUtility utility, ConfigFile theParent)
        {
            base.init(utility, theParent);
            usekeywords ??= Array.Empty<UseKeyword>();
            usestyles ??= Array.Empty<UseStyle>();
            foreach (var it in usekeywords)
            {
                it.init(utility, _parent);
            }
            foreach (var it in usestyles)
            {
                it.language = this;
                it.init(utility, _parent);
            }

            lexer?.init(utility, _parent);
            characterclass ??= new CharacterClass();
            characterclass.init(utility, _parent);
            editorstyle?.init(utility, _parent);
        }

        public void AddExtension(string extension)
        {
            if (HasExtension(extension)) return;
            if (string.IsNullOrEmpty(fileextensions))
                fileextensions = extension;
            else
                fileextensions += "," + extension;
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
            var filePath = Path.Combine(PathHelper.SettingDir, "Languages", $"{name}.xml");
            var doc = new XmlDocument();
            doc.Load(filePath);
            var node = doc.SelectSingleNode("/Scintilla/languages/language/file-extensions");
            if (node != null) node.InnerText = fileextensions;
            doc.Save(filePath);
        }
    }
}