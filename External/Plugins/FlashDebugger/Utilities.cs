// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
                var serializer = XmlSerializer.FromTypes(new[]{typeof(T)})[0];
                var stream = new FileStream(filename, FileMode.Create);
                serializer.Serialize(stream, obj);
                stream.Close();
            }

            public static T LoadFile(string filename)
            {
                var serializer = XmlSerializer.FromTypes(new[]{typeof(T)})[0];
                var stream = new FileStream(filename, FileMode.Open);
                var loadClasses = (T)serializer.Deserialize(stream);
                stream.Close();
                return loadClasses;
            }

            public static T LoadString(string s)
            {
                var serializer = XmlSerializer.FromTypes(new[]{typeof(T)})[0];
                var reader = new StringReader(s);
                var loadClasses = (T)serializer.Deserialize(reader);
                reader.Close();
                return loadClasses;
            }
        }
    }

}
