// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using ASDocGen.Utilities;

namespace ASDocGen.Utilities
{
    public class ObjectSerializer
    {
        /// <summary>
        /// Serializes the specified object to a xml file.
        /// </summary>
        public static void Serialize(String file, Object obj)
        {
            try
            {
                using (TextWriter writer = new StreamWriter(file, false, Encoding.UTF8))
                {
                    XmlSerializer serializer = new XmlSerializer(obj.GetType());
                    serializer.Serialize(writer, obj);
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Deserializes the specified object from a xml file.
        /// </summary>
        public static Object Deserialize(String file, Object obj)
        {
            try
            {
                using (TextReader reader = new StreamReader(file, Encoding.UTF8, false))
                {
                    XmlSerializer serializer = new XmlSerializer(obj.GetType());
                    return serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.Message);
                return null;
            }
        }

    }

}
