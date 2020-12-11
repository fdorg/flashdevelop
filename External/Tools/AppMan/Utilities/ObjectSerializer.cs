// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace AppMan.Utilities
{
    public class ObjectSerializer
    {
        /// <summary>
        /// Serializes the specified object to a xml file.
        /// </summary>
        public static void Serialize(string file, object obj)
        {
            try
            {
                using (TextWriter writer = new StreamWriter(file, false, Encoding.UTF8))
                {
                    var serializer = new XmlSerializer(obj.GetType());
                    serializer.Serialize(writer, obj);
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Deserializes the specified object from a xml file. Optionally exposes commented groups.
        /// </summary>
        public static object Deserialize(string file, object obj) => Deserialize(file, obj, string.Empty);

        public static object Deserialize(string file, object obj, string groups)
        {
            try
            {
                using (TextReader reader = new StreamReader(file, Encoding.UTF8, false))
                {
                    var xmlData = reader.ReadToEnd();
                    var exGroups = groups.Split(',');
                    foreach (var group in exGroups)
                    {
                        xmlData = xmlData.Replace("<!--" + group, "").Replace(group + "-->", "");
                    }
                    var serializer = new XmlSerializer(obj.GetType());
                    return serializer.Deserialize(new StringReader(xmlData));
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.ToString());
                return null;
            }
        }
    }
}