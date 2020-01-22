﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using PluginCore.Controls;

namespace FlashDebugger
{
    internal class ThreadsUI : DockPanelControl
    {
        readonly ListViewEx lv;
        readonly ColumnHeader imageColumnHeader;
        readonly ColumnHeader frameColumnHeader;
        PluginMain pluginMain;
        readonly int runningImageIndex;
        readonly int suspendedImageIndex;

        public ThreadsUI(PluginMain pluginMain, ImageList imageList)
        {
            AutoKeyHandling = true;
            this.pluginMain = pluginMain;
            lv = new ListViewEx();
            lv.ShowItemToolTips = true;
            imageColumnHeader = new ColumnHeader();
            imageColumnHeader.Text = string.Empty;
            imageColumnHeader.Width = 20;
            frameColumnHeader = new ColumnHeader();
            frameColumnHeader.Text = string.Empty;
            lv.Columns.AddRange(new[] {
            imageColumnHeader,
            frameColumnHeader});
            lv.FullRowSelect = true;
            lv.BorderStyle = BorderStyle.None;
            lv.Dock = DockStyle.Fill;
            lv.SmallImageList = imageList;
            runningImageIndex = imageList.Images.IndexOfKey("StartContinue");
            suspendedImageIndex = imageList.Images.IndexOfKey("Pause");
            lv.View = View.Details;
            lv.MouseDoubleClick += lv_MouseDoubleClick;
            lv.KeyDown += lv_KeyDown;
            lv.SizeChanged += lv_SizeChanged;
            Controls.Add(lv);
            ScrollBarEx.Attach(lv);
        }

        void lv_SizeChanged(object sender, EventArgs e)
        {
            frameColumnHeader.Width = lv.Width - imageColumnHeader.Width;
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

        public void SetThreads(Dictionary<int, FlashInterface.IsolateInfo> isolates)
        {
            lv.Items.Clear();
            if (PluginMain.debugManager.FlashInterface.Session is null)
            {
                return;
            }
            // add primary -- flash specific
            string title = "Main thread";
            int image = PluginMain.debugManager.FlashInterface.Session.isSuspended() ? suspendedImageIndex : runningImageIndex;
            lv.Items.Add(new ListViewItem(new[] { "", title }, image));
            lv.Items[lv.Items.Count - 1].Tag = 1;
            foreach (KeyValuePair<int, FlashInterface.IsolateInfo> ii_pair in isolates)
            {
                int i_id = ii_pair.Key;
                FlashInterface.IsolateInfo ii = ii_pair.Value;
                title = "Worker " + i_id;
                image = ii.i_Session.isSuspended() ? suspendedImageIndex : runningImageIndex;
                lv.Items.Add(new ListViewItem(new[] { "", title }, image));
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
