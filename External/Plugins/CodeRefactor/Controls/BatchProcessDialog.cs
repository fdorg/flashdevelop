using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeRefactor.Managers;
using PluginCore.Localization;
using PluginCore.Controls;
using PluginCore;

namespace CodeRefactor.Controls
{
    public class BatchProcessDialog : SmartForm
    {
        private System.Windows.Forms.Label targetLabel;
        private System.Windows.Forms.Label operationLabel;
        private System.Windows.Forms.ComboBox operationComboBox;
        private System.Windows.Forms.ComboBox targetComboBox;
        private System.Windows.Forms.Button processButton;
        private System.Windows.Forms.Button cancelButton;

        public BatchProcessDialog()
        {
            this.Owner = (Form)PluginBase.MainForm;
            this.Font = PluginBase.Settings.DefaultFont;
            this.FormGuid = "8edcdafb-b859-410f-9d78-59c0002db62d";
            this.InitializeComponent();
            this.InitializeDefaults();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cancelButton = new System.Windows.Forms.ButtonEx();
            this.operationComboBox = new System.Windows.Forms.FlatCombo();
            this.operationLabel = new System.Windows.Forms.Label();
            this.targetLabel = new System.Windows.Forms.Label();
            this.processButton = new System.Windows.Forms.ButtonEx();
            this.targetComboBox = new System.Windows.Forms.FlatCombo();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(216, 106);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += CancelButtonClick;
            // 
            // operationComboBox
            // 
            this.operationComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.operationComboBox.FormattingEnabled = true;
            this.operationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.operationComboBox.Location = new System.Drawing.Point(15, 27);
            this.operationComboBox.Name = "operationComboBox";
            this.operationComboBox.Size = new System.Drawing.Size(276, 21);
            this.operationComboBox.TabIndex = 2;
            // 
            // operationLabel
            // 
            this.operationLabel.AutoSize = true;
            this.operationLabel.Location = new System.Drawing.Point(12, 9);
            this.operationLabel.Name = "operationLabel";
            this.operationLabel.Size = new System.Drawing.Size(59, 13);
            this.operationLabel.Text = "Operation:";
            // 
            // targetLabel
            // 
            this.targetLabel.AutoSize = true;
            this.targetLabel.Location = new System.Drawing.Point(12, 55);
            this.targetLabel.Name = "targetLabel";
            this.targetLabel.Size = new System.Drawing.Size(43, 13);
            this.targetLabel.Text = "Target:";
            // 
            // processButton
            // 
            this.processButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.processButton.Location = new System.Drawing.Point(133, 106);
            this.processButton.Name = "processButton";
            this.processButton.Size = new System.Drawing.Size(75, 23);
            this.processButton.TabIndex = 1;
            this.processButton.Text = "Process";
            this.processButton.UseVisualStyleBackColor = true;
            this.processButton.Click += ProcessButtonClick;
            // 
            // targetComboBox
            // 
            this.targetComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.targetComboBox.FormattingEnabled = true;
            this.targetComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.targetComboBox.Location = new System.Drawing.Point(15, 73);
            this.targetComboBox.Name = "targetComboBox";
            this.targetComboBox.Size = new System.Drawing.Size(276, 21);
            this.targetComboBox.TabIndex = 3;
            // 
            // BatchProcessDialog
            //
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.AcceptButton = this.processButton;
            this.CancelButton = this.cancelButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.ClientSize = new System.Drawing.Size(305, 143);
            this.Controls.Add(this.targetComboBox);
            this.Controls.Add(this.processButton);
            this.Controls.Add(this.targetLabel);
            this.Controls.Add(this.operationLabel);
            this.Controls.Add(this.operationComboBox);
            this.Controls.Add(this.cancelButton);
            this.Name = "BatchProcessDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Batch Process";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the settings and localization
        /// </summary>
        private void InitializeDefaults()
        {
            this.targetComboBox.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            this.operationComboBox.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            this.targetComboBox.Items.AddRange(new Object[] 
            {
                TextHelper.GetString("Info.OpenFiles"),
                TextHelper.GetString("Info.ProjectSources")
            });
            //Add processors from BatchProcessManager
            var customProcessors = BatchProcessManager.GetAvailableProcessors();
            foreach (var proc in customProcessors)
            {
                this.operationComboBox.Items.Add(new BatchProcessorItem(proc));
            }
            this.Text = " " + TextHelper.GetString("Title.BatchProcessDialog");
            this.targetComboBox.SelectedIndex = 0;
            this.operationComboBox.SelectedIndex = 0;
            this.processButton.Focus();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ProcessButtonClick(Object sender, EventArgs e)
        {
            var item = (BatchProcessorItem)this.operationComboBox.SelectedItem;

            switch (this.targetComboBox.SelectedIndex)
            {
                case 0: // Open Files
                    var files = new List<string>();
                    foreach (var document in PluginBase.MainForm.Documents)
                    {
                        if (document.IsEditable && !document.IsUntitled) files.Add(document.FileName);
                    }
                    
                    item.Processor.Process(files);
                    
                    break;
                case 1: // Project Sources
                    IProject project = PluginBase.CurrentProject;

                    if (project != null)
                        item.Processor.ProcessProject(project);
                    break;
            }
            this.Close();
        }

        /// <summary>
        /// Closes the batch process dialog
        /// </summary>
        private void CancelButtonClick(Object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

    }

}

