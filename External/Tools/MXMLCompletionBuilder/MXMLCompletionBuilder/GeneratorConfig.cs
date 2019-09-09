// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace MXMLCompletionBuilder
{
    [Serializable]
    public class GeneratorConfig
    {
        public string[] FlexFrameworkSWCs;
        public string[] Classpath;
        public SerializableDictionary<string, string> IncludePackage;
        public string[] ForceMxNamespace;
        public SerializableDictionary<string, string> BuiltInTags;
        public string[] LeafTags;
        public string[] ContainerTags;

        static public void Serialize(String fileName, GeneratorConfig obj)
        {
            XmlSerializer xs = new XmlSerializer(obj.GetType());
            StreamWriter writer = File.CreateText(fileName);
            xs.Serialize(writer, obj);
            writer.Flush();
            writer.Close();
        }

        static public GeneratorConfig Deserialize(String fileName)
        {
            XmlSerializer xs = new XmlSerializer(typeof(GeneratorConfig));
            StreamReader reader = File.OpenText(fileName);
            GeneratorConfig obj = xs.Deserialize(reader) as GeneratorConfig;
            reader.Close();
            return obj;
        }
    }



    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");

                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
    }
}

