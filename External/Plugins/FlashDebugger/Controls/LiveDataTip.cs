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
    class LiveDataTip
    {
        private DataTipForm m_ToolTip;
        private MouseMessageFilter m_MouseMessageFilter;

        public LiveDataTip()
        {
            m_ToolTip = new DataTipForm();
            m_ToolTip.Dock = DockStyle.Fill;
            m_ToolTip.Visible = false;
            m_MouseMessageFilter = new MouseMessageFilter();
            m_MouseMessageFilter.AddControls(new Control[] { m_ToolTip, m_ToolTip.DataTree });
            m_MouseMessageFilter.MouseDownEvent += new MouseDownEventHandler(MouseMessageFilter_MouseDownEvent);
            m_MouseMessageFilter.KeyDownEvent += new EventHandler(MouseMessageFilter_KeyDownEvent);
            Application.AddMessageFilter(m_MouseMessageFilter);
            UITools.Manager.OnMouseHover += new UITools.MouseHoverHandler(Manager_OnMouseHover);
        }

        public void Show(Point point, Variable variable, String path)
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
            m_ToolTip.Visible = false;
        }

        private void MouseMessageFilter_MouseDownEvent(MouseButtons button, Point e)
        {
            if (m_ToolTip.Visible &&
                !m_ToolTip.DataTree.Tree.ContextMenuStrip.Visible &&
                !m_ToolTip.DataTree.Viewer.Visible)
            {
                Hide();
            }
        }

        private void MouseMessageFilter_KeyDownEvent(object sender, EventArgs e)
        {
            if (m_ToolTip.Visible &&
                !m_ToolTip.DataTree.Tree.ContextMenuStrip.Visible &&
                !m_ToolTip.DataTree.Viewer.Visible)
            {
                Hide();
            }
        }

        private void Manager_OnMouseHover(ScintillaControl sci, Int32 position)
        {
            DebuggerManager debugManager = PluginMain.debugManager;
            FlashInterface flashInterface = debugManager.FlashInterface;
            if (!PluginBase.MainForm.EditorMenu.Visible && flashInterface != null && flashInterface.isDebuggerStarted && flashInterface.isDebuggerSuspended)
            {
                if (debugManager.CurrentLocation != null && debugManager.CurrentLocation.getFile() != null)
                {
                    String localPath = debugManager.GetLocalPath(debugManager.CurrentLocation.getFile());
                    if (localPath == null || localPath != PluginBase.MainForm.CurrentDocument.FileName)
                    {
                        return;
                    }
                }
                else return;
                Point dataTipPoint = Control.MousePosition;
                Rectangle rect = new Rectangle(m_ToolTip.Location, m_ToolTip.Size);
                if (m_ToolTip.Visible && rect.Contains(dataTipPoint))
                {
                    return;
                }
                position = sci.WordEndPosition(position, true);
                String leftword = GetWordAtPosition(sci, position);
                if (leftword != String.Empty)
                {
                    try
                    {
                        IASTBuilder b = new ASTBuilder(false);
                        ValueExp exp = b.parse(new StringReader(leftword));
                        var ctx = new ExpressionContext(flashInterface.Session, flashInterface.GetFrames()[debugManager.CurrentFrame]);
                        var obj = exp.evaluate(ctx);
                        if (obj as Variable != null)
                        {
                            Show(dataTipPoint, (Variable)obj, leftword);
                        }
                    }
                    catch (Exception){}
                }
            }
        }

        private String GetWordAtPosition(ScintillaControl sci, Int32 position)
        {
            int insideBrackets = 0;
            Char c;
            StringBuilder sb = new StringBuilder();
            for (Int32 startPosition = position - 1; startPosition >= 0; startPosition--)
            {
                c = (Char)sci.CharAt(startPosition);
                if (c == ')')
                {
                    insideBrackets++;
                }
                else if (c == '(' && insideBrackets > 0)
                {
                    insideBrackets--;
                }
                else if (!(Char.IsLetterOrDigit(c) || c == '_' || c == '$' || c == '.') && insideBrackets == 0)
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
        public event MouseDownEventHandler MouseDownEvent = null;
        public event EventHandler KeyDownEvent = null;
        private Control[] m_ControlList;

        public void AddControls(Control[] controls)
        {
            m_ControlList = controls;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == Win32.WM_KEYDOWN && m.WParam.ToInt32() == Win32.VK_ESCAPE)
            {
                if (KeyDownEvent != null)
                {
                    KeyDownEvent(null, null);
                }
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
                if (MouseDownEvent != null)
                {
                    MouseDownEvent(MouseButtons.Left, new Point(m.LParam.ToInt32()));
                }
            }
            return false;
        }

    }

}
