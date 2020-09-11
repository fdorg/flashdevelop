using System;
using System.Windows.Forms;
using ProjectManager.Projects;
using PluginCore.Localization;

namespace ProjectManager.Controls.AS2
{
    public class LibraryAssetDialog : Form
    {
        readonly bool isAS3;
        readonly bool isSWC;
        bool modified;
        readonly LibraryAsset asset;

        #region Windows Form Designer

        TabControl tabControl;
        TabPage swfTabPage;
        RadioButton addPreloaderButton;
        LinkLabel explainLink;
        RadioButton sharedLibraryButton;
        RadioButton addLibraryButton;
        TabPage fontTabPage;
        TextBox charactersTextBox;
        RadioButton embedTheseButton;
        RadioButton embedAllButton;
        CheckBox autoIDBox;
        TextBox idTextBox;
        CheckBox keepUpdatedBox;
        TextBox updatedTextBox;
        Button cancelButton;
        Button okButton;
        Button browseButton;
        CheckBox specifySharepointBox;
        TextBox sharepointTextBox;
        TabPage swcTabPage;
        RadioButton swcIncOption;
        RadioButton swcExtOption;
        RadioButton swcLibOption;
        CheckBox bitmapLinkageBox;
        TabPage advancedTabPage;
        
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            cancelButton = new ButtonEx();
            okButton = new ButtonEx();
            autoIDBox = new CheckBoxEx();
            idTextBox = new TextBoxEx();
            tabControl = new TabControl();
            swfTabPage = new TabPage();
            specifySharepointBox = new CheckBoxEx();
            addPreloaderButton = new RadioButton();
            explainLink = new LinkLabel();
            sharepointTextBox = new TextBoxEx();
            sharedLibraryButton = new RadioButton();
            addLibraryButton = new RadioButton();
            swcTabPage = new TabPage();
            swcIncOption = new RadioButton();
            swcExtOption = new RadioButton();
            swcLibOption = new RadioButton();
            fontTabPage = new TabPage();
            charactersTextBox = new TextBoxEx();
            embedTheseButton = new RadioButton();
            embedAllButton = new RadioButton();
            advancedTabPage = new TabPage();
            browseButton = new ButtonEx();
            updatedTextBox = new TextBoxEx();
            keepUpdatedBox = new CheckBoxEx();
            bitmapLinkageBox = new CheckBoxEx();
            tabControl.SuspendLayout();
            swfTabPage.SuspendLayout();
            swcTabPage.SuspendLayout();
            fontTabPage.SuspendLayout();
            advancedTabPage.SuspendLayout();
            SuspendLayout();
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.FlatStyle = FlatStyle.System;
            cancelButton.Location = new System.Drawing.Point(239, 202);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 21);
            cancelButton.TabIndex = 2;
            cancelButton.Text = "&Cancel";
            cancelButton.Click += cancelButton_Click;
            // 
            // okButton
            // 
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.FlatStyle = FlatStyle.System;
            okButton.Location = new System.Drawing.Point(156, 202);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 21);
            okButton.TabIndex = 1;
            okButton.Text = "&OK";
            okButton.Click += okButton_Click;
            // 
            // autoIDBox
            // 
            autoIDBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            autoIDBox.Checked = true;
            autoIDBox.CheckState = CheckState.Checked;
            autoIDBox.Location = new System.Drawing.Point(16, 73);
            autoIDBox.Name = "autoIDBox";
            autoIDBox.Size = new System.Drawing.Size(266, 18);
            autoIDBox.TabIndex = 0;
            autoIDBox.Text = "Auto-generate &ID for attachMovie():";
            autoIDBox.CheckedChanged += autoIDBox_CheckedChanged;
            // 
            // idTextBox
            // 
            idTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            idTextBox.Enabled = false;
            idTextBox.Location = new System.Drawing.Point(34, 94);
            idTextBox.Name = "idTextBox";
            idTextBox.Size = new System.Drawing.Size(248, 20);
            idTextBox.TabIndex = 1;
            idTextBox.Text = "Library.WorkerGuy.png";
            idTextBox.TextChanged += idTextBox_TextChanged;
            // 
            // tabControl
            // 
            tabControl.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            tabControl.Controls.Add(swfTabPage);
            tabControl.Controls.Add(swcTabPage);
            tabControl.Controls.Add(fontTabPage);
            tabControl.Controls.Add(advancedTabPage);
            tabControl.Location = new System.Drawing.Point(10, 11);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new System.Drawing.Size(306, 185);
            tabControl.TabIndex = 0;
            // 
            // swfTabPage
            // 
            swfTabPage.Controls.Add(specifySharepointBox);
            swfTabPage.Controls.Add(addPreloaderButton);
            swfTabPage.Controls.Add(explainLink);
            swfTabPage.Controls.Add(sharepointTextBox);
            swfTabPage.Controls.Add(sharedLibraryButton);
            swfTabPage.Controls.Add(addLibraryButton);
            swfTabPage.Location = new System.Drawing.Point(4, 22);
            swfTabPage.Name = "swfTabPage";
            swfTabPage.Size = new System.Drawing.Size(298, 159);
            swfTabPage.TabIndex = 2;
            swfTabPage.Text = "SWF File";
            swfTabPage.UseVisualStyleBackColor = true;
            // 
            // specifySharepointBox
            // 
            specifySharepointBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            specifySharepointBox.BackColor = System.Drawing.Color.Transparent;
            specifySharepointBox.Location = new System.Drawing.Point(35, 81);
            specifySharepointBox.Name = "specifySharepointBox";
            specifySharepointBox.Size = new System.Drawing.Size(197, 18);
            specifySharepointBox.TabIndex = 3;
            specifySharepointBox.Text = "&Specify sharepoint ID:";
            specifySharepointBox.UseVisualStyleBackColor = false;
            specifySharepointBox.CheckedChanged += specifySharepointBox_CheckedChanged;
            // 
            // addPreloaderButton
            // 
            addPreloaderButton.FlatStyle = FlatStyle.System;
            addPreloaderButton.Location = new System.Drawing.Point(16, 36);
            addPreloaderButton.Name = "addPreloaderButton";
            addPreloaderButton.Size = new System.Drawing.Size(184, 16);
            addPreloaderButton.TabIndex = 1;
            addPreloaderButton.Text = " Add as &preloader";
            addPreloaderButton.CheckedChanged += addPreloaderButton_CheckedChanged;
            // 
            // explainLink
            // 
            explainLink.Location = new System.Drawing.Point(155, 8);
            explainLink.Name = "explainLink";
            explainLink.Size = new System.Drawing.Size(136, 16);
            explainLink.TabIndex = 5;
            explainLink.TabStop = true;
            explainLink.Text = "Explain these options";
            explainLink.TextAlign = System.Drawing.ContentAlignment.TopRight;
            explainLink.LinkClicked += explainLink_LinkClicked;
            // 
            // sharepointTextBox
            // 
            sharepointTextBox.Enabled = false;
            sharepointTextBox.Location = new System.Drawing.Point(35, 102);
            sharepointTextBox.Name = "sharepointTextBox";
            sharepointTextBox.Size = new System.Drawing.Size(197, 20);
            sharepointTextBox.TabIndex = 4;
            sharepointTextBox.TextChanged += sharepointTextBox_TextChanged;
            // 
            // sharedLibraryButton
            // 
            sharedLibraryButton.FlatStyle = FlatStyle.System;
            sharedLibraryButton.Location = new System.Drawing.Point(16, 57);
            sharedLibraryButton.Name = "sharedLibraryButton";
            sharedLibraryButton.Size = new System.Drawing.Size(224, 16);
            sharedLibraryButton.TabIndex = 2;
            sharedLibraryButton.Text = " Load at &runtime (shared library)";
            sharedLibraryButton.CheckedChanged += sharedLibraryButton_CheckedChanged;
            // 
            // addLibraryButton
            // 
            addLibraryButton.Checked = true;
            addLibraryButton.FlatStyle = FlatStyle.System;
            addLibraryButton.Location = new System.Drawing.Point(16, 16);
            addLibraryButton.Name = "addLibraryButton";
            addLibraryButton.Size = new System.Drawing.Size(112, 16);
            addLibraryButton.TabIndex = 0;
            addLibraryButton.TabStop = true;
            addLibraryButton.Text = " Add to &library";
            addLibraryButton.CheckedChanged += addLibraryButton_CheckedChanged;
            // 
            // swcTabPage
            // 
            swcTabPage.Controls.Add(swcIncOption);
            swcTabPage.Controls.Add(swcExtOption);
            swcTabPage.Controls.Add(swcLibOption);
            swcTabPage.Location = new System.Drawing.Point(4, 22);
            swcTabPage.Name = "swcTabPage";
            swcTabPage.Padding = new Padding(3);
            swcTabPage.Size = new System.Drawing.Size(298, 159);
            swcTabPage.TabIndex = 3;
            swcTabPage.Text = "SWC File";
            swcTabPage.UseVisualStyleBackColor = true;
            // 
            // swcIncOption
            // 
            swcIncOption.FlatStyle = FlatStyle.System;
            swcIncOption.Location = new System.Drawing.Point(16, 36);
            swcIncOption.Name = "swcIncOption";
            swcIncOption.Size = new System.Drawing.Size(250, 16);
            swcIncOption.TabIndex = 4;
            swcIncOption.Text = "&Included library (include completely)";
            swcIncOption.CheckedChanged += swcLibOption_CheckedChanged;
            // 
            // swcExtOption
            // 
            swcExtOption.FlatStyle = FlatStyle.System;
            swcExtOption.Location = new System.Drawing.Point(16, 57);
            swcExtOption.Name = "swcExtOption";
            swcExtOption.Size = new System.Drawing.Size(250, 16);
            swcExtOption.TabIndex = 5;
            swcExtOption.Text = "&External library (not included)";
            swcExtOption.CheckedChanged += swcLibOption_CheckedChanged;
            // 
            // swcLibOption
            // 
            swcLibOption.Checked = true;
            swcLibOption.FlatStyle = FlatStyle.System;
            swcLibOption.Location = new System.Drawing.Point(16, 16);
            swcLibOption.Name = "swcLibOption";
            swcLibOption.Size = new System.Drawing.Size(250, 16);
            swcLibOption.TabIndex = 3;
            swcLibOption.TabStop = true;
            swcLibOption.Text = "&Library (include referenced classes)";
            swcLibOption.CheckedChanged += swcLibOption_CheckedChanged;
            // 
            // fontTabPage
            // 
            fontTabPage.Controls.Add(charactersTextBox);
            fontTabPage.Controls.Add(embedTheseButton);
            fontTabPage.Controls.Add(embedAllButton);
            fontTabPage.Location = new System.Drawing.Point(4, 22);
            fontTabPage.Name = "fontTabPage";
            fontTabPage.Size = new System.Drawing.Size(298, 159);
            fontTabPage.TabIndex = 1;
            fontTabPage.Text = "Font";
            fontTabPage.UseVisualStyleBackColor = true;
            // 
            // charactersTextBox
            // 
            charactersTextBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            charactersTextBox.Location = new System.Drawing.Point(35, 64);
            charactersTextBox.Multiline = true;
            charactersTextBox.Name = "charactersTextBox";
            charactersTextBox.Size = new System.Drawing.Size(247, 79);
            charactersTextBox.TabIndex = 2;
            charactersTextBox.TextChanged += charactersTextBox_TextChanged;
            // 
            // embedTheseButton
            // 
            embedTheseButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            embedTheseButton.FlatStyle = FlatStyle.System;
            embedTheseButton.Location = new System.Drawing.Point(16, 36);
            embedTheseButton.Name = "embedTheseButton";
            embedTheseButton.Size = new System.Drawing.Size(266, 16);
            embedTheseButton.TabIndex = 1;
            embedTheseButton.Text = " Embed &these characters:";
            embedTheseButton.CheckedChanged += embedTheseButton_CheckedChanged;
            // 
            // embedAllButton
            // 
            embedAllButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            embedAllButton.Checked = true;
            embedAllButton.FlatStyle = FlatStyle.System;
            embedAllButton.Location = new System.Drawing.Point(16, 16);
            embedAllButton.Name = "embedAllButton";
            embedAllButton.Size = new System.Drawing.Size(258, 16);
            embedAllButton.TabIndex = 0;
            embedAllButton.TabStop = true;
            embedAllButton.Text = " Embed &all characters";
            embedAllButton.CheckedChanged += embedAllButton_CheckedChanged;
            // 
            // advancedTabPage
            // 
            advancedTabPage.Controls.Add(bitmapLinkageBox);
            advancedTabPage.Controls.Add(browseButton);
            advancedTabPage.Controls.Add(updatedTextBox);
            advancedTabPage.Controls.Add(keepUpdatedBox);
            advancedTabPage.Controls.Add(autoIDBox);
            advancedTabPage.Controls.Add(idTextBox);
            advancedTabPage.Location = new System.Drawing.Point(4, 22);
            advancedTabPage.Name = "advancedTabPage";
            advancedTabPage.Size = new System.Drawing.Size(298, 159);
            advancedTabPage.TabIndex = 0;
            advancedTabPage.Text = "Advanced";
            advancedTabPage.UseVisualStyleBackColor = true;
            // 
            // browseButton
            // 
            browseButton.FlatStyle = FlatStyle.System;
            browseButton.Location = new System.Drawing.Point(210, 35);
            browseButton.Name = "browseButton";
            browseButton.Size = new System.Drawing.Size(72, 21);
            browseButton.TabIndex = 4;
            browseButton.Text = "&Browse...";
            browseButton.Click += browseButton_Click;
            // 
            // updatedTextBox
            // 
            updatedTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            updatedTextBox.Enabled = false;
            updatedTextBox.Location = new System.Drawing.Point(34, 37);
            updatedTextBox.Name = "updatedTextBox";
            updatedTextBox.Size = new System.Drawing.Size(170, 20);
            updatedTextBox.TabIndex = 3;
            updatedTextBox.TextChanged += updatedTextBox_TextChanged;
            // 
            // keepUpdatedBox
            // 
            keepUpdatedBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            keepUpdatedBox.Location = new System.Drawing.Point(16, 16);
            keepUpdatedBox.Name = "keepUpdatedBox";
            keepUpdatedBox.Size = new System.Drawing.Size(266, 18);
            keepUpdatedBox.TabIndex = 2;
            keepUpdatedBox.Text = "&Keep updated by copying source file:";
            keepUpdatedBox.CheckedChanged += keepUpdatedBox_CheckedChanged;
            // 
            // bitmapLinkageBox
            // 
            bitmapLinkageBox.AutoSize = true;
            bitmapLinkageBox.Location = new System.Drawing.Point(16, 125);
            bitmapLinkageBox.Name = "bitmapLinkageBox";
            bitmapLinkageBox.Size = new System.Drawing.Size(174, 17);
            bitmapLinkageBox.TabIndex = 5;
            bitmapLinkageBox.Text = "Embed as Bitmap instead of Clip";
            bitmapLinkageBox.UseVisualStyleBackColor = true;
            // 
            // LibraryAssetDialog
            // 
            AcceptButton = okButton;
            AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            CancelButton = cancelButton;
            ClientSize = new System.Drawing.Size(324, 235);
            Controls.Add(tabControl);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LibraryAssetDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Library Asset Properties";
            tabControl.ResumeLayout(false);
            swfTabPage.ResumeLayout(false);
            swfTabPage.PerformLayout();
            swcTabPage.ResumeLayout(false);
            fontTabPage.ResumeLayout(false);
            fontTabPage.PerformLayout();
            advancedTabPage.ResumeLayout(false);
            advancedTabPage.PerformLayout();
            ResumeLayout(false);

        }
        #endregion

        public LibraryAssetDialog(LibraryAsset asset, Project project)
        {
            this.asset = asset;
            Text = "\"" + System.IO.Path.GetFileName(asset.Path) + "\"";
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

            autoIDBox.Checked = asset.ManualID is null;
            idTextBox.Text = asset.ID;
            keepUpdatedBox.Checked = (asset.UpdatePath != null);
            updatedTextBox.Text = asset.UpdatePath;
            addLibraryButton.Checked = (asset.SwfMode == SwfAssetMode.Library);
            addPreloaderButton.Checked = (asset.SwfMode == SwfAssetMode.Preloader);
            sharedLibraryButton.Checked = (asset.SwfMode == SwfAssetMode.Shared);
            specifySharepointBox.Checked = asset.Sharepoint != null;
            sharepointTextBox.Text = asset.Sharepoint;
            embedAllButton.Checked = (asset.FontGlyphs is null);
            embedTheseButton.Checked = (asset.FontGlyphs != null);
            charactersTextBox.Text = asset.FontGlyphs;
            bitmapLinkageBox.Checked = asset.BitmapLinkage;

            #endregion

            EnableDisable();
            InitializeLocalization();
            Font = PluginCore.PluginBase.Settings.DefaultFont;
        }

        void InitializeLocalization()
        {
            okButton.Text = TextHelper.GetString("Label.OK");
            cancelButton.Text = TextHelper.GetString("Label.Cancel");
            fontTabPage.Text = TextHelper.GetString("Label.Font");
            swfTabPage.Text = TextHelper.GetString("Label.SwfFile");
            browseButton.Text = TextHelper.GetString("Label.Browse");
            advancedTabPage.Text = TextHelper.GetString("Label.Advanced");
            autoIDBox.Text = TextHelper.GetString("Label.AutoGenerateID");
            addPreloaderButton.Text = TextHelper.GetString("Label.AddAsPreloader");
            explainLink.Text = TextHelper.GetString("Label.ExplainTheseOptions");
            addLibraryButton.Text = TextHelper.GetString("Info.AddToLibrary");
            specifySharepointBox.Text = TextHelper.GetString("Label.SpecifySharePointID");
            sharedLibraryButton.Text = TextHelper.GetString("Label.LoadAtRuntime");
            embedTheseButton.Text = TextHelper.GetString("Label.EmbedTheseChars");
            embedAllButton.Text = TextHelper.GetString("Label.EmbedAllChars");
            keepUpdatedBox.Text = TextHelper.GetString("Label.KeepUpdatedBySourceFile");
            bitmapLinkageBox.Text = TextHelper.GetString("Label.EmbedAsBitmap");
            swcLibOption.Text = TextHelper.GetString("Label.SwcLibraryOption");
            swcIncOption.Text = TextHelper.GetString("Label.SwcIncludedLibraryOption");
            swcExtOption.Text = TextHelper.GetString("Label.SwcExternalLibraryOption");
            Text = " " + TextHelper.GetString("Title.LibraryAssetProperties");
        }

        void EnableDisable()
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

        void Apply()
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

        void Modified()
        {
            modified = true;
            EnableDisable();
        }

        void okButton_Click(object sender, EventArgs e)
        {
            if (modified && !isSWC) Apply();
            DialogResult = DialogResult.OK;
            Close();
        }

        void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        void explainLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        #region Various Change Events

        void addLibraryButton_CheckedChanged(object sender, EventArgs e) { Modified(); }
        void addPreloaderButton_CheckedChanged(object sender, EventArgs e) { Modified(); }
        void sharedLibraryButton_CheckedChanged(object sender, EventArgs e) { Modified(); }
        void sharepointTextBox_TextChanged(object sender, EventArgs e) { Modified(); }
        void embedAllButton_CheckedChanged(object sender, EventArgs e) { Modified(); }
        void embedTheseButton_CheckedChanged(object sender, EventArgs e) { Modified(); }
        void charactersTextBox_TextChanged(object sender, EventArgs e) { Modified(); }
        void idTextBox_TextChanged(object sender, EventArgs e) { Modified(); }
        void updatedTextBox_TextChanged(object sender, EventArgs e) { Modified(); }

        #endregion

        void specifySharepointBox_CheckedChanged(object sender, EventArgs e)
        {
            Modified();
            if (specifySharepointBox.Checked)
                sharepointTextBox.Focus();
        }

        void keepUpdatedBox_CheckedChanged(object sender, EventArgs e)
        {
            Modified();
            if (keepUpdatedBox.Checked)
                browseButton.Focus();
        }

        void autoIDBox_CheckedChanged(object sender, EventArgs e)
        {
            if (autoIDBox.Checked) idTextBox.Text = asset.GetAutoID();
            else idTextBox.Text = asset.ManualID ?? asset.GetAutoID();

            Modified();

            if (!autoIDBox.Checked)
                idTextBox.Focus();
        }

        void browseButton_Click(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog();
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

        void swcLibOption_CheckedChanged(object sender, EventArgs e)
        {
            modified = true;
            if (swcIncOption.Checked) asset.SwfMode = SwfAssetMode.IncludedLibrary;
            else if (swcExtOption.Checked) asset.SwfMode = SwfAssetMode.ExternalLibrary;
            else asset.SwfMode = SwfAssetMode.Library;
        }
    }
}