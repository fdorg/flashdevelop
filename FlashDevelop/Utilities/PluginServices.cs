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
        public static List<String> KnownDLLs;
        public static List<AvailablePlugin> AvailablePlugins;
        public static Int32 REQUIRED_API_LEVEL = 1;
        
        static PluginServices()
        {
            KnownDLLs = new List<String>();
            AvailablePlugins = new List<AvailablePlugin>();
        }

        /// <summary>
        /// Finds plugins from the specified folder
        /// </summary>
        public static void FindPlugins(String path)
        {
            EnsureUpdatedPlugins(path);
            foreach (String fileOn in Directory.GetFiles(path, "*.dll"))
            {
                String name = Path.GetFileNameWithoutExtension(fileOn);
                if (name != "PluginCore" && !KnownDLLs.Contains(name))
                {
                    KnownDLLs.Add(name);
                    AddPlugin(fileOn);
                }
            }
        }

        /// <summary>
        /// Ensures that the plugins are updated before init
        /// </summary>
        public static void EnsureUpdatedPlugins(String path)
        {
            foreach (String file in Directory.GetFiles(path))
            {
                FileHelper.EnsureUpdatedFile(file);
            }
        }

        /// <summary>
        /// Finds a plugin from the plugin collection
        /// </summary>
        public static AvailablePlugin Find(String guid)
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
        private static void AddPlugin(String fileName)
        {
            Assembly pluginAssembly = Assembly.LoadFrom(fileName);
            try
            {
                foreach (Type pluginType in pluginAssembly.GetTypes())
                {
                    if (pluginType.IsPublic && !pluginType.IsAbstract)
                    {
                        Type typeInterface = pluginType.GetInterface("PluginCore.IPlugin", true);
                        if (typeInterface != null)
                        {
                            AvailablePlugin newPlugin = new AvailablePlugin(fileName);
                            newPlugin.Instance = (IPlugin)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));
                            if (newPlugin.Instance.Api != REQUIRED_API_LEVEL)
                            {
                                // Invalid plugin, ignore...
                                throw new Exception("Required API level does not match.");
                            }
                            if (!Globals.Settings.DisabledPlugins.Contains(newPlugin.Instance.Guid))
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
                }
            }
            catch (Exception ex)
            {
                String message = TextHelper.GetString("Info.UnableToLoadPlugin");
                ErrorManager.ShowWarning(message + " \n" + fileName, ex);
            }
        }
    }

    public class AvailablePlugin
    {
        public Boolean IsActive = false;
        public String Assembly = String.Empty;
        public IPlugin Instance = null;

        public AvailablePlugin(String assembly)
        {
            this.Assembly = assembly;
        }
        
    }
    
}
