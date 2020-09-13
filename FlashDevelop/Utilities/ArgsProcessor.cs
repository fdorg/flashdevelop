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

namespace FlashDevelop.Utilities
{
    public class ArgsProcessor
    {
        /// <summary>
        /// Regexes for tab and var replacing
        /// </summary>
        static readonly Regex reTabs = new Regex("^\\t+", RegexOptions.Multiline | RegexOptions.Compiled);

        static readonly Regex reArgs = new Regex("\\$\\(([a-z$]+)\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        
        /// <summary>
        /// Regexes and variables for enhanced arguments
        /// </summary>
        static Dictionary<string, string> userArgs;

        static readonly Regex reUserArgs = new Regex("\\$\\$\\(([a-z0-9]+)\\=?([^\\)]+)?\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        static readonly Regex reSpecialArgs = new Regex("\\$\\$\\(\\#([a-z]+)\\#=?([^\\)]+)?\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        static readonly Regex reEnvArgs = new Regex("\\$\\$\\(\\%([a-z]+)\\%\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        
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
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci is null) return string.Empty;
            if (sci.SelTextSize > 0) return sci.SelText;
            if (PrevSelText.Length > 0) return PrevSelText;
            return string.Empty;
        }

        /// <summary>
        /// Gets the current word
        /// </summary>
        public static string GetCurWord()
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci is null) return string.Empty;
            var word = sci.GetWordFromPosition(sci.CurrentPos);
            if (!string.IsNullOrEmpty(word)) return word;
            if (PrevSelWord.Length > 0) return PrevSelWord;
            return string.Empty;
        }

        /// <summary>
        /// Gets the current file
        /// </summary>
        public static string GetCurFile()
        {
            return PluginBase.MainForm.CurrentDocument.SciControl is { } sci
                ? sci.FileName
                : string.Empty;
        }

        /// <summary>
        /// Gets the current file's path or last active path
        /// </summary>
        public static string GetCurDir()
        {
            return PluginBase.MainForm.CurrentDocument.IsEditable
                ? Path.GetDirectoryName(GetCurFile())
                : PluginBase.MainForm.WorkingDirectory;
        }
        
        /// <summary>
        /// Gets the name of the current file
        /// </summary>
        public static string GetCurFilename()
        {
            return PluginBase.MainForm.CurrentDocument.IsEditable
                ? Path.GetFileName(GetCurFile())
                : string.Empty;
        }

        /// <summary>
        /// Gets the name of the current file without extension
        /// </summary>
        public static string GetCurFilenameNoExt()
        {
            return PluginBase.MainForm.CurrentDocument.IsEditable
                ? Path.GetFileNameWithoutExtension(GetCurFile())
                : string.Empty;
        }

        /// <summary>
        /// Gets the timestamp
        /// </summary>
        public static string GetTimestamp() => DateTime.Now.ToString("g");

        /// <summary>
        /// Gets the desktop path
        /// </summary>
        public static string GetDesktopDir() => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        /// <summary>
        /// Gets the system path
        /// </summary>
        public static string GetSystemDir() => Environment.GetFolderPath(Environment.SpecialFolder.System);

        /// <summary>
        /// Gets the program files path
        /// </summary>
        public static string GetProgramsDir() => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

        /// <summary>
        /// Gets the users personal files path
        /// </summary>
        public static string GetPersonalDir() => Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        /// <summary>
        /// Gets the working directory
        /// </summary>
        public static string GetWorkingDir() => PluginBase.MainForm.WorkingDirectory;

        /// <summary>
        /// Gets the user selected file for opening
        /// </summary>
        public static string GetOpenFile()
        {
            using var dialog = new OpenFileDialog {InitialDirectory = GetCurDir(), Multiselect = false};
            return dialog.ShowDialog(PluginBase.MainForm) == DialogResult.OK
                ? dialog.FileName
                : string.Empty;
        }
        
