using System;
using System.Windows.Forms;
using PluginCore;
using ScintillaNet;
using ScintillaNet.Enums;

namespace CodeRefactor.Commands
{
    public class InlineRename : IMessageFilter, IDisposable
    {
        const int indicator = 0;
        static InlineRename current;

        ScintillaControl sci;
        string oldName, newName;
        int start, end;

        public event Action<string, string> OnRename;
        public event Action<string> OnUpdate;
        public event Action OnCancel;

        public static bool InProgress
        {
            get { return current != null; }
        }

        public static bool CancelExisting()
        {
            if (current == null) return false;
            current.Cancel();
            return true;
        }

        public InlineRename(ScintillaControl control, string original, int position)
        {
            if (InProgress)
                current.Cancel();

            sci = control;
            start = position;
            oldName = original;
            end = start + oldName.Length;

            Application.AddMessageFilter(this);
            sci.TextInserted += OnTextInserted;
            sci.TextDeleted += OnTextDeleted;
            current = this;

            sci.RemoveHighlights(indicator);
            sci.SetIndicSetAlpha(indicator, 100);
            Highlight();
        }

        void Rename()
        {
            using (this)
            {
                GetNewName();
                if (OnRename != null) OnRename(oldName, newName);
            }
        }

        void Cancel()
        {
            using (this)
            {
                GetNewName();
                if (OnCancel != null) OnCancel();
            }
        }

        void Update(int position)
        {
            Highlight();

            if (OnUpdate != null)
            {
                sci.SetSel(start, end);
                string text = sci.SelText;
                sci.SetSel(position, position);
                OnUpdate(text);
            }
        }

        void GetNewName()
        {
            sci.RemoveHighlight(start, end);
            sci.SetIndicSetAlpha(indicator, 40);

            sci.SetSel(start, end);
            newName = sci.SelText;

            if (newName == oldName)
                sci.SetSel(end, end);
            else
                sci.ReplaceSel(oldName);
        }

        void Highlight()
        {
            int es = sci.EndStyled;
            int mask = (1 << sci.StyleBits) - 1;
            sci.SetIndicStyle(indicator, (int) IndicatorStyle.Container);
            sci.SetIndicFore(indicator, 0x00FF00);
            sci.CurrentIndicator = indicator;
            sci.IndicatorFillRange(start, end - start);
            sci.StartStyling(es, mask);
        }

        void OnTextInserted(ScintillaControl sender, int position, int length, int linesAdded)
        {
            if (WithinRange)
            {
                end += length;
                Update(position);
            }
        }

        void OnTextDeleted(ScintillaControl sender, int position, int length, int linesAdded)
        {
            if (WithinRange)
            {
                end -= length;
                Update(position);
            }
        }

        void IDisposable.Dispose()
        {
            Application.RemoveMessageFilter(this);
            sci.TextInserted -= OnTextInserted;
            sci.TextDeleted -= OnTextDeleted;
            current = null;
        }

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            if (PluginBase.MainForm.CurrentDocument.SciControl != sci)
            {
                Cancel();
                return false;
            }

            if (m.Msg == Win32.WM_KEYDOWN)
            {
                switch ((int) m.WParam)
                {
                    case 0x1B: //VK_ESCAPE
                        Cancel();
                        return true;
                    case 0x0D: //VK_RETURN
                        Rename();
                        return true;
                    case 0x08: //VK_BACK
                    case 0x25: //VK_LEFT
                        if (!AtLeftmost) break;
                        return true;
                    case 0x2E: //VK_DELETE
                    case 0x27: //VK_RIGHT
                        if (!AtRightmost) break;
                        if (sci.SelTextSize != 0) sci.SetSel(end, end);
                        return true;
                    case 0x21: //VK_PRIOR
                    case 0x22: //VK_NEXY
                    case 0x26: //VK_UP
                    case 0x28: //VK_DOWN
                        if (!WithinRange) break;
                        return true;
                    case 0x23: //VK_END
                        if (!WithinRange) break;
                        sci.SetSel(end, end);
                        return true;
                    case 0x24: //VK_HOME
                        if (!WithinRange) break;
                        sci.SetSel(start, start);
                        return true;
                }
            }

            return false;
        }

        bool AtLeftmost
        {
            get { return sci.CurrentPos == start; }
        }

        bool AtRightmost
        {
            get { return sci.CurrentPos == end; }
        }

        bool WithinRange
        {
            get { return sci.CurrentPos >= start && sci.CurrentPos <= end; }
        }
    }
}
