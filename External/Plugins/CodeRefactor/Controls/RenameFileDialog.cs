using PluginCore;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CodeRefactor.Controls
{
    public partial class RenameFileDialog : Form
    {
        static readonly Regex re_validFirstChar = new Regex(@"^[A-Z]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private string _oldFullPath;
        private string _oldDirectoryName;
        private string _oldName;
        private string _ext;

        public RenameFileDialog()
        {
            InitializeComponent();
            OKButton.DialogResult = DialogResult.OK;
        }

        internal void ShowDialogFor(string path)
        {
            if (String.IsNullOrEmpty(path))
                Close();
            else
            {
                SetOldFullPath(path);
                ShowDialog();
            }
        }

        private void SetOldFullPath(string path)
        {
            _oldFullPath = path;
            _oldDirectoryName = Path.GetDirectoryName(path);
            _oldName = Path.GetFileNameWithoutExtension(path);
            _ext = Path.GetExtension(path);

            NewName.TextChanged += OnNewNameChanged;
            NewName.Text = _oldName;
            NewName.SelectAll();
        }

        private void OnNewNameChanged(object sender, EventArgs e)
        {
            string newName = NewName.Text;
            string newFileName = string.Concat(newName, _ext);
            string newFullPath = Path.Combine(_oldDirectoryName, newFileName);
            bool canRename = false;
            bool withImage = false;

            WarningLabel.AutoSize = true;

            if(string.IsNullOrEmpty(newName) || newName == _oldName)
                WarningLabel.Text = "Enter a new name.";
            else if(!re_validFirstChar.IsMatch(newName))
            {
                WarningLabel.Text = "New name is not a valid indentifier.";
                withImage = true;
            }
            else if (File.Exists(newFullPath))
            {
                WarningLabel.Text = String.Format("Name {0} is already taken", newFileName);
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