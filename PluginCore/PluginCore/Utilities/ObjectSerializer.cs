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
            var assemblyName = new AssemblyName(args.Name);
            var ffile = Path.Combine(PathHelper.AppDir, assemblyName.Name + ".exe");
            if (File.Exists(ffile)) return Assembly.LoadFrom(ffile);
            var afile = Path.Combine(PathHelper.AppDir, assemblyName.Name + ".dll");
            if (File.Exists(afile)) return Assembly.LoadFrom(afile);
            var dfile = Path.Combine(PathHelper.PluginDir, assemblyName.Name + ".dll");
            if (File.Exists(dfile)) return Assembly.LoadFrom(dfile);
            var ufile = Path.Combine(PathHelper.UserPluginDir, assemblyName.Name + ".dll");
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
                var result = InternalDeserialize(file, obj.GetType());
                if (checkValidity)
                {
                    var defaults = Activator.CreateInstance(obj.GetType());
                    var properties = result.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        var current = GetValue(result, property.Name);
                        if (current is null || (current is Color color && color == Color.Empty))
                        {
                            var value = GetValue(defaults, property.Name);
                            SetValue(result, property.Name, value);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return obj;
            }
        }

        public static object Deserialize(string file, object obj) => Deserialize(file, obj, true);

        /// <summary>
        /// Fixes some common issues when serializing
        /// </summary>
        static object InternalDeserialize(string file, Type type)
        {
            var info = new FileInfo(file);
            if (!info.Exists) return Activator.CreateInstance(type);
            if (info.Length == 0)
            {
                info.Delete();
                return Activator.CreateInstance(type);
            }

            using var stream = info.Open(FileMode.Open, FileAccess.Read);
            return formatter.Deserialize(stream);
        }

        public static T Deserialize<T>(string file, T obj) => Deserialize(file, obj, true);

        /// <summary>
        /// Deserializes the specified object from a binary file
        /// </summary>
        public static T Deserialize<T>(string file, T obj, bool checkValidity)
        {
            try
            {
                FileHelper.EnsureUpdatedFile(file);
                var result = InternalDeserialize<T>(file);
                if (checkValidity)
                {
                    var defaults = Activator.CreateInstance<T>();
                    var properties = result.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        var current = GetValue(result, property.Name);
                        if (current is null || (current is Color color && color == Color.Empty))
                        {
                            var value = GetValue(defaults, property.Name);
                            SetValue(result, property.Name, value);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return obj;
            }
        }

        /// <summary>
        /// Fixes some common issues when serializing
        /// </summary>
        static T InternalDeserialize<T>(string file)
        {
            var info = new FileInfo(file);
            if (!info.Exists) return Activator.CreateInstance<T>();
            if (info.Length == 0)
            {
                info.Delete();
                return Activator.CreateInstance<T>();
            }

            using var stream = info.Open(FileMode.Open, FileAccess.Read);
            return (T)formatter.Deserialize(stream);
        }

        /// <summary>
        /// Sets a value of a setting
        /// </summary>
        public static void SetValue(object obj, string name, object value)
        {
            try
            {
                var type = obj.GetType();
                var info = type.GetProperty(name);
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
                var type = obj.GetType();
                var info = type.GetProperty(name);
                return info?.GetValue(obj, null);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }
    }
}