using System;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Localization;

namespace CodeRefactor.Controls
{
    public class RefactorMenu : ToolStripMenuItem
    {
        public RefactorMenu(Boolean createSurroundMenu)
        {
            this.Text = TextHelper.GetString("Label.Refactor");
            this.RenameMenuItem = new ToolStripMenuItemEx(TextHelper.GetString("Label.Rename"));
            this.RenameMenuItem.Image = PluginBase.MainForm.FindImage("331");
            this.DropDownItems.Add(this.RenameMenuItem);
            this.MoveMenuItem = new ToolStripMenuItemEx(TextHelper.GetString("Label.Move"));
            this.DropDownItems.Add(this.MoveMenuItem);
            this.ExtractMethodMenuItem = new ToolStripMenuItemEx(TextHelper.GetString("Label.ExtractMethod"), null);
            this.DropDownItems.Add(this.ExtractMethodMenuItem);
            this.ExtractLocalVariableMenuItem = new ToolStripMenuItemEx(TextHelper.GetString("Label.ExtractLocalVariable"), null);
            this.DropDownItems.Add(this.ExtractLocalVariableMenuItem);
            this.DelegateMenuItem = new ToolStripMenuItemEx(TextHelper.GetString("Label.DelegateMethods"), null);
            this.DropDownItems.Add(this.DelegateMenuItem);
            if (createSurroundMenu)
            {
                this.SurroundMenu = new SurroundMenu();
                this.DropDownItems.Add(this.SurroundMenu);
            }
            this.DropDownItems.Add(new ToolStripSeparator());
            this.CodeGeneratorMenuItem = new ToolStripMenuItemEx(TextHelper.GetString("Label.InvokeCodeGenerator"), null, null, createSurroundMenu ? Keys.Control | Keys.Shift | Keys.D1 : Keys.None);
            this.DropDownItems.Add(this.CodeGeneratorMenuItem);
            this.DropDownItems.Add(new ToolStripSeparator());
            this.OrganizeMenuItem = new ToolStripMenuItemEx(TextHelper.GetString("Label.OrganizeImports"), null);
            this.DropDownItems.Add(this.OrganizeMenuItem);
            this.TruncateMenuItem = new ToolStripMenuItemEx(TextHelper.GetString("Label.TruncateImports"), null);
            this.DropDownItems.Add(this.TruncateMenuItem);
            this.DropDownItems.Add(new ToolStripSeparator());
            this.BatchMenuItem = new ToolStripMenuItemEx(TextHelper.GetString("Label.BatchProcess"), null);
            this.DropDownItems.Add(this.BatchMenuItem);
        }

        /// <summary>
        /// Accessor to the SurroundMenu
        /// </summary>
        public SurroundMenu SurroundMenu { get; private set; }

        /// <summary>
        /// Accessor to the BatchMenuItem
        /// </summary>
        public ToolStripMenuItemEx BatchMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the RenameMenuItem
        /// </summary>
        public ToolStripMenuItemEx RenameMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the MoveMenuItem
        /// </summary>
        public ToolStripMenuItemEx MoveMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the TruncateMenuItem
        /// </summary>
        public ToolStripMenuItemEx TruncateMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the OrganizeMenuItem
        /// </summary>
        public ToolStripMenuItemEx OrganizeMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the ExtractMethodMenuItem
        /// </summary>
        public ToolStripMenuItemEx ExtractMethodMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the DelegateMenuItem
        /// </summary>
        public ToolStripMenuItemEx DelegateMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the ExtractLocalVariableMenuItem
        /// </summary>
        public ToolStripMenuItemEx ExtractLocalVariableMenuItem { get; private set; }

        /// <summary>
        /// Accessor to the CodeGeneratorMenuItem
        /// </summary>
        public ToolStripMenuItemEx CodeGeneratorMenuItem { get; private set; }

    }

}
