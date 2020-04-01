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