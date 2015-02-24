using System;
using System.Drawing;
using System.Windows.Forms;

namespace PluginCore.Controls
{
    public interface ICompletionListHost
    {

        event EventHandler LostFocus;
        event EventHandler PositionChanged;
        event KeyEventHandler KeyDown;
        event KeyPressEventHandler KeyPress;
        event MouseEventHandler MouseDown;
        
        Control Owner { get; }
        string SelectedText { get; set; }
        int SelectionEnd { get; set; }
        int SelectionStart { get; set; }
        int CurrentPos { get; }
        bool IsEditable { get; }

        Point GetPositionFromCharIndex(int pos);
        int GetLineHeight();
        void SetSelection(int start, int end);

        void BeginUndoAction();
        void EndUndoAction();

    }
}
