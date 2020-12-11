// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Windows.Forms;

namespace FlashDevelop.Controls
{
    public class ArgEditor : UserControl
    {
        Label argLabel;
        ComboBox argValues;

        public ArgEditor(string args, string[] values)
        {
            InitializeComponent();
            argLabel.Text = args;
            Font = PluginCore.PluginBase.Settings.DefaultFont;
            if (values.Length > 0)
            {
                argValues.Items.AddRange(values);
                Value = values[0];
            }
        }

        #region Windows Forms Designer Generated Code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            argLabel = new Label();
            argValues = new FlatCombo();
            SuspendLayout();
            // 
            // argLabel
            //
            argLabel.FlatStyle = FlatStyle.System;
            argLabel.Location = new System.Drawing.Point(0, 3);
            argLabel.Margin = new Padding(0);
            argLabel.Name = "argLabel";
            argLabel.Size = new System.Drawing.Size(100, 22);
            argLabel.TabIndex = 0;
            argLabel.Text = "Argument";
            argLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // argValues
            //
            argValues.FormattingEnabled = true;
            argValues.Location = new System.Drawing.Point(105, 1);
            argValues.Name = "argValues";
            argValues.Size = new System.Drawing.Size(160, 22);
            argValues.TabIndex = 1;
            // 
            // ArgEditor
            // 
            AutoSize = true;
            Controls.Add(argValues);
            Controls.Add(argLabel);
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Name = "ArgEditor";
            Size = new System.Drawing.Size(270, 25);
            ResumeLayout(false);

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// 
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            argValues.Focus();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Argument
        {
            get => argLabel.Text;
            set => argLabel.Text = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Value
        {
            get => argValues.Text;
            set => argValues.Text = value;
        }

        #endregion
    }
}