// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using PluginCore.Localization;
using PluginCore.Helpers;
using PluginCore;

namespace FlashDevelop.Dialogs
{
    public class ExportDialog : Form
    {
        private System.String languageDirectory;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.ListView itemListView;
        
        private ExportDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.InitializeComponent();
            this.ApplyLocalizedTexts();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.columnHeader = new System.Windows.Forms.ColumnHeader();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.itemListView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(138, 327);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(85, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "&Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButtonClick);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.Location = new System.Drawing.Point(12, 327);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(85, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
            // 
            // saveFileDialog
            //
            this.saveFileDialog.AddExtension = true;
            this.saveFileDialog.DefaultExt = "fdz";
            this.saveFileDialog.Filter = "FlashDevelop Zip Files|*.fdz";
            // 
            // itemListView
            //
            this.itemListView.CheckBoxes = true;
            this.itemListView.MultiSelect = false;
            this.itemListView.FullRowSelect = true;
            this.itemListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.itemListView.Location = new System.Drawing.Point(12, 12);
            this.itemListView.Name = "itemListView";
            this.itemListView.Size = new System.Drawing.Size(212, 310);
            this.itemListView.TabIndex = 3;
            this.itemListView.View = System.Windows.Forms.View.Details;
            this.itemListView.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.itemListView.Columns.Add(this.columnHeader);
            // 
            // ExportDialog
            //
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(235, 361);
            this.Controls.Add(this.itemListView);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ExportDialog";
            this.Text = " Export Language Files";
            this.Load += new System.EventHandler(this.SyntaxPanelExportDialogLoad);
            this.ResumeLayout(false);

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Creates language export dialog
        /// </summary>
        public static ExportDialog CreateDialog(String languageDirectory)
        {
            ExportDialog spe = new ExportDialog();
            spe.languageDirectory = languageDirectory;
            return spe;
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        private void ApplyLocalizedTexts()
        {
            this.Text = TextHelper.GetString("Title.SyntaxExportDialog");
            this.saveFileDialog.Filter = TextHelper.GetString("Info.ZipFilter");
            this.cancelButton.Text = TextHelper.GetString("Label.Cancel");
            this.okButton.Text = TextHelper.GetString("Label.Ok");
        }

        /// <summary>
        /// Populates controls with their default items
        /// </summary>
        private void PopulateControls()
        {
            String[] languageFiles = Directory.GetFiles(this.languageDirectory, "*.xml");
            itemListView.Items.Clear();
            foreach (String lang in languageFiles)
            {
                ListViewItem item = new ListViewItem(Path.GetFileNameWithoutExtension(lang));
                item.Checked = true;
                this.itemListView.Items.Add(item);
            }
            this.columnHeader.Width = -2;
        }

        /// <summary>
        /// Imports or exports the language files
        /// </summary>
        private void OkButtonClick(Object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (this.itemListView.CheckedItems.Count > 0)
                {
                    String xmlFile = "";
                    ZipFile zip = ZipFile.Create(this.saveFileDialog.FileName);
                    zip.BeginUpdate();
                    foreach (ListViewItem item in this.itemListView.CheckedItems)
                    {
                        xmlFile = item.Text + ".xml";
                        zip.Add(Path.Combine(this.languageDirectory, xmlFile), "$(BaseDir)\\Settings\\Languages\\" + xmlFile);
                    }
                    zip.CommitUpdate();
                    zip.Close();
                }
                this.Close();
            }
        }

        /// <summary>
        /// Updates the dialog when loaded
        /// </summary>
        private void SyntaxPanelExportDialogLoad(Object sender, EventArgs e)
        {
            this.PopulateControls();
        }

        /// <summary>
        /// Closes the dialog
        /// </summary>
        private void CancelButtonClick(Object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

    }

}
