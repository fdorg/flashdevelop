// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.IO;
using System.Reflection;

namespace FlashDevelop.Helpers
{
    class ResourceHelper
    {
        /// <summary>
        /// Gets the specified resource as an stream
        /// </summary> 
        public static Stream GetStream(String name)
        {
            String prefix = "FlashDevelop.Resources.";
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(prefix + name);
        }

    }

}