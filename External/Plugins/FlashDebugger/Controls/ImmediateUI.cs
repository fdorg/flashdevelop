﻿using System;
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
            completionTarget.CompletionList = completionList;
            this.history = new List<string>();
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
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
                //int curLine = 0;
                //int tmp = 0;
                //while (true)
                //{
                //    tmp += this.textBox.Lines[curLine].Length + 2; // newline chars
                //    if (tmp >= this.textBox.SelectionStart) break;
                //    curLine++;
                //}
                string line = "";
                if (curLine<this.textBox.Lines.Length) line = this.textBox.Lines[curLine];
                if (this.textBox.Lines.Length > 0 && !this.textBox.Lines[this.textBox.Lines.Length - 1].Trim().Equals("")) this.textBox.AppendText(Environment.NewLine);
                try
                {
                    this.history.Add(line);
                    this.historyPos = this.history.Count;
                    if (line == "swfs")
                    {
                        this.textBox.AppendText(processSwfs());
                    }
                    else if (line.StartsWith("p "))
                    {
                        this.textBox.AppendText(processExpr(line.Substring(2)));
                    }
                    else if (line.StartsWith("g "))
                    {
                        this.textBox.AppendText(processGlobal(line.Substring(2)));
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
                if (this.textBox.Lines.Length > 0 && !this.textBox.Lines[this.textBox.Lines.Length - 1].Trim().Equals("")) this.textBox.AppendText(Environment.NewLine);
                this.textBox.Select(this.textBox.Text.Length, 0);
                this.textBox.ScrollToCaret();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Ctrl+Space is detected at the form level instead of the editor level, so when we are docked we need to catch it before
            if ((keyData & Keys.KeyCode) == Keys.Space && (keyData & Keys.Modifiers & Keys.Control) > 0)
            {
                var debugger = PluginMain.debugManager.FlashInterface;
                if (!debugger.isDebuggerStarted || !debugger.isDebuggerSuspended) return true;
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

                    sci.CurrentPos = sci.PositionFromLine(location.getLine());

                    ASCompletion.Completion.ASExpr expr =
                        ASCompletion.Completion.ASComplete.GetExpressionType(sci, sci.CurrentPos).Context;
                    var list = new List<PluginCore.ICompletionListItem>();
                    if (expr.Value != null)
                    {
                        ASCompletion.Model.MemberList locals = ASCompletion.Completion.ASComplete.ParseLocalVars(expr);
                        foreach (ASCompletion.Model.MemberModel local in locals)
                            list.Add(new ASCompletion.Completion.MemberItem(local));
                    }

                    completionList.Show(list, false);
                }

                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private string processSwfs()
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

        private string processExpr(string expr)
        {
            IASTBuilder builder = new ASTBuilder(true);
            ValueExp exp = builder.parse(new java.io.StringReader(expr));
            var ctx = new ExpressionContext(PluginMain.debugManager.FlashInterface.Session, PluginMain.debugManager.FlashInterface.GetFrames()[PluginMain.debugManager.CurrentFrame]);
            var obj = exp.evaluate(ctx);
            if (obj is Variable) return ctx.FormatValue(((Variable)obj).getValue());
            if (obj is Value) return ctx.FormatValue((Value)obj);
            return obj.toString();
        }

        private string processGlobal(string expr)
        {
            var val = PluginMain.debugManager.FlashInterface.Session.getGlobal(expr);
            //var val = PluginMain.debugManager.FlashInterface.Session.getValue(Convert.ToInt64(expr));
            var ctx = new ExpressionContext(PluginMain.debugManager.FlashInterface.Session, PluginMain.debugManager.FlashInterface.GetFrames()[PluginMain.debugManager.CurrentFrame]);
            return ctx.FormatValue(val);
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBox.Clear();
            this.history.Clear();
            this.historyPos = 0;
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBox.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBox.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBox.Paste();
        }

        private class TextBoxTarget : ICompletionListTarget
        {

            #region ICompletionListTarget Members

            public event EventHandler LostFocus;

            public event ScrollEventHandler Scroll;

            public event KeyEventHandler KeyDown;

            public event MouseEventHandler MouseDown;

            private TextBox _owner;
            public Control Owner
            {
                get { return _owner; }
            }

            public string Text
            {
                get { return _owner.Text; }
            }

            public string SelectedText
            {
                get
                {
                    return _owner.SelectedText;
                }
                set
                {
                    _owner.SelectedText = value;
                }
            }

            public int SelectionEnd
            {
                get
                {
                    return _owner.SelectionStart + _owner.SelectionLength;
                }
                set
                {
                    _owner.SelectionLength = value - _owner.SelectionStart;
                }
            }

            public int SelectionStart
            {
                get
                {
                    return _owner.SelectionStart;
                }
                set
                {
                    _owner.SelectionStart = value;
                }
            }

            public int CurrentPos
            {
                get { return _owner.SelectionStart; }
            }

            public bool IsEditable
            {
                get { return !_owner.ReadOnly; }
            }

            private CompletionListControl _completionList;
            public CompletionListControl CompletionList
            {
                get { return _completionList; }
                set
                {
                    _completionList = value;
                }
            }

            public TextBoxTarget(TextBox owner)
            {
                _owner = owner;
                _owner.KeyDown += Owner_KeyDown;
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
                throw new NotImplementedException();
            }

            #endregion

            private void Owner_KeyDown(object sender, KeyEventArgs e)
            {
                e.SuppressKeyPress = _completionList.HandleKeys(e.KeyData);
            }
        }


    }
}
