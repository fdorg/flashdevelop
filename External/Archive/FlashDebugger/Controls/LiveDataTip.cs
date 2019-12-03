using System;
using System.Drawing;
using System.Windows.Forms;
using Flash.Tools.Debugger;
using Flash.Tools.Debugger.Expression;
using PluginCore.Controls;
using FlashDebugger.Controls;
using ScintillaNet;
using PluginCore;

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
			Application.AddMessageFilter(m_MouseMessageFilter);
			UITools.Manager.OnMouseHover += new UITools.MouseHoverHandler(Manager_OnMouseHover);
		}

		public void Show(Point point, Variable variable)
		{
			m_ToolTip.Location = point;
			m_ToolTip.SetVariable(variable);
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

		private void Manager_OnMouseHover(ScintillaControl sci, Int32 position)
		{
			DebuggerManager debugManager = PluginMain.debugManager;
			FlashInterface flashInterface = debugManager.FlashInterface;
			if (!PluginBase.MainForm.EditorMenu.Visible && flashInterface != null && flashInterface.isDebuggerStarted && flashInterface.isDebuggerSuspended)
			{
				if (debugManager.CurrentLocation != null && debugManager.CurrentLocation.File != null)
				{
					String localPath = debugManager.GetLocalPath(debugManager.CurrentLocation.File);
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
						ASTBuilder builder = new ASTBuilder(true);
						ValueExp exp = builder.parse(new System.IO.StringReader(leftword));
						ExpressionContext context = new ExpressionContext(flashInterface.Session);
						context.Depth = debugManager.CurrentFrame;
						Object obj = exp.evaluate(context);
						Show(dataTipPoint, (Variable)obj);
					}
					catch (Exception){}
				}
			}
		}

		private String GetWordAtPosition(ScintillaControl sci, Int32 position)
		{
			Char c;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (Int32 startPosition = position - 1; startPosition >= 0; startPosition--)
			{
				c = (Char)sci.CharAt(startPosition);
				if (!(Char.IsLetterOrDigit(c) || c == '_' || c == '$' || c == '.'))
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
		private Control[] m_ControlList;

        public void AddControls(Control[] controls)
        {
            m_ControlList = controls;
        }

        public bool PreFilterMessage(ref Message m)
        {
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
