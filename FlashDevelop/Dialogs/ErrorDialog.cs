using System;
using System.Media;
using System.Drawing;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Controls;

namespace FlashDevelop.Dialogs
{
    public class ErrorDialog : SmartForm
    {
        System.Windows.Forms.Label countLabel;
        System.Windows.Forms.Label headerLabel;
        System.Windows.Forms.Button exitButton;
        System.Windows.Forms.Button continueButton;
        System.Windows.Forms.PictureBox pictureBox;
        System.Windows.Forms.TextBox infoTextBox;
        static int errorCount = 1;

        public ErrorDialog()
        {
            Owner = Globals.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "4f534f7c-8078-4053-9c54-343129c513b3";
            InitializeComponent();
            ApplyLocalizedTexts();
            InitializeGraphics();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            exitButton = new System.Windows.Forms.ButtonEx();
            headerLabel = new System.Windows.Forms.Label();
            continueButton = new System.Windows.Forms.ButtonEx();
            infoTextBox = new System.Windows.Forms.TextBoxEx();
            countLabel = new System.Windows.Forms.Label();
            pictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(pictureBox)).BeginInit();
            SuspendLayout();
            // 
            // exitButton
            // 
            exitButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            exitButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            exitButton.Location = new Point(331, 351);
            exitButton.Name = "exitButton";
            exitButton.Size = new Size(83, 23);
            exitButton.TabIndex = 2;
            exitButton.Text = "&Exit";
            exitButton.UseVisualStyleBackColor = true;
            exitButton.Click += ExitButtonClick;
            // 
            // headerLabel
            // 
            headerLabel.AutoSize = true;
            headerLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            headerLabel.Location = new Point(34, 13);
            headerLabel.Name = "headerLabel";
            headerLabel.Size = new Size(300, 13);
            headerLabel.TabIndex = 0;
            headerLabel.Text = "Error occurred in FlashDevelop. Here are details of the error:";
            // 
            // continueButton
            // 
            continueButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            continueButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            continueButton.Location = new Point(421, 351);
            continueButton.Name = "continueButton";
            continueButton.Size = new Size(90, 23);
            continueButton.TabIndex = 1;
            continueButton.Text = "&Continue";
            continueButton.UseVisualStyleBackColor = true;
            continueButton.Click += ContinueButtonClick;
            // 
            // infoTextBox
            // Font needs to be set here so that controls resize correctly in high-dpi
            //
            infoTextBox.Font = PluginBase.Settings.ConsoleFont;
            infoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            infoTextBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right;
            infoTextBox.Location = new Point(13, 35);
            infoTextBox.Multiline = true;
            infoTextBox.Name = "infoTextBox";
            infoTextBox.Size = new Size(497, 309);
            infoTextBox.TabIndex = 3;
            infoTextBox.WordWrap = false;
            // 
            // countLabel
            // 
            countLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            countLabel.AutoSize = true;
            countLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            countLabel.Location = new Point(14, 355);
            countLabel.Name = "countLabel";
            countLabel.Size = new Size(109, 13);
            countLabel.TabIndex = 0;
            countLabel.Text = "Errors in this session:";
            // 
            // pictureBox
            //
            pictureBox.Location = new Point(13, 12);
            pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(16, 16);
            pictureBox.TabIndex = 6;
            pictureBox.TabStop = false;
            // 
            // ErrorDialog
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = continueButton;
            ClientSize = new Size(522, 386);
            Controls.Add(pictureBox);
            Controls.Add(countLabel);
            Controls.Add(infoTextBox);
            Controls.Add(continueButton);
            Controls.Add(headerLabel);
            Controls.Add(exitButton);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(350, 290);
            Name = "ErrorDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = " Error";
            ((System.ComponentModel.ISupportInitialize)(pictureBox)).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the external graphics
        /// </summary>
        void InitializeGraphics()
        {
            var error = PluginBase.MainForm.FindImage("197", false);
            if (error != null) pictureBox.Image = error;
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        void ApplyLocalizedTexts()
        {
            exitButton.Text = TextHelper.GetString("Label.Exit");
            continueButton.Text = TextHelper.GetString("Label.Continue");
            headerLabel.Text = TextHelper.GetString("Info.ErrorOccurred");
            countLabel.Text = TextHelper.GetString("Info.ErrorsInThisSession");
            Text = " " + TextHelper.GetString("Title.ErrorDialog");
        }

        /// <summary>
        /// Closes the form error dialog
        /// </summary>
        void ContinueButtonClick(object sender, EventArgs e) => Close();

        /// <summary>
        /// Exits the application entirely
        /// </summary>
        static void ExitButtonClick(object sender, EventArgs e) => Environment.Exit(1);

        /// <summary>
        /// Shows the error dialog
        /// </summary> 
        public static void Show(Exception ex)
        {
            SystemSounds.Hand.Play();
            using var dialog = new ErrorDialog();
            dialog.infoTextBox.Text = ex.Message + "\r\n\r\n" + ex.StackTrace;
            dialog.countLabel.Text += " " + errorCount++;
            dialog.exitButton.Enabled = errorCount >= 7;
            dialog.ShowDialog();
        }

        #endregion
    }
}