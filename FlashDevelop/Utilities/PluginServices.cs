// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;

namespace FlashDevelop.Utilities 
{
    class PluginServices
    {
        public static List<string> KnownDLLs;
        public static List<AvailablePlugin> AvailablePlugins;
        public static int REQUIRED_API_LEVEL = 1;
        
        static PluginServices()
        {
            KnownDLLs = new List<string>();
            AvailablePlugins = new List<AvailablePlugin>();
        }

        /// <summary>
        /// Finds plugins from the specified folder
        /// </summary>
        public static void FindPlugins(string path)
        {
            EnsureUpdatedPlugins(path);
            foreach (string fileOn in Directory.GetFiles(path, "*.dll"))
            {
                string name = Path.GetFileNameWithoutExtension(fileOn);
                if (name != nameof(PluginCore) && !KnownDLLs.Contains(name))
                {
                    KnownDLLs.Add(name);
                    AddPlugin(fileOn);
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
        public static AvailablePlugin Find(string guid)
        {
            foreach (AvailablePlugin plugin in AvailablePlugins)
            {
                if (plugin.Instance.Guid == guid)
                {
                    return plugin;
                }
            }
            return null;
        }

        /// <summary>
        /// Disposes all available plugins that are active
        /// </summary>
        public static void DisposePlugins()
        {
            foreach (AvailablePlugin pluginOn in AvailablePlugins)
            {
                try
                {
                    if (pluginOn.IsActive)
                    {
                        pluginOn.Instance.Dispose();
                    }
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
        private static void AddPlugin(string fileName)
        {
            try
            {
                Assembly pluginAssembly = Assembly.LoadFrom(fileName);
                foreach (Type pluginType in pluginAssembly.GetTypes())
                {
                    if (!pluginType.IsPublic || pluginType.IsAbstract) continue;
                    Type typeInterface = pluginType.GetInterface("PluginCore.IPlugin", true);
                    if (typeInterface is null) continue;
                    AvailablePlugin newPlugin = new AvailablePlugin(fileName);
                    newPlugin.Instance = (IPlugin)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));
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
                    if (!AvailablePlugins.Contains(newPlugin))
                    {
                        AvailablePlugins.Add(newPlugin);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = TextHelper.GetString("Info.UnableToLoadPlugin");
                ErrorManager.ShowWarning(message + " \n" + fileName, ex);
            }
        }
    }

    public class AvailablePlugin
    {
        public bool IsActive;
        public string Assembly;
        public IPlugin Instance;

        public AvailablePlugin(string assembly)
        {
            Assembly = assembly;
        }
    }
}