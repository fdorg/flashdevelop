using System;
using System.Windows.Forms;
using PluginCore;
using ScintillaNet;

namespace CodeRefactor.Commands
{
    public class InlineRename : IMessageFilter, IDisposable
    {
        static InlineRename current;

        ScintillaControl sci;
        string oldName, newName;
        int start, end;

        public event Action<string, string> OnRename;
        public event Action<string> OnUpdate;
        public event Action OnCancel;

        public static bool CancelExisting()
        {
            if (current == null) return false;
            current.Cancel();
            return true;
        }

        public static Rename Start()
        {
            if (current == null)
                return new Rename(true);

            return null;
        }

        public InlineRename(ScintillaControl control, string original, int position)
        {
            if (current != null)
                current.Cancel();

            sci = control;
            start = position;
            oldName = original;
            end = start + oldName.Length;

            Application.AddMessageFilter(this);
            sci.TextInserted += Sci_TextInserted;
            sci.TextDeleted += Sci_TextDeleted;
            current = this;

            AddHighlight();
        }

        public void Dispose()
        {
            Application.RemoveMessageFilter(this);
            sci.TextInserted -= Sci_TextInserted;
            sci.TextDeleted -= Sci_TextDeleted;
            current = null;
        }

        void Rename()
        {
            Dispose();

            RemoveHighlight();
            sci.SetSel(start, end);
            newName = sci.SelText;
            sci.ReplaceSel(oldName);

            if (OnRename != null) OnRename(oldName, newName);
        }

        void Cancel()
        {
            Dispose();
            
            RemoveHighlight();
            sci.SetSel(start, end);
            sci.ReplaceSel(oldName);

            if (OnCancel != null) OnCancel();
        }

        void Update(int position)
        {
            AddHighlight();

            if (OnUpdate != null)
            {
                sci.SetSel(start, end);
                string text = sci.SelText;
                sci.SetSel(position, position);
                OnUpdate(text);
            }
        }

        void AddHighlight()
        {
            //sci.AddHighlight(8, 0x00FF00, start, end - start);
            ////if error: sci.AddHighlight(6, 0xFF0000, start, end - start);
        }

        void RemoveHighlight()
        {
            //sci.RemoveHighlight(start, end - start);
        }

        void Sci_TextInserted(ScintillaControl sender, int position, int length, int linesAdded)
        {
            if (WithinRange)
            {
                RemoveHighlight();
                end += length;
                Update(position);
            }
        }

        void Sci_TextDeleted(ScintillaControl sender, int position, int length, int linesAdded)
        {
            if (WithinRange)
            {
                RemoveHighlight();
                end -= length;
                Update(position);
            }
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
