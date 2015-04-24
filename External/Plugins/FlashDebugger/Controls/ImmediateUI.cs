using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Helpers;
using ASCompletion.Model;
using flash.tools.debugger;
using flash.tools.debugger.expression;
using PluginCore.Controls;
using PluginCore;

namespace FlashDebugger.Controls
{
    public partial class ImmediateUI : DockPanelControl, ASCompletionListBackend.IBackendFileGetter
    {
        private List<string> history;
        private int historyPos;

        private CompletionListControl completionList;
        private ASCompletionListBackend completionBackend;

        public ImmediateUI()
        {
            this.InitializeComponent();
            this.contextMenuStrip.Renderer = new DockPanelStripRenderer(false);
            this.completionList = new CompletionListControl(new TextBoxTarget(textBox));
            this.completionBackend = new ASCompletionListBackend(completionList, this);
            this.history = new List<string>();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (completionList.Active) return;
            if (e.KeyCode == Keys.Back && this.textBox.GetFirstCharIndexOfCurrentLine() == this.textBox.SelectionStart) e.SuppressKeyPress = true;
            if (e.KeyCode == Keys.Up)
            {
                if (textBox.GetLineFromCharIndex(textBox.SelectionStart) != textBox.Lines.Length - 1 || e.Modifiers > 0)
                    return;
                if (this.historyPos > 0)
                {
                    this.historyPos--;
                    this.textBox.Select(this.textBox.Text.Length, 0);
                    this.textBox.Text = this.textBox.Text.Substring(0, this.textBox.GetFirstCharIndexOfCurrentLine()) + this.history[this.historyPos];
                    try
                    {
                        this.textBox.Select(this.textBox.TextLength, 0);
                        this.textBox.ScrollToCaret();
                    }
                    catch { /* WineMod: not supported */ }
                }
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Down)
            {
                if (textBox.GetLineFromCharIndex(textBox.SelectionStart) != textBox.Lines.Length - 1 || e.Modifiers > 0)
                    return;
                if (this.historyPos + 1 < this.history.Count)
                {
                    this.historyPos++;
                    this.textBox.Select(this.textBox.Text.Length, 0);
                    this.textBox.Text = this.textBox.Text.Substring(0, this.textBox.GetFirstCharIndexOfCurrentLine()) + this.history[this.historyPos];
                    try
                    {
                        this.textBox.Select(this.textBox.TextLength, 0);
                        this.textBox.ScrollToCaret();
                    }
                    catch { /* WineMod: not supported */ }
                }
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                int curLine = this.textBox.GetLineFromCharIndex(this.textBox.SelectionStart);
                string line = "";
                if (curLine < this.textBox.Lines.Length) line = this.textBox.Lines[curLine];
                if (this.textBox.Lines.Length > 0 && !this.textBox.Lines[this.textBox.Lines.Length - 1].Trim().Equals("")) this.textBox.AppendText(Environment.NewLine);
                try
                {
                    if (history.Count == 0 || history[history.Count - 1] != line)
                        this.history.Add(line);
                    this.historyPos = this.history.Count;
                    if (line == "swfs")
                    {
                        this.textBox.AppendText(ProcessSwfs());
                    }
                    else if (line.StartsWith("p "))
                    {
                        this.textBox.AppendText(ProcessExpr(line.Substring(2)));
                    }
                    else if (line.StartsWith("g "))
                    {
                        this.textBox.AppendText(ProcessGlobal(line.Substring(2)));
                    }
                    else
                    {
                        this.textBox.AppendText("Commands: swfs, p <exptr>, g <value id>");
                    }
                }
                catch (NoSuchVariableException ex)
                {
                    this.textBox.AppendText(ex.ToString());
                }
                catch (PlayerDebugException ex)
                {
                    this.textBox.AppendText(ex.ToString());
                }
                catch (PlayerFaultException ex)
                {
                    this.textBox.AppendText(ex.ToString());
                }
                catch (Exception ex)
                {
                    this.textBox.AppendText(!string.IsNullOrEmpty(ex.Message) ? ex.GetType().FullName + ": " + ex.Message : ex.ToString());
                }
                if (this.textBox.Lines.Length > 0) this.textBox.AppendText(Environment.NewLine);
                try
                {
                    this.textBox.Select(this.textBox.TextLength, 0);
                    this.textBox.ScrollToCaret();
                }
                catch { /* WineMod: not supported */ }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Ctrl+Space is detected at the form level instead of the editor level, so when we are docked we need to catch it before
            if (keyData == (Keys.Control | Keys.Space) || keyData == (Keys.Control | Keys.Shift | Keys.Space))
            {
                int curLine = this.textBox.GetLineFromCharIndex(this.textBox.SelectionStart);
                string line = (curLine < this.textBox.Lines.Length) ? this.textBox.Lines[curLine] : "";

                if (curLine != textBox.Lines.Length - 1 || !line.StartsWith("p ") && !line.StartsWith("g "))
                    return false;

                int lineLength = textBox.SelectionStart - textBox.GetFirstCharIndexFromLine(textBox.Lines.Length - 1) - 2;
                if (lineLength < 0)
                    return false;

                ASCompletionListBackend.BackendFileInfo file;
                if (!((ASCompletionListBackend.IBackendFileGetter)this).GetFileInfo(out file))
                    return false;

                if (line.StartsWith("g "))
                {
                    string expression = line.Substring(2, lineLength).TrimStart();
                    if (expression.IndexOfAny(".,_();:[]{}=-+*\'\"\\/|<>?! ".ToCharArray()) > -1)
                        return false;

                    var fileModel = ASFileParser.ParseFile(new FileModel(file.File));

                    var members = new MemberList();

                    // We could use Context.GetTopLevelElements, but neither AS2 or super are supported...
                    members.Add(new MemberModel("this", "", FlagType.Variable | FlagType.Intrinsic, Visibility.Public));

                    // root types & packages
                    var newContext = ASCompletion.Context.ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
                    FileModel baseElements = newContext.ResolvePackage(null, false);
                    if (baseElements != null)
                    {
                        foreach (var m in baseElements.Members.Items)
                        {
                            members.Add(m);
                        }
                        foreach (var m in baseElements.Imports.Items)
                        {
                            if (m.Flags == FlagType.Package) continue;
                            members.Add(m);
                        }
                    }

                    for (int i = fileModel.Classes.Count - 1; i >= 0; i--)
                    {
                        var c = fileModel.Classes[i];
                        if (c.LineFrom <= file.Line && c.LineTo >= file.Line)
                        {
                            foreach (var m in c.Members.Items)
                            {
                                if (m.Access != Visibility.Public || (m.Flags & FlagType.Variable) == 0) continue;
                                members.Add(m);
                            }

                            break;
                        }
                    }

                    members.Sort();

                    var language = ScintillaNet.ScintillaControl.Configuration.GetLanguage(PluginBase.CurrentProject.Language);
                    if (language != null)   // Should we provide some custom string otherwise?
                        completionList.CharacterClass = language.characterclass.Characters;

                    members.Sort();
                    var items = new List<ICompletionListItem>();
                    foreach (var m in members.Items) items.Add(new MemberItem(m));
                    completionList.Show(items, true, expression);

                    return true;
                }

                if (completionBackend.SetCompletionBackend(file, line.Substring(2, lineLength)))
                {
                    if (keyData == (Keys.Control | Keys.Space))
                        completionBackend.ShowAutoCompletionList();
                    else
                        completionBackend.ShowFunctionDetails();

                    return true;
                }

                return false;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private string ProcessSwfs()
        {
            StringBuilder ret = new StringBuilder();

            foreach (SwfInfo info in PluginMain.debugManager.FlashInterface.Session.getSwfs())
            {
                if (info == null) continue;
                ret.Append(info.getPath()).Append("\tswfsize ").Append(info.getSwfSize()).Append("\tprocesscomplete ").Append(info.isProcessingComplete())
                    .Append("\tunloaded ").Append(info.isUnloaded()).Append("\turl ").Append(info.getUrl()).Append("\tsourcecount ")
                    .Append(info.getSourceCount(PluginMain.debugManager.FlashInterface.Session)).AppendLine();
            }
            return ret.ToString();
        }

        private string ProcessExpr(string expr)
        {
            IASTBuilder builder = new ASTBuilder(true);
            ValueExp exp = builder.parse(new java.io.StringReader(expr));
            var ctx = new ExpressionContext(PluginMain.debugManager.FlashInterface.Session, PluginMain.debugManager.FlashInterface.GetFrames()[PluginMain.debugManager.CurrentFrame]);
            var obj = exp.evaluate(ctx);
            if (obj is Variable) return ctx.FormatValue(((Variable)obj).getValue());
            if (obj is Value) return ctx.FormatValue((Value)obj);
            return obj.toString();
        }

        private string ProcessGlobal(string expr)
        {
            var val = PluginMain.debugManager.FlashInterface.Session.getGlobal(expr);
            //var val = PluginMain.debugManager.FlashInterface.Session.getValue(Convert.ToInt64(expr));
            var ctx = new ExpressionContext(PluginMain.debugManager.FlashInterface.Session, PluginMain.debugManager.FlashInterface.GetFrames()[PluginMain.debugManager.CurrentFrame]);
            return ctx.FormatValue(val);
        }

        private void ClearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBox.Clear();
            this.history.Clear();
            this.historyPos = 0;
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBox.Cut();
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBox.Copy();
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBox.Paste();
        }

        #region ASCompletionListBackend.BackendFileInfo Methods

        PluginCore.Helpers.EncodingFileInfo ASCompletionListBackend.IBackendFileGetter.GetFileContent(ASCompletionListBackend.BackendFileInfo file)
        {
            var debugger = PluginMain.debugManager.FlashInterface;
            if (!debugger.isDebuggerStarted || !debugger.isDebuggerSuspended || PluginMain.debugManager.CurrentFrame >= debugger.GetFrames().Length)
                return null;
            var location = debugger.GetFrames()[PluginMain.debugManager.CurrentFrame].getLocation();

            // Could somebody want to pass a file pointing to a file different to the one being debugged? highly unlikely

            var sourceFile = location.getFile();
            var sourceFileText = new StringBuilder();
            if (sourceFile != null)
            {
                for (int i = 1, count = sourceFile.getLineCount(); i <= count; i++)
                {
                    sourceFileText.Append(sourceFile.getLine(i).ToString()).Append(PluginCore.Utilities.LineEndDetector.GetNewLineMarker((int)PluginBase.Settings.EOLMode));
                }
            }

            if (sourceFileText.Length == 0)
            {
                if (file.File != null && System.IO.File.Exists(file.File) &&
                    file.File == PluginMain.debugManager.GetLocalPath(sourceFile))
                {
                    // Notify the user of this case?
                    //MessageBox.Show("Source code no available, but potential matching file found on disk, do you want to use it?");
                    return PluginCore.Helpers.FileHelper.GetEncodingFileInfo(file.File);
                }

                return null;
            }

            // Maybe we should convert from UTF-16 to UTF-8? no problems so far
            return new PluginCore.Helpers.EncodingFileInfo { CodePage = Encoding.UTF8.CodePage, Contents = sourceFileText.ToString() };
        }

        bool ASCompletionListBackend.IBackendFileGetter.GetFileInfo(out ASCompletionListBackend.BackendFileInfo file)
        {
            file = default(ASCompletionListBackend.BackendFileInfo);
            var debugger = PluginMain.debugManager.FlashInterface;
            if (!debugger.isDebuggerStarted || !debugger.isDebuggerSuspended || PluginMain.debugManager.CurrentFrame >= debugger.GetFrames().Length)
                return false;
            var location = debugger.GetFrames()[PluginMain.debugManager.CurrentFrame].getLocation();
            file.File = PluginMain.debugManager.GetLocalPath(location.getFile());
            if (file.File == null && (location.getFile() == null || location.getFile().getLineCount() == 0))
                return false;

            file.Line = location.getLine() - 1;

            return true;
        }

        string ASCompletionListBackend.IBackendFileGetter.GetExpression()
        {
            int curLine = this.textBox.GetLineFromCharIndex(this.textBox.SelectionStart);
            string line = (curLine < this.textBox.Lines.Length) ? this.textBox.Lines[curLine] : "";

            if (curLine != textBox.Lines.Length - 1 || !line.StartsWith("p "))
                return null;

            int lineLength = textBox.SelectionStart - textBox.GetFirstCharIndexFromLine(textBox.Lines.Length - 1) - 2;
            if (lineLength < 0)
                return null;

            return line.Substring(2, lineLength);
        }

        #endregion
        
    }
}
