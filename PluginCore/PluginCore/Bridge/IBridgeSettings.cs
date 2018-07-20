namespace PluginCore.Bridge
{
    public interface IBridgeSettings
    {
        /// <summary>
        /// Enable delegation to host system
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Provide a custom IP to override auto-detection
        /// </summary>
        string CustomIP { get; }

        /// <summary>
        /// Bridge port number (default 8007)
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Use Flash CS from the host system
        /// </summary>
        bool TargetRemoteIDE { get; }

        /// <summary>
        /// Use file explorer of the host system
        /// </summary>
        bool UseRemoteExplorer { get; }

        /// <summary>
        /// Drive mapped to shared location between host and guest system
        /// </summary>
        string SharedDrive { get; }

        /// <summary>
        /// List of file extensions which must always be executed by Windows
        /// </summary>
        string[] AlwaysOpenLocal { get; }
    }
}
