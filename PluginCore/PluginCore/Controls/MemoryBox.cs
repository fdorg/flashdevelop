using System;
using System.Windows.Forms;
using PluginCore.Controls;
using PluginCore.Localization;

namespace PluginCore.PluginCore.Controls
{
    public enum MemoryBoxResult
    {
        Yes,
        YesToAll,
        No,
        NoToAll,
        Cancel
    }

    public partial class MemoryBox : SmartForm
    {
        MemoryBoxResult lastResult = MemoryBoxResult.Cancel;
        public MemoryBoxResult Result { get; set; } = MemoryBoxResult.Cancel;

        public MemoryBox()
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

        public MemoryBoxResult ShowDialog(string text, string caption)
        {
            label.Text = text;
            Text = caption;
            return ShowDialog();
        }

        public new MemoryBoxResult ShowDialog()
        {
            Result = MemoryBoxResult.Cancel;
            switch (lastResult)
            {
                case MemoryBoxResult.NoToAll:
                    Result = MemoryBoxResult.No;
                    break;
                case MemoryBoxResult.YesToAll:
                    Result = MemoryBoxResult.Yes;
                    break;
                default:
                    base.ShowDialog();
                    break;
            }
            return Result;
        }

        private void OnYesButtonClick(object sender, EventArgs e)
        {
            Result = MemoryBoxResult.Yes;
            lastResult = MemoryBoxResult.Yes;
            DialogResult = DialogResult.Yes;
        }

        private void OnYesToAllButtonClick(object sender, EventArgs e)
        {
            Result = MemoryBoxResult.Yes;
            lastResult = MemoryBoxResult.YesToAll;
            DialogResult = DialogResult.Yes;
        }

        private void OnNoButtonClick(object sender, EventArgs e)
        {
            Result = MemoryBoxResult.No;
            lastResult = MemoryBoxResult.No;
            DialogResult = DialogResult.No;
        }

        private void OnNoToAllButtonClick(object sender, EventArgs e)
        {
            Result = MemoryBoxResult.No;
            lastResult = MemoryBoxResult.NoToAll;
            DialogResult = DialogResult.No;
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            Result = MemoryBoxResult.Cancel;
            lastResult = MemoryBoxResult.Cancel;
            DialogResult = DialogResult.Cancel;
        }
    }
}
