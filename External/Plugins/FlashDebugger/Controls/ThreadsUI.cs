using System;
using System.Windows.Forms;
using flash.tools.debugger;
using System.Collections.Generic;

namespace FlashDebugger
{
    class ThreadsUI : DockPanelControl
    {
        private ListViewEx lv;
        private ColumnHeader imageColumnHeader;
        private ColumnHeader frameColumnHeader;
        private PluginMain pluginMain;
        private int runningImageIndex;
        private int suspendedImageIndex;

        public ThreadsUI(PluginMain pluginMain, ImageList imageList)
        {
            this.AutoKeyHandling = true;
            this.pluginMain = pluginMain;
            lv = new ListViewEx();
            lv.ShowItemToolTips = true;
            this.imageColumnHeader = new ColumnHeader();
            this.imageColumnHeader.Text = string.Empty;
            this.imageColumnHeader.Width = 20;
            this.frameColumnHeader = new ColumnHeader();
            this.frameColumnHeader.Text = string.Empty;
            lv.Columns.AddRange(new ColumnHeader[] {
            this.imageColumnHeader,
            this.frameColumnHeader});
            lv.FullRowSelect = true;
            lv.BorderStyle = BorderStyle.None;
            lv.Dock = System.Windows.Forms.DockStyle.Fill;
            lv.SmallImageList = imageList;
            runningImageIndex = imageList.Images.IndexOfKey("StartContinue");
            suspendedImageIndex = imageList.Images.IndexOfKey("Pause");
            lv.View = System.Windows.Forms.View.Details;
            lv.MouseDoubleClick += new MouseEventHandler(lv_MouseDoubleClick);
            lv.KeyDown += new KeyEventHandler(lv_KeyDown);
            lv.SizeChanged += new EventHandler(lv_SizeChanged);
            this.Controls.Add(lv);
        }

        void lv_SizeChanged(object sender, EventArgs e)
        {
            this.frameColumnHeader.Width = lv.Width - this.imageColumnHeader.Width;
        }

        public void ClearItem()
        {
            lv.Items.Clear();
        }

        public void ActiveItem()
        {
            foreach (ListViewItem item in lv.Items)
            {
                if ((int)item.Tag == PluginMain.debugManager.FlashInterface.ActiveSession)
                {
                    item.Font = new System.Drawing.Font(item.Font, System.Drawing.FontStyle.Bold);
                }
                else
                {
                    item.Font = new System.Drawing.Font(item.Font, System.Drawing.FontStyle.Regular);
                }
            }
        }

        public void SetThreads(Dictionary<int, FlashDebugger.FlashInterface.IsolateInfo> isolates)
        {
            lv.Items.Clear();
            if (PluginMain.debugManager.FlashInterface.Session == null)
            {
                return;
            }
            // add primary -- flash specific
            String title = "Main thread";
            int image = PluginMain.debugManager.FlashInterface.Session.isSuspended() ? suspendedImageIndex : runningImageIndex;
            lv.Items.Add(new ListViewItem(new string[] { "", title }, image));
            lv.Items[lv.Items.Count - 1].Tag = 1;
            foreach (KeyValuePair<int, FlashDebugger.FlashInterface.IsolateInfo> ii_pair in isolates)
            {
                int i_id = ii_pair.Key;
                FlashDebugger.FlashInterface.IsolateInfo ii = ii_pair.Value;
                title = "Worker " + i_id;
                image = ii.i_Session.isSuspended() ? suspendedImageIndex : runningImageIndex;
                lv.Items.Add(new ListViewItem(new string[] { "", title }, image));
                lv.Items[lv.Items.Count - 1].Tag = i_id;
            }
            ActiveItem();
        }

        void lv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (lv.SelectedIndices.Count > 0)
                {
                    PluginMain.debugManager.FlashInterface.ActiveSession = (int)lv.SelectedItems[0].Tag;
                    ActiveItem();
                }
            }
        }

        void lv_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lv.SelectedIndices.Count > 0)
            {
                PluginMain.debugManager.FlashInterface.ActiveSession = (int)lv.SelectedItems[0].Tag;
                ActiveItem();
            }
        }

    }

}
