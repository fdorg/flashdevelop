// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Windows.Forms;
using FlashDebugger.Controls.DataTree;
using flash.tools.debugger;
using FlashDebugger.Controls;
using PluginCore;

namespace FlashDebugger
{
    internal class LocalsUI : DockPanelControl
    {
        PluginMain pluginMain;
        readonly DataTreeControl treeControl;
        static char[] chTrims = { '.' };

        public LocalsUI(PluginMain pluginMain)
        {
            AutoKeyHandling = true;
            this.pluginMain = pluginMain;
            treeControl = new DataTreeControl();
            treeControl.Tree.BorderStyle = BorderStyle.None;
            treeControl.Resize += TreeControlResize;
            treeControl.Tree.Font = PluginBase.Settings.DefaultFont;
            treeControl.Dock = DockStyle.Fill;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(treeControl);
        }

        void TreeControlResize(object sender, EventArgs e)
        {
            int w = treeControl.Width / 2;
            treeControl.Tree.Columns[0].Width = w;
            treeControl.Tree.Columns[1].Width = w - 8;
        }

        public DataTreeControl TreeControl => treeControl;

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
