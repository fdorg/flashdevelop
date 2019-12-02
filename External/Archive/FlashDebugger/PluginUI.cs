// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Windows.Forms;
using Flash.Tools.Debugger;
using FlashDebugger.Controls;
using PluginCore;

namespace FlashDebugger
{
    class PluginUI : DockPanelControl
    {
        private PluginMain pluginMain;
        private DataTreeControl treeControl;
        private static Char[] chTrims = { '.' };

        public PluginUI(PluginMain pluginMain)
        {
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
			treeControl.Clear();
		}

        public void SetData(Variable[] variables)
        {
            treeControl.Tree.BeginUpdate();
            try
            {
                foreach (Variable item in variables)
                {
					treeControl.AddNode(new DataNode(item));
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
