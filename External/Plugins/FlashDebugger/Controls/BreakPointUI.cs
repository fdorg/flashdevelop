using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore.Localization;
using PluginCore;
using PluginCore.Helpers;
using System.Linq;
using PluginCore.Managers;
using PluginCore.Controls;

namespace FlashDebugger
{
    class BreakPointUI : DockPanelControl
    {
        private DataGridView dgv;
        private PluginMain pluginMain;
        private BreakPointManager breakPointManager;
        private DataGridViewCheckBoxColumn ColumnBreakPointEnable;
        private DataGridViewTextBoxColumn ColumnBreakPointFilePath;
        private DataGridViewTextBoxColumn ColumnBreakPointFileName;
        private DataGridViewTextBoxColumn ColumnBreakPointLine;
        private DataGridViewTextBoxColumn ColumnBreakPointExp;
        private ToolStripEx tsActions;
        private ToolStripButton tsbRemoveSelected;
        private ToolStripButton tsbRemoveFiltered;
        private ToolStripButton tsbAlternateFiltered;
        private ToolStripSeparator tsSeparator;
        private ToolStripButton tsbExportFiltered;
        private ToolStripButton tsbImport;
        private ToolStripTextBox tstxtFilter;
        private ToolStripComboBoxEx tscbFilterColumns;
        private Color defaultColor;

        public BreakPointUI(PluginMain pluginMain, BreakPointManager breakPointManager)
        {
            init();
            InitializeLocalization();
            this.pluginMain = pluginMain;
            this.breakPointManager = breakPointManager;
            this.breakPointManager.ChangeBreakPointEvent += breakPointManager_ChangeBreakPointEvent;
            this.breakPointManager.UpdateBreakPointEvent += breakPointManager_UpdateBreakPointEvent;
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
            if (e.IsDelete) DeleteItem(e.FileFullPath, e.Line + 1);            
            else AddItem(e.FileFullPath, e.Line + 1, e.Exp, e.Enable);
        }

        private void init()
        {
            AutoKeyHandling = true;
            dgv = new DataGridViewEx();
            dgv.Dock = DockStyle.Fill;
            dgv.BorderStyle = BorderStyle.None;
            dgv.BackgroundColor = SystemColors.Window;
            dgv.Font = PluginBase.Settings.DefaultFont;
            if (ScaleHelper.GetScale() > 1) dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            else dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.EnableHeadersVisualStyles = true;
            dgv.RowHeadersVisible = false;
            DataGridViewCellStyle viewStyle = new DataGridViewCellStyle();
            viewStyle.Padding = new Padding(1);
            dgv.ColumnHeadersDefaultCellStyle = viewStyle;
            ColumnBreakPointEnable = new DataGridViewCheckBoxColumn();
            ColumnBreakPointFilePath = new DataGridViewTextBoxColumn();
            ColumnBreakPointFileName = new DataGridViewTextBoxColumn();
            ColumnBreakPointLine = new DataGridViewTextBoxColumn();
            ColumnBreakPointExp = new DataGridViewTextBoxColumn();
            ColumnBreakPointEnable.HeaderText = TextHelper.GetString("Label.Enable");
            ColumnBreakPointEnable.Name = "Enable";
            ColumnBreakPointEnable.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ColumnBreakPointEnable.Width = 70;
            ColumnBreakPointFilePath.HeaderText = TextHelper.GetString("Label.Path");
            ColumnBreakPointFilePath.Name = "FilePath";
            ColumnBreakPointFilePath.ReadOnly = true;
            ColumnBreakPointFileName.HeaderText = TextHelper.GetString("Label.File");
            ColumnBreakPointFileName.Name = "FileName";
            ColumnBreakPointFileName.ReadOnly = true;
            ColumnBreakPointLine.HeaderText = TextHelper.GetString("Label.Line");
            ColumnBreakPointLine.Name = "Line";
            ColumnBreakPointLine.ReadOnly = true;
            ColumnBreakPointExp.HeaderText = TextHelper.GetString("Label.Exp");
            ColumnBreakPointExp.Name = "Exp";
            dgv.AllowUserToAddRows = false;
            dgv.Columns.AddRange(new DataGridViewColumn[] 
            {
                ColumnBreakPointEnable,
                ColumnBreakPointFilePath,
                ColumnBreakPointFileName,
                ColumnBreakPointLine,
                ColumnBreakPointExp
            });
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.Width = ScaleHelper.Scale(column.Width);
            }
            defaultColor = dgv.Rows[dgv.Rows.Add()].DefaultCellStyle.BackColor;
            dgv.Rows.Clear();
            dgv.CellEndEdit += dgv_CellEndEdit;
            dgv.CellMouseUp += dgv_CellMouseUp;
            dgv.CellDoubleClick += dgv_CellDoubleClick;
            Controls.Add(dgv);
            InitializeComponent();
            tsbRemoveSelected.Image = PluginBase.MainForm.FindImage("548|27|5|5");
            tsbRemoveFiltered.Image = PluginBase.MainForm.FindImage("549|27|5|5");
            tsbAlternateFiltered.Image = PluginBase.MainForm.FindImage("136|23|5|5");
            tsbExportFiltered.Image = PluginBase.MainForm.FindImage("549|22|4|4");
            tsbImport.Image = PluginBase.MainForm.FindImage("549|8|4|4");
            tscbFilterColumns.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            tsActions.Renderer = new DockPanelStripRenderer(false);
            ScrollBarEx.Attach(dgv);
        }

