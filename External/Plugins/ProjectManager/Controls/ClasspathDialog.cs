using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Controls;
using PluginCore;

namespace ProjectManager.Controls
{
    public class ClasspathDialog : SmartForm
    {
        #region Form Designer

        private ClasspathControl classpathControl;
        private Button btnCancel;
        private Button btnOK;
        private Label label2;
        private GroupBox groupBox1;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.classpathControl = new ProjectManager.Controls.ClasspathControl();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // classpathControl
            // 
            this.classpathControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.classpathControl.Classpaths = new string[0];
            this.classpathControl.Location = new System.Drawing.Point(10, 19);
            this.classpathControl.Name = "classpathControl";
            this.classpathControl.Size = new System.Drawing.Size(357, 135);
            this.classpathControl.TabIndex = 1;
            this.classpathControl.LanguageBox.SelectedIndexChanged += new System.EventHandler(this.classpathControl_IndexChanged);
            this.classpathControl.Changed += new System.EventHandler(this.classpathControl_Changed);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(314, 206);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 21);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOK.Location = new System.Drawing.Point(233, 206);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 21);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(8, 165);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(358, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "Global classpaths will be saved along with your FlashDevelop settings.";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.classpathControl);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(11, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(377, 192);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Global Classpaths";
            // 
            // ClasspathDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(400, 239);
            this.ControlBox = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.MinimumSize = new System.Drawing.Size(341, 227);
            this.Name = "ClasspathDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Global Classpaths";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #endregion

        ProjectManagerSettings settings;
        bool pathChanged;

        public ClasspathDialog(ProjectManagerSettings settings)
        {
            this.settings = settings;
            InitializeComponent();
            InitializeLocalization();
            this.Font = PluginBase.Settings.DefaultFont;
            this.FormGuid = "695815f3-0c88-418e-aa88-c86a5dfec7ef";
        }

        public string Language
        {
            set 
            {
                if (value == null) return;
                string label = TextHelper.GetString("Title.GlobalClasspathsBox");
                groupBox1.Text = String.Format(label, value.ToUpper());
                classpathControl.Language = value;
            }
            get { return classpathControl.Language; }
        }

        public void InitializeLocalization()
        {
            this.btnOK.Text = TextHelper.GetString("Label.OK");
            this.btnCancel.Text = TextHelper.GetString("Label.Cancel");
            this.label2.Text = TextHelper.GetString("Info.ClasspathsAreSaved");
            this.groupBox1.Text = TextHelper.GetString("Title.GlobalClasspaths");
            this.Text = " " + TextHelper.GetString("Title.GlobalClasspaths");
        }

        public string[] Classpaths
        {
            get { return classpathControl.Classpaths; }
            set { classpathControl.Classpaths = value; }
        }

        private void classpathControl_Changed(object sender, EventArgs e)
        {
            pathChanged = true;
        }

        private void classpathControl_IndexChanged(object sender, EventArgs e)
        {
            SaveClasspath();
            Int32 index = classpathControl.LanguageBox.SelectedIndex;
            this.Language = classpathControl.LanguageBox.Items[index].ToString().ToLower();
            this.Classpaths = this.settings.GetGlobalClasspaths(this.Language).ToArray();
        }

        private void SaveClasspath()
        {
            if (pathChanged)
            {
                pathChanged = false;
                List<String> cps = new List<String>();
                cps.AddRange(this.Classpaths);
                this.settings.SetGlobalClasspaths(this.Language, cps);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveClasspath();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
