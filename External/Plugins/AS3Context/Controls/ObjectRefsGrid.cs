using System;
using System.Collections.Generic;
using System.Text;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using PluginCore;
using PluginCore.Helpers;

namespace AS3Context.Controls
{
    public class ObjectRefsGrid:TreeViewAdv
    {
        NodeTextBox methodTB;
        NodeTextBox fileTB;
        NodeTextBox lineTB;

        public ObjectRefsGrid()
        {
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
        }
    }

    public class ObjectRefsNode : Node
    {
        string method;
        string path;
        string file;
        string line;

        public ObjectRefsNode(string method, string file, string line)
        {
            this.method = method;
            this.path = file;
            int p = file.LastIndexOf(';');
            if (p < 0) p = file.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
            this.file = file.Substring(p + 1);
            this.line = line;
        }

        public String Method
        {
            get { return method; }
        }
        public String Path
        {
            get { return path; }
        }
        public String File
        {
            get { return file; }
        }
        public String Line
        {
            get { return line; }
        }
    }

    public class ObjectRefsModel : TreeModel
    {

    }
}
