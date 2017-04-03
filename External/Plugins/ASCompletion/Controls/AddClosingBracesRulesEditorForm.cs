﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ASCompletion.Completion;
using PluginCore;
using PluginCore.Helpers;

namespace ASCompletion.Controls
{
    public partial class AddClosingBracesRulesEditorForm : Form, IServiceProvider, ITypeDescriptorContext, IWindowsFormsEditorService
    {
        private ListBox listBox;
        private TextBox txtName;
        private TextBox txtOpenChar;
        private TextBox txtCloseChar;
        private DataGridView rulesGridView;
        private DataGridViewCheckBoxColumn not1;
        private DataGridViewTextBoxColumn afterChars;
        private DataGridViewButtonColumn logic1;
        private DataGridViewCheckBoxColumn not2;
        private DataGridViewStylesColumn afterStyles;
        private DataGridViewButtonColumn logic2;
        private DataGridViewCheckBoxColumn not3;
        private DataGridViewTextBoxColumn beforeChars;
        private DataGridViewButtonColumn logic3;
        private DataGridViewCheckBoxColumn not4;
        private DataGridViewStylesColumn beforeStyles;
        private DataGridViewImageColumn colDelete;
        private Button btnUp;
        private Button btnDown;
        private Button btnAdd;
        private Button btnRemove;
        private Button btnAddRule;
        private Button btnOk;
        private Button btnCancel;

        private Brace[] value;
        private BraceInEdit inEdit;

