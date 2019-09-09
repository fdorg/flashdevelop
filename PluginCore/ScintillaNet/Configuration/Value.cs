// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
