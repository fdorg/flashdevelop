using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace AppMan.Utilities
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
                DialogHelper.ShowError(ex.ToString());
            }
        }

        /// <summary>
        /// Deserializes the specified object from a xml file. Optionally includes out commented extras.
        /// </summary>
        public static Object Deserialize(String file, Object obj)
        {
            return Deserialize(file, obj, false);
        }
        public static Object Deserialize(String file, Object obj, Boolean extras)
        {
            try
            {
                using (TextReader reader = new StreamReader(file, Encoding.UTF8, false))
                {
                    String xmlData = reader.ReadToEnd();
                    if (extras) xmlData = xmlData.Replace("<!--EXTRAS", "").Replace("EXTRAS-->", "");
                    XmlSerializer serializer = new XmlSerializer(obj.GetType());
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
