using System;
using System.Text;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using ScintillaNet;

namespace FlashDevelop.Managers
{
    class ButtonManager
    {
        /// <summary>
        /// Updates the flagged buttons
        /// </summary>
        public static void UpdateFlaggedButtons()
        {
            Int32 count = StripBarManager.Items.Count;
            if (Globals.CurrentDocument == null) return;
            for (Int32 i = 0; i < count; i++)
            {
                ToolStripItem item = StripBarManager.Items[i];
                String[] actions = ((ItemData)item.Tag).Flags.Split('+');
                for (Int32 j = 0; j < actions.Length; j++)
                {
                    Boolean value = ValidateFlagAction(item, actions[j]);
                    ExecuteFlagAction(item, actions[j], value);
                }
            }
        }

        /// <summary>
        /// Checks the flagged CommandBar item
        /// </summary>
        public static Boolean ValidateFlagAction(ToolStripItem item, String action)
        {
            IMainForm mainForm = PluginBase.MainForm;
            ITabbedDocument document = mainForm.CurrentDocument;
            ScintillaControl sci = document.SciControl;
            if (action.Contains("!IsEditable"))
            {
                if (document.IsEditable) return false;
            }
            else if (action.Contains("IsEditable"))
            {
                if (!document.IsEditable) return false;
            }
            if (action.Contains("!IsSplitted"))
            {
                if (document.IsSplitted) return false;
            }
            else if (action.Contains("IsSplitted"))
            {
                if (!document.IsSplitted) return false;
            }
            if (action.Contains("!IsModified"))
            {
                if (document.IsModified) return false;
            }
            else if (action.Contains("IsModified"))
            {
                if (!document.IsModified) return false;
            }
            if (action.Contains("!IsUntitled"))
            {
                if (document.IsUntitled) return false;
            }
            else if (action.Contains("IsUntitled"))
            {
                if (!document.IsUntitled) return false;
            }
            if (action.Contains("!HasBookmarks"))
            {
                if (document.HasBookmarks) return false;
            }
            else if (action.Contains("HasBookmarks"))
            {
                if (!document.HasBookmarks) return false;
            }
            if (action.Contains("!IsAloneInPane"))
            {
                if (document.IsAloneInPane) return false;
            }
            else if (action.Contains("IsAloneInPane"))
            {
                if (!document.IsAloneInPane) return false;
            }
            if (action.Contains("!HasModified"))
            {
                if (mainForm.HasModifiedDocuments) return false;
            }
            else if (action.Contains("HasModified"))
            {
                if (!mainForm.HasModifiedDocuments) return false;
            }
            if (action.Contains("!HasClosedDocs"))
            {
                if (OldTabsManager.OldTabs.Count > 0) return false;
            }
            else if (action.Contains("HasClosedDocs"))
            {
                if (!(OldTabsManager.OldTabs.Count > 0)) return false;
            }
            if (action.Contains("!ProcessIsRunning"))
            {
                if (mainForm.ProcessIsRunning) return false;
            }
            else if (action.Contains("ProcessIsRunning"))
            {
                if (!mainForm.ProcessIsRunning) return false;
            }
            if (action.Contains("!StandaloneMode"))
            {
                if (mainForm.StandaloneMode) return false;
            }
            else if (action.Contains("StandaloneMode"))
            {
                if (!mainForm.StandaloneMode) return false;
            }
            if (action.Contains("!MultiInstanceMode"))
            {
                if (mainForm.MultiInstanceMode) return false;
            }
            else if (action.Contains("MultiInstanceMode"))
            {
                if (!mainForm.MultiInstanceMode) return false;
            }
            if (action.Contains("!IsFullScreen"))
            {
                if (mainForm.IsFullScreen) return false;
            }
            else if (action.Contains("IsFullScreen"))
            {
                if (!mainForm.IsFullScreen) return false;
            }
            if (action.Contains("!IsOnlyInstance"))
            {
                if (mainForm.GetInstanceCount() == 1) return false;
            }
            else if (action.Contains("IsOnlyInstance"))
            {
                if (mainForm.GetInstanceCount() > 1) return false;
            }
            if (action.Contains("TracksBoolean"))
            {
                Boolean value = (Boolean)Globals.Settings.GetValue(((ItemData)item.Tag).Tag);
                if (!value) return false;
            }
            if (sci != null)
            {
                if (action.Contains("!CanUndo"))
                {
                    if (sci.CanUndo) return false;
                }
                else if (action.Contains("CanUndo"))
                {
                    if (!sci.CanUndo) return false;
                }
                if (action.Contains("!CanRedo"))
                {
                    if (sci.CanRedo) return false;
                }
                else if (action.Contains("CanRedo"))
                {
                    if (!sci.CanRedo) return false;
                }
                if (action.Contains("!CanPaste"))
                {
                    if (sci.CanPaste) return false;
                }
                else if (action.Contains("CanPaste"))
                {
                    if (!sci.CanPaste) return false;
                }
                if (action.Contains("!HasSelection"))
                {
                    if (sci.SelText.Length > 0) return false;
                }
                else if (action.Contains("HasSelection"))
                {
                    if (sci.SelText.Length == 0) return false;
                }
                if (action.Contains("!SaveBOM"))
                {
                    if (document.SciControl.SaveBOM) return false;
                }
                else if (action.Contains("SaveBOM"))
                {
                    if (!document.SciControl.SaveBOM) return false;
                }
                if (action.Contains("!IsUnicode"))
                {
                    if (ScintillaManager.IsUnicode(document.SciControl.Encoding.CodePage)) return false;
                }
                else if (action.Contains("IsUnicode"))
                {
                    if (!ScintillaManager.IsUnicode(document.SciControl.Encoding.CodePage)) return false;
                }
                if (action.Contains("SyntaxIs?"))
                {
                    String[] chunks = action.Split('?');
                    if (chunks.Length == 2)
                    {
                        String language = document.SciControl.ConfigurationLanguage;
                        if (chunks[chunks.Length - 1] != language.ToUpper()) return false;
                    }
                }
                if (action.Contains("DistroIs?"))
                {
                    String[] chunks = action.Split('?');
                    if (chunks.Length == 2)
                    {
                        String distro = DistroConfig.DISTRIBUTION_NAME;
                        if (chunks[chunks.Length - 1] != distro) return false;
                    }
                }
                if (action.Contains("IsActiveSyntax"))
                {
                    String language = document.SciControl.ConfigurationLanguage;
                    if (((ItemData)item.Tag).Tag != language) return false;
                }
                if (action.Contains("IsActiveEncoding"))
                {
                    Int32 codepage = document.SciControl.Encoding.CodePage;
                    if (codepage == Encoding.Default.CodePage) codepage = 0;
                    if (((ItemData)item.Tag).Tag != codepage.ToString()) return false;
                }
                if (action.Contains("IsActiveEOL"))
                {
                    Int32 eolMode = document.SciControl.EOLMode;
                    if (((ItemData)item.Tag).Tag != eolMode.ToString()) return false;
                }
                if (action.Contains("IsDefaultEncoding"))
                {
                    Int32 codepage = document.SciControl.Encoding.CodePage;
                    if (codepage != Encoding.Default.CodePage) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Modifies the specified CommandBar item
        /// </summary>
        public static void ExecuteFlagAction(ToolStripItem item, String action, Boolean value)
        {
            if (action.StartsWithOrdinal("Check:"))
            {
                if (item is ToolStripMenuItem)
                {
                    ((ToolStripMenuItem)item).Checked = value;
                }
            }
            else if (action.StartsWithOrdinal("Uncheck:"))
            {
                if (item is ToolStripMenuItem)
                {
                    ((ToolStripMenuItem)item).Checked = !value;
                }
            }
            else if (action.StartsWithOrdinal("Enable:"))
            {
                item.Enabled = value;
            }
            else if (action.StartsWithOrdinal("Disable:"))
            {
                item.Enabled = !value;
            }
            else if (action.StartsWithOrdinal("Visible:"))
            {
                item.Visible = value;
            }
            else if (action.StartsWithOrdinal("Invisible:"))
            {
                item.Visible = !value;
            }
        }

        /// <summary>
        /// Populates the reopen menu from the documents class
        /// </summary>
        public static void PopulateReopenMenu()
        {
            try
            {
                ToolStripMenuItem reopenMenu = (ToolStripMenuItem)StripBarManager.FindMenuItem("ReopenMenu");
                reopenMenu.DropDownItems.Clear();
                for (Int32 i = 0; i < Globals.PreviousDocuments.Count; i++)
                {
                    String file = Globals.PreviousDocuments[i];
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Click += new EventHandler(Globals.MainForm.Reopen);
                    item.Tag = file; item.Text = PathHelper.GetCompactPath(file);
                    if (i < 15) reopenMenu.DropDownItems.Add(item);
                    else Globals.PreviousDocuments.Remove(file);
                }
                if (Globals.PreviousDocuments.Count > 0)
                {
                    String cleanLabel = TextHelper.GetString("Label.CleanReopenList");
                    String clearLabel = TextHelper.GetString("Label.ClearReopenList");
                    reopenMenu.DropDownItems.Add(new ToolStripSeparator());
                    reopenMenu.DropDownItems.Add(new ToolStripMenuItem(cleanLabel, null, new EventHandler(Globals.MainForm.CleanReopenList)));
                    reopenMenu.DropDownItems.Add(new ToolStripMenuItem(clearLabel, null, new EventHandler(Globals.MainForm.ClearReopenList)));
                    reopenMenu.Enabled = true;
                }
                else reopenMenu.Enabled = false;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Adds a new menu item to the reopen menu
        /// </summary>
        public static void AddNewReopenMenuItem(String file)
        {
            try
            {
                if (Globals.PreviousDocuments.Contains(file))
                {
                    Globals.PreviousDocuments.Remove(file);
                }
                Globals.PreviousDocuments.Insert(0, file);
                PopulateReopenMenu();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Gets the active encoding name from the current codepage
        /// </summary>
        public static String GetActiveEncodingName()
        {
            ITabbedDocument document = Globals.CurrentDocument;
            if (document != null && document.IsEditable)
            {
                Int32 codepage = document.SciControl.Encoding.CodePage;
                EncodingFileInfo info = FileHelper.GetEncodingFileInfo(document.FileName);
                if (codepage == info.CodePage)
                {
                    if (ScintillaManager.IsUnicode(info.CodePage))
                    {
                        String name = "Unicode (" + info.Charset + ")";
                        return info.ContainsBOM ? name + " (BOM)" : name;
                    }
                    else
                    {
                        String name = TextHelper.GetStringWithoutMnemonics("Label.8Bits");
                        return name + " (" + info.Charset + ")";
                    }
                }
                else // Opened in different encoding...
                {
                    Boolean hasBOM = document.SciControl.SaveBOM;
                    if (codepage == Encoding.UTF8.CodePage)
                    {
                        return GetLabelAsPlainText("Label.UTF8", true, hasBOM);
                    }
                    else if (codepage == Encoding.UTF7.CodePage)
                    {
                        return GetLabelAsPlainText("Label.UTF7", true, hasBOM);
                    }
                    else if (codepage == Encoding.BigEndianUnicode.CodePage)
                    {
                        return GetLabelAsPlainText("Label.BigEndian", true, hasBOM);
                    }
                    else if (codepage == Encoding.Unicode.CodePage)
                    {
                        return GetLabelAsPlainText("Label.LittleEndian", true, hasBOM);
                    }
                    else return GetLabelAsPlainText("Label.8Bits", false, false);
                }
            }
            else return TextHelper.GetString("Info.Unknown");
        }

        /// <summary>
        /// Gets a label as plain text by removing the accelerator key
        /// </summary>
        public static String GetLabelAsPlainText(String name, Boolean unicode, Boolean hasBOM)
        {
            String label = TextHelper.GetStringWithoutMnemonics(name);
            if (unicode) label = "Unicode (" + label.ToLower() + ")";
            return hasBOM ? label + " (BOM)" : label;
        }

    }

}
