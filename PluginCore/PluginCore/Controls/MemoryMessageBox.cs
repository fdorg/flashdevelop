using System;
using System.Windows.Forms;
using PluginCore.Controls;
using PluginCore.Localization;

namespace PluginCore.PluginCore.Controls
{
    public enum MemoryMessageBoxResult
    {
        Yes,
        YesToAll,
        No,
        NoToAll,
        Cancel
    }

    public partial class MemoryMessageBox : /*Smart*/Form
    {
        MemoryMessageBoxResult lastResult = MemoryMessageBoxResult.Cancel;
        public MemoryMessageBoxResult Result { get; set; } = MemoryMessageBoxResult.Cancel;

        public MemoryMessageBox()
        {
            Owner = (Form) PluginBase.MainForm;
            Font = PluginBase.MainForm.Settings.DefaultFont;
            InitializeComponent();
            ApplyLocalizedTexts();
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        private void ApplyLocalizedTexts()
        {
            yesButton.Text = TextHelper.GetString("Label.Yes");
            yesToAllButton.Text = TextHelper.GetString("Label.YesToAll");
            noButton.Text = TextHelper.GetString("Label.No");
            noToAllButton.Text = TextHelper.GetString("Label.NoToAll");
            cancelButton.Text = TextHelper.GetString("Label.Cancel");
        }

        public MemoryMessageBoxResult ShowDialog(string text, string caption)
        {
            label.Text = text;
            Text = caption;
            return ShowDialog();
        }

        public new MemoryMessageBoxResult ShowDialog()
        {
            Result = MemoryMessageBoxResult.Cancel;
            switch (lastResult)
            {
                case MemoryMessageBoxResult.NoToAll:
                    Result = MemoryMessageBoxResult.No;
                    break;
                case MemoryMessageBoxResult.YesToAll:
                    Result = MemoryMessageBoxResult.Yes;
                    break;
                default:
                    base.ShowDialog();
                    break;
            }
            return Result;
        }

        private void OnYesButtonClick(object sender, EventArgs e)
        {
            Result = MemoryMessageBoxResult.Yes;
            lastResult = MemoryMessageBoxResult.Yes;
            DialogResult = DialogResult.Yes;
        }

        private void OnYesToAllButtonClick(object sender, EventArgs e)
        {
            Result = MemoryMessageBoxResult.Yes;
            lastResult = MemoryMessageBoxResult.YesToAll;
            DialogResult = DialogResult.Yes;
        }

        private void OnNoButtonClick(object sender, EventArgs e)
        {
            Result = MemoryMessageBoxResult.No;
            lastResult = MemoryMessageBoxResult.No;
            DialogResult = DialogResult.No;
        }

        private void OnNoToAllButtonClick(object sender, EventArgs e)
        {
            Result = MemoryMessageBoxResult.No;
            lastResult = MemoryMessageBoxResult.NoToAll;
            DialogResult = DialogResult.No;
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            Result = MemoryMessageBoxResult.Cancel;
            lastResult = MemoryMessageBoxResult.Cancel;
            DialogResult = DialogResult.Cancel;
        }
    }
}
