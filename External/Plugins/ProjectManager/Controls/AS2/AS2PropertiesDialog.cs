using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectManager.Projects.AS2;
using PluginCore.Localization;
using PluginCore.Managers;
using System.IO;

namespace ProjectManager.Controls.AS2
{
    public partial class AS2PropertiesDialog : PropertiesDialog
    {
        // For designer
        public AS2PropertiesDialog() 
        { 
            InitializeComponent();
            InitializeLocalization();
        }

        AS2Project project { get { return (AS2Project)BaseProject; } }

        protected override void BuildDisplay()
        {
            base.BuildDisplay();
            injectionCheckBox.Checked = project.UsesInjection;
            inputSwfBox.Text = project.InputPath;
            AssetsChanged = false;
        }

        private void InitializeLocalization()
        {
            this.infoLabel.Text = TextHelper.GetString("Info.CodeInjection");
            this.injectionTab.Text = TextHelper.GetString("Info.Injection");
            this.injectionCheckBox.Text = TextHelper.GetString("Info.UseCodeInjection");
            this.inputBrowseButton.Text = TextHelper.GetString("Label.Browse");
            this.inputFileLabel.Text = TextHelper.GetString("Info.InputSWF");
        }

        private void inputSwfBox_TextChanged(object sender, System.EventArgs e)
        {
            ClasspathsChanged = true;
            Modified();
        }

        private void injectionCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            if (injectionCheckBox.Checked && project.LibraryAssets.Count > 0)
            {
                string msg = TextHelper.GetString("Info.InjectionConfirmation");
                string title = " " + TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");

                DialogResult result = MessageBox.Show(this, msg, title,
                    MessageBoxButtons.OKCancel);

                if (result == DialogResult.Cancel)
                {
                    injectionCheckBox.Checked = false;
                    return;
                }
            }

            Modified();
            bool inject = injectionCheckBox.Checked;
            inputSwfBox.Enabled = inject;
            inputBrowseButton.Enabled = inject;
            widthTextBox.Enabled = !inject;
            heightTextBox.Enabled = !inject;
            colorTextBox.Enabled = !inject;
            fpsTextBox.Enabled = !inject;
        }

        private void inputBrowseButton_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = TextHelper.GetString("Info.FlashMovieFilter");
            dialog.InitialDirectory = project.Directory;

            // try pre-setting the current input path
            try
            {
                string path = project.GetAbsolutePath(inputSwfBox.Text);
                if (File.Exists(path)) dialog.FileName = path;
            }
            catch { }

            if (dialog.ShowDialog(this) == DialogResult.OK)
                inputSwfBox.Text = project.GetRelativePath(dialog.FileName);
        }

        protected override bool Apply()
        {
            if (injectionCheckBox.Checked && inputSwfBox.Text.Length == 0)
            {
                string msg = TextHelper.GetString("Info.SpecifyInputSwfForInjection");
                ErrorManager.ShowInfo(msg);
            }
            else if (injectionCheckBox.Checked)
            {
                project.InputPath = inputSwfBox.Text;

                // unassign any existing assets - you've been warned already
                if (project.LibraryAssets.Count > 0)
                {
                    project.LibraryAssets.Clear();
                    AssetsChanged = true;
                }
            }
            else
                project.InputPath = "";

            return base.Apply();
        }
    }
}

