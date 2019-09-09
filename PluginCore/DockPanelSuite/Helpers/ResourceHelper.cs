// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Resources;

namespace WeifenLuo.WinFormsUI.Docking
{
    internal static class ResourceHelper
    {
        private static ResourceManager _resourceManager = null;

        private static ResourceManager ResourceManager
        {
            get
            {
                if (_resourceManager is null)
                    _resourceManager = new ResourceManager("WeifenLuo.WinFormsUI.Docking.Strings", typeof(ResourceHelper).Assembly);
                return _resourceManager;
            }

        }

        public static string GetString(string name)
        {
            return ResourceManager.GetString(name);
        }
    }
}
