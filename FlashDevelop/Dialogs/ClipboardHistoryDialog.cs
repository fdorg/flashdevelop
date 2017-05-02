﻿using System;
using System.Drawing;
using System.Windows.Forms;
using FlashDevelop.Managers;
using PluginCore.Localization;

namespace FlashDevelop.Dialogs
{
    /// <summary>
    /// A dialog displaying the clipboard history from <see cref="ClipboardManager"/>.
    /// </summary>
    public class ClipboardHistoryDialog : Form
    {
        private static ClipboardHistoryDialog current;

        private SplitContainer splitContainer;
        private ListBox listBox;
        private Button btnPaste;
        private Button btnCancel;
        private Button btnClear;
        private RichTextBox preview;

        /// <summary>
        /// Creates a new instance of <see cref="ClipboardHistoryDialog"/>.
        /// </summary>
        public ClipboardHistoryDialog()
        {
            //FormGuid = "8c53f118-99ac-4287-b772-1424c1e2580a";
            InitializeComponent();
            InitializeFont();
            InitializeLocalization();
            InitializeListBox();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            splitContainer = new SplitContainer();
            btnClear = new Button();
            listBox = new ListBox();
            preview = new RichTextBox();
            btnCancel = new Button();
            btnPaste = new Button();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.FixedPanel = FixedPanel.Panel1;
            splitContainer.Location = new Point(0, 0);
            splitContainer.Name = "splitContainer";
            splitContainer.Orientation = Orientation.Horizontal;
            splitContainer.Panel1MinSize = 100;
            splitContainer.Panel2MinSize = 100;
            splitContainer.Size = new Size(482, 453);
            splitContainer.SplitterDistance = 171;
            splitContainer.TabIndex = 0;
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(btnClear);
            splitContainer.Panel1.Controls.Add(listBox);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(preview);
            splitContainer.Panel2.Controls.Add(btnCancel);
            splitContainer.Panel2.Controls.Add(btnPaste);
            // 
            // listBox
            // 
            listBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBox.DisplayMember = "Text";
            listBox.IntegralHeight = false;
            listBox.ItemHeight = 20;
            listBox.Location = new Point(12, 12);
            listBox.Name = "listBox";
            listBox.Size = new Size(458, 120);
            listBox.TabIndex = 0;
            listBox.DrawMode = DrawMode.OwnerDrawFixed;
            listBox.DrawItem += ListBox_DrawItem;
            listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            // 
            // btnPaste
            // 
            btnPaste.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnPaste.DialogResult = DialogResult.OK;
            btnPaste.Enabled = false;
            btnPaste.Location = new Point(264, 236);
            btnPaste.Name = "btnPaste";
            btnPaste.Size = new Size(100, 30);
            btnPaste.TabIndex = 0;
            btnPaste.Text = "Paste";
            btnPaste.UseMnemonic = false;
            btnPaste.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(370, 236);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 30);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseMnemonic = false;
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClear.Enabled = false;
            btnClear.Location = new Point(370, 138);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(100, 30);
            btnClear.TabIndex = 1;
            btnClear.Text = "Clear";
            btnClear.UseMnemonic = false;
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += BtnClear_Click;
            // 
            // preview
            // 
            preview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            preview.BackColor = SystemColors.Window;
            preview.DetectUrls = false;
            preview.Location = new Point(12, 3);
            preview.Name = "preview";
            preview.ReadOnly = true;
            preview.ShortcutsEnabled = false;
            preview.Size = new Size(458, 227);
            preview.TabIndex = 2;
            preview.Text = "";
            preview.WordWrap = false;
            // 
            // ClipboardHistoryDialog
            // 
            AcceptButton = btnPaste;
            AutoScaleDimensions = new SizeF(8f, 20f);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(482, 453);
            Controls.Add(splitContainer);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(300, 400);
            Name = "ClipboardHistoryDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterParent;
            Text = "ClipboardHistoryDialog";
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            splitContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the selected <see cref="ClipboardTextData"/>.
        /// </summary>
        public ClipboardTextData SelectedData
        {
            get { return (ClipboardTextData) listBox.SelectedItem; }
        }

        #endregion

        #region Initialization

        private void InitializeFont()
        {
            Font = Globals.Settings.DefaultFont;
            listBox.ItemHeight = Font.Height;
            preview.Font = Globals.Settings.ConsoleFont;
        }

        private void InitializeLocalization()
        {
            btnPaste.Text = TextHelper.GetStringWithoutMnemonics("Label.Paste");
            btnCancel.Text = TextHelper.GetStringWithoutMnemonics("Label.Cancel");
            btnClear.Text = TextHelper.GetStringWithoutMnemonics("Label.Clear");
        }

        private void InitializeListBox()
        {
            listBox.BeginUpdate();
            foreach (var data in ClipboardManager.History)
            {
                listBox.Items.Insert(0, data);
            }
            listBox.EndUpdate();

            if (listBox.Items.Count > 0)
            {
                listBox.SelectedIndex = 0;
                btnClear.Enabled = true;
            }
        }

        #endregion

        #region Events

        private void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            using (var brush = new SolidBrush(e.BackColor))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);

                string[] lines = listBox.GetItemText(listBox.Items[e.Index]).Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = lines[i].Trim();
                }
                string text = (e.Index + 1) + "    " + string.Join(" ", lines);

                brush.Color = e.ForeColor;

                var stringFormat = new StringFormat()
                {
                    Trimming = StringTrimming.EllipsisCharacter
                };

                e.Graphics.DrawString(text, e.Font, brush, e.Bounds, stringFormat); 
            }
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = (ClipboardTextData) listBox.SelectedItem;
            preview.Text = selectedItem?.Text;
            btnPaste.Enabled = selectedItem != null;
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            listBox.SelectedIndex = -1;

            ClipboardManager.History.Clear();
            listBox.Items.Clear();
            btnClear.Enabled = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Opens a <see cref="ClipboardHistoryDialog"/> and get the user selected <see cref="ClipboardTextData"/>.
        /// </summary>
        /// <param name="data">User selected <see cref="ClipboardTextData"/>.</param>
        public static bool Show(out ClipboardTextData data)
        {
            try
            {
                current = new ClipboardHistoryDialog();
                Globals.MainForm.ThemeControls(current);
                var dialogResult = current.ShowDialog(Globals.MainForm);
                data = current.SelectedData;
                return dialogResult == DialogResult.OK;
            }
            finally
            {
                current?.Dispose();
                current = null;
            }
        }

        /// <summary>
        /// Updates the clipboard history list by adding the new clipboard data to the list.
        /// </summary>
        public static void UpdateHistory()
        {
            if (current != null)
            {
                current.AddNewClipboardData();
            }
        }

        private void AddNewClipboardData()
        {
            listBox.BeginUpdate();
            while (listBox.Items.Count >= ClipboardManager.History.Count)
            {
                listBox.Items.RemoveAt(listBox.Items.Count - 1);
            }
            listBox.Items.Insert(0, ClipboardManager.History.PeekEnd());
            listBox.EndUpdate();

            btnClear.Enabled = listBox.Items.Count > 0;
        }

        #endregion
    }
}
