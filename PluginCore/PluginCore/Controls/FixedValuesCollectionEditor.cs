using System;
using System.Linq;
using System.Windows.Forms;

namespace PluginCore.Controls
{
    /// <summary>
    /// Provides a dialog that allows users to add/remove items from a fixed set of values to/from an array.
    /// </summary>
    /// <typeparam name="string">The element type of the array.</typeparam>
    public class FixedValuesCollectionEditor<T> : Form
    {
        T[] value;
        Label lblAvailable;
        Label lblUsed;
        ListView availableItems;
        ListView usedItems;
        ColumnHeader availableItemsHeader;
        ColumnHeader usedItemsHeader;
        Button btnAdd;
        Button btnRemove;
        Button btnUp;
        Button btnDown;
        Button btnOK;
        Button btnCancel;

        /// <summary>
        /// Creates an instance of <see cref="FixedValuesCollectionEditor{string}"/>.
        /// </summary>
        /// <param name="all">An array of all available items.</param>
        /// <param name="values">An array to modify.</param>
        /// <param name="lockedValues">Values that cannot be removed from the specified array.</param>
        public FixedValuesCollectionEditor(T[] all, T[] values, params T[] lockedValues)
        {
            InitializeComponent();
            InitializeGraphics();
            InitializeAvailableList(all, values);
            InitializeUsedList(values, lockedValues);
            ValidateButtons();
            value = values;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblAvailable = new System.Windows.Forms.Label();
            this.lblUsed = new System.Windows.Forms.Label();
            this.availableItems = new System.Windows.Forms.ListViewEx();
            this.usedItems = new System.Windows.Forms.ListViewEx();
            this.availableItemsHeader = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.usedItemsHeader = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.btnAdd = new System.Windows.Forms.ButtonEx();
            this.btnRemove = new System.Windows.Forms.ButtonEx();
            this.btnUp = new System.Windows.Forms.ButtonEx();
            this.btnDown = new System.Windows.Forms.ButtonEx();
            this.btnCancel = new System.Windows.Forms.ButtonEx();
            this.btnOK = new System.Windows.Forms.ButtonEx();
            this.SuspendLayout();
            // 
            // availableItemsHeader
            // 
            this.availableItemsHeader.Width = -1;
            // 
            // usedItemsHeader
            // 
            this.usedItemsHeader.Width = -1;
            // 
            // lblAvailable
            // 
            this.lblAvailable.AutoSize = true;
            this.lblAvailable.Location = new System.Drawing.Point(8, 9);
            this.lblAvailable.Name = "lblAvailable";
            this.lblAvailable.Size = new System.Drawing.Size(111, 20);
            this.lblAvailable.Text = "&Available Items";
            // 
            // lblUsed
            // 
            this.lblUsed.AutoSize = true;
            this.lblUsed.Location = new System.Drawing.Point(200, 9);
            this.lblUsed.Name = "lblUsed";
            this.lblUsed.Size = new System.Drawing.Size(82, 20);
            this.lblUsed.Text = "&Used Items";
            // 
            // availableItems
            // 
            this.availableItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.availableItemsHeader });
            this.availableItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.availableItems.Location = new System.Drawing.Point(12, 32);
            this.availableItems.Name = "availableItems";
            this.availableItems.Size = new System.Drawing.Size(150, 260);
            this.availableItems.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.availableItems.TabStop = false;
            this.availableItems.UseCompatibleStateImageBehavior = false;
            this.availableItems.View = System.Windows.Forms.View.Details;
            this.availableItems.SelectedIndexChanged += new System.EventHandler(this.AvailableItems_SelectedIndexChanged);
            this.availableItems.Enter += new System.EventHandler(this.AvailableItems_Enter);
            // 
            // usedItems
            // 
            this.usedItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.usedItemsHeader });
            this.usedItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.usedItems.Location = new System.Drawing.Point(204, 32);
            this.usedItems.Name = "usedItems";
            this.usedItems.Size = new System.Drawing.Size(150, 260);
            this.usedItems.TabStop = false;
            this.usedItems.UseCompatibleStateImageBehavior = false;
            this.usedItems.View = System.Windows.Forms.View.Details;
            this.usedItems.SelectedIndexChanged += new System.EventHandler(this.UsedItems_SelectedIndexChanged);
            this.usedItems.Enter += new System.EventHandler(this.UsedItems_Enter);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(168, 120);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(30, 30);
            this.btnAdd.TabStop = false;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(168, 156);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(30, 30);
            this.btnRemove.TabStop = false;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.BtnRemove_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(360, 60);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(30, 30);
            this.btnUp.TabStop = false;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.BtnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(360, 96);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(30, 30);
            this.btnDown.TabStop = false;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.BtnDown_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(315, 298);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(234, 298);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // FixedValuesCollectionEditor
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(402, 340);
            this.Controls.Add(this.lblAvailable);
            this.Controls.Add(this.lblUsed);
            this.Controls.Add(this.availableItems);
            this.Controls.Add(this.usedItems);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.HelpButton = true;
            this.Name = "FixedValuesCollectionEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FixedValuesCollectionEditor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the modified array of items.
        /// </summary>
        public T[] Value
        {
            get { return value; }
        }

        #endregion

        #region Initialization

        void InitializeGraphics()
        {
            Font = PluginBase.MainForm.Settings.DefaultFont;
            btnAdd.Image = PluginBase.MainForm.FindImage16("67", false);
            btnRemove.Image = PluginBase.MainForm.FindImage16("63", false);
            btnUp.Image = PluginBase.MainForm.FindImage16("74", false);
            btnDown.Image = PluginBase.MainForm.FindImage16("60", false);
        }

        void InitializeAvailableList(T[] all, T[] values)
        {
            availableItems.BeginUpdate();
            for (int i = 0; i < all.Length; i++)
            {
                if (!values.Contains(all[i]))
                {
                    availableItems.Items.Add(new Item(all[i]));
                }
            }
            availableItems.EndUpdate();
        }

        void InitializeUsedList(T[] values, T[] lockedValues)
        {
            usedItems.BeginUpdate();
            for (int i = 0; i < values.Length; i++)
            {
                usedItems.Items.Add(new Item(values[i], lockedValues.Contains(values[i])));
            }
            usedItems.EndUpdate();
        }

        void ValidateButtons()
        {
            AvailableItems_SelectedIndexChanged(null, null);
            UsedItems_SelectedIndexChanged(null, null);
        }

        #endregion

        #region Event handlers

        void AvailableItems_Enter(object sender, EventArgs e)
        {
            usedItems.BeginUpdate();
            foreach (Item item in usedItems.SelectedItems) item.Selected = false;
            usedItems.EndUpdate();
        }

        void UsedItems_Enter(object sender, EventArgs e)
        {
            availableItems.BeginUpdate();
            foreach (Item item in availableItems.SelectedItems) item.Selected = false;
            availableItems.EndUpdate();
        }

        void AvailableItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            var indices = availableItems.SelectedIndices;
            btnAdd.Enabled = indices.Count > 0;
        }

        void UsedItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            var indices = usedItems.SelectedIndices;
            if (indices.Count == 0)
            {
                btnRemove.Enabled = false;
                btnUp.Enabled = false;
                btnDown.Enabled = false;
            }
            else
            {
                btnRemove.Enabled = indices.Count > 1 || !((Item) usedItems.Items[indices[0]]).Locked;
                btnUp.Enabled = indices[0] > 0;
                btnDown.Enabled = indices[indices.Count - 1] < usedItems.Items.Count - 1;
            }
        }

        void BtnAdd_Click(object sender, EventArgs e)
        {
            availableItems.BeginUpdate();
            usedItems.BeginUpdate();

            var selectedItems = availableItems.SelectedItems;
            var items = new Item[selectedItems.Count];
            selectedItems.CopyTo(items, 0);

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                availableItems.Items.Remove(item);
                usedItems.Items.Add(item);
            }

            availableItemsHeader.Width = -1;
            usedItemsHeader.Width = -1;
            availableItems.EndUpdate();
            usedItems.EndUpdate();
            usedItems.Select();
        }

        void BtnRemove_Click(object sender, EventArgs e)
        {
            availableItems.BeginUpdate();
            usedItems.BeginUpdate();

            var selectedItems = usedItems.SelectedItems;
            var items = new Item[selectedItems.Count];
            selectedItems.CopyTo(items, 0);

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                if (item.Locked) continue;
                usedItems.Items.Remove(item);
                availableItems.Items.Add(item);
            }

            availableItemsHeader.Width = -1;
            usedItemsHeader.Width = -1;
            availableItems.EndUpdate();
            usedItems.EndUpdate();
            availableItems.Select();
        }

        void BtnUp_Click(object sender, EventArgs e)
        {
            usedItems.BeginUpdate();

            var indices = usedItems.SelectedIndices;

            for (int i = 0; i < indices.Count; i++)
            {
                int index = indices[i];
                var items = usedItems.Items;
                var item = items[index];
                items.RemoveAt(index);
                items.Insert(index - 1, item);
            }

            usedItems.EndUpdate();
            usedItems.Select();
        }

        void BtnDown_Click(object sender, EventArgs e)
        {
            usedItems.BeginUpdate();

            var indices = usedItems.SelectedIndices;

            for (int i = indices.Count - 1; i >= 0; i--)
            {
                int index = indices[i];
                var items = usedItems.Items;
                var item = items[index];
                items.RemoveAt(index);
                items.Insert(index + 1, item);
            }

            usedItems.EndUpdate();
            usedItems.Select();
        }

        void BtnOK_Click(object sender, EventArgs e)
        {
            var items = usedItems.Items;
            int count = items.Count;
            value = new T[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ((Item) items[i]).Value;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion

        #region Item class

        class Item : ListViewItem
        {
            bool locked;
            T value;

            public bool Locked
            {
                get { return locked; }
            }

            public T Value
            {
                get { return value; }
            }

            public Item(T value) : this(value, false)
            {
            }

            public Item(T value, bool locked)
            {
                Text = Name = Convert.ToString(value);
                this.value = value;
                this.locked = locked;
            }
        }

        #endregion
    }
}
