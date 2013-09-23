using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CodeRefactor.Controls
{
    public partial class RenameFileDialog : Form
    {
        private readonly Regex Regex = new Regex(@"^[A-Z]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private string _oldFullPath;
        private string _oldDirectoryName;
        private string _oldName;
        private string _ext;

        public RenameFileDialog()
        {
            InitializeComponent();
            OKButton.DialogResult = DialogResult.OK;
        }

        internal void setOldFullPath(string path)
        {
            _oldFullPath = path;
            _oldDirectoryName = Path.GetDirectoryName(path);
            _oldName = Path.GetFileNameWithoutExtension(path);
            _ext = Path.GetExtension(path);

            NewName.TextChanged += onNewNameChanged;
            onNewNameChanged();
        }

        private void onNewNameChanged(object sender = null, EventArgs e = null)
        {
            string newName = NewName.Text;
            string newFullPath = Path.Combine(_oldDirectoryName, string.Concat(newName, _ext));
            bool canRename = true;

            if(string.IsNullOrEmpty(newName) || newName == _oldName){
                WarningLabel.Text = "Enter a new name.";
                canRename = false;
            }
            else if(!Regex.IsMatch(newName))
            {
                WarningLabel.Text = "New name is not a valid indentifier.";
                canRename = false;
            }
            else if (File.Exists(newFullPath))
            {
                WarningLabel.Text = String.Format("Name {0} is already taken", _oldName);
                canRename = false;
            }
            else
                WarningLabel.ResetText();

            OKButton.Enabled = canRename;
        }

    }
}