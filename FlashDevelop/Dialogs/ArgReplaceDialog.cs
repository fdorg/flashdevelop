using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FlashDevelop.Controls;
using System.ComponentModel;
using System.Windows.Forms;
using PluginCore.Managers;
using PluginCore.Localization;

namespace FlashDevelop.Dialogs
{
    public class ArgReplaceDialog : Form
    {
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.FlowLayoutPanel argsPanel;
        private System.Collections.Generic.Dictionary<String, String> argDictionary = new Dictionary<String, String>();
        private System.Text.RegularExpressions.Regex regex;
        public System.String text;

        public ArgReplaceDialog(String text, Regex regex)
        {
            this.text = text;
            this.regex = regex;
            this.InitializeComponent();
            this.ApplyLocalizedTexts();
            this.InitializeInterface();
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            if (this.argsPanel.Controls.Count == 0)
            {
                this.Close();
            }
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.argsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.cancelButton);
            this.bottomPanel.Controls.Add(this.okButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 34);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(192, 35);
            this.bottomPanel.TabIndex = 1;
            // 
            // cancelButton
            //
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(147, 1);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
            // 
            // okButton
            //
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(61, 1);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButtonClick);
            // 
            // argsPanel
            // 
            this.argsPanel.AutoSize = true;
            this.argsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.argsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.argsPanel.Location = new System.Drawing.Point(0, 0);
            this.argsPanel.Name = "argsPanel";
            this.argsPanel.Size = new System.Drawing.Size(192, 34);
            this.argsPanel.Padding = new System.Windows.Forms.Padding(2, 10, 7, 5);
            this.argsPanel.TabIndex = 2;
            // 
            // ArgReplaceDialog
            //
            this.AutoSize = true;
            this.ShowIcon = false;
            this.AutoScroll = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 62);
            this.Controls.Add(this.argsPanel);
            this.Controls.Add(this.bottomPanel);
            this.Name = "ArgReplaceDialog";
            this.Text = " Replace Variables";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Activated += new System.EventHandler(this.OnDialogActivated);
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers
        
        /// <summary>
        /// Accessor for the dictionary
        /// </summary>
        public Dictionary<String, String> Dictionary
        {
            get { return this.argDictionary; }
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        private void ApplyLocalizedTexts()
        {
            this.Text = " " + TextHelper.GetString("Title.ArgReplaceDialog");
            this.cancelButton.Text = TextHelper.GetString("Label.Cancel");
            this.okButton.Text = TextHelper.GetString("Label.Ok");
        }

        /// <summary>
        /// Create arg editor controls to edit arguments
        /// </summary>
        private void InitializeInterface()
        {
            String value;
            String argument;
            ArgEditor argEditor;
            Match match = this.regex.Match(this.text);
            while (match.Success)
            {
                argument = match.Groups[1].Value;
                if (!argsPanel.Controls.ContainsKey(argument))
                {
                    value = "";
                    if (match.Groups.Count == 3) value = match.Groups[2].Value;
                    argEditor = new ArgEditor(argument, value.Split(",".ToCharArray()));
                    argEditor.Name = argument;
                    argsPanel.Controls.Add(argEditor);
                }
                match = match.NextMatch();
            }
        }

        /// <summary>
        /// Build argDictionary for arg/value lookup
        /// </summary>
        private void BuildDictionary()
        {
            ArgEditor argEditor;
            foreach (Control control in this.argsPanel.Controls)
            {
                argEditor = control as ArgEditor;
                argDictionary.Add(argEditor.Argument, argEditor.Value);
            }
        }

        /// <summary>
        /// Accept dialog...
        /// </summary>
        private void OkButtonClick(Object sender, EventArgs e)
        {
            this.BuildDictionary();
            this.Close();
        }

        /// <summary>
        /// Cancel dialog...
        /// </summary>
        private void CancelButtonClick(Object sender, EventArgs e)
        {
            this.text = null;
            this.Close();
        }

        /// <summary>
        /// Select first arg editor when form recieves focus
        /// </summary>
        private void OnDialogActivated(Object sender, EventArgs e)
        {
            this.argsPanel.Controls[0].Focus();
        }

        #endregion

    }

}
