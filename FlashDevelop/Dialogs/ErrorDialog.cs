using System;
using System.Media;
using System.Drawing;
using PluginCore.Localization;
using PluginCore.Controls;

namespace FlashDevelop.Dialogs
{
    public class ErrorDialog : SmartForm
    {
        private System.Windows.Forms.Label countLabel;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.TextBox infoTextBox;
        private static Int32 errorCount = 1;

        public ErrorDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.FormGuid = "4f534f7c-8078-4053-9c54-343129c513b3";
            this.InitializeComponent();
            this.ApplyLocalizedTexts();
            this.InitializeGraphics();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.exitButton = new System.Windows.Forms.ButtonEx();
            this.headerLabel = new System.Windows.Forms.Label();
            this.continueButton = new System.Windows.Forms.ButtonEx();
            this.infoTextBox = new System.Windows.Forms.TextBoxEx();
            this.countLabel = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // exitButton
            // 
            this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exitButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.exitButton.Location = new System.Drawing.Point(331, 351);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(83, 23);
            this.exitButton.TabIndex = 2;
            this.exitButton.Text = "&Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.ExitButtonClick);
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.headerLabel.Location = new System.Drawing.Point(34, 13);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(300, 13);
            this.headerLabel.TabIndex = 0;
            this.headerLabel.Text = "Error occurred in FlashDevelop. Here are details of the error:";
            // 
            // continueButton
            // 
            this.continueButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.continueButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.continueButton.Location = new System.Drawing.Point(421, 351);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(90, 23);
            this.continueButton.TabIndex = 1;
            this.continueButton.Text = "&Continue";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.ContinueButtonClick);
            // 
            // infoTextBox
            // Font needs to be set here so that controls resize correctly in high-dpi
            //
            this.infoTextBox.Font = Globals.Settings.ConsoleFont;
            this.infoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.infoTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.infoTextBox.Location = new System.Drawing.Point(13, 35);
            this.infoTextBox.Multiline = true;
            this.infoTextBox.Name = "infoTextBox";
            this.infoTextBox.Size = new System.Drawing.Size(497, 309);
            this.infoTextBox.TabIndex = 3;
            this.infoTextBox.WordWrap = false;
            // 
            // countLabel
            // 
            this.countLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.countLabel.AutoSize = true;
            this.countLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.countLabel.Location = new System.Drawing.Point(14, 355);
            this.countLabel.Name = "countLabel";
            this.countLabel.Size = new System.Drawing.Size(109, 13);
            this.countLabel.TabIndex = 0;
            this.countLabel.Text = "Errors in this session:";
            // 
            // pictureBox
            //
            this.pictureBox.Location = new System.Drawing.Point(13, 12);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(16, 16);
            this.pictureBox.TabIndex = 6;
            this.pictureBox.TabStop = false;
            // 
            // ErrorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.continueButton;
            this.ClientSize = new System.Drawing.Size(522, 386);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.countLabel);
            this.Controls.Add(this.infoTextBox);
            this.Controls.Add(this.continueButton);
            this.Controls.Add(this.headerLabel);
            this.Controls.Add(this.exitButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 290);
            this.Name = "ErrorDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " Error";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the external graphics
        /// </summary>
        private void InitializeGraphics()
        {
            Image error = Globals.MainForm.FindImage("197", false);
            if (error != null) this.pictureBox.Image = error;
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        private void ApplyLocalizedTexts()
        {
            this.exitButton.Text = TextHelper.GetString("Label.Exit");
            this.continueButton.Text = TextHelper.GetString("Label.Continue");
            this.headerLabel.Text = TextHelper.GetString("Info.ErrorOccurred");
            this.countLabel.Text = TextHelper.GetString("Info.ErrorsInThisSession");
            this.Text = " " + TextHelper.GetString("Title.ErrorDialog");
        }

        /// <summary>
        /// Closes the form error dialog
        /// </summary>
        private void ContinueButtonClick(Object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Exits the application entirely
        /// </summary>
        private void ExitButtonClick(Object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        /// <summary>
        /// Shows the error dialog
        /// </summary> 
        public static void Show(Exception ex)
        {
            SystemSounds.Hand.Play();
            using (ErrorDialog errorDialog = new ErrorDialog())
            {
                errorDialog.infoTextBox.Text = ex.Message + "\r\n\r\n" + ex.StackTrace;
                errorDialog.countLabel.Text += " " + errorCount++;
                if (errorCount < 7) errorDialog.exitButton.Enabled = false;
                else errorDialog.exitButton.Enabled = true;
                errorDialog.ShowDialog();
            }
        }

        #endregion

    }

}