        public AddClosingBracesRulesEditorForm(Brace[] value)
        {
            InitializeComponent();
            InitializeGraphics();
            InitializeFonts();
            InitializeListBox(value);
            InitializeGridView();
            ValidateControlStates();
            this.value = value;
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            listBox = new ListBox();
            var lblName = new Label();
            txtName = new TextBox();
            var lblOpenChar = new Label();
            txtOpenChar = new TextBox();
            var lblCloseChar = new Label();
            txtCloseChar = new TextBox();
            rulesGridView = new DataGridView();
            not1 = new DataGridViewCheckBoxColumn();
            afterChars = new DataGridViewTextBoxColumn();
            logic1 = new DataGridViewButtonColumn();
            not2 = new DataGridViewCheckBoxColumn();
            afterStyles = new DataGridViewStylesColumn();
            logic2 = new DataGridViewButtonColumn();
            not3 = new DataGridViewCheckBoxColumn();
            beforeChars = new DataGridViewTextBoxColumn();
            logic3 = new DataGridViewButtonColumn();
            not4 = new DataGridViewCheckBoxColumn();
            beforeStyles = new DataGridViewStylesColumn();
            colDelete = new DataGridViewImageColumn();
            btnUp = new Button();
            btnDown = new Button();
            btnAdd = new Button();
            btnRemove = new Button();
            btnAddRule = new Button();
            btnOk = new Button();
            btnCancel = new Button();
            ((ISupportInitialize) (rulesGridView)).BeginInit();
            SuspendLayout();
            // 
            // listBox
            // 
            listBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listBox.IntegralHeight = false;
            listBox.ItemHeight = 16;
            listBox.Location = new Point(12, 12);
            listBox.Name = "listBox";
            listBox.Size = new Size(156, 243);
            listBox.TabIndex = 0;
            listBox.SelectedValueChanged += ListBox_SelectedValueChanged;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new Point(207, 15);
            lblName.Name = "lblName";
            lblName.Size = new Size(53, 17);
            lblName.Text = "Name: ";
            // 
            // txtName
            // 
            txtName.Location = new Point(266, 12);
            txtName.MaxLength = 32;
            txtName.Name = "txtName";
            txtName.Size = new Size(200, 22);
            txtName.TabIndex = 1;
            txtName.TextChanged += TxtName_TextChanged;
            // 
            // lblOpenChar
            // 
            lblOpenChar.AutoSize = true;
            lblOpenChar.Location = new Point(472, 15);
            lblOpenChar.Name = "lblOpenChar";
            lblOpenChar.Size = new Size(51, 17);
            lblOpenChar.Text = "Open: ";
            // 
            // txtOpenChar
            // 
            txtOpenChar.Location = new Point(529, 12);
            txtOpenChar.MaxLength = 1;
            txtOpenChar.Name = "txtOpenChar";
            txtOpenChar.Size = new Size(30, 22);
            txtOpenChar.TabIndex = 2;
            txtOpenChar.TextAlign = HorizontalAlignment.Center;
            txtOpenChar.TextChanged += TxtOpenChar_TextChanged;
            // 
            // lblCloseChar
            // 
            lblCloseChar.AutoSize = true;
            lblCloseChar.Location = new Point(565, 15);
            lblCloseChar.Name = "lblCloseChar";
            lblCloseChar.Size = new Size(51, 17);
            lblCloseChar.Text = "Close: ";
            // 
            // txtCloseChar
            // 
            txtCloseChar.Location = new Point(622, 12);
            txtCloseChar.MaxLength = 1;
            txtCloseChar.Name = "txtCloseChar";
            txtCloseChar.Size = new Size(30, 22);
            txtCloseChar.TabIndex = 3;
            txtCloseChar.TextAlign = HorizontalAlignment.Center;
            txtCloseChar.TextChanged += TxtCloseChar_TextChanged;
            // 
            // rulesGridView
            // 
            rulesGridView.AllowUserToAddRows = false;
            rulesGridView.AllowUserToDeleteRows = false;
            rulesGridView.AllowUserToResizeColumns = false;
            rulesGridView.AllowUserToResizeRows = false;
            rulesGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rulesGridView.BackgroundColor = SystemColors.ControlLight;
            rulesGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            rulesGridView.Columns.AddRange(new DataGridViewColumn[]
            {
                not1,
                afterChars,
                logic1,
                not2,
                afterStyles,
                logic2,
                not3,
                beforeChars,
                logic3,
                not4,
                beforeStyles,
                colDelete
            });
            rulesGridView.Location = new Point(210, 40);
            rulesGridView.Name = "rulesGridView";
            rulesGridView.RowTemplate.Height = 24;
            rulesGridView.ScrollBars = ScrollBars.Vertical;
            rulesGridView.Size = new Size(760, 265);
            rulesGridView.TabIndex = 4;
            rulesGridView.CellContentClick += RulesGridView_CellContentClick;
            rulesGridView.CellValueChanged += RulesGridView_CellValueChanged;
            // 
            // not1
            // 
            not1.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            not1.HeaderText = "Not";
            not1.Name = "not1";
            not1.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // afterChars
            // 
            afterChars.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            afterChars.HeaderText = "After";
            afterChars.Name = "afterChars";
            afterChars.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // logic1
            // 
            logic1.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            logic1.HeaderText = "";
            logic1.Name = "logic1";
            logic1.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // not2
            // 
            not2.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            not2.HeaderText = "Not";
            not2.Name = "not2";
            not2.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // afterStyles
            // 
            afterStyles.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            afterStyles.HeaderText = "After";
            afterStyles.Name = "afterStyles";
            afterStyles.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // logic2
            // 
            logic2.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            logic2.HeaderText = "";
            logic2.Name = "logic2";
            logic2.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // not3
            // 
            not3.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            not3.HeaderText = "Not";
            not3.Name = "not3";
            not3.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // beforeChars
            // 
            beforeChars.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            beforeChars.HeaderText = "Before";
            beforeChars.Name = "beforeChars";
            beforeChars.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // logic3
            // 
            logic3.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            logic3.HeaderText = "";
            logic3.Name = "logic3";
            logic3.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // not4
            // 
            not4.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            not4.HeaderText = "Not";
            not4.Name = "not4";
            not4.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // beforeStyles
            // 
            beforeStyles.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            beforeStyles.HeaderText = "Before";
            beforeStyles.Name = "beforeStyles";
            beforeStyles.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colDelete
            // 
            colDelete.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            colDelete.HeaderText = "";
            colDelete.Name = "colDelete";
            colDelete.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // btnUp
            // 
            btnUp.Location = new Point(174, 42);
            btnUp.Name = "btnUp";
            btnUp.Size = new Size(30, 30);
            btnUp.TabIndex = 5;
            btnUp.UseVisualStyleBackColor = true;
            btnUp.Click += BtnUp_Click;
            // 
            // btnDown
            // 
            btnDown.Location = new Point(174, 78);
            btnDown.Name = "btnDown";
            btnDown.Size = new Size(30, 30);
            btnDown.TabIndex = 6;
            btnDown.UseVisualStyleBackColor = true;
            btnDown.Click += BtnDown_Click;
            // 
            // btnAdd
            // 
            btnAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAdd.Location = new Point(12, 261);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 30);
            btnAdd.TabIndex = 7;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += BtnAdd_Click;
            // 
            // btnRemove
            // 
            btnRemove.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRemove.Location = new Point(93, 261);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(75, 30);
            btnRemove.TabIndex = 8;
            btnRemove.Text = "Remove";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += BtnRemove_Click;
            // 
            // btnAddRule
            // 
            btnAddRule.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddRule.Location = new Point(210, 311);
            btnAddRule.Name = "btnAddRule";
            btnAddRule.Size = new Size(30, 30);
            btnAddRule.TabIndex = 9;
            btnAddRule.Text = "";
            btnAddRule.UseVisualStyleBackColor = true;
            btnAddRule.Click += BtnAddRule_Click;
            // 
            // btnOk
            // 
            btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(724, 311);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(120, 30);
            btnOk.TabIndex = 10;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += BtnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(850, 311);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(120, 30);
            btnCancel.TabIndex = 11;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += BtnCancel_Click;
            // 
            // AddClosingBracesRulesEditor
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(982, 353);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(btnAddRule);
            Controls.Add(btnRemove);
            Controls.Add(btnAdd);
            Controls.Add(btnDown);
            Controls.Add(btnUp);
            Controls.Add(rulesGridView);
            Controls.Add(txtCloseChar);
            Controls.Add(lblCloseChar);
            Controls.Add(txtOpenChar);
            Controls.Add(lblOpenChar);
            Controls.Add(txtName);
            Controls.Add(lblName);
            Controls.Add(listBox);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimumSize = new Size(900, 300);
            Name = "AddClosingBracesRulesEditor";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "AddClosingBracesRulesEditor";
            ((ISupportInitialize) (rulesGridView)).EndInit();
            ResumeLayout(false);
        }

