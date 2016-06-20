using System.IO;
using System.Xml.Serialization;

namespace FlashDebugger
{
    public class Util
    {
        public class SerializeXML<T>
        {
            public static void SaveFile(string filename, T obj)
            {
                XmlSerializer serializer1 = XmlSerializer.FromTypes(new[]{typeof(T)})[0];
                FileStream fs1 = new FileStream(filename, FileMode.Create);
                serializer1.Serialize(fs1, obj);
                fs1.Close();
            }

            public static T LoadFile(string filename)
            {
                XmlSerializer serializer2 = XmlSerializer.FromTypes(new[]{typeof(T)})[0];
                FileStream fs2 = new FileStream(filename, FileMode.Open);
                T loadClasses = (T)serializer2.Deserialize(fs2);
                fs2.Close();
                return loadClasses;
            }

            public static T LoadString(string s)
            {
                XmlSerializer serializer = XmlSerializer.FromTypes(new[]{typeof(T)})[0];
                StringReader str = new StringReader(s);
                T loadClasses = (T)serializer.Deserialize(str);
                str.Close();
                return loadClasses;
            }
        }
    }

}
