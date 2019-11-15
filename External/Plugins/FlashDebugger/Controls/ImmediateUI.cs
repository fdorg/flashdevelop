// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        private readonly List<string> history;
        private int historyPos;

        public ImmediateUI()
        {
            AutoKeyHandling = true;
            InitializeComponent();
            contextMenuStrip.Renderer = new DockPanelStripRenderer(false);
            history = new List<string>();
            ScrollBarEx.Attach(textBox);
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back && textBox.GetFirstCharIndexOfCurrentLine() == textBox.SelectionStart) e.SuppressKeyPress = true;
            if (e.KeyCode == Keys.Up && historyPos > 0)
            {
                historyPos--;
                textBox.Select(textBox.Text.Length, 0);
                textBox.Text = textBox.Text.Substring(0, textBox.GetFirstCharIndexOfCurrentLine()) + history[historyPos];
                try
                {
                    textBox.Select(textBox.TextLength, 0);
                    textBox.ScrollToCaret();
                }
                catch { /* WineMod: not supported */ }
            }
            if (e.KeyCode == Keys.Down && historyPos + 1 < history.Count)
            {
                historyPos++;
                textBox.Select(textBox.Text.Length, 0);
                textBox.Text = textBox.Text.Substring(0, textBox.GetFirstCharIndexOfCurrentLine()) + history[historyPos];
                try
                {
                    textBox.Select(textBox.TextLength, 0);
                    textBox.ScrollToCaret();
                }
                catch { /* WineMod: not supported */ }
            }
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) e.SuppressKeyPress = true;
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                int curLine = textBox.GetLineFromCharIndex(textBox.SelectionStart);
                //int curLine = 0;
                //int tmp = 0;
                //while (true)
                //{
                //    tmp += this.textBox.Lines[curLine].Length + 2; // newline chars
                //    if (tmp >= this.textBox.SelectionStart) break;
                //    curLine++;
                //}
                string line = "";
                if (curLine<textBox.Lines.Length) line = textBox.Lines[curLine];
                if (textBox.Lines.Length > 0 && !textBox.Lines[textBox.Lines.Length - 1].Trim().Equals("")) textBox.AppendText(Environment.NewLine);
                try
                {
                    history.Add(line);
                    historyPos = history.Count;
                    if (line == "swfs")
                    {
                        textBox.AppendText(processSwfs());
                    }
                    else if (line.StartsWithOrdinal("p "))
                    {
                        textBox.AppendText(processExpr(line.Substring(2)));
                    }
                    else if (line.StartsWithOrdinal("g "))
                    {
                        textBox.AppendText(processGlobal(line.Substring(2)));
                    }
                    else
                    {
                        textBox.AppendText("Commands: swfs, p <exptr>, g <value id>");
                    }
                }
                catch (NoSuchVariableException ex)
                {
                    textBox.AppendText(ex.ToString());
                }
                catch (PlayerDebugException ex)
                {
                    textBox.AppendText(ex.ToString());
                }
                catch (PlayerFaultException ex)
                {
                    textBox.AppendText(ex.ToString());
                }
                catch (Exception ex)
                {
                    textBox.AppendText(!string.IsNullOrEmpty(ex.Message) ? ex.GetType().FullName + ": " + ex.Message : ex.ToString());
                }
                if (textBox.Lines.Length > 0 && !textBox.Lines[textBox.Lines.Length - 1].Trim().Equals("")) textBox.AppendText(Environment.NewLine);
                try
                {
                    textBox.Select(textBox.TextLength, 0);
                    textBox.ScrollToCaret();
                }
                catch { /* WineMod: not supported */ }
            }
        }

        private string processSwfs()
        {
                StringBuilder ret = new StringBuilder();

                foreach (SwfInfo info in PluginMain.debugManager.FlashInterface.Session.getSwfs())
                {
                    if (info is null) continue;
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
            textBox.Clear();
            history.Clear();
            historyPos = 0;
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Paste();
        }

    }
}
