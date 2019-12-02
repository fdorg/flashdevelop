// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        private Label labelStatus;
        private Button closeButton;
        private ProgressBarEx progressBar;

        public ProgressDialog()
        {
            Owner = (Form)PluginBase.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            InitializeComponent();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            progressBar = new ProgressBarEx();
            closeButton = new ButtonEx();
            labelStatus = new Label();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right;
            progressBar.Location = new System.Drawing.Point(12, 23);
            progressBar.Name = "progressBar";
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
            labelStatus.Name = "labelStatus";
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

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            UpdateProgress(0);
            UpdateStatusMessage(string.Empty);
        }

        /// <summary>
        /// Runner reports how much of the lookup is done
        /// </summary>
        public void UpdateProgress(int percentDone)
        {
            progressBar.Value = percentDone;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateStatusMessage(string message)
        {
            labelStatus.Text = TextHelper.GetString("Info.Status") + " " + message;
            labelStatus.Update();
        }

        /// <summary>
        /// Just hides the dialog window when closing
        /// </summary>
        private void DialogClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            PluginBase.MainForm.CurrentDocument.Activate();
            Hide();
        }

        /// <summary>
        /// Some event handling when showing the form
        /// </summary>
        private void VisibleChange(object sender, EventArgs e)
        {
            if (Visible)
            {
                CenterToParent();
            }
        }

        /// <summary>
        /// Update the dialog args when show is called
        /// </summary>
        public new void Show()
        {
            base.Show();
            Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetTitle(string title)
        {
            Text = title;
        }

        #endregion

    }

}
