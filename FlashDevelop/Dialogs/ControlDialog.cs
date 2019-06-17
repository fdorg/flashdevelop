using FlashDevelop.Helpers;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlashDevelop.Dialogs
{
    public class ControlDialog : Form
    {
        private bool showGroups = true;

        public ControlDialog()
        {
            this.InitializeComponent();
            this.propertyGridEx.SelectedObject = this;
            this.customTabControl.DisplayStyle = TabStyle.Flat;
            this.pictureBoxEx.Image = Image.FromStream(ResourceHelper.GetStream("AboutDialog.jpg"));
            this.Load += new EventHandler(this.OnFormLoad);
            ScaleHelper.AdjustForHighDPI(this);
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            ScrollBarEx.Attach(this, true);
            ScrollBarEx.Attach(this.dataGridViewEx);
            PluginBase.MainForm.ThemeControls(this);
            this.BackColor = Globals.MainForm.GetThemeColor("Form.BackColor", SystemColors.Control);
            for (var i = 0; i < 30; i++)
            {
                var msg = "Hello from the other side of the moon " + i;
                this.dataGridViewEx.Rows.Add(msg);
                this.listBoxEx.Items.Add(msg);
            }
            this.treeViewEx.ExpandAll();
        }

        public bool ShowGroups
        {
            get { return showGroups; }
            set
            {
                this.showGroups = value;
                this.listViewEx.ShowGroups = value;
            }
        }

        #region Windows Form Designer Generated Code

        private System.Windows.Forms.RichTextBoxEx richTextBoxEx;
        private System.Windows.Forms.TreeViewEx treeViewEx;
        private System.Windows.Forms.TextBoxEx textBoxEx;
        private System.Windows.Forms.ListViewEx listViewEx;
        private System.Windows.Forms.ListBoxEx listBoxEx;
        private System.Windows.Forms.ButtonEx buttonEx2;
        private System.Windows.Forms.GroupBoxEx groupBoxEx;
        private System.Windows.Forms.CheckBoxEx checkBoxEx;
        private System.Windows.Forms.PropertyGridEx propertyGridEx;
        private System.Windows.Forms.CustomTabControl customTabControl;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ButtonEx buttonEx;
        private System.Windows.Forms.TabPage tabPage1;
        private ColumnHeader columnHeader;
        private ColumnHeader columnHeader1;
        private CheckBoxEx checkBoxEx3;
        private CheckBoxEx checkBoxEx4;
        private StatusBarEx statusBarEx;
        private ButtonEx buttonEx3;
        private ProgressBarEx progressBarEx;
        private PictureBoxEx pictureBoxEx;
        private DataGridViewEx dataGridViewEx;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.CheckBoxEx checkBoxEx2;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("First Group", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup8 = new System.Windows.Forms.ListViewGroup("Second Group", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem61 = new System.Windows.Forms.ListViewItem("Hello Item 1");
            System.Windows.Forms.ListViewItem listViewItem62 = new System.Windows.Forms.ListViewItem("Hello Item 2");
            System.Windows.Forms.ListViewItem listViewItem63 = new System.Windows.Forms.ListViewItem("Hello Item 3");
            System.Windows.Forms.ListViewItem listViewItem64 = new System.Windows.Forms.ListViewItem("Hello Item 4");
            System.Windows.Forms.ListViewItem listViewItem65 = new System.Windows.Forms.ListViewItem("Hello Item 5");
            System.Windows.Forms.ListViewItem listViewItem66 = new System.Windows.Forms.ListViewItem("Hello Item 6");
            System.Windows.Forms.ListViewItem listViewItem67 = new System.Windows.Forms.ListViewItem("Hello Item 7");
            System.Windows.Forms.ListViewItem listViewItem68 = new System.Windows.Forms.ListViewItem("Hello Item 8");
            System.Windows.Forms.ListViewItem listViewItem69 = new System.Windows.Forms.ListViewItem("Hello Item 9");
            System.Windows.Forms.ListViewItem listViewItem70 = new System.Windows.Forms.ListViewItem("Hello Item 10");
            System.Windows.Forms.ListViewItem listViewItem71 = new System.Windows.Forms.ListViewItem("Hello Item 11");
            System.Windows.Forms.ListViewItem listViewItem72 = new System.Windows.Forms.ListViewItem("Hello Item 12");
            System.Windows.Forms.ListViewItem listViewItem73 = new System.Windows.Forms.ListViewItem("Hello Item 13");
            System.Windows.Forms.ListViewItem listViewItem74 = new System.Windows.Forms.ListViewItem("Hello Item 14");
            System.Windows.Forms.ListViewItem listViewItem75 = new System.Windows.Forms.ListViewItem("Hello Item 15");
            System.Windows.Forms.ListViewItem listViewItem76 = new System.Windows.Forms.ListViewItem("Hello Item 16");
            System.Windows.Forms.ListViewItem listViewItem77 = new System.Windows.Forms.ListViewItem("Hello Item 17");
            System.Windows.Forms.ListViewItem listViewItem78 = new System.Windows.Forms.ListViewItem("Hello Item 18");
            System.Windows.Forms.ListViewItem listViewItem79 = new System.Windows.Forms.ListViewItem("Hello Item 19");
            System.Windows.Forms.ListViewItem listViewItem80 = new System.Windows.Forms.ListViewItem("Hello Item 20, Filler, Filler, Filler, Filler, Filler, Filler, Filler, Filler, Fi" +
        "ller, Filler, Filler, Filler, Filler, Filler, Filler, Filler");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlDialog));
            System.Windows.Forms.TreeNode treeNode70 = new System.Windows.Forms.TreeNode("Node1");
            System.Windows.Forms.TreeNode treeNode71 = new System.Windows.Forms.TreeNode("Node2");
            System.Windows.Forms.TreeNode treeNode72 = new System.Windows.Forms.TreeNode("Node3");
            System.Windows.Forms.TreeNode treeNode73 = new System.Windows.Forms.TreeNode("Node4");
            System.Windows.Forms.TreeNode treeNode74 = new System.Windows.Forms.TreeNode("Node5");
            System.Windows.Forms.TreeNode treeNode75 = new System.Windows.Forms.TreeNode("Node0", new System.Windows.Forms.TreeNode[] {
            treeNode70,
            treeNode71,
            treeNode72,
            treeNode73,
            treeNode74});
            System.Windows.Forms.TreeNode treeNode76 = new System.Windows.Forms.TreeNode("Node7");
            System.Windows.Forms.TreeNode treeNode77 = new System.Windows.Forms.TreeNode("Node8");
            System.Windows.Forms.TreeNode treeNode78 = new System.Windows.Forms.TreeNode("Node9");
            System.Windows.Forms.TreeNode treeNode79 = new System.Windows.Forms.TreeNode("Node10");
            System.Windows.Forms.TreeNode treeNode80 = new System.Windows.Forms.TreeNode("Node11");
            System.Windows.Forms.TreeNode treeNode81 = new System.Windows.Forms.TreeNode("Node12");
            System.Windows.Forms.TreeNode treeNode82 = new System.Windows.Forms.TreeNode("Node6", new System.Windows.Forms.TreeNode[] {
            treeNode76,
            treeNode77,
            treeNode78,
            treeNode79,
            treeNode80,
            treeNode81});
            System.Windows.Forms.TreeNode treeNode83 = new System.Windows.Forms.TreeNode("Node14");
            System.Windows.Forms.TreeNode treeNode84 = new System.Windows.Forms.TreeNode("Node15");
            System.Windows.Forms.TreeNode treeNode85 = new System.Windows.Forms.TreeNode("Node16");
            System.Windows.Forms.TreeNode treeNode86 = new System.Windows.Forms.TreeNode("Node17");
            System.Windows.Forms.TreeNode treeNode87 = new System.Windows.Forms.TreeNode("Node18");
            System.Windows.Forms.TreeNode treeNode88 = new System.Windows.Forms.TreeNode("Node19");
            System.Windows.Forms.TreeNode treeNode89 = new System.Windows.Forms.TreeNode("Node20");
            System.Windows.Forms.TreeNode treeNode90 = new System.Windows.Forms.TreeNode("Node21");
            System.Windows.Forms.TreeNode treeNode91 = new System.Windows.Forms.TreeNode("Node22, Filler, Filler, Filler, Filler, Filler, Filler, Filler, Filler, Filler, F" +
        "iller, Filler, Filler, Filler, Filler, Filler, Filler");
            System.Windows.Forms.TreeNode treeNode92 = new System.Windows.Forms.TreeNode("Node13", new System.Windows.Forms.TreeNode[] {
            treeNode83,
            treeNode84,
            treeNode85,
            treeNode86,
            treeNode87,
            treeNode88,
            treeNode89,
            treeNode90,
            treeNode91});
            this.statusBarEx = new System.Windows.Forms.StatusBarEx();
            this.customTabControl = new System.Windows.Forms.CustomTabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridViewEx = new System.Windows.Forms.DataGridViewEx();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.propertyGridEx = new System.Windows.Forms.PropertyGridEx();
            this.groupBoxEx = new System.Windows.Forms.GroupBoxEx();
            this.pictureBoxEx = new System.Windows.Forms.PictureBoxEx();
            this.progressBarEx = new System.Windows.Forms.ProgressBarEx();
            this.buttonEx3 = new System.Windows.Forms.ButtonEx();
            this.checkBoxEx4 = new System.Windows.Forms.CheckBoxEx();
            this.checkBoxEx3 = new System.Windows.Forms.CheckBoxEx();
            this.checkBoxEx2 = new System.Windows.Forms.CheckBoxEx();
            this.buttonEx = new System.Windows.Forms.ButtonEx();
            this.checkBoxEx = new System.Windows.Forms.CheckBoxEx();
            this.buttonEx2 = new System.Windows.Forms.ButtonEx();
            this.listBoxEx = new System.Windows.Forms.ListBoxEx();
            this.listViewEx = new System.Windows.Forms.ListViewEx();
            this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textBoxEx = new System.Windows.Forms.TextBoxEx();
            this.treeViewEx = new System.Windows.Forms.TreeViewEx();
            this.richTextBoxEx = new System.Windows.Forms.RichTextBoxEx();
            this.customTabControl.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEx)).BeginInit();
            this.groupBoxEx.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEx)).BeginInit();
            this.SuspendLayout();
            // 
            // statusBarEx
            // 
            this.statusBarEx.Location = new System.Drawing.Point(0, 769);
            this.statusBarEx.Name = "statusBarEx";
            this.statusBarEx.Size = new System.Drawing.Size(1132, 22);
            this.statusBarEx.TabIndex = 9;
            this.statusBarEx.Text = "StatusBar";
            this.statusBarEx.UseTheme = true;
            // 
            // customTabControl
            // 
            this.customTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.customTabControl.Controls.Add(this.tabPage2);
            this.customTabControl.Controls.Add(this.tabPage1);
            // 
            // 
            // 
            this.customTabControl.DisplayStyleProvider.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.customTabControl.DisplayStyleProvider.BorderColorHot = System.Drawing.SystemColors.ControlDark;
            this.customTabControl.DisplayStyleProvider.BorderColorSelected = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(157)))), ((int)(((byte)(185)))));
            this.customTabControl.DisplayStyleProvider.CloserColor = System.Drawing.Color.DarkGray;
            this.customTabControl.DisplayStyleProvider.FocusTrack = true;
            this.customTabControl.DisplayStyleProvider.HotTrack = true;
            this.customTabControl.DisplayStyleProvider.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.customTabControl.DisplayStyleProvider.Opacity = 1F;
            this.customTabControl.DisplayStyleProvider.Overlap = 0;
            this.customTabControl.DisplayStyleProvider.Padding = new System.Drawing.Point(6, 3);
            this.customTabControl.DisplayStyleProvider.Radius = 2;
            this.customTabControl.DisplayStyleProvider.ShowTabCloser = false;
            this.customTabControl.DisplayStyleProvider.TextColor = System.Drawing.SystemColors.ControlText;
            this.customTabControl.DisplayStyleProvider.TextColorDisabled = System.Drawing.SystemColors.ControlDark;
            this.customTabControl.DisplayStyleProvider.TextColorSelected = System.Drawing.SystemColors.ControlText;
            this.customTabControl.HotTrack = true;
            this.customTabControl.Location = new System.Drawing.Point(413, 528);
            this.customTabControl.Name = "customTabControl";
            this.customTabControl.SelectedIndex = 0;
            this.customTabControl.Size = new System.Drawing.Size(711, 233);
            this.customTabControl.TabIndex = 8;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridViewEx);
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(703, 206);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridViewEx
            // 
            this.dataGridViewEx.AllowUserToAddRows = false;
            this.dataGridViewEx.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewEx.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.dataGridViewEx.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewEx.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewEx.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dataGridViewEx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewEx.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewEx.Name = "dataGridViewEx";
            this.dataGridViewEx.RowHeadersVisible = false;
            this.dataGridViewEx.Size = new System.Drawing.Size(703, 206);
            this.dataGridViewEx.TabIndex = 0;
            this.dataGridViewEx.UseTheme = true;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(703, 206);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // propertyGridEx
            // 
            this.propertyGridEx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridEx.LineColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGridEx.Location = new System.Drawing.Point(822, 13);
            this.propertyGridEx.Name = "propertyGridEx";
            this.propertyGridEx.Size = new System.Drawing.Size(298, 506);
            this.propertyGridEx.TabIndex = 7;
            this.propertyGridEx.ToolbarVisible = false;
            this.propertyGridEx.UseTheme = true;
            // 
            // groupBoxEx
            // 
            this.groupBoxEx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxEx.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.groupBoxEx.Controls.Add(this.pictureBoxEx);
            this.groupBoxEx.Controls.Add(this.progressBarEx);
            this.groupBoxEx.Controls.Add(this.buttonEx3);
            this.groupBoxEx.Controls.Add(this.checkBoxEx4);
            this.groupBoxEx.Controls.Add(this.checkBoxEx3);
            this.groupBoxEx.Controls.Add(this.checkBoxEx2);
            this.groupBoxEx.Controls.Add(this.buttonEx);
            this.groupBoxEx.Controls.Add(this.checkBoxEx);
            this.groupBoxEx.Controls.Add(this.buttonEx2);
            this.groupBoxEx.Location = new System.Drawing.Point(12, 523);
            this.groupBoxEx.Name = "groupBoxEx";
            this.groupBoxEx.Size = new System.Drawing.Size(396, 235);
            this.groupBoxEx.TabIndex = 6;
            this.groupBoxEx.TabStop = false;
            this.groupBoxEx.Text = "GroupBox";
            this.groupBoxEx.UseTheme = true;
            // 
            // pictureBoxEx
            // 
            this.pictureBoxEx.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBoxEx.Location = new System.Drawing.Point(221, 28);
            this.pictureBoxEx.Name = "pictureBoxEx";
            this.pictureBoxEx.Size = new System.Drawing.Size(153, 63);
            this.pictureBoxEx.TabIndex = 13;
            this.pictureBoxEx.TabStop = false;
            this.pictureBoxEx.UseTheme = true;
            // 
            // progressBarEx
            // 
            this.progressBarEx.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.progressBarEx.Location = new System.Drawing.Point(25, 80);
            this.progressBarEx.Name = "progressBarEx";
            this.progressBarEx.Size = new System.Drawing.Size(175, 11);
            this.progressBarEx.TabIndex = 12;
            this.progressBarEx.UseTheme = true;
            this.progressBarEx.Value = 40;
            // 
            // buttonEx3
            // 
            this.buttonEx3.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.buttonEx3.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.buttonEx3.Enabled = false;
            this.buttonEx3.Location = new System.Drawing.Point(25, 104);
            this.buttonEx3.Name = "buttonEx3";
            this.buttonEx3.Size = new System.Drawing.Size(92, 30);
            this.buttonEx3.TabIndex = 11;
            this.buttonEx3.Text = "Button3";
            this.buttonEx3.UseTheme = true;
            this.buttonEx3.UseVisualStyleBackColor = true;
            // 
            // checkBoxEx4
            // 
            this.checkBoxEx4.AutoSize = true;
            this.checkBoxEx4.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.checkBoxEx4.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.checkBoxEx4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxEx4.Location = new System.Drawing.Point(123, 51);
            this.checkBoxEx4.Name = "checkBoxEx4";
            this.checkBoxEx4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxEx4.Size = new System.Drawing.Size(78, 17);
            this.checkBoxEx4.TabIndex = 10;
            this.checkBoxEx4.Text = "Check Me4";
            this.checkBoxEx4.ThreeState = true;
            this.checkBoxEx4.UseTheme = true;
            this.checkBoxEx4.UseVisualStyleBackColor = false;
            // 
            // checkBoxEx3
            // 
            this.checkBoxEx3.AutoSize = true;
            this.checkBoxEx3.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.checkBoxEx3.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.checkBoxEx3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxEx3.Location = new System.Drawing.Point(123, 28);
            this.checkBoxEx3.Name = "checkBoxEx3";
            this.checkBoxEx3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxEx3.Size = new System.Drawing.Size(78, 17);
            this.checkBoxEx3.TabIndex = 9;
            this.checkBoxEx3.Text = "Check Me3";
            this.checkBoxEx3.ThreeState = true;
            this.checkBoxEx3.UseTheme = true;
            this.checkBoxEx3.UseVisualStyleBackColor = false;
            // 
            // checkBoxEx2
            // 
            this.checkBoxEx2.AutoSize = true;
            this.checkBoxEx2.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.checkBoxEx2.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.checkBoxEx2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxEx2.Location = new System.Drawing.Point(25, 51);
            this.checkBoxEx2.Name = "checkBoxEx2";
            this.checkBoxEx2.Size = new System.Drawing.Size(78, 17);
            this.checkBoxEx2.TabIndex = 8;
            this.checkBoxEx2.Text = "Check Me2";
            this.checkBoxEx2.ThreeState = true;
            this.checkBoxEx2.UseTheme = true;
            this.checkBoxEx2.UseVisualStyleBackColor = false;
            // 
            // buttonEx
            // 
            this.buttonEx.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.buttonEx.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.buttonEx.Location = new System.Drawing.Point(25, 180);
            this.buttonEx.Name = "buttonEx";
            this.buttonEx.Size = new System.Drawing.Size(92, 30);
            this.buttonEx.TabIndex = 7;
            this.buttonEx.Text = "Button1";
            this.buttonEx.UseTheme = true;
            this.buttonEx.UseVisualStyleBackColor = true;
            // 
            // checkBoxEx
            // 
            this.checkBoxEx.AutoSize = true;
            this.checkBoxEx.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.checkBoxEx.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.checkBoxEx.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxEx.Location = new System.Drawing.Point(25, 28);
            this.checkBoxEx.Name = "checkBoxEx";
            this.checkBoxEx.Size = new System.Drawing.Size(78, 17);
            this.checkBoxEx.TabIndex = 6;
            this.checkBoxEx.Text = "Check Me1";
            this.checkBoxEx.UseTheme = true;
            this.checkBoxEx.UseVisualStyleBackColor = false;
            // 
            // buttonEx2
            // 
            this.buttonEx2.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.buttonEx2.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.buttonEx2.Location = new System.Drawing.Point(25, 142);
            this.buttonEx2.Name = "buttonEx2";
            this.buttonEx2.Size = new System.Drawing.Size(92, 30);
            this.buttonEx2.TabIndex = 5;
            this.buttonEx2.Text = "Button2";
            this.buttonEx2.UseTheme = true;
            this.buttonEx2.UseVisualStyleBackColor = true;
            // 
            // listBoxEx
            // 
            this.listBoxEx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxEx.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.listBoxEx.FormattingEnabled = true;
            this.listBoxEx.IntegralHeight = false;
            this.listBoxEx.Location = new System.Drawing.Point(553, 13);
            this.listBoxEx.Name = "listBoxEx";
            this.listBoxEx.Size = new System.Drawing.Size(261, 280);
            this.listBoxEx.TabIndex = 4;
            this.listBoxEx.UseTheme = true;
            // 
            // listViewEx
            // 
            this.listViewEx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listViewEx.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.listViewEx.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader,
            this.columnHeader1});
            this.listViewEx.GridLineColor = System.Drawing.SystemColors.Control;
            listViewGroup7.Header = "First Group";
            listViewGroup7.Name = "listViewGroup";
            listViewGroup8.Header = "Second Group";
            listViewGroup8.Name = "listViewGroup2";
            this.listViewEx.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup7,
            listViewGroup8});
            listViewItem61.Group = listViewGroup7;
            listViewItem62.Group = listViewGroup7;
            listViewItem63.Group = listViewGroup7;
            listViewItem64.Group = listViewGroup7;
            listViewItem65.Group = listViewGroup7;
            listViewItem66.Group = listViewGroup7;
            listViewItem67.Group = listViewGroup7;
            listViewItem68.Group = listViewGroup7;
            listViewItem69.Group = listViewGroup7;
            listViewItem70.Group = listViewGroup7;
            listViewItem71.Group = listViewGroup8;
            listViewItem72.Group = listViewGroup8;
            listViewItem73.Group = listViewGroup8;
            listViewItem74.Group = listViewGroup8;
            listViewItem75.Group = listViewGroup8;
            listViewItem76.Group = listViewGroup8;
            listViewItem77.Group = listViewGroup8;
            listViewItem78.Group = listViewGroup8;
            listViewItem79.Group = listViewGroup8;
            listViewItem80.Group = listViewGroup8;
            this.listViewEx.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem61,
            listViewItem62,
            listViewItem63,
            listViewItem64,
            listViewItem65,
            listViewItem66,
            listViewItem67,
            listViewItem68,
            listViewItem69,
            listViewItem70,
            listViewItem71,
            listViewItem72,
            listViewItem73,
            listViewItem74,
            listViewItem75,
            listViewItem76,
            listViewItem77,
            listViewItem78,
            listViewItem79,
            listViewItem80});
            this.listViewEx.Location = new System.Drawing.Point(12, 301);
            this.listViewEx.Name = "listViewEx";
            this.listViewEx.OwnerDraw = true;
            this.listViewEx.Size = new System.Drawing.Size(396, 218);
            this.listViewEx.TabIndex = 3;
            this.listViewEx.UseCompatibleStateImageBehavior = false;
            this.listViewEx.UseTheme = true;
            this.listViewEx.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader
            // 
            this.columnHeader.Text = "Column1";
            this.columnHeader.Width = 277;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Column2";
            this.columnHeader1.Width = 98;
            // 
            // textBoxEx
            // 
            this.textBoxEx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEx.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.textBoxEx.Location = new System.Drawing.Point(282, 12);
            this.textBoxEx.Multiline = true;
            this.textBoxEx.Name = "textBoxEx";
            this.textBoxEx.Size = new System.Drawing.Size(262, 281);
            this.textBoxEx.TabIndex = 2;
            this.textBoxEx.Text = resources.GetString("textBoxEx.Text");
            this.textBoxEx.UseTheme = true;
            // 
            // treeViewEx
            // 
            this.treeViewEx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewEx.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewEx.Location = new System.Drawing.Point(416, 301);
            this.treeViewEx.Name = "treeViewEx";
            treeNode70.Name = "Node1";
            treeNode70.Text = "Node1";
            treeNode71.Name = "Node2";
            treeNode71.Text = "Node2";
            treeNode72.Name = "Node3";
            treeNode72.Text = "Node3";
            treeNode73.Name = "Node4";
            treeNode73.Text = "Node4";
            treeNode74.Name = "Node5";
            treeNode74.Text = "Node5";
            treeNode75.Name = "Node0";
            treeNode75.Text = "Node0";
            treeNode76.Name = "Node7";
            treeNode76.Text = "Node7";
            treeNode77.Name = "Node8";
            treeNode77.Text = "Node8";
            treeNode78.Name = "Node9";
            treeNode78.Text = "Node9";
            treeNode79.Name = "Node10";
            treeNode79.Text = "Node10";
            treeNode80.Name = "Node11";
            treeNode80.Text = "Node11";
            treeNode81.Name = "Node12";
            treeNode81.Text = "Node12";
            treeNode82.Name = "Node6";
            treeNode82.Text = "Node6";
            treeNode83.Name = "Node14";
            treeNode83.Text = "Node14";
            treeNode84.Name = "Node15";
            treeNode84.Text = "Node15";
            treeNode85.Name = "Node16";
            treeNode85.Text = "Node16";
            treeNode86.Name = "Node17";
            treeNode86.Text = "Node17";
            treeNode87.Name = "Node18";
            treeNode87.Text = "Node18";
            treeNode88.Name = "Node19";
            treeNode88.Text = "Node19";
            treeNode89.Name = "Node20";
            treeNode89.Text = "Node20";
            treeNode90.Name = "Node21";
            treeNode90.Text = "Node21";
            treeNode91.Name = "Node22";
            treeNode91.Text = "Node22, Filler, Filler, Filler, Filler, Filler, Filler, Filler, Filler, Filler, F" +
    "iller, Filler, Filler, Filler, Filler, Filler, Filler";
            treeNode92.Name = "Node13";
            treeNode92.Text = "Node13";
            this.treeViewEx.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode75,
            treeNode82,
            treeNode92});
            this.treeViewEx.Size = new System.Drawing.Size(398, 218);
            this.treeViewEx.TabIndex = 1;
            this.treeViewEx.UseTheme = true;
            // 
            // richTextBoxEx
            // 
            this.richTextBoxEx.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.richTextBoxEx.Location = new System.Drawing.Point(12, 12);
            this.richTextBoxEx.Name = "richTextBoxEx";
            this.richTextBoxEx.Size = new System.Drawing.Size(262, 281);
            this.richTextBoxEx.TabIndex = 0;
            this.richTextBoxEx.Text = resources.GetString("richTextBoxEx.Text");
            this.richTextBoxEx.UseTheme = true;
            // 
            // ControlDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1132, 791);
            this.Controls.Add(this.statusBarEx);
            this.Controls.Add(this.customTabControl);
            this.Controls.Add(this.propertyGridEx);
            this.Controls.Add(this.groupBoxEx);
            this.Controls.Add(this.listBoxEx);
            this.Controls.Add(this.listViewEx);
            this.Controls.Add(this.textBoxEx);
            this.Controls.Add(this.treeViewEx);
            this.Controls.Add(this.richTextBoxEx);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(1000, 700);
            this.Name = "ControlDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ControlDialog";
            this.customTabControl.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEx)).EndInit();
            this.groupBoxEx.ResumeLayout(false);
            this.groupBoxEx.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEx)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

    }

}
