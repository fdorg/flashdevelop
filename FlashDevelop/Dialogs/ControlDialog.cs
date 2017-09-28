using System;
using System.Drawing;
using System.Windows.Forms;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore;
using FlashDevelop.Helpers;

namespace FlashDevelop.Dialogs
{
    public class ControlDialog : Form
    {
        public ControlDialog()
        {
            this.InitializeComponent();
            this.propertyGridEx.SelectedObject = this;
            this.customTabControl.DisplayStyle = TabStyle.Flat;
            this.pictureBoxEx.Image = Image.FromStream(ResourceHelper.GetStream("AboutDialog.jpg"));
            this.Load += new EventHandler(this.OnFormLoad);
            ScaleHelper.AdjustForHighDPI(this);
        }

        private void OnFormLoad(Object sender, EventArgs e)
        {
            ScrollBarEx.Attach(this, true);
            ScrollBarEx.Attach(this.dataGridViewEx);
            PluginBase.MainForm.ThemeControls(this);
            this.BackColor = Globals.MainForm.GetThemeColor("Form.BackColor", SystemColors.Control);
            this.dataGridViewEx.Rows.Add(20);
            this.treeViewEx.ExpandAll();
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem17 = new System.Windows.Forms.ListViewItem("Hello Item");
            System.Windows.Forms.ListViewItem listViewItem18 = new System.Windows.Forms.ListViewItem("");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlDialog));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node1");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Node2");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Node3");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Node4");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Node5");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Node0", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Node7");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Node8");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Node9");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Node10");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Node11");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Node12");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Node6", new System.Windows.Forms.TreeNode[] {
            treeNode7,
            treeNode8,
            treeNode9,
            treeNode10,
            treeNode11,
            treeNode12});
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Node14");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Node15");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Node16");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Node17");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Node18");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Node19");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Node20");
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("Node21");
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("Node22");
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("Node13", new System.Windows.Forms.TreeNode[] {
            treeNode14,
            treeNode15,
            treeNode16,
            treeNode17,
            treeNode18,
            treeNode19,
            treeNode20,
            treeNode21,
            treeNode22});
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
            this.buttonEx3.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
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
            this.listBoxEx.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.listBoxEx.FormattingEnabled = true;
            this.listBoxEx.IntegralHeight = false;
            this.listBoxEx.Items.AddRange(new object[] {
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon",
            "Hello from the other side of the moon"});
            this.listBoxEx.Location = new System.Drawing.Point(553, 13);
            this.listBoxEx.Name = "listBoxEx";
            this.listBoxEx.Size = new System.Drawing.Size(261, 280);
            this.listBoxEx.TabIndex = 4;
            this.listBoxEx.UseTheme = true;
            // 
            // listViewEx
            // 
            this.listViewEx.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.listViewEx.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader,
            this.columnHeader1});
            this.listViewEx.GridLineColor = System.Drawing.SystemColors.Control;
            this.listViewEx.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10,
            listViewItem11,
            listViewItem12,
            listViewItem13,
            listViewItem14,
            listViewItem15,
            listViewItem16,
            listViewItem17,
            listViewItem18});
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
            this.treeViewEx.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewEx.Location = new System.Drawing.Point(416, 301);
            this.treeViewEx.Name = "treeViewEx";
            treeNode1.Name = "Node1";
            treeNode1.Text = "Node1";
            treeNode2.Name = "Node2";
            treeNode2.Text = "Node2";
            treeNode3.Name = "Node3";
            treeNode3.Text = "Node3";
            treeNode4.Name = "Node4";
            treeNode4.Text = "Node4";
            treeNode5.Name = "Node5";
            treeNode5.Text = "Node5";
            treeNode6.Name = "Node0";
            treeNode6.Text = "Node0";
            treeNode7.Name = "Node7";
            treeNode7.Text = "Node7";
            treeNode8.Name = "Node8";
            treeNode8.Text = "Node8";
            treeNode9.Name = "Node9";
            treeNode9.Text = "Node9";
            treeNode10.Name = "Node10";
            treeNode10.Text = "Node10";
            treeNode11.Name = "Node11";
            treeNode11.Text = "Node11";
            treeNode12.Name = "Node12";
            treeNode12.Text = "Node12";
            treeNode13.Name = "Node6";
            treeNode13.Text = "Node6";
            treeNode14.Name = "Node14";
            treeNode14.Text = "Node14";
            treeNode15.Name = "Node15";
            treeNode15.Text = "Node15";
            treeNode16.Name = "Node16";
            treeNode16.Text = "Node16";
            treeNode17.Name = "Node17";
            treeNode17.Text = "Node17";
            treeNode18.Name = "Node18";
            treeNode18.Text = "Node18";
            treeNode19.Name = "Node19";
            treeNode19.Text = "Node19";
            treeNode20.Name = "Node20";
            treeNode20.Text = "Node20";
            treeNode21.Name = "Node21";
            treeNode21.Text = "Node21";
            treeNode22.Name = "Node22";
            treeNode22.Text = "Node22";
            treeNode23.Name = "Node13";
            treeNode23.Text = "Node13";
            this.treeViewEx.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode13,
            treeNode23});
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
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
