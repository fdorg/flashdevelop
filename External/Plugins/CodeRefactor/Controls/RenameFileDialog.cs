using PluginCore;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CodeRefactor.Controls
{
    public partial class RenameFileDialog : Form
    {
        static readonly Regex re_validFirstChar = new Regex(@"^[A-Z_$]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private string oldFullPath;
        private string oldDirectoryName;
        private string oldName;
        private string ext;

        public RenameFileDialog()
        {
            InitializeComponent();
            OKButton.DialogResult = DialogResult.OK;
        }

        internal void ShowDialogFor(string path)
        {
            if (string.IsNullOrEmpty(path))
                Close();
            else
            {
                UpdateReferences.Checked = true;

                SetOldFullPath(path);
                ShowDialog();
                OnNewNameChanged();
            }
        }

        private void SetOldFullPath(string path)
        {
            oldFullPath = path;
            oldDirectoryName = Path.GetDirectoryName(path);
            oldName = Path.GetFileNameWithoutExtension(path);
            ext = Path.GetExtension(path);

            NewName.TextChanged += OnNewNameChanged;
            NewName.Text = oldName;
            NewName.SelectAll();
        }

        private void OnNewNameChanged(object sender = null, EventArgs e = null)
        {
            string newName = NewName.Text;
            string newFileName = string.Concat(newName, ext);
            string newFullPath = Path.Combine(oldDirectoryName, newFileName);
            bool canRename = false;
            bool withImage = false;

            WarningLabel.AutoSize = true;

            if (string.IsNullOrEmpty(newName) || newName == oldName)
            {
                //TODO: Localize
                WarningLabel.Text = "Enter a new name.";
            }
            else if (!re_validFirstChar.IsMatch(newName))
            {
                //TODO: Localize
                WarningLabel.Text = "New name is not a valid indentifier.";
                withImage = true;
            }
            else if (File.Exists(newFullPath))
            {
                //TODO: Localize
                WarningLabel.Text = string.Format("Name {0} is already taken", newFileName);
                withImage = true;
            }
            else
            {
                WarningLabel.ResetText();
                canRename = true;
            }

            if (withImage)
            {
                int width = WarningLabel.Width;
                WarningLabel.Image = PluginBase.MainForm.FindImage("197");
                WarningLabel.AutoSize = false;
                WarningLabel.Width = width + WarningLabel.Image.Width;
                WarningLabel.Height = WarningLabel.Image.Height;
            }
            else
                WarningLabel.Image = null;

            OKButton.Enabled = canRename;
        }

    }
}