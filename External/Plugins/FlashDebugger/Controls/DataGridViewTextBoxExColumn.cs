using System;
using System.Drawing;
using System.Windows.Forms;
using PluginCore.Controls;

namespace FlashDebugger.Controls
{
    public class DataGridViewTextBoxExColumn : DataGridViewColumn
    {

        public override DataGridViewCell CellTemplate
        {
            get { return base.CellTemplate; }
            set
            {
                if (value != null && !(value is DataGridViewTextBoxExCell))
                {
                    throw new InvalidCastException("Cell type is not based upon the DataGridViewTextBoxExCell");
                }
                base.CellTemplate = value;
            }
        }

        [System.ComponentModel.DefaultValue(32767)]
        public int MaxInputLength
        {
            get
            {
                return ((DataGridViewTextBoxExCell)CellTemplate).MaxInputLength;
            }
            set
            {
                if (MaxInputLength != value)
                {
                    ((DataGridViewTextBoxExCell)CellTemplate).MaxInputLength = value;
                    if (DataGridView != null)
                    {
                        DataGridViewRowCollection rows = DataGridView.Rows;
                        int count = rows.Count;
                        for (int i = 0; i < count; i++)
                        {
                            DataGridViewRow dataGridViewRow = rows.SharedRow(i);
                            var item = dataGridViewRow.Cells[Index] as DataGridViewTextBoxExCell;
                            if (item != null)
                            {
                                item.MaxInputLength = value;
                            }
                        }
                    }
                }
            }
        }

        public DataGridViewTextBoxExColumn()
            : base(new DataGridViewTextBoxExCell())
        {
            SortMode = DataGridViewColumnSortMode.Automatic;
        }
    }

    public class DataGridViewTextBoxExCell : DataGridViewTextBoxCell
    {

        private TextBox editingControl;

        public override Type EditType
        {
            get { return typeof(DataGridViewTextBoxExEditingControl); }
        }

        private int maxInputLength = 32767;
        public override int MaxInputLength
        {
            get { return maxInputLength; }
            set
            {
                if (maxInputLength == value) return;

                maxInputLength = value;
                if (editingControl != null && RowIndex == ((IDataGridViewEditingControl)editingControl).EditingControlRowIndex)
                    editingControl.MaxLength = value;
            }
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            var ctl = DataGridView.EditingControl as TextBox;
            if (ctl != null)
            {
                ctl.MaxLength = maxInputLength;
                ctl.AcceptsReturn = ctl.Multiline = dataGridViewCellStyle.WrapMode == DataGridViewTriState.True;
                ctl.BorderStyle = BorderStyle.None;
                if (initialFormattedValue != null) ctl.Text = (string)initialFormattedValue;
                editingControl = ctl;
            }
        }

        public override void DetachEditingControl()
        {
            base.DetachEditingControl();
            editingControl = null;
        }

    }

    public class DataGridViewTextBoxExEditingControl : TextBoxEx, IDataGridViewEditingControl
    {

        private static readonly DataGridViewContentAlignment anyRight = DataGridViewContentAlignment.BottomRight |
                                                                        DataGridViewContentAlignment.MiddleRight |
                                                                        DataGridViewContentAlignment.TopRight;

        private static readonly DataGridViewContentAlignment anyCenter = DataGridViewContentAlignment.BottomCenter |
                                                                         DataGridViewContentAlignment.MiddleCenter |
                                                                         DataGridViewContentAlignment.TopCenter;

        private static readonly DataGridViewContentAlignment anyTop = DataGridViewContentAlignment.TopCenter |
                                                                      DataGridViewContentAlignment.TopLeft |
                                                                      DataGridViewContentAlignment.TopRight;

        private Keys lastInputKey;
        private bool repositionOnValueChange;

        public DataGridViewTextBoxExEditingControl()
        {
            TabStop = false;
        }

        #region IDataGridViewEditingControl Members

        public bool RepositionEditingControlOnValueChange
        {
            get { return repositionOnValueChange; }
        }

        public DataGridView EditingControlDataGridView { get; set; }

        public object EditingControlFormattedValue
        {
            get { return Text; }
            set { Text = (string)value; }
        }

        public int EditingControlRowIndex { get; set; }

        public bool EditingControlValueChanged { get; set; }

