// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;

namespace JavaCompatibleClasses
{
    public class SystemProperties
    {
        static private Dictionary<String, String> m_Properties = new Dictionary<string,string>();

        static public NameValueCollection getProperties()
        {
            NameValueCollection props = new NameValueCollection();

            foreach (KeyValuePair<String, String> pair in m_Properties)
            {
                props.Add(pair.Key, pair.Value);
            }

            return props;
        }

        static public String getProperty(String name)
        {
            if (m_Properties.ContainsKey(name))
            {
                return m_Properties[name];
            }
            else
            {
                switch (name)
                {
                    case "file.encoding":
                        return "UTF-8";
                }
            }

            return null;
        }

        static public String setProperty(String name, String value)
        {
            return m_Properties[name] = value;
        }

        static public Stream getResourceAsStream(String name)
        {
            Module[] modules = Assembly.GetExecutingAssembly().GetModules(false);

            foreach (Module module in modules)
            {
                String fileName = new FileInfo(module.FullyQualifiedName).DirectoryName + Path.DirectorySeparatorChar + name;

                try
                {
                    FileStream stream = File.OpenRead(fileName);

                    return stream;
                }
                catch (SystemException)
                {
                }
            }

            return null;
        }
    }
}
