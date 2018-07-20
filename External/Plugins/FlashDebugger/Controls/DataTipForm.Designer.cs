/*
    Copyright (C) 2009  Robert Nelson

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
using System.Drawing;
using System.Windows.Forms;

namespace FlashDebugger.Controls
{
    partial class DataTipForm
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataTreeControl = new FlashDebugger.Controls.DataTreeControl();
            this.SuspendLayout();
            // 
            // dataTreeControl
            // 
            this.dataTreeControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataTreeControl.Location = new System.Drawing.Point(4, 4);
            this.dataTreeControl.Name = "dataTreeControl";
            this.dataTreeControl.Size = new System.Drawing.Size(182, 180);
            this.dataTreeControl.TabIndex = 0;
            // 
            // DataTipForm
            // 
            this.ClientSize = new System.Drawing.Size(190, 188);
            this.ControlBox = false;
            this.Controls.Add(this.dataTreeControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DataTipForm";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion

        private DataTreeControl dataTreeControl;
    }
}
