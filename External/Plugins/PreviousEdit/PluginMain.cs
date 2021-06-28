using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using PreviousEdit.Behavior;
using ProjectManager;
using ScintillaNet;

namespace PreviousEdit
{
    public class PluginMain : IPlugin
    {
        public int Api => 1;
        public string Name => nameof(PreviousEdit);
        public string Guid => "55E1998E-9929-4470-805E-2DB339C29116";
        public string Help => "https://www.flashdevelop.org/community/";
        public string Author => "FlashDevelop Team";
        public string Description => "Navigate Backward and Navigate Forward";

        [Browsable(false)]
        public object Settings => settingObject;
        string settingFilename;
        Settings settingObject;

        readonly VSBehavior behavior = new VSBehavior();

        QueueItem executableStatus;
        List<ToolStripItem> forwardMenuItems;
        List<ToolStripItem> backwardMenuItems;
        int sciPrevPosition;

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            CreateMenuItems();
            AddEventHandlers();
        }

        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose() => SaveSettings();

        /// <summary>
        /// Initializes important variables
        /// </summary>
        void InitBasics()
        {
            var path = Path.Combine(PathHelper.DataDir, Name);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, $"{nameof(Settings)}.fdb");
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        void LoadSettings()
        {
            settingObject = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else settingObject = ObjectSerializer.Deserialize(settingFilename, settingObject);
        }

        /// <summary>
        /// Creates the required menu items
        /// </summary>
        void CreateMenuItems()
        {
            var menu = (ToolStripMenuItem) PluginBase.MainForm.FindMenuItem("SearchMenu");
            menu.DropDownItems.Add(new ToolStripSeparator());
            backwardMenuItems = CreateMenuItem(menu, "1", TextHelper.GetString("Label.NavigateBackward"), NavigateBackward, $"{Name}.NavigateBackward", 0);
            forwardMenuItems = CreateMenuItem(menu, "9", TextHelper.GetString("Label.NavigateForward"), NavigateForward, $"{Name}.NavigateForward", 1);
            PluginBase.MainForm.ToolStrip.Items.Insert(2, new ToolStripSeparator());
        }

        static List<ToolStripItem> CreateMenuItem(ToolStripDropDownItem menu, string imageIndex, string text, EventHandler onClick, string shortcutId, int toolbarIndex)
        {
            var image = PluginBase.MainForm.FindImage(imageIndex);
            var menuItem = new ToolStripMenuItem(text, image, onClick);
            PluginBase.MainForm.RegisterShortcutItem(shortcutId, menuItem);
            menu.DropDownItems.Add(menuItem);
            var toolbarItem = new ToolStripButton(string.Empty, image, onClick) {ToolTipText = text};
            PluginBase.MainForm.ToolStrip.Items.Insert(toolbarIndex, toolbarItem);
            return new List<ToolStripItem> {menuItem, toolbarItem};
        }

        /// <summary>
        /// Updates the state of the menu items
        /// </summary>
        void UpdateMenuItems()
        {
            backwardMenuItems.ForEach(it => it.Enabled = behavior.CanBackward);
            forwardMenuItems.ForEach(it => it.Enabled = behavior.CanForward);
        }

        void AddEventHandlers() => EventManager.AddEventHandler(this, EventType.FileSwitch | EventType.Command);

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.Command && ((DataEvent) e).Action == ProjectManagerEvents.Project)
            {
                behavior.Clear();
                sciPrevPosition = 0;
                executableStatus = null;
                UpdateMenuItems();
                return;
            }
            if (e.Type != EventType.FileSwitch) return;
            var doc = PluginBase.MainForm.CurrentDocument;
            if (doc is null || !doc.IsEditable) return;
            var sci = doc.SciControl;
            if (sci is null) return;
            sci.Modified -= SciControlModified;
            sci.Modified += SciControlModified;
            sci.UpdateUI -= SciControlUpdateUI;
            sci.UpdateUI += SciControlUpdateUI;
            SciControlUpdateUI(sci);
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        /// <summary>
        /// Modified Handler
        /// </summary>
        void SciControlModified(ScintillaControl sci, int position, int modificationType,
            string text, int length, int linesAdded, int line, int intfoldLevelNow, int foldLevelPrev)
        {
            var startPosition = sciPrevPosition < position ? sciPrevPosition : position;
            if (linesAdded < 0) length = -length;
#if DEBUG
            TraceManager.Add(nameof(SciControlModified));
            TraceManager.Add(behavior.ToString());
            TraceManager.Add(sci.FileName);
            TraceManager.Add("startPosition: " + startPosition);
            TraceManager.Add("length: " + length);
            TraceManager.Add("linesAdded: " + linesAdded);
#endif
            behavior.Change(sci.FileName, startPosition, length, linesAdded);
            sciPrevPosition = sci.CurrentPos;
#if DEBUG
            TraceManager.Add(nameof(SciControlModified));
            TraceManager.Add(behavior.ToString());
#endif
        }

        void SciControlUpdateUI(ScintillaControl sci)
        {
            if (executableStatus != null && executableStatus.Equals(behavior.CurrentItem)) return;
            behavior.Add(sci.FileName, sci.CurrentPos, sci.CurrentLine);
            UpdateMenuItems();
        }

        void NavigateBackward(object sender, EventArgs e)
        {
            if (!behavior.CanBackward) return;
            behavior.Backward();
            Navigate(behavior.CurrentItem);
        }

        void NavigateForward(object sender, EventArgs e)
        {
            if (!behavior.CanForward) return;
            behavior.Forward();
            Navigate(behavior.CurrentItem);
        }

        void Navigate(QueueItem to)
        {
            executableStatus = to;
            UpdateMenuItems();
            PluginBase.MainForm.OpenEditableDocument(to.FileName, false);
            var position = to.Position;
            PluginBase.MainForm.CurrentDocument?.SciControl?.SetSel(position, position);
            executableStatus = null;
        }
    }
}