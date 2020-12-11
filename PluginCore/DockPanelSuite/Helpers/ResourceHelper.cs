// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Resources;

namespace WeifenLuo.WinFormsUI.Docking
{
    internal static class ResourceHelper
    {
        static ResourceManager _resourceManager;

        static ResourceManager ResourceManager => _resourceManager ??= new ResourceManager("WeifenLuo.WinFormsUI.Docking.Strings", typeof(ResourceHelper).Assembly);

        public static string GetString(string name) => ResourceManager.GetString(name);
    }
}
