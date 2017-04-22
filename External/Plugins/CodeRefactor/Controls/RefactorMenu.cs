using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ASCompletion;
using ASCompletion.Context;
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
            this.OrganizeMenuItem = new ToolStripMenuItemEx(TextHelper.GetString("Label.OrganizeImports"));
            this.OrganizeMenuItem.Image = Overlay(ASContext.Panel.GetIcon(PluginUI.ICON_PACKAGE), "-1|22|4|4");
            this.DropDownItems.Add(this.OrganizeMenuItem);
            this.TruncateMenuItem = new ToolStripMenuItemEx(TextHelper.GetString("Label.TruncateImports"));
            this.TruncateMenuItem.Image = Overlay(ASContext.Panel.GetIcon(PluginUI.ICON_PACKAGE), "-1|18|4|4");
            this.DropDownItems.Add(this.TruncateMenuItem);
            this.DropDownItems.Add(new ToolStripSeparator());
            this.BatchMenuItem = new ToolStripMenuItemEx(TextHelper.GetString("Label.BatchProcess"), null);
            this.DropDownItems.Add(this.BatchMenuItem);
        }

        /// <summary>
        /// Accessor to the SurroundMenu
        /// </summary>
        public SurroundMenu SurroundMenu { get; }

        /// <summary>
        /// Accessor to the BatchMenuItem
        /// </summary>
        [Obsolete("Type is ToolStripMenuItemEx")]
        public ToolStripMenuItem BatchMenuItem { get; }

        /// <summary>
        /// Accessor to the RenameMenuItem
        /// </summary>
        [Obsolete("Type is ToolStripMenuItemEx")]
        public ToolStripMenuItem RenameMenuItem { get; }

        /// <summary>
        /// Accessor to the MoveMenuItem
        /// </summary>
        [Obsolete("Type is ToolStripMenuItemEx")]
        public ToolStripMenuItem MoveMenuItem { get; }

        /// <summary>
        /// Accessor to the TruncateMenuItem
        /// </summary>
        [Obsolete("Type is ToolStripMenuItemEx")]
        public ToolStripMenuItem TruncateMenuItem { get; }

        /// <summary>
        /// Accessor to the OrganizeMenuItem
        /// </summary>
        [Obsolete("Type is ToolStripMenuItemEx")]
        public ToolStripMenuItem OrganizeMenuItem { get; }

        /// <summary>
        /// Accessor to the ExtractMethodMenuItem
        /// </summary>
        [Obsolete("Type is ToolStripMenuItemEx")]
        public ToolStripMenuItem ExtractMethodMenuItem { get; }

        /// <summary>
        /// Accessor to the DelegateMenuItem
        /// </summary>
        [Obsolete("Type is ToolStripMenuItemEx")]
        public ToolStripMenuItem DelegateMenuItem { get; }

        /// <summary>
        /// Accessor to the ExtractLocalVariableMenuItem
        /// </summary>
        [Obsolete("Type is ToolStripMenuItemEx")]
        public ToolStripMenuItem ExtractLocalVariableMenuItem { get; }

        /// <summary>
        /// Accessor to the CodeGeneratorMenuItem
        /// </summary>
        [Obsolete("Type is ToolStripMenuItemEx")]
        public ToolStripMenuItem CodeGeneratorMenuItem { get; }

        private static Image Overlay(Image source, string overlayData)
        {
            var image = new Bitmap(source);
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.DrawImage(PluginBase.MainForm.FindImage16(overlayData), 0, 0);
            }
            return PluginBase.MainForm.GetAutoAdjustedImage(image);
        }
    }

}
