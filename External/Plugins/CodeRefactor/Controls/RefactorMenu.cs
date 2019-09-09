// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Drawing;
using System.Windows.Forms;
using ASCompletion;
using ASCompletion.Context;
using PluginCore;
using PluginCore.Localization;

namespace CodeRefactor.Controls
{
    public class RefactorMenu : ToolStripMenuItem
    {
        public RefactorMenu(bool createSurroundMenu)
        {
            Text = TextHelper.GetString("Label.Refactor");
            RenameMenuItem = DropDownItems.Add(TextHelper.GetString("Label.Rename")) as ToolStripMenuItem;
            RenameMenuItem.Image = PluginBase.MainForm.FindImage("331");
            MoveMenuItem = DropDownItems.Add(TextHelper.GetString("Label.Move")) as ToolStripMenuItem;
            ExtractMethodMenuItem = DropDownItems.Add(TextHelper.GetString("Label.ExtractMethod"), null) as ToolStripMenuItem;
            ExtractLocalVariableMenuItem = DropDownItems.Add(TextHelper.GetString("Label.ExtractLocalVariable"), null) as ToolStripMenuItem;
            DelegateMenuItem = DropDownItems.Add(TextHelper.GetString("Label.DelegateMethods"), null) as ToolStripMenuItem;
            if (createSurroundMenu)
            {
                SurroundMenu = new SurroundMenu();
                DropDownItems.Add(SurroundMenu);
            }
            DropDownItems.Add(new ToolStripSeparator());
            CodeGeneratorMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.InvokeCodeGenerator"), null, null, createSurroundMenu ? Keys.Control | Keys.Shift | Keys.D1 : Keys.None);
            DropDownItems.Add(CodeGeneratorMenuItem);
            DropDownItems.Add(new ToolStripSeparator());
            OrganizeMenuItem = DropDownItems.Add(TextHelper.GetString("Label.OrganizeImports"), null) as ToolStripMenuItem;
            OrganizeMenuItem.Image = Overlay(ASContext.Panel.GetIcon(PluginUI.ICON_PACKAGE), "-1|22|4|4");
            TruncateMenuItem = DropDownItems.Add(TextHelper.GetString("Label.TruncateImports"), null) as ToolStripMenuItem;
            TruncateMenuItem.Image = Overlay(ASContext.Panel.GetIcon(PluginUI.ICON_PACKAGE), "-1|18|4|4");
            DropDownItems.Add(new ToolStripSeparator());
            BatchMenuItem = DropDownItems.Add(TextHelper.GetString("Label.BatchProcess"), null) as ToolStripMenuItem;
        }

        /// <summary>
        /// Accessor to the SurroundMenu
        /// </summary>
        public SurroundMenu SurroundMenu { get; }

        /// <summary>
        /// Accessor to the BatchMenuItem
        /// </summary>
        public ToolStripMenuItem BatchMenuItem { get; }

        /// <summary>
        /// Accessor to the RenameMenuItem
        /// </summary>
        public ToolStripMenuItem RenameMenuItem { get; }

        /// <summary>
        /// Accessor to the MoveMenuItem
        /// </summary>
        public ToolStripMenuItem MoveMenuItem { get; }

        /// <summary>
        /// Accessor to the TruncateMenuItem
        /// </summary>
        public ToolStripMenuItem TruncateMenuItem { get; }

        /// <summary>
        /// Accessor to the OrganizeMenuItem
        /// </summary>
        public ToolStripMenuItem OrganizeMenuItem { get; }

        /// <summary>
        /// Accessor to the ExtractMethodMenuItem
        /// </summary>
        public ToolStripMenuItem ExtractMethodMenuItem { get; }

        /// <summary>
        /// Accessor to the DelegateMenuItem
        /// </summary>
        public ToolStripMenuItem DelegateMenuItem { get; }

        /// <summary>
        /// Accessor to the ExtractLocalVariableMenuItem
        /// </summary>
        public ToolStripMenuItem ExtractLocalVariableMenuItem { get; }

        /// <summary>
        /// Accessor to the CodeGeneratorMenuItem
        /// </summary>
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