        void dgv_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            breakPointManager.ChangeBreakPointEvent -= breakPointManager_ChangeBreakPointEvent;
            if (e.RowIndex < 0 || e.RowIndex >= dgv.Rows.Count) return;
            if (dgv.Rows[e.RowIndex].Cells["Enable"].ColumnIndex == e.ColumnIndex)
            {
                var row = dgv.Rows[e.RowIndex];
                string filefullpath = (string)row.Cells["FilePath"].Value;
                int line = int.Parse((string)row.Cells["Line"].Value) - 1;
                ITabbedDocument doc = ScintillaHelper.GetDocument(filefullpath);
                bool value = (bool)row.Cells["Enable"].EditedFormattedValue;
                if (doc != null)
                {
                    // This logic should be handled by BPManager, so we'll just work arround bad BPs and ignore them
                    if (line < 0 || (doc.SciControl != null && line >= doc.SciControl.LineCount)) return;
                    int marker = value ? ScintillaHelper.markerBPEnabled : ScintillaHelper.markerBPDisabled;
                    bool m = ScintillaHelper.IsMarkerSet(doc.SciControl, marker, line);
                    if (!m)
                    {
                        doc.SciControl.MarkerAdd(line, marker);
                        doc.SciControl.MarkerDelete(line, value ? ScintillaHelper.markerBPDisabled : ScintillaHelper.markerBPEnabled);
                    }
                }
                else breakPointManager.SetBreakPointInfo(filefullpath, line, false, value);
            }
            breakPointManager.ChangeBreakPointEvent += breakPointManager_ChangeBreakPointEvent;
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

        public new bool Enabled
        {
            get => dgv.Enabled;
            set => dgv.Enabled = value;
        }

