using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ProjectManager.Projects;
using ProjectManager.Projects.AS2;
using PluginCore.Localization;

namespace ProjectManager.Controls.AS2
{
    public class LibraryAssetDialog : System.Windows.Forms.Form
    {
        Boolean isAS3;
        Boolean isSWC;
        Boolean modified;
        LibraryAsset asset;

        #region Windows Form Designer

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage swfTabPage;
        private System.Windows.Forms.RadioButton addPreloaderButton;
        private System.Windows.Forms.LinkLabel explainLink;
        private System.Windows.Forms.RadioButton sharedLibraryButton;
        private System.Windows.Forms.RadioButton addLibraryButton;
        private System.Windows.Forms.TabPage fontTabPage;
        private System.Windows.Forms.TextBox charactersTextBox;
        private System.Windows.Forms.RadioButton embedTheseButton;
        private System.Windows.Forms.RadioButton embedAllButton;
        private System.Windows.Forms.CheckBox autoIDBox;
        private System.Windows.Forms.TextBox idTextBox;
        private System.Windows.Forms.CheckBox keepUpdatedBox;
        private System.Windows.Forms.TextBox updatedTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.CheckBox specifySharepointBox;
        private System.Windows.Forms.TextBox sharepointTextBox;
        private System.Windows.Forms.TabPage swcTabPage;
        private System.Windows.Forms.RadioButton swcIncOption;
        private System.Windows.Forms.RadioButton swcExtOption;
        private System.Windows.Forms.RadioButton swcLibOption;
        private System.Windows.Forms.CheckBox bitmapLinkageBox;
        private System.Windows.Forms.TabPage advancedTabPage;
        
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.autoIDBox = new System.Windows.Forms.CheckBox();
            this.idTextBox = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.swfTabPage = new System.Windows.Forms.TabPage();
            this.specifySharepointBox = new System.Windows.Forms.CheckBox();
            this.addPreloaderButton = new System.Windows.Forms.RadioButton();
            this.explainLink = new System.Windows.Forms.LinkLabel();
            this.sharepointTextBox = new System.Windows.Forms.TextBox();
            this.sharedLibraryButton = new System.Windows.Forms.RadioButton();
            this.addLibraryButton = new System.Windows.Forms.RadioButton();
            this.swcTabPage = new System.Windows.Forms.TabPage();
            this.swcIncOption = new System.Windows.Forms.RadioButton();
            this.swcExtOption = new System.Windows.Forms.RadioButton();
            this.swcLibOption = new System.Windows.Forms.RadioButton();
            this.fontTabPage = new System.Windows.Forms.TabPage();
            this.charactersTextBox = new System.Windows.Forms.TextBox();
            this.embedTheseButton = new System.Windows.Forms.RadioButton();
            this.embedAllButton = new System.Windows.Forms.RadioButton();
            this.advancedTabPage = new System.Windows.Forms.TabPage();
            this.browseButton = new System.Windows.Forms.Button();
            this.updatedTextBox = new System.Windows.Forms.TextBox();
            this.keepUpdatedBox = new System.Windows.Forms.CheckBox();
            this.bitmapLinkageBox = new System.Windows.Forms.CheckBox();
            this.tabControl.SuspendLayout();
            this.swfTabPage.SuspendLayout();
            this.swcTabPage.SuspendLayout();
            this.fontTabPage.SuspendLayout();
            this.advancedTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(239, 202);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 21);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Location = new System.Drawing.Point(156, 202);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 21);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "&OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // autoIDBox
            // 
            this.autoIDBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.autoIDBox.Checked = true;
            this.autoIDBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoIDBox.Location = new System.Drawing.Point(16, 73);
            this.autoIDBox.Name = "autoIDBox";
            this.autoIDBox.Size = new System.Drawing.Size(266, 18);
            this.autoIDBox.TabIndex = 0;
            this.autoIDBox.Text = "Auto-generate &ID for attachMovie():";
            this.autoIDBox.CheckedChanged += new System.EventHandler(this.autoIDBox_CheckedChanged);
            // 
            // idTextBox
            // 
            this.idTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.idTextBox.Enabled = false;
            this.idTextBox.Location = new System.Drawing.Point(34, 94);
            this.idTextBox.Name = "idTextBox";
            this.idTextBox.Size = new System.Drawing.Size(248, 20);
            this.idTextBox.TabIndex = 1;
            this.idTextBox.Text = "Library.WorkerGuy.png";
            this.idTextBox.TextChanged += new System.EventHandler(this.idTextBox_TextChanged);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.swfTabPage);
            this.tabControl.Controls.Add(this.swcTabPage);
            this.tabControl.Controls.Add(this.fontTabPage);
            this.tabControl.Controls.Add(this.advancedTabPage);
            this.tabControl.Location = new System.Drawing.Point(10, 11);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(306, 185);
            this.tabControl.TabIndex = 0;
            // 
            // swfTabPage
            // 
            this.swfTabPage.Controls.Add(this.specifySharepointBox);
            this.swfTabPage.Controls.Add(this.addPreloaderButton);
            this.swfTabPage.Controls.Add(this.explainLink);
            this.swfTabPage.Controls.Add(this.sharepointTextBox);
            this.swfTabPage.Controls.Add(this.sharedLibraryButton);
            this.swfTabPage.Controls.Add(this.addLibraryButton);
            this.swfTabPage.Location = new System.Drawing.Point(4, 22);
            this.swfTabPage.Name = "swfTabPage";
            this.swfTabPage.Size = new System.Drawing.Size(298, 159);
            this.swfTabPage.TabIndex = 2;
            this.swfTabPage.Text = "SWF File";
            this.swfTabPage.UseVisualStyleBackColor = true;
            // 
            // specifySharepointBox
            // 
            this.specifySharepointBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.specifySharepointBox.BackColor = System.Drawing.Color.Transparent;
            this.specifySharepointBox.Location = new System.Drawing.Point(35, 81);
            this.specifySharepointBox.Name = "specifySharepointBox";
            this.specifySharepointBox.Size = new System.Drawing.Size(197, 18);
            this.specifySharepointBox.TabIndex = 3;
            this.specifySharepointBox.Text = "&Specify sharepoint ID:";
            this.specifySharepointBox.UseVisualStyleBackColor = false;
            this.specifySharepointBox.CheckedChanged += new System.EventHandler(this.specifySharepointBox_CheckedChanged);
            // 
            // addPreloaderButton
            // 
            this.addPreloaderButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addPreloaderButton.Location = new System.Drawing.Point(16, 36);
            this.addPreloaderButton.Name = "addPreloaderButton";
            this.addPreloaderButton.Size = new System.Drawing.Size(184, 16);
            this.addPreloaderButton.TabIndex = 1;
            this.addPreloaderButton.Text = " Add as &preloader";
            this.addPreloaderButton.CheckedChanged += new System.EventHandler(this.addPreloaderButton_CheckedChanged);
            // 
            // explainLink
            // 
            this.explainLink.Location = new System.Drawing.Point(155, 8);
            this.explainLink.Name = "explainLink";
            this.explainLink.Size = new System.Drawing.Size(136, 16);
            this.explainLink.TabIndex = 5;
            this.explainLink.TabStop = true;
            this.explainLink.Text = "Explain these options";
            this.explainLink.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.explainLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.explainLink_LinkClicked);
            // 
            // sharepointTextBox
            // 
            this.sharepointTextBox.Enabled = false;
            this.sharepointTextBox.Location = new System.Drawing.Point(35, 102);
            this.sharepointTextBox.Name = "sharepointTextBox";
            this.sharepointTextBox.Size = new System.Drawing.Size(197, 20);
            this.sharepointTextBox.TabIndex = 4;
            this.sharepointTextBox.TextChanged += new System.EventHandler(this.sharepointTextBox_TextChanged);
            // 
            // sharedLibraryButton
            // 
            this.sharedLibraryButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.sharedLibraryButton.Location = new System.Drawing.Point(16, 57);
            this.sharedLibraryButton.Name = "sharedLibraryButton";
            this.sharedLibraryButton.Size = new System.Drawing.Size(224, 16);
            this.sharedLibraryButton.TabIndex = 2;
            this.sharedLibraryButton.Text = " Load at &runtime (shared library)";
            this.sharedLibraryButton.CheckedChanged += new System.EventHandler(this.sharedLibraryButton_CheckedChanged);
            // 
            // addLibraryButton
            // 
            this.addLibraryButton.Checked = true;
            this.addLibraryButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addLibraryButton.Location = new System.Drawing.Point(16, 16);
            this.addLibraryButton.Name = "addLibraryButton";
            this.addLibraryButton.Size = new System.Drawing.Size(112, 16);
            this.addLibraryButton.TabIndex = 0;
            this.addLibraryButton.TabStop = true;
            this.addLibraryButton.Text = " Add to &library";
            this.addLibraryButton.CheckedChanged += new System.EventHandler(this.addLibraryButton_CheckedChanged);
            // 
            // swcTabPage
            // 
            this.swcTabPage.Controls.Add(this.swcIncOption);
            this.swcTabPage.Controls.Add(this.swcExtOption);
            this.swcTabPage.Controls.Add(this.swcLibOption);
            this.swcTabPage.Location = new System.Drawing.Point(4, 22);
            this.swcTabPage.Name = "swcTabPage";
            this.swcTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.swcTabPage.Size = new System.Drawing.Size(298, 159);
            this.swcTabPage.TabIndex = 3;
            this.swcTabPage.Text = "SWC File";
            this.swcTabPage.UseVisualStyleBackColor = true;
            // 
            // swcIncOption
            // 
            this.swcIncOption.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.swcIncOption.Location = new System.Drawing.Point(16, 36);
            this.swcIncOption.Name = "swcIncOption";
            this.swcIncOption.Size = new System.Drawing.Size(250, 16);
            this.swcIncOption.TabIndex = 4;
            this.swcIncOption.Text = "&Included library (include completely)";
            this.swcIncOption.CheckedChanged += new System.EventHandler(this.swcLibOption_CheckedChanged);
            // 
            // swcExtOption
            // 
            this.swcExtOption.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.swcExtOption.Location = new System.Drawing.Point(16, 57);
            this.swcExtOption.Name = "swcExtOption";
            this.swcExtOption.Size = new System.Drawing.Size(250, 16);
            this.swcExtOption.TabIndex = 5;
            this.swcExtOption.Text = "&External library (not included)";
            this.swcExtOption.CheckedChanged += new System.EventHandler(this.swcLibOption_CheckedChanged);
            // 
            // swcLibOption
            // 
            this.swcLibOption.Checked = true;
            this.swcLibOption.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.swcLibOption.Location = new System.Drawing.Point(16, 16);
            this.swcLibOption.Name = "swcLibOption";
            this.swcLibOption.Size = new System.Drawing.Size(250, 16);
            this.swcLibOption.TabIndex = 3;
            this.swcLibOption.TabStop = true;
            this.swcLibOption.Text = "&Library (include referenced classes)";
            this.swcLibOption.CheckedChanged += new System.EventHandler(this.swcLibOption_CheckedChanged);
            // 
            // fontTabPage
            // 
            this.fontTabPage.Controls.Add(this.charactersTextBox);
            this.fontTabPage.Controls.Add(this.embedTheseButton);
            this.fontTabPage.Controls.Add(this.embedAllButton);
            this.fontTabPage.Location = new System.Drawing.Point(4, 22);
            this.fontTabPage.Name = "fontTabPage";
            this.fontTabPage.Size = new System.Drawing.Size(298, 159);
            this.fontTabPage.TabIndex = 1;
            this.fontTabPage.Text = "Font";
            this.fontTabPage.UseVisualStyleBackColor = true;
            // 
            // charactersTextBox
            // 
            this.charactersTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.charactersTextBox.Location = new System.Drawing.Point(35, 64);
            this.charactersTextBox.Multiline = true;
            this.charactersTextBox.Name = "charactersTextBox";
            this.charactersTextBox.Size = new System.Drawing.Size(247, 79);
            this.charactersTextBox.TabIndex = 2;
            this.charactersTextBox.TextChanged += new System.EventHandler(this.charactersTextBox_TextChanged);
            // 
            // embedTheseButton
            // 
            this.embedTheseButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.embedTheseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.embedTheseButton.Location = new System.Drawing.Point(16, 36);
            this.embedTheseButton.Name = "embedTheseButton";
            this.embedTheseButton.Size = new System.Drawing.Size(266, 16);
            this.embedTheseButton.TabIndex = 1;
            this.embedTheseButton.Text = " Embed &these characters:";
            this.embedTheseButton.CheckedChanged += new System.EventHandler(this.embedTheseButton_CheckedChanged);
            // 
            // embedAllButton
            // 
            this.embedAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.embedAllButton.Checked = true;
            this.embedAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.embedAllButton.Location = new System.Drawing.Point(16, 16);
            this.embedAllButton.Name = "embedAllButton";
            this.embedAllButton.Size = new System.Drawing.Size(258, 16);
            this.embedAllButton.TabIndex = 0;
            this.embedAllButton.TabStop = true;
            this.embedAllButton.Text = " Embed &all characters";
            this.embedAllButton.CheckedChanged += new System.EventHandler(this.embedAllButton_CheckedChanged);
            // 
            // advancedTabPage
            // 
            this.advancedTabPage.Controls.Add(this.bitmapLinkageBox);
            this.advancedTabPage.Controls.Add(this.browseButton);
            this.advancedTabPage.Controls.Add(this.updatedTextBox);
            this.advancedTabPage.Controls.Add(this.keepUpdatedBox);
            this.advancedTabPage.Controls.Add(this.autoIDBox);
            this.advancedTabPage.Controls.Add(this.idTextBox);
            this.advancedTabPage.Location = new System.Drawing.Point(4, 22);
            this.advancedTabPage.Name = "advancedTabPage";
            this.advancedTabPage.Size = new System.Drawing.Size(298, 159);
            this.advancedTabPage.TabIndex = 0;
            this.advancedTabPage.Text = "Advanced";
            this.advancedTabPage.UseVisualStyleBackColor = true;
            // 
            // browseButton
            // 
            this.browseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.browseButton.Location = new System.Drawing.Point(210, 35);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(72, 21);
            this.browseButton.TabIndex = 4;
            this.browseButton.Text = "&Browse...";
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // updatedTextBox
            // 
            this.updatedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.updatedTextBox.Enabled = false;
            this.updatedTextBox.Location = new System.Drawing.Point(34, 37);
            this.updatedTextBox.Name = "updatedTextBox";
            this.updatedTextBox.Size = new System.Drawing.Size(170, 20);
            this.updatedTextBox.TabIndex = 3;
            this.updatedTextBox.TextChanged += new System.EventHandler(this.updatedTextBox_TextChanged);
            // 
            // keepUpdatedBox
            // 
            this.keepUpdatedBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.keepUpdatedBox.Location = new System.Drawing.Point(16, 16);
            this.keepUpdatedBox.Name = "keepUpdatedBox";
            this.keepUpdatedBox.Size = new System.Drawing.Size(266, 18);
            this.keepUpdatedBox.TabIndex = 2;
            this.keepUpdatedBox.Text = "&Keep updated by copying source file:";
            this.keepUpdatedBox.CheckedChanged += new System.EventHandler(this.keepUpdatedBox_CheckedChanged);
            // 
            // bitmapLinkageBox
            // 
            this.bitmapLinkageBox.AutoSize = true;
            this.bitmapLinkageBox.Location = new System.Drawing.Point(16, 125);
            this.bitmapLinkageBox.Name = "bitmapLinkageBox";
            this.bitmapLinkageBox.Size = new System.Drawing.Size(174, 17);
            this.bitmapLinkageBox.TabIndex = 5;
            this.bitmapLinkageBox.Text = "Embed as Bitmap instead of Clip";
            this.bitmapLinkageBox.UseVisualStyleBackColor = true;
            // 
            // LibraryAssetDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(324, 235);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LibraryAssetDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Library Asset Properties";
            this.tabControl.ResumeLayout(false);
            this.swfTabPage.ResumeLayout(false);
            this.swfTabPage.PerformLayout();
            this.swcTabPage.ResumeLayout(false);
            this.fontTabPage.ResumeLayout(false);
            this.fontTabPage.PerformLayout();
            this.advancedTabPage.ResumeLayout(false);
            this.advancedTabPage.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        public LibraryAssetDialog(LibraryAsset asset, Project project)
        {
            this.asset = asset;
            this.Text = "\"" + System.IO.Path.GetFileName(asset.Path) + "\"";
            InitializeComponent();
            isAS3 = (project.Language == "as3");
            isSWC = asset.IsSwc 
                || asset.SwfMode == SwfAssetMode.Library 
                || asset.SwfMode == SwfAssetMode.IncludedLibrary 
                || asset.SwfMode == SwfAssetMode.ExternalLibrary;

            #region Setup Tabs

            if (isSWC)
            {
                tabControl.TabPages.Remove(fontTabPage);
                tabControl.TabPages.Remove(advancedTabPage);
                switch (asset.SwfMode)
                {
                    case SwfAssetMode.Library: swcLibOption.Checked = true; break;
                    case SwfAssetMode.IncludedLibrary: swcIncOption.Checked = true; break;
                    case SwfAssetMode.ExternalLibrary: swcExtOption.Checked = true; break;
                }
            }
            else tabControl.TabPages.Remove(swcTabPage);

            if (asset.IsImage || asset.IsSound)
            {
                tabControl.TabPages.Remove(swfTabPage);
                tabControl.TabPages.Remove(fontTabPage);
            }
            else if (asset.IsFont)
            {
                tabControl.TabPages.Remove(swfTabPage);
            }
            else if (isSWC)
            {
                tabControl.TabPages.Remove(fontTabPage);
            }
            
            if (isAS3) tabControl.TabPages.Remove(swfTabPage);

            #endregion

            #region Setup Form

            autoIDBox.Checked = asset.ManualID == null;
            idTextBox.Text = asset.ID;
            keepUpdatedBox.Checked = (asset.UpdatePath != null);
            updatedTextBox.Text = asset.UpdatePath;
            addLibraryButton.Checked = (asset.SwfMode == SwfAssetMode.Library);
            addPreloaderButton.Checked = (asset.SwfMode == SwfAssetMode.Preloader);
            sharedLibraryButton.Checked = (asset.SwfMode == SwfAssetMode.Shared);
            specifySharepointBox.Checked = asset.Sharepoint != null;
            sharepointTextBox.Text = asset.Sharepoint;
            embedAllButton.Checked = (asset.FontGlyphs == null);
            embedTheseButton.Checked = (asset.FontGlyphs != null);
            charactersTextBox.Text = asset.FontGlyphs;
            bitmapLinkageBox.Checked = asset.BitmapLinkage;

            #endregion

            EnableDisable();
            InitializeLocalization();
            this.Font = PluginCore.PluginBase.Settings.DefaultFont;
        }

        private void InitializeLocalization()
        {
            this.okButton.Text = TextHelper.GetString("Label.OK");
            this.cancelButton.Text = TextHelper.GetString("Label.Cancel");
            this.fontTabPage.Text = TextHelper.GetString("Label.Font");
            this.swfTabPage.Text = TextHelper.GetString("Label.SwfFile");
            this.browseButton.Text = TextHelper.GetString("Label.Browse");
            this.advancedTabPage.Text = TextHelper.GetString("Label.Advanced");
            this.autoIDBox.Text = TextHelper.GetString("Label.AutoGenerateID");
            this.addPreloaderButton.Text = TextHelper.GetString("Label.AddAsPreloader");
            this.explainLink.Text = TextHelper.GetString("Label.ExplainTheseOptions");
            this.addLibraryButton.Text = TextHelper.GetString("Info.AddToLibrary");
            this.specifySharepointBox.Text = TextHelper.GetString("Label.SpecifySharePointID");
            this.sharedLibraryButton.Text = TextHelper.GetString("Label.LoadAtRuntime");
            this.embedTheseButton.Text = TextHelper.GetString("Label.EmbedTheseChars");
            this.embedAllButton.Text = TextHelper.GetString("Label.EmbedAllChars");
            this.keepUpdatedBox.Text = TextHelper.GetString("Label.KeepUpdatedBySourceFile");
            this.bitmapLinkageBox.Text = TextHelper.GetString("Label.EmbedAsBitmap");
            this.swcLibOption.Text = TextHelper.GetString("Label.SwcLibraryOption");
            this.swcIncOption.Text = TextHelper.GetString("Label.SwcIncludedLibraryOption");
            this.swcExtOption.Text = TextHelper.GetString("Label.SwcExternalLibraryOption");
            this.Text = " " + TextHelper.GetString("Title.LibraryAssetProperties");
        }

        private void EnableDisable()
        {
            autoIDBox.Visible = !isAS3 && (asset.IsSwf || addLibraryButton.Checked);
            idTextBox.Visible = !isAS3 && (!asset.IsSwf || addLibraryButton.Checked);
            bitmapLinkageBox.Visible = !isAS3 && (asset.IsImage || asset.BitmapLinkage);

            explainLink.Enabled = false;
            idTextBox.Enabled = !autoIDBox.Checked;
            updatedTextBox.Enabled = keepUpdatedBox.Checked;
            browseButton.Enabled = keepUpdatedBox.Checked;
            specifySharepointBox.Enabled = sharedLibraryButton.Checked;
            sharepointTextBox.Enabled = sharedLibraryButton.Checked && specifySharepointBox.Checked;
            charactersTextBox.Enabled = embedTheseButton.Checked;
        }

        private void Apply()
        {
            asset.ManualID = (autoIDBox.Checked) ? null : idTextBox.Text;
            asset.UpdatePath = (keepUpdatedBox.Checked) ? updatedTextBox.Text : null;
            asset.BitmapLinkage = bitmapLinkageBox.Checked;

            if (addLibraryButton.Checked)
                asset.SwfMode = SwfAssetMode.Library;
            else if (addPreloaderButton.Checked)
                asset.SwfMode = SwfAssetMode.Preloader;
            else if (sharedLibraryButton.Checked)
                asset.SwfMode = SwfAssetMode.Shared;

            if (asset.SwfMode == SwfAssetMode.Shared && specifySharepointBox.Checked)
                asset.Sharepoint = sharepointTextBox.Text;
            else
                asset.Sharepoint = null;

            asset.FontGlyphs = (embedAllButton.Checked) ? null : charactersTextBox.Text;
        }

        private void Modified()
        {
            modified = true;
            EnableDisable();
        }

        private void okButton_Click(object sender, System.EventArgs e)
        {
            if (modified && !isSWC) Apply();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void explainLink_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {

        }

        #region Various Change Events

        private void addLibraryButton_CheckedChanged(object sender, System.EventArgs e) { Modified(); }
        private void addPreloaderButton_CheckedChanged(object sender, System.EventArgs e) { Modified(); }
        private void sharedLibraryButton_CheckedChanged(object sender, System.EventArgs e) { Modified(); }
        private void sharepointTextBox_TextChanged(object sender, System.EventArgs e) { Modified(); }
        private void embedAllButton_CheckedChanged(object sender, System.EventArgs e) { Modified(); }
        private void embedTheseButton_CheckedChanged(object sender, System.EventArgs e) { Modified(); }
        private void charactersTextBox_TextChanged(object sender, System.EventArgs e) { Modified(); }
        private void idTextBox_TextChanged(object sender, System.EventArgs e) { Modified(); }
        private void updatedTextBox_TextChanged(object sender, System.EventArgs e) { Modified(); }

        #endregion

        private void specifySharepointBox_CheckedChanged(object sender, System.EventArgs e)
        {
            Modified();
            if (specifySharepointBox.Checked)
                sharepointTextBox.Focus();
        }

        private void keepUpdatedBox_CheckedChanged(object sender, System.EventArgs e)
        {
            Modified();
            if (keepUpdatedBox.Checked)
                browseButton.Focus();
        }

        private void autoIDBox_CheckedChanged(object sender, System.EventArgs e)
        {
            if (autoIDBox.Checked)
                idTextBox.Text = asset.GetAutoID();
            else
                idTextBox.Text = (asset.ManualID == null) ? asset.GetAutoID() : asset.ManualID;

            Modified();

            if (!autoIDBox.Checked)
                idTextBox.Focus();
        }

        private void browseButton_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = TextHelper.GetString("Info.FileFilter");

            // try pre-setting the current update path
            try
            {
                if (asset.UpdatePath.Length > 0)
                    dialog.FileName = asset.Project.GetAbsolutePath(asset.UpdatePath);
                else
                    dialog.FileName = asset.Project.GetAbsolutePath(asset.Path);
            }
            catch { }

            if (dialog.ShowDialog(this) == DialogResult.OK)
                updatedTextBox.Text = asset.Project.GetRelativePath(dialog.FileName);
        }

        private void swcLibOption_CheckedChanged(object sender, EventArgs e)
        {
            modified = true;
            if (swcIncOption.Checked) asset.SwfMode = SwfAssetMode.IncludedLibrary;
            else if (swcExtOption.Checked) asset.SwfMode = SwfAssetMode.ExternalLibrary;
            else asset.SwfMode = SwfAssetMode.Library;
        }
    }
}
