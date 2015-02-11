using System;
using System.Drawing;
using System.Windows.Forms;

namespace PluginCore.Controls
{
    public interface ICompletionListTarget
    {

        event EventHandler LostFocus;
        event ScrollEventHandler Scroll;
        event KeyEventHandler KeyDown;
        event MouseEventHandler MouseDown;

        Control Owner { get; }
        string Text { get; }
        string SelectedText { get; set; }
        int SelectionEnd { get; set; }
        int SelectionStart { get; set; }
        int CurrentPos { get; }
        bool IsEditable { get; }

        Point GetPositionFromCharIndex(int pos);
        int GetLineHeight();
        void SetSelection(int start, int end);

    }
}
