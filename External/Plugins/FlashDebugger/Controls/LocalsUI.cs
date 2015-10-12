using System;
using System.Windows.Forms;
using FlashDebugger.Controls.DataTree;
using flash.tools.debugger;
using FlashDebugger.Controls;
using PluginCore;

namespace FlashDebugger
{
    class LocalsUI : DockPanelControl
    {
        private PluginMain pluginMain;
        private DataTreeControl treeControl;
        private static Char[] chTrims = { '.' };

        public LocalsUI(PluginMain pluginMain)
        {
            this.AutoKeyHandling = true;
            this.pluginMain = pluginMain;
            this.treeControl = new DataTreeControl();
            this.treeControl.Tree.BorderStyle = BorderStyle.None;
            this.treeControl.Resize += new EventHandler(this.TreeControlResize);
            this.treeControl.Tree.Font = PluginBase.Settings.DefaultFont;
            this.treeControl.Dock = DockStyle.Fill;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeControl);
        }

        private void TreeControlResize(Object sender, EventArgs e)
        {
            Int32 w = this.treeControl.Width / 2;
            this.treeControl.Tree.Columns[0].Width = w;
            this.treeControl.Tree.Columns[1].Width = w - 8;
        }

        public DataTreeControl TreeControl 
        {
            get { return this.treeControl; }
        }

        public void Clear()
        {
            treeControl.Nodes.Clear();
        }

        public void SetData(Variable[] variables)
        {
            treeControl.Tree.BeginUpdate();
            try
            {
                foreach (Variable item in variables)
                {
                    treeControl.AddNode(new VariableNode(item)
                                            {
                                                HideClassId = PluginMain.settingObject.HideClassIds,
                                                HideFullClasspath = PluginMain.settingObject.HideFullClasspaths
                                            });
                }
            }
            finally
            {
                treeControl.Tree.EndUpdate();
            }
            treeControl.Enabled = true;
        }

    }

}
