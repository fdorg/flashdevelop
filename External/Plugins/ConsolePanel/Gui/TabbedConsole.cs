using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ConsolePanel.Gui
{
    public partial class TabbedConsole : UserControl
    {
        //private List<CmdPanel> consoles;
        private PluginMain main;

        public ICollection<ConsoleControl.ConsoleControl> Consoles
        {
            get
            {
                return consoleTabMap.Keys;
            }
        }

        public Dictionary<ConsoleControl.ConsoleControl, TabPage> consoleTabMap;
        public Dictionary<TabPage, ConsoleControl.ConsoleControl> tabConsoleMap;

        public TabbedConsole(PluginMain plugin)
        {
            InitializeComponent();

            main = plugin;
            consoleTabMap = new Dictionary<ConsoleControl.ConsoleControl, TabPage>();
            tabConsoleMap = new Dictionary<TabPage, ConsoleControl.ConsoleControl>();

            btnNew.Image = PluginCore.PluginBase.MainForm.FindImage16("33");
        }

        public void AddConsole(ConsoleControl.ConsoleControl console)
        {
            var tabPage = new TabPage("Console");
            console.Dock = DockStyle.Fill;
            tabPage.Controls.Add(console);

            tabConsoles.TabPages.Add(tabPage);
            tabConsoles.SelectTab(tabPage);
            consoleTabMap.Add(console, tabConsoles.SelectedTab);
            tabConsoleMap.Add(tabConsoles.SelectedTab, console);
        }

        public void RemoveConsole(ConsoleControl.ConsoleControl console)
        {
            if (consoleTabMap.ContainsKey(console))
            {
                console.Cancel();

                var page = consoleTabMap[console];
                tabConsoles.TabPages.Remove(page);
                consoleTabMap.Remove(console);
                tabConsoleMap.Remove(page);
            }
        }

        private void tabConsoles_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                for (int i = 0; i < tabConsoles.TabCount; i++)
                {
                    if (tabConsoles.GetTabRect(i).Contains(e.Location))
                    {
                        RemoveConsole(tabConsoleMap[tabConsoles.TabPages[i]]);
                    }
                }
                
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            main.CreateConsolePanel();
        }
    }
}
