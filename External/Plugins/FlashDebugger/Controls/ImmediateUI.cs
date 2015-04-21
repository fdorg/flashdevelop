using System;
using System.Collections.Generic;
using System.Drawing;
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
                        completionBackend.ShowAutoCompletioList();
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
            if (file.File == null)
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
        
        public class TextBoxEx : TextBox
        {
            private const int WM_SYSKEYDOWN = 0x0104;

            public event KeyEventHandler KeyPosted; //Hacky event for MethodCallTip, although with some rather valid use cases
            public event ScrollEventHandler Scroll;

            protected override void DefWndProc(ref Message m)
            {
                base.DefWndProc(ref m);

                if (m.Msg == Win32.WM_KEYDOWN || m.Msg == WM_SYSKEYDOWN)  // If we're worried about performance/GC, we can store latest OnKeyDown e
                    OnKeyPosted(new KeyEventArgs((Keys)((int)m.WParam) | ModifierKeys));
            }

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case Win32.WM_HSCROLL:
                    case Win32.WM_VSCROLL:
                    case Win32.WM_MOUSEWHEEL:
                        WmScroll(ref m);
                        break;
                    default:
                        base.WndProc(ref m);
                        break;
                }

            }

            /// <summary>
            ///     Raises the <see cref="Scroll"/> event.
            /// </summary>
            /// <param name="e">An <see cref="ScrollEventArgs"/> that contains the event data.</param>
            protected virtual void OnScroll(ScrollEventArgs e)
            {
                if (Scroll != null)
                    Scroll(this, e);
            }

            protected virtual void OnKeyPosted(KeyEventArgs e)
            {
                if (KeyPosted != null)
                    KeyPosted(this, e);
            }

            private void WmScroll(ref Message m)
            {
                ScrollOrientation so;
                ScrollEventType set = (ScrollEventType)((short)((int)(long)m.WParam & 0xffff));

                // We're not interested in the actual scroll change right now
                if (m.Msg == Win32.WM_HSCROLL)
                {
                    so = ScrollOrientation.HorizontalScroll;
                    base.WndProc(ref m);
                }
                else
                {
                    so = ScrollOrientation.VerticalScroll;
                    base.WndProc(ref m);
                }

                OnScroll(new ScrollEventArgs(set, 0, 0, so));
            }

        }

        public class TextBoxTarget : ICompletionListHost
        {

            #region ICompletionListTarget Members

            public event EventHandler LostFocus
            {
                add { _owner.LostFocus += value; }
                remove { _owner.LostFocus -= value; }
            }

            private EventHandler positionChanged;
            public event EventHandler PositionChanged
            {
                add
                {
                    if (positionChanged == null || positionChanged.GetInvocationList().Length == 0)
                    {
                        _owner.Scroll += Owner_Scroll;
                        BuildControlHierarchy(_owner);
                    }
                    positionChanged += value;
                }
                remove
                {
                    positionChanged -= value;
                    if (positionChanged == null || positionChanged.GetInvocationList().Length < 1)
                    {
                        _owner.Scroll -= Owner_Scroll;
                        ClearControlHierarchy();
                    }
                }
            }

            public event EventHandler SizeChanged
            {
                add { Owner.SizeChanged += value; }
                remove { Owner.SizeChanged -= value; }
            }

            public event KeyEventHandler KeyDown
            {
                add { _owner.KeyDown += value; }
                remove { _owner.KeyDown -= value; }
            }

            public event KeyEventHandler KeyPosted
            {
                add { _owner.KeyPosted += value; }
                remove { _owner.KeyPosted -= value; }
            }

            public event KeyPressEventHandler KeyPress
            {
                add { _owner.KeyPress += value; }
                remove { _owner.KeyPress -= value; }
            }

            public event MouseEventHandler MouseDown
            {
                add { _owner.MouseDown += value; }
                remove { _owner.MouseDown -= value; }
            }

            private TextBoxEx _owner;
            public Control Owner
            {
                get { return _owner; }
            }

            public string SelectedText
            {
                get { return _owner.SelectedText; }
                set { _owner.SelectedText = value; }
            }

            public int SelectionEnd
            {
                get { return _owner.SelectionStart + _owner.SelectionLength; }
                set { _owner.SelectionLength = value - _owner.SelectionStart; }
            }

            public int SelectionStart
            {
                get { return _owner.SelectionStart; }
                set { _owner.SelectionStart = value; }
            }

            public int CurrentPos
            {
                get { return _owner.SelectionStart; }
            }

            public bool IsEditable
            {
                get { return !_owner.ReadOnly; }
            }

            public TextBoxTarget(TextBoxEx owner)
            {
                _owner = owner;
            }

            public int GetLineFromCharIndex(int pos)
            {
                return _owner.GetLineFromCharIndex(pos);
            }

            public Point GetPositionFromCharIndex(int pos)
            {
                return _owner.GetPositionFromCharIndex(pos == _owner.TextLength ? pos - 1 : pos);
            }

            public int GetLineHeight()
            {
                using (Graphics g = _owner.CreateGraphics())
                {
                    SizeF textSize = g.MeasureString("S", _owner.Font);
                    return (int)Math.Ceiling(textSize.Height);
                }
            }

            public void SetSelection(int start, int end)
            {
                _owner.SelectionStart = start;
                _owner.SelectionLength = end - start;
            }

            public void BeginUndoAction()
            {
                // TODO
            }

            public void EndUndoAction()
            {
                // TODO
            }

            #endregion

            private List<Control> controlHierarchy = new List<Control>();

            private void BuildControlHierarchy(Control current)
            {
                while (current != null)
                {
                    current.LocationChanged += Control_LocationChanged;
                    current.ParentChanged += Control_ParentChanged;
                    controlHierarchy.Add(current);
                    current = current.Parent;
                }
            }

            private void ClearControlHierarchy()
            {
                foreach (var control in controlHierarchy)
                {
                    control.LocationChanged -= Control_LocationChanged;
                    control.ParentChanged -= Control_ParentChanged;
                }
                controlHierarchy.Clear();
            }

            private void Control_LocationChanged(object sender, EventArgs e)
            {
                if (positionChanged != null)
                    positionChanged(sender, e);
            }

            private void Control_ParentChanged(object sender, EventArgs e)
            {
                ClearControlHierarchy();
                BuildControlHierarchy(_owner);
                if (positionChanged != null)
                    positionChanged(sender, e);
            }

            private void Owner_Scroll(object sender, ScrollEventArgs e)
            {
                if (positionChanged != null)
                    positionChanged(sender, e);
            }

        }

    }
}
