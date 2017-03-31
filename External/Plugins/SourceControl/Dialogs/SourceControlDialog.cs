using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SourceControl.Dialogs
{
    public partial class SourceControlDialog : Form
    {
        public bool Remember
        {
            get
            {
                return checkRemember.Checked;
            }
        }

        public SourceControlDialog(string title, string message)
        {
            InitializeComponent();
            this.Font = PluginCore.PluginBase.Settings.DefaultFont;
            this.Text = title;
            this.lblMessage.Text = message;
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
