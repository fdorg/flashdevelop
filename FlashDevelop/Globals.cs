using FlashDevelop.Settings;
using PluginCore;

namespace FlashDevelop
{
    public class Globals
    {
        /// <summary>
        /// Quick reference to MainForm 
        /// </summary> 
        public static MainForm MainForm { get; internal set; }

        /// <summary>
        /// Quick reference to Settings 
        /// </summary>
        public static SettingObject Settings => (SettingObject)PluginBase.MainForm.Settings;
    }
}