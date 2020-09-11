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
