using System;
using System.IO;
using System.Text;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using DifferenceEngine; // http://www.codeproject.com/cs/algorithms/diffengine.asp.

namespace DiffUI
{
    public class MainForm : Form
    {
        private System.Windows.Forms.Panel sourcePanel;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.Panel destinationPanel;
        private System.Windows.Forms.TextBox sourceTextBox;
        private System.Windows.Forms.TextBox destinationTextBox;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ListView sourceListView;
        private System.Windows.Forms.ListView destinationListView;
        private System.Windows.Forms.ColumnHeader sourceLineHeader;
        private System.Windows.Forms.ColumnHeader sourceTextHeader;
        private System.Windows.Forms.ColumnHeader destinationLineHeader;
        private System.Windows.Forms.ColumnHeader destinationTextHeader;
        private System.Windows.Forms.SplitContainer viewSplitContainer;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button browseDestinationButton;
        private System.Windows.Forms.Button browseSourceButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button runButton;
        private System.String[] arguments;

        public MainForm(String[] args)
        {
            this.arguments = args;
            this.Font = SystemFonts.MenuFont;
            this.InitializeComponent();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.sourceListView = new System.Windows.Forms.ListView();
            this.sourceLineHeader = new System.Windows.Forms.ColumnHeader();
            this.sourceTextHeader = new System.Windows.Forms.ColumnHeader();
            this.destinationListView = new System.Windows.Forms.ListView();
            this.destinationLineHeader = new System.Windows.Forms.ColumnHeader();
            this.destinationTextHeader = new System.Windows.Forms.ColumnHeader();
            this.viewSplitContainer = new System.Windows.Forms.SplitContainer();
            this.sourcePanel = new System.Windows.Forms.Panel();
            this.sourceTextBox = new System.Windows.Forms.TextBox();
            this.browseSourceButton = new System.Windows.Forms.Button();
            this.destinationPanel = new System.Windows.Forms.Panel();
            this.destinationTextBox = new System.Windows.Forms.TextBox();
            this.browseDestinationButton = new System.Windows.Forms.Button();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.runButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.viewSplitContainer.Panel1.SuspendLayout();
            this.viewSplitContainer.Panel2.SuspendLayout();
            this.viewSplitContainer.SuspendLayout();
            this.sourcePanel.SuspendLayout();
            this.destinationPanel.SuspendLayout();
            this.controlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // sourceListView
            // 
            this.sourceListView.AllowDrop = true;
            this.sourceListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sourceLineHeader,
            this.sourceTextHeader});
            this.sourceListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceListView.FullRowSelect = true;
            this.sourceListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.sourceListView.HideSelection = false;
            this.sourceListView.Location = new System.Drawing.Point(0, 0);
            this.sourceListView.MultiSelect = false;
            this.sourceListView.Name = "sourceListView";
            this.sourceListView.Size = new System.Drawing.Size(271, 384);
            this.sourceListView.TabIndex = 8;
            this.sourceListView.UseCompatibleStateImageBehavior = false;
            this.sourceListView.View = System.Windows.Forms.View.Details;
            this.sourceListView.SelectedIndexChanged += new System.EventHandler(this.SourceSelectedIndexChanged);
            this.sourceListView.DragOver += new DragEventHandler(this.ListViewDragOver);
            this.sourceListView.DragDrop += new DragEventHandler(this.ListViewDragDrop);
            // 
            // sourceLineHeader
            // 
            this.sourceLineHeader.Text = "Line";
            this.sourceLineHeader.Width = 50;
            // 
            // sourceTextHeader
            // 
            this.sourceTextHeader.Text = "Text (Source)";
            this.sourceTextHeader.Width = 85;
            // 
            // destinationListView
            //
            this.destinationListView.AllowDrop = true;
            this.destinationListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.destinationLineHeader,
            this.destinationTextHeader});
            this.destinationListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.destinationListView.FullRowSelect = true;
            this.destinationListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.destinationListView.HideSelection = false;
            this.destinationListView.Location = new System.Drawing.Point(0, 0);
            this.destinationListView.MultiSelect = false;
            this.destinationListView.Name = "destinationListView";
            this.destinationListView.Size = new System.Drawing.Size(271, 384);
            this.destinationListView.TabIndex = 9;
            this.destinationListView.UseCompatibleStateImageBehavior = false;
            this.destinationListView.View = System.Windows.Forms.View.Details;
            this.destinationListView.SelectedIndexChanged += new System.EventHandler(this.DestinationSelectedIndexChanged);
            this.destinationListView.DragOver += new DragEventHandler(this.ListViewDragOver);
            this.destinationListView.DragDrop += new DragEventHandler(this.ListViewDragDrop);
            // 
            // destinationLineHeader
            // 
            this.destinationLineHeader.Text = "Line";
            this.destinationLineHeader.Width = 50;
            // 
            // destinationTextHeader
            // 
            this.destinationTextHeader.Text = "Text (Destination)";
            this.destinationTextHeader.Width = 105;
            // 
            // viewSplitContainer
            // 
            this.viewSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.viewSplitContainer.Name = "viewSplitContainer";
            // 
            // viewSplitContainer.Panel1
            // 
            this.viewSplitContainer.Panel1.Controls.Add(this.sourceListView);
            this.viewSplitContainer.Panel1.Controls.Add(this.sourcePanel);
            // 
            // viewSplitContainer.Panel2
            // 
            this.viewSplitContainer.Panel2.Controls.Add(this.destinationListView);
            this.viewSplitContainer.Panel2.Controls.Add(this.destinationPanel);
            this.viewSplitContainer.Size = new System.Drawing.Size(546, 411);
            this.viewSplitContainer.SplitterDistance = 271;
            this.viewSplitContainer.TabIndex = 1;
            // 
            // sourcePanel
            // 
            this.sourcePanel.Controls.Add(this.sourceTextBox);
            this.sourcePanel.Controls.Add(this.browseSourceButton);
            this.sourcePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.sourcePanel.Location = new System.Drawing.Point(0, 384);
            this.sourcePanel.Name = "sourcePanel";
            this.sourcePanel.Size = new System.Drawing.Size(271, 27);
            this.sourcePanel.TabIndex = 1;
            // 
            // sourceTextBox
            // 
            this.sourceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceTextBox.Location = new System.Drawing.Point(35, 4);
            this.sourceTextBox.Name = "sourceTextBox";
            this.sourceTextBox.Size = new System.Drawing.Size(236, 21);
            this.sourceTextBox.TabIndex = 1;
            // 
            // browseSourceButton
            // 
            this.browseSourceButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.browseSourceButton.Location = new System.Drawing.Point(3, 3);
            this.browseSourceButton.Name = "browseSourceButton";
            this.browseSourceButton.Size = new System.Drawing.Size(29, 23);
            this.browseSourceButton.TabIndex = 2;
            this.browseSourceButton.Text = "...";
            this.browseSourceButton.UseVisualStyleBackColor = true;
            this.browseSourceButton.Click += new System.EventHandler(this.BrowseSourceButtonClick);
            // 
            // destinationPanel
            // 
            this.destinationPanel.Controls.Add(this.destinationTextBox);
            this.destinationPanel.Controls.Add(this.browseDestinationButton);
            this.destinationPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.destinationPanel.Location = new System.Drawing.Point(0, 384);
            this.destinationPanel.Name = "destinationPanel";
            this.destinationPanel.Size = new System.Drawing.Size(271, 27);
            this.destinationPanel.TabIndex = 1;
            // 
            // destinationTextBox
            // 
            this.destinationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.destinationTextBox.Location = new System.Drawing.Point(31, 4);
            this.destinationTextBox.Name = "destinationTextBox";
            this.destinationTextBox.Size = new System.Drawing.Size(237, 21);
            this.destinationTextBox.TabIndex = 3;
            // 
            // browseDestinationButton
            // 
            this.browseDestinationButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.browseDestinationButton.Location = new System.Drawing.Point(-1, 3);
            this.browseDestinationButton.Name = "browseDestinationButton";
            this.browseDestinationButton.Size = new System.Drawing.Size(29, 23);
            this.browseDestinationButton.TabIndex = 4;
            this.browseDestinationButton.Text = "...";
            this.browseDestinationButton.UseVisualStyleBackColor = true;
            this.browseDestinationButton.Click += new System.EventHandler(this.BrowseDestinationButtonClick);
            // 
            // controlPanel
            // 
            this.controlPanel.Controls.Add(this.progressBar);
            this.controlPanel.Controls.Add(this.runButton);
            this.controlPanel.Controls.Add(this.closeButton);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.controlPanel.Location = new System.Drawing.Point(0, 411);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(546, 27);
            this.controlPanel.TabIndex = 1;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(4, 2);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(378, 21);
            this.progressBar.TabIndex = 5;
            // 
            // runButton
            // 
            this.runButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.runButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.runButton.Location = new System.Drawing.Point(388, 1);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(75, 23);
            this.runButton.TabIndex = 6;
            this.runButton.Text = "&Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.RunButtonClick);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(468, 1);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 7;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "All Files (*.*)|*.*";
            // 
            // MainForm
            // 
            this.AcceptButton = this.runButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(546, 438);
            this.Controls.Add(this.viewSplitContainer);
            this.Controls.Add(this.controlPanel);
            this.DoubleBuffered = true;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " Compare Files";
            this.Resize += new System.EventHandler(this.MainFormResize);
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.viewSplitContainer.Panel1.ResumeLayout(false);
            this.viewSplitContainer.Panel2.ResumeLayout(false);
            this.viewSplitContainer.ResumeLayout(false);
            this.sourcePanel.ResumeLayout(false);
            this.sourcePanel.PerformLayout();
            this.destinationPanel.ResumeLayout(false);
            this.destinationPanel.PerformLayout();
            this.controlPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Makes the list view items autosize.
        /// </summary>
        private void MainFormResize(Object sender, EventArgs e)
        {
            this.sourceTextHeader.Width = -2;
            this.destinationTextHeader.Width = -2;
        }

        /// <summary>
        /// Runs the diff if files are spcified.
        /// </summary>
        private void MainFormLoad(Object sender, EventArgs e)
        {
            this.MainFormResize(null, null);
            Control.CheckForIllegalCrossThreadCalls = false;
            if (this.arguments.Length == 2 && File.Exists(this.arguments[0]) && File.Exists(this.arguments[1]))
            {
                this.sourceTextBox.Text = this.arguments[0];
                this.destinationTextBox.Text = this.arguments[1];
                this.RunButtonClick(null, null);
            }
        }

        /// <summary>
        /// Updates the progress bar.
        /// </summary>
        private void ProgressChanged(Object sender, ProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
        }
        
        /// <summary>
        /// Handles the finish of the work.
        /// </summary>
        private void WorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = Cursors.Default;
            this.browseSourceButton.Enabled = true;
            this.browseDestinationButton.Enabled = true;
            this.runButton.Enabled = true;
            this.progressBar.Value = 0;
        }

        /// <summary>
        /// Handles the drag over event and enables correct dn'd effects.
        /// </summary>
        private void ListViewDragOver(Object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move | DragDropEffects.Copy;
            }
            else e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// Handles the actual file drop and sets the paths correctly.
        /// </summary>
        private void ListViewDragDrop(Object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && File.Exists(files[0]))
                {
                    if (sender == this.sourceListView) this.sourceTextBox.Text = files[0];
                    else this.destinationTextBox.Text = files[0];
                }
            }
        }

        /// <summary>
        /// Changes the selected index of the other list view.
        /// </summary>
        private void SourceSelectedIndexChanged(Object sender, System.EventArgs e)
        {
            if (this.sourceListView.SelectedItems.Count > 0)
            {
                ListViewItem lvi = this.destinationListView.Items[this.sourceListView.SelectedItems[0].Index];
                lvi.Selected = true;
                lvi.EnsureVisible();
            }
        }

        /// <summary>
        ///  Changes the selected index of the other list view.
        /// </summary>
        private void DestinationSelectedIndexChanged(Object sender, System.EventArgs e)
        {
            if (this.destinationListView.SelectedItems.Count > 0)
            {
                ListViewItem lvi = this.sourceListView.Items[this.destinationListView.SelectedItems[0].Index];
                lvi.Selected = true;
                lvi.EnsureVisible();
            }
        }

        /// <summary>
        /// Selects the source file.
        /// </summary>
        private void BrowseSourceButtonClick(Object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK && File.Exists(this.openFileDialog.FileName))
            {
                this.sourceTextBox.Text = this.openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Selects the destination file.
        /// </summary>
        private void BrowseDestinationButtonClick(Object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK && File.Exists(this.openFileDialog.FileName))
            {
                this.destinationTextBox.Text = this.openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        private void CloseButtonClick(Object sender, EventArgs e)
        {
            if (this.backgroundWorker != null)
            {
                this.backgroundWorker.CancelAsync();
            }
            this.Close();
        }

        /// <summary>
        /// Starts the background worker.
        /// </summary>
        private void RunButtonClick(Object sender, EventArgs e)
        {
            String source = this.sourceTextBox.Text;
            String destination = this.destinationTextBox.Text;
            if (File.Exists(source) && File.Exists(destination))
            {
                this.backgroundWorker = new BackgroundWorker();
                this.backgroundWorker.WorkerReportsProgress = true;
                this.backgroundWorker.WorkerSupportsCancellation = true;
                this.backgroundWorker.DoWork += new DoWorkEventHandler(this.CompareDifference);
                this.backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(this.ProgressChanged);
                this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.WorkerCompleted);
                this.backgroundWorker.RunWorkerAsync(new String[2]{source, destination});
                this.browseDestinationButton.Enabled = false;
                this.browseSourceButton.Enabled = false;
                this.runButton.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
            }
        }

        /// <summary>
        /// Runs the diff process for the specified files.
        /// </summary>
        public void CompareDifference(Object sender, DoWorkEventArgs e)
        {
            this.sourceListView.Items.Clear();
            this.destinationListView.Items.Clear();
            this.sourceListView.BeginUpdate();
            this.destinationListView.BeginUpdate();
            try
            {
                String source = ((String[])e.Argument)[0];
                String destination = ((String[])e.Argument)[1];
                Int32 count = 1; ListViewItem lviS, lviD;
                TextFile file1 = new TextFile(source);
                TextFile file2 = new TextFile(destination);
                DiffEngine de = new DiffEngine();
                de.ProcessDiff(file1, file2, DiffEngineLevel.Medium);
                ArrayList lines = de.DiffReport();
                Int32 counter = 0, total = lines.Count;
                foreach (DiffResultSpan drs in lines)
                {
                    switch (drs.Status)
                    {
                        case DiffResultSpanStatus.DeleteSource:
                            for (Int32 i = 0; i < drs.Length; i++)
                            {
                                lviS = new ListViewItem(count.ToString("00000"));
                                lviD = new ListViewItem(count.ToString("00000"));
                                lviS.Font = lviD.Font = new Font("Courier New", 8.25F);
                                lviS.BackColor = Color.Red;
                                lviS.SubItems.Add(((TextLine)file1.GetByIndex(drs.SourceIndex + i)).Line);
                                lviD.BackColor = Color.LightGray;
                                lviD.SubItems.Add("");
                                this.sourceListView.Items.Add(lviS);
                                this.destinationListView.Items.Add(lviD);
                                count++;
                            }
                            break;

                        case DiffResultSpanStatus.NoChange:
                            for (Int32 i = 0; i < drs.Length; i++)
                            {
                                lviS = new ListViewItem(count.ToString("00000"));
                                lviD = new ListViewItem(count.ToString("00000"));
                                lviS.Font = lviD.Font = new Font("Courier New", 8.25F);
                                lviS.BackColor = Color.White;
                                lviS.SubItems.Add(((TextLine)file1.GetByIndex(drs.SourceIndex + i)).Line);
                                lviD.BackColor = Color.White;
                                lviD.SubItems.Add(((TextLine)file2.GetByIndex(drs.DestIndex + i)).Line);
                                this.sourceListView.Items.Add(lviS);
                                this.destinationListView.Items.Add(lviD);
                                count++;
                            }
                            break;

                        case DiffResultSpanStatus.AddDestination:
                            for (Int32 i = 0; i < drs.Length; i++)
                            {
                                lviS = new ListViewItem(count.ToString("00000"));
                                lviD = new ListViewItem(count.ToString("00000"));
                                lviS.Font = lviD.Font = new Font("Courier New", 8.25F);
                                lviS.BackColor = Color.LightGray;
                                lviS.SubItems.Add("");
                                lviD.BackColor = Color.LightGreen;
                                lviD.SubItems.Add(((TextLine)file2.GetByIndex(drs.DestIndex + i)).Line);
                                this.sourceListView.Items.Add(lviS);
                                this.destinationListView.Items.Add(lviD);
                                count++;
                            }
                            break;

                        case DiffResultSpanStatus.Replace:
                            for (Int32 i = 0; i < drs.Length; i++)
                            {
                                lviS = new ListViewItem(count.ToString("00000"));
                                lviD = new ListViewItem(count.ToString("00000"));
                                lviS.Font = lviD.Font = new Font("Courier New", 8.25F);
                                lviS.BackColor = Color.Red;
                                lviS.SubItems.Add(((TextLine)file1.GetByIndex(drs.SourceIndex + i)).Line);
                                lviD.BackColor = Color.LightGreen;
                                lviD.SubItems.Add(((TextLine)file2.GetByIndex(drs.DestIndex + i)).Line);
                                this.sourceListView.Items.Add(lviS);
                                this.destinationListView.Items.Add(lviD);
                                count++;
                            }
                            break;
                    }
                    counter++;
                    Int32 percent = (100 * counter) / total;
                    this.backgroundWorker.ReportProgress(percent);
                }
                e.Result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                e.Result = false;
            }
            this.destinationListView.EndUpdate();
            this.sourceListView.EndUpdate();
        }

        #endregion

    }

}