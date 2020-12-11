// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Drawing;
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
            var assembly = Assembly.GetExecutingAssembly();
            return LoadBitmap(assembly, $"PluginCore.PluginCore.Resources.{filename}");
        }

        /// <summary>
        /// Loads a bitmap from the specified assembly
        /// </summary>
        public static Bitmap LoadBitmap(Assembly assembly, string fullpath)
        {
            var stream = assembly.GetManifestResourceStream(fullpath);
            return new Bitmap(stream);
        }
    }
}