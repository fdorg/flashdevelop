using System;
using System.Drawing;
using System.Windows.Forms;
using ProjectManager.Projects;
using PluginCore.Localization;
using PluginCore.Controls;

namespace ProjectManager.Controls
{
    public class BuildEventDialog : SmartForm
    {
        readonly BuildEventVars vars;

        #region Windows Form Designer

        StatusBar statusBar;
        Panel panel1;
        ListView listView;
        Button cancelButton;
        Button okButton;
        Splitter splitter1;
        ColumnHeader nameColumn;
        ColumnHeader valueColumn;
        TextBox textBox;
        ToolTip toolTip;
        System.ComponentModel.IContainer components;
        Button insertButton;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            statusBar = new StatusBarEx();
            panel1 = new Panel();
            insertButton = new ButtonEx();
            textBox = new TextBoxEx();
            splitter1 = new Splitter();
            okButton = new ButtonEx();
            listView = new ListViewEx();
            nameColumn = new ColumnHeader();
            valueColumn = new ColumnHeader();
            cancelButton = new ButtonEx();
            toolTip = new ToolTip(components);
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // statusBar
            // 
            statusBar.Location = new Point(0, 320);
            statusBar.Name = "statusBar";
            statusBar.Size = new Size(384, 22);
            statusBar.TabIndex = 0;
            // 
            // panel1
            //
            panel1.Controls.Add(okButton);
            panel1.Controls.Add(insertButton);
            panel1.Controls.Add(cancelButton);
            panel1.Controls.Add(textBox);
            panel1.Controls.Add(listView);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(8, 8, 8, 38);
            panel1.Size = new Size(384, 320);
            panel1.TabIndex = 1;
            // 
            // insertButton
            // 
            insertButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            insertButton.Enabled = false;
            insertButton.FlatStyle = FlatStyle.System;
            insertButton.Location = new Point(7, 288);
            insertButton.Name = "insertButton";
            insertButton.Size = new Size(75, 23);
            insertButton.TabIndex = 4;
            insertButton.Text = "&Insert";
            insertButton.Click += insertButton_Click;
            // 
            // textBox
            // 
            textBox.AcceptsReturn = true;
            textBox.AcceptsTab = true;
            textBox.Dock = DockStyle.Fill;
            textBox.Location = new Point(8, 8);
            textBox.Multiline = true;
            textBox.Name = "textBox";
            textBox.Size = new Size(368, 116);
            textBox.TabIndex = 0;
            textBox.TextChanged += textBox_TextChanged;
            // 
            // splitter1
            // 
            splitter1.Dock = DockStyle.Bottom;
            splitter1.Location = new Point(8, 124);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(368, 6);
            splitter1.TabIndex = 3;
            splitter1.TabStop = false;
            // 
            // okButton
            // 
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.DialogResult = DialogResult.OK;
            okButton.Enabled = false;
            okButton.FlatStyle = FlatStyle.System;
            okButton.Location = new Point(221, 288);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 2;
            okButton.Text = "&OK";
            // 
            // listView
            // 
            listView.Columns.AddRange(new[] {
            nameColumn,
            valueColumn});
            listView.Dock = DockStyle.Bottom;
            listView.FullRowSelect = true;
            listView.GridLines = true;
            listView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView.HideSelection = false;
            listView.Location = new Point(8, 130);
            listView.MultiSelect = false;
            listView.Name = "listView";
            listView.Size = new Size(368, 152);
            listView.TabIndex = 1;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = View.Details;
            listView.DoubleClick += listView_DoubleClick;
            listView.SelectedIndexChanged += listView_SelectedIndexChanged;
            listView.MouseMove += listView_MouseMove;
            // 
            // nameColumn
            // 
            nameColumn.Text = "Name";
            nameColumn.Width = 94;
            // 
            // valueColumn
            // 
            valueColumn.Text = "Value";
            valueColumn.Width = 254;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.FlatStyle = FlatStyle.System;
            cancelButton.Location = new Point(302, 288);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 0;
            cancelButton.Text = "&Cancel";
            // 
            // BuildEventDialog
            // 
            AcceptButton = okButton;
            CancelButton = cancelButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 342);
            Controls.Add(panel1);
            Controls.Add(statusBar);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BuildEventDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Command-Line Builder";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);

        }
        #endregion

        public BuildEventDialog(Project project)
        {
            InitializeComponent();
            InitializeLocalization();
            FormGuid = "ada69d37-2ec0-4484-b113-72bfeab2f239";
            Font = PluginCore.PluginBase.Settings.DefaultFont;

            vars = new BuildEventVars(project);
            foreach (BuildEventInfo info in vars.GetVars()) Add(info);
        }

        public void InitializeLocalization()
        {
            okButton.Text = TextHelper.GetString("Label.OK");
            nameColumn.Text = TextHelper.GetString("Column.Name");
            valueColumn.Text = TextHelper.GetString("Column.Value");
            insertButton.Text = TextHelper.GetString("Label.Insert");
            cancelButton.Text = TextHelper.GetString("Label.Cancel");
            Text = " " + TextHelper.GetString("Title.CommandLineBuilder");
        }

        public string CommandLine
        {
            get => textBox.Text;
            set => textBox.Text = value;
        }

        void Add(BuildEventInfo info)
        {
            ListViewItem item = new ListViewItem(info.Name);
            item.SubItems.Add(info.Value);
            item.Tag = info;
            listView.Items.Add(item);
        }

        void textBox_TextChanged(object sender, EventArgs e)
        {
            okButton.Enabled = true;
        }

        void listView_DoubleClick(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
                DoInsert();
        }

        void insertButton_Click(object sender, EventArgs e)
        {
            DoInsert();
        }

        void DoInsert()
        {
            BuildEventInfo info = listView.SelectedItems[0].Tag as BuildEventInfo;
            
            textBox.Focus();
            SendKeys.Send(info.SendKeysName);
        }

        void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            insertButton.Enabled = (listView.SelectedItems.Count > 0);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            valueColumn.Width = listView.Width - 10 - nameColumn.Width;
        }

        void listView_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem item = listView.GetItemAt(e.X,e.Y);
            
            if (item != null && e.X >= nameColumn.Width)
            {
                BuildEventInfo info = item.Tag as BuildEventInfo;
                Graphics g = listView.CreateGraphics();
                if (g.MeasureString(info.Value,listView.Font).Width > valueColumn.Width)
                {
                    toolTip.SetToolTip(listView,info.Value);
                    return;
                }
            }

            toolTip.SetToolTip(listView,"");
        }
    }
}
