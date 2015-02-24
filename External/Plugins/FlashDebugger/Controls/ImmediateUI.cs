using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using flash.tools.debugger;
using flash.tools.debugger.expression;
using PluginCore.Controls;

namespace FlashDebugger.Controls
{
    public partial class ImmediateUI : DockPanelControl
    {
        private List<string> history;
        private int historyPos;

        private CompletionListControl completionList;

        public ImmediateUI()
        {
            this.InitializeComponent();
            this.contextMenuStrip.Renderer = new DockPanelStripRenderer(false);
            var completionTarget = new TextBoxTarget(textBox);
            this.completionList = new CompletionListControl(completionTarget);
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
                    this.textBox.Select(this.textBox.Text.Length, 0);
                    this.textBox.ScrollToCaret();
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
                    this.textBox.Select(this.textBox.Text.Length, 0);
                    this.textBox.ScrollToCaret();
                }
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                int curLine = this.textBox.GetLineFromCharIndex(this.textBox.SelectionStart);
                string line = "";
                if (curLine<this.textBox.Lines.Length) line = this.textBox.Lines[curLine];
                if (this.textBox.Lines.Length > 0 && !this.textBox.Lines[this.textBox.Lines.Length - 1].Trim().Equals("")) this.textBox.AppendText(Environment.NewLine);
                try
                {
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
                this.textBox.Select(this.textBox.Text.Length, 0);
                this.textBox.ScrollToCaret();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Ctrl+Space is detected at the form level instead of the editor level, so when we are docked we need to catch it before
            if ((keyData & Keys.KeyCode) == Keys.Space && (keyData & Keys.Modifiers & Keys.Control) > 0)
            {
                int curLine = this.textBox.GetLineFromCharIndex(this.textBox.SelectionStart);
                string line = (curLine < this.textBox.Lines.Length) ? this.textBox.Lines[curLine] : "";

                if (line == "" || !line.StartsWith("p ")) return true;

                var debugger = PluginMain.debugManager.FlashInterface;
                if (!debugger.isDebuggerStarted || !debugger.isDebuggerSuspended || PluginMain.debugManager.CurrentFrame >= debugger.GetFrames().Length) 
                    return true;
                var location = debugger.GetFrames()[PluginMain.debugManager.CurrentFrame].getLocation();
                string file = PluginMain.debugManager.GetLocalPath(location.getFile());
                if (file == null) return true;
                var info = PluginCore.Helpers.FileHelper.GetEncodingFileInfo(file);
                if (info.CodePage == -1) return true;
                using (var sci = new ScintillaNet.ScintillaControl())
                {
                    sci.Text = info.Contents;
                    sci.CodePage = info.CodePage;
                    sci.Encoding = Encoding.GetEncoding(info.CodePage);
                    sci.ConfigurationLanguage = PluginCore.PluginBase.CurrentProject.Language;

                    sci.CurrentPos = sci.PositionFromLine(location.getLine() - 1);
                    string expression = line.Substring(2);
                    sci.SetSel(sci.CurrentPos, sci.CurrentPos);
                    sci.ReplaceSel(expression);

                    ASCompletion.Completion.ASExpr expr =
                        ASCompletion.Completion.ASComplete.GetExpressionType(sci, sci.CurrentPos).Context;
                    var list = new List<PluginCore.ICompletionListItem>();
                    if (expr.Value != null)
                    {
                        ASCompletion.Model.MemberList locals = ASCompletion.Completion.ASComplete.ParseLocalVars(expr);
                        foreach (ASCompletion.Model.MemberModel local in locals)
                            list.Add(new ASCompletion.Completion.MemberItem(local));
                    }
                    completionList.Show(list, true, line.Substring(line.LastIndexOfAny(new[]{' ', '.'}) + 1));
                }

                return true;
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

        private class TextBoxTarget : ICompletionListHost
        {

            #region ICompletionListTarget Members

            public event EventHandler LostFocus
            {
                add { _owner.LostFocus += value; }
                remove { _owner.LostFocus -= value; }
            }

            public event EventHandler PositionChanged;

            public event KeyEventHandler KeyDown
            {
                add { _owner.KeyDown += value; }
                remove { _owner.KeyDown -= value; }
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

            private TextBox _owner;
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

            public TextBoxTarget(TextBox owner)
            {
                _owner = owner;
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
        }


    }
}
