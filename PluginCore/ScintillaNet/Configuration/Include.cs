using System;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [Serializable()]
    public class include : ConfigItem
    {
        [XmlAttribute()]
        public string file;
        
    }
    
}
