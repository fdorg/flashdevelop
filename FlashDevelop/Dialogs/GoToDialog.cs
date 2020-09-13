using System;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Controls;

namespace FlashDevelop.Dialogs
{
    public class GoToDialog : SmartForm
    {
        System.Windows.Forms.Button lineButton;
        System.Windows.Forms.Button closeButton;
        System.Windows.Forms.Button positionButton;
        System.Windows.Forms.TextBox lineTextBox;
        System.Windows.Forms.Label valueLabel;

        public GoToDialog()
        {
            Owner = Globals.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "4d5fdc1c-2698-46e9-b22d-fa9a42ba8d26";
            InitializeComponent();
            ApplyLocalizedTexts();
        }
        
        #region Windows Forms Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent() 
        {
            lineTextBox = new System.Windows.Forms.TextBoxEx();
            positionButton = new System.Windows.Forms.ButtonEx();
            valueLabel = new System.Windows.Forms.Label();
            closeButton = new System.Windows.Forms.ButtonEx();
            lineButton = new System.Windows.Forms.ButtonEx();
            SuspendLayout();
            // 
            // lineTextBox
            // 
            lineTextBox.Location = new System.Drawing.Point(52, 10);
            lineTextBox.Name = "lineTextBox";
            lineTextBox.Size = new System.Drawing.Size(150, 21);
            lineTextBox.TabIndex = 1;
            // 
            // positionButton
            // 
            positionButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            positionButton.Location = new System.Drawing.Point(79, 38);
            positionButton.Name = "positionButton";
            positionButton.Size = new System.Drawing.Size(59, 23);
            positionButton.TabIndex = 3;
            positionButton.Text = "&Position";
            positionButton.Click += PositionButtonClick;
            // 
            // valueLabel
            // 
            valueLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            valueLabel.Location = new System.Drawing.Point(15, 12);
            valueLabel.Name = "valueLabel";
            valueLabel.Size = new System.Drawing.Size(37, 15);
            valueLabel.TabIndex = 0;
            valueLabel.Text = "Value:";
            // 
            // closeButton
            //
            closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            closeButton.Location = new System.Drawing.Point(150, 38);
            closeButton.Name = "closeButton";
            closeButton.Size = new System.Drawing.Size(53, 23);
            closeButton.TabIndex = 4;
            closeButton.Text = "&Close";
            closeButton.Click += CancelButtonClick;
            // 
            // lineButton
            // 
            lineButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            lineButton.Location = new System.Drawing.Point(12, 38);
            lineButton.Name = "lineButton";
            lineButton.Size = new System.Drawing.Size(55, 23);
            lineButton.TabIndex = 2;
            lineButton.Text = "&Line";
            lineButton.Click += LineButtonClick;
            // 
            // GoToDialog
            // 
            AcceptButton = lineButton;
            CancelButton = closeButton;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(216, 71);
            Controls.Add(lineButton);
            Controls.Add(positionButton);
            Controls.Add(closeButton);
            Controls.Add(lineTextBox);
            Controls.Add(valueLabel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GoToDialog";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = " Goto";
            VisibleChanged += VisibleChange;
            Closing += DialogClosing;
            ResumeLayout(false);
            PerformLayout();

        }
        #endregion
        
        #region Methods And Event Handlers

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        void ApplyLocalizedTexts()
        {
            lineButton.Text = TextHelper.GetString("Label.Line");
            positionButton.Text = TextHelper.GetString("Label.Position");
            closeButton.Text = TextHelper.GetString("Label.Close");
            valueLabel.Text = TextHelper.GetString("Info.Value");
            Text = " " + TextHelper.GetString("Title.GoToDialog");
        }

        /// <summary>
        /// Selects the textfield's text
        /// </summary>
        void SelectLineTextBox()
        {
            lineTextBox.Select();
            lineTextBox.SelectAll();
        }

        /// <summary>
        /// Some event handling when showing the form
        /// </summary>
        void VisibleChange(object sender, EventArgs e)
        {
            if (!Visible) return;
            SelectLineTextBox();
            CenterToParent();
        }
        
        /// <summary>
        /// Hides only the dialog when user closes it
        /// </summary>
        void DialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            PluginBase.MainForm.CurrentDocument.Activate();
            Hide();
        }

        /// <summary>
        /// Moves the cursor to the specified line
        /// </summary>
        void LineButtonClick(object sender, EventArgs e)
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci is null) return;
            try
            {
                int line = Convert.ToInt32(lineTextBox.Text) - 1;
                sci.EnsureVisibleEnforcePolicy(line);
                sci.GotoLineIndent(line);
                Close();
            }
            catch
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.GiveProperInt32Value"));
            }
        }

        /// <summary>
        /// Moves the cursor to the specified position
        /// </summary>
        void PositionButtonClick(object sender, EventArgs e)
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci is null) return;
            try
            {
                int pos = Convert.ToInt32(lineTextBox.Text) - 1;
                int line = sci.LineFromPosition(pos);
                sci.EnsureVisibleEnforcePolicy(line);
                sci.GotoPos(pos);
                Close();
            }
            catch
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.GiveProperInt32Value"));
            }
        }

        /// <summary>
        /// Hides the goto dialog
        /// </summary>
        void CancelButtonClick(object sender, EventArgs e) => Close();

        #endregion
    }
}