        #endregion

        #region Initialization

        private void InitializeGraphics()
        {
            colDelete.Image = PluginBase.MainForm.FindImage16("153", false);
            btnUp.Image = PluginBase.MainForm.FindImage16("74", false);
            btnDown.Image = PluginBase.MainForm.FindImage16("60", false);
            btnAddRule.Image = PluginBase.MainForm.FindImage16("33", false); // Seriously no plus icon?
        }

        private void InitializeFonts()
        {
            afterChars.DefaultCellStyle.Font = PluginBase.Settings.ConsoleFont;
            beforeChars.DefaultCellStyle.Font = PluginBase.Settings.ConsoleFont;
            txtOpenChar.Font = PluginBase.Settings.ConsoleFont;
            txtCloseChar.Font = PluginBase.Settings.ConsoleFont;
        }

        private void InitializeListBox(Brace[] value)
        {
            listBox.GetType().GetProperty(nameof(DoubleBuffered), BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listBox, true, null);
            listBox.BeginUpdate();
            foreach (var brace in value)
            {
                listBox.Items.Add(new BraceInEdit(brace));
            }
            listBox.EndUpdate();
        }

        private void InitializeGridView()
        {
            rulesGridView.GetType().GetProperty(nameof(DoubleBuffered), BindingFlags.NonPublic | BindingFlags.Instance).SetValue(rulesGridView, true, null);
            rulesGridView.RowTemplate = new DataGridViewRow()
            {
                Height = ScaleHelper.Scale(30)
            };

            not1.ValueType = typeof(bool);
            not2.ValueType = typeof(bool);
            not3.ValueType = typeof(bool);
            not4.ValueType = typeof(bool);
            not1.FalseValue = false;
            not2.FalseValue = false;
            not3.FalseValue = false;
            not4.FalseValue = false;
            not1.TrueValue = true;
            not2.TrueValue = true;
            not3.TrueValue = true;
            not4.TrueValue = true;

            logic1.ValueType = typeof(Brace.Logic);
            logic2.ValueType = typeof(Brace.Logic);
            logic3.ValueType = typeof(Brace.Logic);

            afterStyles.ValueType = typeof(Style[]);
            beforeStyles.ValueType = typeof(Style[]);
        }

        private void ValidateControlStates()
        {
            listBox.SelectedIndex = 0;
        }

        #endregion

        public Brace[] Value
        {
            get { return value; }
        }
        
        private void ShowRules()
        {
            txtName.Text = inEdit.Name;
            txtOpenChar.Text = inEdit.Open == '\0' ? "" : inEdit.Open.ToString();
            txtCloseChar.Text = inEdit.Close == '\0' ? "" : inEdit.Close.ToString();

            rulesGridView.Rows.Clear();
            foreach (var rule in inEdit.Rules)
            {
                rulesGridView.Rows.Add(CreateRow(rule));
            }
        }

