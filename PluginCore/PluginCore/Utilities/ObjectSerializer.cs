using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using PluginCore.Helpers;
using PluginCore.Managers;

namespace PluginCore.Utilities
{
    public class ObjectSerializer
    {
        private static BinaryFormatter formatter = new BinaryFormatter();

        static ObjectSerializer()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomainAssemblyResolve);
        }

        /// <summary>
        /// The BinaryFormatter may need some help finding Assemblies from various directories
        /// </summary>
        static Assembly CurrentDomainAssemblyResolve(Object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);
            String ffile = Path.Combine(PathHelper.AppDir, assemblyName.Name + ".exe");
            String afile = Path.Combine(PathHelper.AppDir, assemblyName.Name + ".dll");
            String dfile = Path.Combine(PathHelper.PluginDir, assemblyName.Name + ".dll");
            String ufile = Path.Combine(PathHelper.UserPluginDir, assemblyName.Name + ".dll");
            if (File.Exists(ffile)) return Assembly.LoadFrom(ffile);
            if (File.Exists(afile)) return Assembly.LoadFrom(afile);
            if (File.Exists(dfile)) return Assembly.LoadFrom(dfile);
            if (File.Exists(ufile)) return Assembly.LoadFrom(ufile);
            return null;
        }

        /// <summary>
        /// Serializes the specified object to a binary file
        /// </summary>
        public static void Serialize(String file, Object obj)
        {
            Int32 count = 0;
            while (true)
            {
                try
                {
                    using (FileStream stream = File.Create(file))
                    {
                        formatter.Serialize(stream, obj);
                    }
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
        public static Object Deserialize(String file, Object obj, Boolean checkValidity)
        {
            try
            {
                FileHelper.EnsureUpdatedFile(file);
                Object settings = InternalDeserialize(file, obj.GetType());
                if (checkValidity)
                {
                    Object defaults = Activator.CreateInstance(obj.GetType());
                    PropertyInfo[] properties = settings.GetType().GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        Object current = GetValue(settings, property.Name);
                        if (current == null || (current is Color && (Color)current == Color.Empty))
                        {
                            Object value = GetValue(defaults, property.Name);
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
        public static Object Deserialize(String file, Object obj)
        {
            return Deserialize(file, obj, true);
        }

        /// <summary>
        /// Fixes some common issues when serializing
        /// </summary>
        private static Object InternalDeserialize(String file, Type type)
        {
            FileInfo info = new FileInfo(file);
            if (!info.Exists)
            {
                return Activator.CreateInstance(type);
            }
            else if (info.Exists && info.Length == 0)
            {
                info.Delete();
                return Activator.CreateInstance(type);
            }
            else
            {
                using (FileStream stream = info.Open(FileMode.Open, FileAccess.Read))
                {
                    return formatter.Deserialize(stream);
                }
            }
        }

        /// <summary>
        /// Sets a value of a setting
        /// </summary>
        public static void SetValue(Object obj, String name, Object value)
        {
            try
            {
                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(name);
                if (info == null) return;
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
        public static Object GetValue(Object obj, String name)
        {
            try
            {
                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(name);
                if (info == null) return null;
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
