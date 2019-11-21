using System;
using System.IO;
using System.Text;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using SharedObjectReader.Controls;
using SOReader.Sol.AMF.DataType;
using SOReader.Sol.AMF;
using SOReader.Sol;
using SOReader;

namespace SharedObjectReader
{
    public class MainForm : Form
    {
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList imageList;
        private SharedObject currentSol;
        private TabControl tabControl;
        private TabPage tabPage;
        private String currentFile;

        public MainForm()
        {
            this.Font = SystemFonts.MenuFont;
            ToolStripManager.VisualStylesEnabled = false;
            this.InitializeComponent();
        }

        #region Windows Form Designer Generated Code

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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage = new System.Windows.Forms.TabPage();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "*.sol";
            this.openFileDialog.Filter = "SharedObject Files|*.sol|All Files|*.*";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip.Size = new System.Drawing.Size(654, 24);
            this.menuStrip.TabIndex = 4;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.reloadToolStripMenuItem,
            this.toolStripSeparator1,
            this.closeToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Enabled = false;
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.reloadToolStripMenuItem.Text = "&Reload";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(115, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 24);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tabControl);
            this.splitContainer.Size = new System.Drawing.Size(654, 426);
            this.splitContainer.SplitterDistance = 218;
            this.splitContainer.TabIndex = 5;
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.FullRowSelect = true;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList;
            this.treeView.Location = new System.Drawing.Point(6, 6);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(211, 392);
            this.treeView.TabIndex = 0;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "None");
            this.imageList.Images.SetKeyName(1, "Array");
            this.imageList.Images.SetKeyName(2, "ByteArray");
            this.imageList.Images.SetKeyName(3, "Date");
            this.imageList.Images.SetKeyName(4, "Int");
            this.imageList.Images.SetKeyName(5, "null");
            this.imageList.Images.SetKeyName(6, "Number");
            this.imageList.Images.SetKeyName(7, "Object");
            this.imageList.Images.SetKeyName(8, "String");
            this.imageList.Images.SetKeyName(9, "undefined");
            this.imageList.Images.SetKeyName(10, "XML");
            this.imageList.Images.SetKeyName(11, "Mixed");
            this.imageList.Images.SetKeyName(12, "Ref");
            this.imageList.Images.SetKeyName(13, "Boolean");
            this.imageList.Images.SetKeyName(14, "LostReference");
            this.imageList.Images.SetKeyName(15, "shared_object");
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPage);
            this.tabControl.ImageList = this.imageList;
            this.tabControl.Location = new System.Drawing.Point(0, 6);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(9, 5);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(429, 394);
            this.tabControl.TabIndex = 1;
            this.tabControl.Visible = false;
            // 
            // tabPage
            // 
            this.tabPage.Location = new System.Drawing.Point(4, 27);
            this.tabPage.Name = "tabPage";
            this.tabPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage.Size = new System.Drawing.Size(421, 363);
            this.tabPage.TabIndex = 1;
            this.tabPage.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 428);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(654, 22);
            this.statusStrip.TabIndex = 6;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Margin = new System.Windows.Forms.Padding(3, 3, 0, 2);
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolStripStatusLabel.Size = new System.Drawing.Size(42, 17);
            this.toolStripStatusLabel.Text = "Ready.";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 450);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " SharedObject Reader";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        private void OpenSharedObject(string filename)
        {
            try
            {
                currentFile = filename;
                currentSol = Reader.Parse(filename);
                tabControl.Visible = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error while parsing the shared object.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            reloadToolStripMenuItem.Enabled = true;
            PopulateTree(currentSol);
        }

        /// <summary>
        /// 
        /// </summary>
        private void CloseSharedObject()
        {
            if (currentSol != null)
            {
                EmptyTree();
                currentSol = null;
            }
            currentFile = null;
            reloadToolStripMenuItem.Enabled = false;
            tabControl.Visible = false;
            toolStripStatusLabel.Text = "Ready.";
        }

        /// <summary>
        /// 
        /// </summary>
        private void EmptyTree()
        {
            this.treeView.Nodes.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        private void PopulateTree(SharedObject obj)
        {
            this.treeView.Nodes.Clear();
            this.treeView.BeginUpdate();

            TreeNode rootnode = new TreeNode(currentSol.Name);
            rootnode.Tag = currentSol;
            rootnode.ImageKey = "shared_object";
            rootnode.SelectedImageKey = "shared_object";
            treeView.Nodes.Add(rootnode);

            TreeNode datanode = new TreeNode("data");
            AssignImage(datanode, (IAMFBase)obj.Data);
            datanode.Tag = currentSol.Data;

            foreach (string key in ((Dictionary<string,object>)obj.Data.Source).Keys)
            {
                Dictionary<string, object> data = (Dictionary<string, object>)obj.Data.Source;
                TreeNode node = new TreeNode(key);
                AssignImage(node, (AMFBase)data[key]);
                node.Tag = ((Dictionary<string, object>)obj.Data.Source)[key];
                PopulateTree(node, ((Dictionary<string, object>)obj.Data.Source)[key]);
                datanode.Nodes.Add(node);
            }
            rootnode.Nodes.Add(datanode);
            this.treeView.EndUpdate();
            toolStripStatusLabel.Text = "Name: " + currentSol.Name + "  |  Size: " + currentSol.FileSize + " bytes  |  AMF encoding: " + currentSol.AMFEncoding;
            this.treeView.SelectedNode = rootnode;

        }

        /// <summary>
        /// 
        /// </summary>
        private void PopulateTree(TreeNode node, object p)
        {
            IAMFBase realObj = (IAMFBase)p;
            if (realObj.AmfType == AMFType.AMF0_ARRAY)
            {
                PopulateNode(node, (Dictionary<string, object>)((AMF0Array)p).Source);
            }
            else if (realObj.AmfType == AMFType.AMF3_ARRAYLIST)
            {
                PopulateNode(node, (ArrayList)((AMF3ArrayList)p).Source);
            }
            else if (realObj.AmfType == AMFType.AMF0_OBJECT || realObj.AmfType == AMFType.AMF3_OBJECT)
            {
                PopulateNode(node, (Dictionary<string, object>)((IAMFBase)p).Source);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void PopulateNode(TreeNode parent_node, Dictionary<string, object> item)
        {
            foreach (string key in item.Keys)
            {
                TreeNode node = new TreeNode(key);
                node.Tag = item[key];
                AssignImage(node, (IAMFBase)item[key]);
                parent_node.Nodes.Add(node);
                PopulateTree(node, item[key]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void PopulateNode(TreeNode parent_node, ArrayList item)
        {
            for (int i = 0; i < item.Count; i++)
            {
                TreeNode node = new TreeNode("" + i);
                node.Tag = item[i];
                AssignImage(node, (IAMFBase)item[i]);
                parent_node.Nodes.Add(node);
                PopulateTree(node, item[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void AssignImage(TreeNode node, IAMFBase element)
        {
            if (imageList.Images.ContainsKey(element.Name))
            {
                node.ImageKey = element.Name;
                node.SelectedImageKey = element.Name;
            }
            else
            {
                node.ImageKey = "None";
                node.SelectedImageKey = "None";
            }
        }

        #endregion

        #region Events Handlers

        /// <summary>
        /// 
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            String appDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            String macromedia = Path.Combine(appDir, "Macromedia");
            String flashPlayer = Path.Combine(macromedia, "Flash Player");
            String sharedObjects = Path.Combine(flashPlayer, "#SharedObjects");
            openFileDialog.InitialDirectory = sharedObjects;
        }

        /// <summary>
        /// 
        /// </summary>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                CloseSharedObject();
                OpenSharedObject(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseSharedObject();
        }

        /// <summary>
        /// 
        /// </summary>
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSharedObject(currentFile);
        }

        /// <summary>
        /// 
        /// </summary>
        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (((TreeView)sender).SelectedNode.Tag != null)
            {
                if (((TreeView)sender).SelectedNode.Tag is IAMFBase)
                {
                    tab_Select(((TreeView)sender).SelectedNode.ImageKey, ((TreeView)sender).SelectedNode.Text, (IAMFBase)((TreeView)sender).SelectedNode.Tag);
                }
                else if (((TreeView)sender).SelectedNode.Tag is SharedObject)
                {
                    tab_Select(((TreeView)sender).SelectedNode.ImageKey, ((TreeView)sender).SelectedNode.Text, (SharedObject)((TreeView)sender).SelectedNode.Tag);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void tab_Select(string imagekey, string p, IAMFBase element)
        {
            tabPage.Text = element.Name;
            tabPage.ImageKey = imagekey;
            tabPage.Controls.Clear();
            UserControl panel = AddPropertyPanel(element);
            if (panel != null)
            {
                panel.Dock = DockStyle.Fill;
                tabPage.Controls.Add(panel);
                ((IAMFDisplayPanel)panel).Populate(p, element);
                if (panel is panelAMF3Reference)
                {
                    ((panelAMF3Reference)panel).Reference += new EventHandler<EventArgs>(MainForm_Reference);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void tab_Select(string imagekey, string p, SharedObject element)
        {
            tabPage.Text = element.Name;
            tabPage.ImageKey = imagekey;
            tabPage.Controls.Clear();
            UserControl panel = AddPropertyPanel(element);
            if (panel != null)
            {
                panel.Dock = DockStyle.Fill;
                tabPage.Controls.Add(panel);
                ((panelSharedObject)panel).Populate(p, element);
            }
        }

        /// <summary>
        /// Click on the link label into a reference panel
        /// </summary>
        void MainForm_Reference(object sender, EventArgs e)
        {
            AMF0Reference element = (AMF0Reference)((panelAMF3Reference)sender).Element;
            IAMFBase result = null;
            try
            {
                result = element.Parser.FindObjectReference(element.Reference);
            } 
            catch(ArgumentOutOfRangeException error)
            {
                MessageBox.Show(error.Message, "Invalid reference #" + element.Reference, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (result != null)
            {
                TreeNode treenode_result = FindForm(this.treeView.Nodes, result);
                if (treenode_result != null) this.treeView.SelectedNode = treenode_result;
            }
            else Debug.WriteLine("Cannot find reference " + element.Reference + ".");
        }

        /// <summary>
        /// 
        /// </summary>
        private TreeNode FindForm(TreeNodeCollection treeNodeCollection, object element)
        {
            foreach (TreeNode node in treeNodeCollection)
            {
                if (node.Tag == element) return node;
                if (node.Nodes.Count > 0)
                {
                    TreeNode ret = FindForm(node.Nodes, element);
                    if (ret != null) return ret;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        private UserControl AddPropertyPanel(SharedObject element)
        {
            return new panelSharedObject();
        }

        /// <summary>
        /// 
        /// </summary>
        private UserControl AddPropertyPanel(IAMFBase element)
        {
            UserControl panel = null;
            switch (element.AmfType)
            {
                case AMFType.AMF0_STRING:
                    panel = new panelAMF0String();
                    break;

                case AMFType.AMF0_BOOLEAN:
                    panel = new panelAMF0Boolean();
                    break;

                case AMFType.AMF0_CLASS:
                    break;

                case AMFType.AMF0_DATE:
                    panel = new panelAMF0Date();
                    break;

                case AMFType.AMF0_NULL:
                case AMFType.AMF0_UNDEFINED:
                    panel = new panelAMF0Null();
                    break;

                case AMFType.AMF0_NUMBER:
                    panel = new panelAMF0Number();
                    break;

                case AMFType.AMF0_XML_STRING:
                case AMFType.AMF3_XML:
                    panel = new panelAMF0Xml();
                    break;

                case AMFType.AMF3_BYTEARRAY:
                    panel = new panelAMF3ByteArray();
                    break;

                case AMFType.AMF3_INT:
                    panel = new panelAMF3Int();
                    break;

                case AMFType.AMF3_REFERENCE:
                case AMFType.AMF0_REFERENCE:
                case AMFType.AMF0_LOST_REFERENCE:
                    panel = new panelAMF3Reference();
                    break;

                case AMFType.AMF3_OBJECT:
                case AMFType.AMF0_OBJECT:
                case AMFType.AMF0_ARRAY:
                case AMFType.AMF0_LIST:
                case AMFType.AMF3_ARRAYLIST:
                default:
                    panel = new panelAMFDefault();
                    Debug.WriteLine("Unknown: " + element.AmfType);
                    break;
            }
            return panel;
        }

        #endregion

    }

}