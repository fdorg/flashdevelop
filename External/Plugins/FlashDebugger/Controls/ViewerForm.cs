using System;
using System.Windows.Forms;
using PluginCore.Localization;

namespace FlashDebugger.Controls
{
    public partial class ViewerForm : Form
    {
        public string Exp
        {
            get => ExptextBox.Text;
            set => ExptextBox.Text = value;
        }

        public string Value
        {
            get => ValuetextBox.Text;
            set => ValuetextBox.Text = value;
        }

        public ViewerForm()
        {
            InitializeComponent();
            InitializeLocalization();
        }

        private void InitializeLocalization()
        {
            label1.Text = TextHelper.GetString("Label.Exp");
            label2.Text = TextHelper.GetString("Label.Value");
            Closebutton.Text = TextHelper.GetString("Label.Close");
            CopyAllbutton.Text = TextHelper.GetString("Label.CopyAll");
            WordWrapcheckBox.Text = TextHelper.GetString("Label.WordWrap");
            CopyValuebutton.Text = TextHelper.GetString("Label.CopyValue");
            CopyExpbutton.Text = TextHelper.GetString("Label.CopyExp");
            Text = " " + TextHelper.GetStringWithoutMnemonics("Label.Viewer");
        }

        private void ViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }

        private void Closebutton_Click(object sender, EventArgs e)
        {
            Visible = false;
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
