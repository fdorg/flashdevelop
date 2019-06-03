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
using WeifenLuo.WinFormsUI.Docking;

namespace FlashViewer
{
    public class PluginMain : IPlugin
    {
        private List<Form> popups = new List<Form>();
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
        public string Name { get; } = "FlashViewer";

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
        public object Settings => this.settingObject;

        #endregion
        
        #region Required Methods
        
        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            this.InitBasics();
            this.LoadSettings();
            this.AddEventHandlers();
        }
        
        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose() => this.SaveSettings();

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.Command : 
                    this.HandleCommand(((DataEvent)e));
                    break;

                case EventType.FileOpening : 
                    this.HandleFileOpening(((TextEvent)e));
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
            string dataPath = Path.Combine(PathHelper.DataDir, "FlashViewer");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resource = "FlashViewer.Resources.Player.ico";
            Stream stream = assembly.GetManifestResourceStream(resource);
            this.Description = TextHelper.GetString("Info.Description");
            this.playerIcon = new Icon(stream);
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
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
            }
            string oldPath = this.settingObject.PlayerPath;
            // Recheck after installer update if auto config is not disabled
            if (!this.settingObject.DisableAutoConfig && PluginBase.MainForm.RefreshConfig)
            {
                this.settingObject.PlayerPath = null;
            }
            // Try to find player path from AppMan archive
            if (string.IsNullOrEmpty(this.settingObject.PlayerPath))
            {
                string appManDir = Path.Combine(PathHelper.BaseDir, @"Apps\flashsa");
                if (Directory.Exists(appManDir))
                {
                    string[] exeFiles = Directory.GetFiles(appManDir, "*.exe", SearchOption.AllDirectories);
                    foreach (string exeFile in exeFiles)
                    {
                        this.settingObject.PlayerPath = exeFile;
                    }
                }
            }
            // Try to find player path from: Tools/flexlibs/
            if (string.IsNullOrEmpty(this.settingObject.PlayerPath))
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
                if (File.Exists(playerPath119)) this.settingObject.PlayerPath = playerPath119;
                else if (File.Exists(playerPath118)) this.settingObject.PlayerPath = playerPath118;
                else if (File.Exists(playerPath117)) this.settingObject.PlayerPath = playerPath117;
                else if (File.Exists(playerPath116)) this.settingObject.PlayerPath = playerPath116;
                else if (File.Exists(playerPath115)) this.settingObject.PlayerPath = playerPath115;
                else if (File.Exists(playerPath114)) this.settingObject.PlayerPath = playerPath114;
                else if (File.Exists(playerPath113)) this.settingObject.PlayerPath = playerPath113;
                else if (File.Exists(playerPath112)) this.settingObject.PlayerPath = playerPath112;
                else if (File.Exists(playerPath111)) this.settingObject.PlayerPath = playerPath111;
                else if (File.Exists(playerPath11)) this.settingObject.PlayerPath = playerPath11;
            }
            // Try to find player path from: Tools/flexsdk/
            if (string.IsNullOrEmpty(this.settingObject.PlayerPath))
            {
                string playerPath10 = Path.Combine(PathHelper.ToolDir, @"flexsdk\runtimes\player\10\win\FlashPlayer.exe");
                string playerPath101 = Path.Combine(PathHelper.ToolDir, @"flexsdk\runtimes\player\10.1\win\FlashPlayerDebugger.exe");
                string playerPath102 = Path.Combine(PathHelper.ToolDir, @"flexsdk\runtimes\player\10.2\win\FlashPlayerDebugger.exe");
                if (File.Exists(playerPath102)) this.settingObject.PlayerPath = playerPath102;
                else if (File.Exists(playerPath101)) this.settingObject.PlayerPath = playerPath101;
                else if (File.Exists(playerPath10)) this.settingObject.PlayerPath = playerPath10;
            }
            // Try to find player path from: FlexSDK
            if (string.IsNullOrEmpty(this.settingObject.PlayerPath))
            {
                string compiler = PluginBase.MainForm.ProcessArgString("$(CompilerPath)");
                string playerPath10 = Path.Combine(compiler, @"runtimes\player\10\win\FlashPlayer.exe");
                string playerPath101 = Path.Combine(compiler, @"runtimes\player\10.1\win\FlashPlayerDebugger.exe");
                string playerPath102 = Path.Combine(compiler, @"runtimes\player\10.2\win\FlashPlayerDebugger.exe");
                if (File.Exists(playerPath102)) this.settingObject.PlayerPath = playerPath102;
                else if (File.Exists(playerPath101)) this.settingObject.PlayerPath = playerPath101;
                else if (File.Exists(playerPath10)) this.settingObject.PlayerPath = playerPath10;
            }
            // After detection, if the path is incorrect, keep old valid path or clear it
            if (this.settingObject.PlayerPath == null || !File.Exists(this.settingObject.PlayerPath))
            {
                if (!string.IsNullOrEmpty(oldPath) && File.Exists(oldPath)) this.settingObject.PlayerPath = oldPath;
                else this.settingObject.PlayerPath = string.Empty;
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        /// <summary>
        /// Handles the Command event and displays the movie
        /// </summary>
        public void HandleCommand(DataEvent evnt)
        {
            try
            {
                if (evnt.Action.StartsWithOrdinal("FlashViewer."))
                {
                    string action = evnt.Action;
                    string[] args = evnt.Data?.ToString().Split(',');
                    
                    if (args == null || string.IsNullOrEmpty(args[0]))
                    {
                        if (action == "FlashViewer.GetFlashPlayer")
                        {
                            evnt.Data = PathHelper.ResolvePath(this.settingObject.PlayerPath);
                            evnt.Handled = true;
                        }
                        return;
                    }

                    if (action == "FlashViewer.Default")
                    {
                        switch (this.settingObject.DisplayStyle)
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
                            this.CreatePopup(args[0], new Size(width, height));
                            break;

                        case "FlashViewer.Document":
                            this.CreateDocument(args[0]);
                            break;

                        case "FlashViewer.External":
                            this.LaunchExternal(args[0]);
                            break;

                        case "FlashViewer.GetDisplayStyle":
                            evnt.Data = this.settingObject.DisplayStyle.ToString();
                            break;

                        case "FlashViewer.SetDisplayStyle":
                            ViewStyle vs = (ViewStyle)Enum.Parse(typeof(ViewStyle), evnt.Data.ToString());
                            this.settingObject.DisplayStyle = vs;
                            break;
                    }
                    evnt.Handled = true;
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
        public void HandleFileOpening(TextEvent evnt)
        {
            if (File.Exists(evnt.Value) && Path.GetExtension(evnt.Value) == ".swf")
            {
                switch (this.settingObject.DisplayStyle)
                {
                    case ViewStyle.Popup : 
                        this.CreatePopup(evnt.Value, new Size(550, 400));
                        break;

                    case ViewStyle.Document : 
                        this.CreateDocument(evnt.Value);
                        break;

                    case ViewStyle.External:
                        this.LaunchExternal(evnt.Value);
                        break;
                }
                evnt.Handled = true;
            }
        }

        /// <summary>
        /// Displays the flash movie in a popup
        /// </summary>
        public void CreatePopup(string file, Size size)
        {
            FlashView flashView;
            if (!File.Exists(file)) return;
            foreach (Form form in this.popups)
            {
                flashView = form.Controls[0] as FlashView;
                if (flashView != null && flashView.MoviePath.Equals(file, StringComparison.OrdinalIgnoreCase))
                {
                    form.Controls.Remove(flashView);
                    flashView.Dispose();
                    flashView = CreateFlashView(file);
                    if (flashView == null) return;
                    flashView.Dock = DockStyle.Fill;
                    form.Controls.Add(flashView);
                    form.Activate();
                    return;
                }
            }
            Form popup = new Form();
            popup.Icon = this.playerIcon;
            popup.Text = Path.GetFileName(file);
            popup.ClientSize = new Size(size.Width, size.Height);
            popup.StartPosition = FormStartPosition.CenterScreen;
            flashView = this.CreateFlashView(null);
            if (flashView == null) return;
            flashView.Size = popup.ClientSize;
            flashView.Dock = DockStyle.Fill;
            popup.Controls.Add(flashView);
            flashView.MoviePath = file;
            popup.Show();
            popup.FormClosing += this.PopupFormClosing;
            popup.Disposed += delegate { NotifyDisposed(file); };
            this.popups.Add(popup);
        }

        /// <summary>
        /// Removes the popup from the tracking
        /// </summary>
        private void PopupFormClosing(object sender, FormClosingEventArgs e)
        {
            popups.Remove(sender as Form);
        }

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
                                if (flashView == null) return;
                                flashView.Dock = DockStyle.Fill;
                                document.Controls.Add(flashView);
                                document.Activate();
                                return;
                            }
                        }
                }
            }
            flashView = CreateFlashView(null);
            if (flashView == null) return;
            flashView.Dock = DockStyle.Fill;
            DockContent flashDoc = PluginBase.MainForm.CreateCustomDocument(flashView);
            flashDoc.Text = Path.GetFileName(file);
            flashView.MoviePath = file;
            flashDoc.Disposed += delegate { NotifyDisposed(file); };
        }
        private void NotifyDisposed(string file)
        {
            DataEvent de = new DataEvent(EventType.Command, "FlashViewer.Closed", file);
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
                string player = PathHelper.ResolvePath(this.settingObject.PlayerPath);
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
