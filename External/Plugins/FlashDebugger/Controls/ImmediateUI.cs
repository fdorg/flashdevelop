using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using flash.tools.debugger;
using flash.tools.debugger.expression;
using PluginCore;
using PluginCore.Controls;

namespace FlashDebugger.Controls
{
    public partial class ImmediateUI : DockPanelControl
    {
        private List<string> history;
        private int historyPos;

        public ImmediateUI()
        {
            this.AutoKeyHandling = true;
            this.InitializeComponent();
            this.contextMenuStrip.Renderer = new DockPanelStripRenderer(false);
            this.history = new List<string>();
            ScrollBarEx.Attach(textBox);
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back && this.textBox.GetFirstCharIndexOfCurrentLine() == this.textBox.SelectionStart) e.SuppressKeyPress = true;
            if (e.KeyCode == Keys.Up && this.historyPos > 0)
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
            if (e.KeyCode == Keys.Down && this.historyPos + 1 < this.history.Count)
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
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) e.SuppressKeyPress = true;
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
                    else if (line.StartsWithOrdinal("p "))
                    {
                        this.textBox.AppendText(processExpr(line.Substring(2)));
                    }
                    else if (line.StartsWithOrdinal("g "))
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
                try
                {
                    this.textBox.Select(this.textBox.TextLength, 0);
                    this.textBox.ScrollToCaret();
                }
                catch { /* WineMod: not supported */ }
            }
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

    }
}
