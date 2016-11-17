
using System;
using PluginCore;
using System.ComponentModel;
using PluginCore.Managers;
using PluginCore.Utilities;
using System.IO;
using PluginCore.Helpers;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Drawing;
using ProjectManager.Projects;

namespace ConsolePanel
{
    public class PluginMain : IPlugin
    {
        private string pluginName = "ConsolePanel";
        private string pluginGuid = "AEDE556E-54C3-4EFB-8EF3-54E85DC37D1E";
        private string pluginHelp = "http://hexmachina.org/";
        private string pluginDesc = "Plugin that adds an embedded console window to FlashDevelop.";
        private string pluginAuth = "Christoph Otter";
        private string settingFilename;
        private Settings settingObject;
        private DockContent cmdPanelDockContent;
        private Gui.TabbedConsole tabView;
        private Image image;

        private IConsoleProvider provider;

        public int Api
        {
            get
            {
                return 1;
            }
        }

        public string Author
        {
            get
            {
                return pluginAuth;
            }
        }

        public string Description
        {
            get
            {
                return pluginDesc;
            }
        }

        public string Guid
        {
            get
            {
                return pluginGuid;
            }
        }

        public string Help
        {
            get
            {
                return pluginHelp;
            }
        }

        public string Name
        {
            get
            {
                return pluginName;
            }
        }

        [Browsable(false)]
        public object Settings
        {
            get
            {
                return settingObject;
            }
        }

        public IConsoleProvider ConsoleProvider
        {
            get
            {
                return provider;
            }
            set
            {
                provider = value;
            }
        }

        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            CreatePluginPanel();
            CreateDefaultConsoleProvider();
            CreateConsolePanel();
            CreateMenuItem();

            EventManager.AddEventHandler(this, EventType.Command, HandlingPriority.Normal);
        }

        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.Command:
                    var data = (DataEvent)e;
                    if (data.Action == "ProjectManager.Project")
                    {
                        var project = (Project) data.Data;
                        foreach (var panel in tabView.Consoles)
                        {
                            panel.WorkingDirectory = PluginBase.CurrentProject.GetAbsolutePath("");
                        }
                    }
                    break;
            }
        }

        public void Dispose()
        {
            SaveSettings();
        }

        private void InitBasics()
        {
            string dataPath = Path.Combine(PathHelper.DataDir, pluginName);
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");

            image = PluginBase.MainForm.FindImage("57");

            Managers.ConsoleManager.Init(this);
        }

        private void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
            }
        }

        private void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        private void CreatePluginPanel()
        {
            tabView = new Gui.TabbedConsole(this);
            cmdPanelDockContent = PluginBase.MainForm.CreateDockablePanel(tabView, pluginGuid, image, DockState.DockBottom);
            cmdPanelDockContent.Text = "Console";
        }

        private void CreateDefaultConsoleProvider()
        {
            ConsoleProvider = new Implementation.CmdProcess.CmdConsoleProvider();
        }

        public IConsole CreateConsolePanel()
        {
            cmdPanelDockContent.Show();

            var cmdPanel = ConsoleProvider.GetConsole();
            //var cmdPanel = new ConsoleControl.ConsoleControl(false);
            //cmdPanel.Text = "Console";
            //cmdPanel.ConsoleBackColor = settingObject.BackgroundColor;
            //cmdPanel.ConsoleForeColor = settingObject.ForegroundColor;

            cmdPanel.Exited += delegate
            {
                if (tabView.InvokeRequired)
                {
                    tabView.Invoke((MethodInvoker)delegate
                    {
                        if (!PluginBase.MainForm.ClosingEntirely)
                            tabView.RemoveConsole(cmdPanel);
                    });
                }
                else
                {
                    if (!PluginBase.MainForm.ClosingEntirely)
                        tabView.RemoveConsole(cmdPanel);
                }
            };

            tabView.AddConsole(cmdPanel);

            return cmdPanel;
        }

        private void CreateMenuItem()
        {
            string label = "Embedded Console";
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            ToolStripMenuItem cmdItem = new ToolStripMenuItem(label, image, OpenCmdPanel);

            viewMenu.DropDownItems.Add(cmdItem);
        }

        private void OpenCmdPanel(object sender, EventArgs e)
        {
            CreateConsolePanel();
            //cmdPanelDockContent.Show();
        }
    }
}
