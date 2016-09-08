using System;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;

namespace CodeRefactor.Controls
{
    public class RefactorMenu : ToolStripMenuItem
    {
        public RefactorMenu(Boolean createSurroundMenu)
        {
            this.Text = TextHelper.GetString("Label.Refactor");
            this.RenameMenuItem = this.DropDownItems.Add(TextHelper.GetString("Label.Rename")) as ToolStripMenuItem;
            this.RenameMenuItem.Image = PluginBase.MainForm.FindImage("331");
            this.MoveMenuItem = this.DropDownItems.Add(TextHelper.GetString("Label.Move")) as ToolStripMenuItem;
            this.ExtractMethodMenuItem = this.DropDownItems.Add(TextHelper.GetString("Label.ExtractMethod"), null) as ToolStripMenuItem;
            this.ExtractLocalVariableMenuItem = this.DropDownItems.Add(TextHelper.GetString("Label.ExtractLocalVariable"), null) as ToolStripMenuItem;
            this.DelegateMenuItem = this.DropDownItems.Add(TextHelper.GetString("Label.DelegateMethods"), null) as ToolStripMenuItem;
            if (createSurroundMenu)
            {
                this.SurroundMenu = new SurroundMenu();
                this.DropDownItems.Add(this.SurroundMenu);
            }
            this.DropDownItems.Add(new ToolStripSeparator());
            this.CodeGeneratorMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.InvokeCodeGenerator"), null, null, createSurroundMenu ? Keys.Control | Keys.Shift | Keys.D1 : Keys.None);
            this.DropDownItems.Add(this.CodeGeneratorMenuItem);
            this.DropDownItems.Add(new ToolStripSeparator());
            this.OrganizeMenuItem = this.DropDownItems.Add(TextHelper.GetString("Label.OrganizeImports"), null) as ToolStripMenuItem;
            this.TruncateMenuItem = this.DropDownItems.Add(TextHelper.GetString("Label.TruncateImports"), null) as ToolStripMenuItem;
            this.DropDownItems.Add(new ToolStripSeparator());
            this.BatchMenuItem = this.DropDownItems.Add(TextHelper.GetString("Label.BatchProcess"), null) as ToolStripMenuItem;
        }

        /// <summary>
        /// Accessor to the SurroundMenu
        /// </summary>
        public SurroundMenu SurroundMenu { get; private set; }

        /// <summary>
        /// Accessor to the BatchMenuItem
        /// </summary>
        public ToolStripMenuItem BatchMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the RenameMenuItem
        /// </summary>
        public ToolStripMenuItem RenameMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the MoveMenuItem
        /// </summary>
        public ToolStripMenuItem MoveMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the TruncateMenuItem
        /// </summary>
        public ToolStripMenuItem TruncateMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the OrganizeMenuItem
        /// </summary>
        public ToolStripMenuItem OrganizeMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the ExtractMethodMenuItem
        /// </summary>
        public ToolStripMenuItem ExtractMethodMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the DelegateMenuItem
        /// </summary>
        public ToolStripMenuItem DelegateMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the ExtractLocalVariableMenuItem
        /// </summary>
        public ToolStripMenuItem ExtractLocalVariableMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the CodeGeneratorMenuItem
        /// </summary>
        public ToolStripMenuItem CodeGeneratorMenuItem { get; private set; }

    }

}
