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
        private String pluginName = "FlashViewer";
        private String pluginGuid = "cba5ca4c-db80-43c2-9219-a15ee4d76aac";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Displays flash movies in FlashDevelop.";
        private String pluginAuth = "FlashDevelop Team";
        private List<Form> popups = new List<Form>();
        private String settingFilename;
        private Settings settingObject;
        private Icon playerIcon;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public Int32 Api
        {
            get { return 1; }
        }

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public String Name
        {
            get { return this.pluginName; }
        }

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public String Guid
        {
            get { return this.pluginGuid; }
        }

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public String Author
        {
            get { return this.pluginAuth; }
        }

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public String Description
        {
            get { return this.pluginDesc; }
        }

        /// <summary>
        /// Web address for help
        /// </summary> 
        public String Help
        {
            get { return this.pluginHelp; }
        }

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public Object Settings
        {
            get { return this.settingObject; }
        }
        
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
        public void Dispose()
        {
            this.SaveSettings();
        }
        
        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
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
            String dataPath = Path.Combine(PathHelper.DataDir, "FlashViewer");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            Assembly assembly = Assembly.GetExecutingAssembly();
            String resource = "FlashViewer.Resources.Player.ico";
            Stream stream = assembly.GetManifestResourceStream(resource);
            this.pluginDesc = TextHelper.GetString("Info.Description");
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
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
            }
            String oldPath = this.settingObject.PlayerPath;
            // Recheck after installer update if auto config is not disabled
            if (!this.settingObject.DisableAutoConfig && PluginBase.MainForm.RefreshConfig)
            {
                this.settingObject.PlayerPath = null;
            }
            // Try to find player path from AppMan archive
            if (String.IsNullOrEmpty(this.settingObject.PlayerPath))
            {
                String appManDir = Path.Combine(PathHelper.BaseDir, @"Apps\flashsa");
                if (Directory.Exists(appManDir))
                {
                    String[] exeFiles = Directory.GetFiles(appManDir, "*.exe", SearchOption.AllDirectories);
                    foreach (String exeFile in exeFiles)
                    {
                        this.settingObject.PlayerPath = exeFile;
                    }
                }
            }
            // Try to find player path from: Tools/flexlibs/
            if (String.IsNullOrEmpty(this.settingObject.PlayerPath))
            {
                String playerPath11 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.0\win\FlashPlayerDebugger.exe");
                String playerPath111 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.1\win\FlashPlayerDebugger.exe");
                String playerPath112 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.2\win\FlashPlayerDebugger.exe");
                String playerPath113 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.3\win\FlashPlayerDebugger.exe");
                String playerPath114 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.4\win\FlashPlayerDebugger.exe");
                String playerPath115 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.5\win\FlashPlayerDebugger.exe");
                String playerPath116 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.6\win\FlashPlayerDebugger.exe");
                String playerPath117 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.7\win\FlashPlayerDebugger.exe");
                String playerPath118 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.8\win\FlashPlayerDebugger.exe");
                String playerPath119 = Path.Combine(PathHelper.ToolDir, @"flexlibs\runtimes\player\11.9\win\FlashPlayerDebugger.exe");
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
            if (String.IsNullOrEmpty(this.settingObject.PlayerPath))
            {
                String playerPath10 = Path.Combine(PathHelper.ToolDir, @"flexsdk\runtimes\player\10\win\FlashPlayer.exe");
                String playerPath101 = Path.Combine(PathHelper.ToolDir, @"flexsdk\runtimes\player\10.1\win\FlashPlayerDebugger.exe");
                String playerPath102 = Path.Combine(PathHelper.ToolDir, @"flexsdk\runtimes\player\10.2\win\FlashPlayerDebugger.exe");
                if (File.Exists(playerPath102)) this.settingObject.PlayerPath = playerPath102;
                else if (File.Exists(playerPath101)) this.settingObject.PlayerPath = playerPath101;
                else if (File.Exists(playerPath10)) this.settingObject.PlayerPath = playerPath10;
            }
            // Try to find player path from: FlexSDK
            if (String.IsNullOrEmpty(this.settingObject.PlayerPath))
            {
                String compiler = PluginBase.MainForm.ProcessArgString("$(CompilerPath)");
                String playerPath10 = Path.Combine(compiler, @"runtimes\player\10\win\FlashPlayer.exe");
                String playerPath101 = Path.Combine(compiler, @"runtimes\player\10.1\win\FlashPlayerDebugger.exe");
                String playerPath102 = Path.Combine(compiler, @"runtimes\player\10.2\win\FlashPlayerDebugger.exe");
                if (File.Exists(playerPath102)) this.settingObject.PlayerPath = playerPath102;
                else if (File.Exists(playerPath101)) this.settingObject.PlayerPath = playerPath101;
                else if (File.Exists(playerPath10)) this.settingObject.PlayerPath = playerPath10;
            }
            // After detection, if the path is incorrect, keep old valid path or clear it
            if (this.settingObject.PlayerPath == null || !File.Exists(this.settingObject.PlayerPath))
            {
                if (!String.IsNullOrEmpty(oldPath) && File.Exists(oldPath)) this.settingObject.PlayerPath = oldPath;
                else this.settingObject.PlayerPath = String.Empty;
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
                    String action = evnt.Action;
                    String[] args = evnt.Data != null ? evnt.Data.ToString().Split(',') : null;
                    
                    if (args == null || String.IsNullOrEmpty(args[0]))
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
                            Int32 width = 800;
                            Int32 height = 600;
                            if (args.Length >= 3)
                            {
                                Int32.TryParse(args[1], out width);
                                Int32.TryParse(args[2], out height);
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
        public void CreatePopup(String file, Size size)
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
        private void PopupFormClosing(Object sender, FormClosingEventArgs e)
        {
            popups.Remove(sender as Form);
        }

        /// <summary>
        /// Displays the flash movie in a document
        /// </summary>
        public void CreateDocument(String file)
        {
            FlashView flashView;
            if (!File.Exists(file)) return;
            foreach (ITabbedDocument document in PluginBase.MainForm.Documents)
            {
                if (!document.IsEditable)
                {
                    foreach (Control ctrl in document.Controls) if (ctrl is FlashView)
                    {
                        flashView = ctrl as FlashView;
                        if (flashView != null && flashView.MoviePath.Equals(file, StringComparison.OrdinalIgnoreCase))
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
            flashView = this.CreateFlashView(null);
            if (flashView == null) return;
            flashView.Dock = DockStyle.Fill;
            DockContent flashDoc = PluginBase.MainForm.CreateCustomDocument(flashView);
            flashDoc.Text = Path.GetFileName(file);
            flashView.MoviePath = file;
            flashDoc.Disposed += delegate { NotifyDisposed(file); };
        }
        private void NotifyDisposed(String file)
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
                String msg = TextHelper.GetString("Info.FlashMissing");
                ErrorManager.ShowWarning(msg, ex);
                return null;
            }
        }

        /// <summary>
        /// Displays the flash movie in an external player
        /// </summary>
        public void LaunchExternal(String file)
        {
            try
            {
                String player = PathHelper.ResolvePath(this.settingObject.PlayerPath);
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
