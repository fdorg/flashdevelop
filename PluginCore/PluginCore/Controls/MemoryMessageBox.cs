using System;
using System.Collections.Generic;
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

    public partial class MemoryMessageBox : SmartForm
    {
        readonly Dictionary<MemoryMessageBoxResult, Button> resultToButton;
        MemoryMessageBoxResult lastResult = MemoryMessageBoxResult.Cancel;
        public MemoryMessageBoxResult Result { get; set; } = MemoryMessageBoxResult.Cancel;

        public MemoryMessageBox()
        {
            Owner = (Form) PluginBase.MainForm;
            Font = PluginBase.MainForm.Settings.DefaultFont;
            InitializeComponent();
            ApplyLocalizedTexts();
            resultToButton = new Dictionary<MemoryMessageBoxResult, Button>
            {
                {MemoryMessageBoxResult.Yes, yesButton},
                {MemoryMessageBoxResult.YesToAll, yesToAllButton},
                {MemoryMessageBoxResult.No, noButton},
                {MemoryMessageBoxResult.NoToAll, noToAllButton},
                {MemoryMessageBoxResult.Cancel, cancelButton}
            };
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
            dontAsk.Text = TextHelper.GetString("Label.DontAskAgain");
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
            if (dontAsk.Checked)
            {
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
            }
            else base.ShowDialog();
            return Result;
        }

        protected override void OnShown(EventArgs e)
        {
            if (lastResult != MemoryMessageBoxResult.Cancel) resultToButton[lastResult].Select();
            else resultToButton[MemoryMessageBoxResult.Yes].Select();
            base.OnShown(e);
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
