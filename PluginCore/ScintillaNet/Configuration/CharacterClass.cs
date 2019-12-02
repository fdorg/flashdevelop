// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [Serializable()]
    public class CharacterClass : ConfigItem
    {
        [XmlAttribute()]
        public string name;

        [XmlAttribute("inherit")]
        public string inherit;

        [XmlText()]
        public string val = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789_";

        public string Characters
        {
            get
            {
                string result = val;
                if (!string.IsNullOrEmpty(inherit))
                {
                    CharacterClass cc = _parent.MasterScintilla.GetCharacterClass(inherit);         
                    if (cc != null) result += cc.Characters;
                }
                return result;
            }
        }
        
    }
    
}
