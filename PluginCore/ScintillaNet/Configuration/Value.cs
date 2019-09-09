using System;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [Serializable()]
    public class Value : ConfigItem
    {
        [XmlAttribute()]
        public string name;

        [XmlText()]
        public string val;
        
    }
    
}
