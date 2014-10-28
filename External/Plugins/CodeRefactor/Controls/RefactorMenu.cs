using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using PluginCore.Localization;
using PluginCore;

namespace CodeRefactor.Controls
{
    public class RefactorMenu : ToolStripMenuItem
    {
        private SurroundMenu surroundMenu;
        private ToolStripMenuItem renameMenuItem;
        private ToolStripMenuItem truncateMenuItem;
        private ToolStripMenuItem organizeMenuItem;
        private ToolStripMenuItem delegateMenuItem;
        private ToolStripMenuItem generatorMenuItem;
        private ToolStripMenuItem extractMethodMenuItem;
        private ToolStripMenuItem extractLocalVariableMenuItem;

        public RefactorMenu(Boolean createSurroundMenu)
        {
            Image empty = PluginBase.MainForm.FindImage("559");
            this.Text = TextHelper.GetString("Label.Refactor");
            this.renameMenuItem = this.DropDownItems.Add(TextHelper.GetString("Label.Rename"), empty) as ToolStripMenuItem;
            this.extractMethodMenuItem = this.DropDownItems.Add(TextHelper.GetString("Label.ExtractMethod"), null) as ToolStripMenuItem;
            this.extractLocalVariableMenuItem = this.DropDownItems.Add(TextHelper.GetString("Label.ExtractLocalVariable"), null) as ToolStripMenuItem;
			this.delegateMenuItem = this.DropDownItems.Add(TextHelper.GetString("Label.DelegateMethods"), null) as ToolStripMenuItem;
            if (createSurroundMenu)
            {
                this.surroundMenu = new SurroundMenu();
                this.DropDownItems.Add(this.surroundMenu);
            }
            this.DropDownItems.Add(new ToolStripSeparator());
            this.generatorMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.InvokeCodeGenerator"), null, null, createSurroundMenu ? Keys.Control | Keys.Shift | Keys.D1 : Keys.None);
            this.DropDownItems.Add(this.generatorMenuItem);
            this.DropDownItems.Add(new ToolStripSeparator());
            this.organizeMenuItem = this.DropDownItems.Add(TextHelper.GetString("Label.OrganizeImports"), null) as ToolStripMenuItem;
            this.truncateMenuItem = this.DropDownItems.Add(TextHelper.GetString("Label.TruncateImports"), null) as ToolStripMenuItem;
        }

        /// <summary>
        /// Accessor to the SurroundMenu
        /// </summary>
        public SurroundMenu SurroundMenu
        {
            get { return this.surroundMenu; }
        }

        /// <summary>
        /// Accessor to the RenameMenuItem
        /// </summary>
        public ToolStripMenuItem RenameMenuItem
        {
            get { return this.renameMenuItem; }
        }

        /// <summary>
        /// Accessor to the TruncateMenuItem
        /// </summary>
        public ToolStripMenuItem TruncateMenuItem
        {
            get { return this.truncateMenuItem; }
        }

        /// <summary>
        /// Accessor to the OrganizeMenuItem
        /// </summary>
        public ToolStripMenuItem OrganizeMenuItem
        {
            get { return this.organizeMenuItem; }
        }

        /// <summary>
        /// Accessor to the ExtractMethodMenuItem
        /// </summary>
        public ToolStripMenuItem ExtractMethodMenuItem
        {
            get { return this.extractMethodMenuItem; }
        }

        /// <summary>
        /// Accessor to the DelegateMenuItem
        /// </summary>
        public ToolStripMenuItem DelegateMenuItem
        {
            get { return this.delegateMenuItem; }
        }

        /// <summary>
        /// Accessor to the ExtractLocalVariableMenuItem
        /// </summary>
        public ToolStripMenuItem ExtractLocalVariableMenuItem
        {
            get { return this.extractLocalVariableMenuItem; }
        }

        /// <summary>
        /// Accessor to the CodeGeneratorMenuItem
        /// </summary>
        public ToolStripMenuItem CodeGeneratorMenuItem
        {
            get { return this.generatorMenuItem; }
        }

    }

}
