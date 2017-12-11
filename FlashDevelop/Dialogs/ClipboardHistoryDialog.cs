using System;
using System.Drawing;
using System.Windows.Forms;
using FlashDevelop.Managers;
using PluginCore.Localization;
using PluginCore.Controls;
using PluginCore.Helpers;

namespace FlashDevelop.Dialogs
{
    public class ClipboardHistoryDialog : SmartForm
    {
        private Button btnCopy;
        private Button btnClear;
        private Button btnPaste;
        private Button btnCancel;
        private ListBox listBox;
        private TextBox previewBox;
        private SplitContainer splitContainer;
        private static ClipboardHistoryDialog current;

        public ClipboardHistoryDialog()
        {
            this.Owner = Globals.MainForm;
            this.FormGuid = "9c9f995e-ea37-4359-8e3c-28a57f10f249";
            this.InitializeComponent();
            this.InitializeFont();
            this.InitializeLocalization();
            this.InitializeListBox();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            splitContainer = new SplitContainer();
            btnClear = new ButtonEx();
            listBox = new ListBoxEx();
            previewBox = new TextBoxEx();
            btnCancel = new ButtonEx();
            btnCopy = new ButtonEx();
            btnPaste = new ButtonEx();
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
            splitContainer.Panel2.Controls.Add(previewBox);
            splitContainer.Panel2.Controls.Add(btnCopy);
            splitContainer.Panel2.Controls.Add(btnCancel);
            splitContainer.Panel2.Controls.Add(btnPaste);
            // 
            // listBox
            // 
            listBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBox.DisplayMember = "Text";
            listBox.IntegralHeight = false;
            listBox.ItemHeight = 20;
            listBox.Location = new Point(6, 10);
            listBox.Name = "listBox";
            listBox.Size = new Size(470, 106);
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
            btnPaste.Location = new Point(269, 232);
            btnPaste.Name = "btnPaste";
            btnPaste.Size = new Size(100, 34);
            btnPaste.TabIndex = 0;
            btnPaste.Text = "Paste";
            btnPaste.UseMnemonic = false;
            btnPaste.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(377, 232);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 34);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseMnemonic = false;
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnCopy
            // 
            btnCopy.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCopy.Enabled = false;
            btnCopy.Location = new Point(161, 232);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(100, 34);
            btnCopy.TabIndex = 2;
            btnCopy.Text = "Copy";
            btnCopy.UseMnemonic = false;
            btnCopy.UseVisualStyleBackColor = true;
            btnCopy.Click += BtnCopy_Click;
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClear.Enabled = false;
            btnClear.Location = new Point(377, 124);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(100, 34);
            btnClear.TabIndex = 1;
            btnClear.Text = "Clear";
            btnClear.UseMnemonic = false;
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += BtnClear_Click;
            // 
            // preview
            // 
            previewBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            previewBox.BackColor = SystemColors.Window;
            previewBox.BorderStyle = BorderStyle.FixedSingle;
            previewBox.Location = new Point(6, 0);
            previewBox.Multiline = true;
            previewBox.Name = "preview";
            previewBox.ReadOnly = true;
            previewBox.Size = new Size(470, 224);
            previewBox.TabIndex = 3;
            previewBox.Text = "";
            previewBox.WordWrap = false;
            // 
            // ClipboardHistoryDialog
            // 
            AcceptButton = btnPaste;
            AutoScaleDimensions = new SizeF(8f, 20f);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(484, 454);
            Controls.Add(splitContainer);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(300, 420);
            Name = "ClipboardHistoryDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Clipboard History";
            Padding = new Padding(10);
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
            get { return (ClipboardTextData)listBox.SelectedItem; }
        }

        #endregion

        #region Initialization

        private void InitializeFont()
        {
            Font = Globals.Settings.DefaultFont;
            previewBox.Font = Globals.Settings.ConsoleFont;
            listBox.ItemHeight = Font.Height;
        }

        private void InitializeLocalization()
        {
            Text = " " + TextHelper.GetStringWithoutMnemonics("Label.ClipboardHistory");
            btnPaste.Text = TextHelper.GetStringWithoutMnemonics("Label.Paste");
            btnCancel.Text = TextHelper.GetStringWithoutMnemonics("Label.Cancel");
            btnCopy.Text = TextHelper.GetStringWithoutMnemonics("Label.Copy");
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
                using (var stringFormat = new StringFormat())
                {
                    stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                    e.Graphics.DrawString(text, e.Font, brush, e.Bounds, stringFormat);
                }
            }
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = (ClipboardTextData)listBox.SelectedItem;
            previewBox.Text = selectedItem?.Text;
            btnPaste.Enabled = selectedItem != null;
            btnCopy.Enabled = !string.IsNullOrEmpty(selectedItem?.Text);
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            previewBox.SelectAll();
            previewBox.Copy();
            btnPaste.Select();
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

        /// <summary>
        /// 
        /// </summary>
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
