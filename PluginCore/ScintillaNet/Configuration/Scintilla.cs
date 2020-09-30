using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [Serializable]
    public class Scintilla : ConfigFile
    {
        Language[] _languages;
        
        [XmlArrayItem("value")]
        [XmlArray("globals")]
        public Value[] globals;

        [XmlArray("style-classes")]
        [XmlArrayItem("style-class")]
        public StyleClass[] styleclasses;

        [XmlArrayItem("keyword-class")]
        [XmlArray("keyword-classes")]
        public KeywordClass[] keywordclass;

        [XmlArrayItem("language")]
        [XmlArray("languages")]
        public Language[] languages;

        [XmlArrayItem("charcacter-class")]
        [XmlArray("character-classes")]
        public CharacterClass[] characterclasses;

        protected override Scintilla ChildScintilla => this;

        public bool IsKnownFile(string file)
        {
            var extension = Path.GetExtension(file).ToLower().Substring(1);
            var mask = "," + extension + ",";
            foreach (var lang in AllLanguages)
            {
                var extensions = "," + lang.fileextensions + ",";
                if (extensions.Contains(mask)) return true;
            }
            return false;
        }

        public string GetLanguageFromFile(string file)
        {
            var result = "text";
            var extension = Path.GetExtension(file);
            if (extension.Length == 0) return result;
            extension = extension.ToLower().Substring(1);
            var mask = "," + extension + ",";
            foreach (var lang in AllLanguages)
            {
                var extensions = "," + lang.fileextensions + ",";
                if (extensions.Contains(",*,")) result = lang.name;
                if (extensions.Contains(mask)) return lang.name;
            }
            return result;
        }
        
        public Language[] AllLanguages
        {
            get
            {
                if (_languages != null) return _languages;
                var result = new HashSet<Language>();
                if (MasterScintilla == this)
                {
                    // Check the children first (from the end)
                    for (var i = includedFiles.Length - 1; i > -1; i--)
                    {
                        var child = (Scintilla)includedFiles[i];
                        foreach (var language in child.AllLanguages)
                        {
                            result.Add(language);
                        }
                    }
                }
                // Otherwise just check here.
                for (var i = languages.Length - 1; i > -1; i--)
                {
                    result.Add(languages[i]);
                }
                _languages = new Language[result.Count];
                result.CopyTo(_languages, 0);
                return _languages;  
            }
        }

        public Value GetValue(string keyName)
        {
            Value result = null;
            if (MasterScintilla == this)
            {
                // Check the children first (from the end)
                for (int i = includedFiles.Length-1; i>-1; i--)
                {
                    var child = (Scintilla)includedFiles[i];
                    result = child.GetValue(keyName);
                    if (result != null) return result;
                }
            }
            // Other wise just check here.
            for (int i = globals.Length-1; i>-1; i--)
            {
                if (globals[i].name.Equals(keyName)) result = globals[i];
            }
            return result;  
        }
        
        public CharacterClass GetCharacterClass(string keyName)
        {
            CharacterClass result = null;
            if (MasterScintilla == this)
            {
                // Check the children first (from the end)
                for (int i = includedFiles.Length-1; i>-1; i--)
                {
                    var child = (Scintilla)includedFiles[i];
                    result = child.GetCharacterClass(keyName);
                    if (result != null) return result;
                }
            }
            // Other wise just check here.
            for (int i = characterclasses.Length-1; i>-1 ;i--)
            {
                if (characterclasses[i].name.Equals(keyName)) result = characterclasses[i];
            }
            return result;  
        }


        public StyleClass GetStyleClass(string styleName)
        {
            StyleClass result = null;
            if (MasterScintilla == this)
            {
                // Check the children first (from the end)
                for (int i = includedFiles.Length-1; i>-1; i--)
                {
                    var child = (Scintilla)includedFiles[i];
                    result = child.GetStyleClass(styleName);
                    if (result != null) return result;
                }
            }
            // Other wise just check here.
            for (int i = styleclasses.Length-1; i>-1; i--)
            {
                if (styleclasses[i].name.Equals(styleName)) result = styleclasses[i];
            }
            return result;  
        }

        public KeywordClass GetKeywordClass(string keywordName)
        {
            KeywordClass result = null;
            if (MasterScintilla == this)
            {
                // Check the children first (from the end)
                for (int i = includedFiles.Length-1; i>-1; i--)
                {
                    var child = (Scintilla)includedFiles[i];
                    result = child.GetKeywordClass(keywordName);
                    if (result != null) return result;
                }
            }
            // Other wise just check here.
            for (int i = keywordclass.Length-1; i>-1; i--)
            {
                if (keywordclass[i].name.Equals(keywordName)) result = keywordclass[i];
            }
            return result;  
        }

        public Language GetLanguage(string languageName) => GetLanguages().Find(language => language.name == languageName);

        public List<Language> GetLanguages()
        {
            var allLanguages = new List<Language>();
            if (MasterScintilla == this)
            {
                // Check the children first (from the end)
                for (int i = includedFiles.Length - 1; i > -1; i--)
                {
                    var child = (Scintilla)includedFiles[i];
                    allLanguages.AddRange(child.GetLanguages());
                }
            }
            // Other wise just check here.
            for (int i = languages.Length - 1; i > -1; i--)
            {
                allLanguages.Add(languages[i]);
            }
            return allLanguages;
        }

        public override void init(ConfigurationUtility utility, ConfigFile theParent)
        {
            base.init(utility, theParent);
            languages ??= Array.Empty<Language>();
            styleclasses ??= Array.Empty<StyleClass>();
            keywordclass ??= Array.Empty<KeywordClass>();
            globals ??= Array.Empty<Value>();
            foreach (var it in languages)
            {
                it.init(utility, _parent);
            }
            foreach (var it in styleclasses)
            {
                it.init(utility, _parent);
            }
            foreach (var it in keywordclass)
            {
                it.init(utility, _parent);
            }
            foreach (var it in globals)
            {
                it.init(utility, _parent);
            }
            characterclasses ??= Array.Empty<CharacterClass>();
            foreach (var it in characterclasses)
            {
                it.init(utility, _parent);
            }
        }
    }
}