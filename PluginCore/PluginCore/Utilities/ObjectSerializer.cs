// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using PluginCore.Managers;
using PluginCore.Helpers;

namespace PluginCore.Utilities
{
    public class ObjectSerializer
    {
        static readonly BinaryFormatter formatter = new BinaryFormatter();

        static ObjectSerializer()
        {
            formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
        }

        /// <summary>
        /// The BinaryFormatter may need some help finding Assemblies from various directories
        /// </summary>
        static Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);
            string ffile = Path.Combine(PathHelper.AppDir, assemblyName.Name + ".exe");
            string afile = Path.Combine(PathHelper.AppDir, assemblyName.Name + ".dll");
            string dfile = Path.Combine(PathHelper.PluginDir, assemblyName.Name + ".dll");
            string ufile = Path.Combine(PathHelper.UserPluginDir, assemblyName.Name + ".dll");
            if (File.Exists(ffile)) return Assembly.LoadFrom(ffile);
            if (File.Exists(afile)) return Assembly.LoadFrom(afile);
            if (File.Exists(dfile)) return Assembly.LoadFrom(dfile);
            if (File.Exists(ufile)) return Assembly.LoadFrom(ufile);
            return null;
        }

        /// <summary>
        /// Serializes the specified object to a binary file
        /// </summary>
        public static void Serialize(string file, object obj)
        {
            int count = 0;
            while (true)
            {
                try
                {
                    using var stream = File.Create(file);
                    formatter.Serialize(stream, obj);
                    return;
                }
                catch (Exception ex)
                {
                    count++;
                    if (count > 10)
                    {
                        ErrorManager.ShowError(ex);
                        return;
                    }
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// Deserializes the specified object from a binary file
        /// </summary>
        public static object Deserialize(string file, object obj, bool checkValidity)
        {
            try
            {
                FileHelper.EnsureUpdatedFile(file);
                object settings = InternalDeserialize(file, obj.GetType());
                if (checkValidity)
                {
                    object defaults = Activator.CreateInstance(obj.GetType());
                    PropertyInfo[] properties = settings.GetType().GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        object current = GetValue(settings, property.Name);
                        if (current is null || (current is Color && (Color)current == Color.Empty))
                        {
                            object value = GetValue(defaults, property.Name);
                            SetValue(settings, property.Name, value);
                        }
                    }
                }
                return settings;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return obj;
            }
        }
        public static object Deserialize(string file, object obj)
        {
            return Deserialize(file, obj, true);
        }

        /// <summary>
        /// Fixes some common issues when serializing
        /// </summary>
        static object InternalDeserialize(string file, Type type)
        {
            FileInfo info = new FileInfo(file);
            if (!info.Exists)
            {
                return Activator.CreateInstance(type);
            }

            if (info.Length == 0)
            {
                info.Delete();
                return Activator.CreateInstance(type);
            }

            using FileStream stream = info.Open(FileMode.Open, FileAccess.Read);
            return formatter.Deserialize(stream);
        }

        /// <summary>
        /// Sets a value of a setting
        /// </summary>
        public static void SetValue(object obj, string name, object value)
        {
            try
            {
                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(name);
                if (info is null) return;
                info.SetValue(obj, value, null);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Gets a value of a setting as an object
        /// </summary>
        public static object GetValue(object obj, string name)
        {
            try
            {
                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(name);
                if (info is null) return null;
                return info.GetValue(obj, null);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }

    }

}
