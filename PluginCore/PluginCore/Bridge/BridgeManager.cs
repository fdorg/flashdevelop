// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.IO;
using PluginCore.Managers;

namespace PluginCore.Bridge
{
    public class BridgeManager
    {
        static BridgeClient remoteClient;

        #region Properties

        /// <summary>
        /// Enable delegation to host system
        /// </summary>
        public static bool Active => Settings?.Active ?? false;

        /// <summary>
        /// Bridge configuration is externalized in a plugin
        /// </summary>
        public static IBridgeSettings Settings { get; set; }

        #endregion

        #region Tool functions

        /// <summary>
        /// Check if the provided file type should always be executed under Windows
        /// </summary>
        public static bool AlwaysOpenLocal(string path)
        {
            if (!Active) return true;
            string ext = Path.GetExtension(path).ToLower();
            foreach (string item in Settings.AlwaysOpenLocal)
                if (ext == item) return true;
            return false;
        }

        /// <summary>
        /// Open a shared document by the associated application of the host
        /// </summary>
        public static void RemoteOpen(string shared)
        {
            if (remoteClient is null || !remoteClient.Connected)
                remoteClient = new BridgeClient();
            if (remoteClient.Connected)
            {
                PluginBase.MainForm.StatusStrip.Items[0].Text = "  Opening document in host system...";
                remoteClient.Send("open:" + shared);
            }
            else TraceManager.AddAsync("Unable to connect to FlashDevelop Bridge.");
        }

        /// <summary>
        /// Open the console in the shared location on the host
        /// </summary>
        /*static public void RemoteConsole(string shared)
        {
            if (remoteClient is null || !remoteClient.Connected)
                remoteClient = new BridgeClient();
            if (remoteClient != null && remoteClient.Connected)
            {
                PluginBase.MainForm.StatusStrip.Items[0].Text = "  Opening console in host system...";
                remoteClient.Send("console:" + shared);
            }
            else TraceManager.AddAsync("Unable to connect to host bridge.");
        }*/

        #endregion

        public static bool IsRemote(string path)
        {
            return Active && path != null && path.StartsWithOrdinal(Settings.SharedDrive);
        }
    }
}
