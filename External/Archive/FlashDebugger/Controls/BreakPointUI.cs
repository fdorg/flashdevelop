using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using PluginCore.Localization;
using PluginCore;

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
        private Color defaultColor;

        public BreakPointUI(PluginMain pluginMain, BreakPointManager breakPointManager)
        {
            init();
            this.pluginMain = pluginMain;
            this.breakPointManager = breakPointManager;
            this.breakPointManager.ChangeBreakPointEvent += new ChangeBreakPointEventHandler(breakPointManager_ChangeBreakPointEvent);
            this.breakPointManager.UpdateBreakPointEvent += new UpdateBreakPointEventHandler(breakPointManager_UpdateBreakPointEvent);
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

            defaultColor = dgv.Rows[dgv.Rows.Add()].DefaultCellStyle.BackColor;
            dgv.Rows.Clear();

            this.dgv.CellEndEdit += new DataGridViewCellEventHandler(dgv_CellEndEdit);
            this.dgv.CellMouseUp += new DataGridViewCellMouseEventHandler(dgv_CellMouseUp);
            this.dgv.CellDoubleClick += new DataGridViewCellEventHandler(dgv_CellDoubleClick);
        }

        void dgv_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgv.Rows.Count) return;
            if (dgv.Rows[e.RowIndex].Cells["Enable"].ColumnIndex == e.ColumnIndex)
            {
                Boolean enable = !(Boolean)dgv.Rows[e.RowIndex].Cells["Enable"].Value;
                string filefullpath = (string)dgv.Rows[e.RowIndex].Cells["FilePath"].Value;
                int line = int.Parse((string)dgv.Rows[e.RowIndex].Cells["Line"].Value);
                ITabbedDocument doc = ScintillaHelper.GetDocument(filefullpath);
                if (doc != null)
                {
					// This logic should be handled by BPMAnager, wo we'll just work arround bad BPs and ignore them
					if (line < 1 || (doc.SciControl != null && line > doc.SciControl.LineCount)) return;
					Boolean m = ScintillaHelper.IsMarkerSet(doc.SciControl, ScintillaHelper.markerBPDisabled, line - 1);
					if (m)
					{
						doc.SciControl.MarkerAdd(line - 1, ScintillaHelper.markerBPEnabled);
						doc.SciControl.MarkerDelete(line - 1, ScintillaHelper.markerBPDisabled);
					}
					else
					{
						doc.SciControl.MarkerAdd(line - 1, ScintillaHelper.markerBPDisabled);
						doc.SciControl.MarkerDelete(line - 1, ScintillaHelper.markerBPEnabled);
					}
                    
                    if (e.RowIndex >= 0 && e.RowIndex < dgv.Rows.Count) // list can have been updated in the meantime
                    if ((Boolean)dgv.Rows[e.RowIndex].Cells["Enable"].Value != m)
                    {
                        dgv.Rows[e.RowIndex].Cells["Enable"].Value = m;
                    }
                    dgv.RefreshEdit();
                }
            }
        }

        void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgv.Rows[e.RowIndex].Cells["Line"].ColumnIndex == e.ColumnIndex)
            {
                string filename = (string)dgv.Rows[e.RowIndex].Cells["FilePath"].Value;
                int line = int.Parse((string)dgv.Rows[e.RowIndex].Cells["Line"].Value);
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
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                if ((string)dgv.Rows[i].Cells["FilePath"].Value == filename && (string)dgv.Rows[i].Cells["Line"].Value == line.ToString())
                {
                    return i;
                }
            }
            return -1;
        }

    }

}
