using System.IO;
using System.Linq;
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
            var ext = Path.GetExtension(path).ToLower();
            return Settings.AlwaysOpenLocal.Any(item => ext == item);
        }

        /// <summary>
        /// Open a shared document by the associated application of the host
        /// </summary>
        public static void RemoteOpen(string shared)
        {
            if (remoteClient is null || !remoteClient.Connected) remoteClient = new BridgeClient();
            if (remoteClient.Connected)
            {
                PluginBase.MainForm.StatusStrip.Items[0].Text = "  Opening document in host system...";
                remoteClient.Send("open:" + shared);
            }
            else TraceManager.AddAsync("Unable to connect to FlashDevelop Bridge.");
        }

        #endregion

        public static bool IsRemote(string path) => Active && path != null && path.StartsWithOrdinal(Settings.SharedDrive);
    }
}
