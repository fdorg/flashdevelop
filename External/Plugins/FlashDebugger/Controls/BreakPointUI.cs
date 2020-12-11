// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
    internal class BreakPointUI : DockPanelControl
    {
        DataGridView dgv;
        readonly BreakPointManager breakPointManager;
        DataGridViewCheckBoxColumn ColumnBreakPointEnable;
        DataGridViewTextBoxColumn ColumnBreakPointFilePath;
        DataGridViewTextBoxColumn ColumnBreakPointFileName;
        DataGridViewTextBoxColumn ColumnBreakPointLine;
        DataGridViewTextBoxColumn ColumnBreakPointExp;
        ToolStripEx tsActions;
        ToolStripButton tsbRemoveSelected;
        ToolStripButton tsbRemoveFiltered;
        ToolStripButton tsbAlternateFiltered;
        ToolStripSeparator tsSeparator;
        ToolStripButton tsbExportFiltered;
        ToolStripButton tsbImport;
        ToolStripTextBox tstxtFilter;
        ToolStripComboBoxEx tscbFilterColumns;
        Color defaultColor;

        public BreakPointUI(BreakPointManager breakPointManager)
        {
            Init();
            InitializeLocalization();
            this.breakPointManager = breakPointManager;
            this.breakPointManager.ChangeBreakPointEvent += BreakPointManager_ChangeBreakPointEvent;
            this.breakPointManager.UpdateBreakPointEvent += BreakPointManager_UpdateBreakPointEvent;
        }

        void BreakPointManager_UpdateBreakPointEvent(object sender, UpdateBreakPointArgs e)
        {
            int index = ItemIndex(e.FileFullPath, e.OldLine);
            if (index >= 0)
            {
                dgv.Rows[index].Cells["Line"].Value = e.NewLine.ToString();
            }
        }

        void BreakPointManager_ChangeBreakPointEvent(object sender, BreakPointArgs e)
        {
            if (e.IsDelete) DeleteItem(e.FileFullPath, e.Line + 1);            
            else AddItem(e.FileFullPath, e.Line + 1, e.Exp, e.Enable);
        }

        void Init()
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
            dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle {Padding = new Padding(1)};
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
            dgv.Columns.AddRange(ColumnBreakPointEnable, ColumnBreakPointFilePath, ColumnBreakPointFileName, ColumnBreakPointLine, ColumnBreakPointExp);
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.Width = ScaleHelper.Scale(column.Width);
            }
            defaultColor = dgv.Rows[dgv.Rows.Add()].DefaultCellStyle.BackColor;
            dgv.Rows.Clear();
            dgv.CellEndEdit += Dgv_CellEndEdit;
            dgv.CellMouseUp += Dgv_CellMouseUp;
            dgv.CellDoubleClick += Dgv_CellDoubleClick;
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

        void Dgv_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            breakPointManager.ChangeBreakPointEvent -= BreakPointManager_ChangeBreakPointEvent;
            if (e.RowIndex < 0 || e.RowIndex >= dgv.Rows.Count) return;
            if (dgv.Rows[e.RowIndex].Cells["Enable"].ColumnIndex == e.ColumnIndex)
            {
                var row = dgv.Rows[e.RowIndex];
                var line = int.Parse((string)row.Cells["Line"].Value) - 1;
                var value = (bool)row.Cells["Enable"].EditedFormattedValue;
                var filefullpath = (string)row.Cells["FilePath"].Value;
                var doc = ScintillaHelper.GetDocument(filefullpath);
                if (doc != null)
                {
                    // This logic should be handled by BPManager, so we'll just work arround bad BPs and ignore them
                    var sci = doc.SciControl;
                    if (line < 0 || (sci != null && line >= sci.LineCount)) return;
                    var marker = value ? ScintillaHelper.markerBPEnabled : ScintillaHelper.markerBPDisabled;
                    if (!ScintillaHelper.IsMarkerSet(sci, marker, line))
                    {
                        sci.MarkerAdd(line, marker);
                        sci.MarkerDelete(line, value ? ScintillaHelper.markerBPDisabled : ScintillaHelper.markerBPEnabled);
                    }
                }
                else breakPointManager.SetBreakPointInfo(filefullpath, line, false, value);
            }
            breakPointManager.ChangeBreakPointEvent += BreakPointManager_ChangeBreakPointEvent;
        }

        void Dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgv.Rows[e.RowIndex];
            if ((row.Cells["FilePath"].ColumnIndex == e.ColumnIndex)
                || (row.Cells["FileName"].ColumnIndex == e.ColumnIndex)
                || (row.Cells["Line"].ColumnIndex == e.ColumnIndex)) 
            {
                var filename = (string)row.Cells["FilePath"].Value;
                var line = int.Parse((string)row.Cells["Line"].Value);
                ScintillaHelper.ActivateDocument(filename, line - 1, true);
            }
        }

        void Dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgv.Rows[e.RowIndex].Cells["Exp"].ColumnIndex != e.ColumnIndex) return;
            dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = defaultColor;
            var filename = (string)dgv.Rows[e.RowIndex].Cells["FilePath"].Value;
            var line = int.Parse((string)dgv.Rows[e.RowIndex].Cells["Line"].Value);
            var exp = (string)dgv.Rows[e.RowIndex].Cells["Exp"].Value;
            breakPointManager.SetBreakPointCondition(filename, line - 1, exp);
        }

        public void Clear() => dgv.Rows.Clear();

        public new bool Enabled
        {
            get => dgv.Enabled;
            set => dgv.Enabled = value;
        }

        void AddItem(string filename, int line, string exp, bool enable)
        {
            int i = ItemIndex(filename, line);
            var dgvrow = i >= 0 ? dgv.Rows[i] : dgv.Rows[dgv.Rows.Add()];
            dgvrow.Cells["Enable"].Value = enable;
            dgvrow.Cells["FilePath"].Value = filename;
            dgvrow.Cells["FileName"].Value = Path.GetFileName(filename);
            dgvrow.Cells["Line"].Value = line.ToString();
            dgvrow.Cells["Exp"].Value = exp;
        }

        void DeleteItem(string filename, int line)
        {
            int i = ItemIndex(filename, line);
            if (i >= 0) dgv.Rows.RemoveAt(i);
        }

        int ItemIndex(string filename, int line)
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

        void InitializeComponent()
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

        void InitializeLocalization()
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

        void TsbRemoveSelected_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedCells.Count == 0) return;
            breakPointManager.ChangeBreakPointEvent -= BreakPointManager_ChangeBreakPointEvent;
            var processedRows = new HashSet<DataGridViewRow>();
            foreach (DataGridViewCell selectedCell in dgv.SelectedCells)
            {
                var selected = selectedCell.OwningRow;
                if (processedRows.Contains(selected)) continue;
                processedRows.Add(selected);
                var filefullpath = (string)selected.Cells["FilePath"].Value;
                var line = int.Parse((string)selected.Cells["Line"].Value) - 1;
                var doc = ScintillaHelper.GetDocument(filefullpath);
                if (doc != null)
                {
                    bool m = ScintillaHelper.IsMarkerSet(doc.SciControl, ScintillaHelper.markerBPDisabled, line);
                    doc.SciControl.MarkerDelete(line, m ? ScintillaHelper.markerBPDisabled : ScintillaHelper.markerBPEnabled);
                }
                else breakPointManager.SetBreakPointInfo(filefullpath, line, true, false);
                dgv.Rows.Remove(selected);
            }
            breakPointManager.ChangeBreakPointEvent += BreakPointManager_ChangeBreakPointEvent;
            breakPointManager.Save();
        }

        void TsbRemoveFiltered_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0) return;
            breakPointManager.ChangeBreakPointEvent -= BreakPointManager_ChangeBreakPointEvent;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                var filefullpath = (string)row.Cells["FilePath"].Value;
                var line = int.Parse((string)row.Cells["Line"].Value) - 1;
                var doc = ScintillaHelper.GetDocument(filefullpath);
                if (doc != null)
                {
                    var m = ScintillaHelper.IsMarkerSet(doc.SciControl, ScintillaHelper.markerBPDisabled, line);
                    doc.SciControl.MarkerDelete(line, m ? ScintillaHelper.markerBPDisabled : ScintillaHelper.markerBPEnabled);
                }
                else breakPointManager.SetBreakPointInfo(filefullpath, line, true, false);
            }
            dgv.Rows.Clear();
            breakPointManager.ChangeBreakPointEvent += BreakPointManager_ChangeBreakPointEvent;
            breakPointManager.Save();
        }

        void TsbAlternateFiltered_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0) return;
            breakPointManager.ChangeBreakPointEvent -= BreakPointManager_ChangeBreakPointEvent;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                var filefullpath = (string)row.Cells["FilePath"].Value;
                var line = int.Parse((string)row.Cells["Line"].Value) - 1;
                var value = !(bool) row.Cells["Enable"].Value;
                var doc = ScintillaHelper.GetDocument(filefullpath);
                if (doc != null)
                {
                    int newMarker = value ? ScintillaHelper.markerBPEnabled : ScintillaHelper.markerBPDisabled;
                    if (!ScintillaHelper.IsMarkerSet(doc.SciControl, newMarker, line))
                    {
                        doc.SciControl.MarkerAdd(line, newMarker);
                        doc.SciControl.MarkerDelete(line, value ? ScintillaHelper.markerBPDisabled : ScintillaHelper.markerBPEnabled);
                    }
                }
                else breakPointManager.SetBreakPointInfo(filefullpath, line, false, value);
                row.Cells["Enable"].Value = value;
            }
            dgv.EndEdit();
            breakPointManager.ChangeBreakPointEvent += BreakPointManager_ChangeBreakPointEvent;
            breakPointManager.Save();
        }

        void TsbExportFiltered_Click(object sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog();
            dialog.OverwritePrompt = true;
            dialog.Filter = TextHelper.GetString("ProjectManager.Info.FileFilter");
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                breakPointManager.Save(dialog.FileName);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        void TsbImport_Click(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Filter = TextHelper.GetString("ProjectManager.Info.FileFilter");
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                breakPointManager.Import(dialog.FileName);
                breakPointManager.SetBreakPointsToEditor(PluginBase.MainForm.Documents);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowWarning(TextHelper.GetString("Error.InvalidFile"), ex);
            }
        }

        void TstxtFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            try
            {
                var regex = new Regex(tstxtFilter.Text, RegexOptions.IgnoreCase);
                var rows = dgv.Rows.OfType<DataGridViewRow>().ToArray();
                dgv.Rows.Clear();
                foreach (var row in rows)
                {
                    if (tstxtFilter.Text.Length == 0) row.Visible = true;
                    else
                    {
                        var matches = false;
                        var cells = tscbFilterColumns.SelectedIndex == 0 ? row.Cells : (IEnumerable)new[] { row.Cells[tscbFilterColumns.SelectedIndex] };
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