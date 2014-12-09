using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FlashDebugger.Properties;
using PluginCore.Localization;
using PluginCore;
using PluginCore.Helpers;
using System.Linq;
using PluginCore.Managers;

namespace FlashDebugger
{
    class BreakPointUI : DockPanelControl
    {
        private static ImageList imageList;

        private DataGridView dgv;
        private PluginMain pluginMain;
        private BreakPointManager breakPointManager;
        private DataGridViewCheckBoxColumn ColumnBreakPointEnable;
        private DataGridViewTextBoxColumn ColumnBreakPointFilePath;
        private DataGridViewTextBoxColumn ColumnBreakPointFileName;
        private DataGridViewTextBoxColumn ColumnBreakPointLine;
        private DataGridViewTextBoxColumn ColumnBreakPointExp;
        private ToolStrip tsActions;
        private ToolStripButton tsbRemoveSelected;
        private ToolStripButton tsbRemoveFiltered;
        private ToolStripButton tsbAlternateFiltered;
        private ToolStripSeparator tsSeparator;
        private ToolStripButton tsbExportFiltered;
        private ToolStripButton tsbImport;
        private ToolStripTextBox tstxtFilter;
        private ToolStripComboBox tscbFilterColumns;
        private Color defaultColor;

        public BreakPointUI(PluginMain pluginMain, BreakPointManager breakPointManager)
        {
            init();
            InitializeLocalization();
            this.pluginMain = pluginMain;
            this.breakPointManager = breakPointManager;
            this.breakPointManager.ChangeBreakPointEvent += breakPointManager_ChangeBreakPointEvent;
            this.breakPointManager.UpdateBreakPointEvent += breakPointManager_UpdateBreakPointEvent;
            this.Controls.Add(this.dgv);
        }

        void breakPointManager_UpdateBreakPointEvent(object sender, UpdateBreakPointArgs e)
        {
            int index = ItemIndex(e.FileFullPath, e.OldLine);
            if (index >= 0)
            {
                dgv.Rows[index].Cells["Line"].Value = e.NewLine.ToString();
            }
        }

        void breakPointManager_ChangeBreakPointEvent(object sender, BreakPointArgs e)
        {
            if (e.IsDelete)
            {
                DeleteItem(e.FileFullPath, e.Line + 1);            
            }
            else
            {
                AddItem(e.FileFullPath, e.Line + 1, e.Exp, e.Enable);
            }
        }

        private void init()
        {
            if (imageList == null)
            {
                imageList = new ImageList();
                imageList.ColorDepth = ColorDepth.Depth32Bit;
                imageList.Images.Add("DeleteBreakpoint", PluginBase.MainForm.ImageSetAdjust(Resource.DeleteBreakpoint));
                imageList.Images.Add("DeleteBreakpoints", PluginBase.MainForm.ImageSetAdjust(Resource.DeleteBreakpoints));
                imageList.Images.Add("ToggleBreakpoints", PluginBase.MainForm.ImageSetAdjust(Resource.ToggleBreakpoints));
                imageList.Images.Add("ExportBreakpoints", PluginBase.MainForm.ImageSetAdjust(Resource.ExportBreakpoints));
                imageList.Images.Add("ImportBreakpoints", PluginBase.MainForm.ImageSetAdjust(Resource.ImportBreakpoints));
            }

            this.dgv = new DataGridView();
            this.dgv.Dock = DockStyle.Fill;
            this.dgv.BorderStyle = BorderStyle.None;
            this.dgv.BackgroundColor = SystemColors.Window;
            this.dgv.Font = PluginBase.Settings.DefaultFont;
            this.dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            this.dgv.EnableHeadersVisualStyles = true;
            this.dgv.RowHeadersVisible = false;

            DataGridViewCellStyle viewStyle = new DataGridViewCellStyle();
            viewStyle.Padding = new Padding(1);
            this.dgv.ColumnHeadersDefaultCellStyle = viewStyle;

            this.ColumnBreakPointEnable = new DataGridViewCheckBoxColumn();
            this.ColumnBreakPointFilePath = new DataGridViewTextBoxColumn();
            this.ColumnBreakPointFileName = new DataGridViewTextBoxColumn();
            this.ColumnBreakPointLine = new DataGridViewTextBoxColumn();
            this.ColumnBreakPointExp = new DataGridViewTextBoxColumn();

            this.ColumnBreakPointEnable.HeaderText = TextHelper.GetString("Label.Enable");
            this.ColumnBreakPointEnable.Name = "Enable";
            this.ColumnBreakPointEnable.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.ColumnBreakPointEnable.Width = 70;

            this.ColumnBreakPointFilePath.HeaderText = TextHelper.GetString("Label.Path");
            this.ColumnBreakPointFilePath.Name = "FilePath";
            this.ColumnBreakPointFilePath.ReadOnly = true;

            this.ColumnBreakPointFileName.HeaderText = TextHelper.GetString("Label.File");
            this.ColumnBreakPointFileName.Name = "FileName";
            this.ColumnBreakPointFileName.ReadOnly = true;

            this.ColumnBreakPointLine.HeaderText = TextHelper.GetString("Label.Line");
            this.ColumnBreakPointLine.Name = "Line";
            this.ColumnBreakPointLine.ReadOnly = true;

            this.ColumnBreakPointExp.HeaderText = TextHelper.GetString("Label.Exp");
            this.ColumnBreakPointExp.Name = "Exp";

            this.dgv.AllowUserToAddRows = false;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.ColumnBreakPointEnable,
                this.ColumnBreakPointFilePath,
                this.ColumnBreakPointFileName,
                this.ColumnBreakPointLine,
                this.ColumnBreakPointExp});

