// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Windows.Forms;
using Flash.Tools.Debugger;

namespace FlashDebugger
{
    class StackframeUI : DockPanelControl
    {
        private ListView lv;
        private ColumnHeader imageColumnHeader;
        private ColumnHeader frameColumnHeader;
        private PluginMain pluginMain;
        private int currentImageIndex;

        public StackframeUI(PluginMain pluginMain, ImageList imageList)
        {
            this.pluginMain = pluginMain;

            lv = new ListView();
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
            currentImageIndex = imageList.Images.IndexOfKey("StartContinue");

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
                if (item.ImageIndex == currentImageIndex)
                {
                    item.ImageIndex = -1;
                    break;
                }
            }
			lv.SelectedItems[0].ImageIndex = currentImageIndex;
		}

        public void AddFrames(Frame[] frames)
        {
            lv.Items.Clear();
            if (frames.GetLength(0) > 0)
            {
                foreach (Frame item in frames)
                {
					String title = item.CallSignature;
					if (item.Location.File != null) title += " at " + item.Location.File + ":" + item.Location.Line;
                    lv.Items.Add(new ListViewItem(new string[] {"", title}, -1));
                }
				lv.Items[0].ImageIndex = currentImageIndex;
            }
        }

        void lv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
				if (lv.SelectedIndices.Count > 0)
				{
					PluginMain.debugManager.CurrentFrame = lv.SelectedIndices[0];
					ActiveItem();
				}
			}
        }

        void lv_MouseDoubleClick(object sender, MouseEventArgs e)
        {
			if (lv.SelectedIndices.Count > 0)
			{
				if (PluginMain.debugManager.CurrentFrame == lv.SelectedIndices[0])
				{
					Location tmp = PluginMain.debugManager.CurrentLocation;
					PluginMain.debugManager.CurrentLocation = null;
					PluginMain.debugManager.CurrentLocation = tmp;
				}
				else
				{
					PluginMain.debugManager.CurrentFrame = lv.SelectedIndices[0];
					ActiveItem();
				}
			}
        }

    }

}
