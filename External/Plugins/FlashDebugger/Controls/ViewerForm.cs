using System;
using System.Windows.Forms;
using PluginCore.Localization;

namespace FlashDebugger.Controls
{
    public partial class ViewerForm : Form
    {
        public string Exp
        {
            get { return ExptextBox.Text; }
            set { ExptextBox.Text = value; }
        }

        public string Value
        {
            get { return ValuetextBox.Text; }
            set { ValuetextBox.Text = value; }
        }

        public ViewerForm()
        {
            InitializeComponent();
            InitializeLocalization();
        }

        private void InitializeLocalization()
        {
            this.label1.Text = TextHelper.GetString("Label.Exp");
            this.label2.Text = TextHelper.GetString("Label.Value");
            this.Closebutton.Text = TextHelper.GetString("Label.Close");
            this.CopyAllbutton.Text = TextHelper.GetString("Label.CopyAll");
            this.WordWrapcheckBox.Text = TextHelper.GetString("Label.WordWrap");
            this.CopyValuebutton.Text = TextHelper.GetString("Label.CopyValue");
            this.CopyExpbutton.Text = TextHelper.GetString("Label.CopyExp");
            this.Text = " " + TextHelper.GetStringWithoutMnemonics("Label.Viewer");
        }

        private void ViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
        }

        private void Closebutton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void WordWrapcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ValuetextBox.WordWrap = WordWrapcheckBox.Checked;
        }

        private void CopyExpbutton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ExptextBox.Text);
        }

        private void CopyValuebutton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ValuetextBox.Text);
        }

        private void CopyAllbutton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(string.Format("{0} = {1}", ExptextBox.Text, ValuetextBox.Text));
        }

    }

}