        /// <summary>
        /// Gets the user selected file for saving
        /// </summary>
        public static string GetSaveFile()
        {
            using var dialog = new SaveFileDialog {InitialDirectory = GetCurDir()};
            return dialog.ShowDialog(PluginBase.MainForm) == DialogResult.OK
                ? dialog.FileName
                : string.Empty;
        }
        
        /// <summary>
        /// Gets the user selected folder
        /// </summary>
        public static string GetOpenDir()
        {
            using var dialog = new VistaFolderBrowserDialog {RootFolder = Environment.SpecialFolder.MyComputer};
            return dialog.ShowDialog(PluginBase.MainForm) == DialogResult.OK
                ? dialog.SelectedPath
                : string.Empty;
        }
        
        /// <summary>
        /// Gets the clipboard text
        /// </summary>
        public static string GetClipboard()
        {
            var data = Clipboard.GetDataObject();
            return data.GetDataPresent("System.String", true)
                ? data.GetData("System.String", true).ToString()
                : string.Empty;
        }

        /// <summary>
        /// Gets the comment block indent
        /// </summary>
        public static string GetCBI() => PluginBase.Settings.CommentBlockStyle == CommentBlockStyle.Indented ? " " : "";

        /// <summary>
        /// Gets the space or tab character based on settings
        /// </summary>
        public static string GetSTC() => PluginBase.Settings.UseTabs ? "\t" : " ";

        /// <summary>
        /// Gets the current syntax based on project or current file.
        /// </summary>
        public static string GetCurSyntax()
        {
            if (PluginBase.CurrentProject != null && PluginBase.CurrentProject.Language != "*")
            {
                return PluginBase.CurrentProject.Language.ToLower();
            }

            if (PluginBase.MainForm.CurrentDocument.SciControl is {} sci)
            {
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
            var nextIndex = text.IndexOfOrdinal(CSLB);
            if (nextIndex < 0) return text;
            var cs = PluginBase.Settings.CodingStyle;
            if (cs == CodingStyle.BracesOnLine) return text.Replace(CSLB, "");
            var eolMode = (int)PluginBase.Settings.EOLMode;
            var lineBreak = LineEndDetector.GetNewLineMarker(eolMode);
            var result = "";
            var currentIndex = 0;
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
            int startPos = position;
            while (startPos > 0)
            {
                var c = text[startPos];
                if (c == 10 || c == 13) break;
                startPos--;
            }
            int endPos = ++startPos;
            while (endPos < position)
            {
                var c = text[endPos];
                if (c != '\t' && c != ' ') break;
                endPos++;
            }
            return text.Substring(startPos, endPos-startPos);
        }

        /// <summary>
        /// Gets the current locale
        /// </summary>
        static string GetLocale() => PluginBase.Settings.LocaleVersion.ToString();

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
                var te = new TextEvent(EventType.ProcessArgs, result);
                EventManager.DispatchEvent(PluginBase.MainForm, te);
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
        public static string ReplaceTabs(Match match) => new string(' ', match.Length * PluginBase.Settings.IndentSize);

        /// <summary>
        /// Match evaluator for vars
        /// </summary>
        public static string ReplaceVars(Match match)
        {
            if (match.Groups.Count == 0) return match.Value;
            var name = match.Groups[1].Value;
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
            foreach (var arg in ArgumentDialog.CustomArguments)
            {
                if (name == arg.Key) return arg.Value;
            }
            return "$(" + name + ")";

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
                using var dialog = new ArgReplaceDialog(args, reUserArgs);
                userArgs = dialog.Dictionary; // Save dictionary temporarily...
                if (dialog.ShowDialog() == DialogResult.OK)
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
            return match.Groups.Count > 0
                ? userArgs[match.Groups[1].Value]
                : match.Value;
        }

        /// <summary>
        /// Match evaluator for Environment Variables
        /// </summary>
        public static string ReplaceEnvArgs(Match match)
        {
            return match.Groups.Count > 0
                ? Environment.GetEnvironmentVariable(match.Groups[1].Value)
                : match.Value;
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
                        return DateTime.Now.ToString(dateFormat);
                    }
                }
            }
            return match.Value;
        }
    }
}