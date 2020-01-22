// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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

        void InitializeLocalization()
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

        void ViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }

        void Closebutton_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        void WordWrapcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ValuetextBox.WordWrap = WordWrapcheckBox.Checked;
        }

        void CopyExpbutton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ExptextBox.Text);
        }

        void CopyValuebutton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ValuetextBox.Text);
        }

        void CopyAllbutton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText($"{ExptextBox.Text} = {ValuetextBox.Text}");
        }

    }

}
