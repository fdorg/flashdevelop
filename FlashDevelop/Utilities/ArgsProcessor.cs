using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FlashDevelop.Dialogs;
using Ookii.Dialogs;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;

namespace FlashDevelop.Utilities
{
    public class ArgsProcessor
    {
        /// <summary>
        /// Regexes for tab and var replacing
        /// </summary>
        private static readonly Regex reTabs = new Regex("^\\t+", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex reArgs = new Regex("\\$\\(([a-z$]+)\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        
        /// <summary>
        /// Regexes and variables for enhanced arguments
        /// </summary>
        private static Dictionary<string, string> userArgs;
        private static readonly Regex reUserArgs = new Regex("\\$\\$\\(([a-z0-9]+)\\=?([^\\)]+)?\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static readonly Regex reSpecialArgs = new Regex("\\$\\$\\(\\#([a-z]+)\\#=?([^\\)]+)?\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static readonly Regex reEnvArgs = new Regex("\\$\\$\\(\\%([a-z]+)\\%\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        
        /// <summary>
        /// Previously selected text, if the selection canceled
        /// </summary>
        public static string PrevSelText = string.Empty;

        /// <summary>
        /// Previously selected word, some cases need this
        /// </summary>
        public static string PrevSelWord = string.Empty;

        /// <summary>
        /// Gets the FlashDevelop root directory
        /// </summary>
        public static string GetAppDir() => PathHelper.AppDir;

        /// <summary>
        /// Gets the user's FlashDevelop directory
        /// </summary>
        public static string GetUserAppDir() => PathHelper.UserAppDir;

        /// <summary>
        /// Gets the data file directory
        /// </summary>
        public static string GetBaseDir() => PathHelper.BaseDir;

        /// <summary>
        /// Gets the template file directory
        /// </summary>
        public static string GetTemplateDir() => PathHelper.TemplateDir;

        /// <summary>
        /// Gets the selected text
        /// </summary>
        public static string GetSelText()
        {
            if (!Globals.CurrentDocument.IsEditable) return string.Empty;
            if (Globals.SciControl.SelText.Length > 0) return Globals.SciControl.SelText;
            if (PrevSelText.Length > 0) return PrevSelText;
            return string.Empty;
        }

        /// <summary>
        /// Gets the current word
        /// </summary>
        public static string GetCurWord()
        {
            if (!Globals.CurrentDocument.IsEditable) return string.Empty;
            string curWord = Globals.SciControl.GetWordFromPosition(Globals.SciControl.CurrentPos);
            if (!string.IsNullOrEmpty(curWord)) return curWord;
            if (PrevSelWord.Length > 0) return PrevSelWord;
            return string.Empty;
        }

        /// <summary>
        /// Gets the current file
        /// </summary>
        public static string GetCurFile()
        {
            if (!Globals.CurrentDocument.IsEditable) return string.Empty;
            return Globals.CurrentDocument.FileName;
        }

        /// <summary>
        /// Gets the current file's path or last active path
        /// </summary>
        public static string GetCurDir()
        {
            if (!Globals.CurrentDocument.IsEditable) return Globals.MainForm.WorkingDirectory;
            return Path.GetDirectoryName(GetCurFile());
        }
        
        /// <summary>
        /// Gets the name of the current file
        /// </summary>
        public static string GetCurFilename()
        {
            if (!Globals.CurrentDocument.IsEditable) return string.Empty;
            return Path.GetFileName(GetCurFile());
        }

        /// <summary>
        /// Gets the name of the current file without extension
        /// </summary>
        public static string GetCurFilenameNoExt()
        {
            if (!Globals.CurrentDocument.IsEditable) return string.Empty;
            return Path.GetFileNameWithoutExtension(GetCurFile());
        }

        /// <summary>
        /// Gets the timestamp
        /// </summary>
        public static string GetTimestamp() => DateTime.Now.ToString("g");

        /// <summary>
        /// Gets the desktop path
        /// </summary>
        public static string GetDesktopDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }
        
        /// <summary>
        /// Gets the system path
        /// </summary>
        public static string GetSystemDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.System);
        }
        
        /// <summary>
        /// Gets the program files path
        /// </summary>
        public static string GetProgramsDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }
        
        /// <summary>
        /// Gets the users personal files path
        /// </summary>
        public static string GetPersonalDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
        
        /// <summary>
        /// Gets the working directory
        /// </summary>
        public static string GetWorkingDir() => Globals.MainForm.WorkingDirectory;

