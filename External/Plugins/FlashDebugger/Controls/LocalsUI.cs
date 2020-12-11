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
        public LocalsUI()
        {
            AutoKeyHandling = true;
            TreeControl = new DataTreeControl();
            TreeControl.Tree.BorderStyle = BorderStyle.None;
            TreeControl.Resize += TreeControlResize;
            TreeControl.Tree.Font = PluginBase.Settings.DefaultFont;
            TreeControl.Dock = DockStyle.Fill;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(TreeControl);
        }

        void TreeControlResize(object sender, EventArgs e)
        {
            int w = TreeControl.Width / 2;
            TreeControl.Tree.Columns[0].Width = w;
            TreeControl.Tree.Columns[1].Width = w - 8;
        }

        public DataTreeControl TreeControl { get; }

        public void Clear() => TreeControl.Nodes.Clear();

        public void SetData(Variable[] variables)
        {
            TreeControl.Tree.BeginUpdate();
            try
            {
                foreach (Variable item in variables)
                {
                    TreeControl.AddNode(new VariableNode(item)
                                            {
                                                HideClassId = PluginMain.settingObject.HideClassIds,
                                                HideFullClasspath = PluginMain.settingObject.HideFullClasspaths
                                            });
                }
            }
            finally
            {
                TreeControl.Tree.EndUpdate();
            }
            TreeControl.Enabled = true;
        }
    }
}