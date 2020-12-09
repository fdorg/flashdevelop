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
        Label targetLabel;
        Label operationLabel;
        ComboBox operationComboBox;
        ComboBox targetComboBox;
        Button processButton;
        Button cancelButton;

        public BatchProcessDialog()
        {
            Owner = (Form)PluginBase.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "8edcdafb-b859-410f-9d78-59c0002db62d";
            InitializeComponent();
            InitializeDefaults();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            cancelButton = new ButtonEx();
            operationComboBox = new FlatCombo();
            operationLabel = new Label();
            targetLabel = new Label();
            processButton = new ButtonEx();
            targetComboBox = new FlatCombo();
            SuspendLayout();
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.Location = new System.Drawing.Point(216, 106);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.TabIndex = 0;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButtonClick;
            // 
            // operationComboBox
            // 
            operationComboBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            operationComboBox.FormattingEnabled = true;
            operationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            operationComboBox.Location = new System.Drawing.Point(15, 27);
            operationComboBox.Name = "operationComboBox";
            operationComboBox.Size = new System.Drawing.Size(276, 21);
            operationComboBox.TabIndex = 2;
            // 
            // operationLabel
            // 
            operationLabel.AutoSize = true;
            operationLabel.Location = new System.Drawing.Point(12, 9);
            operationLabel.Name = "operationLabel";
            operationLabel.Size = new System.Drawing.Size(59, 13);
            operationLabel.Text = "Operation:";
            // 
            // targetLabel
            // 
            targetLabel.AutoSize = true;
            targetLabel.Location = new System.Drawing.Point(12, 55);
            targetLabel.Name = "targetLabel";
            targetLabel.Size = new System.Drawing.Size(43, 13);
            targetLabel.Text = "Target:";
            // 
            // processButton
            // 
            processButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            processButton.Location = new System.Drawing.Point(133, 106);
            processButton.Name = "processButton";
            processButton.Size = new System.Drawing.Size(75, 23);
            processButton.TabIndex = 1;
            processButton.Text = "Process";
            processButton.UseVisualStyleBackColor = true;
            processButton.Click += ProcessButtonClick;
            // 
            // targetComboBox
            // 
            targetComboBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            targetComboBox.FormattingEnabled = true;
            targetComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            targetComboBox.Location = new System.Drawing.Point(15, 73);
            targetComboBox.Name = "targetComboBox";
            targetComboBox.Size = new System.Drawing.Size(276, 21);
            targetComboBox.TabIndex = 3;
            // 
            // BatchProcessDialog
            //
            MinimizeBox = false;
            MaximizeBox = false;
            AcceptButton = processButton;
            CancelButton = cancelButton;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            SizeGripStyle = SizeGripStyle.Hide;
            ClientSize = new System.Drawing.Size(305, 143);
            Controls.Add(targetComboBox);
            Controls.Add(processButton);
            Controls.Add(targetLabel);
            Controls.Add(operationLabel);
            Controls.Add(operationComboBox);
            Controls.Add(cancelButton);
            Name = "BatchProcessDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Batch Process";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the settings and localization
        /// </summary>
        void InitializeDefaults()
        {
            targetComboBox.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            operationComboBox.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            targetComboBox.Items.AddRange(new object[] 
            {
                TextHelper.GetString("Info.OpenFiles"),
                TextHelper.GetString("Info.ProjectSources")
            });
            //Add processors from BatchProcessManager
            var customProcessors = BatchProcessManager.GetAvailableProcessors();
            foreach (var proc in customProcessors)
            {
                operationComboBox.Items.Add(new BatchProcessorItem(proc));
            }
            Text = " " + TextHelper.GetString("Title.BatchProcessDialog");
            targetComboBox.SelectedIndex = 0;
            operationComboBox.SelectedIndex = 0;
            processButton.Focus();
        }

        void ProcessButtonClick(object sender, EventArgs e)
        {
            var item = (BatchProcessorItem)operationComboBox.SelectedItem;
            switch (targetComboBox.SelectedIndex)
            {
                case 0: // Open Files
                    var files = new List<string>();
                    foreach (var document in PluginBase.MainForm.Documents)
                    {
                        if (document.SciControl is { } sci && !document.IsUntitled) files.Add(sci.FileName);
                    }
                    
                    item.Processor.Process(files);
                    
                    break;
                case 1: // Project Sources
                    var project = PluginBase.CurrentProject;
                    if (project != null)
                        item.Processor.ProcessProject(project);
                    break;
            }
            Close();
        }

        /// <summary>
        /// Closes the batch process dialog
        /// </summary>
        void CancelButtonClick(object sender, EventArgs e) => Close();

        #endregion
    }
}