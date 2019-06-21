using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using FlashViewer.Controls;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace FlashViewer
{
    public class PluginMain : IPlugin
    {
        private readonly List<Form> popups = new List<Form>();
        private string settingFilename;
        private Settings settingObject;
        private Icon playerIcon;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = nameof(FlashViewer);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "cba5ca4c-db80-43c2-9219-a15ee4d76aac";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; set; } = "Displays flash movies in FlashDevelop.";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public object Settings => settingObject;

        #endregion
        
        #region Required Methods
        
        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            AddEventHandlers();
        }
        
        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose() => SaveSettings();

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.Command : 
                    HandleCommand(((DataEvent)e));
                    break;

                case EventType.FileOpening : 
                    HandleFileOpening(((TextEvent)e));
                    break;
            }
        }
        
        #endregion

        #region Custom Methods
       
        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            var path = Path.Combine(PathHelper.DataDir, nameof(FlashViewer));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resource = "FlashViewer.Resources.Player.ico";
            Stream stream = assembly.GetManifestResourceStream(resource);
            Description = TextHelper.GetString("Info.Description");
            playerIcon = new Icon(stream);
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.FileOpening, HandlingPriority.Low);
            EventManager.AddEventHandler(this, EventType.Command);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            settingObject = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else settingObject = (Settings) ObjectSerializer.Deserialize(settingFilename, settingObject);
            string oldPath = settingObject.PlayerPath;
            // Recheck after installer update if auto config is not disabled
            if (!settingObject.DisableAutoConfig && PluginBase.MainForm.RefreshConfig)
            {
                settingObject.PlayerPath = null;
            }
            // Try to find player path from AppMan archive
            if (string.IsNullOrEmpty(settingObject.PlayerPath))
            {
                string appManDir = Path.Combine(PathHelper.BaseDir, @"Apps\flashsa");
                if (Directory.Exists(appManDir))
                {
                    string[] exeFiles = Directory.GetFiles(appManDir, "*.exe", SearchOption.AllDirectories);
                    foreach (string exeFile in exeFiles)
                    {
                        settingObject.PlayerPath = exeFile;
                    }
                }
            }
            // Try to find player path from: Tools/flexlibs/
            if (string.IsNullOrEmpty(settingObject.PlayerPath))
            {
                string playerPath11 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.0\win\FlashPlayerDebugger.exe");
                string playerPath111 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.1\win\FlashPlayerDebugger.exe");
                string playerPath112 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.2\win\FlashPlayerDebugger.exe");
                string playerPath113 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.3\win\FlashPlayerDebugger.exe");
                string playerPath114 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.4\win\FlashPlayerDebugger.exe");
                string playerPath115 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.5\win\FlashPlayerDebugger.exe");
                string playerPath116 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.6\win\FlashPlayerDebugger.exe");
                string playerPath117 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.7\win\FlashPlayerDebugger.exe");
                string playerPath118 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.8\win\FlashPlayerDebugger.exe");
                string playerPath119 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.9\win\FlashPlayerDebugger.exe");
                if (File.Exists(playerPath119)) settingObject.PlayerPath = playerPath119;
                else if (File.Exists(playerPath118)) settingObject.PlayerPath = playerPath118;
                else if (File.Exists(playerPath117)) settingObject.PlayerPath = playerPath117;
                else if (File.Exists(playerPath116)) settingObject.PlayerPath = playerPath116;
                else if (File.Exists(playerPath115)) settingObject.PlayerPath = playerPath115;
                else if (File.Exists(playerPath114)) settingObject.PlayerPath = playerPath114;
                else if (File.Exists(playerPath113)) settingObject.PlayerPath = playerPath113;
                else if (File.Exists(playerPath112)) settingObject.PlayerPath = playerPath112;
                else if (File.Exists(playerPath111)) settingObject.PlayerPath = playerPath111;
                else if (File.Exists(playerPath11)) settingObject.PlayerPath = playerPath11;
            }
            // Try to find player path from: Tools/flexsdk/
            if (string.IsNullOrEmpty(settingObject.PlayerPath))
            {
                string playerPath10 = Path.Combine(PathHelper.ToolDir, @"flexsdk\runtimes\player\10\win\FlashPlayer.exe");
                string playerPath101 = Path.Combine(PathHelper.ToolDir, @"flexsdk\runtimes\player\10.1\win\FlashPlayerDebugger.exe");
                string playerPath102 = Path.Combine(PathHelper.ToolDir, @"flexsdk\runtimes\player\10.2\win\FlashPlayerDebugger.exe");
                if (File.Exists(playerPath102)) settingObject.PlayerPath = playerPath102;
                else if (File.Exists(playerPath101)) settingObject.PlayerPath = playerPath101;
                else if (File.Exists(playerPath10)) settingObject.PlayerPath = playerPath10;
            }
            // Try to find player path from: FlexSDK
            if (string.IsNullOrEmpty(settingObject.PlayerPath))
            {
                string compiler = PluginBase.MainForm.ProcessArgString("$(CompilerPath)");
                string playerPath10 = Path.Combine(compiler, @"runtimes\player\10\win\FlashPlayer.exe");
                string playerPath101 = Path.Combine(compiler, @"runtimes\player\10.1\win\FlashPlayerDebugger.exe");
                string playerPath102 = Path.Combine(compiler, @"runtimes\player\10.2\win\FlashPlayerDebugger.exe");
                if (File.Exists(playerPath102)) settingObject.PlayerPath = playerPath102;
                else if (File.Exists(playerPath101)) settingObject.PlayerPath = playerPath101;
                else if (File.Exists(playerPath10)) settingObject.PlayerPath = playerPath10;
            }
            // After detection, if the path is incorrect, keep old valid path or clear it
            if (settingObject.PlayerPath is null || !File.Exists(settingObject.PlayerPath))
            {
                if (!string.IsNullOrEmpty(oldPath) && File.Exists(oldPath)) settingObject.PlayerPath = oldPath;
                else settingObject.PlayerPath = string.Empty;
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        /// <summary>
        /// Handles the Command event and displays the movie
        /// </summary>
        public void HandleCommand(DataEvent e)
        {
            try
            {
                if (e.Action.StartsWithOrdinal("FlashViewer."))
                {
                    string action = e.Action;
                    string[] args = e.Data?.ToString().Split(',');
                    
                    if (args is null || string.IsNullOrEmpty(args[0]))
                    {
                        if (action == "FlashViewer.GetFlashPlayer")
                        {
                            e.Data = PathHelper.ResolvePath(settingObject.PlayerPath);
                            e.Handled = true;
                        }
                        return;
                    }

                    if (action == "FlashViewer.Default")
                    {
                        switch (settingObject.DisplayStyle)
                        {
                            case ViewStyle.Popup:
                                action = "FlashViewer.Popup";
                                break;

                            case ViewStyle.Document:
                                action = "FlashViewer.Document";
                                break;

                            case ViewStyle.External:
                                action = "FlashViewer.External";
                                break;
                        }
                    }
                    switch (action)
                    {
                        case "FlashViewer.Popup":
                            int width = 800;
                            int height = 600;
                            if (args.Length >= 3)
                            {
                                int.TryParse(args[1], out width);
                                int.TryParse(args[2], out height);
                            }
                            CreatePopup(args[0], new Size(width, height));
                            break;

                        case "FlashViewer.Document":
                            CreateDocument(args[0]);
                            break;

                        case "FlashViewer.External":
                            LaunchExternal(args[0]);
                            break;

                        case "FlashViewer.GetDisplayStyle":
                            e.Data = settingObject.DisplayStyle.ToString();
                            break;

                        case "FlashViewer.SetDisplayStyle":
                            ViewStyle vs = (ViewStyle)Enum.Parse(typeof(ViewStyle), e.Data.ToString());
                            settingObject.DisplayStyle = vs;
                            break;
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Handles the FileOpen event and displays the movie
        /// </summary>
        public void HandleFileOpening(TextEvent e)
        {
            if (File.Exists(e.Value) && Path.GetExtension(e.Value) == ".swf")
            {
                switch (settingObject.DisplayStyle)
                {
                    case ViewStyle.Popup : 
                        CreatePopup(e.Value, new Size(550, 400));
                        break;

                    case ViewStyle.Document : 
                        CreateDocument(e.Value);
                        break;

                    case ViewStyle.External:
                        LaunchExternal(e.Value);
                        break;
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Displays the flash movie in a popup
        /// </summary>
        public void CreatePopup(string file, Size size)
        {
            FlashView flashView;
            if (!File.Exists(file)) return;
            foreach (Form form in popups)
            {
                flashView = form.Controls[0] as FlashView;
                if (flashView != null && flashView.MoviePath.Equals(file, StringComparison.OrdinalIgnoreCase))
                {
                    form.Controls.Remove(flashView);
                    flashView.Dispose();
                    flashView = CreateFlashView(file);
                    if (flashView is null) return;
                    flashView.Dock = DockStyle.Fill;
                    form.Controls.Add(flashView);
                    form.Activate();
                    return;
                }
            }
            Form popup = new Form();
            popup.Icon = playerIcon;
            popup.Text = Path.GetFileName(file);
            popup.ClientSize = new Size(size.Width, size.Height);
            popup.StartPosition = FormStartPosition.CenterScreen;
            flashView = CreateFlashView(null);
            if (flashView is null) return;
            flashView.Size = popup.ClientSize;
            flashView.Dock = DockStyle.Fill;
            popup.Controls.Add(flashView);
            flashView.MoviePath = file;
            popup.Show();
            popup.FormClosing += PopupFormClosing;
            popup.Disposed += delegate { NotifyDisposed(file); };
            popups.Add(popup);
        }

        /// <summary>
        /// Removes the popup from the tracking
        /// </summary>
        private void PopupFormClosing(object sender, FormClosingEventArgs e) => popups.Remove(sender as Form);

        /// <summary>
        /// Displays the flash movie in a document
        /// </summary>
        public void CreateDocument(string file)
        {
            FlashView flashView;
            if (!File.Exists(file)) return;
            foreach (var document in PluginBase.MainForm.Documents)
            {
                if (!document.IsEditable)
                {
                    foreach (Control ctrl in document.Controls)
                        if (ctrl is FlashView view)
                        {
                            flashView = view;
                            if (flashView.MoviePath.Equals(file, StringComparison.OrdinalIgnoreCase))
                            {
                                document.Controls.Remove(flashView);
                                flashView.Dispose();
                                flashView = CreateFlashView(file);
                                if (flashView is null) return;
                                flashView.Dock = DockStyle.Fill;
                                document.Controls.Add(flashView);
                                document.Activate();
                                return;
                            }
                        }
                }
            }
            flashView = CreateFlashView(null);
            if (flashView is null) return;
            flashView.Dock = DockStyle.Fill;
            var flashDoc = PluginBase.MainForm.CreateCustomDocument(flashView);
            flashDoc.Text = Path.GetFileName(file);
            flashView.MoviePath = file;
            flashDoc.Disposed += delegate { NotifyDisposed(file); };
        }
        private void NotifyDisposed(string file)
        {
            var de = new DataEvent(EventType.Command, "FlashViewer.Closed", file);
            EventManager.DispatchEvent(this, de);
        }

        /// <summary>
        /// Creates a flash view control and if it fails gives an error message
        /// </summary>
        private FlashView CreateFlashView(string file)
        {
            try
            {
                return new FlashView(file);
            }
            catch (Exception ex)
            {
                string msg = TextHelper.GetString("Info.FlashMissing");
                ErrorManager.ShowWarning(msg, ex);
                return null;
            }
        }

        /// <summary>
        /// Displays the flash movie in an external player
        /// </summary>
        public void LaunchExternal(string file)
        {
            try
            {
                var player = PathHelper.ResolvePath(settingObject.PlayerPath);
                if (File.Exists(player)) ProcessHelper.StartAsync(player, file); 
                else ProcessHelper.StartAsync(file);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        #endregion
    }
}