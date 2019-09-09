﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using PluginCore;
using PluginCore.Helpers;
using System.Drawing;

namespace AS3Context.Controls
{
    public class ObjectRefsGrid:TreeViewAdv
    {
        readonly NodeTextBox methodTB;
        readonly NodeTextBox fileTB;
        readonly NodeTextBox lineTB;

        public ObjectRefsGrid()
        {
            BorderStyle = System.Windows.Forms.BorderStyle.None;
            Dock = System.Windows.Forms.DockStyle.Fill;
            GridLineStyle = GridLineStyle.HorizontalAndVertical;
            Font = PluginBase.Settings.DefaultFont;

            UseColumns = true;

            Columns.Add(new TreeColumn(PluginCore.Localization.TextHelper.GetString("Columns.Method"), 350));
            Columns.Add(new TreeColumn(PluginCore.Localization.TextHelper.GetString("Columns.File"), 200));
            Columns.Add(new TreeColumn(PluginCore.Localization.TextHelper.GetString("Columns.Line"), 50));

            foreach (TreeColumn column in this.Columns)
                column.Width = ScaleHelper.Scale(column.Width);

            methodTB = new NodeTextBox();
            methodTB.DataPropertyName = "Method";
            methodTB.ParentColumn = Columns[0];
            methodTB.Font = PluginBase.Settings.DefaultFont;
            fileTB = new NodeTextBox();
            fileTB.DataPropertyName = "File";
            fileTB.ParentColumn = Columns[1];
            fileTB.Font = PluginBase.Settings.DefaultFont;
            lineTB = new NodeTextBox();
            lineTB.DataPropertyName = "Line";
            lineTB.ParentColumn = Columns[2];
            lineTB.Font = PluginBase.Settings.DefaultFont;

            NodeControls.Add(methodTB);
            NodeControls.Add(fileTB);
            NodeControls.Add(lineTB);

            this.CustomDrawHeaders = PluginBase.MainForm.GetThemeColor("ColumnHeader.BorderColor") != Color.Empty;
            this.ColumnHeaderBackColor = PluginBase.MainForm.GetThemeColor("ColumnHeader.BackColor", SystemColors.Control);
            this.ColumnHeaderTextColor = PluginBase.MainForm.GetThemeColor("ColumnHeader.TextColor", SystemColors.ControlText);
            this.ColumnHeaderBorderColor = PluginBase.MainForm.GetThemeColor("ColumnHeader.BorderColor", SystemColors.ActiveBorder);
            this.LineColor = PluginBase.MainForm.GetThemeColor("DataTreeControl.LineColor", SystemColors.ActiveBorder);
            this.LineColor2 = PluginBase.MainForm.GetThemeColor("DataTreeControl.LineColor", SystemColors.ActiveBorder);
            this.DragDropMarkColor = PluginBase.MainForm.GetThemeColor("DataTreeControl.ForeColor", SystemColors.WindowText);
            this.ForeColor = PluginBase.MainForm.GetThemeColor("TreeViewAdv.ForeColor", SystemColors.ControlText);
            this.BackColor = PluginBase.MainForm.GetThemeColor("TreeViewAdv.BackColor", SystemColors.Control);
        }
    }

    public class ObjectRefsNode : Node
    {
        readonly string method;
        readonly string path;
        readonly string file;
        readonly string line;

        public ObjectRefsNode(string method, string file, string line)
        {
            this.method = method;
            this.path = file;
            int p = file.LastIndexOf(';');
            if (p < 0) p = file.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
            this.file = file.Substring(p + 1);
            this.line = line;
        }

        public string Method => method;

        public string Path => path;

        public string File => file;

        public string Line => line;
    }

    public class ObjectRefsModel : TreeModel
    {

    }
}
