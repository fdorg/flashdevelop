using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FlashDevelop.Controls;
using System.Windows.Forms;
using PluginCore.Localization;
using PluginCore.Controls;
using PluginCore;

namespace FlashDevelop.Dialogs
{
    public class ArgReplaceDialog : SmartForm, IThemeHandler
    {
        Button okButton;
        Panel bottomPanel;
        Button cancelButton;
        FlowLayoutPanel argsPanel;
        readonly Regex regex;
        public string text;

        public ArgReplaceDialog(string text, Regex regex)
        {
            this.text = text;
            this.regex = regex;
            InitializeComponent();
            ApplyLocalizedTexts();
            InitializeInterface();
            Owner = Globals.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "c52528e8-084c-4cb7-9129-cfb64b4184c6";
            if (argsPanel.Controls.Count == 0) Close();
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            bottomPanel = new Panel();
            cancelButton = new ButtonEx();
            okButton = new ButtonEx();
            argsPanel = new FlowLayoutPanel();
            bottomPanel.SuspendLayout();
            SuspendLayout();
            // 
            // bottomPanel
            // 
            bottomPanel.Controls.Add(cancelButton);
            bottomPanel.Controls.Add(okButton);
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Location = new Point(0, 34);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(192, 35);
            bottomPanel.TabIndex = 1;
            // 
            // cancelButton
            //
            cancelButton.FlatStyle = FlatStyle.System;
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(147, 1);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "&Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButtonClick;
            // 
            // okButton
            //
            okButton.FlatStyle = FlatStyle.System;
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(61, 1);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 0;
            okButton.Text = "&OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += OkButtonClick;
            // 
            // argsPanel
            // 
            argsPanel.AutoSize = true;
            argsPanel.Dock = DockStyle.Fill;
            argsPanel.FlowDirection = FlowDirection.TopDown;
            argsPanel.Location = new Point(0, 0);
            argsPanel.Name = "argsPanel";
            argsPanel.Size = new Size(192, 34);
            argsPanel.Padding = new Padding(2, 10, 7, 5);
            argsPanel.TabIndex = 2;
            // 
            // ArgReplaceDialog
            //
            AutoSize = true;
            ShowIcon = false;
            AutoScroll = true;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            AcceptButton = okButton;
            CancelButton = cancelButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(282, 62);
            Controls.Add(argsPanel);
            Controls.Add(bottomPanel);
            Name = "ArgReplaceDialog";
            Text = " Replace Variables";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            Activated += OnDialogActivated;
            bottomPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Accessor for the dictionary
        /// </summary>
        public Dictionary<string, string> Dictionary { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Make sure back colors match
        /// </summary>
        public void AfterTheming() => argsPanel.BackColor = bottomPanel.BackColor = PluginBase.MainForm.GetThemeColor("Form.BackColor", SystemColors.Control);

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        void ApplyLocalizedTexts()
        {
            Text = " " + TextHelper.GetString("Title.ArgReplaceDialog");
            cancelButton.Text = TextHelper.GetString("Label.Cancel");
            okButton.Text = TextHelper.GetString("Label.Ok");
        }

        /// <summary>
        /// Create arg editor controls to edit arguments
        /// </summary>
        void InitializeInterface()
        {
            var match = regex.Match(text);
            while (match.Success)
            {
                var argument = match.Groups[1].Value;
                if (!argsPanel.Controls.ContainsKey(argument))
                {
                    var value = "";
                    if (match.Groups.Count == 3) value = match.Groups[2].Value;
                    var editor = new ArgEditor(argument, value.Split(",".ToCharArray())) {Name = argument};
                    argsPanel.Controls.Add(editor);
                }
                match = match.NextMatch();
            }
        }

        /// <summary>
        /// Build argDictionary for arg/value lookup
        /// </summary>
        void BuildDictionary()
        {
            foreach (ArgEditor control in argsPanel.Controls)
            {
                Dictionary.Add(control.Argument, control.Value);
            }
        }

        /// <summary>
        /// Accept dialog...
        /// </summary>
        void OkButtonClick(object sender, EventArgs e)
        {
            BuildDictionary();
            Close();
        }

        /// <summary>
        /// Cancel dialog...
        /// </summary>
        void CancelButtonClick(object sender, EventArgs e)
        {
            text = null;
            Close();
        }

        /// <summary>
        /// Select first arg editor when form recieves focus
        /// </summary>
        void OnDialogActivated(object sender, EventArgs e) => argsPanel.Controls[0].Focus();

        #endregion

    }
}