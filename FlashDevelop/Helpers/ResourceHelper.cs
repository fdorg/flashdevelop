// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.IO;
using System.Reflection;

namespace FlashDevelop.Helpers
{
    class ResourceHelper
    {
        /// <summary>
        /// Gets the specified resource as an stream
        /// </summary> 
        public static Stream GetStream(string name) => Assembly.GetExecutingAssembly().GetManifestResourceStream($"FlashDevelop.Resources.{name}");
    }
}