        private object[] CreateRow(Brace.Rule rule)
        {
            return new object[]
            {
                rule.NotAfterChars, rule.AfterChars, rule.Logic1,
                rule.NotAfterStyles, rule.AfterStyles, rule.Logic2,
                rule.NotBeforeChars, rule.BeforeChars, rule.Logic3,
                rule.NotBeforeStyles, rule.BeforeStyles
            };
        }

        private Style[] EditStyles(Style[] value)
        {
            return (Style[]) new ArrayEditor(typeof(Style[])).EditValue(this, this, value);
        }

        private void InvalidateListBox()
        {
            listBox.DisplayMember = "Invalidate";
            listBox.DisplayMember = "";
        }

        #region Event Handlers

        private void ListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            inEdit = null;

            if (listBox.SelectedItem == null)
            {
                if (listBox.Items.Count == 0)
                {
                    txtName.Enabled = false;
                    txtOpenChar.Enabled = false;
                    txtCloseChar.Enabled = false;
                    btnUp.Enabled = false;
                    btnDown.Enabled = false;
                    btnRemove.Enabled = false;
                    rulesGridView.Enabled = false;
                }
                else
                {
                    listBox.SelectedIndex = 0;
                }
            }
            else
            {
                txtName.Enabled = true;
                txtOpenChar.Enabled = true;
                txtCloseChar.Enabled = true;
                btnUp.Enabled = listBox.SelectedIndex > 0;
                btnDown.Enabled = listBox.SelectedIndex < listBox.Items.Count - 1;
                btnRemove.Enabled = true;
                rulesGridView.Enabled = true;

                inEdit = (BraceInEdit) listBox.SelectedItem;
                ShowRules();
            }
        }

        private void TxtName_TextChanged(object sender, EventArgs e)
        {
            inEdit.Name = txtName.Text;
            InvalidateListBox();
        }

        private void TxtOpenChar_TextChanged(object sender, EventArgs e)
        {
            if (txtOpenChar.TextLength == 1 && !char.IsWhiteSpace(txtOpenChar.Text[0]))
            {
                inEdit.Open = txtOpenChar.Text[0];
            }
            else
            {
                inEdit.Open = '\0';
            }
            InvalidateListBox();
        }

        private void TxtCloseChar_TextChanged(object sender, EventArgs e)
        {
            if (txtCloseChar.TextLength == 1 && !char.IsWhiteSpace(txtCloseChar.Text[0]))
            {
                inEdit.Close = txtCloseChar.Text[0];
            }
            else
            {
                inEdit.Close = '\0';
            }
            InvalidateListBox();
        }

        private void BtnUp_Click(object sender, EventArgs e)
        {
            int index = listBox.SelectedIndex;
            object itemAbove = listBox.Items[index - 1];
            listBox.Items.RemoveAt(index - 1);
            listBox.Items.Insert(index, itemAbove);
            btnUp.Enabled = index - 1 > 0;
            btnDown.Enabled = true;
            txtName.Select();
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            int index = listBox.SelectedIndex;
            object itemBelow = listBox.Items[index + 1];
            listBox.Items.RemoveAt(index + 1);
            listBox.Items.Insert(index, itemBelow);
            btnDown.Enabled = index + 1 < listBox.Items.Count - 1;
            btnUp.Enabled = true;
            txtName.Select();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox.SelectedIndex;
            listBox.Items.Insert(++selectedIndex, new BraceInEdit());
            listBox.SelectedIndex = selectedIndex;
            listBox.Select();
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox.SelectedIndex;
            listBox.Items.RemoveAt(selectedIndex--);
            listBox.SelectedIndex = selectedIndex;
            listBox.Select();
        }

        private void BtnAddRule_Click(object sender, EventArgs e)
        {
            var newRule = new Brace.Rule();
            inEdit.Rules.Add(newRule);
            rulesGridView.Rows.Add(CreateRow(newRule));
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            int count = 0;
            value = new Brace[listBox.Items.Count];

            foreach (BraceInEdit braceInEdit in listBox.Items)
            {
                if (braceInEdit.Open != '\0' && braceInEdit.Close != '\0' && braceInEdit.Rules.Count > 0)
                {
                    value[count++] = new Brace(braceInEdit.Name, braceInEdit.Open, braceInEdit.Close, braceInEdit.Rules.ToArray());
                }
            }

            Array.Resize(ref value, count);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {

        }

        private void RulesGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            else if (e.ColumnIndex == logic1.Index)
            {
                inEdit.Rules[e.RowIndex].Logic1 ^= Brace.Logic.And;
                rulesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = inEdit.Rules[e.RowIndex].Logic1;
            }
            else if (e.ColumnIndex == logic2.Index)
            {
                inEdit.Rules[e.RowIndex].Logic2 ^= Brace.Logic.And;
                rulesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = inEdit.Rules[e.RowIndex].Logic2;
            }
            else if (e.ColumnIndex == logic3.Index)
            {
                inEdit.Rules[e.RowIndex].Logic3 ^= Brace.Logic.And;
                rulesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = inEdit.Rules[e.RowIndex].Logic3;
            }
            else if (e.ColumnIndex == afterStyles.Index)
            {
                inEdit.Rules[e.RowIndex].AfterStyles = EditStyles(inEdit.Rules[e.RowIndex].AfterStyles);
                rulesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = inEdit.Rules[e.RowIndex].AfterStyles;
            }
            else if (e.ColumnIndex == beforeStyles.Index)
            {
                inEdit.Rules[e.RowIndex].BeforeStyles = EditStyles(inEdit.Rules[e.RowIndex].BeforeStyles);
                rulesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = inEdit.Rules[e.RowIndex].BeforeStyles;
            }
            else if (e.ColumnIndex == colDelete.Index)
            {
                inEdit.Rules.RemoveAt(e.RowIndex);
                rulesGridView.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void RulesGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            else if (e.ColumnIndex == not1.Index)
            {
                inEdit.Rules[e.RowIndex].NotAfterChars = (bool) rulesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            }
            else if (e.ColumnIndex == not2.Index)
            {
                inEdit.Rules[e.RowIndex].NotAfterStyles = (bool) rulesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            }
            else if (e.ColumnIndex == not3.Index)
            {
                inEdit.Rules[e.RowIndex].NotBeforeChars = (bool) rulesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            }
            else if (e.ColumnIndex == not4.Index)
            {
                inEdit.Rules[e.RowIndex].NotBeforeStyles = (bool) rulesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            }
            else if (e.ColumnIndex == afterChars.Index)
            {
                inEdit.Rules[e.RowIndex].AfterChars = (string) rulesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            }
            else if (e.ColumnIndex == beforeChars.Index)
            {
                inEdit.Rules[e.RowIndex].BeforeChars = (string) rulesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            }
        }

        #endregion

        #region IServiceProvider, ITypeDescriptorContext, IWindowsFormsEditorService

        IContainer ITypeDescriptorContext.Container
        {
            get { return null; }
        }

        object ITypeDescriptorContext.Instance
        {
            get { return null; }
        }

        PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
        {
            get { return null; }
        }

        void IWindowsFormsEditorService.CloseDropDown()
        {

        }

        void IWindowsFormsEditorService.DropDownControl(Control control)
        {

        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return serviceType == typeof(IWindowsFormsEditorService) ? this : null;
        }

        void ITypeDescriptorContext.OnComponentChanged()
        {

        }

        bool ITypeDescriptorContext.OnComponentChanging()
        {
            return true;
        }

        DialogResult IWindowsFormsEditorService.ShowDialog(Form dialog)
        {
            return dialog.ShowDialog(this);
        }

        #endregion

        private sealed class BraceInEdit
        {
            public string Name;
            public char Open;
            public char Close;
            public List<Brace.Rule> Rules;

            public BraceInEdit()
            {
                Name = "";
                Open = '\0';
                Close = '\0';
                Rules = new List<Brace.Rule>();
            }

            public BraceInEdit(Brace value)
            {
                Name = value.Name;
                Open = value.Open;
                Close = value.Close;
                Rules = new List<Brace.Rule>(value.Rules.Length);
                foreach (var rule in value.Rules)
                {
                    Rules.Add(rule.Clone());
                }
            }

            public override string ToString()
            {
                return Open == '\0' || Close == '\0' ? "New" : Open + " " + Name + " " + Close;
            }
        }

        private sealed class DataGridViewStylesColumn : DataGridViewColumn
        {
            public DataGridViewStylesColumn() : base(new DataGridViewStylesCell())
            {
                
            }
        }

        private sealed class DataGridViewStylesCell : DataGridViewButtonCell
        {
            protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
            {
                int length = ((Style[]) value).Length;
                return length + " " + (length == 1 ? "Style" : "Styles");
            }

            protected override bool SetValue(int rowIndex, object value)
            {
                if (value == null || ((Style[]) value).Length == 0)
                {
                    ToolTipText = "{ }";
                }
                else
                {
                    string[] styles = Array.ConvertAll((Style[]) value, (style) => style.ToString());
                    ToolTipText = "{ " + string.Join(", ", styles) + " }";
                }
                return base.SetValue(rowIndex, value);
            }
        }
    }
}
