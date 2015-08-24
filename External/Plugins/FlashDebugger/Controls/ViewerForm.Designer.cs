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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FlashDebugger.Controls
{
    partial class ViewerForm
    {
        /// <summary>
        /// 
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows

        /// <summary>
        /// </summary>
        private void InitializeComponent()
        {
            this.ExptextBox = new TextBox();
            this.ValuetextBox = new TextBox();
            this.WordWrapcheckBox = new CheckBox();
            this.Closebutton = new Button();
            this.CopyExpbutton = new Button();
            this.CopyValuebutton = new Button();
            this.CopyAllbutton = new Button();
            this.label1 = new Label();
            this.label2 = new Label();
            this.SuspendLayout();
            // 
            // ExptextBox
            // 
            this.ExptextBox.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
                        | AnchorStyles.Right)));
            this.ExptextBox.BackColor = SystemColors.Window;
            this.ExptextBox.Location = new Point(10, 25);
            this.ExptextBox.Name = "ExptextBox";
            this.ExptextBox.ReadOnly = true;
            this.ExptextBox.Size = new Size(306, 21);
            this.ExptextBox.TabIndex = 0;
            // 
            // ValuetextBox
            // 
            this.ValuetextBox.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
                        | AnchorStyles.Left)
                        | AnchorStyles.Right)));
            this.ValuetextBox.BackColor = SystemColors.Window;
            this.ValuetextBox.Location = new Point(10, 72);
            this.ValuetextBox.Multiline = true;
            this.ValuetextBox.Name = "ValuetextBox";
            this.ValuetextBox.ReadOnly = true;
            this.ValuetextBox.Size = new Size(306, 157);
            this.ValuetextBox.TabIndex = 1;
            this.ValuetextBox.WordWrap = false;
            // 
            // WordWrapcheckBox
            // 
            this.WordWrapcheckBox.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            this.WordWrapcheckBox.AutoSize = true;
            this.WordWrapcheckBox.Location = new Point(12, 235);
            this.WordWrapcheckBox.Name = "WordWrapcheckBox";
            this.WordWrapcheckBox.Size = new Size(78, 17);
            this.WordWrapcheckBox.TabIndex = 2;
            this.WordWrapcheckBox.Text = "WordWrap";
            this.WordWrapcheckBox.UseVisualStyleBackColor = true;
            this.WordWrapcheckBox.CheckedChanged += new EventHandler(this.WordWrapcheckBox_CheckedChanged);
            // 
            // Closebutton
            // 
            this.Closebutton.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this.Closebutton.Location = new Point(331, 206);
            this.Closebutton.Name = "Closebutton";
            this.Closebutton.Size = new Size(96, 23);
            this.Closebutton.TabIndex = 3;
            this.Closebutton.Text = "Close";
            this.Closebutton.UseVisualStyleBackColor = true;
            this.Closebutton.Click += new EventHandler(this.Closebutton_Click);
            // 
            // CopyExpbutton
            // 
            this.CopyExpbutton.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.CopyExpbutton.Location = new Point(331, 24);
            this.CopyExpbutton.Name = "CopyExpbutton";
            this.CopyExpbutton.Size = new Size(96, 23);
            this.CopyExpbutton.TabIndex = 4;
            this.CopyExpbutton.Text = "Copy Exp";
            this.CopyExpbutton.UseVisualStyleBackColor = true;
            this.CopyExpbutton.Click += new EventHandler(this.CopyExpbutton_Click);
            // 
            // CopyValuebutton
            // 
            this.CopyValuebutton.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.CopyValuebutton.Location = new Point(331, 53);
            this.CopyValuebutton.Name = "CopyValuebutton";
            this.CopyValuebutton.Size = new Size(96, 23);
            this.CopyValuebutton.TabIndex = 5;
            this.CopyValuebutton.Text = "Copy Value";
            this.CopyValuebutton.UseVisualStyleBackColor = true;
            this.CopyValuebutton.Click += new EventHandler(this.CopyValuebutton_Click);
            // 
            // CopyAllbutton
            // 
            this.CopyAllbutton.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.CopyAllbutton.Location = new Point(331, 82);
            this.CopyAllbutton.Name = "CopyAllbutton";
            this.CopyAllbutton.Size = new Size(96, 23);
            this.CopyAllbutton.TabIndex = 6;
            this.CopyAllbutton.Text = "Copy All";
            this.CopyAllbutton.UseVisualStyleBackColor = true;
            this.CopyAllbutton.Click += new EventHandler(this.CopyAllbutton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(9, 8);
            this.label1.Name = "label1";
            this.label1.Size = new Size(25, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Exp";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new Point(9, 55);
            this.label2.Name = "label2";
            this.label2.Size = new Size(33, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Value";
            // 
            // ViewerForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(438, 261);
            this.MinimumSize = new Size(250, 200);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CopyAllbutton);
            this.Controls.Add(this.CopyValuebutton);
            this.Controls.Add(this.CopyExpbutton);
            this.Controls.Add(this.Closebutton);
            this.Controls.Add(this.WordWrapcheckBox);
            this.Controls.Add(this.ValuetextBox);
            this.Controls.Add(this.ExptextBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewerForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "ViewerForm";
            this.FormClosing += new FormClosingEventHandler(this.ViewerForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox ExptextBox;
        private TextBox ValuetextBox;
        private CheckBox WordWrapcheckBox;
        private Button Closebutton;
        private Button CopyExpbutton;
        private Button CopyValuebutton;
        private Button CopyAllbutton;
        private Label label1;
        private Label label2;
    }
}
