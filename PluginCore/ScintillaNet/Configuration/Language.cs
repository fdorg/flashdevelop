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
        
        [XmlElement(ElementName="line-comment")]
        public string linecomment;
        
        [XmlElement(ElementName="comment-start")]
        public string commentstart;
        
        [XmlElement(ElementName="comment-end")]
        public string commentend;
        
        [XmlElement(ElementName="file-extensions")]
        public string fileextensions;
        
        [XmlElement(ElementName="character-class")]
        public CharacterClass characterclass;

        [XmlElement(ElementName="editor-style")]
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
            if (usekeywords == null) usekeywords = new UseKeyword[0];
            if (usestyles == null) usestyles = new UseStyle[0];
            for (int j = 0; j<usekeywords.Length; j++)
            {
                usekeywords[j].init(utility, _parent);
            }
            for (int i = 0; i<usestyles.Length; i++)
            {
                usestyles[i].language = this;
                usestyles[i].init(utility, _parent);
            }
            if (lexer != null) lexer.init(utility, _parent);
            if (characterclass == null) characterclass = new CharacterClass();
            characterclass.init(utility, _parent);
            if (editorstyle != null) editorstyle.init(utility, _parent);
        }

        public void AddExtension(string extension)
        {
            if (!HasExtension(extension))
            {
                if (String.IsNullOrEmpty(fileextensions))
                    fileextensions = extension;
                else
                    fileextensions += "," + extension;
            }
        }

        public bool RemoveExtension(string extension)
        {
            var extensions = new List<string>(fileextensions.Split(','));
            bool anyRemoved = extensions.RemoveAll(s => s == extension) > 0;
            fileextensions = String.Join(",", extensions.ToArray());
            return anyRemoved;
        }

        public bool HasExtension(string extension)
        {
            return fileextensions.Split(',').Contains(extension);
        }

        public override string ToString()
        {
            return name;
        }

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
