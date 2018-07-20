/*
    Copyright (C) 2009  Robert Nelson

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/

using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace FlashDebugger.Controls
{
    partial class DataTreeControl
    {
        /// <summary> 
        /// 
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 

        /// <summary> 
        /// </summary>
        private void InitializeComponent()
        {
            this._tree = new TreeViewAdv();
            this.NameTreeColumn = new TreeColumn();
            this.ValueTreeColumn = new Aga.Controls.Tree.TreeColumn();
            this.NameNodeTextBox = new NodeTextBox();
            this.ValueNodeTextBox = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.SuspendLayout();
            // 
            // _tree
            //
            this._tree.UseColumns = true;
            this._tree.BackColor = System.Drawing.SystemColors.Window;
            this._tree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tree.Columns.Add(this.NameTreeColumn);
            this._tree.Columns.Add(this.ValueTreeColumn);
            this._tree.DefaultToolTipProvider = null;
            this._tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tree.DragDropMarkColor = System.Drawing.Color.Black;
            this._tree.GridLineStyle = Aga.Controls.Tree.GridLineStyle.HorizontalAndVertical;
            this._tree.LineColor = System.Drawing.SystemColors.Control;
            this._tree.Location = new System.Drawing.Point(0, 0);
            this._tree.Model = null;
            this._tree.Name = "_tree";
            this._tree.NodeControls.Add(this.NameNodeTextBox);
            this._tree.NodeControls.Add(this.ValueNodeTextBox);
            this._tree.SelectedNode = null;
            this._tree.ShowNodeToolTips = true;
            this._tree.Size = new System.Drawing.Size(282, 155);
            this._tree.TabIndex = 0;
            this._tree.Text = "treeViewAdv1";
            this._tree.UseColumns = true;
            // 
            // NameTreeColumn
            // 
            this.NameTreeColumn.Header = "Name";
            this.NameTreeColumn.SortOrder = System.Windows.Forms.SortOrder.None;
            this.NameTreeColumn.TooltipText = null;
            this.NameTreeColumn.Width = 100;
            // 
            // ValueTreeColumn
            // 
            this.ValueTreeColumn.Header = "Value";
            this.ValueTreeColumn.SortOrder = System.Windows.Forms.SortOrder.None;
            this.ValueTreeColumn.TooltipText = null;
            this.ValueTreeColumn.Width = 100;
            // 
            // NameNodeTextBox
            // 
            this.NameNodeTextBox.DataPropertyName = "Text";
            this.NameNodeTextBox.IncrementalSearchEnabled = true;
            this.NameNodeTextBox.LeftMargin = 3;
            this.NameNodeTextBox.ParentColumn = this.NameTreeColumn;
            // 
            // ValueNodeTextBox
            // 
            this.ValueNodeTextBox.DataPropertyName = "Value";
            this.ValueNodeTextBox.IncrementalSearchEnabled = true;
            this.ValueNodeTextBox.LeftMargin = 3;
            this.ValueNodeTextBox.ParentColumn = this.ValueTreeColumn;
            // 
            // DataTreeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._tree);
            this.Name = "DataTreeControl";
            this.Size = new System.Drawing.Size(282, 155);
            this.ResumeLayout(false);

        }

        #endregion

        private Aga.Controls.Tree.TreeViewAdv _tree;
        private Aga.Controls.Tree.TreeColumn NameTreeColumn;
        private Aga.Controls.Tree.TreeColumn ValueTreeColumn;
        private Aga.Controls.Tree.NodeControls.NodeTextBox NameNodeTextBox;
        private Aga.Controls.Tree.NodeControls.NodeTextBox ValueNodeTextBox;
    }
}
