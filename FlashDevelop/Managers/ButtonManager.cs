using System;
using System.Text;
using System.Windows.Forms;
using FlashDevelop.Settings;
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
            int count = StripBarManager.Items.Count;
            if (PluginBase.MainForm.CurrentDocument is null) return;
            for (int i = 0; i < count; i++)
            {
                var item = StripBarManager.Items[i];
                var actions = ((ItemData)item.Tag).Flags.Split('+');
                foreach (var action in actions)
                {
                    bool value = ValidateFlagAction(item, action);
                    ExecuteFlagAction(item, action, value);
                }
            }
        }

        /// <summary>
        /// Checks the flagged CommandBar item
        /// </summary>
        public static bool ValidateFlagAction(ToolStripItem item, string action)
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
                bool value = (bool)((SettingObject)PluginBase.MainForm.Settings).GetValue(((ItemData)item.Tag).Tag);
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
                    string[] chunks = action.Split('?');
                    if (chunks.Length == 2)
                    {
                        string language = document.SciControl.ConfigurationLanguage;
                        if (chunks[chunks.Length - 1] != language.ToUpper()) return false;
                    }
                }
                if (action.Contains("DistroIs?"))
                {
                    string[] chunks = action.Split('?');
                    if (chunks.Length == 2)
                    {
                        string distro = DistroConfig.DISTRIBUTION_NAME;
                        if (chunks[chunks.Length - 1] != distro) return false;
                    }
                }
                if (action.Contains("IsActiveSyntax"))
                {
                    string language = document.SciControl.ConfigurationLanguage;
                    if (((ItemData)item.Tag).Tag != language) return false;
                }
                if (action.Contains("IsActiveEncoding"))
                {
                    int codepage = document.SciControl.Encoding.CodePage;
                    if (codepage == Encoding.Default.CodePage) codepage = 0;
                    if (((ItemData)item.Tag).Tag != codepage.ToString()) return false;
                }
                if (action.Contains("IsActiveEOL"))
                {
                    int eolMode = document.SciControl.EOLMode;
                    if (((ItemData)item.Tag).Tag != eolMode.ToString()) return false;
                }
                if (action.Contains("IsDefaultEncoding"))
                {
                    int codepage = document.SciControl.Encoding.CodePage;
                    if (codepage != Encoding.Default.CodePage) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Modifies the specified CommandBar item
        /// </summary>
        public static void ExecuteFlagAction(ToolStripItem item, string action, bool value)
        {
            if (action.StartsWithOrdinal("Check:"))
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    menuItem.Checked = value;
                }
            }
            else if (action.StartsWithOrdinal("Uncheck:"))
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    menuItem.Checked = !value;
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
                for (int i = 0; i < Globals.PreviousDocuments.Count; i++)
                {
                    string file = Globals.PreviousDocuments[i];
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Click += Globals.MainForm.Reopen;
                    item.Tag = file; item.Text = PathHelper.GetCompactPath(file);
                    if (i < ((SettingObject)PluginBase.MainForm.Settings).MaxRecentFiles) reopenMenu.DropDownItems.Add(item);
                    else Globals.PreviousDocuments.Remove(file);
                }
                if (Globals.PreviousDocuments.Count > 0)
                {
                    string cleanLabel = TextHelper.GetString("Label.CleanReopenList");
                    string clearLabel = TextHelper.GetString("Label.ClearReopenList");
                    reopenMenu.DropDownItems.Add(new ToolStripSeparator());
                    reopenMenu.DropDownItems.Add(new ToolStripMenuItem(cleanLabel, null, Globals.MainForm.CleanReopenList));
                    reopenMenu.DropDownItems.Add(new ToolStripMenuItem(clearLabel, null, Globals.MainForm.ClearReopenList));
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
        public static void AddNewReopenMenuItem(string file)
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
        public static string GetActiveEncodingName()
        {
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document != null && document.IsEditable)
            {
                int codepage = document.SciControl.Encoding.CodePage;
                EncodingFileInfo info = FileHelper.GetEncodingFileInfo(document.FileName);
                if (codepage == info.CodePage)
                {
                    if (ScintillaManager.IsUnicode(info.CodePage))
                    {
                        string name = "Unicode (" + info.Charset + ")";
                        return info.ContainsBOM ? name + " (BOM)" : name;
                    }
                    else
                    {
                        string name = TextHelper.GetStringWithoutMnemonics("Label.8Bits");
                        return name + " (" + info.Charset + ")";
                    }
                }
                bool hasBOM = document.SciControl.SaveBOM;
                if (codepage == Encoding.UTF8.CodePage)
                {
                    return GetLabelAsPlainText("Label.UTF8", true, hasBOM);
                }
                if (codepage == Encoding.UTF7.CodePage)
                {
                    return GetLabelAsPlainText("Label.UTF7", true, hasBOM);
                }
                if (codepage == Encoding.BigEndianUnicode.CodePage)
                {
                    return GetLabelAsPlainText("Label.BigEndian", true, hasBOM);
                }
                if (codepage == Encoding.Unicode.CodePage)
                {
                    return GetLabelAsPlainText("Label.LittleEndian", true, hasBOM);
                }
                return GetLabelAsPlainText("Label.8Bits", false, false);
            }
            return TextHelper.GetString("Info.Unknown");
        }

        /// <summary>
        /// Gets a label as plain text by removing the accelerator key
        /// </summary>
        public static string GetLabelAsPlainText(string name, bool unicode, bool hasBOM)
        {
            string label = TextHelper.GetStringWithoutMnemonics(name);
            if (unicode) label = "Unicode (" + label.ToLower() + ")";
            return hasBOM ? label + " (BOM)" : label;
        }

    }

}