        public Cursor EditingPanelCursor
        {
            get { return Cursors.Default; }
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
            if (dataGridViewCellStyle.BackColor.A < 255)
            {
                Color color = Color.FromArgb(255, dataGridViewCellStyle.BackColor);
                BackColor = color;
                EditingControlDataGridView.EditingPanel.BackColor = color;
            }
            else
            {
                BackColor = dataGridViewCellStyle.BackColor;
            }
            ForeColor = dataGridViewCellStyle.ForeColor;
            if (dataGridViewCellStyle.WrapMode == DataGridViewTriState.True)
            {
                WordWrap = true;
            }
            TextAlign = TranslateAlignment(dataGridViewCellStyle.Alignment);
            repositionOnValueChange = dataGridViewCellStyle.WrapMode == DataGridViewTriState.True &&
                (dataGridViewCellStyle.Alignment & anyTop) == 0;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            if (keyData == lastInputKey) return true;
            Keys key = keyData & Keys.KeyCode;
            if (key != Keys.Return)
            {
                switch (key)
                {
                    case Keys.Prior:
                    case Keys.Next:
                        if (!EditingControlValueChanged)
                        {
                            break;
                        }
                        return true;
                    case Keys.End:
                    case Keys.Home:
                        if (this.SelectionLength == this.Text.Length)
                        {
                            break;
                        }
                        return true;
                    case Keys.Left:
                        if ((this.RightToLeft != RightToLeft.No || this.SelectionLength == 0 && base.SelectionStart == 0) && (this.RightToLeft != RightToLeft.Yes || this.SelectionLength == 0 && base.SelectionStart == this.Text.Length))
                        {
                            break;
                        }
                        return true;
                    case Keys.Up:
                        if (this.Text.IndexOf("\r\n") < 0 || base.SelectionStart + this.SelectionLength < this.Text.IndexOf("\r\n"))
                        {
                            break;
                        }
                        return true;
                    case Keys.Right:
                        if ((this.RightToLeft != RightToLeft.No || this.SelectionLength == 0 && base.SelectionStart == this.Text.Length) && (this.RightToLeft != RightToLeft.Yes || this.SelectionLength == 0 && base.SelectionStart == 0))
                        {
                            break;
                        }
                        return true;
                    case Keys.Down:
                        int selectionStart = base.SelectionStart + this.SelectionLength;
                        if (this.Text.IndexOf("\r\n", selectionStart) == -1)
                        {
                            break;
                        }
                        return true;
                    case Keys.Delete:
                        if (this.SelectionLength <= 0 && base.SelectionStart >= this.Text.Length)
                        {
                            break;
                        }
                        return true;
                }
            }
            else if ((keyData & (Keys.Shift | Keys.Control | Keys.Alt)) == Keys.Shift && this.Multiline && base.AcceptsReturn)
            {
                return true;
            }
            return !dataGridViewWantsInputKey;
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return Text;
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            if (selectAll)
                SelectAll();
            else
                SelectionStart = Text.Length;
        }

        #endregion

        private void NotifyDataGridViewOfValueChange()
        {
            EditingControlValueChanged = true;
            EditingControlDataGridView.NotifyCurrentCellDirty(true);
        }

        private static HorizontalAlignment TranslateAlignment(DataGridViewContentAlignment align)
        {
            if ((align & anyRight) != 0)
                return HorizontalAlignment.Right;

            if ((align & anyCenter) != 0)
                return HorizontalAlignment.Center;

            return HorizontalAlignment.Left;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            NotifyDataGridViewOfValueChange();
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            lastInputKey = e.IsInputKey ? e.KeyData : Keys.None;
        }

        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            Keys wParam = (Keys)((int)m.WParam);
            if (wParam == Keys.LineFeed)
            {
                if (m.Msg == 258 && Control.ModifierKeys == Keys.Control && this.Multiline && base.AcceptsReturn)
                {
                    return true;
                }
            }
            else if (wParam != Keys.Return)
            {
                if (wParam == Keys.A)
                {
                    if (m.Msg == 256 && Control.ModifierKeys == Keys.Control)
                    {
                        base.SelectAll();
                        return true;
                    }
                }
            }
            else if (m.Msg == 258 && (Control.ModifierKeys != Keys.Shift || !this.Multiline || !base.AcceptsReturn))
            {
                return true;
            }
            return base.ProcessKeyEventArgs(ref m);
        }
    }

}
