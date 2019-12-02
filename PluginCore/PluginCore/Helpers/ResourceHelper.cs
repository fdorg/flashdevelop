// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        public static Bitmap LoadBitmap(string filename)
        {
            string prefix = "PluginCore.PluginCore.Resources.";
            Assembly assebly = Assembly.GetExecutingAssembly();
            return LoadBitmap(assebly, prefix + filename);
        }

        /// <summary>
        /// Loads a bitmap from the specified assembly
        /// </summary>
        public static Bitmap LoadBitmap(Assembly assembly, string fullpath)
        {
            Stream stream = assembly.GetManifestResourceStream(fullpath);
            return new Bitmap(stream);
        }

    }

}
