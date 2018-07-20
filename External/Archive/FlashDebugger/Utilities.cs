﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace FlashDebugger
{
    public class Util
    {
        public class SerializeXML<T>
        {
            public static void SaveFile(string filename, T obj)
            {
                XmlSerializer serializer1 = new XmlSerializer(typeof(T));
                FileStream fs1 = new FileStream(filename, FileMode.Create);
                serializer1.Serialize(fs1, obj);
                fs1.Close();
            }

            public static T LoadFile(string filename)
            {
                XmlSerializer serializer2 = new XmlSerializer(typeof(T));
                FileStream fs2 = new FileStream(filename, FileMode.Open);
                T loadClasses = (T)serializer2.Deserialize(fs2);
                fs2.Close();
                return loadClasses;
            }

            public static T LoadString(string s)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                StringReader str = new StringReader(s);
                T loadClasses = (T)serializer.Deserialize(str);
                str.Close();
                return loadClasses;
            }
        }
    }

}
