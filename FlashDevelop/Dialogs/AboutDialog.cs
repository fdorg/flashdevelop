using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using PluginCore.Localization;
using FlashDevelop.Helpers;
using PluginCore;

namespace FlashDevelop.Dialogs
{
    public class AboutDialog : Form
    {
        Label copyLabel;
        LinkLabel versionLabel;
        PictureBox imageBox;

        public AboutDialog()
        {
            Owner = Globals.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            InitializeComponent();
            ApplyLocalizedTexts();
            InitializeGraphics();
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeComponent() 
        {
            imageBox = new PictureBox();
            copyLabel = new Label();
            versionLabel = new LinkLabel();
            versionLabel.LinkColor = Color.DarkGray;
            ((System.ComponentModel.ISupportInitialize)(imageBox)).BeginInit();
            SuspendLayout();
            // 
            // imageBox
            //
            imageBox.BorderStyle = BorderStyle.None;
            imageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            imageBox.Location = new Point(0, 0);
            imageBox.Name = "imageBox";
            imageBox.Size = new Size(450, 244);
            imageBox.TabIndex = 0;
            imageBox.TabStop = false;
            imageBox.Click += DialogCloseClick;
            // 
            // copyLabel
            //
            copyLabel.AutoSize = true;
            copyLabel.ForeColor = Color.DarkGray;
            copyLabel.BackColor = Color.FromArgb(37, 37, 37);
            copyLabel.FlatStyle = FlatStyle.System;
            copyLabel.Location = new Point(25, 192);
            copyLabel.Name = "copyLabel";
            copyLabel.Size = new Size(383, 30);
            copyLabel.TabIndex = 0;
            copyLabel.Text = DistroConfig.DISTRIBUTION_ABOUT;
            copyLabel.Click += DialogCloseClick;
            // 
            // versionLabel
            //
            versionLabel.AutoSize = true;
            versionLabel.ForeColor = Color.DarkGray;
            versionLabel.BackColor = Color.FromArgb(37, 37, 37);
            versionLabel.FlatStyle = FlatStyle.System;
            versionLabel.Location = new Point(23, 172);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new Size(289, 15);
            versionLabel.TabIndex = 0;
            versionLabel.Text = "FlashDevelop 5.0.0.99 for .NET 3.5 (master#1234567890)";
            versionLabel.Click += DialogCloseClick;
            versionLabel.LinkClicked += VersionLabelLinkClicked;
            // 
            // AboutDialog
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(450, 244);
            Controls.Add(copyLabel);
            Controls.Add(versionLabel);
            Controls.Add(imageBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutDialog";
            ShowInTaskbar = false;
            KeyDown += DialogKeyDown;
            StartPosition = FormStartPosition.CenterParent;
            Text = " About FlashDevelop";
            ((System.ComponentModel.ISupportInitialize)(imageBox)).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Attaches the image to the imagebox
        /// </summary>
        void InitializeGraphics()
        {
            var stream = ResourceHelper.GetStream("AboutDialog.jpg");
            imageBox.Image = Image.FromStream(stream);
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        void ApplyLocalizedTexts()
        {
            string name = Application.ProductName;
            string bit = Application.ExecutablePath.Contains("64") ? "[x64]" : "[x86]";
            Text = " " + TextHelper.GetString("Title.AboutDialog") + " " + bit;
            versionLabel.Font = new Font(Font, FontStyle.Bold);
            versionLabel.Text = name;
            Regex shaRegex = new Regex("#([a-f0-9]*)");
            string sha = shaRegex.Match(name).Captures[0].ToString().Remove(0, 1);
            string link = "www.github.com/fdorg/flashdevelop/commit/" + sha;
            int lastChar = versionLabel.Text.Length;
            int firstChar = versionLabel.Text.IndexOf('(');
            versionLabel.Links.Add(new LinkLabel.Link(firstChar, lastChar, link));
            ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(versionLabel, link);
        }

        /// <summary>
        /// When user clicks the link, open the Github commit in the browser.
        /// </summary>
        static void VersionLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link.LinkData is string target) Process.Start(target);
        }

        /// <summary>
        /// Closes the about dialog
        /// </summary>
        void DialogKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Escape)
            {
                Close();
            }
        }

        /// <summary>
        /// Closes the about dialog
        /// </summary>
        void DialogCloseClick(object sender, EventArgs e) => Close();

        /// <summary>
        /// Shows the about dialog
        /// </summary>
        public new static void Show()
        {
            using var dialog = new AboutDialog();
            dialog.ShowDialog();
        }

        #endregion
    }
}