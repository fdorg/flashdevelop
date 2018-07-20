using System;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Controls;

namespace FlashDevelop.Dialogs
{
    public class GoToDialog : SmartForm
    {
        private System.Windows.Forms.Button lineButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button positionButton;
        private System.Windows.Forms.TextBox lineTextBox;
        private System.Windows.Forms.Label valueLabel;

        public GoToDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.FormGuid = "4d5fdc1c-2698-46e9-b22d-fa9a42ba8d26";
            this.InitializeComponent();
            this.ApplyLocalizedTexts();
        }
        
        #region Windows Forms Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() 
        {
            this.lineTextBox = new System.Windows.Forms.TextBoxEx();
            this.positionButton = new System.Windows.Forms.ButtonEx();
            this.valueLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.ButtonEx();
            this.lineButton = new System.Windows.Forms.ButtonEx();
            this.SuspendLayout();
            // 
            // lineTextBox
            // 
            this.lineTextBox.Location = new System.Drawing.Point(52, 10);
            this.lineTextBox.Name = "lineTextBox";
            this.lineTextBox.Size = new System.Drawing.Size(150, 21);
            this.lineTextBox.TabIndex = 1;
            // 
            // positionButton
            // 
            this.positionButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.positionButton.Location = new System.Drawing.Point(79, 38);
            this.positionButton.Name = "positionButton";
            this.positionButton.Size = new System.Drawing.Size(59, 23);
            this.positionButton.TabIndex = 3;
            this.positionButton.Text = "&Position";
            this.positionButton.Click += new System.EventHandler(this.PositionButtonClick);
            // 
            // valueLabel
            // 
            this.valueLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.valueLabel.Location = new System.Drawing.Point(15, 12);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(37, 15);
            this.valueLabel.TabIndex = 0;
            this.valueLabel.Text = "Value:";
            // 
            // closeButton
            //
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(150, 38);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(53, 23);
            this.closeButton.TabIndex = 4;
            this.closeButton.Text = "&Close";
            this.closeButton.Click += new System.EventHandler(this.CancelButtonClick);
            // 
            // lineButton
            // 
            this.lineButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lineButton.Location = new System.Drawing.Point(12, 38);
            this.lineButton.Name = "lineButton";
            this.lineButton.Size = new System.Drawing.Size(55, 23);
            this.lineButton.TabIndex = 2;
            this.lineButton.Text = "&Line";
            this.lineButton.Click += new System.EventHandler(this.LineButtonClick);
            // 
            // GoToDialog
            // 
            this.AcceptButton = this.lineButton;
            this.CancelButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(216, 71);
            this.Controls.Add(this.lineButton);
            this.Controls.Add(this.positionButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.lineTextBox);
            this.Controls.Add(this.valueLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GoToDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " Goto";
            this.VisibleChanged += new System.EventHandler(this.VisibleChange);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.DialogClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        
        #region Methods And Event Handlers

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        private void ApplyLocalizedTexts()
        {
            this.lineButton.Text = TextHelper.GetString("Label.Line");
            this.positionButton.Text = TextHelper.GetString("Label.Position");
            this.closeButton.Text = TextHelper.GetString("Label.Close");
            this.valueLabel.Text = TextHelper.GetString("Info.Value");
            this.Text = " " + TextHelper.GetString("Title.GoToDialog");
        }

        /// <summary>
        /// Selects the textfield's text
        /// </summary>
        private void SelectLineTextBox()
        {
            this.lineTextBox.Select();
            this.lineTextBox.SelectAll();
        }

        /// <summary>
        /// Some event handling when showing the form
        /// </summary>
        private void VisibleChange(Object sender, System.EventArgs e)
        {
            if (this.Visible)
            {
                this.SelectLineTextBox();
                this.CenterToParent();
            }
        }
        
        /// <summary>
        /// Hides only the dialog when user closes it
        /// </summary>
        private void DialogClosing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Globals.CurrentDocument.Activate();
            this.Hide();
        }

        /// <summary>
        /// Moves the cursor to the specified line
        /// </summary>
        private void LineButtonClick(Object sender, System.EventArgs e)
        {
            if (Globals.SciControl == null) return;
            try
            {
                Int32 line = Convert.ToInt32(this.lineTextBox.Text) - 1;
                Globals.SciControl.EnsureVisibleEnforcePolicy(line);
                Globals.SciControl.GotoLineIndent(line);
                this.Close();
            }
            catch
            {
                String message = TextHelper.GetString("Info.GiveProperInt32Value");
                ErrorManager.ShowInfo(message);
            }
        }

        /// <summary>
        /// Moves the cursor to the specified position
        /// </summary>
        private void PositionButtonClick(Object sender, System.EventArgs e)
        {
            if (Globals.SciControl == null) return;
            try
            {
                Int32 pos = Convert.ToInt32(this.lineTextBox.Text) - 1;
                Int32 line = Globals.SciControl.LineFromPosition(pos);
                Globals.SciControl.EnsureVisibleEnforcePolicy(line);
                Globals.SciControl.GotoPos(pos);
                this.Close();
            }
            catch
            {
                String message = TextHelper.GetString("Info.GiveProperInt32Value");
                ErrorManager.ShowInfo(message);
            }
        }

        /// <summary>
        /// Hides the goto dialog
        /// </summary>
        private void CancelButtonClick(Object sender, System.EventArgs e)
        {
            this.Close();
        }

        #endregion
        
    }
    
}