        private void AddItem(string filename, int line, string exp, bool enable)
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
            tsActions = new ToolStripEx();
            tsbRemoveSelected = new ToolStripButton();
            tsbRemoveFiltered = new ToolStripButton();
            tsbAlternateFiltered = new ToolStripButton();
            tsSeparator = new ToolStripSeparator();
            tsbExportFiltered = new ToolStripButton();
            tsbImport = new ToolStripButton();
            tstxtFilter = new ToolStripTextBox();
            tscbFilterColumns = new ToolStripComboBoxEx();
            tsActions.SuspendLayout();
            SuspendLayout();
            // 
            // tsActions
            // 
            tsActions.GripStyle = ToolStripGripStyle.Hidden;
            tsActions.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            tsActions.Items.AddRange(new ToolStripItem[] {
            tsbRemoveSelected,
            tsbRemoveFiltered,
            tsbAlternateFiltered,
            tsSeparator,
            tsbExportFiltered,
            tsbImport,
            tstxtFilter,
            tscbFilterColumns});
            tsActions.Location = new Point(1, 0);
            tsActions.Name = "tsActions";
            tsActions.Size = new Size(148, 27);
            tsActions.TabIndex = 0;
            tsActions.Text = "toolStrip1";
            // 
            // tsbRemoveSelected
            // 
            tsbRemoveSelected.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbRemoveSelected.ImageTransparentColor = Color.Magenta;
            tsbRemoveSelected.Name = "tsbRemoveSelected";
            tsbRemoveSelected.Size = new Size(23, 24);
            tsbRemoveSelected.Text = "toolStripButton1";
            tsbRemoveSelected.Click += TsbRemoveSelected_Click;
            // 
            // tsbRemoveFiltered
            // 
            tsbRemoveFiltered.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbRemoveFiltered.ImageTransparentColor = Color.Magenta;
            tsbRemoveFiltered.Name = "tsbRemoveFiltered";
            tsbRemoveFiltered.Size = new Size(23, 24);
            tsbRemoveFiltered.Text = "toolStripButton2";
            tsbRemoveFiltered.Click += TsbRemoveFiltered_Click;
            // 
            // tsbAlternateFiltered
            // 
            tsbAlternateFiltered.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbAlternateFiltered.ImageTransparentColor = Color.Magenta;
            tsbAlternateFiltered.Name = "tsbAlternateFiltered";
            tsbAlternateFiltered.Size = new Size(23, 24);
            tsbAlternateFiltered.Text = "toolStripButton3";
            tsbAlternateFiltered.Click += TsbAlternateFiltered_Click;
            // 
            // tsSeparator
            // 
            tsSeparator.Name = "tsSeparator";
            tsSeparator.Size = new Size(6, 27);
            // 
            // tsbExportFiltered
            // 
            tsbExportFiltered.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbExportFiltered.ImageTransparentColor = Color.Magenta;
            tsbExportFiltered.Name = "tsbExportFiltered";
            tsbExportFiltered.Size = new Size(23, 24);
            tsbExportFiltered.Text = "toolStripButton4";
            tsbExportFiltered.Click += TsbExportFiltered_Click;
            // 
            // tsbImport
            // 
            tsbImport.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbImport.ImageTransparentColor = Color.Magenta;
            tsbImport.Name = "tsbImport";
            tsbImport.Size = new Size(23, 24);
            tsbImport.Text = "toolStripButton5";
            tsbImport.Click += TsbImport_Click;
            // 
            // tstxtFilter
            // 
            tstxtFilter.Name = "tstxtFilter";
            tstxtFilter.Size = new Size(ScaleHelper.Scale(90), 23);
            tstxtFilter.KeyDown += TstxtFilter_KeyDown;
            // 
            // tscbFilterColumns
            // 
            tscbFilterColumns.DropDownStyle = ComboBoxStyle.DropDownList;
            tscbFilterColumns.Name = "tscbFilterColumns";
            tscbFilterColumns.Size = new Size(ScaleHelper.Scale(116), 23);
            // 
            // BreakPointUI
            // 
            Controls.Add(tsActions);
            Name = "BreakPointUI";
            tsActions.ResumeLayout(false);
            tsActions.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
            breakPointManager.ChangeBreakPointEvent -= breakPointManager_ChangeBreakPointEvent;
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
                    bool m = ScintillaHelper.IsMarkerSet(doc.SciControl, ScintillaHelper.markerBPDisabled, line);
                    doc.SciControl.MarkerDelete(line, m ? ScintillaHelper.markerBPDisabled : ScintillaHelper.markerBPEnabled);
                }
                else breakPointManager.SetBreakPointInfo(filefullpath, line, true, false);
                dgv.Rows.Remove(selected);
            }
            breakPointManager.ChangeBreakPointEvent += breakPointManager_ChangeBreakPointEvent;
            breakPointManager.Save();
        }

        private void TsbRemoveFiltered_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0) return;
            breakPointManager.ChangeBreakPointEvent += breakPointManager_ChangeBreakPointEvent;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                string filefullpath = (string)row.Cells["FilePath"].Value;
                int line = int.Parse((string)row.Cells["Line"].Value) - 1;
                ITabbedDocument doc = ScintillaHelper.GetDocument(filefullpath);
                if (doc != null)
                {
                    bool m = ScintillaHelper.IsMarkerSet(doc.SciControl, ScintillaHelper.markerBPDisabled, line);
                    doc.SciControl.MarkerDelete(line, m ? ScintillaHelper.markerBPDisabled : ScintillaHelper.markerBPEnabled);
                }
                else breakPointManager.SetBreakPointInfo(filefullpath, line, true, false);
            }
            dgv.Rows.Clear();
            breakPointManager.ChangeBreakPointEvent += breakPointManager_ChangeBreakPointEvent;
            breakPointManager.Save();
        }

        private void TsbAlternateFiltered_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0) return;
            breakPointManager.ChangeBreakPointEvent -= breakPointManager_ChangeBreakPointEvent;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                string filefullpath = (string)row.Cells["FilePath"].Value;
                int line = int.Parse((string)row.Cells["Line"].Value) - 1;
                bool value = !(bool) row.Cells["Enable"].Value;
                ITabbedDocument doc = ScintillaHelper.GetDocument(filefullpath);
                if (doc != null)
                {
                    int newMarker = value ? ScintillaHelper.markerBPEnabled : ScintillaHelper.markerBPDisabled;
                    bool m = ScintillaHelper.IsMarkerSet(doc.SciControl, newMarker, line);
                    if (!m)
                    {
                        doc.SciControl.MarkerAdd(line, newMarker);
                        doc.SciControl.MarkerDelete(line, value ? ScintillaHelper.markerBPDisabled : ScintillaHelper.markerBPEnabled);
                    }
                }
                else breakPointManager.SetBreakPointInfo(filefullpath, line, false, value);
                row.Cells["Enable"].Value = value;
            }
            dgv.EndEdit();
            breakPointManager.ChangeBreakPointEvent += breakPointManager_ChangeBreakPointEvent;
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
                        if (tstxtFilter.Text == string.Empty) row.Visible = true;
                        else
                        {
                            bool matches = false;
                            IEnumerable cells = tscbFilterColumns.SelectedIndex == 0 ? row.Cells : (IEnumerable)new[] { row.Cells[tscbFilterColumns.SelectedIndex] };
                            foreach (DataGridViewCell cell in cells)
                            {
                                if (cell.OwningColumn != ColumnBreakPointEnable && ((string)cell.Value).Length > 0 && regex.IsMatch((string)cell.Value))
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
                    ErrorManager.ShowWarning("Error filtering list, please ensure you've entered a valid RegEx pattern", ex);
                }
            }
        }

    }

}