        /// <summary>
        /// Gets the user selected file for opening
        /// </summary>
        public static string GetOpenFile()
        {
            using OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = GetCurDir();
            ofd.Multiselect = false;
            if (ofd.ShowDialog(Globals.MainForm) == DialogResult.OK) return ofd.FileName;
            return string.Empty;
        }
        
        /// <summary>
        /// Gets the user selected file for saving
        /// </summary>
        public static string GetSaveFile()
        {
            using var sfd = new SaveFileDialog {InitialDirectory = GetCurDir()};
            if (sfd.ShowDialog(Globals.MainForm) == DialogResult.OK) return sfd.FileName;
            return string.Empty;
        }
        
        /// <summary>
        /// Gets the user selected folder
        /// </summary>
        public static string GetOpenDir()
        {
            using var fbd = new VistaFolderBrowserDialog {RootFolder = Environment.SpecialFolder.MyComputer};
            if (fbd.ShowDialog(Globals.MainForm) == DialogResult.OK) return fbd.SelectedPath;
            return string.Empty;
        }
        
        /// <summary>
        /// Gets the clipboard text
        /// </summary>
        public static string GetClipboard()
        {
            IDataObject cbdata = Clipboard.GetDataObject();
            if (cbdata.GetDataPresent("System.String", true)) 
            {
                return cbdata.GetData("System.String", true).ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the comment block indent
        /// </summary>
        public static string GetCBI()
        {
            CommentBlockStyle cbs = Globals.Settings.CommentBlockStyle;
            if (cbs == CommentBlockStyle.Indented) return " ";
            return "";
        }

        /// <summary>
        /// Gets the space or tab character based on settings
        /// </summary>
        public static string GetSTC() => Globals.Settings.UseTabs ? "\t" : " ";

        /// <summary>
        /// Gets the current syntax based on project or current file.
        /// </summary>
        public static string GetCurSyntax()
        {
            if (PluginBase.CurrentProject != null && PluginBase.CurrentProject.Language != "*")
            {
                string syntax = PluginBase.CurrentProject.Language;
                return syntax.ToLower();
            }

            if (Globals.CurrentDocument.IsEditable)
            {
                ScintillaControl sci = Globals.SciControl;
                return sci.ConfigurationLanguage.ToLower();
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the correct coding style line break chars
        /// </summary>
        public static string ProcessCodeStyleLineBreaks(string text)
        {
            const string CSLB = "$(CSLB)";
            int nextIndex = text.IndexOfOrdinal(CSLB);
            if (nextIndex < 0) return text;
            CodingStyle cs = PluginBase.Settings.CodingStyle;
            if (cs == CodingStyle.BracesOnLine) return text.Replace(CSLB, "");
            int eolMode = (int)Globals.Settings.EOLMode;
            string lineBreak = LineEndDetector.GetNewLineMarker(eolMode);
            string result = ""; int currentIndex = 0;
            while (nextIndex >= 0)
            {
                result += text.Substring(currentIndex, nextIndex - currentIndex) + lineBreak + GetLineIndentation(text, nextIndex);
                currentIndex = nextIndex + CSLB.Length;
                nextIndex = text.IndexOfOrdinal(CSLB, currentIndex);
            }
            return result + text.Substring(currentIndex);
        }

        /// <summary>
        /// Gets the line intendation from the text
        /// </summary>
        public static string GetLineIndentation(string text, int position)
        {
            char c;
            int startPos = position;
            while (startPos > 0)
            {
                c = text[startPos];
                if (c == 10 || c == 13) break;
                startPos--;
            }
            int endPos = ++startPos;
            while (endPos < position)
            {
                c = text[endPos];
                if (c != '\t' && c != ' ') break;
                endPos++;
            }
            return text.Substring(startPos, endPos-startPos);
        }

        /// <summary>
        /// Gets the current locale
        /// </summary>
        private static string GetLocale() => Globals.Settings.LocaleVersion.ToString();

        /// <summary>
        /// Processes the argument String variables
        /// </summary>
        public static string ProcessString(string args, bool dispatch)
        {
            try
            {
                var result = args;
                if (result is null) return string.Empty;
                result = ProcessCodeStyleLineBreaks(result);
                if (!PluginBase.Settings.UseTabs) result = reTabs.Replace(result, ReplaceTabs);
                result = reArgs.Replace(result, ReplaceVars);
                if (!dispatch || !result.Contains('$')) return result;
                TextEvent te = new TextEvent(EventType.ProcessArgs, result);
                EventManager.DispatchEvent(Globals.MainForm, te);
                result = ReplaceArgsWithGUI(te.Value);
                PrevSelWord = string.Empty;
                PrevSelText = string.Empty;
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Match evaluator for tabs
        /// </summary>
        public static string ReplaceTabs(Match match)
        {
            return new string(' ', match.Length * PluginBase.Settings.IndentSize);
        }
        
        /// <summary>
        /// Match evaluator for vars
        /// </summary>
        public static string ReplaceVars(Match match)
        {
            if (match.Groups.Count > 0)
            {
                string name = match.Groups[1].Value;
                switch (name)
                {
                    case "Quote" : return "\"";
                    case "CBI" : return GetCBI();
                    case "STC" : return GetSTC();
                    case "AppDir" : return GetAppDir();
                    case "UserAppDir" : return GetUserAppDir();
                    case "TemplateDir": return GetTemplateDir();
                    case "BaseDir" : return GetBaseDir();
                    case "SelText" : return GetSelText();
                    case "CurFilename": return GetCurFilename();
                    case "CurFilenameNoExt": return GetCurFilenameNoExt();
                    case "CurFile" : return GetCurFile();
                    case "CurDir" : return GetCurDir();
                    case "CurWord" : return GetCurWord();
                    case "CurSyntax": return GetCurSyntax();
                    case "Timestamp" : return GetTimestamp();
                    case "OpenFile" : return GetOpenFile();
                    case "SaveFile" : return GetSaveFile();
                    case "OpenDir" : return GetOpenDir();
                    case "DesktopDir" : return GetDesktopDir();
                    case "SystemDir" : return GetSystemDir();
                    case "ProgramsDir" : return GetProgramsDir();
                    case "PersonalDir" : return GetPersonalDir();
                    case "WorkingDir" : return GetWorkingDir();
                    case "Clipboard": return GetClipboard();
                    case "Locale": return GetLocale();
                    case "Dollar": return "$";
                }
                foreach (Argument arg in ArgumentDialog.CustomArguments)
                {
                    if (name == arg.Key) return arg.Value;
                }
                return "$(" + name + ")";
            }

            return match.Value;
        }
        
        /// <summary>
        /// Replaces the enchanced arguments with gui
        /// </summary>
        public static string ReplaceArgsWithGUI(string args)
        {
            if (!args.Contains("$$(")) return args;
            if (reEnvArgs.IsMatch(args)) // Environmental arguments
            {
                args = reEnvArgs.Replace(args, ReplaceEnvArgs);
            }
            if (reSpecialArgs.IsMatch(args)) // Special arguments
            {
                args = reSpecialArgs.Replace(args, ReplaceSpecialArgs);
            }
            if (reUserArgs.IsMatch(args)) // User arguments
            {
                using var rvd = new ArgReplaceDialog(args, reUserArgs);
                userArgs = rvd.Dictionary; // Save dictionary temporarily...
                if (rvd.ShowDialog() == DialogResult.OK)
                {
                    args = reUserArgs.Replace(args, ReplaceUserArgs);
                }
                else args = reUserArgs.Replace(args, ReplaceWithEmpty);
            }
            return args;
        }

        /// <summary>
        /// Match evaluator for to clear args
        /// </summary>
        public static string ReplaceWithEmpty(Match match) => string.Empty;

        /// <summary>
        /// Match evaluator for User Arguments
        /// </summary>
        public static string ReplaceUserArgs(Match match)
        {
            if (match.Groups.Count > 0) return userArgs[match.Groups[1].Value];
            return match.Value;
        }

        /// <summary>
        /// Match evaluator for Environment Variables
        /// </summary>
        public static string ReplaceEnvArgs(Match match)
        {
            if (match.Groups.Count > 0) return Environment.GetEnvironmentVariable(match.Groups[1].Value);
            return match.Value;
        }

        /// <summary>
        /// Match evaluator for Special Arguments
        /// </summary>
        public static string ReplaceSpecialArgs(Match match)
        {
            if (match.Groups.Count > 0)
            {
                switch (match.Groups[1].Value.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "DATETIME":
                    {
                        string dateFormat = "";
                        if (match.Groups.Count == 3) dateFormat = match.Groups[2].Value;
                        return (DateTime.Now.ToString(dateFormat));
                    }
                }
            }
            return match.Value;
        }

    }
    
}
