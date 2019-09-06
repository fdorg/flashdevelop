using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace AirProperties
{
    /// <remarks>Not thread safe.</remarks>
    class IphonePlistManager : IDictionary<string, object>
    {
        readonly Dictionary<string, object> backData = new Dictionary<string, object>();

        readonly Dictionary<string, XmlNode> nodeMapping = new Dictionary<string, XmlNode>();
        // I don't think it's worth to, slightly, raise complexity by adding this mapping to backData

        readonly XmlDocument backDoc = new XmlDocument();

        public bool RemoveOnNullValue { get; set; }

        public ICollection<string> Keys => backData.Keys;

        public ICollection<object> Values => backData.Values;

        public object this[string key]
        {
            get => backData[key];
            set
            {
                if (backData.ContainsKey(key))
                {
                    if (value != null || !RemoveOnNullValue)
                    {
                        backData[key] = value;

                        var valueNode = SerializeValue(value);
                        backDoc.FirstChild.ReplaceChild(valueNode, nodeMapping[key]);
                        nodeMapping[key] = valueNode;
                    }
                    else
                    {
                        Remove(key);
                    }
                }
                else if (value != null || !RemoveOnNullValue)
                {
                    Add(key, value);
                }
            }
        }

        public int Count => backData.Count;

        public bool IsReadOnly => false;

        public IphonePlistManager()
            : this(string.Empty)
        {
        }

        public IphonePlistManager(string plist)
        {
            if (string.IsNullOrEmpty(plist)) return;
            backDoc.LoadXml(plist);
            var contentNode = backDoc.FirstChild;

            for (int i = 0, count = contentNode.ChildNodes.Count; i < count; i++)
            {
                var keyNode = contentNode.ChildNodes[i];
                //AFAIK actual plist files do not keep comments, but we're doing it since we're not the actual thing.
                if (keyNode.NodeType != XmlNodeType.Element) continue;

                if (keyNode.Name != "key")
                    throw new Exception("Unexpected node");

                i++;
                if (i >= count) throw new Exception("Malformed plist");
                var valueNode = contentNode.ChildNodes[i];
                var key = keyNode.InnerText;
                backData[key] = GetValue(valueNode);
                nodeMapping[key] = valueNode;
            }
        }

        public void Add(string key, object value)
        {
            if (value != null || !RemoveOnNullValue)
            {
                backData.Add(key, value);

                var keyNode = backDoc.CreateElement("key");
                keyNode.InnerText = key;

                var valueNode = SerializeValue(value);
                backDoc.FirstChild.AppendChild(keyNode);
                backDoc.FirstChild.AppendChild(valueNode);
                nodeMapping.Add(key, valueNode);
            }
        }

        public bool ContainsKey(string key) => backData.ContainsKey(key);

        public bool Remove(string key)
        {
            if (!backData.Remove(key)) return false;
            var node = nodeMapping[key];
            backDoc.FirstChild.RemoveChild(node.PreviousSibling);
            backDoc.FirstChild.RemoveChild(node);
            nodeMapping.Remove(key);
            return true;
        }

        public bool TryGetValue(string key, out object value) => backData.TryGetValue(key, out value);

        public void Add(KeyValuePair<string, object> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            backData.Clear();
            nodeMapping.Clear();
            backDoc.FirstChild.RemoveAll();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)backData).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, object>>)backData).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return backData.TryGetValue(item.Key, out var value) && value == item.Value && Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => backData.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)backData).GetEnumerator();

        object GetValue(XmlNode valueNode)
        {
            return valueNode.Name switch
            {
                "string" => (object) valueNode.InnerText,
                "true" => true,
                "false" => false,
                "array" => FillArray(valueNode),
                // Some unneeded type right now
                _ => null
            };
        }

        List<object> FillArray(XmlNode arrayNode)
        {
            var arr = new List<object>(arrayNode.ChildNodes.Count);
            foreach (XmlNode childNode in arrayNode.ChildNodes)
            {
                if (childNode.NodeType != XmlNodeType.Element) continue;
                arr.Add(GetValue(childNode));
            }
            return arr;
        }

        XmlNode SerializeValue(object value)
        {
            XmlNode retVal;
            if (value is null || value is string)
            {
                retVal = backDoc.CreateElement("string");
                if (value != null) retVal.InnerText = value as string;
            }
            else if (value is bool)
            {
                retVal = backDoc.CreateElement(value.ToString().ToLower());
            }
            else if (value is IEnumerable)
            {
                retVal = backDoc.CreateElement("array");
                foreach (object subVal in (IEnumerable)value)
                {
                    retVal.AppendChild(SerializeValue(subVal));
                }
            }
            else
            {
                throw new Exception("Unsupported value type");
            }

            return retVal;
        }

        public object ValueOrNull(string key)
        {
            TryGetValue(key, out var result);
            return result;
        }

        public string GetPlistXml()
        {
            var xml = backDoc.FirstChild.InnerXml;
            var readerSettings = new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment};
            var ms = new MemoryStream();
            // Create a XMLTextWriter that will send its output to a memory stream (file)
            using var xtw = new XmlTextWriter(ms, Encoding.Unicode);
            var parserContext = new XmlParserContext(null, null, xtw.XmlLang, xtw.XmlSpace);
            try
            {
                // Load the unformatted XML text string into an XPath Document
                var doc = new XPathDocument(XmlReader.Create(new StringReader(xml), readerSettings, parserContext));
                // Set the formatting property of the XML Text Writer to indented
                // the text writer is where the indenting will be performed
                xtw.Formatting = Formatting.Indented;
                xtw.IndentChar = '\x09'; //tab
                xtw.Indentation = 1;
                // write doc xml to the xmltextwriter
                doc.CreateNavigator().WriteSubtree(xtw);
                // Flush the contents of the text writer
                // to the memory stream, which is simply a memory file
                xtw.Flush();
                // set to start of the memory stream (file)
                ms.Seek(0, SeekOrigin.Begin);
                // create a reader to read the contents of
                // the memory stream (file)
                var sr = new StreamReader(ms);
                // return the formatted string to caller (without the namespace declaration)
                return sr.ReadToEnd();
            }
            catch (Exception)
            {
                //debug purposes only
                //MessageBox.Show(ex.ToString());
                //return original xml
                return xml;
            }
        }
    }
}