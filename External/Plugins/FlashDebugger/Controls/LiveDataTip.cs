// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using flash.tools.debugger;
using flash.tools.debugger.expression;
using FlashDebugger.Controls;
using java.io;
using PluginCore;
using PluginCore.Controls;
using ScintillaNet;

namespace FlashDebugger
{
    internal class LiveDataTip
    {
        DataTipForm m_ToolTip;
        MouseMessageFilter m_MouseMessageFilter;

        public LiveDataTip() => UITools.Manager.OnMouseHover += Manager_OnMouseHover;

        void Initialize()
        {
            m_ToolTip = new DataTipForm();
            m_ToolTip.Dock = DockStyle.Fill;
            m_ToolTip.Visible = false;
            m_MouseMessageFilter = new MouseMessageFilter();
            m_MouseMessageFilter.AddControls(new Control[] { m_ToolTip, m_ToolTip.DataTree });
            m_MouseMessageFilter.MouseDownEvent += MouseMessageFilter_MouseDownEvent;
            m_MouseMessageFilter.KeyDownEvent += MouseMessageFilter_KeyDownEvent;
            Application.AddMessageFilter(m_MouseMessageFilter);
        }

        public void Show(Point point, Variable variable, string path)
        {
            m_ToolTip.Location = point;
            m_ToolTip.SetVariable(variable, path);
            m_ToolTip.Visible = true;
            m_ToolTip.Location = point;
            m_ToolTip.BringToFront();
            m_ToolTip.Focus();
        }

        public void Hide()
        {
            if (m_ToolTip != null)
                m_ToolTip.Visible = false;
        }

        void MouseMessageFilter_MouseDownEvent(MouseButtons button, Point e)
        {
            if (m_ToolTip.Visible &&
                !m_ToolTip.DataTree.Tree.ContextMenuStrip.Visible &&
                !m_ToolTip.DataTree.Viewer.Visible)
            {
                Hide();
            }
        }

        void MouseMessageFilter_KeyDownEvent(object sender, EventArgs e)
        {
            if (m_ToolTip.Visible &&
                !m_ToolTip.DataTree.Tree.ContextMenuStrip.Visible &&
                !m_ToolTip.DataTree.Viewer.Visible)
            {
                Hide();
            }
        }

        void Manager_OnMouseHover(ScintillaControl sci, int position)
        {
            if (m_ToolTip is null)
                Initialize();

            var debugManager = PluginMain.debugManager;
            var flashInterface = debugManager.FlashInterface;
            if (PluginBase.MainForm.EditorMenu.Visible
                || flashInterface is null
                || !flashInterface.isDebuggerStarted
                || !flashInterface.isDebuggerSuspended) return;
            var sourceFile = debugManager.CurrentLocation?.getFile();
            if (sourceFile != null)
            {
                var localPath = debugManager.GetLocalPath(sourceFile);
                if (localPath is null || localPath != PluginBase.MainForm.CurrentDocument.FileName)
                {
                    return;
                }
            }
            else return;
            var dataTipPoint = Control.MousePosition;
            if (m_ToolTip.Visible && new Rectangle(m_ToolTip.Location, m_ToolTip.Size).Contains(dataTipPoint)) return;
            position = sci.WordEndPosition(position, true);
            var leftword = GetWordAtPosition(sci, position);
            if (leftword.Length != 0)
            {
                try
                {
                    IASTBuilder b = new ASTBuilder(false);
                    var exp = b.parse(new StringReader(leftword));
                    var ctx = new ExpressionContext(flashInterface.Session, flashInterface.GetFrames()[debugManager.CurrentFrame]);
                    var obj = exp.evaluate(ctx);
                    if (obj is Variable variable) Show(dataTipPoint, variable, leftword);
                }
                catch {}
            }
        }

        static string GetWordAtPosition(ScintillaControl sci, int position)
        {
            var insideBrackets = 0;
            var sb = new StringBuilder();
            for (int startPosition = position - 1; startPosition >= 0; startPosition--)
            {
                var c = (char)sci.CharAt(startPosition);
                if (c == ')')
                {
                    insideBrackets++;
                }
                else if (c == '(' && insideBrackets > 0)
                {
                    insideBrackets--;
                }
                else if (!(char.IsLetterOrDigit(c) || c == '_' || c == '$' || c == '.') && insideBrackets == 0)
                {
                    break;
                }
                sb.Insert(0, c);
            }
            return sb.ToString();
        }
    }

    public delegate void MouseDownEventHandler(MouseButtons button, Point e);

    public class MouseMessageFilter : IMessageFilter
    {
        public event MouseDownEventHandler MouseDownEvent;
        public event EventHandler KeyDownEvent;
        Control[] m_ControlList;

        public void AddControls(Control[] controls) => m_ControlList = controls;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == Win32.WM_KEYDOWN && m.WParam.ToInt32() == Win32.VK_ESCAPE)
            {
                KeyDownEvent?.Invoke(null, null);
            }
            if (m.Msg == Win32.WM_LBUTTONDOWN)
            {
                Control target = Control.FromHandle(m.HWnd);
                foreach (Control c in m_ControlList)
                {
                    if (c == target || c.Contains(target))
                    {
                        return false;
                    }
                }
                MouseDownEvent?.Invoke(MouseButtons.Left, new Point(m.LParam.ToInt32()));
            }
            return false;
        }
    }
}