using System;
using System.Text;
using System.Windows.Forms;
using FlashDevelop.Settings;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;

namespace FlashDevelop.Managers
{
    class ButtonManager
    {
        /// <summary>
        /// Updates the flagged buttons
        /// </summary>
        public static void UpdateFlaggedButtons()
        {
            if (PluginBase.MainForm.CurrentDocument is null) return;
            foreach (var item in StripBarManager.Items)
            {
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
            var mainForm = PluginBase.MainForm;
            var document = mainForm.CurrentDocument;
            if (document is null) return false;
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
                bool value = (bool)((SettingObject)PluginBase.Settings).GetValue(((ItemData)item.Tag).Tag);
                if (!value) return false;
            }
            var sci = document.SciControl;
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
                    if (sci.SelTextSize > 0) return false;
                }
                else if (action.Contains("HasSelection"))
                {
                    if (sci.SelTextSize == 0) return false;
                }
                if (action.Contains("!SaveBOM"))
                {
                    if (sci.SaveBOM) return false;
                }
                else if (action.Contains("SaveBOM"))
                {
                    if (!sci.SaveBOM) return false;
                }
                if (action.Contains("!IsUnicode"))
                {
                    if (ScintillaManager.IsUnicode(sci.Encoding.CodePage)) return false;
                }
                else if (action.Contains("IsUnicode"))
                {
                    if (!ScintillaManager.IsUnicode(sci.Encoding.CodePage)) return false;
                }
                if (action.Contains("SyntaxIs?"))
                {
                    string[] chunks = action.Split('?');
                    if (chunks.Length == 2)
                    {
                        string language = sci.ConfigurationLanguage;
                        if (chunks[chunks.Length - 1] != language.ToUpper()) return false;
                    }
                }
                if (action.Contains("DistroIs?"))
                {
                    var chunks = action.Split('?');
                    if (chunks.Length == 2)
                    {
                        if (chunks[chunks.Length - 1] != DistroConfig.DISTRIBUTION_NAME) return false;
                    }
                }
                if (action.Contains("IsActiveSyntax"))
                {
                    if (((ItemData)item.Tag).Tag != sci.ConfigurationLanguage) return false;
                }
                if (action.Contains("IsActiveEncoding"))
                {
                    int codepage = sci.Encoding.CodePage;
                    if (codepage == Encoding.Default.CodePage) codepage = 0;
                    if (((ItemData)item.Tag).Tag != codepage.ToString()) return false;
                }
                if (action.Contains("IsActiveEOL"))
                {
                    if (((ItemData)item.Tag).Tag != sci.EOLMode.ToString()) return false;
                }
                if (action.Contains("IsDefaultEncoding"))
                {
                    if (sci.Encoding.CodePage != Encoding.Default.CodePage) return false;
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
                var menu = (ToolStripMenuItem)StripBarManager.FindMenuItem("ReopenMenu");
                menu.DropDownItems.Clear();
                var documents = PluginBase.Settings.PreviousDocuments;
                for (var i = 0; i < documents.Count; i++)
                {
                    var file = documents[i];
                    var item = new ToolStripMenuItem {Tag = file, Text = PathHelper.GetCompactPath(file)};
                    item.Click += Globals.MainForm.Reopen;
                    if (i < ((SettingObject)PluginBase.Settings).MaxRecentFiles) menu.DropDownItems.Add(item);
                    else documents.Remove(file);
                }
                if (PluginBase.Settings.PreviousDocuments.Count > 0)
                {
                    string cleanLabel = TextHelper.GetString("Label.CleanReopenList");
                    string clearLabel = TextHelper.GetString("Label.ClearReopenList");
                    menu.DropDownItems.Add(new ToolStripSeparator());
                    menu.DropDownItems.Add(new ToolStripMenuItem(cleanLabel, null, Globals.MainForm.CleanReopenList));
                    menu.DropDownItems.Add(new ToolStripMenuItem(clearLabel, null, Globals.MainForm.ClearReopenList));
                    menu.Enabled = true;
                }
                else menu.Enabled = false;
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
                var documents = PluginBase.Settings.PreviousDocuments;
                documents.Remove(file);
                documents.Insert(0, file);
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
            if (PluginBase.MainForm.CurrentDocument?.SciControl is {} sci)
            {
                var codepage = sci.Encoding.CodePage;
                var info = FileHelper.GetEncodingFileInfo(sci.FileName);
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
                if (codepage == Encoding.UTF8.CodePage)
                {
                    return GetLabelAsPlainText("Label.UTF8", true, sci.SaveBOM);
                }
                if (codepage == Encoding.UTF7.CodePage)
                {
                    return GetLabelAsPlainText("Label.UTF7", true, sci.SaveBOM);
                }
                if (codepage == Encoding.BigEndianUnicode.CodePage)
                {
                    return GetLabelAsPlainText("Label.BigEndian", true, sci.SaveBOM);
                }
                if (codepage == Encoding.Unicode.CodePage)
                {
                    return GetLabelAsPlainText("Label.LittleEndian", true, sci.SaveBOM);
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