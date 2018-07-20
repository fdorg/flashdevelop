// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace PluginCore.Helpers
{
    class ResourceHelper
    {
        /// <summary>
        /// Loads a bitmap from the internal resources
        /// </summary>
        public static Bitmap LoadBitmap(String filename)
        {
            String prefix = "PluginCore.PluginCore.Resources.";
            Assembly assebly = Assembly.GetExecutingAssembly();
            return LoadBitmap(assebly, prefix + filename);
        }

        /// <summary>
        /// Loads a bitmap from the specified assembly
        /// </summary>
        public static Bitmap LoadBitmap(Assembly assembly, String fullpath)
        {
            Stream stream = assembly.GetManifestResourceStream(fullpath);
            return new Bitmap(stream);
        }

    }

}