            foreach (DataGridViewColumn column in dgv.Columns)
                column.Width = ScaleHelper.Scale(column.Width);

            defaultColor = dgv.Rows[dgv.Rows.Add()].DefaultCellStyle.BackColor;
            dgv.Rows.Clear();

            this.dgv.CellEndEdit += new DataGridViewCellEventHandler(dgv_CellEndEdit);
            this.dgv.CellMouseUp += new DataGridViewCellMouseEventHandler(dgv_CellMouseUp);
            this.dgv.CellDoubleClick += new DataGridViewCellEventHandler(dgv_CellDoubleClick);

            InitializeComponent();
            tsbRemoveSelected.Image = imageList.Images["DeleteBreakpoint"];
            tsbRemoveFiltered.Image = imageList.Images["DeleteBreakpoints"];
            tsbAlternateFiltered.Image = imageList.Images["ToggleBreakpoints"];
            tsbExportFiltered.Image = imageList.Images["ExportBreakpoints"];
            tsbImport.Image = imageList.Images["ImportBreakpoints"];
            this.tsActions.Renderer = new DockPanelStripRenderer(false);
        }

        void dgv_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            this.breakPointManager.ChangeBreakPointEvent -= breakPointManager_ChangeBreakPointEvent;

            if (e.RowIndex < 0 || e.RowIndex >= dgv.Rows.Count) return;
            if (dgv.Rows[e.RowIndex].Cells["Enable"].ColumnIndex == e.ColumnIndex)
            {
                var row = dgv.Rows[e.RowIndex];
                string filefullpath = (string)row.Cells["FilePath"].Value;
                int line = int.Parse((string)row.Cells["Line"].Value) - 1;
                ITabbedDocument doc = ScintillaHelper.GetDocument(filefullpath);
                bool value = (Boolean)row.Cells["Enable"].EditedFormattedValue;
                if (doc != null)
                {
                    // This logic should be handled by BPManager, so we'll just work arround bad BPs and ignore them
                    if (line < 0 || (doc.SciControl != null && line >= doc.SciControl.LineCount)) return;
                    int marker = value ? ScintillaHelper.markerBPEnabled : ScintillaHelper.markerBPDisabled;
                    Boolean m = ScintillaHelper.IsMarkerSet(doc.SciControl, marker, line);
                    if (!m)
                    {
                        doc.SciControl.MarkerAdd(line, marker);
                        doc.SciControl.MarkerDelete(line, value ? ScintillaHelper.markerBPDisabled : ScintillaHelper.markerBPEnabled);
                    }
                }
                else
                {
                    breakPointManager.SetBreakPointInfo(filefullpath, line, false, value);
                }
            }

            this.breakPointManager.ChangeBreakPointEvent += breakPointManager_ChangeBreakPointEvent;
        }

        void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgv.Rows[e.RowIndex];
            if ((row.Cells["FilePath"].ColumnIndex == e.ColumnIndex) ||
                (row.Cells["FileName"].ColumnIndex == e.ColumnIndex) ||
                (row.Cells["Line"].ColumnIndex == e.ColumnIndex)) 
            {
                string filename = (string)row.Cells["FilePath"].Value;
                int line = int.Parse((string)row.Cells["Line"].Value);
                ScintillaHelper.ActivateDocument(filename, line - 1, true);
            }
        }

        void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgv.Rows[e.RowIndex].Cells["Exp"].ColumnIndex == e.ColumnIndex)
            {
                dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = defaultColor;
                string filename = (string)dgv.Rows[e.RowIndex].Cells["FilePath"].Value;
                int line = int.Parse((string)dgv.Rows[e.RowIndex].Cells["Line"].Value);
                string exp = (string)dgv.Rows[e.RowIndex].Cells["Exp"].Value;
                breakPointManager.SetBreakPointCondition(filename, line - 1, exp);
            }
        }

        public void Clear()
        {
            dgv.Rows.Clear();
        }

        public new Boolean Enabled
        {
            get { return dgv.Enabled; }
            set { dgv.Enabled = value; }
        }

        private void AddItem(string filename, int line, string exp, Boolean enable)
        {
            DataGridViewRow dgvrow;
            int i = ItemIndex(filename, line);
            if (i >= 0) dgvrow = dgv.Rows[i];
            else dgvrow = dgv.Rows[dgv.Rows.Add()];
            dgvrow.Cells["Enable"].Value = enable;
            dgvrow.Cells["FilePath"].Value = filename;
            dgvrow.Cells["FileName"].Value = Path.GetFileName(filename);
            dgvrow.Cells["Line"].Value = line.ToString();
            dgvrow.Cells["Exp"].Value = exp;
        }

        private void DeleteItem(string filename, int line)
        {
            int i = ItemIndex(filename, line);
            if (i >= 0)
            {
                dgv.Rows.RemoveAt(i);
            }
        }

        private int ItemIndex(string filename, int line)
        {
            for (int i = 0, count = dgv.Rows.Count; i < count; i++)
            {
                if ((string)dgv.Rows[i].Cells["FilePath"].Value == filename && (string)dgv.Rows[i].Cells["Line"].Value == line.ToString())
                {
                    return i;
                }
            }
            return -1;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BreakPointUI));
            this.tsActions = new System.Windows.Forms.ToolStrip();
            this.tsbRemoveSelected = new System.Windows.Forms.ToolStripButton();
            this.tsbRemoveFiltered = new System.Windows.Forms.ToolStripButton();
            this.tsbAlternateFiltered = new System.Windows.Forms.ToolStripButton();
            this.tsSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tsbExportFiltered = new System.Windows.Forms.ToolStripButton();
            this.tsbImport = new System.Windows.Forms.ToolStripButton();
            this.tstxtFilter = new System.Windows.Forms.ToolStripTextBox();
            this.tscbFilterColumns = new System.Windows.Forms.ToolStripComboBox();
            this.tsActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsActions
            // 
            this.tsActions.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbRemoveSelected,
            this.tsbRemoveFiltered,
            this.tsbAlternateFiltered,
            this.tsSeparator,
            this.tsbExportFiltered,
            this.tsbImport,
            this.tstxtFilter,
            this.tscbFilterColumns});
            this.tsActions.Location = new System.Drawing.Point(1, 0);
            this.tsActions.Name = "tsActions";
            this.tsActions.Size = new System.Drawing.Size(148, 27);
            this.tsActions.TabIndex = 0;
            this.tsActions.Text = "toolStrip1";
            // 
            // tsbRemoveSelected
            // 
            this.tsbRemoveSelected.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRemoveSelected.Image = ((System.Drawing.Image)(resources.GetObject("tsbRemoveSelected.Image")));
            this.tsbRemoveSelected.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRemoveSelected.Name = "tsbRemoveSelected";
            this.tsbRemoveSelected.Size = new System.Drawing.Size(23, 24);
            this.tsbRemoveSelected.Text = "toolStripButton1";
            this.tsbRemoveSelected.Click += new System.EventHandler(this.TsbRemoveSelected_Click);
            // 
            // tsbRemoveFiltered
            // 
            this.tsbRemoveFiltered.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRemoveFiltered.Image = ((System.Drawing.Image)(resources.GetObject("tsbRemoveFiltered.Image")));
            this.tsbRemoveFiltered.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRemoveFiltered.Name = "tsbRemoveFiltered";
            this.tsbRemoveFiltered.Size = new System.Drawing.Size(23, 24);
            this.tsbRemoveFiltered.Text = "toolStripButton2";
            this.tsbRemoveFiltered.Click += new System.EventHandler(this.TsbRemoveFiltered_Click);
            // 
            // tsbAlternateFiltered
            // 
            this.tsbAlternateFiltered.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAlternateFiltered.Image = ((System.Drawing.Image)(resources.GetObject("tsbAlternateFiltered.Image")));
            this.tsbAlternateFiltered.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAlternateFiltered.Name = "tsbAlternateFiltered";
            this.tsbAlternateFiltered.Size = new System.Drawing.Size(23, 24);
            this.tsbAlternateFiltered.Text = "toolStripButton3";
            this.tsbAlternateFiltered.Click += new System.EventHandler(this.TsbAlternateFiltered_Click);
            // 
            // tsSeparator
            // 
            this.tsSeparator.Name = "tsSeparator";
            this.tsSeparator.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbExportFiltered
            // 
            this.tsbExportFiltered.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbExportFiltered.Image = ((System.Drawing.Image)(resources.GetObject("tsbExportFiltered.Image")));
            this.tsbExportFiltered.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbExportFiltered.Name = "tsbExportFiltered";
            this.tsbExportFiltered.Size = new System.Drawing.Size(23, 24);
            this.tsbExportFiltered.Text = "toolStripButton4";
            this.tsbExportFiltered.Click += new System.EventHandler(this.TsbExportFiltered_Click);
            // 
            // tsbImport
            // 
            this.tsbImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbImport.Image = ((System.Drawing.Image)(resources.GetObject("tsbImport.Image")));
            this.tsbImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbImport.Name = "tsbImport";
            this.tsbImport.Size = new System.Drawing.Size(23, 24);
            this.tsbImport.Text = "toolStripButton5";
            this.tsbImport.Click += new System.EventHandler(this.TsbImport_Click);
            // 
            // tstxtFilter
            // 
            this.tstxtFilter.Name = "tstxtFilter";
            this.tstxtFilter.Size = new System.Drawing.Size(90, 23);
            this.tstxtFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TstxtFilter_KeyDown);
            // 
            // tscbFilterColumns
            // 
            this.tscbFilterColumns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbFilterColumns.Name = "tscbFilterColumns";
            this.tscbFilterColumns.Size = new System.Drawing.Size(116, 23);
            // 
            // BreakPointUI
            // 
            this.Controls.Add(this.tsActions);
            this.Name = "BreakPointUI";
            this.tsActions.ResumeLayout(false);
            this.tsActions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void InitializeLocalization()
        {
            tsbRemoveSelected.ToolTipText = TextHelper.GetString("BreakPoints.RemoveSelected");
            tsbRemoveFiltered.ToolTipText = TextHelper.GetString("BreakPoints.RemoveAll");
            tsbAlternateFiltered.ToolTipText = TextHelper.GetString("BreakPoints.ToggleAll");
            tsbExportFiltered.ToolTipText = TextHelper.GetString("BreakPoints.ExportAll");
            tsbImport.ToolTipText = TextHelper.GetString("BreakPoints.ImportAll");

            tscbFilterColumns.Items.Add(TextHelper.GetString("BreakPoints.FilterAll"));
            tscbFilterColumns.Items.Add(TextHelper.GetString("BreakPoints.FilterPath"));
            tscbFilterColumns.Items.Add(TextHelper.GetString("BreakPoints.FilterFile"));
            tscbFilterColumns.Items.Add(TextHelper.GetString("BreakPoints.FilterLine"));
            tscbFilterColumns.Items.Add(TextHelper.GetString("BreakPoints.FilterExp"));

            tscbFilterColumns.SelectedIndex = 0;
        }

        private void TsbRemoveSelected_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedCells.Count == 0) return;

            this.breakPointManager.ChangeBreakPointEvent -= breakPointManager_ChangeBreakPointEvent;
            var processedRows = new HashSet<DataGridViewRow>();

            foreach (DataGridViewCell selectedCell in dgv.SelectedCells)
            {
                var selected = selectedCell.OwningRow;
                if (processedRows.Contains(selected)) continue;
                processedRows.Add(selected);

                string filefullpath = (string)selected.Cells["FilePath"].Value;
                int line = int.Parse((string)selected.Cells["Line"].Value) - 1;
                ITabbedDocument doc = ScintillaHelper.GetDocument(filefullpath);
                if (doc != null)
                {
                    Boolean m = ScintillaHelper.IsMarkerSet(doc.SciControl, ScintillaHelper.markerBPDisabled, line);
                    doc.SciControl.MarkerDelete(line,
                                                m ? ScintillaHelper.markerBPDisabled : ScintillaHelper.markerBPEnabled);
                }
                else
                {
                    breakPointManager.SetBreakPointInfo(filefullpath, line, true, false);
                }
                dgv.Rows.Remove(selected);
            }

            this.breakPointManager.ChangeBreakPointEvent += breakPointManager_ChangeBreakPointEvent;
            breakPointManager.Save();
        }

        private void TsbRemoveFiltered_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0) return;

            this.breakPointManager.ChangeBreakPointEvent += breakPointManager_ChangeBreakPointEvent;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                string filefullpath = (string)row.Cells["FilePath"].Value;
                int line = int.Parse((string)row.Cells["Line"].Value) - 1;
                ITabbedDocument doc = ScintillaHelper.GetDocument(filefullpath);
                if (doc != null)
                {
                    Boolean m = ScintillaHelper.IsMarkerSet(doc.SciControl, ScintillaHelper.markerBPDisabled, line);
                    doc.SciControl.MarkerDelete(line, m ? ScintillaHelper.markerBPDisabled : ScintillaHelper.markerBPEnabled);
                }
                else
                    breakPointManager.SetBreakPointInfo(filefullpath, line, true, false);
            }

            dgv.Rows.Clear();
            this.breakPointManager.ChangeBreakPointEvent += breakPointManager_ChangeBreakPointEvent;
            breakPointManager.Save();
        }

        private void TsbAlternateFiltered_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0) return;

            this.breakPointManager.ChangeBreakPointEvent -= breakPointManager_ChangeBreakPointEvent;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                string filefullpath = (string)row.Cells["FilePath"].Value;
                int line = int.Parse((string)row.Cells["Line"].Value) - 1;
                bool value = !(bool) row.Cells["Enable"].Value;
                ITabbedDocument doc = ScintillaHelper.GetDocument(filefullpath);
                if (doc != null)
                {
                    int newMarker = value ? ScintillaHelper.markerBPEnabled : ScintillaHelper.markerBPDisabled;
                    Boolean m = ScintillaHelper.IsMarkerSet(doc.SciControl, newMarker, line);
                    if (!m)
                    {
                        doc.SciControl.MarkerAdd(line, newMarker);
                        doc.SciControl.MarkerDelete(line,
                                                    value
                                                        ? ScintillaHelper.markerBPDisabled
                                                        : ScintillaHelper.markerBPEnabled);
                    }
                }
                else
                {
                    breakPointManager.SetBreakPointInfo(filefullpath, line, false, value);
                }
                row.Cells["Enable"].Value = value;
            }
            dgv.EndEdit();

            this.breakPointManager.ChangeBreakPointEvent += breakPointManager_ChangeBreakPointEvent;
            breakPointManager.Save();
        }

        private void TsbExportFiltered_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new SaveFileDialog())
            {
                fileDialog.OverwritePrompt = true;
                fileDialog.Filter = TextHelper.GetString("ProjectManager.Info.FileFilter");
                if (fileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        breakPointManager.Save(fileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.ShowError(ex);
                    }
                }
            }
        }

        private void TsbImport_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.CheckFileExists = true;
                fileDialog.Filter = TextHelper.GetString("ProjectManager.Info.FileFilter");
                if (fileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        breakPointManager.Import(fileDialog.FileName);
                        breakPointManager.SetBreakPointsToEditor(PluginBase.MainForm.Documents);
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.ShowWarning(TextHelper.GetString("Error.InvalidFile"), ex);
                    }
                }
            }
        }

        private void TstxtFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    var regex = new Regex(tstxtFilter.Text, RegexOptions.IgnoreCase);

                    var rows = dgv.Rows.OfType<DataGridViewRow>().ToArray();

                    dgv.Rows.Clear();

                    foreach (var row in rows)
                    {
                        if (tstxtFilter.Text == string.Empty)
                            row.Visible = true;
                        else
                        {
                            bool matches = false;
                            IEnumerable cells = tscbFilterColumns.SelectedIndex == 0
                                                    ? row.Cells
                                                    : (IEnumerable)new[] { row.Cells[tscbFilterColumns.SelectedIndex] };

                            foreach (DataGridViewCell cell in cells)
                            {
                                if (cell.OwningColumn != ColumnBreakPointEnable && ((string)cell.Value).Length > 0 &&
                                    regex.IsMatch((string)cell.Value))
                                {
                                    matches = true;
                                    break;
                                }
                            }

                            row.Visible = matches;
                        }
                    }

                    dgv.Rows.AddRange(rows);
                }
                catch (Exception ex)
                {
                    ErrorManager.ShowWarning(
                        "Error filtering list, please, ensure you've entered a valid RegEx pattern", ex);
                }
            }
        }

    }

}
