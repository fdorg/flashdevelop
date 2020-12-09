using System;
using System.Windows.Forms;
using System.ComponentModel;
using PluginCore.Localization;
using PluginCore.Controls;
using PluginCore;

namespace CodeRefactor.Controls
{
    public class ProgressDialog : SmartForm
    {
        Label labelStatus;
        ProgressBarEx progressBar;

        public ProgressDialog()
        {
            Owner = (Form) PluginBase.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            InitializeComponent();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            progressBar = new ProgressBarEx();
            labelStatus = new Label();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right;
            progressBar.Location = new System.Drawing.Point(12, 23);
            progressBar.Name = nameof(progressBar);
            progressBar.Size = new System.Drawing.Size(491, 14);
            progressBar.TabIndex = 0;
            progressBar.UseWaitCursor = true;
            // 
            // labelStatus
            // 
            labelStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelStatus.AutoEllipsis = true;
            labelStatus.AutoSize = true;
            labelStatus.BackColor = System.Drawing.SystemColors.Control;
            labelStatus.FlatStyle = FlatStyle.System;
            labelStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            labelStatus.Location = new System.Drawing.Point(12, 7);
            labelStatus.Name = nameof(labelStatus);
            labelStatus.Size = new System.Drawing.Size(43, 13);
            labelStatus.TabIndex = 0;
            labelStatus.Text = "Status: ";
            labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelStatus.UseWaitCursor = true;
            // 
            // FindingReferencesDialogue
            //
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(515, 49);
            Controls.Add(labelStatus);
            Controls.Add(progressBar);
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Finding References...";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            VisibleChanged += VisibleChange;
            Closing += DialogClosing;
            Name = "FindingReferencesDialog";
            UseWaitCursor = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        #region Methods And Event Handlers

        public void Reset()
        {
            UpdateProgress(0);
            UpdateStatusMessage(string.Empty);
        }

        /// <summary>
        /// Runner reports how much of the lookup is done
        /// </summary>
        public void UpdateProgress(int percentDone) => progressBar.Value = percentDone;

        public void UpdateStatusMessage(string message)
        {
            labelStatus.Text = TextHelper.GetString("Info.Status") + " " + message;
            labelStatus.Update();
        }

        /// <summary>
        /// Just hides the dialog window when closing
        /// </summary>
        void DialogClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            PluginBase.MainForm.CurrentDocument?.Activate();
            Hide();
        }

        /// <summary>
        /// Some event handling when showing the form
        /// </summary>
        void VisibleChange(object sender, EventArgs e)
        {
            if (Visible) CenterToParent();
        }

        /// <summary>
        /// Update the dialog args when show is called
        /// </summary>
        public new void Show()
        {
            base.Show();
            Reset();
        }

        public void SetTitle(string title) => Text = title;

        #endregion
    }
}