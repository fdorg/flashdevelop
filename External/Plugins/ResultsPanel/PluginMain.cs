using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ResultsPanel.Helpers;
using ScintillaNet;
using WeifenLuo.WinFormsUI.Docking;

namespace ResultsPanel
{
    public class PluginMain : IPlugin
    {
        private Settings settingObject;
        private string settingFilename;
        internal PluginUI pluginUI;
        internal Image pluginImage;
        internal PanelContextMenu contextMenuStrip;
        private ToolStripMenuItem viewItem;
        private ToolStripMenuItem viewItemMainPanel;
        private ToolStripSeparator viewItemSeparator;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = "ResultsPanel";

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "24df7cd8-e5f0-4171-86eb-7b2a577703ba";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; set; } = "Adds a results panel for console info to FlashDevelop";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        public object Settings => settingObject;

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
            this.CreateMenuItem();
            this.CreatePluginPanel();
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
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.Command:
                    var de = (DataEvent) e;
                    switch (de.Action)
                    {
                        case "ResultsPanel.ClearResults":
                            de.Handled = true;
                            ResultsPanelHelper.ClearResults((string) de.Data);
                            break;

                        case "ResultsPanel.ShowResults":
                            e.Handled = true;
                            ResultsPanelHelper.ShowResults((string) de.Data);
                            break;
                    }
                    break;

                case EventType.ApplySettings:
                case EventType.ApplyTheme:
                    ResultsPanelHelper.ApplySettings();
                    break;

                case EventType.ProcessStart:
                    this.pluginUI.ClearOutput();
                    break;

                case EventType.ProcessEnd:
                    this.pluginUI.DisplayOutput();
                    break;

                case EventType.Trace:
                    ResultsPanelHelper.OnTrace();
                    break;

                case EventType.FileOpen:
                    ResultsPanelHelper.OnFileOpen((TextEvent) e);
                    break;

                case EventType.Keys:
                    KeyEvent ke = (KeyEvent) e;
                    switch (PluginBase.MainForm.GetShortcutItemId(ke.Value))
                    {
                        case null:
                            break;
                        case "ResultsPanel.ShowNextResult":
                            ke.Handled = ResultsPanelHelper.ActiveUI.NextEntry();
                            break;
                        case "ResultsPanel.ShowPrevResult":
                            ke.Handled = ResultsPanelHelper.ActiveUI.PreviousEntry();
                            break;
                        case "ResultsPanel.ClearResults":
                            ke.Handled = ResultsPanelHelper.ActiveUI.ClearOutput();
                            break;
                        case "ResultsPanel.ClearIgnoredEntries":
                            ke.Handled = ResultsPanelHelper.ActiveUI.ClearIgnoredEntries();
                            break;
                        default:
                            if (ke.Value == PanelContextMenu.CopyEntryKeys) ke.Handled = ResultsPanelHelper.ActiveUI.CopyTextShortcut();
                            else if (ke.Value == PanelContextMenu.IgnoreEntryKeys) ke.Handled = ResultsPanelHelper.ActiveUI.IgnoreEntryShortcut();
                            break;
                    }

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
            string dataDir = Path.Combine(PathHelper.DataDir, "ResultsPanel");
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            this.settingFilename = Path.Combine(dataDir, "Settings.fdb");
            this.Description = TextHelper.GetString("Info.Description");
            this.pluginImage = PluginBase.MainForm.FindImage("127");
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else settingObject = (Settings) ObjectSerializer.Deserialize(settingFilename, settingObject);
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            EventType eventMask = EventType.ProcessEnd | EventType.ProcessStart | EventType.FileOpen | EventType.Command
                | EventType.Trace | EventType.Keys | EventType.Shortcut | EventType.ApplySettings | EventType.ApplyTheme;
            EventManager.AddEventHandler(this, eventMask);

            UITools.Manager.OnMouseHover += Scintilla_OnMouseHover;
            UITools.Manager.OnMouseHoverEnd += Scintilla_OnMouseHoverEnd;
        }

        private void Scintilla_OnMouseHover(ScintillaControl sender, int position)
        {
            var document = DocumentManager.FindDocument(sender);
            if (document == null)
                return;

            var results = new List<string>();
            foreach (var ui in ResultsPanelHelper.PluginUIs)
            {
                ui.GetResultsAt(results, document, position);
            }
            //Main panel has to be handled specifically
            ResultsPanelHelper.MainUI.GetResultsAt(results, document, position);

            if (results.Count > 0)
            {
                var desc = string.Join(Environment.NewLine, results.ToArray());
                UITools.ErrorTip.ShowAtMouseLocation(desc);
            }
        }

        private void Scintilla_OnMouseHoverEnd(ScintillaControl sender, int position)
        {
            UITools.ErrorTip.Hide();
        }

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        public void CreatePluginPanel()
        {
            this.pluginUI = new PluginUI(this);
            this.pluginUI.Text = TextHelper.GetString("Title.PluginPanel");
            this.pluginUI.ParentPanel = PluginBase.MainForm.CreateDockablePanel(this.pluginUI, this.Guid, this.pluginImage, DockState.DockBottomAutoHide);
            ResultsPanelHelper.Initialize(this, this.pluginUI);
        }

        /// <summary>
        /// Creates a menu item for the plugin
        /// </summary>
        public void CreateMenuItem()
        {
            viewItemMainPanel = new ToolStripMenuItem(TextHelper.GetString("Label.ViewMenuItem"), pluginImage) { Tag = null };
            viewItemSeparator = new ToolStripSeparator();

            viewItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewMenuItem"), pluginImage);
            viewItem.DropDownItems.Add(viewItemMainPanel);
            viewItem.DropDownOpening += ViewItem_DropDownOpening;
            viewItem.DropDownItemClicked += ViewItem_DropDownItemClicked;

            var viewMenu = (ToolStripMenuItem) PluginBase.MainForm.FindMenuItem("ViewMenu");
            viewMenu.DropDownItems.Add(viewItem);

            this.contextMenuStrip = new PanelContextMenu();
            PluginBase.MainForm.RegisterShortcutItem("ResultsPanel.ShowNextResult", this.contextMenuStrip.NextEntry);
            PluginBase.MainForm.RegisterShortcutItem("ResultsPanel.ShowPrevResult", this.contextMenuStrip.PreviousEntry);
            PluginBase.MainForm.RegisterShortcutItem("ResultsPanel.ClearResults", this.contextMenuStrip.ClearEntries);
            PluginBase.MainForm.RegisterShortcutItem("ResultsPanel.ClearIgnoredEntries", this.contextMenuStrip.ClearIgnoredEntries);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowResults", viewItemMainPanel);
        }

        private void ViewItem_DropDownOpening(object sender, EventArgs e)
        {
            viewItem.DropDownItems.Clear();
            viewItem.DropDownItems.Add(viewItemMainPanel);

            if (ResultsPanelHelper.PluginUIs.Count > 0)
            {
                viewItem.DropDownItems.Add(viewItemSeparator);
                foreach (var ui in ResultsPanelHelper.PluginUIs)
                {
                    viewItem.DropDownItems.Add(new ToolStripMenuItem(ui.Text) { Tag = ui.GroupData });
                }
            }
        }

        private void ViewItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem is ToolStripMenuItem)
            {
                string groupData = (string) e.ClickedItem.Tag;

                if (groupData == null)
                {
                    pluginUI.ParentPanel.Show();
                }
                else
                {
                    foreach (var ui in ResultsPanelHelper.PluginUIs)
                    {
                        if (ui.GroupData == groupData)
                        {
                            ui.ParentPanel.Show();
                            break;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
