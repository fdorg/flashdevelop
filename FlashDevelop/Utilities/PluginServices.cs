using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;

namespace FlashDevelop.Utilities 
{
    internal class PluginServices
    {
        public static List<string> KnownDLLs = new List<string>();
        public static List<AvailablePlugin> AvailablePlugins = new List<AvailablePlugin>();
        public static int REQUIRED_API_LEVEL = 1;
        
        /// <summary>
        /// Finds plugins from the specified folder
        /// </summary>
        public static void FindPlugins(string directory)
        {
            EnsureUpdatedPlugins(directory);
            foreach (var fileName in Directory.GetFiles(directory, "*.dll"))
            {
                var name = Path.GetFileNameWithoutExtension(fileName);
                if (name != nameof(PluginCore) && !KnownDLLs.Contains(name))
                {
                    KnownDLLs.Add(name);
                    AddPlugin(fileName);
                }
            }
        }

        /// <summary>
        /// Ensures that the plugins are updated before init
        /// </summary>
        public static void EnsureUpdatedPlugins(string path)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                FileHelper.EnsureUpdatedFile(file);
            }
        }

        /// <summary>
        /// Finds a plugin from the plugin collection
        /// </summary>
        public static AvailablePlugin Find(string guid) => AvailablePlugins.FirstOrDefault(plugin => plugin.Instance.Guid == guid);

        /// <summary>
        /// Disposes all available plugins that are active
        /// </summary>
        public static void DisposePlugins()
        {
            foreach (var pluginOn in AvailablePlugins)
            {
                try
                {
                    if (pluginOn.IsActive) pluginOn.Instance.Dispose();
                } 
                catch (Exception ex)
                {
                    ErrorManager.ShowError(ex);
                }
            }
            AvailablePlugins.Clear();
        }
        
        /// <summary>
        /// Adds a plugin to the plugin collection
        /// </summary>
        static void AddPlugin(string fileName)
        {
            try
            {
                Assembly pluginAssembly = Assembly.LoadFrom(fileName);
                foreach (Type pluginType in pluginAssembly.GetTypes())
                {
                    if (!pluginType.IsPublic || pluginType.IsAbstract) continue;
                    var typeInterface = pluginType.GetInterface("PluginCore.IPlugin", true);
                    if (typeInterface is null) continue;
                    var newPlugin = new AvailablePlugin(fileName)
                    {
                        Instance = (IPlugin) Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()))
                    };
                    if (newPlugin.Instance.Api != REQUIRED_API_LEVEL)
                    {
                        // Invalid plugin, ignore...
                        throw new Exception("Required API level does not match.");
                    }
                    if (!PluginBase.Settings.DisabledPlugins.Contains(newPlugin.Instance.Guid))
                    {
                        newPlugin.Instance.Initialize();
                        newPlugin.IsActive = true;
                    }
                    if (!AvailablePlugins.Contains(newPlugin)) AvailablePlugins.Add(newPlugin);
                }
            }
            catch (Exception ex)
            {
                var message = TextHelper.GetString("Info.UnableToLoadPlugin");
                ErrorManager.ShowWarning(message + " \n" + fileName, ex);
            }
        }
    }

    public class AvailablePlugin
    {
        public bool IsActive;
        public string Assembly;
        public IPlugin Instance;

        public AvailablePlugin(string assembly) => Assembly = assembly;
    }
}