// Original source code from a CodeProject article, several improvements where added.

using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace AirProperties.Controls
{
    public class CheckedComboBox : ComboBox
    {
        /// <summary>
        /// Internal class to represent the dropdown list of the CheckedComboBox
        /// </summary>
        internal class Dropdown : Form
        {
/*            private const int WM_NCPAINT = 0x85;
            private const int WM_NCACTIVATE = 0x86;

            protected override void WndProc(ref Message m)
            {
                Debug.WriteLine(m.Msg);
                if (!dropdownClosed && (m.Msg == WM_NCACTIVATE || m.Msg == WM_NCPAINT && m.WParam.ToInt32() != 1 && !this.Bounds.Contains(Control.MousePosition))) // Should we check mouse buttons as well?
                {
                    // Workaround to improve ux in some cases
                    OnDeactivate(EventArgs.Empty);
                }
                base.WndProc(ref m);
            }*/
            // ---------------------------------- internal class CCBoxEventArgs --------------------------------------------
            /// <summary>
            /// Custom EventArgs encapsulating value as to whether the combo box value(s) should be assignd to or not.
            /// </summary>
            internal class CCBoxEventArgs : EventArgs
            {
                public bool AssignValues { get; set; }
                public EventArgs EventArgs { get; set; }

                public CCBoxEventArgs(EventArgs e, bool assignValues)
                    : base()
                {
                    EventArgs = e;
                    AssignValues = assignValues;
                }
            }

            // ---------------------------------- internal class CustomCheckedListBox --------------------------------------------

            /// <summary>
            /// A custom CheckedListBox being shown within the dropdown form representing the dropdown list of the CheckedComboBox.
            /// </summary>
            internal class CustomCheckedListBox : CheckedListBox
            {
                private int curSelIndex = -1;

                public CustomCheckedListBox()
                    : base()
                {
                    SelectionMode = SelectionMode.One;
                    HorizontalScrollbar = true;
                }
                /// <summary>
                /// Intercepts the keyboard input, [Enter] confirms a selection and [Esc] cancels it.
                /// </summary>
                /// <param name="e">The Key event arguments</param>
                protected override void OnKeyDown(KeyEventArgs e)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        // Enact selection.
                        ((Dropdown)Parent).OnDeactivate(new CCBoxEventArgs(null, true));
                        e.Handled = true;

                    }
                    else if (e.KeyCode == Keys.Escape)
                    {
                        // Cancel selection.
                        ((Dropdown)Parent).OnDeactivate(new CCBoxEventArgs(null, false));
                        e.Handled = true;

                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        // Delete unckecks all, [Shift + Delete] checks all.
                        for (int i = 0; i < Items.Count; i++)
                        {
                            SetItemChecked(i, e.Shift);
                        }
                        e.Handled = true;
                    }
                    // If no Enter or Esc keys presses, let the base class handle it.
                    base.OnKeyDown(e);
                }

                protected override void OnMouseMove(MouseEventArgs e)
                {
                    base.OnMouseMove(e);
                    int index = IndexFromPoint(e.Location);
                    Debug.WriteLine("Mouse over item: " + (index >= 0 ? GetItemText(Items[index]) : "None"));
                    if ((index >= 0) && (index != curSelIndex))
                    {
                        curSelIndex = index;
                        SetSelected(index, true);
                    }
                }

            } // end internal class CustomCheckedListBox

            // --------------------------------------------------------------------------------------------------------

            private const int CS_DROPSHADOW = 0x00020000;
            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.ClassStyle |= CS_DROPSHADOW;
                    return cp;
                }
            }

            // ********************************************* Data *********************************************

            private readonly CheckedComboBox ccbParent;

            // Keeps track of whether checked item(s) changed, hence the value of the CheckedComboBox as a whole changed.
            // This is simply done via maintaining the old string-representation of the value(s) and the new one and comparing them!
            private string oldStrValue = "";
            public bool ValueChanged
            {
                get
                {
                    string newStrValue = ccbParent.Text;
                    if ((oldStrValue.Length > 0) && (newStrValue.Length > 0))
                    {
                        return (oldStrValue.CompareTo(newStrValue) != 0);
                    }

                    return (oldStrValue.Length != newStrValue.Length);
                }
            }

            // Array holding the checked states of the items. This will be used to reverse any changes if user cancels selection.
            bool[] checkedStateArr;

            // Whether the dropdown is closed.
            private bool dropdownClosed = true;

            private CustomCheckedListBox cclb;
            public CustomCheckedListBox List
            {
                get => cclb;
                set => cclb = value;
            }

            // ********************************************* Construction *********************************************

            public Dropdown(CheckedComboBox ccbParent)
            {
                this.ccbParent = ccbParent;
                InitializeComponent();
                ShowInTaskbar = false;
                // Add a handler to notify our parent of ItemCheck events.
                cclb.ItemCheck += cclb_ItemCheck;
            }

            // ********************************************* Methods *********************************************

            // Create a CustomCheckedListBox which fills up the entire form area.
            private void InitializeComponent()
            {
                cclb = new CustomCheckedListBox();
                SuspendLayout();
                // 
                // cclb
                // 
                cclb.BorderStyle = BorderStyle.FixedSingle;
                cclb.Dock = DockStyle.Fill;
                cclb.FormattingEnabled = true;
                cclb.Location = new Point(0, 0);
                cclb.Name = "cclb";
                cclb.Size = new Size(47, 15);
                cclb.TabIndex = 0;
                // 
                // Dropdown
                // 
                AutoScaleDimensions = new SizeF(6F, 13F);
                AutoScaleMode = AutoScaleMode.Font;
                BackColor = SystemColors.Menu;
                ClientSize = new Size(47, 16);
                ControlBox = false;
                Controls.Add(cclb);
                ForeColor = SystemColors.ControlText;
                FormBorderStyle = FormBorderStyle.None;
                MinimizeBox = false;
                Name = "ccbParent";
                StartPosition = FormStartPosition.Manual;
                ResumeLayout(false);
            }

            public string GetCheckedItemsStringValue()
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < cclb.CheckedItems.Count; i++)
                {
                    sb.Append(cclb.GetItemText(cclb.CheckedItems[i])).Append(ccbParent.ValueSeparator);
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - ccbParent.ValueSeparator.Length, ccbParent.ValueSeparator.Length);
                }
                return sb.ToString();
            }

            /// <summary>
            /// Closes the dropdown portion and enacts any changes according to the specified boolean parameter.
            /// NOTE: even though the caller might ask for changes to be enacted, this doesn't necessarily mean
            ///       that any changes have occurred as such. Caller should check the ValueChanged property of the
            ///       CheckedComboBox (after the dropdown has closed) to determine any actual value changes.
            /// </summary>
            /// <param name="enactChanges"></param>
            public void CloseDropdown(bool enactChanges)
            {
                if (dropdownClosed)
                {
                    return;
                }
                // Perform the actual selection and display of checked items.
                if (enactChanges)
                {
                    ccbParent.SelectedIndex = -1;
                    // Set the text portion equal to the string comprising all checked items (if any, otherwise empty!).
                    ccbParent.SetText(GetCheckedItemsStringValue());

                }
                else
                {
                    // Caller cancelled selection - need to restore the checked items to their original state.
                    for (int i = 0; i < cclb.Items.Count; i++)
                    {
                        cclb.SetItemChecked(i, checkedStateArr[i]);
                    }
                }
                // From now on the dropdown is considered closed. We set the flag here to prevent OnDeactivate() calling
                // this method once again after hiding this window.
                dropdownClosed = true;
                // Set the focus to our parent CheckedComboBox and hide the dropdown check list.
                ccbParent.Focus();
                Hide();
                // Notify CheckedComboBox that its dropdown is closed. (NOTE: it does not matter which parameters we pass to
                // OnDropDownClosed() as long as the argument is CCBoxEventArgs so that the method knows the notification has
                // come from our code and not from the framework).
                ccbParent.OnDropDownClosed(new CCBoxEventArgs(null, false));
            }

            protected override void OnActivated(EventArgs e)
            {
                Debug.WriteLine("OnAc");
                base.OnActivated(e);
                dropdownClosed = false;
                // Assign the old string value to compare with the new value for any changes.
                oldStrValue = ccbParent.Text;
                // Make a copy of the checked state of each item, in cace caller cancels selection.
                checkedStateArr = new bool[cclb.Items.Count];
                for (int i = 0; i < cclb.Items.Count; i++)
                {
                    checkedStateArr[i] = cclb.GetItemChecked(i);
                }
            }

            protected override void OnDeactivate(EventArgs e)
            {
                base.OnDeactivate(e);
                CCBoxEventArgs ce = e as CCBoxEventArgs;
                if (ce != null)
                {
                    CloseDropdown(ce.AssignValues);
                }
                else
                {
                    // If not custom event arguments passed, means that this method was called from the
                    // framework. We assume that the checked values should be registered regardless.
                    CloseDropdown(true);
                }
            }

            private void cclb_ItemCheck(object sender, ItemCheckEventArgs e)
            {
                ccbParent.ItemCheck?.Invoke(sender, e);
            }

        } // end internal class Dropdown

        // ******************************** Data ********************************
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.IContainer components = null;
        // A form-derived object representing the drop-down list of the checked combo box.
        private readonly Dropdown dropdown;

        // The valueSeparator character(s) between the ticked elements as they appear in the 
        // text portion of the CheckedComboBox.
        public string ValueSeparator { get; set; }

        public bool CheckOnClick
        {
            get => dropdown.List.CheckOnClick;
            set => dropdown.List.CheckOnClick = value;
        }

        public new string DisplayMember
        {
            get => dropdown.List.DisplayMember;
            set => dropdown.List.DisplayMember = value;
        }

        public new CheckedListBox.ObjectCollection Items => dropdown.List.Items;

        public CheckedListBox.CheckedItemCollection CheckedItems => dropdown.List.CheckedItems;

        public CheckedListBox.CheckedIndexCollection CheckedIndices => dropdown.List.CheckedIndices;

        public bool ValueChanged => dropdown.ValueChanged;

        /*protected override CreateParams CreateParams
        {
            get
            {
                var baseParams = base.CreateParams;
                baseParams.ClassName = "ComboBoxEx32";
                baseParams.ExStyle = baseParams.ExStyle & ~0x200;

                return baseParams;
            }
        }*/

        // Event handler for when an item check state changes.
        public event ItemCheckEventHandler ItemCheck;

        // ******************************** Construction ********************************

        public CheckedComboBox()
            : base()
        {
            // We want to do the drawing of the dropdown.
            DrawMode = DrawMode.Normal;
            // Default value separator.
            ValueSeparator = ", ";
            // This prevents the actual ComboBox dropdown to show, although it's not strickly-speaking necessary.
            // But including this remove a slight flickering just before our dropdown appears (which is caused by
            // the empty-dropdown list of the ComboBox which is displayed for fractions of a second).
            DropDownHeight = 1;
            dropdown = new Dropdown(this);
            // CheckOnClick style for the dropdown (NOTE: must be set after dropdown is created).
            CheckOnClick = true;
        }

        // ******************************** Operations ********************************

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DoDropDown()
        {
            if (!dropdown.Visible)
            {
                Rectangle rect = RectangleToScreen(ClientRectangle);
                dropdown.Location = new Point(rect.X, rect.Y + Size.Height);
                int count = dropdown.List.Items.Count;
                if (count > MaxDropDownItems)
                {
                    count = MaxDropDownItems;
                }
                else if (count == 0)
                {
                    count = 1;
                }
                dropdown.Size = new Size(Size.Width, (dropdown.List.ItemHeight) * count + 2);
                dropdown.List.Focus();
                dropdown.Show(this);
            }
        }



        protected override void OnDropDownClosed(EventArgs e)
        {
            // Call the handlers for this event only if the call comes from our code - NOT the framework's!
            // NOTE: that is because the events were being fired in a wrong order, due to the actual dropdown list
            //       of the ComboBox which lies underneath our dropdown and gets involved every time.
            if (e is Dropdown.CCBoxEventArgs)
            {
                base.OnDropDownClosed(e);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                // Signal that the dropdown is "down". This is required so that the behaviour of the dropdown is the same
                // when it is a result of user pressing the Down_Arrow (which we handle and the framework wouldn't know that
                // the list portion is down unless we tell it so).
                // NOTE: all that so the DropDownClosed event fires correctly!                
                OnDropDown(null);
            }
            // Make sure that certain keys or combinations are not blocked.
            e.Handled = !e.Alt && !(e.KeyCode == Keys.Tab) &&
                !((e.KeyCode == Keys.Left) || (e.KeyCode == Keys.Right) || (e.KeyCode == Keys.Home) || (e.KeyCode == Keys.End));

            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            e.Handled = true;
            base.OnKeyPress(e);
        }

        public bool GetItemChecked(int index)
        {
            if (index < 0 || index > Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "value out of range");
            }

            return dropdown.List.GetItemChecked(index);
        }

        public void SetItemChecked(int index, bool isChecked)
        {
            if (index < 0 || index > Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "value out of range");
            }

            dropdown.List.SetItemChecked(index, isChecked);
            // Need to update the Text.
            SetText(dropdown.GetCheckedItemsStringValue());
        }

        public CheckState GetItemCheckState(int index)
        {
            if (index < 0 || index > Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "value out of range");
            }

            return dropdown.List.GetItemCheckState(index);
        }

        public void SetItemCheckState(int index, CheckState state)
        {
            if (index < 0 || index > Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "value out of range");
            }

            dropdown.List.SetItemCheckState(index, state);
            // Need to update the Text.
            SetText(dropdown.GetCheckedItemsStringValue());
        }

        /*protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 && this.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                e.Graphics.DrawString(dropdown.GetCheckedItemsStringValue(), e.Font, SystemBrushes.WindowText,
                                      e.Bounds.Left, e.Bounds.Top);
            }

            base.OnDrawItem(e);
        }*/

        private void SetText(string value)
        {
            if (DropDownStyle == ComboBoxStyle.DropDownList)
            {
                base.Items.Clear();
                if (CheckedIndices.Count > 0)
                {
                    base.Items.Add(value);
                    SelectedIndex = 0;
                }
            }
            else
            {
                Text = value;
            }
        }

        protected override void WndProc(ref Message m)
        {
            //if (m.Msg == 0x2B) m.Msg = 0x202B;
            base.WndProc(ref m);
            if ((m.Msg == 0x201 || m.Msg == 0x203) && !dropdown.Visible) DoDropDown();
        }

    } // end public class CheckedComboBox
}
