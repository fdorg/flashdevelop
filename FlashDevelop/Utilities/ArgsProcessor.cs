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
        private static Regex reTabs = new Regex("^\\t+", RegexOptions.Multiline | RegexOptions.Compiled);
        private static Regex reArgs = new Regex("\\$\\(([a-z$]+)\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        
        /// <summary>
        /// Regexes and variables for enhanced arguments
        /// </summary>
        private static Dictionary<String, String> userArgs;
        private static Regex reUserArgs = new Regex("\\$\\$\\(([a-z0-9]+)\\=?([^\\)]+)?\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static Regex reSpecialArgs = new Regex("\\$\\$\\(\\#([a-z]+)\\#=?([^\\)]+)?\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static Regex reEnvArgs = new Regex("\\$\\$\\(\\%([a-z]+)\\%\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        
        /// <summary>
        /// Previously selected text, if the selection canceled
        /// </summary>
        public static String PrevSelText = String.Empty;

        /// <summary>
        /// Previously selected word, some cases need this
        /// </summary>
        public static String PrevSelWord = String.Empty;

        /// <summary>
        /// Gets the FlashDevelop root directory
        /// </summary>
        public static String GetAppDir()
        {
            return PathHelper.AppDir;
        }

        /// <summary>
        /// Gets the user's FlashDevelop directory
        /// </summary>
        public static String GetUserAppDir()
        {
            return PathHelper.UserAppDir;
        }

        /// <summary>
        /// Gets the data file directory
        /// </summary>
        public static String GetBaseDir()
        {
            return PathHelper.BaseDir;
        }

        /// <summary>
        /// Gets the template file directory
        /// </summary>
        public static String GetTemplateDir()
        {
            return PathHelper.TemplateDir;
        }

        /// <summary>
        /// Gets the selected text
        /// </summary>
        public static String GetSelText()
        {
            if (!Globals.CurrentDocument.IsEditable) return String.Empty;
            if (Globals.SciControl.SelText.Length > 0) return Globals.SciControl.SelText;
            else if (PrevSelText.Length > 0) return PrevSelText;
            else return String.Empty;
        }

        /// <summary>
        /// Gets the current word
        /// </summary>
        public static String GetCurWord()
        {
            if (!Globals.CurrentDocument.IsEditable) return String.Empty;
            String curWord = Globals.SciControl.GetWordFromPosition(Globals.SciControl.CurrentPos);
            if (!String.IsNullOrEmpty(curWord)) return curWord;
            else if (PrevSelWord.Length > 0) return PrevSelWord;
            else return String.Empty;
        }

        /// <summary>
        /// Gets the current file
        /// </summary>
        public static String GetCurFile()
        {
            if (!Globals.CurrentDocument.IsEditable) return String.Empty;
            else return Globals.CurrentDocument.FileName;
        }

        /// <summary>
        /// Gets the current file's path or last active path
        /// </summary>
        public static String GetCurDir()
        {
            if (!Globals.CurrentDocument.IsEditable) return Globals.MainForm.WorkingDirectory;
            else return Path.GetDirectoryName(GetCurFile());
        }
        
        /// <summary>
        /// Gets the name of the current file
        /// </summary>
        public static String GetCurFilename()
        {
            if (!Globals.CurrentDocument.IsEditable) return String.Empty;
            else return Path.GetFileName(GetCurFile());
        }

        /// <summary>
        /// Gets the name of the current file without extension
        /// </summary>
        public static String GetCurFilenameNoExt()
        {
            if (!Globals.CurrentDocument.IsEditable) return String.Empty;
            else return Path.GetFileNameWithoutExtension(GetCurFile());
        }

        /// <summary>
        /// Gets the timestamp
        /// </summary>
        public static String GetTimestamp()
        {
            return DateTime.Now.ToString("g");
        }
        
        /// <summary>
        /// Gets the desktop path
        /// </summary>
        public static String GetDesktopDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }
        
        /// <summary>
        /// Gets the system path
        /// </summary>
        public static String GetSystemDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.System);
        }
        
        /// <summary>
        /// Gets the program files path
        /// </summary>
        public static String GetProgramsDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }
        
        /// <summary>
        /// Gets the users personal files path
        /// </summary>
        public static String GetPersonalDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
        
        /// <summary>
        /// Gets the working directory
        /// </summary>
        public static String GetWorkingDir()
        {
            return Globals.MainForm.WorkingDirectory;
        }
        
        /// <summary>
        /// Gets the user selected file for opening
        /// </summary>
        public static String GetOpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = GetCurDir();
            ofd.Multiselect = false;
            if (ofd.ShowDialog(Globals.MainForm) == DialogResult.OK) return ofd.FileName;
            else return String.Empty;
        }
        
        /// <summary>
        /// Gets the user selected file for saving
        /// </summary>
        public static String GetSaveFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = GetCurDir();
            if (sfd.ShowDialog(Globals.MainForm) == DialogResult.OK) return sfd.FileName;
            else return String.Empty;
        }
        
        /// <summary>
        /// Gets the user selected folder
        /// </summary>
        public static String GetOpenDir()
        {
            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            if (fbd.ShowDialog(Globals.MainForm) == DialogResult.OK) return fbd.SelectedPath;
            else return String.Empty;
        }
        
        /// <summary>
        /// Gets the clipboard text
        /// </summary>
        public static String GetClipboard()
        {
            IDataObject cbdata = Clipboard.GetDataObject();
            if (cbdata.GetDataPresent("System.String", true)) 
            {
                return cbdata.GetData("System.String", true).ToString();
            }
            else return String.Empty;
        }

        /// <summary>
        /// Gets the comment block indent
        /// </summary>
        public static String GetCBI()
        {
            CommentBlockStyle cbs = Globals.Settings.CommentBlockStyle;
            if (cbs == CommentBlockStyle.Indented) return " ";
            else return "";
        }

        /// <summary>
        /// Gets the space or tab character based on settings
        /// </summary>
        public static String GetSTC()
        {
            if (Globals.Settings.UseTabs) return "\t";
            else return " ";
        }

        /// <summary>
        /// Gets the current syntax based on project or current file.
        /// </summary>
        public static String GetCurSyntax()
        {
            if (PluginBase.CurrentProject != null && PluginBase.CurrentProject.Language != "*")
            {
                String syntax = PluginBase.CurrentProject.Language;
                return syntax.ToLower();
            }
            else if (Globals.CurrentDocument.IsEditable)
            {
                ScintillaControl sci = Globals.SciControl;
                return sci.ConfigurationLanguage.ToLower();
            }
            else return String.Empty;
        }

        /// <summary>
        /// Gets the correct coding style line break chars
        /// </summary>
        public static String ProcessCodeStyleLineBreaks(String text)
        {
            String CSLB = "$(CSLB)";
            Int32 nextIndex = text.IndexOf(CSLB);
            if (nextIndex < 0) return text;
            CodingStyle cs = Globals.Settings.CodingStyle;
            if (cs == CodingStyle.BracesOnLine) return text.Replace(CSLB, "");
            Int32 eolMode = (Int32)Globals.Settings.EOLMode;
            String lineBreak = LineEndDetector.GetNewLineMarker(eolMode);
            String result = ""; Int32 currentIndex = 0;
            while (nextIndex >= 0)
            {
                result += text.Substring(currentIndex, nextIndex - currentIndex) + lineBreak + GetLineIndentation(text, nextIndex);
                currentIndex = nextIndex + CSLB.Length;
                nextIndex = text.IndexOf(CSLB, currentIndex);
            }
            return result + text.Substring(currentIndex);
        }

        /// <summary>
        /// Gets the line intendation from the text
        /// </summary>
        public static String GetLineIndentation(String text, Int32 position)
        {
            Char c;
            Int32 startPos = position;
            while (startPos > 0)
            {
                c = text[startPos];
                if (c == 10 || c == 13) break;
                startPos--;
            }
            Int32 endPos = ++startPos;
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
        private static String GetLocale()
        {
            return Globals.Settings.LocaleVersion.ToString();
        }

        /// <summary>
        /// Processes the argument String variables
        /// </summary>
        public static String ProcessString(String args, Boolean dispatch)
        {
            try
            {
                String result = args;
                if (result == null) return String.Empty;
                result = ProcessCodeStyleLineBreaks(result);
                if (!Globals.Settings.UseTabs) result = reTabs.Replace(result, new MatchEvaluator(ReplaceTabs));
                result = reArgs.Replace(result, new MatchEvaluator(ReplaceVars));
                if (!dispatch || result.IndexOf('$') < 0) return result;
                TextEvent te = new TextEvent(EventType.ProcessArgs, result);
                EventManager.DispatchEvent(Globals.MainForm, te);
                result = ReplaceArgsWithGUI(te.Value);
                PrevSelWord = String.Empty;
                PrevSelText = String.Empty;
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return String.Empty;
            }
        }

        /// <summary>
        /// Match evaluator for tabs
        /// </summary>
        public static String ReplaceTabs(Match match)
        {
            return new String(' ', match.Length * Globals.Settings.IndentSize);
        }
        
        /// <summary>
        /// Match evaluator for vars
        /// </summary>
        public static String ReplaceVars(Match match)
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
            else return match.Value;
        }
        
        /// <summary>
        /// Replaces the enchanced arguments with gui
        /// </summary>
        public static String ReplaceArgsWithGUI(String args)
        {
            if (args.IndexOf("$$(") < 0) return args;
            if (reEnvArgs.IsMatch(args)) // Environmental arguments
            {
                args = reEnvArgs.Replace(args, new MatchEvaluator(ReplaceEnvArgs));
            }
            if (reSpecialArgs.IsMatch(args)) // Special arguments
            {
                args = reSpecialArgs.Replace(args, new MatchEvaluator(ReplaceSpecialArgs));
            }
            if (reUserArgs.IsMatch(args)) // User arguments
            {
                ArgReplaceDialog rvd = new ArgReplaceDialog(args, reUserArgs);
                userArgs = rvd.Dictionary; // Save dictionary temporarily...
                if (rvd.ShowDialog() == DialogResult.OK)
                {
                    args = reUserArgs.Replace(args, new MatchEvaluator(ReplaceUserArgs));
                }
                else args = reUserArgs.Replace(args, new MatchEvaluator(ReplaceWithEmpty));
            }
            return args;
        }

        /// <summary>
        /// Match evaluator for to clear args
        /// </summary>
        public static String ReplaceWithEmpty(Match match)
        {
            return String.Empty;
        }

        /// <summary>
        /// Match evaluator for User Arguments
        /// </summary>
        public static String ReplaceUserArgs(Match match)
        {
            if (match.Groups.Count > 0) return userArgs[match.Groups[1].Value];
            else return match.Value;
        }

        /// <summary>
        /// Match evaluator for Environment Variables
        /// </summary>
        public static String ReplaceEnvArgs(Match match)
        {
            if (match.Groups.Count > 0) return Environment.GetEnvironmentVariable(match.Groups[1].Value);
            else return match.Value;
        }

        /// <summary>
        /// Match evaluator for Special Arguments
        /// </summary>
        public static String ReplaceSpecialArgs(Match match)
        {
            if (match.Groups.Count > 0)
            {
                switch (match.Groups[1].Value.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "DATETIME":
                    {
                        String dateFormat = "";
                        if (match.Groups.Count == 3) dateFormat = match.Groups[2].Value;
                        return (DateTime.Now.ToString(dateFormat));
                    }
                }
            }
            return match.Value;
        }

    }
    
}
