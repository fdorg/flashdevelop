using System;
using System.Runtime;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [SerializableAttribute()]
    public class CharacterClass : ConfigItem
    {
        [XmlAttributeAttribute()]
        public string name;

        [XmlAttributeAttribute("inherit")]
        public string inherit;

        [XmlTextAttribute()]
        public string val = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789_";

        public string Characters
        {
            get
            {
                string result = val;
                if (inherit != null && inherit.Length > 0)
                {
                    CharacterClass cc = _parent.MasterScintilla.GetCharacterClass(inherit);         
                    if (cc != null) result += cc.Characters;
                }
                return result;
            }
        }
        
    }
    
}
