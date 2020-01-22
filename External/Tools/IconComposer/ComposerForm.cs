// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace IconComposer
{
    public partial class ComposerForm : Form
    {
        private ComposedIcon info;
        private int currentIndex;

        public ComposerForm()
        {
            Font = SystemFonts.MenuFont;
            InitializeComponent();

            pictureBox.Load("../../Settings/Images.png");
            pictureBox.Width = pictureBox.Image.Width;
            pictureBox.Height = pictureBox.Image.Height;

            info = new ComposedIcon();
            UpdatePreview();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            int x = Math.Min(e.X, 16 * 16 - 1) / 16;
            int y = e.Y / 16;
            currentIndex = (y * 16 + x);
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            // bullet icon
            if (currentIndex < 32)
            {
                info.bullet = currentIndex;
            }
            // base icon
            else
            {
                info.icon = currentIndex;
            }
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            // icon code
            labelCode.Text = info.ToString();

            // draw preview
            Image src = pictureBox.Image;
            previewBox.Image = info.GetComposedBitmap(src);
        }

        private void numX_ValueChanged(object sender, EventArgs e)
        {
            info.x = (int)numX.Value;
            UpdatePreview();
        }

        private void numY_ValueChanged(object sender, EventArgs e)
        {
            info.y = (int)numY.Value;
            UpdatePreview();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            info.Clear();
            numX.Value = info.x;
            numY.Value = info.x;
            UpdatePreview();
        }

        private void buttonPreview_Click(object sender, EventArgs e)
        {
            info.deserialize(labelCode.Text);
            numX.Value = info.x;
            numY.Value = info.y;
            UpdatePreview();
        }
    }
}